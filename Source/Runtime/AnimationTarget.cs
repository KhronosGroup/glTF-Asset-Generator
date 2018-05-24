using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal struct AnimationTarget
    {
        public Node Node { get; set; }

        public enum PathEnum { TRANSLATION, ROTATION, SCALE, WEIGHT}

        public PathEnum Path { get; set; }
    }
}
