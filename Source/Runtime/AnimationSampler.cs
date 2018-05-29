﻿using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal abstract class AnimationSampler
    {
        public abstract IEnumerable<float> InputKeys { get; }
    }

    internal class StepSampler<T> : AnimationSampler
    {
        public override IEnumerable<float> InputKeys { get; }

        public List<T> OutputKeys { get; }

        public StepSampler(List<float> inputKeys, List<T> outputKeys)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
        }
    }

    internal class LinearAnimationSampler<T> : AnimationSampler
    {
        public override IEnumerable<float> InputKeys { get; }

        public List<T> OutputKeys { get; }

        public LinearAnimationSampler(List<float> inputKeys, List<T> outputKeys)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
        }
    }

    internal class CubicSplineSampler<T> : AnimationSampler
    {
        public struct Key
        {
            public T InTangent;
            public T Value;
            public T OutTangent;

            public Key(T inTangent, T value, T outTangent)
            {
                this.InTangent = inTangent;
                this.Value = value;
                this.OutTangent = outTangent;
            }
        }

        public override IEnumerable<float> InputKeys { get; }

        public List<Key> OutputKeys { get; }

        public CubicSplineSampler(List<float> inputKeys, List<Key> outputKeys)
        {
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
        }
    }
}
