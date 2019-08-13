using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Buffer_Interleaved : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Buffer_Interleaved;

        public Buffer_Interleaved(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Grey");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane();

                // Apply the common properties to the gltf.
                meshPrimitive.Interleave = true;
                meshPrimitive.Colors = new Accessor
                (
                    new[]
                    {
                        new Vector4(0.0f, 1.0f, 0.0f, 0.2f),
                        new Vector4(1.0f, 0.0f, 0.0f, 0.2f),
                        new Vector4(1.0f, 1.0f, 0.0f, 0.2f),
                        new Vector4(0.0f, 0.0f, 1.0f, 0.2f),
                    }
                );
                meshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new Texture
                        {
                            Source = baseColorTextureImage,
                            Sampler = new Sampler(),
                        },
                    },
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

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

            void SetUvTypeFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordSets.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordSets.ComponentType.ToReadmeString()));
            }

            void SetUvTypeTypeByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordSets.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordSets.ComponentType.ToReadmeString()));
            }

            void SetUvTypeTypeShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordSets.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordSets.ComponentType.ToReadmeString()));
            }

            void SetColorTypeFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC3;
                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetColorTypeByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC3;
                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetColorTypeShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC3;
                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeByte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeShort(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeTypeByte(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeTypeShort(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
