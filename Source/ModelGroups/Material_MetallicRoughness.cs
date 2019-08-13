using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Material_MetallicRoughness : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_MetallicRoughness;

        public Material_MetallicRoughness(List<string> imageList)
        {
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            Image metallicRoughnessTextureImage = UseTexture(imageList, "MetallicRoughness_Plane");

            // There are no common properties in this model group.

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, PbrMetallicRoughness> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

                // Apply the properties that are specific to this gltf.
                meshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new PbrMetallicRoughness()
                };

                setProperties(properties, meshPrimitive, meshPrimitive.Material.MetallicRoughnessMaterial);

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
                meshPrimitive.Colors = new Accessor
                (
                    new[]
                    {
                        new Vector4(0.0f, 0.0f, 1.0f, 0.8f),
                        new Vector4(1.0f, 0.0f, 0.0f, 0.8f),
                        new Vector4(0.0f, 0.0f, 1.0f, 0.8f),
                        new Vector4(1.0f, 0.0f, 0.0f, 0.8f),
                    },
                    Accessor.ComponentTypeEnum.FLOAT,
                    Accessor.TypeEnum.VEC3
                );

                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetBaseColorTexture(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.BaseColorTexture = new Texture { Source = baseColorTextureImage };
                properties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));
            }

            void SetBaseColorFactor(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                var baseColorFactorValue = new Vector4(0.2f, 0.2f, 0.2f, 0.8f);
                metallicRoughness.BaseColorFactor = baseColorFactorValue;
                properties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorValue.ToReadmeString()));
            }

            void SetMetallicRoughnessTexture(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.MetallicRoughnessTexture = new Texture { Source = metallicRoughnessTextureImage };
                properties.Add(new Property(PropertyName.MetallicRoughnessTexture, metallicRoughnessTextureImage.ToReadmeString()));
            }

            void SetMetallicFactor(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.MetallicFactor = 0.0f;
                properties.Add(new Property(PropertyName.MetallicFactor, metallicRoughness.MetallicFactor.ToReadmeString()));
            }

            void SetRoughnessFactor(List<Property> properties, PbrMetallicRoughness metallicRoughness)
            {
                metallicRoughness.RoughnessFactor = 0.0f;
                properties.Add(new Property(PropertyName.RoughnessFactor, metallicRoughness.RoughnessFactor.ToReadmeString()));
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
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
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
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
                    SetVertexColor(properties, meshPrimitive);
                    SetBaseColorTexture(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
                    SetBaseColorTexture(properties, metallicRoughness);
                    SetBaseColorFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
                    SetMetallicRoughnessTexture(properties, metallicRoughness);
                    SetMetallicFactor(properties, metallicRoughness);
                }),
                CreateModel((properties, meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
                    SetMetallicRoughnessTexture(properties, metallicRoughness);
                    SetRoughnessFactor(properties, metallicRoughness);
                }),
                CreateModel((properties,meshPrimitive, metallicRoughness) =>
                {
                    meshPrimitive.TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
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
