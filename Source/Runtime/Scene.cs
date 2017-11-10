using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Scene
    /// </summary>
    internal class Scene
    {
        /// <summary>
        /// List of nodes in the scene
        /// </summary>
        public List<Runtime.Node> Nodes { get; set; }

        /// <summary>
        /// The user-defined name of the scene
        /// </summary>
        public string Name { get; set; }

    }
}
