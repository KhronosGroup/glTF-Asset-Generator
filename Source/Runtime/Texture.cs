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
    internal class Texture
    {
        /// <summary>
        /// Image source for the texture
        /// </summary>
        public Runtime.Image Source { get; set; }
        /// <summary>
        /// Texture coordinate index used for this texture
        /// </summary>
        public int? TexCoordIndex { get; set; }
        /// <summary>
        /// Sampler for this texture.
        /// </summary>
        public Runtime.Sampler Sampler { get; set; }

        /// <summary>
        /// User defined name
        /// </summary>
        public string Name { get; set; }
    }
}
