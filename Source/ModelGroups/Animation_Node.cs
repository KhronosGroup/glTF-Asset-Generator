using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class Animation_Node : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Node;

        public Animation_Node(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.AnimationChannel>, Runtime.Node> setProperties)
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

            void SetTranslationChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.TRANSLATION,
                };
                properties.Add(new Property(PropertyName.Targets, "Translation"));
            }

            void SetRotationChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.ROTATION,
                };
                properties.Add(new Property(PropertyName.Targets, "Rotation"));
            }

            void SetScaleChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.ChannelTarget
                {
                    Node = node,
                    Path = Runtime.ChannelTarget.PathEnum.SCALE,
                };
                properties.Add(new Property(PropertyName.Targets, "Scale"));
            }

            void SetLinearSamplerForTranslation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetLinearSamplerForScale(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(0.8f, 0.8f, 0.8f),
                        new Vector3(1.2f, 1.2f, 1.2f),
                        new Vector3(0.8f, 0.8f, 0.8f),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
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
                        Quaternion.CreateFromYawPitchRoll(-quarterTurn*2, 0, 0),
                        Quaternion.CreateFromYawPitchRoll(-quarterTurn*3, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetStepSampler(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.StepAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    },
                    new List<Vector3>
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Step"));
            }

            void SetCubicSplineSampler(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.CubicSplineAnimationSampler<Vector3>(
                    new List<float>
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new List<Runtime.CubicSplineAnimationSampler<Vector3>.Key>
                    {
                        new Runtime.CubicSplineAnimationSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(0, 0, 0),
                            Value = new Vector3(1, 1, 1),
                            OutTangent = new Vector3(1, 1, 1)
                        },
                        new Runtime.CubicSplineAnimationSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(1, 1, 1),
                            Value = new Vector3(0.1f, 0.1f, 0.1f),
                            OutTangent = new Vector3(0, 0, 0)
                        },
                        new Runtime.CubicSplineAnimationSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(1, 1, 1),
                            Value = new Vector3(0.1f, 0.1f, 0.1f),
                            OutTangent = new Vector3(1, 1, 1)
                        }
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
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
                        new Quaternion(0, 0, 0, 0),
                        Quaternion.CreateFromYawPitchRoll(quarterTurn*(-1), 0, 0),
                        new Quaternion(0, 0, 0, 0),
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0, 0),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void CreateCubicSplineSamplerForRotation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.CubicSplineAnimationSampler<Quaternion>(
                    new List<float>
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new List<Runtime.CubicSplineAnimationSampler<Quaternion>.Key>
                    {
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0, 0, 0, 0),
                            Value = new Quaternion(1, 1, 1, 1),
                            OutTangent = new Quaternion(1, 1, 1, 1)
                        },
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(1, 1, 1, 1),
                            Value = new Quaternion(0.1f, 0.1f, 0.1f, 0.1f),
                            OutTangent = new Quaternion(1, 1, 1, 1)
                        },
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(1, 1, 1, 1),
                            Value = new Quaternion(0.1f, 0.1f, 0.1f, 0.1f),
                            OutTangent = new Quaternion(1, 1, 1, 1)
                        }
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            void CreateMultipleChannelsWithUniqueTargets(List<Property> properties, List<Runtime.AnimationChannel> channels, Runtime.Node node)
            {
                // The first channel is already added as a common property.
                channels.Add(new Runtime.AnimationChannel());
                channels.Add(new Runtime.AnimationChannel());

                var targetPropertiesList = new List<Property>();
                SetTranslationChannelTarget(targetPropertiesList, channels[0], node);
                SetRotationChannelTarget(targetPropertiesList, channels[1], node);
                SetScaleChannelTarget(targetPropertiesList, channels[2], node);

                var samplerPropertiesList = new List<Property>();
                SetLinearSamplerForTranslation(samplerPropertiesList, channels[0]);
                SetLinearSamplerForRotation(samplerPropertiesList, channels[1]);
                SetLinearSamplerForScale(samplerPropertiesList, channels[2]);

                // Takes the properties created by the animation target helper functions and condenses them into a single property, then adds that property to the list of used properties.
                var targetReadmeValue = new StringBuilder();
                var samplerReadmeValue = new StringBuilder();
                for (int x = 0; x < targetPropertiesList.Count; x++)
                {
                    targetReadmeValue.Append($"{targetPropertiesList[x].ReadmeValue}<br>");
                    samplerReadmeValue.Append($"{samplerPropertiesList[x].ReadmeValue}<br>");
                }
                properties.Add(new Property(PropertyName.Targets, targetReadmeValue.ToString()));
                properties.Add(new Property(PropertyName.Interpolation, samplerReadmeValue.ToString()));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, channels, node) => {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForTranslation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetRotationChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForRotation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetScaleChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForScale(properties, channels[0]);
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
