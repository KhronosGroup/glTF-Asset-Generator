using System;
using System.Collections.Generic;
using System.Numerics;
using static AssetGenerator.Runtime.MeshPrimitive;
using static glTFLoader.Schema.Material;

namespace AssetGenerator
{
    internal class Material_AlphaBlend : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_AlphaBlend;

        public Material_AlphaBlend(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");

            // Track the common properties for use in the readme.
            var alphaModeValue = AlphaModeEnum.BLEND;
            CommonProperties.Add(new Property(PropertyName.AlphaMode, alphaModeValue.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

                // Apply the common properties to the gltf.
                meshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        MetallicFactor = 0
                    },
                    AlphaMode = alphaModeValue,
                };

                // Apply the properties that are specific to this gltf.
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
                                    MeshPrimitives = new[]
                                    {
                                        meshPrimitive
                                    }
                                },
                            },
                        },
                    }),
                };
            }

            void SetNoMetallicRoughness(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.MetallicRoughnessMaterial = null;
            }

            void SetBaseColorFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                var baseColorFactorValue = new Vector4(1.0f, 1.0f, 1.0f, 0.7f);
                metallicRoughness.BaseColorFactor = baseColorFactorValue;
                properties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue.ToReadmeString()));
            }

            void SetBaseColorTexture(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };
                properties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));
            }

            void SetVertexColor(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var vertexColors = new[]
                {
                    new Vector4(0.3f, 0.3f, 0.3f, 0.4f),
                    new Vector4(0.3f, 0.3f, 0.3f, 0.2f),
                    new Vector4(0.3f, 0.3f, 0.3f, 0.8f),
                    new Vector4(0.3f, 0.3f, 0.3f, 0.6f),
                };
                meshPrimitive.ColorComponentType = ColorComponentTypeEnum.FLOAT;
                meshPrimitive.ColorType = ColorTypeEnum.VEC4;
                meshPrimitive.Colors = vertexColors;

                properties.Add(new Property(PropertyName.VertexColor, "Vector4 Float"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetVertexColor(properties, meshPrimitive);
                    SetNoMetallicRoughness(properties, meshPrimitive);
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
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties,meshPrimitive, metallicRoughness) =>
                {
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorFactor(properties, metallicRoughness);
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
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
