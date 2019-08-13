using AssetGenerator.Runtime;
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
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<AnimationChannel>, Node> setProperties)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new Texture { Source = baseColorTextureImage },
                    },
                };
                var channels = new List<AnimationChannel>
                {
                    new AnimationChannel()
                };
                var node = new Node();

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
                GLTF gltf = CreateGLTF(() => new Scene
                {
                    Nodes = new[]
                    {
                        node
                    },
                });
                gltf.Animations = new[]
                {
                    new Animation
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

            void SetTranslationChannelTarget(List<Property> properties, AnimationChannel channel, Node node)
            {
                channel.Target = new AnimationChannelTarget
                {
                    Node = node,
                    Path = AnimationChannelTarget.PathEnum.TRANSLATION,
                };
                properties.Add(new Property(PropertyName.Target, channel.Target.Path.ToReadmeString()));
            }

            void SetRotationChannelTarget(List<Property> properties, AnimationChannel channel, Node node)
            {
                channel.Target = new AnimationChannelTarget
                {
                    Node = node,
                    Path = AnimationChannelTarget.PathEnum.ROTATION,
                };
                properties.Add(new Property(PropertyName.Target, channel.Target.Path.ToReadmeString()));
            }

            void SetScaleChannelTarget(List<Property> properties, AnimationChannel channel, Node node)
            {
                channel.Target = new AnimationChannelTarget
                {
                    Node = node,
                    Path = AnimationChannelTarget.PathEnum.SCALE,
                };
                properties.Add(new Property(PropertyName.Target, channel.Target.Path.ToReadmeString()));
            }

            void SetLinearSamplerForTranslation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                (
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
                    }
                );

                properties.Add(new Property(PropertyName.Interpolation, channel.Sampler.Interpolation.ToReadmeString()));
            }

            void SetLinearSamplerForScale(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                (
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
                    }
                );

                properties.Add(new Property(PropertyName.Interpolation, channel.Sampler.Interpolation.ToReadmeString()));
            }

            void SetLinearSamplerForRotation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                (
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
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(-90.0f), 0.0f, 0.0f),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                    }
                );

                properties.Add(new Property(PropertyName.Interpolation, channel.Sampler.Interpolation.ToReadmeString()));
            }

            void SetStepSamplerForTranslation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                (
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
                    },
                    Accessor.ComponentTypeEnum.FLOAT,
                    Accessor.TypeEnum.VEC3,
                    AnimationSampler.InterpolationEnum.STEP
                );

                properties.Add(new Property(PropertyName.Interpolation, channel.Sampler.Interpolation.ToReadmeString()));
            }

            void SetCubicSplineSamplerForTranslation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                (
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
                    {
                        new AnimationSampler.Key<Vector3>
                        {
                            InTangent = new Vector3(0.0f, 0.0f, 0.0f),
                            Value = new Vector3(-0.1f, 0.0f, 0.0f),
                            OutTangent = new Vector3(0.0f, 0.0f, 0.0f)
                        },
                        new AnimationSampler.Key<Vector3>
                        {
                            InTangent = new Vector3(0.0f, 0.0f, 0.0f),
                            Value = new Vector3(0.1f, 0.0f, 0.0f),
                            OutTangent = new Vector3(0.0f, -0.3f, 0.0f)
                        },
                        new AnimationSampler.Key<Vector3>
                        {
                            InTangent = new Vector3(0.0f, 0.0f, 0.0f),
                            Value = new Vector3(-0.1f, 0.0f, 0.0f),
                            OutTangent = new Vector3(0.0f, 0.0f, 0.0f)
                        }
                    },
                    Accessor.ComponentTypeEnum.FLOAT,
                    Accessor.TypeEnum.VEC3,
                    AnimationSampler.InterpolationEnum.CUBIC_SPLINE
                );

                properties.Add(new Property(PropertyName.Interpolation, channel.Sampler.Interpolation.ToReadmeString()));
            }

            void CreateCubicSplineSamplerForRotation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                (
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
                        new AnimationSampler.Key<Quaternion>
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new AnimationSampler.Key<Quaternion>
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.Identity,
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new AnimationSampler.Key<Quaternion>
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(-90.0f), 0.0f, 0.0f),
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new AnimationSampler.Key<Quaternion>
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.Identity,
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                        new AnimationSampler.Key<Quaternion>
                        {
                            InTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                            Value = Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                            OutTangent = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)
                        },
                    },
                    Accessor.ComponentTypeEnum.FLOAT,
                    Accessor.TypeEnum.VEC4,
                    AnimationSampler.InterpolationEnum.CUBIC_SPLINE
                );
                properties.Add(new Property(PropertyName.Interpolation, channel.Sampler.Interpolation.ToReadmeString()));
            }

            Models = new List<Model>
            {
                CreateModel((properties, channels, node) =>
                {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForTranslation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) =>
                {
                    SetRotationChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForRotation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) =>
                {
                    SetScaleChannelTarget(properties, channels[0], node);
                    SetLinearSamplerForScale(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) =>
                {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetStepSamplerForTranslation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) =>
                {
                    SetTranslationChannelTarget(properties, channels[0], node);
                    SetCubicSplineSamplerForTranslation(properties, channels[0]);
                }),
                CreateModel((properties, channels, node) =>
                {
                    SetRotationChannelTarget(properties, channels[0], node);
                    CreateCubicSplineSamplerForRotation(properties, channels[0]);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
