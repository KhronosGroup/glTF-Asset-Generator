using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Mesh_PrimitiveVertexColor : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitiveVertexColor;

        public Mesh_PrimitiveVertexColor(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            var colors = new[]
            {
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
            };

            Data<Vector3> GetColors3()
            {
                return Data.Create(colors);
            }

            Data<Vector4> GetColors4()
            {
                return Data.Create(colors.Select(color => new Vector4(color, 0.2f)));
            }

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

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

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Colors = GetColors3();
                    meshPrimitive.Colors.OutputType = DataType.Float;
                    properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Colors = GetColors3();
                    meshPrimitive.Colors.OutputType = DataType.NormalizedUnsignedByte;
                    properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Colors = GetColors3();
                    meshPrimitive.Colors.OutputType = DataType.NormalizedUnsignedShort;
                    properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Colors = GetColors4();
                    meshPrimitive.Colors.OutputType = DataType.Float;
                    properties.Add(new Property(PropertyName.VertexColor, $"Vector4 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Colors = GetColors4();
                    meshPrimitive.Colors.OutputType = DataType.NormalizedUnsignedByte;
                    properties.Add(new Property(PropertyName.VertexColor, $"Vector4 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    meshPrimitive.Colors = GetColors4();
                    meshPrimitive.Colors.OutputType = DataType.NormalizedUnsignedShort;
                    properties.Add(new Property(PropertyName.VertexColor, $"Vector4 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
