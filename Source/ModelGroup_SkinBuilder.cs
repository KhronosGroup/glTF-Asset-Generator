﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            public static List<Runtime.Node> CreateFoldingPlaneSkin(string skinName, int numberOfNodesInJointHierarchy, float numberOfVertexPairs, int? indexOfTransformNode = null, bool rotationJoint1 = true, float vertexVerticalSpacingMultiplier = 1.0f)
            {
                var color = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                var positions = new List<Vector3>();
                var indices = new List<int>();
                float vertexHeightOffset = 0.2f * vertexVerticalSpacingMultiplier;

                float startHeight = -((numberOfVertexPairs - 1) * vertexHeightOffset / 2);
                float positionZ = startHeight;
                int index1 = 0;
                int index2 = 1;
                int index3 = 2;

                // Create positions and indices. Pattern for indices is as follows:
                // 0, 1, 2,
                // 2, 1, 3,
                // 2, 3, 4,
                // 4, 3, 5,
                // 4, 5, 6,
                // 6, 5, 7,
                // 6, 7, 8,
                // 8, 7, 9,
                // 8, 9, 10,
                // 10, 9, 11
                for (int vertexPairIndex = 0; vertexPairIndex < numberOfVertexPairs; vertexPairIndex++)
                {
                    positions.Add(new Vector3(-0.25f, 0.0f, positionZ));
                    positions.Add(new Vector3(0.25f, 0.0f, positionZ));
                    positionZ = positionZ + vertexHeightOffset;

                    if (vertexPairIndex > 0)
                    {
                        indices.Add(index1);
                        indices.Add(index2);
                        indices.Add(index3);
                        indices.Add(index1 + 2);
                        indices.Add(index2);
                        indices.Add(index3 + 1);
                        index1 += 2;
                        index2 += 2;
                        index3 += 2;
                    }
                }

                Matrix4x4 baseRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90.0f), 0.0f);
                Quaternion jointRotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-30.0f), 0.0f));
                var translationVector = new Vector3(0.0f, 0.0f, vertexHeightOffset);
                var translationVectorJoint0 = new Vector3(0.0f, startHeight, 0.0f);
                var matrixJoint0 = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, startHeight));
                Matrix4x4.Invert(matrixJoint0, out Matrix4x4 invertedJoint0);
                Matrix4x4 invertedTranslationMatrix = Matrix4x4.CreateTranslation(-translationVector);

                var jointHierarchyNodes = new List<Runtime.Node>();
                // Create the root node, since it is a special case
                jointHierarchyNodes.Add(new Runtime.Node
                {
                    Name = "joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(baseRotation),
                    Translation = translationVectorJoint0,
                });
                // Create the child nodes in the joint hierarchy
                for (int nodeIndex = 1, jointIndex = 1; nodeIndex < numberOfNodesInJointHierarchy; nodeIndex++)
                {
                    string name;
                    if (nodeIndex == indexOfTransformNode)
                    {
                        name = "transformNode";
                    }
                    else
                    {
                        name = $"joint{jointIndex}";
                        jointIndex++;
                    }

                    Quaternion rotationToUse = Quaternion.Identity;
                    if (nodeIndex == 1 && rotationJoint1)
                    {
                        rotationToUse = jointRotation;
                    }

                    jointHierarchyNodes.Add(new Runtime.Node
                    {
                        Name = name,
                        Rotation = rotationToUse,
                        Translation = translationVector,
                    });

                    // Add as a child of the previous node
                    jointHierarchyNodes[nodeIndex - 1].Children = new[] { jointHierarchyNodes[nodeIndex] };
                }

                var skinJoints = new List<Runtime.SkinJoint>();
                // Create the skinjoint for the root node, since it is a special case 
                skinJoints.Add(new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint0,
                    node: jointHierarchyNodes[0]
                ));
                // Create the skinjoints for the rest of the joints
                for (int nodeIndex = 1, translationMultiplier = 1; nodeIndex < numberOfNodesInJointHierarchy; nodeIndex++)
                {
                    // Create the skinjoint. Transform node is skipped.
                    if (nodeIndex != indexOfTransformNode)
                    {
                        skinJoints.Add(new Runtime.SkinJoint
                        (
                            inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, -positions[nodeIndex * 2].Z)),
                            node: jointHierarchyNodes[nodeIndex]
                        ));
                    }
                    translationMultiplier--;
                }

                // Assign weights
                var weights = new List<List<Runtime.JointWeight>>();
                for (int vertexPairIndex = 0, jointIndex2 = 0; vertexPairIndex < numberOfVertexPairs; vertexPairIndex++)
                {
                    Runtime.SkinJoint jointToUse;
                    // If there is a transform node, use the joint from the node before it.
                    // Else if there are more vertex pairs than joints, then the last ones use the last joint.
                    // Otherwise, use the joint with the same index as the vertex pair.
                    if (vertexPairIndex == indexOfTransformNode)
                    {
                        jointToUse = skinJoints[jointIndex2 - 1];
                    }
                    else if (vertexPairIndex > skinJoints.Count - 1)
                    {
                        jointToUse = skinJoints[skinJoints.Count - 1];
                    }
                    else
                    {
                        jointToUse = skinJoints[jointIndex2];
                    }

                    weights.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = jointToUse,
                            Weight = 1,
                        },
                    });
                    weights.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = jointToUse,
                            Weight = 1,
                        },
                    });
                    jointIndex2++;
                }

                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin()
                    {
                        Name = skinName,
                        SkinJoints = skinJoints
                    },
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                VertexJointWeights = weights,
                                Positions = positions,
                                Indices = indices,
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true,
                                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                                    {
                                        BaseColorFactor = color
                                    }
                                }
                            }
                        }
                    },
                };

                return new List<Runtime.Node>
                {
                    nodePlane,
                    jointHierarchyNodes[0]
                };
            }
        }
    }
}
