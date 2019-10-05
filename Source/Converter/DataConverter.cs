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
        public class Info
        {
            public struct Element
            {
                public Action Write;
            }

            public IEnumerable<Element> Elements;

            public Action<Schema.Accessor> SetCommon;
            public Action<Schema.Accessor> SetMinMax;
        }

        public static void IntAccessorCommon(DataType outputType, BinaryWriter binaryWriter, out ComponentTypeEnum componentType, out Action<int> writeAction)
        {
            switch (outputType)
            {
                case DataType.Default:
                case DataType.UnsignedInt:
                    writeAction = value => binaryWriter.Write((uint)value);
                    componentType = ComponentTypeEnum.UNSIGNED_INT;
                    break;
                case DataType.UnsignedShort:
                    writeAction = value => binaryWriter.Write((ushort)value);
                    componentType = ComponentTypeEnum.UNSIGNED_SHORT;
                    break;
                case DataType.UnsignedByte:
                    writeAction = value => binaryWriter.Write((byte)value);
                    componentType = ComponentTypeEnum.UNSIGNED_BYTE;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid data output type {outputType}");
            }
        }

        public static void FloatAccessorCommon(DataType outputType, BinaryWriter binaryWriter, out ComponentTypeEnum componentType, out bool normalized, out Action<float> writeAction)
        {
            switch (outputType)
            {
                case DataType.Default:
                case DataType.Float:
                    writeAction = value => binaryWriter.Write(value);
                    componentType = ComponentTypeEnum.FLOAT;
                    normalized = false;
                    break;
                case DataType.NormalizedShort:
                    writeAction = value => binaryWriter.Write(System.Convert.ToInt16(Math.Round(value * short.MaxValue)));
                    componentType = ComponentTypeEnum.SHORT;
                    normalized = true;
                    break;
                case DataType.NormalizedByte:
                    writeAction = value => binaryWriter.Write(System.Convert.ToSByte(Math.Round(value * sbyte.MaxValue)));
                    componentType = ComponentTypeEnum.BYTE;
                    normalized = true;
                    break;
                case DataType.NormalizedUnsignedShort:
                    writeAction = value => binaryWriter.Write(System.Convert.ToUInt16(Math.Round(value * ushort.MaxValue)));
                    componentType = ComponentTypeEnum.UNSIGNED_SHORT;
                    normalized = true;
                    break;
                case DataType.NormalizedUnsignedByte:
                    writeAction = value => binaryWriter.Write(System.Convert.ToByte(Math.Round(value * byte.MaxValue)));
                    componentType = ComponentTypeEnum.UNSIGNED_BYTE;
                    normalized = true;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid accessor output type {outputType}");
            }
        }

        public static void GetInfo(Data<int> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            IntAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.SCALAR;
                accessor.ComponentType = componentType;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () => writeAction(value),
            });
        }

        public static void GetInfo(Data<float> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.SCALAR;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.SetMinMax = accessor =>
            {
                accessor.Min = new[] { runtimeData.Values.Min() };
                accessor.Max = new[] { runtimeData.Values.Max() };
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () => writeAction(value),
            });
        }

        private static void GetInfo(Data<Vector2> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC2;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.X);
                    writeAction(value.Y);
                },
            });
        }

        private static void GetInfo(Data<Vector3> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC3;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.SetMinMax = accessor =>
            {
                accessor.Min = new[]
                {
                    runtimeData.Values.Select(value => value.X).Min(),
                    runtimeData.Values.Select(value => value.Y).Min(),
                    runtimeData.Values.Select(value => value.Z).Min(),
                };
                accessor.Max = new[]
                {
                    runtimeData.Values.Select(value => value.X).Max(),
                    runtimeData.Values.Select(value => value.Y).Max(),
                    runtimeData.Values.Select(value => value.Z).Max(),
                };
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.X);
                    writeAction(value.Y);
                    writeAction(value.Z);
                },
            });
        }

        private static void GetInfo(Data<Vector4> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.X);
                    writeAction(value.Y);
                    writeAction(value.Z);
                    writeAction(value.W);
                },
            });
        }

        private static void GetInfo(Data<Quaternion> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.X);
                    writeAction(value.Y);
                    writeAction(value.Z);
                    writeAction(value.W);
                },
            });
        }

        private static void GetInfo(Data<Matrix4x4> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.MAT4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.M11);
                    writeAction(value.M12);
                    writeAction(value.M13);
                    writeAction(value.M14);
                    writeAction(value.M21);
                    writeAction(value.M22);
                    writeAction(value.M23);
                    writeAction(value.M24);
                    writeAction(value.M31);
                    writeAction(value.M32);
                    writeAction(value.M33);
                    writeAction(value.M34);
                    writeAction(value.M41);
                    writeAction(value.M42);
                    writeAction(value.M43);
                    writeAction(value.M44);
                },
            });
        }

        private static void GetInfo(Data<JointVector> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            IntAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.Index0);
                    writeAction(value.Index1);
                    writeAction(value.Index2);
                    writeAction(value.Index3);
                },
            });
        }

        private static void GetInfo(Data<WeightVector> runtimeData, Info info, BinaryWriter binaryWriter)
        {
            FloatAccessorCommon(runtimeData.OutputType, binaryWriter, out var componentType, out var normalized, out var writeAction);

            info.SetCommon = accessor =>
            {
                accessor.Type = TypeEnum.VEC4;
                accessor.ComponentType = componentType;
                accessor.Normalized = normalized;
                accessor.Count = runtimeData.Values.Count();
            };

            info.Elements = runtimeData.Values.Select(value => new Info.Element
            {
                Write = () =>
                {
                    writeAction(value.Value0);
                    writeAction(value.Value1);
                    writeAction(value.Value2);
                    writeAction(value.Value3);
                },
            });
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
