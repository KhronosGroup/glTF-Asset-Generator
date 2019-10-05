using AssetGenerator.Runtime;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Animation_SamplerType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_SamplerType;

        public Animation_SamplerType(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Cube") };

            CommonProperties.Add(new Property(PropertyName.Target, "Rotation"));
            CommonProperties.Add(new Property(PropertyName.Interpolation, "Linear"));

            Model CreateModel(DataType outputType)
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
                        Path = AnimationChannelTargetPath.Rotation,
                    },
                    Sampler = new AnimationSampler
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
                        }, outputType),
                    },
                };

                // Apply the properties that are specific to this gltf.
                properties.Add(new Property(PropertyName.SamplerOutputComponentType, outputType.ToReadmeString()));

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
                CreateModel(DataType.Float),
                CreateModel(DataType.NormalizedByte),
                CreateModel(DataType.NormalizedShort),
            };

            GenerateUsedPropertiesList();
        }
    }
}
