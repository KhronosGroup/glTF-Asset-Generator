using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class Animation_Skin : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Skin;

        public Animation_Skin(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.GLTF> setProperties)
            {
                var properties = new List<Property>();

                // Apply the common properties to the gltf.


                // Create the gltf object
                var gltf = new Runtime.GLTF();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf);

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
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

            void GiveJointRootParent(Runtime.GLTF gltf)
            {
                var nodeList = new List<Runtime.Node>();
                nodeList.Add(gltf.Scenes.First().Nodes.First());
                nodeList.First().Children = new List<Runtime.Node>()
                {
                    gltf.Scenes.First().Nodes.ElementAt(1)
                };
                gltf.Scenes.First().Nodes = nodeList;
            }

            void AnimateWithRotation(List<Runtime.AnimationChannel> channelList, Runtime.Node node, float turnValue)
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

            void AnimateFiveJointsWithRotation(Runtime.GLTF gltf)
            {
                var rootNode = gltf.Scenes.First().Nodes.ElementAt(1);
                var rootMidNode = rootNode.Children.First();
                var midNode = rootMidNode.Children.First();
                var midTopNode = midNode.Children.First();
                var TopNode = midTopNode.Children.First();

                var channelList = new List<Runtime.AnimationChannel>();
                var quarterTurn = (FloatMath.Pi / 2);

                AnimateWithRotation(channelList, rootMidNode, quarterTurn/2); // 45
                AnimateWithRotation(channelList, midNode, -quarterTurn); // -90
                AnimateWithRotation(channelList, midTopNode, quarterTurn); // 90
                AnimateWithRotation(channelList, TopNode, -quarterTurn);  // -90

                SetNewAnimation(gltf, channelList);
            }

            void AnimateFourJointsWithRotation(Runtime.GLTF gltf)
            {
                var rootNode = gltf.Scenes.First().Nodes.ElementAt(1);
                var rootMidNode = rootNode.Children.First();
                var midNode = rootMidNode.Children.First();
                var midTopNode = midNode.Children.First();

                var channelList = new List<Runtime.AnimationChannel>();
                var quarterTurn = (FloatMath.Pi / 2);

                AnimateWithRotation(channelList, rootMidNode, quarterTurn / 2); // 45
                AnimateWithRotation(channelList, midNode, -quarterTurn); // -90
                AnimateWithRotation(channelList, midTopNode, quarterTurn); // 90

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

            // Rebuilds the default jointWeights so that the first three weights are 0, and the fourth is the one with the actual value
            void SetVertexJointWeightsWithPadding(Runtime.GLTF gltf)
            {
                var rootJoint = gltf.Scenes.First().Nodes.First().Skin.SkinJoints.First();

                var paddedJointWeightList = new List<List<Runtime.JointWeight>>();
                var defaultJointWeights = gltf.Scenes.First().Nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights;
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

                gltf.Scenes.First().Nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights = paddedJointWeightList;
            }

            // Rebuilds the default jointWeights so that each vertex has weights for the four surrounding nodes
            void SetOverlappingWeightsWithFiveJoints(Runtime.GLTF gltf)
            {
                var skingJointsList = gltf.Scenes.First().Nodes.First().Skin.SkinJoints;

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
                        overlappingJointWeightLists.Add(new List<Runtime.JointWeight>()
                        {
                            new Runtime.JointWeight
                            {
                                Joint = rootJoint,
                                Weight = secondaryWeight,
                            },
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

                gltf.Scenes.First().Nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights = overlappingJointWeightLists;
            }

            // Rebuilds the default jointWeights so that each vertex has weights for the four surrounding nodes, and removes the fifth joint
            void SetOverlappingWeightsWithFourJoints(Runtime.GLTF gltf)
            {
                var skingJointsList = gltf.Scenes.First().Nodes.First().Skin.SkinJoints;

                // Removes the fifth joint
                var fourSkinJointList = new List<Runtime.SkinJoint>()
                {
                    skingJointsList.First(),
                    skingJointsList.ElementAt(1),
                    skingJointsList.ElementAt(2),
                    skingJointsList.ElementAt(3),
                };
                gltf.Scenes.First().Nodes.ElementAt(1).Children.First().Children.First().Children.First().Children = null;

                var rootJoint = fourSkinJointList.First();
                var rootMidJoint = fourSkinJointList.ElementAt(1);
                var midJoint = fourSkinJointList.ElementAt(2);
                var midTopJoint = fourSkinJointList.ElementAt(3);

                var overlappingJointWeightLists = new List<List<Runtime.JointWeight>>();
                var mainWeight = 0.7f;
                var secondaryWeight = 0.1f;

                // Add weights for all off the vertexes
                for (int x = 0; x < 12; x++)
                {
                    overlappingJointWeightLists.Add(new List<Runtime.JointWeight>()
                    {
                        new Runtime.JointWeight
                        {
                            Joint = rootJoint,
                            Weight = secondaryWeight,
                        },
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
                    });

                    int index = x / 2;
                    if (x > 7)
                    {
                        index = 3;
                    }
                    overlappingJointWeightLists.ElementAt(x).ElementAt(index).Weight = mainWeight;
                }

                gltf.Scenes.First().Nodes.First().Skin.SkinJoints = fourSkinJointList;
                gltf.Scenes.First().Nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights = overlappingJointWeightLists;
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    var channelList = new List<Runtime.AnimationChannel>();
                    AnimateWithRotation(channelList, gltf.Scenes.First().Nodes.ElementAt(1).Children.First(), (FloatMath.Pi / 5));
                    SetNewAnimation(gltf, channelList);
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
                    GiveJointRootParent(gltf);
                    var channelList = new List<Runtime.AnimationChannel>();
                    AnimateWithRotation(channelList, gltf.Scenes.First().Nodes.First().Children.First().Children.First(), (FloatMath.Pi / 5));
                    SetNewAnimation(gltf, channelList);
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints. The root joint is not the root node."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateFourJointsWithRotation(gltf);
                    SetOverlappingWeightsWithFourJoints(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with four joints. Four joints have weights for any given vertex."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateFiveJointsWithRotation(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with five joints, all of which animate their respective vertex with a weight of 1."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateFiveJointsWithRotation(gltf);
                    SetVertexJointWeightsWithPadding(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with five joints. The first three weights for each vertex are 0, with the fourth being 1."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateFiveJointsWithRotation(gltf);
                    SetOverlappingWeightsWithFiveJoints(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with five joints. Four joints have weights for any given vertex."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
