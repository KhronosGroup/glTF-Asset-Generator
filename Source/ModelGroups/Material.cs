using System.Collections.Generic;
using System.Numerics;
using System;

namespace AssetGenerator
{
    internal class Material : initializeModelGroup
    {
        public Material(List<string> imageList)
        {
            modelGroupName = ModelGroupName.Material;

            Runtime.Image emissiveImage = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Emissive_Plane"))
            };
            Runtime.Image normalImage = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Normal_Plane"))
            };
            Runtime.Image occlusionImage = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Occlusion_Plane"))
            };
            usedTextures.Add(emissiveImage);
            usedTextures.Add(normalImage);
            usedTextures.Add(occlusionImage);

            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f)
            };
            var baseColorFactor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            var emissiveFactorValue = new Vector3(1.0f, 1.0f, 1.0f);

            requiredProperty = new List<Property>
            {
                new Property("Metallic Factor", "0.0", new Action<Runtime.GLTF>((wrapper) => ModelGroup.MetallicFactor(wrapper, new ValueIndexPositions(), 0.0f))),
                new Property("Base Color Factor", ReadmeStringHelper.ConvertValueToString(baseColorFactor), new Action<Runtime.GLTF>((wrapper) => ModelGroup.BaseColorFactor(wrapper, new ValueIndexPositions(), baseColorFactor))),
            };

            // Declares the properties and their values. Order doesn't matter here.
            //Set normals with normal texture!
            Property normalTexture = new Property("Normal Texture", ReadmeStringHelper.ConvertValueToString(normalImage), new Action<Runtime.GLTF>((wrapper) => { ModelGroup.NormalTexture(wrapper, new ValueIndexPositions(), normalImage); } ));
            Property normals = new Property("Normals", ReadmeStringHelper.ConvertValueToString(planeNormals), new Action<Runtime.GLTF>((wrapper) => { ModelGroup.Normals(wrapper, new ValueIndexPositions(), planeNormals); } ));
            Property scale = new Property("Scale", "10.0", new Action<Runtime.GLTF>((wrapper) => { ModelGroup.Scale(wrapper, new ValueIndexPositions(), 10.0f); }));
            Property occlusionTexture = new Property("Occlusion Texture", ReadmeStringHelper.ConvertValueToString(occlusionImage), new Action<Runtime.GLTF>((wrapper) => { ModelGroup.OcclusionTexture(wrapper, new ValueIndexPositions(), occlusionImage); }));
            Property strength = new Property("Strength", "0.5", new Action<Runtime.GLTF>((wrapper) => { ModelGroup.Strength(wrapper, new ValueIndexPositions(), 0.5f); }));
            Property emissiveTexture = new Property("Emissive Texture", ReadmeStringHelper.ConvertValueToString(emissiveImage), new Action<Runtime.GLTF>((wrapper) => { ModelGroup.EmissiveTexture(wrapper, new ValueIndexPositions(), emissiveImage); }));
            Property emissiveFactor = new Property("Emissive Factor", ReadmeStringHelper.ConvertValueToString(emissiveFactorValue), new Action<Runtime.GLTF>((wrapper) => { ModelGroup.EmissiveFactor(wrapper, new ValueIndexPositions(), emissiveFactorValue); }));

            // Declares the list of properties. The order here determins the column order. Leave items off of this list to have them not show up in the readme.
            properties = new List<Property>
            {
                normalTexture,
                scale,
                occlusionTexture,
                strength,
                emissiveTexture,
                emissiveFactor
            };

            // Declares the combos. The order here determines the number for the model (assending order) and the order that the properties are applied to a model.
            combos.Add(new List<Property>()
            {

            });
            combos.Add(new List<Property>()
            {
                normalTexture,
                normals
            });
            combos.Add(new List<Property>()
            {
                occlusionTexture
            });
            combos.Add(new List<Property>()
            {
                emissiveFactor
            });
            combos.Add(new List<Property>()
            {
                normalTexture,
                normals,
                scale
            });
            combos.Add(new List<Property>()
            {
                occlusionTexture,
                strength
            });
            combos.Add(new List<Property>()
            {
                emissiveTexture,
                emissiveFactor
            });
            combos.Add(new List<Property>()
            {
                normalTexture,
                normals,
                scale,
                occlusionTexture,
                strength,
                emissiveTexture,
                emissiveFactor
            });

            // Adds the required properties to all of the models.
            foreach (var combo in combos)
            {
                foreach (var property in requiredProperty)
                {
                    combo.Add(property);
                }
            }
        }
    }
}
