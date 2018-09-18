﻿using System;
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

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var nodes = Nodes.CreatePlaneWithSkinA();
                var animations = new List<Runtime.Animation>();
                var meshPrimitive = nodes[0].Mesh.MeshPrimitives.First();
                var jointComponentType = meshPrimitive.JointComponentType;
                var weightComponentType = meshPrimitive.WeightComponentType;

                // Apply the common properties to the gltf.
                AnimateWithRotation(animations, nodes);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene()
                    {
                        Nodes = nodes
                    }, animations: animations),
                };
            }

            void AnimateWithRotation(List<Runtime.Animation> animations, List<Runtime.Node> nodes)
            {
                animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = new List<Runtime.AnimationChannel>
                        {
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = nodes[1].Children.First(),
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            }
                        }
                    }
                };
                var quarterTurn = (FloatMath.Pi / 2);
                animations[0].Channels.First().Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
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

            void JointsAreByte(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.JointComponentType = Runtime.MeshPrimitive.JointComponentTypeEnum.UNSIGNED_BYTE;
            }

            void JointsAreShort(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.JointComponentType = Runtime.MeshPrimitive.JointComponentTypeEnum.UNSIGNED_SHORT;
            }

            void WeightsAreFloat(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.WeightComponentType = Runtime.MeshPrimitive.WeightComponentTypeEnum.FLOAT;
            }

            void WeightsAreByte(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.WeightComponentType = Runtime.MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE;
            }

            void WeightsAreShort(Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.WeightComponentType = Runtime.MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT;
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) => {
                    JointsAreByte(meshPrimitive);
                    WeightsAreFloat(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Float"));
                }),
                CreateModel((properties, meshPrimitive) => {
                    JointsAreByte(meshPrimitive);
                    WeightsAreByte(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Byte"));
                }),
                CreateModel((properties, meshPrimitive) => {
                    JointsAreByte(meshPrimitive);
                    WeightsAreShort(meshPrimitive);
                    properties.Add(new Property(PropertyName.JointsComponentType, "Byte"));
                    properties.Add(new Property(PropertyName.WeightComponentType, "Short"));
                }),
                CreateModel((properties, meshPrimitive) => {
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
