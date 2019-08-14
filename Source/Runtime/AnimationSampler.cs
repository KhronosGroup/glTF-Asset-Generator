using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class AnimationSampler
    {
        public Accessor InputKeys { get; }
        public Accessor OutputKeys { get; }
        public InterpolationEnum Interpolation { get; }

        public enum InterpolationEnum { LINEAR, STEP, CUBIC_SPLINE }

        public struct Key<T>
        {
            public T InTangent;
            public T Value;
            public T OutTangent;

            public Key(T inTangent, T value, T outTangent)
            {
                InTangent = inTangent;
                Value = value;
                OutTangent = outTangent;
            }
        }

        /// <summary>
        /// Uses default values for a linear Vector3 sampler animation. 
        /// </summary>
        public AnimationSampler(IEnumerable<float> inputKeys, IEnumerable<Vector3> outputKeys)
        {
            InputKeys = new Accessor(inputKeys, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.SCALAR);
            OutputKeys = new Accessor(outputKeys, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.VEC3);
            Interpolation = InterpolationEnum.LINEAR;
        }

        /// <summary>
        /// Uses default values for a linear Quaternion sampler animation. 
        /// </summary>
        public AnimationSampler(IEnumerable<float> inputKeys, IEnumerable<Quaternion> outputKeys)
        {
            InputKeys = new Accessor(inputKeys, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.SCALAR);
            OutputKeys = new Accessor(outputKeys, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.VEC4);
            Interpolation = InterpolationEnum.LINEAR;
        }

        /// <summary>
        /// Set all values for a sampler. Will allow invalid combinations.
        /// </summary>
        public AnimationSampler(IEnumerable<float> inputKeys, IEnumerable outputKeys, Accessor.ComponentTypeEnum outputComponentType, Accessor.TypeEnum outputType, InterpolationEnum interpolation)
        {
            InputKeys = new Accessor(inputKeys, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.SCALAR);
            OutputKeys = new Accessor(outputKeys, outputComponentType, outputType);
            Interpolation = interpolation;

            // Add validation! Key only with Cublic spline, etc...
        }
    }
}