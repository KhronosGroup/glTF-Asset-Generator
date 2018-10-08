﻿using System.Collections.Generic;
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
                var color = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin()
                    {
                        Name = "skinC",
                    },
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                Positions = new List<Vector3>()
                                {
                                    new Vector3(-0.25f, 0.0f,-0.4f),
                                    new Vector3( 0.25f, 0.0f,-0.4f),
                                    new Vector3(-0.25f, 0.0f,-0.2f),
                                    new Vector3( 0.25f, 0.0f,-0.2f),
                                    new Vector3(-0.25f, 0.0f, 0.0f),
                                    new Vector3( 0.25f, 0.0f, 0.0f),
                                    new Vector3(-0.25f, 0.0f, 0.2f),
                                    new Vector3( 0.25f, 0.0f, 0.2f),
                                    new Vector3(-0.25f, 0.0f, 0.4f),
                                    new Vector3( 0.25f, 0.0f, 0.4f),
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
                                },
                                Colors = new List<Vector4>()
                                {
                                    color,
                                    color,
                                    color,
                                    color,
                                    color,
                                    color,
                                    color,
                                    color,
                                    color,
                                    color,
                                },
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true
                                }
                            }
                        }
                    },
                };

                Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(FloatMath.ConvertDegreesToRadians(60.0f), FloatMath.ConvertDegreesToRadians(-90.0f), 0.0f);
                var translationValue = 0.2f;
                var translationVector = new Vector3(0.0f, 0.0f, translationValue);
                var translationVectorJoint0 = new Vector3(0.0f, -2 * translationValue, 0.0f);
                var matrixJoint0 = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, -2 * translationValue));
                Matrix4x4 invertedJoint0;
                Matrix4x4.Invert(matrixJoint0, out invertedJoint0);
                Matrix4x4 invertedTranslationMatrix = Matrix4x4.CreateTranslation(-translationVector);

                var nodeJoint4 = new Runtime.Node
                {
                    Name = "joint4",
                    Translation = translationVector,
                };
                var nodeJoint3 = new Runtime.Node
                {
                    Name = "joint3",
                    Translation = translationVector,
                    Children = new[]
                    {
                        nodeJoint4
                    }
                };
                var nodeJoint2 = new Runtime.Node
                {
                    Name = "joint2",
                    Translation = translationVector,
                    Children = new[]
                    {
                        nodeJoint3
                    }
                };
                var nodeJoint1 = new Runtime.Node
                {
                    Name = "joint1",
                    Translation = translationVector,
                    Children = new[]
                    {
                        nodeJoint2
                    }
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(rotation),
                    Translation = translationVectorJoint0,
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var joint4 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(-2 * translationVector),
                    node: nodeJoint4
                );
                var joint3 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(-translationVector),
                    node: nodeJoint3
                );
                var joint2 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.Identity,
                    node: nodeJoint2
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(translationVector),
                    node: nodeJoint1
                );
                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint0,
                    node: nodeJoint0
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

                // Assign joint weights to pairs of vertexs
                var weightsList = new List<List<Runtime.JointWeight>>();
                int jointIndex = 0;
                for (int vertexIndex = 0; vertexIndex < 10; vertexIndex++)
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
