using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using static glTFLoader.Schema.Accessor;
using Schema = glTFLoader.Schema;

namespace AssetGenerator.Conversion
{
    internal static class Extensions
    {
        public static bool Align(this BinaryWriter writer, int value)
        {
            var mod = writer.BaseStream.Position % value;
            if (mod == 0)
            {
                return false;
            }

            writer.Write(new byte[value - mod]);
            return true;
        }

        public static int GetSize(this ComponentTypeEnum componentType)
        {
            switch (componentType)
            {
                case ComponentTypeEnum.BYTE:
                case ComponentTypeEnum.UNSIGNED_BYTE:
                    return 1;
                case ComponentTypeEnum.SHORT:
                case ComponentTypeEnum.UNSIGNED_SHORT:
                    return 2;
                case ComponentTypeEnum.UNSIGNED_INT:
                case ComponentTypeEnum.FLOAT:
                    return 4;
                default:
                    throw new ArgumentException();
            }
        }
    }

    internal static class DataConverter
    {
        public struct Element
        {
            public Action Write;
        }

        public class Info
        {
            public IEnumerable<Element> Values;

            public Action<Schema.Accessor> SetCommon;
            public Action<Schema.Accessor> SetMinMax;

            public IEnumerable<Element> SparseIndices;
            public IEnumerable<Element> SparseValues;
        }

        public static void IntAccessorCommon(DataType outputType, BinaryWriter binaryWriter, out ComponentTypeEnum componentType, out Action<int> writeComponent)
        {
            switch (outputType)
            {
                case DataType.UnsignedInt:
                    writeComponent = value => binaryWriter.Write((uint)value);
                    componentType = ComponentTypeEnum.UNSIGNED_INT;
                    break;
                case DataType.UnsignedShort:
                    writeComponent = value => binaryWriter.Write((ushort)value);
                    componentType = ComponentTypeEnum.UNSIGNED_SHORT;
                    break;
                case DataType.UnsignedByte:
                    writeComponent = value => binaryWriter.Write((byte)value);
                    componentType = ComponentTypeEnum.UNSIGNED_BYTE;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid data output type {outputType}");
            }
        }

        public static void FloatAccessorCommon(DataType outputType, BinaryWriter binaryWriter, out ComponentTypeEnum componentType, out bool normalized, out Action<float> writeComponent)
        {
            switch (outputType)
            {
                case DataType.Float:
                    writeComponent = value => binaryWriter.Write(value);
                    componentType = ComponentTypeEnum.FLOAT;
                    normalized = false;
                    break;
                case DataType.NormalizedShort:
                    writeComponent = value => binaryWriter.Write(System.Convert.ToInt16(Math.Round(value * short.MaxValue)));
                    componentType = ComponentTypeEnum.SHORT;
                    normalized = true;
                    break;
                case DataType.NormalizedByte:
                    writeComponent = value => binaryWriter.Write(System.Convert.ToSByte(Math.Round(value * sbyte.MaxValue)));
                    componentType = ComponentTypeEnum.BYTE;
                    normalized = true;
                    break;
                case DataType.NormalizedUnsignedShort:
                    writeComponent = value => binaryWriter.Write(System.Convert.ToUInt16(Math.Round(value * ushort.MaxValue)));
                    componentType = ComponentTypeEnum.UNSIGNED_SHORT;
                    normalized = true;
                    break;
                case DataType.NormalizedUnsignedByte:
                    writeComponent = value => binaryWriter.Write(System.Convert.ToByte(Math.Round(value * byte.MaxValue)));
                    componentType = ComponentTypeEnum.UNSIGNED_BYTE;
                    normalized = true;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid accessor output type {outputType}");
            }
        }

        private static void GetValuesInfo<T>(Data<T> runtimeData, Info info, Action<T> writeValue)
        {
            if (runtimeData.Sparse != null && runtimeData.Values.All(value => value.Equals(default(T))))
            {
                return;
            }

            info.Values = runtimeData.Values.Select(value => new Element
            {
                Write = () => writeValue(value),
            });
        }

        private static void GetSparseInfo<T>(DataSparse<T> runtimeDataSparse, Info info, BinaryWriter binaryWriter, Action<T> writeValue)
        {
            if (runtimeDataSparse == null)
            {
                return;
            }

            IntAccessorCommon(runtimeDataSparse.IndicesOutputType, binaryWriter, out var indicesComponentType, out var writeIndex);

            var setCommon = info.SetCommon;
            info.SetCommon = accessor =>
            {
                setCommon(accessor);

                accessor.Sparse = new Schema.AccessorSparse
                {
                    Count = runtimeDataSparse.Map.Count(),
                    Indices = new Schema.AccessorSparseIndices
                    {
                        ComponentType = (Schema.AccessorSparseIndices.ComponentTypeEnum)indicesComponentType,
                    },
                    Values = new Schema.AccessorSparseValues(),
                };
            };

            info.SparseIndices = runtimeDataSparse.Map.Keys.Select(index => new Element
            {
                Write = () => writeIndex(index),
            });

            info.SparseValues = runtimeDataSparse.Map.Values.Select(value => new Element
            {
                Write = () => writeValue(value),
            });
        }

