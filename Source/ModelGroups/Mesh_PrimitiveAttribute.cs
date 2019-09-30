using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Mesh_PrimitiveAttribute : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitiveAttribute;

        public Mesh_PrimitiveAttribute(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Plane") };
            var normalTexture = new Texture { Source = UseTexture(imageList, "Normal_Plane") };

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.PbrMetallicRoughness = new PbrMetallicRoughness
                {
                    BaseColorTexture = new TextureInfo { Texture = baseColorTexture },
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

                // Create the gltf object.
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

            void SetVertexUVFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TexCoords0.OutputType = DataType.Float;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TexCoords0.OutputType.ToReadmeString()));
            }

            void SetVertexUVByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TexCoords0.OutputType = DataType.NormalizedUnsignedByte;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TexCoords0.OutputType.ToReadmeString()));
            }

            void SetVertexUVShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TexCoords0.OutputType = DataType.NormalizedUnsignedShort;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TexCoords0.OutputType.ToReadmeString()));
            }

            void SetVertexNormal(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var normals = MeshPrimitive.GetSinglePlaneNormals();
                meshPrimitive.Normals = Data.Create(normals);
                properties.Add(new Property(PropertyName.VertexNormal, normals.ToReadmeString()));
            }

            void SetVertexTangent(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var tangents = new[]
                {
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
                };
                meshPrimitive.Tangents = Data.Create(tangents);
                properties.Add(new Property(PropertyName.VertexTangent, tangents.ToReadmeString()));
            }

            void SetNormalTexture(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.NormalTexture = new NormalTextureInfo { Texture = normalTexture };
                properties.Add(new Property(PropertyName.NormalTexture, normalTexture.Source.ToReadmeString()));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVByte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVShort(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetVertexNormal(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetVertexNormal(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetVertexNormal(properties, meshPrimitive);
                    SetVertexTangent(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
