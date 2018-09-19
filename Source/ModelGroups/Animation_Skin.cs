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
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Animation>, List<Runtime.Node>> setProperties, Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null)
            {
                var properties = new List<Property>();
                var nodes = new List<Runtime.Node>();
                var animations = new List<Runtime.Animation>();

                // There are no common properties in this model group.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animations, nodes);

                // If no animations are used, null out that property.
                if (animations.Count() == 0)
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

                return model;
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

            void AnimateJointsWithRotation(List<Runtime.Animation> animations, Runtime.Node JointRootNode, List<Runtime.AnimationChannel> channelList = null)
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
                    foreach (var node in nodeCheck.Children)
                    {
                        nodeList.Add(node);
                    }
                    nodeCheck = nodeCheck.Children.First();
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
                SetNewAnimation(animations, channelList);
            }

            void SetNewAnimation(List<Runtime.Animation> animations, List<Runtime.AnimationChannel> channelList)
            {
                animations.Add(new Runtime.Animation
                {
                    Channels = channelList
                });
            }

            //Create a second skin that is effectivly a copy of the first, except they share joints
            // TODO: All of the values of the second skin are being replaced. Just create one from scratch instead of copying from SkinA.
            //void SetSecondSkin(List<Runtime.Node> nodes)
            //{
            //    var skinTwoNode = Nodes.CreatePlaneWithSkinA();
            //    skinTwoNode[0].Name = "plane2";
            //    var rootNode = nodes[1];
            //    var midNode = rootNode.Children.First();
            //    var topNode = new Runtime.Node
            //    {
            //        Name = "topNode",
            //        Translation = new Vector3(0.0f, 0.5f, 0.0f),
            //    };
            //    midNode.Children = new List<Runtime.Node>()
            //    {
            //        topNode
            //    };

            //    // Recreates the node list with the new skin node
            //    var node0 = nodes[0];
            //    var node1 = nodes[1];
            //    nodes.Clear();
            //    nodes.Add(node0);
            //    nodes.Add(node1);
            //    nodes.Add(skinTwoNode[0]);

            //    // New positions for the second skin
            //    skinTwoNode[0].Mesh.MeshPrimitives.First().Positions = new List<Vector3>()
            //    {
            //        new Vector3(-0.5f, 0.5f, 0.0f),
            //        new Vector3( 0.5f, 0.5f, 0.0f),
            //        new Vector3(-0.5f, 0.75f, 0.0f),
            //        new Vector3( 0.5f, 0.75f, 0.0f),
            //        new Vector3(-0.5f, 1.0f, 0.0f),
            //        new Vector3( 0.5f, 1.0f, 0.0f),
            //    };

            //    // Set the joints for the second skin node
            //    skinTwoNode[0].Skin.SkinJoints = new[]
            //    {
            //        nodes.First().Skin.SkinJoints.ElementAt(1),
            //        new Runtime.SkinJoint
            //        (
            //            inverseBindMatrix: new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,-0.5f,0,1),
            //            node: topNode
            //        )
            //    };
            //    var midJoint = skinTwoNode[0].Skin.SkinJoints.First();
            //    var topJoint = skinTwoNode[0].Skin.SkinJoints.ElementAt(1);

            //    // Set the weights for both skins
            //    var skinOneJointWeights = nodes.First().Mesh.MeshPrimitives.First().VertexJointWeights;
            //    var skinTwoJointWeights = new List<List<Runtime.JointWeight>>();
            //    skinTwoNode[0].Mesh.MeshPrimitives.First().VertexJointWeights = skinTwoJointWeights;
            //    var skinOneJointWeightsCount = skinOneJointWeights.Count();
            //    for (int x = 0; x < skinOneJointWeightsCount; x++)
            //    {
            //        var firstWeight = skinOneJointWeights.ElementAt(x).First().Weight;
            //        var secondWeight = skinOneJointWeights.ElementAt(x).ElementAt(1).Weight;
            //        if (x < 2)
            //        {
            //            firstWeight = 1.0f;
            //            secondWeight = 0.0f;
            //        }
            //        else
            //        {
            //            firstWeight = 0.0f;
            //            secondWeight = 1.0f;
            //        }
            //        skinOneJointWeights.ElementAt(x).First().Weight = firstWeight;
            //        skinOneJointWeights.ElementAt(x).ElementAt(1).Weight = secondWeight;


            //        firstWeight = 1;
            //        secondWeight = 1;
            //        skinTwoJointWeights.Add(new List<Runtime.JointWeight>()
            //        {
            //            new Runtime.JointWeight
            //            {
            //                Joint = midJoint,
            //                Weight = firstWeight,
            //            },
            //            new Runtime.JointWeight
            //            {
            //                Joint = topJoint,
            //                Weight = secondWeight,
            //            },
            //        });
            //    }
            //}

            // Removes the expected joints from the scene
            void SetPostRuntimeJointsOutsideScene(glTFLoader.Schema.Gltf gltf)
            {
                gltf.Scenes.First().Nodes = new int[]
                {
                    0,
                };
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinA())
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`SkinA`."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinA())
                    {
                        nodes.Add(node);
                    }
                    AnimateJointsWithRotation(animations, nodes[1]);

                    properties.Add(new Property(PropertyName.Description, "`SkinA` where `Joint1` is animating with a rotation."));
                }),
                CreateModel((properties, animations, nodes) => {
                    var tempNodeList = Nodes.CreatePlaneWithSkinA();

                    // Give the skin node a rotation 
                    tempNodeList[0].Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4), 0.0f, 0.0f);

                    // Create a new parent node and give it a rotation
                    tempNodeList[0] = new Runtime.Node
                    {
                        Name = "planeParent",
                        Rotation = Quaternion.CreateFromYawPitchRoll((FloatMath.Pi / 4), 0.0f, 0.0f),
                        Children = new List<Runtime.Node>
                        {
                            tempNodeList[0]
                        }
                    };

                    foreach (var node in tempNodeList)
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`SkinA` where the skinned node has a transform and a parent node with a transform. Both transforms should be ignored."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinB())
                    {
                        nodes.Add(node);
                    }

                    // Animate the joints
                    var nodeJoint0 = nodes[1];
                    var nodeJoint1 = nodeJoint0.Children.First();
                    var nodeJoint2 = nodeJoint1.Children.First();
                    var channelList = new List<Runtime.AnimationChannel>();
                    var rotationValue = (FloatMath.Pi / 3);
                    SetRotationAnimation(channelList, nodeJoint1, rotationValue);
                    SetRotationAnimation(channelList, nodeJoint2, -rotationValue);
                    SetNewAnimation(animations, channelList);

                    properties.Add(new Property(PropertyName.Description, "`SkinB` where `Joint1` and `Joint2` are animating with a rotation."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinA())
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`SkinA`. The skin joints are not referenced by the scene nodes."));
                }, SetPostRuntimeJointsOutsideScene),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinA())
                    {
                        nodes.Add(node);
                    }
                    foreach (var joint in nodes[0].Skin.SkinJoints)
                    {
                        joint.InverseBindMatrix = Matrix4x4.Identity;
                    }

                    properties.Add(new Property(PropertyName.Description, "'Skin A` without `inverseBindMatrices`."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinC())
                    {
                        nodes.Add(node);
                    }
                    
                    // Rotate each joint by 10 degrees
                    var nodeCheck = nodes[1];
                    var rotationValue = Quaternion.CreateFromYawPitchRoll(0.0f, (10 * FloatMath.Pi / 180), 0.0f);
                    nodeCheck.Rotation = rotationValue;
                    while (nodeCheck.Children != null)
                    {
                        foreach (var node in nodeCheck.Children)
                        {
                            node.Rotation = rotationValue;
                        }
                        nodeCheck = nodeCheck.Children.First();
                    }

                    properties.Add(new Property(PropertyName.Description, "`SkinC where all of the joints have a local rotation of ~10 degrees."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinC())
                    {
                        nodes.Add(node);
                    }
                    AnimateJointsWithRotation(animations, nodes.ElementAt(1));

                    // New skinjoints list with midTopJoint removed
                    var skinJoints = new List<Runtime.SkinJoint>()
                    {
                        nodes[0].Skin.SkinJoints.First(),
                        nodes[0].Skin.SkinJoints.ElementAt(1),
                        nodes[0].Skin.SkinJoints.ElementAt(2),
                        nodes[0].Skin.SkinJoints.ElementAt(4),
                    };
                    nodes[0].Skin.SkinJoints = skinJoints;

                    // New jointweights list with midTopJoint weights set to zero
                    var jointWeights = new List<List<Runtime.JointWeight>>();
                    foreach (var jointWeightList in nodes[0].Mesh.MeshPrimitives.First().VertexJointWeights)
                    {
                        jointWeights.Add(new List<Runtime.JointWeight>()
                        {
                            jointWeightList.First(),
                        });
                    }
                    // Reallocate midTopJoint's weights to other joints
                    var midJoint = nodes[0].Skin.SkinJoints.ElementAt(2);
                    jointWeights.ElementAt(6).First().Joint = midJoint;
                    jointWeights.ElementAt(7).First().Joint = midJoint;
                    jointWeights.ElementAt(6).First().Weight = 1;
                    jointWeights.ElementAt(7).First().Weight = 1;

                    // Remove animation for midTopJoint
                    animations[0].Channels = new List<Runtime.AnimationChannel>()
                    {
                        animations[0].Channels.First(),
                        animations[0].Channels.ElementAt(1),
                        animations[0].Channels.ElementAt(3),
                    };

                    properties.Add(new Property(PropertyName.Description, "Skin with four joints. A node in the middle of the joint hierarchy is skipped."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinC())
                    {
                        nodes.Add(node);
                    }
                    AnimateJointsWithRotation(animations, nodes.ElementAt(1));

                    // Attach a new node with a mesh to the end of the joint hierarchy 
                    var attachedMeshPrimitive = MeshPrimitive.CreateSinglePlane();
                    attachedMeshPrimitive.Material = new Runtime.Material
                    {
                        DoubleSided = true,
                    };
                    nodes[1].Children.First().Children.First().Children.First().Children.First().Children = new List<Runtime.Node>()
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

                    properties.Add(new Property(PropertyName.Description, "`SkinA` where `Joint1` is animated with a rotation and `Joint1` has a triangle mesh attached to it."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinD())
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`SkinD."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
