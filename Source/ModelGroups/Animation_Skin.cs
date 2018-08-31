﻿using System;
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
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.GLTF> setProperties, Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null)
            {
                var properties = new List<Property>();

                // There are no common properties in this model group.

                // Create the gltf object
                var gltf = new Runtime.GLTF();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf);

                var model = new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };

                if (postRuntimeChanges != null)
                {
                    model.PostRuntimeChanges = postRuntimeChanges;
                }

                return model;
            }

            void SetCommonGltf(Runtime.GLTF sourceGltf, Runtime.GLTF destinationGltf)
            {
                destinationGltf.Asset = sourceGltf.Asset;
                destinationGltf.Scenes = sourceGltf.Scenes;
            }

            void SetBasicSkin(Runtime.GLTF gltf)
            {
                var planeSkinScene = Scene.CreatePlaneWithSkin();
                Runtime.GLTF tempGltf = CreateGLTF(() => planeSkinScene);
                SetCommonGltf(tempGltf, gltf);
            }

            void SetFiveJointSkin(Runtime.GLTF gltf)
            {
                var planeSkinScene = Scene.CreateComplexPlaneWithSkin();
                Runtime.GLTF tempGltf = CreateGLTF(() => planeSkinScene);
                SetCommonGltf(tempGltf, gltf);
            }

            void SetRotationAnimation(List<Runtime.AnimationChannel> channelList, Runtime.Node node, float turnValue)
            {
                channelList.Add(
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = node,
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
                                Quaternion.Identity,
                                Quaternion.CreateFromYawPitchRoll(0.0f, turnValue, 0.0f),
                                Quaternion.Identity,
                            })
                    });
            }

            // This function assumes there is only one child per node. 
            void AnimateJointsWithRotation(Runtime.GLTF gltf, Runtime.Node JointRootNode, List<Runtime.AnimationChannel> channelList = null)
            {
                if(channelList == null)
                {
                    channelList = new List<Runtime.AnimationChannel>();
                }
                var nodeCheck = JointRootNode;
                var quarterTurn = (FloatMath.Pi / -2);
                var nodeList = new List<Runtime.Node>()
                {
                    JointRootNode,
                };
                while(nodeCheck.Children != null)
                {
                    nodeCheck = nodeCheck.Children.First();
                    nodeList.Add(nodeCheck);
                }
                int nodeListCount = nodeList.Count();
                for(int x = 1; x < nodeListCount; x++)
                {
                    float rotateValueModifier = 1.0f;
                    if(x == 1)
                    {
                        rotateValueModifier = 0.5f;
                    }
                    else if(x % 2 == 0)
                    {
                        rotateValueModifier = -1.0f;
                    }
                    SetRotationAnimation(channelList, nodeList[x], quarterTurn * rotateValueModifier);
                }
                SetNewAnimation(gltf, channelList);
            }

            void SetNewAnimation(Runtime.GLTF gltf, List<Runtime.AnimationChannel> channelList)
            {
                gltf.Animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = channelList
                    }
                };
            }

            // Rebuilds the default jointWeights so that each vertex has weights for the four surrounding nodes
            void SetOverlappingWeightsWithFiveJoints(Runtime.Node skinNode)
            {
                var skingJointsList = skinNode.Skin.SkinJoints;

                var rootJoint = skingJointsList.First();
                var rootMidJoint = skingJointsList.ElementAt(1);
                var midJoint = skingJointsList.ElementAt(2);
                var midTopJoint = skingJointsList.ElementAt(3);
                var topJoint = skingJointsList.ElementAt(4);

                //var defaultJointWeights = gltf.Scenes.First().Nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights;
                var overlappingJointWeightLists = new List<List<Runtime.JointWeight>>();
                var mainWeight = 0.7f;
                var secondaryWeight = 0.1f;

                // Add weights for all off the vertexes
                for (int x = 0; x < 12; x++)
                {
                    if (x < 4)
                    {
                        var weightToUse = secondaryWeight;
                        var weightToUseRoot = secondaryWeight;
                        if (x < 2)
                        {
                            weightToUse = 0;
                            weightToUseRoot = 1;
                        }
                        overlappingJointWeightLists.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = rootJoint,
                                Weight = weightToUseRoot,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = rootMidJoint,
                                Weight = weightToUse,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = midJoint,
                                Weight = weightToUse,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = midTopJoint,
                                Weight = weightToUse,
                            },
                        });
                    }
                    else
                    {
                        overlappingJointWeightLists.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = rootMidJoint,
                                Weight = secondaryWeight,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = midJoint,
                                Weight = secondaryWeight,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = midTopJoint,
                                Weight = secondaryWeight,
                            },
                            new Runtime.JointWeight
                            {
                                Joint = topJoint,
                                Weight = secondaryWeight,
                            },
                        });
                    }

                    int index = x / 2;
                    if (!(x < 4))
                    {
                        index--;
                        if (x > 9)
                        {
                            index--;
                        }
                    }
                    overlappingJointWeightLists.ElementAt(x).ElementAt(index).Weight = mainWeight;
                }

                skinNode.Mesh.MeshPrimitives.First().VertexJointWeights = overlappingJointWeightLists;
            }

            // Rebuilds the default jointWeights so that each vertex has weights for the four surrounding nodes, and removes the fifth joint
            void SetOverlappingWeightsWithFourJoints(Runtime.GLTF gltf, IEnumerable<Runtime.Node> nodes)
            {
                var skingJointsList = nodes.First().Skin.SkinJoints;

                // Removes the fifth joint
                var fourSkinJointList = new List<Runtime.SkinJoint>()
                {
                    skingJointsList.First(),
                    skingJointsList.ElementAt(1),
                    skingJointsList.ElementAt(2),
                    skingJointsList.ElementAt(3),
                };
                nodes.ElementAt(1).Children.First().Children.First().Children.First().Children = null;

                // Removes the animation for the fifth joint
                gltf.Animations.First().Channels = new List<Runtime.AnimationChannel>()
                {
                    gltf.Animations.First().Channels.First(),
                    gltf.Animations.First().Channels.ElementAt(1),
                    gltf.Animations.First().Channels.ElementAt(2),
                };

                var rootJoint = fourSkinJointList.First();
                var rootMidJoint = fourSkinJointList.ElementAt(1);
                var midJoint = fourSkinJointList.ElementAt(2);
                var midTopJoint = fourSkinJointList.ElementAt(3);

                var overlappingJointWeightLists = new List<List<Runtime.JointWeight>>();
                var mainWeight = 0.7f;
                var secondaryWeight = 0.1f;
                var rootWeight = 0;

                // Add weights for all off the vertexes
                for (int x = 0; x < 12; x++)
                {
                    var weightToUse = secondaryWeight;
                    var weightToUseRoot = secondaryWeight;
                    if (x < 2)
                    {
                        weightToUse = 0;
                        weightToUseRoot = 1;
                    }
                    overlappingJointWeightLists.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = weightToUseRoot,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = rootMidJoint,
                            Weight = weightToUse,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = weightToUse,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = midTopJoint,
                            Weight = weightToUse,
                        },
                    });

                    int index = x / 2;
                    if (x > 7)
                    {
                        index = 3;
                    }
                    overlappingJointWeightLists.ElementAt(x).ElementAt(index).Weight = mainWeight;
                }

                nodes.First().Skin.SkinJoints = fourSkinJointList;
                nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights = overlappingJointWeightLists;
            }

            void SetSecondSkin(Runtime.GLTF gltf, IEnumerable<Runtime.Node> nodes)
            {
                //Create a second skin that is effectivly a copy of the first, except they share joints
                var skinTwoNode = Scene.CreatePlaneWithSkin().Nodes.First();
                skinTwoNode.Name = "plane2";
                var rootNode = nodes.ElementAt(1);
                var midNode = rootNode.Children.First();
                var topNode = new Runtime.Node
                {
                    Name = "topNode",
                    Translation = new Vector3(0.0f, 0.5f, 0.0f),
                };
                midNode.Children = new List<Runtime.Node>()
                {
                    topNode
                };

                // Recreates the node list with the new skin node
                gltf.Scenes.First().Nodes = new List<Runtime.Node>()
                {
                    nodes.First(),
                    nodes.ElementAt(1),
                    skinTwoNode,
                };

                // New positions for the second skin
                skinTwoNode.Mesh.MeshPrimitives.First().Positions = new List<Vector3>()
                {
                    new Vector3(-0.5f, 0.5f, 0.0f),
                    new Vector3( 0.5f, 0.5f, 0.0f),
                    new Vector3(-0.5f, 0.75f, 0.0f),
                    new Vector3( 0.5f, 0.75f, 0.0f),
                    new Vector3(-0.5f, 1.0f, 0.0f),
                    new Vector3( 0.5f, 1.0f, 0.0f),
                };

                // Set the joints for the second skin node
                skinTwoNode.Skin.SkinJoints = new[]
                {
                        nodes.First().Skin.SkinJoints.ElementAt(1),
                        new Runtime.SkinJoint
                        (
                            inverseBindMatrix: new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,-0.5f,0,1),
                            node: topNode
                        )
                };
                var midJoint = skinTwoNode.Skin.SkinJoints.First();
                var topJoint = skinTwoNode.Skin.SkinJoints.ElementAt(1);

                // Set the weights for both skins
                var skinOneJointWeights = nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights;
                var skinTwoJointWeights = new List<List<Runtime.JointWeight>>();
                skinTwoNode.Mesh.MeshPrimitives.First().VertexJointWeights = skinTwoJointWeights;
                var skinOneJointWeightsCount = skinOneJointWeights.Count();
                for (int x = 0; x < skinOneJointWeightsCount; x++)
                {
                    var firstWeight = skinOneJointWeights.ElementAt(x).First().Weight;
                    var secondWeight = skinOneJointWeights.ElementAt(x).ElementAt(1).Weight;
                    if (x < 2)
                    {
                        firstWeight = 1.0f;
                        secondWeight = 0.0f;
                    }
                    else
                    {
                        firstWeight = 0.0f;
                        secondWeight = 1.0f;
                    }
                    skinOneJointWeights.ElementAt(x).First().Weight = firstWeight;
                    skinOneJointWeights.ElementAt(x).ElementAt(1).Weight = secondWeight;


                    firstWeight = 1;
                    secondWeight = 1;
                    skinTwoJointWeights.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = midJoint,
                            Weight = firstWeight,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = topJoint,
                            Weight = secondWeight,
                        },
                    });
                }
            }

            void SetPostRuntimeJointsOutsideScene(glTFLoader.Schema.Gltf gltf)
            {
                // Removes the joints from the scene
                gltf.Scenes.First().Nodes = new int[]
                {
                    0,
                };
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);

                    properties.Add(new Property(PropertyName.Description, "Skin with two joints."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));

                    properties.Add(new Property(PropertyName.Description, "Skin with two joints, one of which is animated with a rotation."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    gltf.Scenes.First().Nodes.First().Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4), 0.0f, 0.0f);
                    gltf.Scenes.First().Nodes.ElementAt(1).Rotation = Quaternion.CreateFromYawPitchRoll(-(FloatMath.Pi / 4), 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.Description, "Skin with two joints. The skin node has a transformation which is overridden by the joints."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    gltf.Scenes.First().Nodes.ElementAt(1).Rotation = Quaternion.CreateFromYawPitchRoll(-(FloatMath.Pi / 4), 0.0f, 0.0f);
                    gltf.Scenes.First().Nodes = new List<Runtime.Node>()
                    {
                        new Runtime.Node
                        {
                            Name = "planeParent",
                            Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4), 0.0f, 0.0f),
                            Children = new List<Runtime.Node>
                            {
                                gltf.Scenes.First().Nodes.First()
                            }
                        },
                        gltf.Scenes.First().Nodes.ElementAt(1),
                    };

                    properties.Add(new Property(PropertyName.Description, "Skin with two joints. The skin node has a parent with a transformation which is overridden by the joints."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));
                    var gltfNodeList = gltf.Scenes.First().Nodes;
                    var nodeList = new List<Runtime.Node>();

                    nodeList.Add(gltfNodeList.First());
                    nodeList.First().Children = new List<Runtime.Node>()
                    {
                        gltfNodeList.ElementAt(1)
                    };
                    gltf.Scenes.First().Nodes = nodeList;

                    properties.Add(new Property(PropertyName.Description, "Skin with two joints. The root joint is not the root node."));
                }),

                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));

                    properties.Add(new Property(PropertyName.Description, "Skin with five joints, all of which animate their respective vertex with a weight of 1."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));

                    // Rebuilds the default jointWeights so that the first three weights are 0, and the fourth is the one with the actual value
                    var skinNode = gltf.Scenes.First().Nodes.First();
                    var rootJoint = skinNode.Skin.SkinJoints.First();

                    var paddedJointWeightList = new List<List<Runtime.JointWeight>>();
                    var defaultJointWeights = skinNode.Mesh.MeshPrimitives.First().VertexJointWeights;
                    var jointPadding = new Runtime.JointWeight
                    {
                        Joint = rootJoint,
                        Weight = 0,
                    };

                    foreach (var jointWeightList in defaultJointWeights)
                    {
                        paddedJointWeightList.Add(new List<Runtime.JointWeight>()
                        {
                            jointPadding,
                            jointPadding,
                            jointPadding,
                            jointWeightList.First(),
                        });
                    };
                    skinNode.Mesh.MeshPrimitives.First().VertexJointWeights = paddedJointWeightList;

                    properties.Add(new Property(PropertyName.Description, "Skin with five joints. The first three weights for each vertex are 0, with the fourth being 1."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));
                    SetOverlappingWeightsWithFiveJoints(gltf.Scenes.First().Nodes.First());
                    properties.Add(new Property(PropertyName.Description, "Skin with five joints. Four joints have weights for any given vertex."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));
                    var midNode = gltf.Scenes.First().Nodes.ElementAt(1).Children.First().Children.First();
                    var midTopNode = midNode.Children.First();
                    var topNode = midTopNode.Children.First();

                    var multipleChildren = new List<Runtime.Node>()
                    {
                        midTopNode,
                        topNode,
                    };
                    multipleChildren.First().Children = null;

                    multipleChildren.ElementAt(1).Translation = new Vector3(0.0f, 0.4f, 0.0f); // Increace the translation for the new topnode, due to no longer having midtop as a parent
                    gltf.Animations.First().Channels.ElementAt(3).Target.Node = multipleChildren.ElementAt(1); // Set animation for top to the new topnode
                    gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(4).Node = topNode; // Set the top joint for the new topnode

                    midNode.Children = multipleChildren; // Overwrite the midtop and top nodes with our modified version


                    //DEBUG
                    //var channelList = new List<Runtime.AnimationChannel>();
                    //var quarterTurn = (FloatMath.Pi / -2);
                    //SetRotationAnimation(channelList, midNode.Children.ElementAt(0), -quarterTurn);
                    //SetNewAnimation(gltf, channelList);
                    //gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(3).Node = midNode.Children.ElementAt(0);
                    //gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(4).Node = midNode.Children.ElementAt(1);


                    properties.Add(new Property(PropertyName.Description, "Skin with five joints. The some of the joints share the same parent."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));
                    var midNode = gltf.Scenes.First().Nodes.ElementAt(1).Children.First().Children.First();
                    var midTopNode = midNode.Children.First();
                    var topNode = midTopNode.Children.First();

                    // New skinjoints list with midTopJoint removed
                    var skinJoints = new List<Runtime.SkinJoint>()
                    {
                        gltf.Scenes.First().Nodes.First().Skin.SkinJoints.First(),
                        gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(1),
                        gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(2),
                        gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(4),
                    };
                    gltf.Scenes.First().Nodes.First().Skin.SkinJoints = skinJoints;

                    // New jointweights list with midTopJoint weights set to zero
                    var jointWeights = new List<List<Runtime.JointWeight>>();
                    foreach (var jointWeightList in gltf.Scenes.First().Nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights)
                    {
                        jointWeights.Add(new List<Runtime.JointWeight>()
                        {
                            jointWeightList.First(),
                        });
                    }
                    // Reallocate midTopJoint's weights to other joints
                    var midJoint = gltf.Scenes.First().Nodes.First().Skin.SkinJoints.ElementAt(2);
                    jointWeights.ElementAt(6).First().Joint = midJoint;
                    jointWeights.ElementAt(7).First().Joint = midJoint;
                    jointWeights.ElementAt(6).First().Weight = 1;
                    jointWeights.ElementAt(7).First().Weight = 1;

                    // Remove animation for midTopJoint
                    gltf.Animations.First().Channels = new List<Runtime.AnimationChannel>()
                    {
                        gltf.Animations.First().Channels.First(),
                        gltf.Animations.First().Channels.ElementAt(1),
                        gltf.Animations.First().Channels.ElementAt(3),
                    };

                    properties.Add(new Property(PropertyName.Description, "Skin with four joints. A node in the middle of the joint hierarchy is skipped."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));
                    SetOverlappingWeightsWithFourJoints(gltf, gltf.Scenes.First().Nodes);

                    properties.Add(new Property(PropertyName.Description, "Skin with four joints. Four joints have weights for any given vertex."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));

                    // Attach a new node with a mesh to the end of the joint hierarchy 
                    var attachedMeshPrimitive = MeshPrimitive.CreateSinglePlane();
                    attachedMeshPrimitive.Material = new Runtime.Material
                    {
                        DoubleSided = true,
                    };
                    gltf.Scenes.First().Nodes.ElementAt(1).Children.First().Children.First().Children.First().Children.First().Children = new List<Runtime.Node>()
                    {
                        new Runtime.Node
                        {
                            Name = "attachedPlane",
                            Translation = new Vector3(0.0f, 0.2f, 0.0f),
                            Rotation = Quaternion.CreateFromYawPitchRoll(0.0f, (FloatMath.Pi / 2), 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>()
                                {
                                    attachedMeshPrimitive,
                                }
                            }
                        }
                    };

                    properties.Add(new Property(PropertyName.Description, "Skin with five joints. Another mesh is attached to the end of the joint hierarchy."));
                }),
                    CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    SetSecondSkin(gltf, gltf.Scenes.First().Nodes);

                    // Animate the joints
                    var rootNode = gltf.Scenes.First().Nodes.ElementAt(1);
                    var midNode = rootNode.Children.First();
                    var topNode = midNode.Children.First();
                    var channelList = new List<Runtime.AnimationChannel>();
                    var quarterTurn = (FloatMath.Pi / 2);
                    SetRotationAnimation(channelList, midNode, -quarterTurn);
                    SetRotationAnimation(channelList, topNode, quarterTurn);
                    SetNewAnimation(gltf, channelList);

                    properties.Add(new Property(PropertyName.Description, "Two skins which share a joint."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    AnimateJointsWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1));

                    properties.Add(new Property(PropertyName.Description, "Skin with two joints. The joints are not in a scene."));
                }, SetPostRuntimeJointsOutsideScene),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    var skinNode = gltf.Scenes.First().Nodes.First();
                    var rootNode = gltf.Scenes.First().Nodes.ElementAt(1);

                    // Remove the second joint
                    rootNode.Children = null;

                    // Update the joints
                    var updatedJoints = new List<Runtime.SkinJoint>()
                    {
                        new Runtime.SkinJoint
                        (
                            inverseBindMatrix: Matrix4x4.Identity,
                            node: rootNode
                        )
                    };

                    // Update the weights
                    var updatedJointWeights = new List<List<Runtime.JointWeight>>();

                    for (int x = 0; x < 6; x++)
                    {
                        updatedJointWeights.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = updatedJoints.First(),
                                Weight = 1
                            }
                        });
                    }

                    // Plug in the new values
                    skinNode.Mesh.MeshPrimitives.First().VertexJointWeights = updatedJointWeights;
                    skinNode.Skin.SkinJoints = updatedJoints;

                    // Animation
                    var channelList = new List<Runtime.AnimationChannel>();
                    var quarterTurn = (FloatMath.Pi / 2);
                    SetRotationAnimation(channelList, rootNode, -quarterTurn / 2);
                    SetNewAnimation(gltf, channelList);

                    properties.Add(new Property(PropertyName.Description, "Skin with one joint. The inverseBindMatrix is not specified."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
