using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal class Animation_SkinType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_SkinType;

        public Animation_SkinType(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive.JointComponentTypeEnum, Runtime.MeshPrimitive.WeightComponentTypeEnum> setProperties)
            {
                var properties = new List<Property>();
                var planeSkinScene = Scene.CreatePlaneWithSkin();
                var jointComponentType = planeSkinScene.Nodes.First().Mesh.MeshPrimitives.First().JointComponentType;
                var weightComponentType = planeSkinScene.Nodes.First().Mesh.MeshPrimitives.First().WeightComponentType; ;

                // Create the gltf object
                Runtime.GLTF gltf = CreateGLTF(() => planeSkinScene);

                // Apply the common properties to the gltf.
                AnimateWithRotation(gltf);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, jointComponentType, weightComponentType);

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
            }

            void AnimateWithRotation(Runtime.GLTF gltf)
            {
                gltf.Animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = new List<Runtime.AnimationChannel>
                        {
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = gltf.Scenes.First().Nodes.ElementAt(1).Children.First(),
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            }
                        }
                    }
                };
                var quarterTurn = (FloatMath.Pi / 2);
                gltf.Animations.First().Channels.First().Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
                    {
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                        Quaternion.Identity,
                    });
            }

            void JointsAreByte(Runtime.MeshPrimitive.JointComponentTypeEnum JointComponentType)
            {
                JointComponentType = Runtime.MeshPrimitive.JointComponentTypeEnum.UNSIGNED_BYTE;
            }

            void JointsAreShort(Runtime.MeshPrimitive.JointComponentTypeEnum JointComponentType)
            {
                JointComponentType = Runtime.MeshPrimitive.JointComponentTypeEnum.UNSIGNED_SHORT;
            }

            void WeightsAreFloat(Runtime.MeshPrimitive.WeightComponentTypeEnum weightComponentType)
            {
                weightComponentType = Runtime.MeshPrimitive.WeightComponentTypeEnum.FLOAT;
            }

            void WeightsAreByte(Runtime.MeshPrimitive.WeightComponentTypeEnum weightComponentType)
            {
                weightComponentType = Runtime.MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE;
            }

            void WeightsAreShort(Runtime.MeshPrimitive.WeightComponentTypeEnum weightComponentType)
            {
                weightComponentType = Runtime.MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT;
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, jointComponentType, weightComponentType) => {
                    JointsAreByte(jointComponentType);
                    WeightsAreFloat(weightComponentType);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Float"));
                }),
                CreateModel((properties, jointComponentType, weightComponentType) => {
                    JointsAreByte(jointComponentType);
                    WeightsAreByte(weightComponentType);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Byte"));
                }),
                CreateModel((properties, jointComponentType, weightComponentType) => {
                    JointsAreByte(jointComponentType);
                    WeightsAreShort(weightComponentType);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Short"));
                }),
                CreateModel((properties, jointComponentType, weightComponentType) => {
                    JointsAreShort(jointComponentType);
                    WeightsAreFloat(weightComponentType);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Short"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Float"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
