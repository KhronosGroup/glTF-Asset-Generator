using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Animation_Node : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Node;

        public Animation_Node(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Cube") };

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<AnimationChannel>, Node> setProperties)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTexture },
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
                    Path = AnimationChannelTargetPath.Translation,
                };
                properties.Add(new Property(PropertyName.Target, channel.Target.Path.ToReadmeString()));
            }

            void SetRotationChannelTarget(List<Property> properties, AnimationChannel channel, Node node)
            {
                channel.Target = new AnimationChannelTarget
                {
                    Node = node,
                    Path = AnimationChannelTargetPath.Rotation,
                };
                properties.Add(new Property(PropertyName.Target, channel.Target.Path.ToReadmeString()));
            }

            void SetScaleChannelTarget(List<Property> properties, AnimationChannel channel, Node node)
            {
                channel.Target = new AnimationChannelTarget
                {
                    Node = node,
                    Path = AnimationChannelTargetPath.Scale,
                };
                properties.Add(new Property(PropertyName.Target, channel.Target.Path.ToReadmeString()));
            }

            void SetLinearSamplerForTranslation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.Linear,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    }),
                };

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetLinearSamplerForScale(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.Linear,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        new Vector3(0.8f, 0.8f, 0.8f),
                        new Vector3(1.2f, 1.2f, 1.2f),
                        new Vector3(0.8f, 0.8f, 0.8f),
                    }),
                };

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetLinearSamplerForRotation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.Linear,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(-90.0f), 0.0f, 0.0f),
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                    }),
                };

                properties.Add(new Property(PropertyName.Interpolation, "Linear"));
            }

            void SetStepSamplerForTranslation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.Step,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    }),
                };

                properties.Add(new Property(PropertyName.Interpolation, "Step"));
            }

            void SetCubicSplineSamplerForTranslation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.CubicSpline,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),

                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, -0.3f, 0.0f),

                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                    }),
                };

                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
            }

            void CreateCubicSplineSamplerForRotation(List<Property> properties, AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.CubicSpline,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                        3.0f,
                        4.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),

                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                        Quaternion.Identity,
                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),

                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(-90.0f), 0.0f, 0.0f),
                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),

                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                        Quaternion.Identity,
                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),

                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                        Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                        new Quaternion(0.0f, 0.0f, 0.0f, 0.0f),
                    }),
                };
                properties.Add(new Property(PropertyName.Interpolation, "Cubic Spline"));
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
