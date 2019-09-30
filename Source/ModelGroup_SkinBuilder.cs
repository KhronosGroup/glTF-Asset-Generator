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
                var vertexHeightOffset = 0.2f * vertexVerticalSpacingMultiplier;

                float startHeight = -((numberOfVertexPairs - 1) * vertexHeightOffset / 2);
                float positionZ = startHeight;
                var index1 = 0;
                var index2 = 1;
                var index3 = 2;

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
                for (var vertexPairIndex = 0; vertexPairIndex < numberOfVertexPairs; vertexPairIndex++)
                {
                    positions.Add(new Vector3(-0.25f, 0.0f, positionZ));
                    positions.Add(new Vector3(0.25f, 0.0f, positionZ));
                    positionZ += vertexHeightOffset;

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

                Matrix4x4 baseRotation = Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f);
                Quaternion jointRotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-30.0f), 0.0f));
                var translationVector = new Vector3(0.0f, 0.0f, vertexHeightOffset);
                var translationVectorJoint0 = new Vector3(0.0f, startHeight, 0.0f);
                var matrixJoint0 = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, startHeight));
                Matrix4x4.Invert(matrixJoint0, out Matrix4x4 invertedJoint0);

                var jointNodes = new List<Runtime.Node>();
                // Create the root node, since it is a special case.
                jointNodes.Add(new Runtime.Node
                {
                    Name = "joint0",
                    Rotation = Quaternion.CreateFromRotationMatrix(baseRotation),
                    Translation = translationVectorJoint0,
                });
                // Create the child nodes in the joint hierarchy.
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

                    jointNodes.Add(new Runtime.Node
                    {
                        Name = name,
                        Rotation = rotationToUse,
                        Translation = translationVector,
                    });

                    // Add as a child of the previous node.
                    jointNodes[nodeIndex - 1].Children = new[] { jointNodes[nodeIndex] };
                }

                // Assembles the joints and inverseBindMatrices to create the skin.
                var skinJoints = new List<Runtime.Node>
                {
                    jointNodes[0]
                };
                var inverseBindMatrices = new List<Matrix4x4>
                {
                    invertedJoint0
                };
                for (int nodeIndex = 1, translationMultiplier = 1; nodeIndex < numberOfNodesInJointHierarchy; nodeIndex++)
                {
                    if (nodeIndex != indexOfTransformNode)
                    {
                        skinJoints.Add(jointNodes[nodeIndex]);
                        inverseBindMatrices.Add(Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, -positions[nodeIndex * 2].Z)));
                    }
                    translationMultiplier--;
                }

                var joints = new List<Runtime.JointVector>();
                var weights = new List<Runtime.WeightVector>();
                for (int i = 0; i < numberOfVertexPairs; i++)
                {
                    // If there is a transform node, use the joint from the node before it.
                    // Else if there are more vertex pairs than joints, then the last ones use the last joint.
                    // Otherwise, use the joint with the same index as the vertex pair.
                    int joint = i;
                    if (i == indexOfTransformNode)
                    {
                        joint = joint - 1;
                    }
                    else if (i > skinJoints.Count - 1)
                    {
                        joint = skinJoints.Count - 1;
                    }

                    joints.AddRange(new[]
                    {
                        new Runtime.JointVector(joint),
                        new Runtime.JointVector(joint),
                    });

                    weights.AddRange(new[]
                    {
                        new Runtime.WeightVector(1.0f),
                        new Runtime.WeightVector(1.0f),
                    });
                }

                var nodePlane = new Runtime.Node
                {
                    Name = "plane",
                    Skin = new Runtime.Skin
                    {
                        Name = skinName,
                        Joints = skinJoints,
                        InverseBindMatrices = Runtime.Data.Create(inverseBindMatrices)
                    },
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            new Runtime.MeshPrimitive
                            {
                                Joints = Runtime.Data.Create(joints, Runtime.DataType.UnsignedShort),
                                Weights = Runtime.Data.Create(weights),
                                Positions = Runtime.Data.Create(positions),
                                Indices = Runtime.Data.Create(indices),
                                Material = new Runtime.Material
                                {
                                    DoubleSided = true,
                                    PbrMetallicRoughness = new Runtime.PbrMetallicRoughness
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
                    jointNodes[0]
                };
            }

            public static Vector2[] GetSkinATexCoords()
            {
                return new[]
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 0.5f),
                    new Vector2(1.0f, 0.5f),
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f),
                };
            }
        }
    }
}
