using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using glTFLoader.Schema;

namespace AssetGenerator.Runtime
{
    internal struct CubicSplineSampler<T> : IAnimationSampler
    {
        public struct CubicSplineKey
        {
            public T InTangent;
            public T Value;
            public T OutTangent;
        }
        public AnimationSamplerComponentTypeEnum OutputAccessorComponentType { get; set; }

        public List<float> InputKeys { get; set; }

        public List<CubicSplineKey> OutputKeys;

        public AnimationSampler.InterpolationEnum GetInterpolation()
        {
            return glTFLoader.Schema.AnimationSampler.InterpolationEnum.CUBICSPLINE;
        }

        public int GetOutputKeyCount()
        {
            return OutputKeys.Count * 3;
        }

        public Accessor.TypeEnum GetOutputAccessorType()
        {
            return (typeof(T) == typeof(Vector3)) ? Accessor.TypeEnum.VEC3 : Accessor.TypeEnum.VEC4;
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
                            var keyFrame = (CubicSplineKey)(object)value;
                            var inTangent = (Vector3)(object)keyFrame.InTangent;
                            geometryData.Writer.Write(inTangent.X);
                            geometryData.Writer.Write(inTangent.Y);
                            geometryData.Writer.Write(inTangent.Z);

                            var keyFrameValue = (Vector3)(object)keyFrame.Value;
                            geometryData.Writer.Write(keyFrameValue.X);
                            geometryData.Writer.Write(keyFrameValue.Y);
                            geometryData.Writer.Write(keyFrameValue.Z);

                            var outTangent = (Vector3)(object)keyFrame.OutTangent;
                            geometryData.Writer.Write(outTangent.X);
                            geometryData.Writer.Write(outTangent.Y);
                            geometryData.Writer.Write(outTangent.Z);
                        }
                    }
                    else if (typeof(T) == typeof(Vector4))
                    {
                        foreach (var value in OutputKeys)
                        {                        
                            var keyFrame = (CubicSplineKey)(object)value;
                            var inTangent = (Vector4)(object)keyFrame.InTangent;
                            geometryData.Writer.Write(inTangent.X);
                            geometryData.Writer.Write(inTangent.Y);
                            geometryData.Writer.Write(inTangent.Z);
                            geometryData.Writer.Write(inTangent.W);

                            var keyFrameValue = (Vector4)(object)keyFrame.Value;
                            geometryData.Writer.Write(keyFrameValue.X);
                            geometryData.Writer.Write(keyFrameValue.Y);
                            geometryData.Writer.Write(keyFrameValue.Z);
                            geometryData.Writer.Write(keyFrameValue.W);

                            var outTangent = (Vector4)(object)keyFrame.OutTangent;
                            geometryData.Writer.Write(outTangent.X);
                            geometryData.Writer.Write(outTangent.Y);
                            geometryData.Writer.Write(outTangent.Z);
                            geometryData.Writer.Write(outTangent.W);
                        }
                    }
                    else if (typeof(T) == typeof(Quaternion))
                    {
                        foreach (var value in OutputKeys)
                        {
                            var keyFrame = (CubicSplineKey)(object)value;
                            var inTangent = (Quaternion)(object)keyFrame.InTangent;
                            geometryData.Writer.Write(inTangent.X);
                            geometryData.Writer.Write(inTangent.Y);
                            geometryData.Writer.Write(inTangent.Z);
                            geometryData.Writer.Write(inTangent.W);

                            var keyFrameValue = (Quaternion)(object)keyFrame.Value;
                            geometryData.Writer.Write(keyFrameValue.X);
                            geometryData.Writer.Write(keyFrameValue.Y);
                            geometryData.Writer.Write(keyFrameValue.Z);
                            geometryData.Writer.Write(keyFrameValue.W);

                            var outTangent = (Quaternion)(object)keyFrame.OutTangent;
                            geometryData.Writer.Write(outTangent.X);
                            geometryData.Writer.Write(outTangent.Y);
                            geometryData.Writer.Write(outTangent.Z);
                            geometryData.Writer.Write(outTangent.W);
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
