using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Mesh_PrimitivesUV : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitivesUV;

        public Mesh_PrimitivesUV(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Plane") };
            var normalTexture = new Texture { Source = UseTexture(imageList, "Normal_Plane") };
            UseFigure(imageList, "Indices_Primitive0");
            UseFigure(imageList, "Indices_Primitive1");
            UseFigure(imageList, "UVSpace2");
            UseFigure(imageList, "UVSpace3");
            UseFigure(imageList, "UVSpace4");
            UseFigure(imageList, "UVSpace5");

            // Track the common properties for use in the readme.
            var normals = Data.Create(new[]
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
            });
            var tangents = Data.Create(new[]
            {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            });
            var colors = Data.Create(new[]
            {
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
            });
            CommonProperties.Add(new Property(PropertyName.VertexNormal, normals.Values.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.VertexTangent, tangents.Values.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.VertexColor, colors.Values.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalTexture.Source.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));

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
                meshPrimitive.Normals = normals;
                meshPrimitive.Tangents = tangents;
                meshPrimitive.Colors = colors;

                meshPrimitive.Material = new Runtime.Material
                {
                    NormalTexture = new NormalTextureInfo { Texture = normalTexture },
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTexture },
                        MetallicFactor = 0,
                    }
                };
            }

            void SetPrimitive0VertexUV0(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                meshPrimitive.TexCoords0 = Data.Create
                (
                    new[]
                    {
                        new Vector2(0.0f, 1.0f),
                        new Vector2(1.0f, 0.0f),
                        new Vector2(0.0f, 0.0f),
                    }
                );
                properties.Add(new Property(PropertyName.Primitive0VertexUV0, ":white_check_mark:"));
            }

            void SetPrimitive0VertexUV1(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.PbrMetallicRoughness.BaseColorTexture.TexCoord = 1;
                meshPrimitive.Material.NormalTexture.TexCoord = 1;
                meshPrimitive.TexCoords1 = Data.Create
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
                meshPrimitive.TexCoords0 = Data.Create(new[]
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f),
                });
                properties.Add(new Property(PropertyName.Primitive1VertexUV0, ":white_check_mark:"));
            }

            void SetPrimitive1VertexUV1(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.PbrMetallicRoughness.BaseColorTexture.TexCoord = 1;
                meshPrimitive.Material.NormalTexture.TexCoord = 1;
                meshPrimitive.TexCoords1 = Data.Create(new[]
                {
                    new Vector2(0.5f, 0.5f),
                    new Vector2(1.0f, 0.5f),
                    new Vector2(1.0f, 0.0f),
                });
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
