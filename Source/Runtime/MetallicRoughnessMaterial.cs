using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// GLTF Wrapper for glTF loader's MetallicRoughness
    /// </summary>
    public class MetallicRoughnessMaterial
    {
        /// <summary>
        /// The main texture that will be applied to the object.
        /// </summary>
        public AssetGenerator.Runtime.Texture baseColorTexture;
        /// <summary>
        /// The scaling factors for the red, green, blue and alpha components of the color.
        /// </summary>
        public Vector4? baseColorFactor;
        /// <summary>
        /// Texture containing the metalness value in the "blue" color channel, and the roughness value in the "green" color channel.
        /// </summary>
        public AssetGenerator.Runtime.Texture metallicRoughnessTexture;
        /// <summary>
        /// Scaling factor for the metalness component
        /// </summary>
        public float? metallicFactor;
        /// <summary>
        /// Scaling factor for the roughness component
        /// </summary>
        public float? roughnessFactor;
    }
}
