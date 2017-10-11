using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    public class KHRMaterialSpecularGlossinessExtension: glTFLoader.Schema.Extension
    {
        public float[] DiffuseFactor { get; set; }
        public glTFLoader.Schema.TextureInfo DiffuseTexture { get; set; }
        public float[] SpecularFactor { get; set; }
        public float? GlossinessFactor { get; set; }
        public glTFLoader.Schema.TextureInfo SpecularGlossinessTexture { get; set; }
    }
}
