using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal struct Animation
    {
        public String Name { get; set; }
        public List<AnimationChannel> AnimationChannels { get; set; }
    }
}
