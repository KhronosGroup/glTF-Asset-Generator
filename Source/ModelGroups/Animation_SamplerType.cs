using AssetGenerator.Runtime;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Animation_SamplerType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_SamplerType;

        public Animation_SamplerType(List<string> imageList)
        {
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Cube");

            CommonProperties.Add(new Property(PropertyName.Target, "Rotation"));
            CommonProperties.Add(new Property(PropertyName.Interpolation, "Linear"));

            Model CreateModel(Accessor.ComponentTypeEnum samplerOutputComponentType)
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
                var node = new Node
                {
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            cubeMeshPrimitive
                        }
                    }
                };
                var channel = new AnimationChannel
                {
                    Target = new AnimationChannelTarget
                    {
                        Node = node,
                        Path = AnimationChannelTarget.PathEnum.ROTATION,
                    },
                    Sampler = new AnimationSampler
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
                        },
                        samplerOutputComponentType,
                        Accessor.TypeEnum.VEC4,
                        AnimationSampler.InterpolationEnum.LINEAR
                    )
                };

                // Apply the properties that are specific to this gltf.
                properties.Add(new Property(PropertyName.SamplerOutputComponentType, samplerOutputComponentType.ToReadmeString()));

                // Create the gltf object.
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
                        Channels = new List<AnimationChannel>
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
                CreateModel(Accessor.ComponentTypeEnum.FLOAT),
                CreateModel(Accessor.ComponentTypeEnum.BYTE),
                CreateModel(Accessor.ComponentTypeEnum.SHORT),
            };

            GenerateUsedPropertiesList();
        }
    }
}
