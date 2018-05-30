using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Animation_Node : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Node;

        public Animation_Node(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Channel>, Runtime.Node> setProperties)
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
                var channels = new List<Runtime.Channel>()
                {
                    new Runtime.Channel()
                };
                var node = new Runtime.Node();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, channels, node);

                // Create the gltf object
                node.Mesh = new Runtime.Mesh
                {
                    MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        cubeMeshPrimitive
                    }
                };
                Runtime.GLTF gltf = CreateGLTF(() => new Runtime.Scene()
                {
                    Nodes = new List<Runtime.Node>
                    {
                        node
                    },
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

            void SetTranslationChannelTarget(List<Property> properties, Runtime.Channel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.TRANSLATION,
                };
                properties.Add(new Property(PropertyName.Translation, ":white_check_mark:"));
            }

            void SetRotationChannelTarget(List<Property> properties, Runtime.Channel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.ROTATION,
                };
                properties.Add(new Property(PropertyName.Rotation, ":white_check_mark:"));
            }

            void SetScaleChannelTarget(List<Property> properties, Runtime.Channel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.SCALE,
                };
                properties.Add(new Property(PropertyName.Scale, ":white_check_mark:"));
            }

            void SetLinearSampler(List<Property> properties, Runtime.Channel channel)
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
                        new Vector3(0, 0, 0),
                        new Vector3(1, 1, 1),
                        new Vector3(0, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetStepSampler(List<Property> properties, Runtime.Channel channel)
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
                        new Vector3(0, 0, 0),
                        new Vector3(1, 1, 1),
                        new Vector3(0, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Step"));
            }

            void SetCubicSplineSampler(List<Property> properties, Runtime.Channel channel)
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
                            InTangent = new Vector3(0, 0, 0),
                            Value = new Vector3(1, 1, 1),
                            OutTangent = new Vector3(1, 1, 1)
                        },
                        new Runtime.CubicSplineSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(1, 1, 1),
                            Value = new Vector3(0.1f, 0.1f, 0.1f),
                            OutTangent = new Vector3(1,1, 1)
                        },
                        new Runtime.CubicSplineSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(1, 1, 1),
                            Value = new Vector3(0.1f, 0.1f, 0.1f),
                            OutTangent = new Vector3(1, 1, 1)
                        }
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            void SetLinearSamplerForRotation(List<Property> properties, Runtime.Channel channel)
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
                // The first channel is already added as a common property.
                channels.Add(new Runtime.Channel());
                channels.Add(new Runtime.Channel());

                SetTranslationChannelTarget(properties, channels[0], node);
                SetRotationChannelTarget(properties, channels[1], node);
                SetScaleChannelTarget(properties, channels[2], node);

                SetLinearSampler(properties, channels[0]);
                SetLinearSamplerForRotation(properties, channels[1]);
                SetLinearSampler(properties, channels[2]);
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, channels, node) => {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetLinearSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetRotationChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForRotation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetScaleChannelTarget(properties, channels[0], node);
                    SetLinearSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetStepSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetCubicSplineSampler(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetRotationChannelTarget(properties, channels[0], node);
                    CreateCubicSplineSamplerForRotation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    CreateMultipleChannelsWithUniqueTargets(properties, channels, node);
                }),
                CreateModel((properties, channels, node) => {
                    // Curve that doesn't start at zero
                    SetRotationChannelTarget(properties, channels[0], node);
                    CreateSamplerStartsAboveZero(properties, channels[0]);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
