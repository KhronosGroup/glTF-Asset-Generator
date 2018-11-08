using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            internal static List<Runtime.Node> CreatePlaneWithSkinE(bool jointsHaveCommonParent = true)
            {
                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin()
                    {
                        Name = "skinE",
                    },
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                Positions = new List<Vector3>()
                                {
                                    // Trunk
                                    new Vector3(-0.125f, 0.0f,-0.25f),
                                    new Vector3( 0.125f, 0.0f,-0.25f),
                                    new Vector3(-0.125f, 0.0f, 0.00f),
                                    new Vector3( 0.125f, 0.0f, 0.00f),

                                    // Root of V split
                                    new Vector3( 0.0f, 0.0f, 0.0f),

                                    // Left branch
                                    new Vector3(-0.250f, 0.0f, 0.25f),
                                    new Vector3(-0.125f, 0.0f, 0.25f),
                                    new Vector3(-0.375f, 0.0f, 0.50f),
                                    new Vector3(-0.250f, 0.0f, 0.50f),

                                    // Right branch
                                    new Vector3( 0.125f, 0.0f, 0.25f),
                                    new Vector3( 0.250f, 0.0f, 0.25f),
                                    new Vector3( 0.250f, 0.0f, 0.50f),
                                    new Vector3( 0.375f, 0.0f, 0.50f),
                                },
                                Indices = new List<int>
                                {
                                    0, 1, 2,
                                    2, 1, 3,
                                    2, 4, 5,
                                    5, 4, 6,
                                    5, 6, 7,
                                    7, 6, 8,
                                    4, 3, 9,
                                    9, 3, 10,
                                    9, 10, 11,
                                    11, 10, 12
                                },
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true,
                                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                                    {
                                        BaseColorFactor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f)
                                    }
                                }
                            }
                        }
                    },
                };

                // Two different ways to setup the joints and weights, where either there is only one parent or there are two parents that have the same child.
                if (jointsHaveCommonParent)
                {
                    return CreateJointsAndWeightsForCommonRoot(nodePlane);
                }
                else
                {
                    return CreateJointsAndWeightsForMultipleRoots(nodePlane);
                }
            }

            private static List<Runtime.Node> CreateJointsAndWeightsForCommonRoot(Runtime.Node nodePlane)
            {
                Matrix4x4 baseRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90.0f), 0.0f);
                Matrix4x4 jointRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-15.0f), 0.0f);
                var translationVectorJoint3 = new Vector3(0.1875f, 0.0f, 0.25f);
                var translationVectorJoint2 = new Vector3(-0.1875f, 0.0f, 0.25f);
                var translationVectorJoint1 = new Vector3(0.0f, 0.0f, 0.25f);
                var translationVectorJoint0 = new Vector3(0.0f, -0.25f, 0.0f);
                Matrix4x4 invertedTranslationMatrixJoint3 = Matrix4x4.CreateTranslation(-translationVectorJoint3);
                Matrix4x4 invertedTranslationMatrixJoint2 = Matrix4x4.CreateTranslation(-translationVectorJoint2);

                var matrixJoint1 = jointRotation;
                Matrix4x4.Invert(matrixJoint1, out Matrix4x4 invertedJoint1);

                var matrixJoint0 = Matrix4x4.CreateTranslation(new Vector3(0, 0.0f, -0.25f));
                Matrix4x4.Invert(matrixJoint0, out Matrix4x4 invertedJoint0);

                var nodeJoint3 = new Runtime.Node
                {
                    Name = "joint3",
                    Rotation = Quaternion.CreateFromRotationMatrix(jointRotation),
                    Translation = translationVectorJoint3,
                };
                var nodeJoint2 = new Runtime.Node
                {
                    Name = "joint2",
                    Rotation = Quaternion.CreateFromRotationMatrix(jointRotation),
                    Translation = translationVectorJoint2,
                };
                var nodeJoint1 = new Runtime.Node
                {
                    Name = "joint1",
                    Rotation = Quaternion.CreateFromRotationMatrix(jointRotation),
                    Translation = translationVectorJoint1,
                    Children = new[]
                    {
                        nodeJoint2,
                        nodeJoint3
                    }
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(baseRotation),
                    Translation = translationVectorJoint0,
                    Children = new[]
                    {
                        nodeJoint1
                    }
                };

                var joint3 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedTranslationMatrixJoint3,
                    node: nodeJoint3
                );
                var joint2 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedTranslationMatrixJoint2,
                    node: nodeJoint2
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint1,
                    node: nodeJoint1
                );
                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint0,
                    node: nodeJoint0
                );

                nodePlane.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                    joint2,
                    joint3
                };

                // Top four vertexes of each arm have a weight for the relevant joint. Otherwise the vertex has a weight from the root
                var jointWeights = new List<List<Runtime.JointWeight>>();
                // Base of trunk
                for (int vertexIndex = 0; vertexIndex < 2; vertexIndex++)
                {
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
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 0,
                        }
                    });
                }
                // Top of trunk
                for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                {
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
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 0,
                        }
                    });
                }
                // Left arm
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {

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
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 0,
                        }
                    });
                }
                // Right arm
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
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
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 1,
                        }
                    });
                }
                nodePlane.Mesh.MeshPrimitives.First().VertexJointWeights = jointWeights;

                return new List<Runtime.Node>
                {
                    nodePlane,
                    nodeJoint0
                };
            }

            private static List<Runtime.Node> CreateJointsAndWeightsForMultipleRoots(Runtime.Node nodePlane)
            {
                Matrix4x4 baseRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90.0f), 0.0f);
                Matrix4x4 jointRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-15.0f), 0.0f);
                var translationVectorJoint3 = new Vector3(0.0f, 0.25f, 0.0f);
                var translationVectorJoint2 = new Vector3(0.0f, 0.0f, 0.25f);
                var translationVectorJoint1 = new Vector3(0.1875f, -0.25f, 0.0f);
                var translationVectorJoint0 = new Vector3(-0.1875f, -0.25f, 0.0f);
                Matrix4x4 invertedJoint3 = Matrix4x4.CreateTranslation(-translationVectorJoint3);
                Matrix4x4 invertedJoint2 = Matrix4x4.CreateTranslation(-translationVectorJoint2);
                Matrix4x4 invertedJoint1 = Matrix4x4.CreateTranslation(new Vector3(-0.1875f, 0.0f, 0.25f));
                Matrix4x4 invertedJoint0 = Matrix4x4.CreateTranslation(new Vector3(0.1875f, 0.0f, 0.25f));

                var nodeJoint3 = new Runtime.Node
                {
                    Name = "joint3",
                    Rotation = Quaternion.CreateFromRotationMatrix(jointRotation),
                    Translation = translationVectorJoint3,
                };
                var nodeJoint2 = new Runtime.Node
                {
                    Name = "joint2",
                    Rotation = Quaternion.CreateFromRotationMatrix(jointRotation),
                    Translation = translationVectorJoint2,
                    Children = new[]
                    {
                        nodeJoint3
                    }
                };
                var nodeJoint1 = new Runtime.Node
                {
                    Name = "joint1",
                    Rotation = Quaternion.CreateFromRotationMatrix(baseRotation),
                    Translation = translationVectorJoint1,
                    Children = new[]
                    {
                        nodeJoint2
                    }
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(baseRotation),
                    Translation = translationVectorJoint0,
                    Children = new[]
                    {
                        nodeJoint2
                    }
                };

                var joint3 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint3,
                    node: nodeJoint3
                );
                var joint2 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint2,
                    node: nodeJoint2
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint1,
                    node: nodeJoint1
                );
                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint0,
                    node: nodeJoint0
                );

                nodePlane.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                    joint2,
                    joint3
                };

                // Top four vertexes of each arm have a weight for the relevant joint. Otherwise the vertex has a weight from the root
                var jointWeights = new List<List<Runtime.JointWeight>>();
                // Base of trunk
                for (int vertexIndex = 0; vertexIndex < 2; vertexIndex++)
                {
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
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 1,
                        }
                    });
                }
                // Top of trunk
                for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                {
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
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 0,
                        }
                    });
                }
                // Left arm
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {

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
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 0,
                        }
                    });
                }
                // Right arm
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
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
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint3,
                            Weight = 0,
                        }
                    });
                }
                nodePlane.Mesh.MeshPrimitives.First().VertexJointWeights = jointWeights;

                return new List<Runtime.Node>
                {
                    nodePlane,
                    nodeJoint0,
                    nodeJoint1
                };
            }
        }
    }
}
