using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal class Animation_NodeMisc : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_NodeMisc;

        public Animation_NodeMisc(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.AnimationChannel>, List<Runtime.Node>, List<Runtime.Animation>> setProperties)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material()
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                    {
                        BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                    },
                };
                var channels = new List<Runtime.AnimationChannel>()
                {
                    new Runtime.AnimationChannel()
                };
                var nodes = new List<Runtime.Node>
                {
                    new Runtime.Node(),
                };
                var animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = channels
                    }
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, channels, nodes, animations);

                // Create the gltf object
                foreach (var node in nodes)
                {
                    node.Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new List<Runtime.MeshPrimitive>
                        {
                            cubeMeshPrimitive
                        }
                    };
                }                
                Runtime.GLTF gltf = CreateGLTF(() => new Runtime.Scene()
                {
                    Nodes = nodes
                });
                gltf.Animations = animations;
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
            }

            void SetTranslationChannelTarget(Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.TRANSLATION,
                };
            }

            void SetRotationChannelTarget(Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                };
            }

            void SetLinearSamplerForTranslation(Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new[]
                    {
                        0.0f,
                        2.0f,
                        4.0f,
                    },
                    new[]
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });
            }

            void SetLinearSamplerForHorizontalRotation(Runtime.AnimationChannel channel)
            {
                var quarterTurn = (FloatMath.Pi / 2);
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    },
                    new[]
                    {
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(-quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                    });
            }

            void SetLinearSamplerForVerticalRotation(Runtime.AnimationChannel channel)
            {
                var quarterTurn = (FloatMath.Pi / 2);
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    },
                    new[]
                    {
                        Quaternion.CreateFromYawPitchRoll(0, quarterTurn, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(0, -quarterTurn, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(0, quarterTurn, 0),
                    });
            }

            void SetLinearSamplerForConstantRotation(Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        0.0f,
                        6.0f,
                    },
                    new[]
                    {
                        Quaternion.CreateFromYawPitchRoll(-FloatMath.Pi / 3, 0, 0),
                        Quaternion.CreateFromYawPitchRoll(-FloatMath.Pi / 3, 0, 0),
                    });
            }

            void SetLinearSamplerForTranslationStartsAboveZero(Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new[]
                    {
                        2.0f,
                        4.0f,
                        6.0f,
                    },
                    new[]
                    {
                        new Vector3(0, -0.1f, 0),
                        new Vector3(0, 0.1f, 0),
                        new Vector3(0, -0.1f, 0),
                    });
            }

            void SetLinearSamplerWithOneKey(Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new[]
                    {
                        0.0f,
                    },
                    new[]
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });
            }

            void SetLinearSamplerForRotationThatStartsAboveZero(Runtime.AnimationChannel channel)
            {
                var quarterTurn = (FloatMath.Pi / 2);
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                        5.0f,
                    },
                    new[]
                    {
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(-quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                    });
            }

            void CreateMultipleChannelsWithUniqueTargets(List<Runtime.AnimationChannel> channels, Runtime.Node node)
            {
                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());

                SetTranslationChannelTarget(channels[0], node);
                SetRotationChannelTarget(channels[1], node);

                var samplerPropertiesList = new List<Property>();
                SetLinearSamplerForTranslation(channels[0]);
                SetLinearSamplerForHorizontalRotation(channels[1]);
            }

            void CreateMultipleChannelsWithDifferentTimes(List<Runtime.AnimationChannel> channels, Runtime.Node node)
            {
                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());

                SetTranslationChannelTarget(channels[0], node);
                SetRotationChannelTarget(channels[1], node);

                SetLinearSamplerForTranslationStartsAboveZero(channels[0]);
                SetLinearSamplerForRotationThatStartsAboveZero(channels[1]);
            }

            void CreateMultipleChannelsForDifferentNodes(List<Runtime.AnimationChannel> channels, List<Runtime.Node> nodes)
            {
                // Creates a second node based on the existing node, and applies a transform to help differentiate them.
                nodes.Add(DeepCopy.CloneObject(nodes[0]));
                nodes[0].Translation = new Vector3(-0.2f, 0.0f, 0f);
                nodes[1].Translation = new Vector3(0.2f, 0.0f, 0f);
                nodes[0].Scale = new Vector3(0.5f, 0.5f, 0.5f);
                nodes[1].Scale = new Vector3(0.5f, 0.5f, 0.5f);

                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());
                
                SetRotationChannelTarget(channels[0], nodes[0]);
                SetRotationChannelTarget(channels[1], nodes[1]);

                SetLinearSamplerForHorizontalRotation(channels[0]);
                SetLinearSamplerForVerticalRotation(channels[1]);
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, channels, nodes, animations) => {
                    // Multiple channels
                    CreateMultipleChannelsWithUniqueTargets(channels, nodes[0]);
                    properties.Add(new Property(PropertyName.Description,
                        "There are two channels. The first channel targets translation. The second channel targets rotation. The start and end times of both channels are `0.0` and `4.0` respectively."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // Curve that doesn't start at zero
                    SetRotationChannelTarget(channels[0], nodes[0]);
                    SetLinearSamplerForRotationThatStartsAboveZero(channels[0]);
                    properties.Add(new Property(PropertyName.Description,
                        "There is one channel with a non-zero start time. The channel targets rotation. The start time is `1.0`."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // Two channels with different start/end times
                    CreateMultipleChannelsWithDifferentTimes(channels, nodes[0]);
                    properties.Add(new Property(PropertyName.Description,
                        "There are two channels with different start and end times. The first channel targets translation with start and end times of `2.0` and `6.0` respectively. " +
                        "The second channel targets rotation with start and end times of `1.0` and `5.0` respectively."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // Has only one key
                    SetTranslationChannelTarget(channels[0], nodes[0]);
                    SetLinearSamplerWithOneKey(channels[0]);
                    properties.Add(new Property(PropertyName.Description,
                        "There is one channel with only one keyframe. The channel targets translation with a value of <code>[-0.1,&nbsp;0.0,&nbsp;0.0]</code>."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // One animation, two channels for two nodes
                    CreateMultipleChannelsForDifferentNodes(channels, nodes);
                    properties.Add(new Property(PropertyName.Description,
                        "There are two channels with different nodes. The first channel targets the left node and rotation along the X axis. " +
                        "The second channel targets the right node and rotation along the Y axis."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // Rotate the model, and then apply the same target animation to it (Animation overrides)
                    nodes[0].Rotation = Quaternion.CreateFromYawPitchRoll(FloatMath.Pi / 3, 0, 0);
                    SetRotationChannelTarget(channels[0], nodes[0]);
                    SetLinearSamplerForConstantRotation(channels[0]);
                    properties.Add(new Property(PropertyName.Description,
                        "There is one channel that targets a node. The node has a rotation of <code>[0.0,&nbsp;0.5,&nbsp;0.0,&nbsp;0.866]</code>. The channel overrides the rotation of the node to a different constant value of <code>[0.0,&nbsp;-0.5,&nbsp;0.0,&nbsp;0.866]</code>."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // Rotate the model, and then apply an translation animation to it (Animation doesn't override rotation)
                    nodes[0].Rotation = Quaternion.CreateFromYawPitchRoll(FloatMath.Pi / 3, 0, 0);
                    SetTranslationChannelTarget(channels[0], nodes[0]);
                    SetLinearSamplerForTranslation(channels[0]);
                    properties.Add(new Property(PropertyName.Description,
                        "There is one channel that targets a node. The node has a rotation of <code>[0.0,&nbsp;0.5,&nbsp;0.0,&nbsp;0.866]</code>. The channel targets the translation of the node."));
                }),
                CreateModel((properties, channels, nodes, animations) => {
                    // Two animations. One rotates, the other translates. They should not interact or bleed across.
                    // The first animation is already added as an empty common property.
                    animations.Add(new Runtime.Animation
                    {
                        Channels = new[]
                        {
                            new Runtime.AnimationChannel()
                        }
                    });
                    SetRotationChannelTarget(channels[0], nodes[0]);
                    SetLinearSamplerForHorizontalRotation(channels[0]);
                    SetTranslationChannelTarget(animations.ElementAt(1).Channels.First(), nodes.First());
                    SetLinearSamplerForTranslation(animations.ElementAt(1).Channels.First());
                    properties.Add(new Property(PropertyName.Description,
                        "There are two animations, each with one channel. The first animation's channel targets rotation. The second animation's channel targets translation."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
