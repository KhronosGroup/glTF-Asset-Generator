using System.Collections;
using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal abstract class AnimationSampler
    {
        public IEnumerable<float> InputKeys { get; }
        public IEnumerable OutputKeys { get; }
        public enum ComponentTypeEnum { FLOAT, NORMALIZED_BYTE, NORMALIZED_UNSIGNED_BYTE, NORMALIZED_SHORT, NORMALIZED_UNSIGNED_SHORT };
        public ComponentTypeEnum OutputComponentType { get; }

        public AnimationSampler(IEnumerable<float> inputKeys, IEnumerable outputKeys, ComponentTypeEnum outputComponentType)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            OutputComponentType = outputComponentType;
        }
    }

    internal class StepAnimationSampler<T> : AnimationSampler
    {
        public new IEnumerable<T> OutputKeys { get; }

        public StepAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
            : base(inputKeys, outputKeys, outputComponentType)
        {
            OutputKeys = outputKeys;
        }
    }

    internal class LinearAnimationSampler<T> : AnimationSampler
    {
        public new IEnumerable<T> OutputKeys { get; }

        public LinearAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
            : base(inputKeys, outputKeys, outputComponentType)
        {
            OutputKeys = outputKeys;
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

        public new IEnumerable<Key> OutputKeys { get; }

        public CubicSplineAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<Key> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
            : base(inputKeys, outputKeys, outputComponentType)
        {
            OutputKeys = outputKeys;
        }
    }
}