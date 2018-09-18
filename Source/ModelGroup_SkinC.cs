using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreatePlaneWithSkinC()
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
                                    new Vector3(-0.25f,-0.4f, 0.0f),
                                    new Vector3( 0.25f,-0.4f, 0.0f),
                                    new Vector3(-0.25f,-0.2f, 0.0f),
                                    new Vector3( 0.25f,-0.2f, 0.0f),
                                    new Vector3(-0.25f, 0.0f, 0.0f),
                                    new Vector3( 0.25f, 0.0f, 0.0f),
                                    new Vector3(-0.25f, 0.2f, 0.0f),
                                    new Vector3( 0.25f, 0.2f, 0.0f),
                                    new Vector3(-0.25f, 0.4f, 0.0f),
                                    new Vector3( 0.25f, 0.4f, 0.0f),
                                    new Vector3(-0.25f, 0.6f, 0.0f),
                                    new Vector3( 0.25f, 0.6f, 0.0f),
                                },
                                Indices = new List<int>
                                {
                                    0, 1, 2,
                                    2, 1, 3,
                                    2, 3, 4,
                                    4, 3, 5,
                                    4, 5, 6,
                                    6, 5, 7,
                                    6, 7, 8,
                                    8, 7, 9,
                                    8, 9, 10,
                                    10, 9, 11
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
                                },
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true
                                }
                            }
                        }
                    },
                };

                var nodeJoint4 = new Runtime.Node
                {
                    Name = "Joint4",
                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                };

                var nodeJoint3 = new Runtime.Node
                {
                    Name = "Joint3",
                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint4
                    }
                };

                var nodeJoint2 = new Runtime.Node
                {
                    Name = "Joint2",
                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint3
                    }
                };

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "Joint1",
                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint2
                    }
                };

                var nodeJoint0 = new Runtime.Node
                {
                    Name = "Joint0",
                    Translation = new Vector3(0.0f, -0.4f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.4f, 0.0f)),
                    node: nodeJoint0
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.2f, 0.0f)),
                    node: nodeJoint1
                );
                var joint2 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.Identity,
                    node: nodeJoint2
                );
                var joint3 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, -0.2f, 0.0f)),
                    node: nodeJoint3
                );
                var joint4 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, -0.4f, 0.0f)),
                    node: nodeJoint4
                );
                var skinJointsList = new List<Runtime.SkinJoint>()
                {
                    joint0,
                    joint1,
                    joint2,
                    joint3,
                    joint4
                };
                nodePlane.Skin.SkinJoints = skinJointsList;

                // Assign joint weights to pairs of vertexs, except the last four which all share a joint
                var weightsList = new List<List<Runtime.JointWeight>>();
                int jointIndex = 0;
                for (int vertexIndex = 0; vertexIndex < 8; vertexIndex++)
                {
                    weightsList.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinJointsList[jointIndex],
                            Weight = 1,
                        },
                    });
                    if (vertexIndex % 2 != 0)
                    {
                        jointIndex++;
                    }
                }
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
                    weightsList.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = joint4,
                            Weight = 1,
                        },
                    });
                }
                nodePlane.Mesh.MeshPrimitives.First().VertexJointWeights = weightsList;

                return new List<Runtime.Node>
                {
                    nodePlane,
                    nodeJoint0
                };
            }
        }
    }
}
