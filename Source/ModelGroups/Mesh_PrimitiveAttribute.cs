using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Mesh_PrimitiveAttribute : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitiveAttribute;

        public Mesh_PrimitiveAttribute(List<string> imageList)
        {
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            Image normalImage = UseTexture(imageList, "Normal_Plane");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.MetallicRoughnessMaterial = new PbrMetallicRoughness
                {
                    BaseColorTexture = new Texture
                    {
                        Source = baseColorTextureImage
                    }
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
                meshPrimitive.TextureCoordSets.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordSets.ComponentType.ToReadmeString()));
            }

            void SetVertexUVByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordSets.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordSets.ComponentType.ToReadmeString()));
            }

            void SetVertexUVShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordSets.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordSets.ComponentType.ToReadmeString()));
            }

            void SetVertexNormal(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeNormalsValue = MeshPrimitive.GetSinglePlaneNormals();
                meshPrimitive.Normals = new Accessor(planeNormalsValue);
                properties.Add(new Property(PropertyName.VertexNormal, planeNormalsValue.ToReadmeString()));
            }

            void SetVertexTangent(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeTangentValue = new[]
                {
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
                };
                meshPrimitive.Tangents = new Accessor(planeTangentValue, Accessor.ComponentTypeEnum.FLOAT, Accessor.TypeEnum.VEC4);
                properties.Add(new Property(PropertyName.VertexTangent, planeTangentValue.ToReadmeString()));
            }

            void SetNormalTexture(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.NormalTexture = new Texture { Source = normalImage };
                properties.Add(new Property(PropertyName.NormalTexture, normalImage.ToReadmeString()));
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
