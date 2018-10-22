using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Nodes
        {
            // Add two to each index after calling this function.
            // Pattern is as follows
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
            private static void AddIndicesForTrianglePair(List<int> indicesList, int index1, int index2, int index3)
            {
                indicesList.Add(index1);
                indicesList.Add(index2);
                indicesList.Add(index3);
                indicesList.Add(index1 + 2);
                indicesList.Add(index2);
                indicesList.Add(index3 + 1);
            }

            public static List<Runtime.Node> CreateFoldingPlaneSkin(string skinName, int numberOfNodesInJointHierarchy, float numberOfVertexPairs, int indexOfTransformNode = -1, bool rotateFirstJoint = true)
            {
                var colors = new List<Vector4>();
                var positions = new List<Vector3>();
                var indices = new List<int>();

                var startDepth = -(numberOfVertexPairs - 1) / 10;
                var positionZ = startDepth;
                int index1 = 0;
                int index2 = 1;
                int index3 = 2;

                // Create colors, positions, and indices
                for (int vertexPairIndex = 0; vertexPairIndex < numberOfVertexPairs; vertexPairIndex++)
                {
                    colors.Add(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
                    colors.Add(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

                    positions.Add(new Vector3(-0.25f, 0.0f, positionZ));
                    positions.Add(new Vector3(0.25f, 0.0f, positionZ));
                    positionZ = positionZ + 0.2f;

                    if (vertexPairIndex > 0)
                    {
                        AddIndicesForTrianglePair(indices, index1, index2, index3);
                        index1 += 2;
                        index2 += 2;
                        index3 += 2;
                    }
                }

                Matrix4x4 baseRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90.0f), 0.0f);
                Quaternion jointRotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-30.0f), 0.0f));
                var translationValue = 0.2f;
                var translationVector = new Vector3(0.0f, 0.0f, translationValue);
                var translationVectorJoint0 = new Vector3(0.0f, startDepth, 0.0f);
                var matrixJoint0 = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, startDepth));
                Matrix4x4 invertedJoint0;
                Matrix4x4.Invert(matrixJoint0, out invertedJoint0);
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
                for (int nodeIndex = 1; nodeIndex < numberOfNodesInJointHierarchy; nodeIndex++)
                {
                    string name;
                    if (nodeIndex == indexOfTransformNode)
                    {
                        name = "transformNode";
                    }
                    else if (indexOfTransformNode != -1 && nodeIndex > indexOfTransformNode)
                    {
                        name = String.Concat("joint", (nodeIndex - 1).ToString());
                    }
                    else
                    {
                        name = String.Concat("joint", nodeIndex.ToString());
                    }

                    Quaternion rotationToUse = Quaternion.Identity;
                    if (nodeIndex == 1 && rotateFirstJoint == true)
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

                var skinJointsList = new List<Runtime.SkinJoint>();
                // Create the skinjoint for the root node, since it is a special case 
                skinJointsList.Add(new Runtime.SkinJoint
                (
                    inverseBindMatrix: invertedJoint0,
                    node: jointHierarchyNodes[0]
                ));
                // Create the skinjoints for the rest of the joints
                int translationMultiplier = 1;
                for (int nodeIndex = 1; nodeIndex < numberOfNodesInJointHierarchy; nodeIndex++)
                {
                    // Create the skinjoint. Transform node is skipped.
                    if (nodeIndex != indexOfTransformNode)
                    {
                        skinJointsList.Add(new Runtime.SkinJoint
                        (
                            inverseBindMatrix: Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, -positions[nodeIndex * 2].Z)),
                            node: jointHierarchyNodes[nodeIndex]
                        ));
                    }
                    translationMultiplier--;
                }

                // Assign weights
                var weightsList = new List<List<Runtime.JointWeight>>();
                int jointIndex = 0;
                var numberOfVertexes = numberOfVertexPairs * 2;
                for (int vertexPairIndex = 0; vertexPairIndex < numberOfVertexPairs; vertexPairIndex++)
                {
                    Runtime.SkinJoint jointToUse;
                    // If there is a transform node, use the joint from the node before it.
                    // Else if there are more vertex pairs than joints, then the last ones use the last joint.
                    // Otherwise, use the joint with the same index as the vertex pair.
                    if (vertexPairIndex == indexOfTransformNode)
                    {
                        jointToUse = skinJointsList[jointIndex - 1];
                    }
                    else if (vertexPairIndex > skinJointsList.Count - 1)
                    {
                        jointToUse = skinJointsList[skinJointsList.Count - 1];
                    }
                    else
                    {
                        jointToUse = skinJointsList[jointIndex];
                    }

                    weightsList.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = jointToUse,
                            Weight = 1,
                        },
                    });
                    weightsList.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = jointToUse,
                            Weight = 1,
                        },
                    });
                    jointIndex++;
                }

                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin()
                    {
                        Name = skinName,
                        SkinJoints = skinJointsList
                    },
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                VertexJointWeights = weightsList,
                                Positions = positions,
                                Indices = indices,
                                Colors = colors,
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true
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
