using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime.Extensions
{
    internal abstract class Extension
    {
        /// <summary>
        /// The name of the extension
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Converts the extension to schema
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <returns></returns>
        public abstract Object ConvertToSchema(Runtime.GLTF gltf, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures);
   }
 }
