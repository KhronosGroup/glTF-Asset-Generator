using System.Collections.Generic;
using System.Numerics;
using System;

namespace AssetGenerator
{
    partial class ModelGroup
    {
        void GenerateGroup_Material(List<string> imageList)
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
                new Property("Metallic Factor", "0.0", new Action<Runtime.GLTF>((wrapper) => { wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.MetallicRoughnessMaterial.MetallicFactor = 0.0f; })),
                new Property("Base Color Factor", ReadmeStringHelper.ConvertTestValueToString(baseColorFactor), new Action<Runtime.GLTF>((wrapper) => { wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorFactor = baseColorFactor; })),
            };

            // Declares the properties and their values. Order doesn't matter here.
            Property normalTexture = new Property("Normal Texture", ReadmeStringHelper.ConvertTestValueToString(normalImage), new Action<Runtime.GLTF>((wrapper) =>
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.NormalTexture = new Runtime.Texture();
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.NormalTexture.Source = normalImage;
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = planeNormals;
            }));
            Property scale = new Property("Scale", "10.0", new Action<Runtime.GLTF>((wrapper) => { wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.NormalScale = 10.0f; }));
            Property occlusionTexture = new Property("Occlusion Texture", ReadmeStringHelper.ConvertTestValueToString(occlusionImage), new Action<Runtime.GLTF>((wrapper) =>
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.OcclusionTexture = new Runtime.Texture();
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.OcclusionTexture.Source = occlusionImage;
            }));
            Property strength = new Property("Strength", "0.5", new Action<Runtime.GLTF>((wrapper) => { wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.OcclusionStrength = 0.5f; }));
            Property emissiveTexture = new Property("Emissive Texture", ReadmeStringHelper.ConvertTestValueToString(emissiveImage), new Action<Runtime.GLTF>((wrapper) =>
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.EmissiveTexture = new Runtime.Texture();
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.EmissiveTexture.Source = emissiveImage;
            }));
            Property emissiveFactor = new Property("Emissive Factor", ReadmeStringHelper.ConvertTestValueToString(emissiveFactorValue), new Action<Runtime.GLTF>((wrapper) => { wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.EmissiveFactor = emissiveFactorValue; }));

            // Declares the list of properties. The order here determins the column order.
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
                normalTexture
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
                scale,
                occlusionTexture,
                strength,
                emissiveTexture,
                emissiveFactor
            });
        }
    }
}
