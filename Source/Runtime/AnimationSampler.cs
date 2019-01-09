using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal abstract class AnimationSampler
    {
        public abstract IEnumerable<float> InputKeys { get; }
    }

    internal class StepAnimationSampler<T> : AnimationSampler
    {
        public override IEnumerable<float> InputKeys { get; }

        public IEnumerable<T> OutputKeys { get; }

        public StepAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
        }
    }

    internal class LinearAnimationSampler<T> : AnimationSampler
    {
        public override IEnumerable<float> InputKeys { get; }

        public IEnumerable<T> OutputKeys { get; }

        public LinearAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<T> outputKeys)
        {
            InputKeys = inputKeys;
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

        public override IEnumerable<float> InputKeys { get; }

        public IEnumerable<Key> OutputKeys { get; }

        public CubicSplineAnimationSampler(IEnumerable<float> inputKeys, IEnumerable<Key> outputKeys)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
        }
    }
}
