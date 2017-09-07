using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Texture
    /// </summary>
    public class Texture
    {
        /// <summary>
        /// Image source for the texture
        /// </summary>
        public Runtime.Image source;
        /// <summary>
        /// Texture coordinate index used for this texture
        /// </summary>
        public int? texCoordIndex;
        /// <summary>
        /// Sampler for this texture.
        /// </summary>
        public Runtime.Sampler sampler;

        /// <summary>
        /// User defined name
        /// </summary>
        public string name;

    }
}
