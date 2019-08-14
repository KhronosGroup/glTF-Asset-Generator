using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;
using static AssetGenerator.Runtime.Accessor;
using static AssetGenerator.Runtime.MeshPrimitive;

namespace AssetGenerator
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
                    new Runtime.MeshPrimitive
                    {
                        Indices = new Accessor()
                    },
                    new Runtime.MeshPrimitive
                    {
                        Indices = new Accessor()
                    }
                };

                // Apply the properties that are specific to this gltf.
                setProperties(meshPrimitives);

                var properties = new List<Property>
                {
                    new Property(PropertyName.IndicesComponentType, meshPrimitives[0].Indices.ComponentType.ToReadmeString()),
                    new Property(PropertyName.LeftPrimitiveIndices, ((IEnumerable<int>)meshPrimitives[0].Indices.Values).ToReadmeString()),
                    new Property(PropertyName.RightPrimitiveIndices, ((IEnumerable<int>)meshPrimitives[1].Indices.Values).ToReadmeString()),
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

            IEnumerable<Vector3> BuildPositions(ComponentTypeEnum type, bool restart)
            {
                var offset = restart ? -0.6f : 0.6f;
                int count;
                switch (type)
                {
                    case ComponentTypeEnum.UNSIGNED_BYTE:
                        count = 255;
                        break;
                    case ComponentTypeEnum.UNSIGNED_SHORT:
                        count = 65535;
                        break;
                    case ComponentTypeEnum.UNSIGNED_INT:
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

            var typeDelegates = new List<Action<List<Runtime.MeshPrimitive>>>
            {
                meshPrimitives =>
                {
                    meshPrimitives[0].Positions = new Accessor(BuildPositions(ComponentTypeEnum.UNSIGNED_BYTE, true), ComponentTypeEnum.FLOAT, TypeEnum.VEC3);
                    meshPrimitives[0].Indices.ComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                    meshPrimitives[1].Positions = new Accessor(BuildPositions(ComponentTypeEnum.UNSIGNED_BYTE, false), ComponentTypeEnum.FLOAT, TypeEnum.VEC3);
                    meshPrimitives[1].Indices.ComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                },
                meshPrimitives =>
                {
                    meshPrimitives[0].Positions = new Accessor(BuildPositions(ComponentTypeEnum.UNSIGNED_SHORT, true), ComponentTypeEnum.FLOAT, TypeEnum.VEC3);
                    meshPrimitives[0].Indices.ComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                    meshPrimitives[1].Positions = new Accessor(BuildPositions(ComponentTypeEnum.UNSIGNED_SHORT, false), ComponentTypeEnum.FLOAT, TypeEnum.VEC3);
                    meshPrimitives[1].Indices.ComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                }
            };

            var topologyDelegates = new List<Action<Runtime.MeshPrimitive, int>>
            {
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.POINTS;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.LINES;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, 1, maxValue, maxValue, 0
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.LINE_LOOP;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.LINE_STRIP;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, maxValue, 0
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.TRIANGLES;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.TRIANGLE_STRIP;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, maxValue
                    };
                },
                (meshPrimitive, maxValue) =>
                {
                    meshPrimitive.Mode = ModeEnum.TRIANGLE_FAN;
                    meshPrimitive.Indices.Values = new int[]
                    {
                        0, 1, maxValue
                    };
                }
            };

            Models = new List<Model>();

            foreach (var topologyDelegate in topologyDelegates)
            {
                foreach (var typeDelegate in typeDelegates)
                {
                    Models.Add(CreateModel(meshPrimitives =>
                    {
                        typeDelegate(meshPrimitives);
                        // Models triggering restart
                        topologyDelegate(meshPrimitives[0], meshPrimitives[0].Positions.ValuesCount - 1);
                        // Models avoiding restart
                        topologyDelegate(meshPrimitives[1], meshPrimitives[1].Positions.ValuesCount - 1);
                    }));
                }
            }

            GenerateUsedPropertiesList();
        }
    }
}