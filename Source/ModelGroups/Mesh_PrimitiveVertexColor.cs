using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Mesh_PrimitiveVertexColor : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitiveVertexColor;

        public Mesh_PrimitiveVertexColor(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

                // Apply the common properties to the gltf. 
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

            void SetVertexColorVec3Float(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC3;

                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetVertexColorVec3Byte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC3;

                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetVertexColorVec3Short(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC3;

                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetVertexColorVec4Float(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC4;

                properties.Add(new Property(PropertyName.VertexColor, $"Vector4 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetVertexColorVec4Byte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC4;

                properties.Add(new Property(PropertyName.VertexColor, $"Vector4 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetVertexColorVec4Short(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                meshPrimitive.Colors.Type = Accessor.TypeEnum.VEC4;

                properties.Add(new Property(PropertyName.VertexColor, $"Vector4 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexColorVec3Float(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexColorVec3Byte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexColorVec3Short(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexColorVec4Float(properties, meshPrimitive);
                }),
                CreateModel((properties,meshPrimitive) =>
                {
                    SetVertexColorVec4Byte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexColorVec4Short(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
