﻿using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreatePlaneWithSkinB()
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
                                    new Vector3(-0.25f,-0.5f, 0.0f),
                                    new Vector3( 0.25f,-0.5f, 0.0f),
                                    new Vector3(-0.25f, 0.0f, 0.0f),
                                    new Vector3( 0.25f, 0.0f, 0.0f),
                                    new Vector3(-0.25f, 0.5f, 0.0f),
                                    new Vector3( 0.25f, 0.5f, 0.0f),
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

                var nodeTriangle = CreateTriangle()[0];
                nodeTriangle.Skin = new Runtime.Skin();

                var nodeJoint2 = new Runtime.Node
                {
                    Name = "Joint2",
                    Translation = new Vector3(0.0f, 0.5f, 0.0f),
                };
                var nodeJoint1 = new Runtime.Node
                {
                    Name = "Joint1",
                    Translation = new Vector3(0.0f, 0.5f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint2
                    },
                };
                var nodeJoint0 = new Runtime.Node
                {
                    Name = "Joint0",
                    Translation = new Vector3(0.0f, -0.5f, 0.0f),
                    Children = new[]
                    {
                        nodeJoint1
                    },
                };

                var joint0 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.5f, 0.0f)),
                    node: nodeJoint0
                );
                var joint1 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.Identity,
                    node: nodeJoint1
                );
                var joint2 = new Runtime.SkinJoint
                (
                    inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, -0.5f, 0.0f)),
                    node: nodeJoint2
                );
                nodePlane.Skin.SkinJoints = new[]
                {
                    joint0,
                    joint1,
                };
                nodeTriangle.Skin.SkinJoints = new[]
                {
                    joint1,
                    joint2
                };

                var weightsListPlane = new List<List<Runtime.JointWeight>>();
                for (int vertexIndex = 0; vertexIndex < 2; vertexIndex++)
                {
                    weightsListPlane.Add(new List<Runtime.JointWeight>()
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
                    weightsListPlane.Add(new List<Runtime.JointWeight>()
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
                nodePlane.Mesh.MeshPrimitives.First().VertexJointWeights = weightsListPlane;

                var weightsListTriangle = new List<List<Runtime.JointWeight>>();
                for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                {
                    weightsListTriangle.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = joint1,
                            Weight = 0.5f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = joint2,
                            Weight = 0.5f,
                        },
                    });
                }
                nodeTriangle.Mesh.MeshPrimitives.First().VertexJointWeights = weightsListTriangle;

                return new List<Runtime.Node>
                {
                    nodePlane,
                    nodeJoint0,
                    nodeTriangle
                };
            }
        }
    }
}
