﻿using System;
using System.Numerics;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Material_AlphaMask : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Material_AlphaMask;

        public Material_AlphaMask(List<string> imageList)
        {
            var baseColorTextureImage = GetImage(imageList, "BaseColor_Plane");

            // Track the common properties for use in the readme.
            var alphaModeValue = glTFLoader.Schema.Material.AlphaModeEnum.MASK;
            CommonProperties.Add(new Property(PropertyName.AlphaMode, alphaModeValue));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.Material, Runtime.PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.AlphaMode = alphaModeValue;
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive.Material, meshPrimitive.Material.MetallicRoughnessMaterial);

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

            void SetAlphaCutoff_Low(List<Property> properties, Runtime.Material material)
            {
                material.AlphaCutoff = 0.4f;
                properties.Add(new Property(PropertyName.AlphaCutoff, material.AlphaCutoff, group : 1));
            }

            void SetAlphaCutoff_High(List<Property> properties, Runtime.Material material)
            {
                material.AlphaCutoff = 0.7f;
                properties.Add(new Property(PropertyName.AlphaCutoff, material.AlphaCutoff, group: 1));
            }

            void SetAlphaCutoff_Multiplied(List<Property> properties, Runtime.Material material)
            {
                material.AlphaCutoff = 0.6f;
                properties.Add(new Property(PropertyName.AlphaCutoff, material.AlphaCutoff, group: 1));
            }

            void SetAlphaCutoff_All(List<Property> properties, Runtime.Material material)
            {
                material.AlphaCutoff = 1.1f;
                properties.Add(new Property(PropertyName.AlphaCutoff, material.AlphaCutoff, group: 1));
            }

            void SetAlphaCutoff_None(List<Property> properties, Runtime.Material material)
            {
                material.AlphaCutoff = 0.0f;
                properties.Add(new Property(PropertyName.AlphaCutoff, material.AlphaCutoff, group: 1));
            }

            void SetBaseColorFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                var baseColorFactorValue = new Vector4(1.0f, 1.0f, 1.0f, 0.7f);
                metallicRoughness.BaseColorFactor = baseColorFactorValue;
                properties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, material, metallicRoughness) => {

                }),
                CreateModel((properties, material, metallicRoughness) => {
                    SetAlphaCutoff_Low(properties, material);
                }),
                CreateModel((properties, material, metallicRoughness) => {
                    SetAlphaCutoff_High(properties, material);
                }),
                CreateModel((properties, material, metallicRoughness) => {
                    SetAlphaCutoff_All(properties, material);
                }),
                CreateModel((properties, material, metallicRoughness) => {
                    SetAlphaCutoff_None(properties, material);
                }),
                CreateModel((properties, material, metallicRoughness) => {
                    SetAlphaCutoff_Low(properties, material);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, material, metallicRoughness) => {
                    SetAlphaCutoff_Multiplied(properties, material);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}