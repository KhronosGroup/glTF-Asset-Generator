using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper class for abstracting the glTF Loader API
    /// </summary>
    internal class GLTF
    {
        /// <summary>
        /// List of scenes in the gltf wrapper
        /// </summary>
        public List<Runtime.Scene> Scenes { get; set; }
        /// <summary>
        /// index of the main scene
        /// </summary>
        public int? MainScene { get; set; }

        public List<string> ExtensionsUsed { get; set; }
        public List<string> ExtensionsRequired { get; set; }

        /// <summary>
        /// Initializes the gltf wrapper
        /// </summary>
        public GLTF()
        {
            Scenes = new List<Runtime.Scene>();
        }
        /// <summary>
        /// Holds the Asset data
        /// </summary>
        public Asset Asset { get; set; }
    }
}
