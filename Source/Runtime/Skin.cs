using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal class Skin
    {
        /// <summary>
        /// Name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// joints in the skin
        /// </summary>
        public IEnumerable<SkinJoint> SkinJoints = null;
    }
}
