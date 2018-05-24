using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal struct StepSampler<T> : IAnimationSampler
    {
        /// <summary>
        /// The Time key frames.
        /// </summary>
        public List<float> InputKeys { get; set; }

        public List<T> OutputKeys { get; set; }

        public AnimationSamplerComponentTypeEnum OutputAccessorComponentType { get; set; }

        public glTFLoader.Schema.AnimationSampler.InterpolationEnum GetInterpolation()
        {
            return glTFLoader.Schema.AnimationSampler.InterpolationEnum.STEP;
        }

        public glTFLoader.Schema.Accessor.TypeEnum GetOutputAccessorType()
        {
            return (typeof(T) == typeof(Vector3)) ? glTFLoader.Schema.Accessor.TypeEnum.VEC3 : glTFLoader.Schema.Accessor.TypeEnum.VEC4;
        }

        public int GetOutputKeyCount()
        {
            return OutputKeys.Count;
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
