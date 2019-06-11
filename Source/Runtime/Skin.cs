using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class Skin
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Joints in the skin.
        /// </summary>
        public IEnumerable<SkinJoint> SkinJoints = null;

        /// <summary>
        /// Toggles if the inverseBindMatrices will be checked for instantiation.
        /// </summary>
        public bool InverseBindMatrixInstanced = false;
    }
}
