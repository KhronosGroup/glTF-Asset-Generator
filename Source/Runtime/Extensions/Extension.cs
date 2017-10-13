using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime.Extensions
{
    internal abstract class Extension
    {
        public abstract string Name { get; }

        public abstract Object ConvertToExtension(Runtime.GLTF gltf, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures);
   }
 }