        private static IEnumerable<T> GetFinalValues<T>(Data<T> runtimeData)
        {
            if (runtimeData.Sparse == null)
            {
                return runtimeData.Values;
            }

            return runtimeData.Values.Select((value, index) =>
                runtimeData.Sparse.Map.TryGetValue(index, out T sparseValue) ? sparseValue : value);
        }

        public static void GetInfo(Data<int> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            IntAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var writeValue);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.SCALAR;
                accessor.ComponentType = componentType;
                accessor.Count = runtimeData.Values.Count();
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        public static void GetInfo(Data<float> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeValue);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.SCALAR;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);

            info.SetMinMax = accessor =>
            {
                var finalValues = GetFinalValues(runtimeData);
                accessor.Min = new[] { finalValues.Min() };
                accessor.Max = new[] { finalValues.Max() };
            };
        }

        private static void GetInfo(Data<Vector2> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC2;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<Vector2> writeValue = (value) =>
            {
                writeComponent(value.X);
                writeComponent(value.Y);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        private static void GetInfo(Data<Vector3> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC3;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<Vector3> writeValue = (value) =>
            {
                writeComponent(value.X);
                writeComponent(value.Y);
                writeComponent(value.Z);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);

            info.SetMinMax = accessor =>
            {
                var finalValues = GetFinalValues(runtimeData);
                accessor.Min = new[]
                {
                    finalValues.Select(value => value.X).Min(),
                    finalValues.Select(value => value.Y).Min(),
                    finalValues.Select(value => value.Z).Min(),
                };
                accessor.Max = new[]
                {
                    finalValues.Select(value => value.X).Max(),
                    finalValues.Select(value => value.Y).Max(),
                    finalValues.Select(value => value.Z).Max(),
                };
            };
        }

        private static void GetInfo(Data<Vector4> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<Vector4> writeValue = (value) =>
            {
                writeComponent(value.X);
                writeComponent(value.Y);
                writeComponent(value.Z);
                writeComponent(value.W);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        private static void GetInfo(Data<Quaternion> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<Quaternion> writeValue = (value) =>
            {
                writeComponent(value.X);
                writeComponent(value.Y);
                writeComponent(value.Z);
                writeComponent(value.W);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        private static void GetInfo(Data<Matrix4x4> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.MAT4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<Matrix4x4> writeValue = (value) =>
            {
                writeComponent(value.M11);
                writeComponent(value.M12);
                writeComponent(value.M13);
                writeComponent(value.M14);
                writeComponent(value.M21);
                writeComponent(value.M22);
                writeComponent(value.M23);
                writeComponent(value.M24);
                writeComponent(value.M31);
                writeComponent(value.M32);
                writeComponent(value.M33);
                writeComponent(value.M34);
                writeComponent(value.M41);
                writeComponent(value.M42);
                writeComponent(value.M43);
                writeComponent(value.M44);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        private static void GetInfo(Data<JointVector> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            IntAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<JointVector> writeValue = (value) =>
            {
                writeComponent(value.Index0);
                writeComponent(value.Index1);
                writeComponent(value.Index2);
                writeComponent(value.Index3);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        private static void GetInfo(Data<WeightVector> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeComponent);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            Action<WeightVector> writeValue = (value) =>
            {
                writeComponent(value.Value0);
                writeComponent(value.Value1);
                writeComponent(value.Value2);
                writeComponent(value.Value3);
            };

            GetValuesInfo(runtimeData, info, writeValue);
            GetSparseInfo(runtimeData.Sparse, info, binaryWriter, writeValue);
        }

        public static Info GetInfo(Data runtimeData, BinaryWriter binaryWriter)
        {
            var info = new Info();

            if (runtimeData is Data<int>)
            {
                GetInfo((Data<int>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<float>)
            {
                GetInfo((Data<float>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<Vector2>)
            {
                GetInfo((Data<Vector2>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<Vector3>)
            {
                GetInfo((Data<Vector3>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<Vector4>)
            {
                GetInfo((Data<Vector4>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<Quaternion>)
            {
                GetInfo((Data<Quaternion>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<Matrix4x4>)
            {
                GetInfo((Data<Matrix4x4>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<JointVector>)
            {
                GetInfo((Data<JointVector>)runtimeData, info, binaryWriter);
            }
            else if (runtimeData is Data<WeightVector>)
            {
                GetInfo((Data<WeightVector>)runtimeData, info, binaryWriter);
            }
            else
            {
                throw new InvalidOperationException($"Invalid data type {runtimeData.GetType()}");
            }

            return info;
        }
    }
}
