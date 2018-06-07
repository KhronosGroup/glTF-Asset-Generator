﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class Animation_NodeMisc : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_NodeMisc;

        public Animation_NodeMisc(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.AnimationChannel>, List<Runtime.Node>> setProperties)
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

                // Apply the properties that are specific to this gltf.
                setProperties(properties, channels, nodes);

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
                gltf.Animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = channels
                    }
                };
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
            }

            void SetTranslationChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.TRANSLATION,
                };
            }

            void SetRotationChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                };
            }

            void SetScaleChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.SCALE,
                };
            }

            void SetLinearSamplerForTranslation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        2.0f,
                        4.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });
            }

            void SetLinearSamplerForScale(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        2.0f,
                        4.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(0.8f, 0.8f, 0.8f),
                        new Vector3(1.2f, 1.2f, 1.2f),
                        new Vector3(0.8f, 0.8f, 0.8f),
                    });
            }

            void SetLinearSamplerForRotation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                var quarterTurn = (float)(Math.PI / 2);
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new List<float>
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    },
                    new List<Quaternion>
                    {
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(-quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                    });
            }

            void SetLinearSamplerWithNoTransforms(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        2.0f,
                        6.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, 0, 0),
                    });
            }

            void SetLinearSamplerWithOneKey(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        1.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });
            }

            void CreateSamplerStartsAboveZero(List<Property> properties, Runtime.AnimationChannel channel)
            {
                var quarterTurn = (float)(Math.PI / 2);
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new List<float>
                    {
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                        5.0f,
                    },
                    new List<Quaternion>
                    {
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(quarterTurn*(-1), 0, 0),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                    });
            }

            void CreateMultipleChannelsWithUniqueTargets(List<Property> properties, List<Runtime.AnimationChannel> channels, Runtime.Node node)
            {
                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());

                var targetPropertiesList = new List<Property>();
                SetTranslationChannelTarget(targetPropertiesList, channels[0], node);
                SetRotationChannelTarget(targetPropertiesList, channels[1], node);

                var samplerPropertiesList = new List<Property>();
                SetLinearSamplerForTranslation(samplerPropertiesList, channels[0]);
                SetLinearSamplerForRotation(samplerPropertiesList, channels[1]);

                // Takes the properties created by the animation target helper functions and condenses them into a single property, then adds that property to the list of used properties.
                var targetReadmeValue = new StringBuilder();
                var samplerReadmeValue = new StringBuilder();
                for (int x = 0; x < targetPropertiesList.Count; x++)
                {
                    targetReadmeValue.Append($"{targetPropertiesList[x].ReadmeValue}<br>");
                    samplerReadmeValue.Append($"{samplerPropertiesList[x].ReadmeValue}<br>");
                }
            }

            void CreateMultipleChannelsWithDifferentTimes(List<Property> properties, List<Runtime.AnimationChannel> channels, Runtime.Node node)
            {
                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());

                var targetPropertiesList = new List<Property>();
                SetTranslationChannelTarget(targetPropertiesList, channels[0], node);
                SetRotationChannelTarget(targetPropertiesList, channels[1], node);

                var samplerPropertiesList = new List<Property>();
                SetLinearSamplerWithNoTransforms(samplerPropertiesList, channels[0]);
                CreateSamplerStartsAboveZero(samplerPropertiesList, channels[1]);

                // Takes the properties created by the animation target helper functions and condenses them into a single property, then adds that property to the list of used properties.
                var targetReadmeValue = new StringBuilder();
                var samplerReadmeValue = new StringBuilder();
                for (int x = 0; x < targetPropertiesList.Count; x++)
                {
                    targetReadmeValue.Append($"{targetPropertiesList[x].ReadmeValue}<br>");
                    samplerReadmeValue.Append($"{samplerPropertiesList[x].ReadmeValue}<br>");
                }
            }

            void CreateMultipleChannelsForDifferentNodes(List<Property> properties, List<Runtime.AnimationChannel> channels, List<Runtime.Node> nodes)
            {
                // Creates a second node based on the existing node, and applies a transform to both to help differentiate them.
                nodes.Add(DeepCopy.CloneObject(nodes[0]));
                nodes[0].Translation = new Vector3(-0.5f, 0, 0);
                nodes[1].Translation = new Vector3(0.5f, 0, 0);

                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());

                var targetPropertiesList = new List<Property>();
                SetRotationChannelTarget(targetPropertiesList, channels[0], nodes[0]);
                SetScaleChannelTarget(targetPropertiesList, channels[1], nodes[1]);

                var samplerPropertiesList = new List<Property>();
                SetLinearSamplerForRotation(samplerPropertiesList, channels[0]);
                SetLinearSamplerForScale(samplerPropertiesList, channels[1]);

                // Takes the properties created by the animation target helper functions and condenses them into a single property, then adds that property to the list of used properties.
                var targetReadmeValue = new StringBuilder();
                var samplerReadmeValue = new StringBuilder();
                for (int x = 0; x < targetPropertiesList.Count; x++)
                {
                    targetReadmeValue.Append($"{targetPropertiesList[x].ReadmeValue}<br>");
                    samplerReadmeValue.Append($"{samplerPropertiesList[x].ReadmeValue}<br>");
                }
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, channels, nodes) => {
                    // Multiple channels
                    CreateMultipleChannelsWithUniqueTargets(properties, channels, nodes[0]);
                    properties.Add(new Property(PropertyName.Description, "Multiple channels are used, each with the same start and end time."));
                }),
                CreateModel((properties, channels, nodes) => {
                    // Curve that doesn't start at zero
                    SetRotationChannelTarget(properties, channels[0], nodes[0]);
                    CreateSamplerStartsAboveZero(properties, channels[0]);
                    properties.Add(new Property(PropertyName.Description, "The time of the first keyframe does not start at zero."));
                }),
                CreateModel((properties, channels, nodes) => {
                    // Two channels with different start/end times
                    CreateMultipleChannelsWithDifferentTimes(properties, channels, nodes[0]);
                    properties.Add(new Property(PropertyName.Description, "There are two channels. The first channel has a constant transform value and " +
                        "starts after the second channel. The second channel is a rotation who's keyframe starts above zero and ends before the other channel."));
                }),
                CreateModel((properties, channels, nodes) => {
                    // Has only one key
                    SetTranslationChannelTarget(properties, channels[0], nodes[0]);
                    SetLinearSamplerWithOneKey(properties, channels[0]);
                    properties.Add(new Property(PropertyName.Description, "The channel has only one keyframe."));
                }),
                CreateModel((properties, channels, nodes) => {
                    // One animation, two channels for two nodes
                    CreateMultipleChannelsForDifferentNodes(properties, channels, nodes);
                    properties.Add(new Property(PropertyName.Description, "There are two channels, the first targeting rotation and the second scale. " +
                        "The rotation is applied to the left node, and the scale to the right node."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
