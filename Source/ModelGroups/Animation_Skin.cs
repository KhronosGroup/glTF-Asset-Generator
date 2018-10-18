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
            var closeCamera = new Vector3(0, 0, 0.7f);
            var defaultCamera = new Vector3(0, 0, 1.3f);

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Animation>, List<Runtime.Node>, Manifest.Camera> setProperties, Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null)
            {
                var properties = new List<Property>();
                var nodes = new List<Runtime.Node>();
                var animations = new List<Runtime.Animation>();
                var camera = new Manifest.Camera(closeCamera);

                // There are no common properties in this model group.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animations, nodes, camera);

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
                    Camera = camera
                };

                if (postRuntimeChanges != null)
                {
                    model.PostRuntimeChanges = postRuntimeChanges;
                }

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
                var nodeCheck = jointRootNode;
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
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA`."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    animations.Add(CreateFoldingAnimation(nodes[1]));

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is animating with a rotation."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
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

                    foreach (var node in tempNodeList)
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA` where the skinned node has a transform and a parent node with a transform. Both transforms should be ignored."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA`. The skin joints are not referenced by the scene nodes."));
                }, (glTFLoader.Schema.Gltf gltf) => {gltf.Scenes.First().Nodes = new int[]{0,};}),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
                    {
                        nodes.Add(node);
                    }
                    foreach (var joint in nodes[0].Skin.SkinJoints)
                    {
                        joint.InverseBindMatrix = Matrix4x4.Identity;
                    }

                    properties.Add(new Property(PropertyName.Description, "`skinA` without inverse bind matrices."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinA", 2, 3))
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
                    nodeCheck.Children = new List<Runtime.Node>
                    {
                        new Runtime.Node
                        {
                            Mesh = Mesh.CreateTriangle()
                        }
                    };

                    properties.Add(new Property(PropertyName.Description, "`skinA` where `joint1` is animated with a rotation and `joint1` has a triangle mesh attached to it."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinB())
                    {
                        nodes.Add(node);
                    }

                    // Animate the joints
                    var nodeJoint0 = nodes[1];
                    var nodeJoint1 = nodeJoint0.Children.First();
                    var channelList = new List<Runtime.AnimationChannel>();
                    var rotationValue = FloatMath.ConvertDegreesToRadians(-15.0f);
                    AddRotationAnimationChannel(channelList, nodeJoint1, Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, rotationValue), Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, 0.0f));
                    animations.Add(new Runtime.Animation
                    {
                        Channels = channelList
                    });

                    camera.SetTranslationWithVector3 = defaultCamera;
                    properties.Add(new Property(PropertyName.Description, "`skinB` where `joint1` is animating with a rotation."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinC", 5, 5))
                    {
                        nodes.Add(node);
                    }
                    
                    // Rotate each joint node, except the root which already has the desired rotation
                    var nodeCheck = nodes[1].Children.First();
                    var rotationRadian = FloatMath.ConvertDegreesToRadians(-10.0f);
                    var rotationQuaternion = Quaternion.CreateFromYawPitchRoll(0.0f, rotationRadian, 0.0f);
                    nodeCheck.Rotation = rotationQuaternion;
                    while (nodeCheck.Children != null)
                    {
                        foreach (var node in nodeCheck.Children)
                        {
                            node.Rotation = rotationQuaternion;
                        }
                        nodeCheck = nodeCheck.Children.First();
                    }

                    // Rebuild the inverseBindMatrix for each joint (except the root) to work with the new rotation
                    Matrix4x4 invertedRotation;
                    var skinJointList = nodes[0].Skin.SkinJoints;
                    for (int skinJointIndex = 1; skinJointIndex < skinJointList.Count(); skinJointIndex++)
                    {
                        var translationInverseBindMatrix = skinJointList.ElementAt(skinJointIndex).InverseBindMatrix;
                        Matrix4x4.Invert(Matrix4x4.CreateRotationX(rotationRadian * (skinJointIndex + 1)) , out invertedRotation);
                        skinJointList.ElementAt(skinJointIndex).InverseBindMatrix = Matrix4x4.Multiply(translationInverseBindMatrix, invertedRotation);
                    }

                    camera.SetTranslationWithVector3 = defaultCamera;
                    properties.Add(new Property(PropertyName.Description, "`skinC` where all of the joints have a local rotation of -10 degrees, except the root which is rotated -90 degrees."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreateFoldingPlaneSkin("skinD", 5, 6, 3, false))
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

                    camera.SetTranslationWithVector3 = defaultCamera;
                    properties.Add(new Property(PropertyName.Description, "`skinD` where each joint is animating with a rotation. There is a transform node in the joint hierarchy that is not a joint. That node has a mesh attached to it in order to show its location."));
                }),
                CreateModel((properties, animations, nodes, camera) => {
                    foreach (var node in Nodes.CreatePlaneWithSkinE())
                    {
                        nodes.Add(node);
                    }

                    camera.SetTranslationWithVector3 = defaultCamera;
                    properties.Add(new Property(PropertyName.Description, "`skinE`."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
