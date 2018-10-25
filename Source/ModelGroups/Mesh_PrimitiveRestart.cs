using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AssetGenerator.Runtime;

namespace AssetGenerator
{
    internal class Mesh_PrimitiveRestart : ModelGroup
    {
        public Mesh_PrimitiveRestart(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Runtime.MeshPrimitive>> setProperties)
            {
                var meshPrimitives = new List<Runtime.MeshPrimitive>
                {
                    new Runtime.MeshPrimitive(),
                    new Runtime.MeshPrimitive()
                };

                // Apply the properties that are specific to this gltf.
                setProperties(meshPrimitives);

                var properties = new List<Property>
                {
                    new Property(PropertyName.IndicesComponentType, meshPrimitives[0].IndexComponentType),
                    new Property(PropertyName.LeftPrimitiveIndices, meshPrimitives[0].Indices),
                    new Property(PropertyName.RightPrimitiveIndices, meshPrimitives[1].Indices),
                    new Property(PropertyName.Mode, meshPrimitives[0].Mode)
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
                    })
                };
            }

            IEnumerable<Vector3> BuildPositions(Runtime.MeshPrimitive.IndexComponentTypeEnum type, bool restart)
            {
                var offset = restart ? -0.6f : 0.6f;
                int count;
                switch (type)
                {
                    case Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE:
                        count = 255;
                        break;
                    case Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT:
                        count = 65535;
                        break;
                    case Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                if (restart) ++count;

                var positions = new Vector3[count];
                positions[0] = new Vector3(0.5f + offset, -0.5f, 0.0f);
                positions[1] = new Vector3(-0.5f + offset, 0.5f, 0.0f);
                for (var i = 2; i < count - 1; ++i) positions[i] = Vector3.Zero;
                positions[count - 1] = new Vector3(-0.5f + offset, -0.5f, 0.0f);
                return positions;
            }

            var typeDelegates = new List<Action<List<Runtime.MeshPrimitive>>>
            {
                meshPrimitives =>
                {
                    meshPrimitives[0].Positions =
                        BuildPositions(Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE, true);
                    meshPrimitives[0].IndexComponentType = Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE;
                    meshPrimitives[1].Positions =
                        BuildPositions(Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE, false);
                    meshPrimitives[1].IndexComponentType = Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE;
                },
                meshPrimitives =>
                {
                    meshPrimitives[0].Positions =
                        BuildPositions(Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT, true);
                    meshPrimitives[0].IndexComponentType = Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT;
                    meshPrimitives[1].Positions =
                        BuildPositions(Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT, false);
                    meshPrimitives[1].IndexComponentType = Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT;
                }
            };

            var topologyDelegates = new List<Action<Runtime.MeshPrimitive, int>>
            {
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.POINTS;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.LINES;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, 1, maxValue, maxValue, 0
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.LINE_LOOP;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.LINE_STRIP;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, maxValue, 0
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.TRIANGLES;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.TRIANGLE_STRIP;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = Runtime.MeshPrimitive.ModeEnum.TRIANGLE_FAN;
                    meshPrimitive.Indices = new List<int>
                    {
                        0, 1, maxValue
                    };
                }
            };

            Models = new List<Model>();

            foreach (var topologyDelegate in topologyDelegates)
            foreach (var typeDelegate in typeDelegates)
                Models.Add(CreateModel(meshPrimitives =>
                {
                    typeDelegate(meshPrimitives);
                    // Models triggering restart
                    topologyDelegate(meshPrimitives[0], meshPrimitives[0].Positions.Count() - 1);
                    // Models avoiding restart
                    topologyDelegate(meshPrimitives[1], meshPrimitives[1].Positions.Count() - 1);
                }));

            GenerateUsedPropertiesList();
        }

        public override ModelGroupId Id => ModelGroupId.Mesh_PrimitiveRestart;
    }
}