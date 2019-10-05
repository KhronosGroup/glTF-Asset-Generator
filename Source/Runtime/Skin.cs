using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class Skin
    {
        public string Name { get; set; }
        public IEnumerable<Node> Joints;
        public Data<Matrix4x4> InverseBindMatrices;
    }
}
