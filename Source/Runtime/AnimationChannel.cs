using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal struct AnimationChannel
    {
        public IAnimationSampler Sampler { get; set; }

        public AnimationTarget AnimationTarget { get; set; }
    }
}
