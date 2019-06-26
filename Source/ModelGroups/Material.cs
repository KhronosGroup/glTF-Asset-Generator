using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Material : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material;

        public Material(List<string> imageList)
        {
            Runtime.Image emissiveImage = UseTexture(imageList, "Emissive_Plane");
            Runtime.Image normalImage = UseTexture(imageList, "Normal_Plane");
            Runtime.Image occlusionImage = UseTexture(imageList, "Occlusion_Plane");

            // Track the common properties for use in the readme.
            var metallicFactorValue = 0.0f;
            var baseColorFactorValue = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            CommonProperties.Add(new Property(PropertyName.MetallicFactor, metallicFactorValue));
            CommonProperties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.Material> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

                // Apply the common properties to the gltf.
                meshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        MetallicFactor = metallicFactorValue,
                        BaseColorFactor = baseColorFactorValue,
                    }
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene
                    {
                        Nodes = new List<Runtime.Node>
                        {
                            new Runtime.Node
                            {
                                Mesh = new Runtime.Mesh
                                {
                                    MeshPrimitives = new List<Runtime.MeshPrimitive>
                                    {
                                        meshPrimitive
                                    }
                                },
                            },
                        },
                    }),
                };
            }

            void SetNormalTexture(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Normals = MeshPrimitive.GetSinglePlaneNormals();
                meshPrimitive.Material.NormalTexture = new Runtime.Texture { Source = normalImage };
                properties.Add(new Property(PropertyName.NormalTexture, normalImage));
            }

            void SetNormalScale(List<Property> properties, Runtime.Material material)
            {
                material.NormalScale = 10.0f;
                properties.Add(new Property(PropertyName.NormalTextureScale, material.NormalScale));
            }

            void SetOcclusionTexture(List<Property> properties, Runtime.Material material)
            {
                material.OcclusionTexture = new Runtime.Texture { Source = occlusionImage };
                properties.Add(new Property(PropertyName.OcclusionTexture, occlusionImage));
            }

            void SetOcclusionStrength(List<Property> properties, Runtime.Material material)
            {
                material.OcclusionStrength = 0.5f;
                properties.Add(new Property(PropertyName.OcclusionTextureStrength, material.OcclusionStrength));
            }

            void SetEmissiveTexture(List<Property> properties, Runtime.Material material)
            {
                material.EmissiveTexture = new Runtime.Texture { Source = emissiveImage };
                properties.Add(new Property(PropertyName.EmissiveTexture, emissiveImage));
            }

            void SetEmissiveFactor(List<Property> properties, Runtime.Material material)
            {
                var emissiveFactorValue = new Vector3(1.0f, 1.0f, 1.0f);
                material.EmissiveFactor = emissiveFactorValue;
                properties.Add(new Property(PropertyName.EmissiveFactor, emissiveFactorValue));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, material) => {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetOcclusionTexture(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    SetEmissiveFactor(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetNormalTexture(properties, meshPrimitive);
                    SetNormalScale(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetOcclusionTexture(properties, material);
                    SetOcclusionStrength(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetEmissiveTexture(properties, material);
                    SetEmissiveFactor(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetNormalTexture(properties, meshPrimitive);
                    SetNormalScale(properties, material);
                    SetOcclusionTexture(properties, material);
                    SetOcclusionStrength(properties, material);
                    SetEmissiveTexture(properties, material);
                    SetEmissiveFactor(properties, material);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
