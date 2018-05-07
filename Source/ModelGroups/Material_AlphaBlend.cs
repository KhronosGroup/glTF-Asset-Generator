using System;
using System.Numerics;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Material_AlphaBlend : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Material_AlphaBlend;

        public Material_AlphaBlend(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");

            // Track the common properties for use in the readme.
            var alphaModeValue = glTFLoader.Schema.Material.AlphaModeEnum.BLEND;
            CommonProperties.Add(new Property(PropertyName.AlphaMode, alphaModeValue));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.AlphaMode = alphaModeValue;

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material.MetallicRoughnessMaterial);

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

            void SetNoMetallicRoughness(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.MetallicRoughnessMaterial = null;
            }

            void SetBaseColorFactor(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                var baseColorFactorValue = new Vector4(1.0f, 1.0f, 1.0f, 0.7f);
                metallicRoughness.BaseColorFactor = baseColorFactorValue;
                properties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue));
            }

            void SetBaseColorTexture(List<Property> properties, Runtime.PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };
                properties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));
            }

            void SetVertexColor(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                List<Vector4> vertexColors = new List<Vector4>()
                {
                    new Vector4( 0.3f, 0.3f, 0.3f, 0.4f),
                    new Vector4( 0.3f, 0.3f, 0.3f, 0.2f),
                    new Vector4( 0.3f, 0.3f, 0.3f, 0.8f),
                    new Vector4( 0.3f, 0.3f, 0.3f, 0.6f)
                };
                meshPrimitive.ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                meshPrimitive.ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                meshPrimitive.Colors = vertexColors;

                var vertexColorsValue = new VertexColor(meshPrimitive.ColorComponentType, meshPrimitive.ColorType, meshPrimitive.Colors);
                properties.Add(new Property(PropertyName.VertexColor, vertexColorsValue));
            }


            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, metallicRoughness) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetNoMetallicRoughness(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) => {
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) => {
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties,meshPrimitive, metallicRoughness) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) => {
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
