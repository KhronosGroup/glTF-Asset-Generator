using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
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
            var midCamera = new Manifest.Camera(new Vector3(0.8f, 0.0f, 1.0f));
            var distantCamera = new Manifest.Camera(new Vector3(1.5f, 0.0f, 1.0f));
            var skinBCamera = new Manifest.Camera(new Vector3(0.5f, 0.6f, 1.1f));

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Animation>, List<Runtime.Node>> setProperties, Action<Model> setCamera, Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null)
            {
                var properties = new List<Property>();
                var nodes = new List<Runtime.Node>();
                var animations = new List<Runtime.Animation>();

                // There are no common properties in this model group.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animations, nodes);

                // If no animations are used, null out that property.
                if (!animations.Any())
                {
                    animations = null;
                }

                // Create the gltf object
                var model = new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene()
                    {
                        Nodes = nodes
                    }, animations: animations),
                };

                if (postRuntimeChanges != null)
                {
                    model.PostRuntimeChanges = postRuntimeChanges;
                }

                setCamera(model);

                return model;
            }

            void AddRotationAnimationChannel(List<Runtime.AnimationChannel> channelList, Runtime.Node targetNode, Quaternion pitchValue, Quaternion restValue)
            {
                channelList.Add(
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = targetNode,
                            Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                        },
                        Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                            new[]
                            {
                                0.0f,
                                1.0f,
                                2.0f,
                            },
                            new[]
                            {
                                restValue,
                                pitchValue,
                                restValue,
                            })
                    });
            }

            Runtime.Animation CreateFoldingAnimation(Runtime.Node jointRootNode, List<Runtime.AnimationChannel> channelList = null)
            {
                if (channelList == null)
                {
                    channelList = new List<Runtime.AnimationChannel>();
                }
                Runtime.Node nodeCheck = jointRootNode;
                var pitchValue = FloatMath.ConvertDegreesToRadians(-90.0f);
                var nodeList = new List<Runtime.Node>()
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
                for (int nodeIndex = 1; nodeIndex < nodeList.Count(); nodeIndex++)
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
                return new Runtime.Animation
                {
                    Channels = channelList
                };
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA`."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is animating with a rotation."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) => {
                    var tempNodeList = Nodes.CreateFoldingPlaneSkin("skinA", 2, 3);

                    // Give the skin node a rotation 
                    tempNodeList[0].Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4), 0.0f, 0.0f);

                    // Create a new parent node and give it a rotation
                    tempNodeList[0] = new Runtime.Node
                    {
                        Name = "jointParent",
                        Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4), 0.0f, 0.0f),
                        Children = new List<Runtime.Node>
                        {
                            tempNodeList[0]
                        }
                    };

                    foreach (Runtime.Node node in tempNodeList)
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA` where the skinned node has a transform and a parent node with a transform. Both transforms should be ignored."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA`. The skin joints are not referenced by the scene nodes."));
                }, (model) => { model.Camera = closeCamera; }, (gltf) => {gltf.Scenes.First().Nodes = new []{0,};}),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    foreach (var joint in nodes[0].Skin.SkinJoints)
                    {
                        joint.InverseBindMatrix = Matrix4x4.Identity;
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA` without inverse bind matrices."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    // Attach a node with a mesh to the end of the joint hierarchy 
                    Runtime.Node nodeCheck = nodes[1];
                    while (nodeCheck.Children != null)
                    {
                        nodeCheck = nodeCheck.Children.First();
                    }
                    nodeCheck.Children = new List<Runtime.Node>
                    {
                        new Runtime.Node
                        {
                            Mesh = Mesh.CreateTriangle()
                        }
                    };

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is animated with a rotation and `joint1` has a triangle mesh attached to it."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    // Create a set of positions for the second mesh that are offset from the first mesh.
                    Runtime.MeshPrimitive originalMeshPrimitive = nodes[0].Mesh.MeshPrimitives.First();
                    var offsetPositions = new List<Vector3>();
                    foreach (Vector3 position in originalMeshPrimitive.Positions)
                    {
                        var offsetPosition = position;
                        offsetPosition.X += 0.6f;
                        offsetPositions.Add(offsetPosition);
                    }

                    // Create a second mesh
                    nodes.Add(new Runtime.Node
                    {
                        Name = "plane2",
                        Skin = nodes[0].Skin,
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new[]
                            {
                                new Runtime.MeshPrimitive
                                {
                                    VertexJointWeights = originalMeshPrimitive.VertexJointWeights,
                                    Positions = offsetPositions,
                                    Indices = originalMeshPrimitive.Indices,
                                    Material = new Runtime.Material
                                    {
                                        DoubleSided = true,
                                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                                        {
                                            BaseColorFactor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                                        }
                                    }
                                }
                            }
                        }
                    });

                    properties.Add(new Property(PropertyName.Description, "`skinA` where there are two meshes sharing a single skin."));
                }, (model) => { model.Camera = midCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    // Make joint1 a root joint
                    nodes.Add(nodes[1].Children.First());
                    nodes[1].Children = null;

                    // Compensate for no longer inheriting from joint0
                    nodes[2].Rotation = Quaternion.Multiply((Quaternion)nodes[2].Rotation, (Quaternion)nodes[1].Rotation);
                    nodes[2].Translation = null;
                    nodes[0].Skin.SkinJoints.ElementAt(1).InverseBindMatrix = Matrix4x4.Identity;

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is a root node and not a child of `joint0`."));
                }, (model) => { model.Camera = closeCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreatePlaneWithSkinB())
                    {
                        nodes.Add(node);
                    }

                    // Animate the joints
                    Runtime.Node nodeJoint0 = nodes[1];
                    Runtime.Node nodeJoint1 = nodeJoint0.Children.First();
                    var channelList = new List<Runtime.AnimationChannel>();
                    float rotationValue = FloatMath.ConvertDegreesToRadians(-15.0f);
                    AddRotationAnimationChannel(channelList, nodeJoint1, Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, rotationValue), Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, 0.0f));
                    animations.Add(new Runtime.Animation
                    {
                        Channels = channelList
                    });

                    properties.Add(new Property(PropertyName.Description, "`skinB` where `joint1` is animating with a rotation."));
                }, (model) => { model.Camera = skinBCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinC", 5, 5))
                    {
                        nodes.Add(node);
                    }
                    
                    // Rotate each joint node, except the root which already has the desired rotation
                    Runtime.Node nodeCheck = nodes[1].Children.First();
                    float rotationRadian = FloatMath.ConvertDegreesToRadians(-10.0f);
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
                    List<Runtime.SkinJoint> skinJointList = (List<Runtime.SkinJoint>)nodes[0].Skin.SkinJoints;
                    for (int skinJointIndex = 1; skinJointIndex < skinJointList.Count(); skinJointIndex++)
                    {
                        var translationInverseBindMatrix = skinJointList[skinJointIndex].InverseBindMatrix;
                        Matrix4x4.Invert(Matrix4x4.CreateRotationX(rotationRadian * (skinJointIndex + 1)), out Matrix4x4 invertedRotation);
                        skinJointList[skinJointIndex].InverseBindMatrix = Matrix4x4.Multiply(translationInverseBindMatrix, invertedRotation);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinC` where all of the joints have a local rotation of -10 degrees, except the root which is rotated -90 degrees."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreateFoldingPlaneSkin("skinD", 5, 6, 3, false))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    // Remove animation for the transform node
                    animations[0].Channels = new List<Runtime.AnimationChannel>()
                    {
                        animations[0].Channels.First(),
                        animations[0].Channels.ElementAt(1),
                        animations[0].Channels.ElementAt(3),
                    };

                    // Add the mesh to the transform node
                    nodes[1].Children.First().Children.First().Children.First().Mesh = Mesh.CreateTriangle();

                    properties.Add(new Property(PropertyName.Description, "`skinD` where each joint is animating with a rotation. There is a transform node in the joint hierarchy that is not a joint. That node has a mesh attached to it in order to show its location."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, animations, nodes) => {
                    foreach (Runtime.Node node in Nodes.CreatePlaneWithSkinE())
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinE`."));
                }, (model) => { model.Camera = distantCamera; }),
                //CreateModel((properties, animations, nodes) => {
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
                //    for (int skinJointIndex = 1; skinJointIndex < skinJointList.Count(); skinJointIndex++)
                //    {
                //        Matrix4x4 translationInverseBindMatrix = skinJointList.ElementAt(skinJointIndex).InverseBindMatrix;
                //        Matrix4x4.Invert(Matrix4x4.CreateRotationX(rotationRadian * (skinJointIndex + 1)) , out Matrix4x4 invertedRotation);
                //        skinJointList.ElementAt(skinJointIndex).InverseBindMatrix = Matrix4x4.Multiply(translationInverseBindMatrix, invertedRotation);
                //    }

                //    // Rebuild weights to include every joint instead of just the ones with a weight > 0
                //    var weightList = (List<List<Runtime.JointWeight>>)nodes[0].Mesh.MeshPrimitives.First().VertexJointWeights;
                //    for (int weightIndex = 0; weightIndex < weightList.Count(); weightIndex++)
                //    {
                //        var jointWeight = new List<Runtime.JointWeight>();

                //        for (int skinJointIndex = 0; skinJointIndex < skinJointList.Count; skinJointIndex++)
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
            };

            GenerateUsedPropertiesList();
        }
    }
}
