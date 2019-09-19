using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Mesh_PrimitivesUV : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitivesUV;

        public Mesh_PrimitivesUV(List<string> imageList)
        {
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            Image normalImage = UseTexture(imageList, "Normal_Plane");
            UseFigure(imageList, "Indices_Primitive0");
            UseFigure(imageList, "Indices_Primitive1");
            UseFigure(imageList, "UVSpace2");
            UseFigure(imageList, "UVSpace3");
            UseFigure(imageList, "UVSpace4");
            UseFigure(imageList, "UVSpace5");

            // Track the common properties for use in the readme.
            var vertexNormalValue = new[]
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
            };
            var tangentValue = new[]
            {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            };
            var vertexColorValue = new[]
            {
                new Vector4(0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4(1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4(0.0f, 0.0f, 1.0f, 0.2f),
            };
            CommonProperties.Add(new Property(PropertyName.VertexNormal, vertexNormalValue.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.VertexTangent, tangentValue.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.VertexColor, vertexColorValue.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalImage.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                List<Runtime.MeshPrimitive> meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane(false);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitives[0], meshPrimitives[1]);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = new List<Node>
                        {
                            new Node
                            {
                                Mesh = new Runtime.Mesh
                                {
                                    MeshPrimitives = meshPrimitives
                                },
                            },
                        },
                    }),
                };
            }

            void SetCommonProperties(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Normals = new Accessor(vertexNormalValue);
                meshPrimitive.Tangents = new Accessor(tangentValue);
                meshPrimitive.Colors = new Accessor(vertexColorValue, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.VEC3);

                meshPrimitive.Material = new Runtime.Material
                {
                    NormalTexture = new Texture { Source = normalImage },
                    MetallicRoughnessMaterial = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new Texture() { Source = baseColorTextureImage },
                        MetallicFactor = 0,
                    }
                };
            }

            void SetPrimitive0VertexUV0(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                meshPrimitive.TextureCoordSets = new Accessor
                (
                    new List<Vector2[]>
                    {
                        new[]
                        {
                            new Vector2(0.0f, 1.0f),
                            new Vector2(1.0f, 0.0f),
                            new Vector2(0.0f, 0.0f),
                        }
                    }
                );
                properties.Add(new Property(PropertyName.Primitive0VertexUV0, ":white_check_mark:"));
            }

            void SetPrimitive0VertexUV1(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                meshPrimitive.Material.NormalTexture.TexCoordIndex = 1;
                ((List<Vector2[]>)meshPrimitive.TextureCoordSets.Values).Add
                (
                    new[]
                    {
                        new Vector2(0.5f, 0.5f),
                        new Vector2(1.0f, 0.0f),
                        new Vector2(0.5f, 0.0f),
                    }
                );
                properties.Add(new Property(PropertyName.Primitive0VertexUV1, ":white_check_mark:"));
            }

            void SetPrimitive1VertexUV0(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                meshPrimitive.TextureCoordSets = new Accessor
                (
                    new List<Vector2[]>
                    {
                        new[]
                        {
                            new Vector2(0.0f, 1.0f),
                            new Vector2(1.0f, 1.0f),
                            new Vector2(1.0f, 0.0f),
                        }
                    }
                );
                properties.Add(new Property(PropertyName.Primitive1VertexUV0, ":white_check_mark:"));
            }

            void SetPrimitive1VertexUV1(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                meshPrimitive.Material.NormalTexture.TexCoordIndex = 1;
                ((List<Vector2[]>)meshPrimitive.TextureCoordSets.Values).Add
                (
                    new[]
                    {
                        new Vector2(0.5f, 0.5f),
                        new Vector2(1.0f, 0.5f),
                        new Vector2(1.0f, 0.0f),
                    }
                );
                properties.Add(new Property(PropertyName.Primitive1VertexUV1, ":white_check_mark:"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) =>
                {
                    // do nothing
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive0VertexUV0(properties, meshPrimitive0);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive1VertexUV0(properties, meshPrimitive1);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive0VertexUV0(properties, meshPrimitive0);
                    SetPrimitive1VertexUV0(properties, meshPrimitive1);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive0VertexUV0(properties, meshPrimitive0);
                    SetPrimitive0VertexUV1(properties, meshPrimitive0);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive1VertexUV0(properties, meshPrimitive1);
                    SetPrimitive1VertexUV1(properties, meshPrimitive1);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive0VertexUV0(properties, meshPrimitive0);
                    SetPrimitive1VertexUV0(properties, meshPrimitive1);
                    SetPrimitive1VertexUV1(properties, meshPrimitive1);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive0VertexUV0(properties, meshPrimitive0);
                    SetPrimitive0VertexUV1(properties, meshPrimitive0);
                    SetPrimitive1VertexUV0(properties, meshPrimitive1);
                }),
                CreateModel((properties, meshPrimitive0, meshPrimitive1) =>
                {
                    SetPrimitive0VertexUV0(properties, meshPrimitive0);
                    SetPrimitive1VertexUV0(properties, meshPrimitive1);
                    SetPrimitive0VertexUV1(properties, meshPrimitive0);
                    SetPrimitive1VertexUV1(properties, meshPrimitive1);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
