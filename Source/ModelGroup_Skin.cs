using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Scene
        {
            public static Runtime.Scene CreatePlaneWithSkin()
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
                                            new Vector3(-0.5f,-0.5f, 0.0f),
                                            new Vector3( 0.5f,-0.5f, 0.0f),
                                            new Vector3(-0.5f, 0.0f, 0.0f),
                                            new Vector3( 0.5f, 0.0f, 0.0f),
                                            new Vector3(-0.5f, 0.5f, 0.0f),
                                            new Vector3( 0.5f, 0.5f, 0.0f),
                                        },
                                        Normals = new List<Vector3>()
                                        {
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                        },
                                        Indices = new List<int>
                                        {
                                            0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5
                                        },
                                        Colors = new List<Vector4>()
                                        {
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
                            Children = new[]
                            {
                                new Runtime.Node
                                {
                                    Name = "midJoint",
                                }
                            },
                        },
                    }
                };

                var planeNode = scene.Nodes.First();
                var rootJoint = scene.Nodes.ElementAt(1);
                var midJoint = rootJoint.Children.First();

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
                        node: midJoint
                    )
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
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = planeNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                };

                return scene;
            }
        }
    }
}

