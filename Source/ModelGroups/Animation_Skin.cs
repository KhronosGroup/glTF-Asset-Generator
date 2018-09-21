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

            void AddRotationAnimationChannel(List<Runtime.AnimationChannel> channelList, Runtime.Node targetNode, float pitchValue)
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
                                Quaternion.Identity,
                                Quaternion.CreateFromYawPitchRoll(0.0f, pitchValue * 1.5f, 0.0f),
                                Quaternion.Identity,
                            })
                    });
            }

            Runtime.Animation CreateFoldingAnimation(Runtime.Node jointRootNode, List<Runtime.AnimationChannel> channelList = null)
            {
                if (channelList == null)
                {
                    channelList = new List<Runtime.AnimationChannel>();
                }
                var nodeCheck = jointRootNode;
                var pitchValue = (-FloatMath.Pi / 2);
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
                    AddRotationAnimationChannel(channelList, nodeList[nodeIndex], pitchValue * rotateValueModifier);
                }
                return new Runtime.Animation
                {
                    Channels = channelList
                };
            }

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
                    animations.Add(CreateFoldingAnimation(nodes[1]));

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

                    properties.Add(new Property(PropertyName.Description, "`Skin A` without `inverseBindMatrices`."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinA())
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    // Attach a node with a mesh to the end of the joint hierarchy 
                    var nodeCheck = nodes[1];
                    while (nodeCheck.Children != null)
                    {
                        nodeCheck = nodeCheck.Children.First();
                    }
                    nodeCheck.Children = Nodes.CreateTriangle();

                    properties.Add(new Property(PropertyName.Description, "`SkinA` where `Joint1` is animated with a rotation and `Joint1` has a triangle mesh attached to it."));
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
                    var rotationValue = (-FloatMath.Pi / 4);
                    AddRotationAnimationChannel(channelList, nodeJoint1, rotationValue);
                    AddRotationAnimationChannel(channelList, nodeJoint2, -rotationValue);
                    animations.Add(new Runtime.Animation
                    {
                        Channels = channelList
                    });

                    properties.Add(new Property(PropertyName.Description, "`SkinB` where `Joint1` and `Joint2` are animating with a rotation."));
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

                    properties.Add(new Property(PropertyName.Description, "`SkinC` where all of the joints have a local rotation of ~10 degrees."));
                }),
                CreateModel((properties, animations, nodes) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinC())
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

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
                    foreach (var node in Nodes.CreatePlaneWithSkinD())
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`SkinD`."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
