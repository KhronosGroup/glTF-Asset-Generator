using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime.Extensions
{
    internal class PbrSpecularGlossiness : Extension
    {
        /// <summary>
        /// The name of the extension
        /// </summary>
        public override string Name
        {
            get
            {
                return "KHR_materials_pbrSpecularGlossiness";
            }
        }

        /// <summary>
        /// The reflected diffuse factor of the material
        /// </summary>
        public Vector4? DiffuseFactor { get; set; }
        /// <summary>
        /// The diffuse texture
        /// </summary>
        public Texture DiffuseTexture { get; set; }
        /// <summary>
        /// The specular RGB color of the material
        /// </summary>
        public Vector3? SpecularFactor { get; set; }
        /// <summary>
        /// The glossiness or smoothness of the material
        /// </summary>
        public float? GlossinessFactor { get; set; }
        /// <summary>
        /// The specular-glossiness texture
        /// </summary>
        public Texture SpecularGlossinessTexture { get; set; }

    }
}
