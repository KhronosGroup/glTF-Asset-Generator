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

            Model CreateModel(AnimationSampler.ComponentTypeEnum samplerOutputComponentType, string samplerOutputComponentTypeDisplayValue)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage },
                    },
                };
                var node = new Runtime.Node
                {
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            cubeMeshPrimitive
                        }
                    }
                };
                var channel = new Runtime.AnimationChannel
                {
                    Target =  new Runtime.AnimationChannelTarget
                    {
                        Node = node,
                        Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                    },
                    Sampler = new Runtime.LinearAnimationSampler<Quaternion>
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
                            Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90), 0.0f, 0.0f),
                            Quaternion.Identity,
                            Quaternion.CreateFromYawPitchRoll(-FloatMath.ToRadians(90), 0.0f, 0.0f),
                            Quaternion.Identity,
                            Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90), 0.0f, 0.0f),
                        },
                        outputComponentType: samplerOutputComponentType
                    )
                };

                // Apply the properties that are specific to this gltf.
                properties.Add(new Property(PropertyName.SamplerOutputComponentType, samplerOutputComponentTypeDisplayValue));

                // Create the gltf object.
                Runtime.GLTF gltf = CreateGLTF(() => new Runtime.Scene
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
                        Channels = new List<Runtime.AnimationChannel>
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
                CreateModel(AnimationSampler.ComponentTypeEnum.FLOAT, "Float"),
                CreateModel(AnimationSampler.ComponentTypeEnum.NORMALIZED_BYTE,"Byte"),
                CreateModel(AnimationSampler.ComponentTypeEnum.NORMALIZED_SHORT, "Short"),
            };

            GenerateUsedPropertiesList();
        }
    }
}
