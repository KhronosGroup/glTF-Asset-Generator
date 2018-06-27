using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal class Mesh_PrimitivesUV : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitivesUV;

        public Mesh_PrimitivesUV(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            var normalImage = UseTexture(imageList, "Normal_Plane");
            UseFigure(imageList, "Indices_Primitive0");
            UseFigure(imageList, "Indices_Primitive1");
            UseFigure(imageList, "UVSpace2");
            UseFigure(imageList, "UVSpace3");
            UseFigure(imageList, "UVSpace4");
            UseFigure(imageList, "UVSpace5");

            // Track the common properties for use in the readme.
            var vertexNormalValue = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f)
            };
            var tangentValue = new List<Vector4>()
            {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
            };
            var vertexColorValue = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
            };
            CommonProperties.Add(new Property(PropertyName.VertexNormal, vertexNormalValue));
            CommonProperties.Add(new Property(PropertyName.VertexTangent, tangentValue));
            CommonProperties.Add(new Property(PropertyName.VertexColor, vertexColorValue));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalImage));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane();

                // Apply the common properties to the gltf. 
                foreach (var meshPrimitive in meshPrimitives)
                {
                    //   meshPrimitive.TextureCoordSets = new List<List<Vector2>>();
                    meshPrimitive.Material = new Runtime.Material();
                    meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                }

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitives.ElementAt(0), meshPrimitives.ElementAt(1));

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
                                    MeshPrimitives = meshPrimitives
                                },
                            },
                        },
                    }),
                };
            }

            void SetCommonProperties(Runtime.MeshPrimitive meshPrimitive)
            {
                for (var i = 0; i < meshPrimitive.Vertices.Count(); ++i)
                {
                    var vertex = meshPrimitive.Vertices.ElementAt(i);
                    vertex.Normal = vertexNormalValue.ElementAt(i);
                    vertex.Tangent = tangentValue.ElementAt(i);
                    vertex.Color = vertexColorValue.ElementAt(i);
                }

                meshPrimitive.Material.NormalTexture = new Runtime.Texture() { Source = normalImage };
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage };
            }

            void SetNullUV(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material = null;
                for (var i = 0; i < meshPrimitive.Vertices.Count(); ++i)
                {
                    var vertex = meshPrimitive.Vertices.ElementAt(i);
                    vertex.TextureCoordSet = null;
                }
            }

            void SetPrimitiveZeroVertexUVZero(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                var textureCoordSets =
                    new List<Vector2>()
                    {
                        new Vector2( 0.0f, 1.0f),
                        new Vector2( 1.0f, 0.0f),
                        new Vector2( 0.0f, 0.0f)
                    };
                Runtime.MeshPrimitive.SetVertexProperties(meshPrimitive.Vertices, textureCoordSets, (vertex, textureCoord) =>
                {
                    vertex.TextureCoordSet = new List<Vector2>
                    {
                        textureCoord
                    };
                });
               
                properties.Add(new Property(PropertyName.Primitive0VertexUV0, ":white_check_mark:"));
            }

            void SetPrimitiveOneVertexUVZero(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                var textureCoordSets =
                    new List<Vector2>()
                    {
                        new Vector2( 0.0f, 1.0f),
                        new Vector2( 1.0f, 1.0f),
                        new Vector2( 1.0f, 0.0f)
                    };
                Runtime.MeshPrimitive.SetVertexProperties(meshPrimitive.Vertices, textureCoordSets, (vertex, textureCoord) =>
                {
                    vertex.TextureCoordSet = new List<Vector2>
                    {
                        textureCoord
                    };
                });
                properties.Add(new Property(PropertyName.Primitive1VertexUV0, ":white_check_mark:"));
            }

            void SetPrimitiveZeroVertexUVOne(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                meshPrimitive.Material.NormalTexture.TexCoordIndex = 1;
                var textureCoordSets =
                    new List<Vector2>()
                    {
                        new Vector2( 0.5f, 0.5f),
                        new Vector2( 1.0f, 0.0f),
                        new Vector2( 0.5f, 0.0f)
                    };
                Runtime.MeshPrimitive.SetVertexProperties(meshPrimitive.Vertices, textureCoordSets, (vertex, textureCoord) =>
                {
                    vertex.TextureCoordSet = vertex.TextureCoordSet.Concat(new[] { textureCoord });
                });

                properties.Add(new Property(PropertyName.Primitive0VertexUV1, ":white_check_mark:"));
            }

            void SetPrimitiveOneVertexUVOne(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                SetCommonProperties(meshPrimitive);
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                meshPrimitive.Material.NormalTexture.TexCoordIndex = 1;
                var textureCoordSets =
                     new List<Vector2>()
                     {
                        new Vector2( 0.5f, 0.5f),
                        new Vector2( 1.0f, 0.5f),
                        new Vector2( 1.0f, 0.0f)
                     };
                Runtime.MeshPrimitive.SetVertexProperties(meshPrimitive.Vertices, textureCoordSets, (vertex, textureCoord) =>
                {
                    vertex.TextureCoordSet = vertex.TextureCoordSet.Concat(new[] { textureCoord });
                });
               
                properties.Add(new Property(PropertyName.Primitive1VertexUV1, ":white_check_mark:"));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetNullUV(meshPrimitiveZero);
                    SetNullUV(meshPrimitiveOne);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveZeroVertexUVZero(properties, meshPrimitiveZero);
                    SetNullUV(meshPrimitiveOne);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveOneVertexUVZero(properties, meshPrimitiveOne);
                    SetNullUV(meshPrimitiveZero);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveZeroVertexUVZero(properties, meshPrimitiveZero);
                    SetPrimitiveOneVertexUVZero(properties, meshPrimitiveOne);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveZeroVertexUVZero(properties, meshPrimitiveZero);
                    SetPrimitiveZeroVertexUVOne(properties, meshPrimitiveZero);
                    SetNullUV(meshPrimitiveOne);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveOneVertexUVZero(properties, meshPrimitiveOne);
                    SetPrimitiveOneVertexUVOne(properties, meshPrimitiveOne);
                    SetNullUV(meshPrimitiveZero);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveZeroVertexUVZero(properties, meshPrimitiveZero);
                    SetPrimitiveOneVertexUVZero(properties, meshPrimitiveOne);
                    SetPrimitiveOneVertexUVOne(properties, meshPrimitiveOne);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveZeroVertexUVZero(properties, meshPrimitiveZero);
                    SetPrimitiveOneVertexUVZero(properties, meshPrimitiveOne);
                    SetPrimitiveZeroVertexUVOne(properties, meshPrimitiveZero);
                }),
                CreateModel((properties, meshPrimitiveZero, meshPrimitiveOne) => {
                    SetPrimitiveZeroVertexUVZero(properties, meshPrimitiveZero);
                    SetPrimitiveOneVertexUVZero(properties, meshPrimitiveOne);
                    SetPrimitiveZeroVertexUVOne(properties, meshPrimitiveZero);
                    SetPrimitiveOneVertexUVOne(properties, meshPrimitiveOne);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
