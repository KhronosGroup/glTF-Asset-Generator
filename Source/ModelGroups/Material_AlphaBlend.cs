using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Material_AlphaBlend : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_AlphaBlend;

        public Material_AlphaBlend(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Plane") };

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.AlphaMode, MaterialAlphaMode.Blend.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

                // Apply the common properties to the gltf.
                meshPrimitive.Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        MetallicFactor = 0
                    },
                    AlphaMode = MaterialAlphaMode.Blend,
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material.PbrMetallicRoughness);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = new[]
                        {
                            new Node
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
                meshPrimitive.Material.PbrMetallicRoughness = null;
            }

            void SetBaseColorFactor(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                var baseColorFactorValue = new Vector4(1.0f, 1.0f, 1.0f, 0.7f);
                metallicRoughness.BaseColorFactor = baseColorFactorValue;
                properties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue.ToReadmeString()));
            }

            void SetBaseColorTexture(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.BaseColorTexture = new TextureInfo { Texture = baseColorTexture };
                properties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));
            }

            void SetVertexColor(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors = Data.Create
                (
                    new[]
                    {
                        new Vector4(0.3f, 0.3f, 0.3f, 0.4f),
                        new Vector4(0.3f, 0.3f, 0.3f, 0.2f),
                        new Vector4(0.3f, 0.3f, 0.3f, 0.8f),
                        new Vector4(0.3f, 0.3f, 0.3f, 0.6f),
                    }
                );

                properties.Add(new Property(PropertyName.VertexColor, $"Vector4 Float"));
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
                    meshPrimitive.TexCoords0 = Data.Create(MeshPrimitive.GetSinglePlaneTexCoords());
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TexCoords0 = Data.Create(MeshPrimitive.GetSinglePlaneTexCoords());
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
                    meshPrimitive.TexCoords0 = Data.Create(MeshPrimitive.GetSinglePlaneTexCoords());
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TexCoords0 = Data.Create(MeshPrimitive.GetSinglePlaneTexCoords());
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
