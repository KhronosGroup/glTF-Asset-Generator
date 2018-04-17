using System.Collections.Generic;
using System.Numerics;
using System;

namespace AssetGenerator
{
    internal class Material : ModelGroup
    {
        Runtime.Material material;
        Runtime.MeshPrimitive meshPrimitive;
        Runtime.PbrMetallicRoughness metallicRoughness;

        public Material(List<string> imageList)
        {
            modelGroupName = ModelGroupName.Material;
            returnModelToBaseState();

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
                new Property(
                    "Metallic Factor", 
                    "0.0", 
                    (() => MetallicFactor(metallicRoughness, 0.0f))),
                new Property(
                    "Base Color Factor", 
                    baseColorFactor, 
                    (() => BaseColorFactor(metallicRoughness, baseColorFactor))),
            };

            // Declares the properties and their values. Order doesn't matter here.
            var normalTexture = new Property(
                "Normal Texture", 
                normalImage, 
                (() =>  NormalTexture(material, normalImage)));
            var scale = new Property(
                "Scale", 
                "10.0", 
                (() => Scale(material, 10.0f)));
            var occlusionTexture = new Property(
                "Occlusion Texture", 
                occlusionImage, 
                (() => OcclusionTexture(material, occlusionImage)));
            var strength = new Property(
                "Strength", 
                "0.5", 
                (() => Strength(material, 0.5f)));
            var emissiveTexture = new Property(
                "Emissive Texture", 
                emissiveImage, 
                (() => EmissiveTexture(material, emissiveImage)));
            var emissiveFactor = new Property(
                "Emissive Factor", 
                emissiveFactorValue, 
                (() => EmissiveFactor(material, emissiveFactorValue)));

            // Used, but not in the readme
            var normals = new Property(
                "Normals", 
                planeNormals, 
                (() => Normals(meshPrimitive, planeNormals)));

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

        void returnModelToBaseState()
        {
            material = new Runtime.Material();
            meshPrimitive = Common.SinglePlane().Scenes[0].Nodes[0].Mesh.MeshPrimitives[0];
            metallicRoughness = new Runtime.PbrMetallicRoughness();
        }

        public override Runtime.GLTF SetModelAttributes(List<Property> combo)
        {
            returnModelToBaseState();
            Runtime.GLTF gltf = Common.SinglePlane();

            foreach (var property in combo)
            {
                property.value(); 
            }
            gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0] = meshPrimitive;
            gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
            gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material.MetallicRoughnessMaterial = metallicRoughness;

            return gltf;
        }

        internal Runtime.PbrMetallicRoughness MetallicFactor(Runtime.PbrMetallicRoughness metallicRoughness, float value)
        {
            metallicRoughness.MetallicFactor = value;
            return metallicRoughness;
        }
        internal Runtime.PbrMetallicRoughness BaseColorFactor(Runtime.PbrMetallicRoughness metallicRoughness, Vector4 value)
        {
            metallicRoughness.BaseColorFactor = value;
            return metallicRoughness;
        }
        internal Runtime.Material NormalTexture(Runtime.Material material, Runtime.Image value)
        {
            material.NormalTexture = new Runtime.Texture
            {
                Source = value
            };
            return material;
        }
        internal Runtime.MeshPrimitive Normals(Runtime.MeshPrimitive meshPrimitive, List<Vector3> value)
        {
            meshPrimitive.Normals = value;
            return meshPrimitive;
        }
        internal Runtime.Material Scale(Runtime.Material material, float value)
        {
            material.NormalScale = value;
            return material;
        }
        internal Runtime.Material OcclusionTexture(Runtime.Material material, Runtime.Image value)
        {
            material.OcclusionTexture = new Runtime.Texture();
            material.OcclusionTexture.Source = value;
            return material;
        }
        internal Runtime.Material Strength(Runtime.Material material, float value)
        {
            material.OcclusionStrength = value;
            return material;
        }
        internal Runtime.Material EmissiveTexture(Runtime.Material material, Runtime.Image value)
        {
            material.EmissiveTexture = new Runtime.Texture();
            material.EmissiveTexture.Source = value;
            return material;
        }
        internal Runtime.Material EmissiveFactor(Runtime.Material material, Vector3 value)
        {
            material.EmissiveFactor = value;
            return material;
        }
    }
}
