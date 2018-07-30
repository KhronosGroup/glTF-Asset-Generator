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
                                            0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7, 6, 7, 8, 8, 7, 9, 8, 9, 10, 10, 9 ,11
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
                                    }
                                }
                            },
                        },
                        new Runtime.Node
                        {
                            Name = "rootJoint",
                            //Rotation = new Quaternion(0.0f, 0.0f, 0.707106769f, 0.707106769f),
                            Children = new[]
                            {
                                new Runtime.Node
                                {
                                    Name = "rootMidJoint",
                                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                    //Rotation = new Quaternion(0.9995769f, 0.0f, 0.0290847216f, 0.0f),
                                    Children = new[]
                                    {
                                        new Runtime.Node
                                        {
                                            Name = "midJoint",
                                            Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                            //Rotation = new Quaternion(0.9983082f, 0.0f, 0.05814483f, 0.0f),
                                            Children = new[]
                                            {
                                                new Runtime.Node
                                                {
                                                    Name = "midTopJoint",
                                                    Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                                    //Rotation = new Quaternion(0.0f, -0.0581448376f, 0.0f, 0.9983082f),
                                                    Children = new[]
                                                    {
                                                        new Runtime.Node
                                                        {
                                                            Name = "topJoint",
                                                            Translation = new Vector3(0.0f, 0.2f, 0.0f),
                                                            //Rotation = new Quaternion(-0.04111461f, 0.04111461f, -0.705910563f, 0.7059104f),
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
                var rootJoint = scene.Nodes.ElementAt(1);
                var rootMidJoint = rootJoint.Children.First();
                var midJoint = rootMidJoint.Children.First();
                var midTopJoint = midJoint.Children.First();
                var TopJoint = midTopJoint.Children.First();

                planeNode.Skin.SkinJoints = new[]
                {
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: rootJoint
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: rootMidJoint
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: midJoint
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: midTopJoint
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: TopJoint
                    ),
                };

                planeNode.Mesh.MeshPrimitives.First().VertexJointWeights = new[]
                {
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.94865f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.05135f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.94865f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.05135f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.90232f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.04884f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.04884f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.90232f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.04884f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.04884f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.58700f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.38122f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.03177f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.58700f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.38122f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.03177f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.60627f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.39373f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0.60627f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.39373f,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                    },
                };

                return scene;
            }
        }
    }
}

