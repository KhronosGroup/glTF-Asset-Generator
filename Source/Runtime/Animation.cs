using System;
using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class Animation
    {
        public String Name { get; set; }
        public IEnumerable<AnimationChannel> Channels { get; set; }
    }
}
