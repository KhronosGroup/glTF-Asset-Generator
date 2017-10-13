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
    internal class Sampler
    {
        /// <summary>
        /// Magnification filter
        /// </summary>
        public glTFLoader.Schema.Sampler.MagFilterEnum? MagFilter { get; set; }
        /// <summary>
        /// Minification filter
        /// </summary>
        public glTFLoader.Schema.Sampler.MinFilterEnum? MinFilter { get; set; }
        /// <summary>
        /// S wrapping mode
        /// </summary>
        public glTFLoader.Schema.Sampler.WrapSEnum? WrapS { get; set; }
        /// <summary>
        /// T wrapping mode
        /// </summary>
        public glTFLoader.Schema.Sampler.WrapTEnum? WrapT { get; set; }
        /// <summary>
        /// User-defined name of the sampler
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Converts the GLTFSampler into a glTF loader Sampler object.
        /// </summary>
        /// <returns>Returns a Sampler object</returns>
        public glTFLoader.Schema.Sampler ConvertToSampler()
        {
            glTFLoader.Schema.Sampler sampler = new glTFLoader.Schema.Sampler();
            if (MagFilter.HasValue)
            {
                sampler.MagFilter = MagFilter.Value;
            }
            if (MinFilter.HasValue)
            {
                sampler.MinFilter = MinFilter.Value;
            }
            if (WrapS.HasValue)
            {
                sampler.WrapS = WrapS.Value;
            }
            if (WrapT.HasValue)
            {
                sampler.WrapT = WrapT.Value;
            }
            if (Name != null)
            {
                sampler.Name = Name;
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

            return (MagFilter == other.MagFilter) && (MinFilter == other.MinFilter) && (WrapS == other.WrapS) && (WrapT == other.WrapT);
        }
        public override int GetHashCode()
        {
            return (MagFilter.GetHashCode() + MinFilter.GetHashCode() + WrapS.GetHashCode() + WrapT.GetHashCode());
        }
    }
}
