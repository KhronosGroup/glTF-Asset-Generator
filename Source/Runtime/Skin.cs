using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class Skin
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nodes used as joints in the skin.
        /// </summary>
        public IEnumerable<Node> Joints;

        /// <summary>
        /// InverseBindMatrices for each joint in the skin.
        /// </summary>
        public IEnumerable<Matrix4x4> InverseBindMatrices;
    }
}
