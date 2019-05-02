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
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

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

                // Create the gltf object.
                node.Mesh = new Runtime.Mesh
                {
                    MeshPrimitives = new[]
                    {
                        cubeMeshPrimitive
                    }
                };
                Runtime.GLTF gltf = CreateGLTF(() => new Runtime.Scene()
                {
                    Nodes = new[]
                    {
                        node
                    },
                });
                gltf.Animations = new[]
                {
                    new Runtime.Animation
                    {
                        Channels = channels
                    }
                };
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                    Animated = true,
                };
            }

            void SetTranslationChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.TRANSLATION,
                };
                properties.Add(new Property(PropertyName.Target, "Translation"));
            }

            void SetRotationChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                };
                properties.Add(new Property(PropertyName.Target, "Rotation"));
            }

            void SetScaleChannelTarget(List<Property> properties, Runtime.AnimationChannel channel, Runtime.Node node)
            {
                channel.Target = new Runtime.AnimationChannelTarget
                {
                    Node = node,
                    Path = Runtime.AnimationChannelTarget.PathEnum.SCALE,
                };
                properties.Add(new Property(PropertyName.Target, "Scale"));
            }

            void SetLinearSamplerForTranslation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.LinearAnimationSampler<Vector3>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
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
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
                    {
                        new Vector3(0.8f, 0.8f, 0.8f),
                        new Vector3(1.2f, 1.2f, 1.2f),
                        new Vector3(0.8f, 0.8f, 0.8f),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetLinearSamplerForRotation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                var quarterTurn = (FloatMath.Pi / 2.0f);
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
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0.0f, 0.0f),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(-quarterTurn, 0.0f, 0.0f),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(quarterTurn, 0.0f, 0.0f),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetStepSamplerForTranslation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.StepAnimationSampler<Vector3>(
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
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Step"));
            }

            void SetCubicSplineSamplerForTranslation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                channel.Sampler = new Runtime.CubicSplineAnimationSampler<Vector3>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
                    {
                        new Runtime.CubicSplineAnimationSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(0.0f, 0.0f, 0.0f),
                            Value = new Vector3(-0.1f, 0.0f, 0.0f),
                            OutTangent = new Vector3(0.0f, 0.0f, 0.0f)
                        },
                        new Runtime.CubicSplineAnimationSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(0.0f, 0.0f, 0.0f),
                            Value = new Vector3(0.1f, 0.0f, 0.0f),
                            OutTangent = new Vector3(0.0f, -0.3f, 0.0f)
                        },
                        new Runtime.CubicSplineAnimationSampler<Vector3>.Key
                        {
                            InTangent = new Vector3(0.0f, 0.0f, 0.0f),
                            Value = new Vector3(-0.1f, 0.0f, 0.0f),
                            OutTangent = new Vector3(0.0f, 0.0f, 0.0f)
                        }
                    });

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            void CreateCubicSplineSamplerForRotation(List<Property> properties, Runtime.AnimationChannel channel)
            {
                var quarterTurn = (FloatMath.Pi / 2.0f);
                channel.Sampler = new Runtime.CubicSplineAnimationSampler<Quaternion>(
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
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.CreateFromYawPitchRoll(quarterTurn, 0.0f, 0.0f),
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.Identity,
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.CreateFromYawPitchRoll(-quarterTurn, 0.0f, 0.0f),
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.Identity,
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new Runtime.CubicSplineAnimationSampler<Quaternion>.Key
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.CreateFromYawPitchRoll(quarterTurn, 0.0f, 0.0f),
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                    });
                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            Models = new List<Model>
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
                    SetStepSamplerForTranslation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetCubicSplineSamplerForTranslation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) => {
                    SetRotationChannelTarget(properties, channels[0], node);
                    CreateCubicSplineSamplerForRotation(properties, channels[0]);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
