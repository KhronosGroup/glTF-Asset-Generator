using System.Collections.Generic;
using System.Numerics;
using System;

namespace AssetGenerator
{
    internal class Material : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Material;

        public Material(List<string> imageList)
        {
            var emissiveImage = GetImage(imageList, "Emissive_Plane");
            var normalImage = GetImage(imageList, "Normal_Plane");
            var occlusionImage = GetImage(imageList, "Occlusion_Plane");

            //this.CommonProperties = new List<Property>
            //{
                // Put common properties here
            //};


            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.Material> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                SetMetallicFactor(CommonProperties, meshPrimitive.Material.MetallicRoughnessMaterial);
                SetBaseColorFactor(CommonProperties, meshPrimitive.Material.MetallicRoughnessMaterial);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene()
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

            void SetMetallicFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.MetallicFactor = 0;
                properties.Add(new Property(PropertyName.MetallicFactor, "Metallic Factor", metallicRoughness.MetallicFactor));
            }

            void SetBaseColorFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.BaseColorFactor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                properties.Add(new Property(PropertyName.BaseColorFactor, "Base Color Factor", metallicRoughness.BaseColorFactor));
            }

            void SetNormalTexture(List<Property> properties, Runtime.Material material)
            {
                material.NormalTexture = new Runtime.Texture { Source = normalImage };
                properties.Add(new Property(PropertyName.NormalTexture, "Normal Texture", normalImage));
            }

            void SetNormals(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeNormalsValue = new List<Vector3>()
                {
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f)
                };
                meshPrimitive.Normals = planeNormalsValue;
                properties.Add(new Property(PropertyName.Normals, "Normals", planeNormalsValue));
            }

            void SetNormalScale(List<Property> properties, Runtime.Material material)
            {
                material.NormalScale = 10;
                properties.Add(new Property(PropertyName.NormalScale, "Normal Scale", material.NormalScale));
            }

            void SetOcclusionTexture(List<Property> properties, Runtime.Material material)
            {
                material.OcclusionTexture = new Runtime.Texture { Source = occlusionImage };
                properties.Add(new Property(PropertyName.OcclusionTexture, "Occlusion Texture", occlusionImage));
            }

            void SetOcclusionStrength(List<Property> properties, Runtime.Material material)
            {
                material.OcclusionStrength = 0.5f;
                properties.Add(new Property(PropertyName.OcclusionTextureStrength, "Occlusion Strength", material.OcclusionStrength));
            }

            void SetEmissiveTexture(List<Property> properties, Runtime.Material material)
            {
                material.EmissiveTexture = new Runtime.Texture { Source = emissiveImage };
                properties.Add(new Property(PropertyName.EmissiveTexture, "Emissive Texture", emissiveImage));
            }

            void SetEmissiveFactor(List<Property> properties, Runtime.Material material)
            {
                var emissiveFactorValue = new Vector3(1.0f, 1.0f, 1.0f);
                material.EmissiveFactor = emissiveFactorValue;
                properties.Add(new Property(PropertyName.EmissiveFactor, "Emissive Factor", emissiveFactorValue));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, material) => {

                }),
                CreateModel((properties,meshPrimitive, material) => {
                    SetNormalTexture(properties, material);
                    SetNormals(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    SetOcclusionTexture(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    SetEmissiveFactor(properties, material);
                }),
                CreateModel((properties,meshPrimitive, material) => {
                    SetNormalTexture(properties, material);
                    SetNormals(properties, meshPrimitive);
                    SetNormalScale(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    SetOcclusionTexture(properties, material);
                    SetOcclusionStrength(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    SetEmissiveTexture(properties, material);
                    SetEmissiveFactor(properties, material);
                }),
                CreateModel((properties, meshPrimitive, material) => {
                    SetNormalTexture(properties, material);
                    SetNormals(properties, meshPrimitive);
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
