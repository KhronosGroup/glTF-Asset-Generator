using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Mesh_PrimitiveRestart : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitiveRestart;

        public Mesh_PrimitiveRestart(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            NoSampleImages = true;

            Model CreateModel(Action<List<Runtime.MeshPrimitive>> setProperties)
            {
                var meshPrimitives = new List<Runtime.MeshPrimitive>
                {
                    new Runtime.MeshPrimitive(),
                    new Runtime.MeshPrimitive(),
                };

                // Apply the properties that are specific to this gltf.
                setProperties(meshPrimitives);

                var properties = new List<Property>
                {
                    new Property(PropertyName.IndicesComponentType, meshPrimitives[0].Indices.OutputType.ToReadmeString()),
                    new Property(PropertyName.LeftPrimitiveIndices, meshPrimitives[0].Indices.Values.ToReadmeString()),
                    new Property(PropertyName.RightPrimitiveIndices, meshPrimitives[1].Indices.Values.ToReadmeString()),
                    new Property(PropertyName.Mode, meshPrimitives[0].Mode.ToReadmeString())
                };

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
                                }
                            }
                        }
                    }),
                    Loadable = null
                };
            }

            Vector3[] BuildPositions(DataType type, bool restart)
            {
                var offset = restart ? -0.6f : 0.6f;
                int count;
                switch (type)
                {
                    case DataType.UnsignedByte:
                        count = byte.MaxValue;
                        break;
                    case DataType.UnsignedShort:
                        count = ushort.MaxValue;
                        break;
                    case DataType.UnsignedInt:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                if (restart) ++count;

                var positions = new Vector3[count];
                positions[0] = new Vector3(0.5f + offset, -0.5f, 0.0f);
                positions[1] = new Vector3(-0.5f + offset, 0.5f, 0.0f);
                for (var i = 2; i < count - 1; ++i)
                {
                    positions[i] = Vector3.Zero;
                }
                positions[count - 1] = new Vector3(-0.5f + offset, -0.5f, 0.0f);
                return positions;
            }

            var typeDelegates = new List<Func<List<Runtime.MeshPrimitive>, DataType>>
            {
                meshPrimitives =>
                {
                    meshPrimitives[0].Positions = Data.Create(BuildPositions(DataType.UnsignedByte, true));
                    meshPrimitives[1].Positions = Data.Create(BuildPositions(DataType.UnsignedByte, false));
                    return DataType.UnsignedByte;
                },
                meshPrimitives =>
                {
                    meshPrimitives[0].Positions = Data.Create(BuildPositions(DataType.UnsignedShort, true));
                    meshPrimitives[1].Positions = Data.Create(BuildPositions(DataType.UnsignedShort, false));
                    return DataType.UnsignedShort;
                }
            };

            var topologyDelegates = new List<Action<Runtime.MeshPrimitive, int, DataType>>
            {
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.Points;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, maxValue
                    }, accessorType);
                },
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.Lines;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, 1, maxValue, maxValue, 0
                    }, accessorType);
                },
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.LineLoop;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, maxValue
                    }, accessorType);
                },
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.LineStrip;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, maxValue, 0
                    }, accessorType);
                },
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.Triangles;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, maxValue
                    }, accessorType);
                },
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.TriangleStrip;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, maxValue
                    }, accessorType);
                },
                (meshPrimitive, maxValue, accessorType) =>
                {
                    meshPrimitive.Mode = MeshPrimitiveMode.TriangleFan;
                    meshPrimitive.Indices = Data.Create(new[]
                    {
                        0, 1, maxValue
                    }, accessorType);
                }
            };

            Models = new List<Model>();

            foreach (var topologyDelegate in topologyDelegates)
            {
                foreach (var typeDelegate in typeDelegates)
                {
                    Models.Add(CreateModel(meshPrimitives =>
                    {
                        DataType accessorType = typeDelegate(meshPrimitives);
                        // Models triggering restart
                        topologyDelegate(meshPrimitives[0], meshPrimitives[0].Positions.Values.Count() - 1, accessorType);
                        // Models avoiding restart
                        topologyDelegate(meshPrimitives[1], meshPrimitives[1].Positions.Values.Count() - 1, accessorType);
                    }));
                }
            }

            GenerateUsedPropertiesList();
        }
    }
}