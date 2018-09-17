using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Scene
        {
            public static Runtime.Scene CreatePlaneWithSkinD()
            {
                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin(),
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                Positions = new List<Vector3>()
                                {
                                    new Vector3(-0.125f,-0.25f, 0.0f),
                                    new Vector3( 0.125f,-0.25f, 0.0f),
                                    new Vector3(-0.125f, 0.00f, 0.0f),
                                    new Vector3( 0.125f, 0.00f, 0.0f),
                                    new Vector3(-0.125f, 0.25f, 0.0f),
                                    new Vector3( 0.125f, 0.25f, 0.0f),

                                    new Vector3( 0.00f, 0.25f, 0.0f),

                                    new Vector3(-0.25f, 0.50f, 0.0f),
                                    new Vector3(-0.125f, 0.50f, 0.0f),
                                    new Vector3(-0.375f, 0.75f, 0.0f),
                                    new Vector3(-0.25f, 0.75f, 0.0f),

                                    new Vector3( 0.125f, 0.50f, 0.0f),
                                    new Vector3( 0.25f, 0.50f, 0.0f),
                                    new Vector3( 0.25f, 0.75f, 0.0f),
                                    new Vector3( 0.375f, 0.75f, 0.0f),
                                },
                                Indices = new List<int>
                                {
                                    0, 1, 2,
                                    2, 1, 3,
                                    2, 3, 4,
                                    4, 3, 5,
                                    4, 6, 7,
                                    7, 6, 8,
                                    7, 8, 9,
                                    9, 8, 10,
                                    6, 5, 11,
                                    11, 5, 12,
                                    11, 12, 13,
                                    13, 12, 14
                                },
                                Colors = new List<Vector4>()
                                {
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                    new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                },
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true
                                }
                            }
                        }
                    },
                };

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "joint1",
                    Translation = new Vector3(-0.1875f, 0.25f, 0.0f),
                };
                var nodeJoint2 = new Runtime.Node
                {
                    Name = "joint2",
                    Translation = new Vector3(0.1875f, 0.25f, 0.0f),
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "joint0",
                    Translation = new Vector3(0.0f, 0.25f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint1,
                        nodeJoint2
                    }
                };

                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, -0.25f, 0.0f)),
                    node: nodeJoint0
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.1875f, -0.5f, 0.0f)),
                    node: nodeJoint1
                );
                var joint2 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(-0.1875f, -0.5f, 0.0f)),
                    node: nodeJoint2
                );
                nodePlane.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                    joint2
                };

                // Top four vertexes of each arm have a weight for the relevant joint. Otherwise the vertex has a weight from the root
                var jointWeights = new List<List<Runtime.JointWeight>>();
                for (int x = 0; x < 15; x++)
                {
                    if (x > 6 && x < 11)
                    {
                        // Left arm
                        jointWeights.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = joint0,
                                Weight = 0,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = joint1,
                                Weight = 1,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = joint2,
                                Weight = 0,
                            }
                        });
                    }
                    else if (x >= 11)
                    {
                        // Right arm
                        jointWeights.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = joint0,
                                Weight = 0,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = joint1,
                                Weight = 0,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = joint2,
                                Weight = 1,
                            }
                        });
                    }
                    else
                    {
                        // Common parent
                        jointWeights.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = joint0,
                                Weight = 1,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = joint1,
                                Weight = 0,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = joint2,
                                Weight = 0,
                            }
                        });
                    }
                }
                nodePlane.Mesh.MeshPrimitives.First().VertexJointWeights = jointWeights;

                return new Runtime.Scene
                {
                    Nodes = new[]
                    {
                        nodeJoint0,
                    }
                };
            }
        }
    }
}
