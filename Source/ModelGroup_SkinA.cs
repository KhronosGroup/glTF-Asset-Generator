﻿using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreatePlaneWithSkinA()
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
                                    new Vector3(-0.25f, 0.0f,-0.5f),
                                    new Vector3( 0.25f, 0.0f,-0.5f),
                                    new Vector3(-0.25f, 0.0f, 0.0f),
                                    new Vector3( 0.25f, 0.0f, 0.0f),
                                    new Vector3(-0.25f, 0.0f, 0.5f),
                                    new Vector3( 0.25f, 0.0f, 0.5f),
                                },
                                Indices = new List<int>
                                {
                                    0, 1, 2,
                                    2, 1, 3,
                                    2, 3, 4,
                                    4, 3, 5
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
                };

                Matrix4x4 rotation = Matrix4x4.CreateRotationX(-FloatMath.Pi / 2);
                var translationValue = 0.5f;
                var translationVector = new Vector3(0.0f, 0.0f, translationValue);
                var translationVectorJoint0 = new Vector3(0.0f, -translationValue, 0.0f);
                var translationMatrix = Matrix4x4.CreateTranslation(translationVector);
                var matrixJoint0 = Matrix4x4.Multiply(rotation, Matrix4x4.CreateTranslation(new Vector3(0, 0.0f, -translationValue)));
                Matrix4x4 invertedJoint0;
                Matrix4x4.Invert(matrixJoint0, out invertedJoint0);

                var nodeJoint1 = new Runtime.Node
                {
                    Name = "Joint1",
                    Translation = translationVector,
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "Joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(rotation),
                    Translation = translationVectorJoint0,
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.Identity,
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
                    joint1
                };

                var weightsList = new List<List<Runtime.JointWeight>>();
                for (int vertexIndex = 0; vertexIndex < 2; vertexIndex++)
                {
                    weightsList.Add(new List<Runtime.JointWeight>()
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
                    });
                }
                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
                    weightsList.Add(new List<Runtime.JointWeight>()
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
