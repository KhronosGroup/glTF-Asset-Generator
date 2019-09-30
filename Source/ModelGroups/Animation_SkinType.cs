using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Animation_SkinType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_SkinType;

        public Animation_SkinType(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                List<Node> nodes = Nodes.CreateFoldingPlaneSkin("skinA", 2, 3);
                var animations = new List<Animation>();
                Runtime.MeshPrimitive meshPrimitive = nodes[0].Mesh.MeshPrimitives.First();
                var closeCameraTranslation = new Manifest.Camera(new Vector3(0.5f, 0.0f, 0.6f));

                // Apply the common properties to the gltf.
                AnimateWithRotation(animations, nodes);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = nodes
                    }, animations: animations),
                    Animated = true,
                    Camera = closeCameraTranslation,
                };
            }

            void AnimateWithRotation(List<Animation> animations, List<Node> nodes)
            {
                animations.Add(
                    new Animation
                    {
                        Channels = new List<AnimationChannel>
                        {
                            new AnimationChannel
                            {
                                Target = new AnimationChannelTarget
                                {
                                    Node = nodes[1].Children.First(),
                                    Path = AnimationChannelTargetPath.Rotation,
                                }
                            }
                        }
                    }
                );

                animations[0].Channels.First().Sampler = new AnimationSampler
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
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                        Quaternion.Identity,
                    }),
                };
            }

            void JointsAreByte(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Joints.OutputType = DataType.UnsignedByte;
            }

            void JointsAreShort(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Joints.OutputType = DataType.UnsignedShort;
            }

            void WeightsAreFloat(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Weights.OutputType = DataType.Float;
            }

            void WeightsAreByte(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Weights.OutputType = DataType.NormalizedUnsignedByte;
            }

            void WeightsAreShort(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Weights.OutputType = DataType.NormalizedUnsignedShort;
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    JointsAreByte(meshPrimitive);
                    WeightsAreFloat(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Float"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    JointsAreByte(meshPrimitive);
                    WeightsAreByte(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Byte"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    JointsAreByte(meshPrimitive);
                    WeightsAreShort(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Short"));
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    JointsAreShort(meshPrimitive);
                    WeightsAreFloat(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Short"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Float"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
