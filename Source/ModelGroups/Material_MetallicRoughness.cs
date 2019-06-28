﻿using System;
using System.Collections.Generic;
using System.Numerics;
using static AssetGenerator.Runtime.MeshPrimitive;

namespace AssetGenerator
{
    internal class Material_MetallicRoughness : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_MetallicRoughness;

        public Material_MetallicRoughness(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            Runtime.Image metallicRoughnessTextureImage = UseTexture(imageList, "MetallicRoughness_Plane");

            // There are no common properties in this model group.

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

                // Apply the properties that are specific to this gltf.
                meshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                };

                setProperties(properties, meshPrimitive, meshPrimitive.Material.MetallicRoughnessMaterial);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene
                    {
                        Nodes = new[]
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

            void SetVertexColor(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var vertexColors = new[]
                {
                    new Vector4(0.0f, 0.0f, 1.0f, 0.8f),
                    new Vector4(1.0f, 0.0f, 0.0f, 0.8f),
                    new Vector4(0.0f, 0.0f, 1.0f, 0.8f),
                    new Vector4(1.0f, 0.0f, 0.0f, 0.8f),
                };
                meshPrimitive.ColorComponentType = ColorComponentTypeEnum.FLOAT;
                meshPrimitive.ColorType = ColorTypeEnum.VEC3;
                meshPrimitive.Colors = vertexColors;

                properties.Add(new Property(PropertyName.VertexColor, "Vector3 Float"));
            }

            void SetBaseColorTexture(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };
                properties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));
            }

            void SetBaseColorFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                var baseColorFactorValue = new Vector4(0.2f, 0.2f, 0.2f, 0.8f);
                metallicRoughness.BaseColorFactor = baseColorFactorValue;
                properties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue));
            }

            void SetMetallicRoughnessTexture(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.MetallicRoughnessTexture = new Runtime.Texture { Source = metallicRoughnessTextureImage };
                properties.Add(new Property(PropertyName.MetallicRoughnessTexture, metallicRoughnessTextureImage));
            }

            void SetMetallicFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.MetallicFactor = 0.0f;
                properties.Add(new Property(PropertyName.MetallicFactor, metallicRoughness.MetallicFactor));
            }

            void SetRoughnessFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.RoughnessFactor = 0.0f;
                properties.Add(new Property(PropertyName.RoughnessFactor, metallicRoughness.RoughnessFactor));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetVertexColor(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetMetallicRoughnessTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetMetallicFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetRoughnessFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetMetallicRoughnessTexture(properties, metallicRoughness);
                    SetMetallicFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetMetallicRoughnessTexture(properties, metallicRoughness);
                    SetRoughnessFactor(properties, metallicRoughness);
                }),
                CreateModel((properties,meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                    SetMetallicRoughnessTexture(properties, metallicRoughness);
                    SetMetallicFactor(properties, metallicRoughness);
                    SetRoughnessFactor(properties, metallicRoughness);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
