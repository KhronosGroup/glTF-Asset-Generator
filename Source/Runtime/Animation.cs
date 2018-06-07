using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal class Animation
    {
        public String Name { get; set; }
        public List<AnimationChannel> Channels { get; set; }
    }
}
