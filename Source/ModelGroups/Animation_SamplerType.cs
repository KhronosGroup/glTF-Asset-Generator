using System;
using System.Collections.Generic;
using System.Numerics;
using AnimationSampler = AssetGenerator.Runtime.AnimationSampler;

namespace AssetGenerator
{
    internal class Animation_SamplerType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_SamplerType;

        public Animation_SamplerType(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

            CommonProperties.Add(new Property(PropertyName.Target, "Rotation"));
            CommonProperties.Add(new Property(PropertyName.Interpolation, "Linear"));

            Model CreateModel(Action<List<Property>, AnimationSampler> setProperties)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();
                var quarterTurn = (FloatMath.Pi / 2.0f);

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material()
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                    {
                        BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                    },
                };
                var node = new Runtime.Node()
                {
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            cubeMeshPrimitive
                        }
                    }
                };
                var channel = new Runtime.AnimationChannel()
                {
                    Target =  new Runtime.AnimationChannelTarget
                    {
                        Node = node,
                        Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                    },
                    Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
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
                        }
                    )
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, channel.Sampler);

                // Create the gltf object.
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
                        Channels = new List<Runtime.AnimationChannel>()
                        {
                            channel
                        }
                    }
                };
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                    Animated = true,
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, sampler) => {
                    sampler.OutputComponentType = AnimationSampler.ComponentTypeEnum.FLOAT;
                    properties.Add(new Property(PropertyName.SamplerComponentType, "Float"));
                }),
                CreateModel((properties, sampler) => {
                    sampler.OutputComponentType = AnimationSampler.ComponentTypeEnum.NORMALIZED_BYTE;
                    properties.Add(new Property(PropertyName.SamplerComponentType, "Byte"));
                }),
                CreateModel((properties, sampler) => {
                    sampler.OutputComponentType = AnimationSampler.ComponentTypeEnum.NORMALIZED_SHORT;
                    properties.Add(new Property(PropertyName.SamplerComponentType, "Short"));
                }),
                // Commenting these models out for now as rotations using unsigned types must be positive, which doesn't seem like a real world case.
                // CreateModel((properties, sampler) => {
                //     sampler.OutputComponentType = AnimationSampler.ComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE;
                //     properties.Add(new Property(PropertyName.SamplerComponentType, "uByte"));
                // }),
                // CreateModel((properties, sampler) => {
                //     sampler.OutputComponentType = AnimationSampler.ComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT;
                //     properties.Add(new Property(PropertyName.SamplerComponentType, "uShort"));
                // }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
