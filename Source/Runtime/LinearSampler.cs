using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using glTFLoader.Schema;

namespace AssetGenerator.Runtime
{
    internal struct LinearSampler<T>: IAnimationSampler
    {
        public List<float> InputKeys { get; set; }

        public List<T> OutputKeys { get; set; }

        int IAnimationSampler.GetOutputKeyCount()
        {
            return OutputKeys.Count;
        }

        public Accessor.TypeEnum GetOutputAccessorType()
        {
            return (typeof(T) == typeof(Vector3)) ? Accessor.TypeEnum.VEC3 : Accessor.TypeEnum.VEC4;
        }

        public AnimationSamplerComponentTypeEnum OutputAccessorComponentType { get; set; }

        public AnimationSampler.InterpolationEnum GetInterpolation()
        {
            return glTFLoader.Schema.AnimationSampler.InterpolationEnum.LINEAR;
        }

        public void WriteOutputData(Data geometryData)
        {
            // Write Output Data
            switch (OutputAccessorComponentType)
            {
                case AnimationSamplerComponentTypeEnum.FLOAT:
                    if (typeof(T) == typeof(Vector3))
                    {
                        foreach (var value in OutputKeys)
                        {
                            var component = (Vector3)(object)value;
                            geometryData.Writer.Write(component.X);
                            geometryData.Writer.Write(component.Y);
                            geometryData.Writer.Write(component.Z);
                        }
                    }
                    else if (typeof(T) == typeof(Vector4))
                    {
                        foreach (var value in OutputKeys)
                        {
                            var component = (Vector4)(object)value;
                            geometryData.Writer.Write(component.X);
                            geometryData.Writer.Write(component.Y);
                            geometryData.Writer.Write(component.Z);
                            geometryData.Writer.Write(component.W);
                        }
                    }
                    else if (typeof(T) == typeof(Quaternion))
                    {
                        foreach (var value in OutputKeys)
                        {
                            var component = (Quaternion)(object)value;
                            geometryData.Writer.Write(component.X);
                            geometryData.Writer.Write(component.Y);
                            geometryData.Writer.Write(component.Z);
                            geometryData.Writer.Write(component.W);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"Type {typeof(T)} not supported!");
                    }
                    break;

                default:
                    {
                        throw new NotImplementedException($"AnimationSamplerComponentType {OutputAccessorComponentType} not implemented!");
                    }
            }
        }
    }
}
