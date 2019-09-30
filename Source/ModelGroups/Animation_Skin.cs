using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Animation_Skin : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Skin;

        public Animation_Skin(List<string> imageList)
        {
            UseFigure(imageList, "skinA");
            UseFigure(imageList, "skinB");
            UseFigure(imageList, "skinC");
            UseFigure(imageList, "skinD");
            UseFigure(imageList, "skinE");
            UseFigure(imageList, "skinF");
            var closeCamera = new Manifest.Camera(new Vector3(0.5f, 0.0f, 0.6f));
            var distantCamera = new Manifest.Camera(new Vector3(1.5f, 0.0f, 1.0f));
            var skinBCamera = new Manifest.Camera(new Vector3(0.5f, 0.6f, 1.1f));

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Animation>, List<Node>> setProperties, Action<Model> setCamera, Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null)
            {
                var properties = new List<Property>();
                var nodes = new List<Node>();
                var animations = new List<Animation>();
                var animated = true;

                // There are no common properties in this model group.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animations, nodes);

                // If no animations are used, null out that property.
                if (!animations.Any())
                {
                    animations = null;
                    animated = false;
                }

                // Create the gltf object.
                var model = new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = nodes
                    }, animations: animations),
                    Animated = animated,
                };

                if (postRuntimeChanges != null)
                {
                    model.PostRuntimeChanges = postRuntimeChanges;
                }

                setCamera(model);

                return model;
            }

            void AddRotationAnimationChannel(List<AnimationChannel> channelList, Node targetNode, Quaternion pitchValue, Quaternion restValue)
            {
                channelList.Add(
                    new AnimationChannel
                    {
                        Target = new AnimationChannelTarget
                        {
                            Node = targetNode,
                            Path = AnimationChannelTargetPath.Rotation,
                        },
                        Sampler = new AnimationSampler
                        {
                            Interpolation = AnimationSamplerInterpolation.Linear,
                            Input = Data.Create(new[]
                            {
                                0.0f,
                                1.0f,
                                2.0f,
                            }),
                            Output = Data.Create(new[]
                            {
                                restValue,
                                pitchValue,
                                restValue,
                            }),
                        },
                    });
            }

            Animation CreateFoldingAnimation(Node jointRootNode, List<AnimationChannel> channelList = null)
            {
                if (channelList == null)
                {
                    channelList = new List<AnimationChannel>();
                }

                Node nodeCheck = jointRootNode;
                float pitchValue = FloatMath.ToRadians(-90.0f);
                var nodeList = new List<Node>
                {
                    jointRootNode,
                };

                while (nodeCheck.Children != null)
                {
                    foreach (var node in nodeCheck.Children)
                    {
                        nodeList.Add(node);
                    }
                    nodeCheck = nodeCheck.Children.First();
                }

                for (var nodeIndex = 1; nodeIndex < nodeList.Count(); nodeIndex++)
                {
                    float rotateValueModifier = 1.0f;
                    if (nodeIndex == 1)
                    {
                        rotateValueModifier = 0.5f;
                    }
                    else if (nodeIndex % 2 == 0)
                    {
                        rotateValueModifier = -1.0f;
                    }
                    AddRotationAnimationChannel(channelList, nodeList[nodeIndex], Quaternion.CreateFromYawPitchRoll(0.0f, pitchValue * rotateValueModifier, 0.0f), Quaternion.Identity);
                }
                return new Animation
                {
                    Channels = channelList
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA`."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is animating with a rotation."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    var tempNodeList = Nodes.CreateFoldingPlaneSkin("skinA", 2, 3);

                    // Give the skin node a rotation 
                    tempNodeList[0].Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4.0f), 0.0f, 0.0f);

                    // Create a new parent node and give it a rotation
                    tempNodeList[0] = new Node
                    {
                        Name = "jointParent",
                        Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4.0f), 0.0f, 0.0f),
                        Children = new List<Node>
                        {
                            tempNodeList[0]
                        }
                    };

                    foreach (Node node in tempNodeList)
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA` where the skinned node has a transform and a parent node with a transform. Both transforms should be ignored."));
                }, (model) => { model.Camera = closeCamera; }),
                // Removed Animation_Skin_03 due to a change in the spec that disallows this situation.
                // Left commented out because this will likely be re-added as a negative test in the future.
                // CreateModel((properties, animations, nodes) =>
                //{
                //     foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                //     {
                //         nodes.Add(node);
                //     }

                //     properties.Add(new Property(PropertyName.Description, "`skinA`. The skin joints are not referenced by the scene nodes."));
                // }, (model) => { model.Camera = closeCamera; }, (gltf) => {gltf.Scenes.First().Nodes = new []{0,};}),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    nodes[0].Skin.InverseBindMatrices = null;

                    properties.Add(new Property(PropertyName.Description, "`skinA` without inverse bind matrices."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    // Attach a node with a mesh to the end of the joint hierarchy 
                    Node nodeCheck = nodes[1];
                    while (nodeCheck.Children != null)
                    {
                        nodeCheck = nodeCheck.Children.First();
                    }

                    nodeCheck.Children = new List<Node>
                    {
                        new Node
                        {
                            Mesh = Mesh.CreateTriangle()
                        }
                    };

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is animated with a rotation and `joint1` has a triangle mesh attached to it."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    // Create a set of positions for the second mesh that are offset from the first mesh.
                    Runtime.MeshPrimitive originalMeshPrimitive = nodes[0].Mesh.MeshPrimitives.First();
                    var offsetPositions = new List<Vector3>();
                    foreach (Vector3 position in originalMeshPrimitive.Positions.Values)
                    {
                        var offsetPosition = position;
                        offsetPosition.X += 0.6f;
                        offsetPositions.Add(offsetPosition);
                    }

                    // Create a second mesh
                    nodes.Add(new Node
                    {
                        Name = "plane2",
                        Skin = nodes[0].Skin,
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new[]
                            {
                                new Runtime.MeshPrimitive
                                {
                                    Joints = originalMeshPrimitive.Joints,
                                    Weights = originalMeshPrimitive.Weights,
                                    Positions = Data.Create(offsetPositions),
                                    Indices = originalMeshPrimitive.Indices,
                                    Material = new Runtime.Material
                                    {
                                        DoubleSided = true,
                                        PbrMetallicRoughness = new PbrMetallicRoughness
                                        {
                                            BaseColorFactor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                                        }
                                    }
                                }
                            }
                        }
                    });

                    properties.Add(new Property(PropertyName.Description, "`skinA` where there are two meshes sharing a single skin."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    // Make joint1 a root joint
                    nodes.Add(nodes[1].Children.First());
                    nodes[1].Children = null;

                    // Compensate for no longer inheriting from joint0
                    nodes[2].Rotation = Quaternion.Multiply((Quaternion)nodes[2].Rotation, (Quaternion)nodes[1].Rotation);
                    nodes[2].Translation = null;
                    nodes[0].Skin.InverseBindMatrices = Data.Create(new[]
                    {
                        nodes[0].Skin.InverseBindMatrices.Values.First(),
                        Matrix4x4.Identity
                    });

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is a root node and not a child of `joint0`."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreatePlaneWithSkinB())
                    {
                        nodes.Add(node);
                    }

                    // Animate the joints
                    Node nodeJoint0 = nodes[1];
                    Node nodeJoint1 = nodeJoint0.Children.First();
                    var channelList = new List<AnimationChannel>();
                    float rotationValue = FloatMath.ToRadians(-15.0f);
                    AddRotationAnimationChannel(channelList, nodeJoint1, Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, rotationValue), Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, 0.0f));
                    animations.Add(new Animation
                    {
                        Channels = channelList
                    });

                    properties.Add(new Property(PropertyName.Description, "`skinB` which is made up of two skins. `joint1` is referenced by both skins and is animating with a rotation."));
                }, (model) => { model.Camera = skinBCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinC", 5, 5))
                    {
                        nodes.Add(node);
                    }
                    
                    // Rotate each joint node, except the root which already has the desired rotation
                    Node nodeCheck = nodes[1].Children.First();
                    float rotationRadian = FloatMath.ToRadians(-10.0f);
                    Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0.0f, rotationRadian, 0.0f);
                    nodeCheck.Rotation = rotation;
                    while (nodeCheck.Children != null)
                    {
                        foreach (var node in nodeCheck.Children)
                        {
                            node.Rotation = rotation;
                        }
                        nodeCheck = nodeCheck.Children.First();
                    }

                    // Rebuild the inverseBindMatrix for each joint (except the root) to work with the new rotation
                    var inverseBindMatrixList = nodes[0].Skin.InverseBindMatrices.Values.Select((value, index) =>
                    {
                        Matrix4x4.Invert(Matrix4x4.CreateRotationX(rotationRadian * (index + 1)), out Matrix4x4 invertedRotation);
                        return Matrix4x4.Multiply(value, invertedRotation);
                    });

                    properties.Add(new Property(PropertyName.Description, "`skinC` where all of the joints have a local rotation of -10 degrees, except the root which is rotated -90 degrees."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreateFoldingPlaneSkin("skinD", 5, 6, 3, false))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    // Remove animation for the transform node
                    animations[0].Channels = new List<AnimationChannel>
                    {
                        animations[0].Channels.First(),
                        animations[0].Channels.ElementAt(1),
                        animations[0].Channels.ElementAt(3),
                    };

                    // Add the mesh to the transform node
                    nodes[1].Children.First().Children.First().Children.First().Mesh = Mesh.CreateTriangle();

                    properties.Add(new Property(PropertyName.Description, "`skinD` where each joint is animating with a rotation. There is a transform node in the joint hierarchy that is not a joint. That node has a mesh attached to it in order to show its location."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    foreach (Node node in Nodes.CreatePlaneWithSkinE())
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinE`."));
                }, (model) => { model.Camera = distantCamera; }),
                // Removing this model for now, since no viewer currently supports models that have >4 jointweights per vertex.
                //CreateModel((properties, animations, nodes) =>
                //{
                //    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinF", 8, 9, vertexVerticalSpacingMultiplier: 0.5f))
                //    {
                //        nodes.Add(node);
                //    }

                //    // Rotate each joint node, except the root which already has the desired rotation
                //    Runtime.Node nodeCheck = nodes[1].Children.First();
                //    float rotationRadian = FloatMath.ConvertDegreesToRadians(-10.0f);
                //    Quaternion rotationQuaternion = Quaternion.CreateFromYawPitchRoll(0.0f, rotationRadian, 0.0f);
                //    nodeCheck.Rotation = rotationQuaternion;
                //    while (nodeCheck.Children != null)
                //    {
                //        foreach (Runtime.Node node in nodeCheck.Children)
                //        {
                //            node.Rotation = rotationQuaternion;
                //        }
                //        nodeCheck = nodeCheck.Children.First();
                //    }

                //    // Rebuild the inverseBindMatrix for each joint (except the root) to work with the new rotation
                //    var skinJointList = (List<Runtime.SkinJoint>)nodes[0].Skin.SkinJoints;
                //    for (var skinJointIndex = 1; skinJointIndex < skinJointList.Count(); skinJointIndex++)
                //    {
                //        Matrix4x4 translationInverseBindMatrix = skinJointList.ElementAt(skinJointIndex).InverseBindMatrix;
                //        Matrix4x4.Invert(Matrix4x4.CreateRotationX(rotationRadian * (skinJointIndex + 1)) , out Matrix4x4 invertedRotation);
                //        skinJointList.ElementAt(skinJointIndex).InverseBindMatrix = Matrix4x4.Multiply(translationInverseBindMatrix, invertedRotation);
                //    }

                //    // Rebuild weights to include every joint instead of just the ones with a weight > 0
                //    var weightList = (List<List<Runtime.JointWeight>>)nodes[0].Mesh.MeshPrimitives.First().VertexJointWeights;
                //    for (var weightIndex = 0; weightIndex < weightList.Count(); weightIndex++)
                //    {
                //        var jointWeight = new List<Runtime.JointWeight>();

                //        for (var skinJointIndex = 0; skinJointIndex < skinJointList.Count; skinJointIndex++)
                //        {
                //            int weightToUse = 0;
                //            // Set the weight to 1 if the skinJoint is at the same level as the vertex.
                //            // Or Set the weight to 1 if the vertex is further out than the last skinjoint and the last skinjoint is being set.
                //            if (skinJointIndex == (weightIndex / 2) || (((weightIndex / 2) > skinJointList.Count - 1) && (skinJointIndex == skinJointList.Count - 1)) )
                //            {
                //                weightToUse = 1;
                //            }

                //            jointWeight.Add(new Runtime.JointWeight
                //            {
                //                Joint = skinJointList[skinJointIndex],
                //                Weight = weightToUse,
                //            });
                //        }

                //        weightList[weightIndex] = jointWeight;
                //    }

                //    properties.Add(new Property(PropertyName.Description, "`skinF`. Each vertex has weights for more than four joints."));
                //}, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, animations, nodes) =>
                {
                    var skinA1 = Nodes.CreateFoldingPlaneSkin("skinA", 2, 3);
                    var skinA2 = Nodes.CreateFoldingPlaneSkin("skinA", 2, 3);

                    // Set the same mesh on both nodes.
                    skinA2[0].Mesh = skinA1[0].Mesh;

                    // Offset one of the models so they aren't overlapping.
                    Vector3 translation = skinA2[1].Translation.Value;
                    skinA2[1].Translation = new Vector3(translation.X + 0.6f, translation.Y, translation.Z);

                    foreach (Node node in skinA1)
                    {
                        nodes.Add(node);
                    }

                    foreach (Node node in skinA2)
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "Two instances of `skinA` sharing a mesh but with separate skins."));
                }, (model) => { model.Camera = distantCamera; }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
