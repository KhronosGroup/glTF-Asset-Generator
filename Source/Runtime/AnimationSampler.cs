using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class AnimationSampler
    {
        public IEnumerable<float> InputKeys { get; protected set; }
        public enum ComponentTypeEnum { FLOAT, NORMALIZED_BYTE, NORMALIZED_UNSIGNED_BYTE, NORMALIZED_SHORT, NORMALIZED_UNSIGNED_SHORT };
        public ComponentTypeEnum OutputComponentType { get; protected set; }
    }

    internal class StepAnimationSampler<T> : AnimationSampler
    {
        public IEnumerable<T> OutputKeys { get; }

        public StepAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            OutputComponentType = outputComponentType;
        }
    }

    internal class LinearAnimationSampler<T> : AnimationSampler
    {
        public IEnumerable<T> OutputKeys { get; }

        public LinearAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            OutputComponentType = outputComponentType;
        }
    }

    internal class CubicSplineAnimationSampler<T> : AnimationSampler
    {
        public struct Key
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

        public IEnumerable<Key> OutputKeys { get; }

        public CubicSplineAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<Key> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            OutputComponentType = outputComponentType;
        }
    }
}
