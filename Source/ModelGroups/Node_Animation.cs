using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Node_Animation : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Node_Animation;

        public Node_Animation(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Channel>, Runtime.Node> setProperties)
            {
                var properties = new List<Property>();
                var cube = Gltf.CreateCube();
                var gltf = CreateGLTF(() => cube.Scenes[0]);

                // Apply the common properties to the gltf.
                gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = new Runtime.Material()
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                    {
                        BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                    },
                };
                gltf.Animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = new List<Runtime.Channel>()
                        {
                            new Runtime.Channel()
                        }
                    }
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf.Animations[0].Channels, gltf.Scenes[0].Nodes[0]);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                };
            }

            void CreateChannelTargetWithTranslation(List<Property> properties, Runtime.Channel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.TRANSLATION,
                };
                properties.Add(new Property(PropertyName.Translation, "Translation"));
            }

            void CreateChannelTargetWithRotation(List<Property> properties, Runtime.Channel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.ROTATION,
                };
                properties.Add(new Property(PropertyName.Rotation, "Rotation"));
            }

            void CreateChannelTargetWithScale(List<Property> properties, Runtime.Channel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.SCALE,
                };
                properties.Add(new Property(PropertyName.Scale, "Scale"));
            }

            void CreateLinearSampler(List<Property> properties, Runtime.Channel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        0.5f,
                        1.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3( 0, 0, 0),
                        new Vector3( 1, 1, 1),
                        new Vector3( 0, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void CreateStepSampler(List<Property> properties, Runtime.Channel channel)
            {
                channel.Sampler = new Runtime.StepSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        0.5f,
                        1.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3( 0, 0, 0),
                        new Vector3( 1, 1, 1),
                        new Vector3( 0, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Step"));
            }

            void CreateCubicSplineSampler(List<Property> properties, Runtime.Channel channel)
            {
                channel.Sampler = new Runtime.CubicSplineSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        0.5f,
                        1.0f,
                    },
                    new List<Runtime.CubicSplineSampler<Vector3>.Key>
                    {
                        new Runtime.CubicSplineSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(0,0, 0),
                            Value = new Vector3(1,1, 1),
                            OutTangent = new Vector3(1,1, 1)
                        },
                        new Runtime.CubicSplineSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(1,1, 1),
                            Value = new Vector3(0.1f,0.1f, 0.1f),
                            OutTangent = new Vector3(1,1, 1)
                        },
                        new Runtime.CubicSplineSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(1,1, 1),
                            Value = new Vector3(0.1f,0.1f, 0.1f),
                            OutTangent = new Vector3(1,1, 1)
                        }
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            void CreateLinearSamplerForRotation(List<Property> properties, Runtime.Channel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new List<float>
                    {
                        0.0f,
                        0.5f,
                        1.0f,
                    },
                    new List<Quaternion>
                    {
                        new Quaternion(0, 0, 0, 0),
                        new Quaternion(1, 1, 1, 1),
                        new Quaternion(0, 0, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void CreateSamplerStartsAboveZero(List<Property> properties, Runtime.Channel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new List<float>
                    {
                        0.0f,
                        0.5f,
                        1.0f,
                    },
                    new List<Quaternion>
                    {
                        new Quaternion(0.5f, 0.5f, 0.5f, 0.5f),
                        new Quaternion(1, 1, 1, 1),
                        new Quaternion(0, 0, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void CreateCubicSplineSamplerForRotation(List<Property> properties, Runtime.Channel channel)
            {
                channel.Sampler = new Runtime.CubicSplineSampler<Quaternion>(
                    new List<float>
                    {
                        0.0f,
                        0.5f,
                        1.0f,
                    },
                    new List<Runtime.CubicSplineSampler<Quaternion>.Key>
                    {
                        new Runtime.CubicSplineSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0, 0, 0, 0),
                            Value = new Quaternion(1, 1, 1, 1),
                            OutTangent = new Quaternion(1, 1, 1, 1)
                        },
                        new Runtime.CubicSplineSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(1, 1, 1, 1),
                            Value = new Quaternion(0.1f, 0.1f, 0.1f, 0.1f),
                            OutTangent = new Quaternion(1, 1, 1, 1)
                        },
                        new Runtime.CubicSplineSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(1, 1, 1, 1),
                            Value = new Quaternion(0.1f, 0.1f, 0.1f, 0.1f),
                            OutTangent = new Quaternion(1, 1, 1, 1)
                        }
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            void CreateMultipleChannelsWithUniqueTargets(List<Property> properties, List<Runtime.Channel> channels, Runtime.Node node)
            {
                channels.Clear();
                channels.AddRange( new List<Runtime.Channel>
                {
                    new Runtime.Channel(),
                    new Runtime.Channel(),
                    new Runtime.Channel(),
                });

                CreateChannelTargetWithTranslation(properties, channels[0], node);
                CreateChannelTargetWithRotation(properties, channels[1], node);
                CreateChannelTargetWithScale(properties, channels[2], node);

                CreateLinearSampler(properties, channels[0]);
                CreateLinearSamplerForRotation(properties, channels[1]);
                CreateLinearSampler(properties, channels[2]);
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, channels, node) => {
                    CreateChannelTargetWithTranslation(properties, channels[0], node);
                    CreateLinearSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateChannelTargetWithRotation(properties, channels[0], node);
                    CreateLinearSamplerForRotation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateChannelTargetWithScale(properties, channels[0], node);
                    CreateLinearSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateChannelTargetWithTranslation(properties, channels[0], node);
                    CreateStepSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateChannelTargetWithTranslation(properties, channels[0], node);
                    CreateCubicSplineSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateChannelTargetWithRotation(properties, channels[0], node);
                    CreateCubicSplineSamplerForRotation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateMultipleChannelsWithUniqueTargets(properties, channels, node);
                }),
                CreateModel((properties, channels, node) => {
                    // Curve that doesn't start at zero
                    CreateChannelTargetWithRotation(properties, channels[0], node);
                    CreateSamplerStartsAboveZero(properties, channels[0]);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
