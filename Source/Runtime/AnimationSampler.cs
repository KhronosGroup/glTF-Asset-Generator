using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal abstract class AnimationSampler
    {
        public IEnumerable<float> InputKeys { get; protected set; }
        public abstract object OutputKeys { get; protected set; }
        public enum ComponentTypeEnum { FLOAT, NORMALIZED_BYTE, NORMALIZED_UNSIGNED_BYTE, NORMALIZED_SHORT, NORMALIZED_UNSIGNED_SHORT };
        public ComponentTypeEnum OutputComponentType { get; protected set; }
    }

    internal class StepAnimationSampler<T> : AnimationSampler
    {
        private IEnumerable<T> OutputKeysValue;
        public override object OutputKeys
        {
            get
            {
                return OutputKeysValue;
            }
            protected set
            {
                if (value is IEnumerable<T>)
                {
                    OutputKeysValue = (IEnumerable<T>)value;
                }
            }
        }

        public StepAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            OutputComponentType = outputComponentType;
        }
    }

    internal class LinearAnimationSampler<T> : AnimationSampler
    {
        private IEnumerable<T> OutputKeysValue;
        public override object OutputKeys
        {
            get
            {
                return OutputKeysValue;
            }
            protected set
            {
                if (value is IEnumerable<T>)
                {
                    OutputKeysValue = (IEnumerable<T>)value;
                }
            }
        }

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

        private IEnumerable<Key> OutputKeysValue;
        public override object OutputKeys
        {
            get
            {
                return OutputKeysValue;
            }
            protected set
            {
                if (value is IEnumerable<Key>)
                {
                    OutputKeysValue = (IEnumerable<Key>)value;
                }
            }
        }

        public CubicSplineAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<Key> outputKeys, ComponentTypeEnum outputComponentType = ComponentTypeEnum.FLOAT)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            OutputComponentType = outputComponentType;
        }
    }
}
