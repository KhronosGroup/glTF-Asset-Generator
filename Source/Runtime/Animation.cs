using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class Animation
    {
        public string Name { get; set; }
        public IEnumerable<AnimationChannel> Channels { get; set; }
    }
}
