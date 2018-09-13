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
                                            new Vector3(-0.25f,-0.5f, 0.0f),
                                            new Vector3( 0.25f,-0.5f, 0.0f),
                                            new Vector3(-0.25f, 0.0f, 0.0f),
                                            new Vector3( 0.25f, 0.0f, 0.0f),
                                            new Vector3(-0.25f, 0.5f, 0.0f),
                                            new Vector3( 0.25f, 0.5f, 0.0f),
                                        },
                                        Indices = new List<int>
                                        {
                                            0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5
                                        },
                                        Colors = new List<Vector4>()
                                        {
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
                        },
                        new Runtime.Node
                        {
                            Name = "rootNode",
                            Translation = new Vector3(0.0f, -0.5f, 0.0f),
                            Children = new[]
                            {
                                new Runtime.Node
                                {
                                    Name = "midNode",
                                    Translation = new Vector3(0.0f, 0.5f, 0.0f),
                                }
                            },
                        },
                    }
                };

                var planeNode = scene.Nodes.First();
                var rootNode = scene.Nodes.ElementAt(1);
                var midNode = rootNode.Children.First();

                planeNode.Skin.SkinJoints = new[]
                {
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,0.5f,0,1),
                        node: rootNode
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: Matrix4x4.Identity,
                        node: midNode
                    )
                };


                var rootJoint = planeNode.Skin.SkinJoints.First();
                var midJoint = planeNode.Skin.SkinJoints.ElementAt(1);

                planeNode.Mesh.MeshPrimitives.First().VertexJointWeights = new[]
                {
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = 0,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = 0,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = 0,
                        },
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
                            Joint = rootJoint,
                            Weight = 0,
                        },
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
                            Joint = rootJoint,
                            Weight = 0,
                        },
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
                            Joint = rootJoint,
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = 1,
                        },
                    },
                };

                return scene;
            }
        }
    }
}

