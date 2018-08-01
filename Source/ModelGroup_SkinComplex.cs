using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Scene
        {
            public static Runtime.Scene CreateComplexPlaneWithSkin()
            {
                Runtime.Scene scene = new Runtime.Scene
                {
                    Nodes = new[]
                    {
                        new Runtime.Node
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
                                            new Vector3(-0.5f, 0.0f, 0.0f),
                                            new Vector3( 0.5f, 0.0f, 0.0f),
                                            new Vector3(-0.5f, 0.2f, 0.0f),
                                            new Vector3( 0.5f, 0.2f, 0.0f),
                                            new Vector3(-0.5f, 0.4f, 0.0f),
                                            new Vector3( 0.5f, 0.4f, 0.0f),
                                            new Vector3(-0.5f, 0.6f, 0.0f),
                                            new Vector3( 0.5f, 0.6f, 0.0f),
                                            new Vector3(-0.5f, 0.8f, 0.0f),
                                            new Vector3( 0.5f, 0.8f, 0.0f),
                                            new Vector3(-0.5f, 1.0f, 0.0f),
                                            new Vector3( 0.5f, 1.0f, 0.0f),
                                        },
                                        Normals = new List<Vector3>()
                                        {
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                        },
                                        Indices = new List<int>
                                        {
                                            0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7, 6, 7, 8, 8, 7, 9, 8, 9, 10, 10, 9, 11
                                        },
                                        Colors = new List<Vector4>()
                                        {
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                        },
                                        Material = new Runtime.Material
                                        {
                                            DoubleSided = true
                                        }
                                    }
                                }
                            },
                        },
                        new Runtime.Node
                        {
                            Name = "rootJoint",
                            Children = new[]
                            {
                                new Runtime.Node
                                {
                                    Name = "rootMidJoint",
                                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                    Children = new[]
                                    {
                                        new Runtime.Node
                                        {
                                            Name = "midJoint",
                                            Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                            Children = new[]
                                            {
                                                new Runtime.Node
                                                {
                                                    Name = "midTopJoint",
                                                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                                    Children = new[]
                                                    {
                                                        new Runtime.Node
                                                        {
                                                            Name = "topJoint",
                                                            Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                        },
                    }
                };

                var planeNode = scene.Nodes.First();
                var rootNode = scene.Nodes.ElementAt(1);
                var rootMidNode = rootNode.Children.First();
                var midNode = rootMidNode.Children.First();
                var midTopNode = midNode.Children.First();
                var TopNode = midTopNode.Children.First();

                planeNode.Skin.SkinJoints = new[]
{
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: rootNode
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,-0.2f,0,1),
                        node: rootMidNode
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,-0.4f,0,1),
                        node: midNode
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix:  new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,-0.6f,0,1),
                        node: midTopNode
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix:  new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,-0.8f,0,1),
                        node: TopNode
                    ),
                };

                var rootJoint = planeNode.Skin.SkinJoints.First();
                var rootMidJoint = planeNode.Skin.SkinJoints.ElementAt(1);
                var midJoint = planeNode.Skin.SkinJoints.ElementAt(2);
                var midTopJoint = planeNode.Skin.SkinJoints.ElementAt(3);
                var TopJoint = planeNode.Skin.SkinJoints.ElementAt(4);

                planeNode.Mesh.MeshPrimitives.First().VertexJointWeights = new[]
                {
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootMidJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootMidJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = midTopJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = midTopJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = TopJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = TopJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = TopJoint,
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = TopJoint,
                            Weight = 1,
                        },
                    },
                };

                return scene;
            }
        }
    }
}

