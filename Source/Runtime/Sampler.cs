using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Sampler.  The sampler descibe the wrapping and scaling of textures.
    /// </summary>
    public class Sampler
    {
        /// <summary>
        /// Magnification filter
        /// </summary>
        public glTFLoader.Schema.Sampler.MagFilterEnum? magFilter;
        /// <summary>
        /// Minification filter
        /// </summary>
        public glTFLoader.Schema.Sampler.MinFilterEnum? minFilter;
        /// <summary>
        /// S wrapping mode
        /// </summary>
        public glTFLoader.Schema.Sampler.WrapSEnum? wrapS;
        /// <summary>
        /// T wrapping mode
        /// </summary>
        public glTFLoader.Schema.Sampler.WrapTEnum? wrapT;
        /// <summary>
        /// User-defined name of the sampler
        /// </summary>
        public string name;
        /// <summary>
        /// Converts the GLTFSampler into a glTF loader Sampler object.
        /// </summary>
        /// <returns>Returns a Sampler object</returns>
        public glTFLoader.Schema.Sampler convertToSampler()
        {
            glTFLoader.Schema.Sampler sampler = new glTFLoader.Schema.Sampler();
            if (magFilter.HasValue)
            {
                sampler.MagFilter = magFilter.Value;
            }
            if (minFilter.HasValue)
            {
                sampler.MinFilter = minFilter.Value;
            }
            if (wrapS.HasValue)
            {
                sampler.WrapS = wrapS.Value;
            }
            if (wrapT.HasValue)
            {
                sampler.WrapT = wrapT.Value;
            }
            if (name != null)
            {
                sampler.Name = name;
            }
            return sampler;
        }
        /// <summary>
        /// Determines if two GLTFSamplers have the same property values
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>boolean indicating if the properties are the same (true) or not (false)</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Runtime.Sampler other = obj as Runtime.Sampler;
            if ((System.Object)other == null)
                return false;

            return (magFilter == other.magFilter) && (minFilter == other.minFilter) && (wrapS == other.wrapS) && (wrapT == other.wrapT);
        }
    }
}
