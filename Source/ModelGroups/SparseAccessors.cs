using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using static glTFLoader.Schema.Sampler;
using static AssetGenerator.Runtime.AnimationChannelTarget.PathEnum;

namespace AssetGenerator
{
    internal class SparseAccessors : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.SparseAccessors;

        public SparseAccessors(List<string> imageList)
        {
            Runtime.Image baseColorTextureImageCube = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.Animation, List<Runtime.Node>, Dictionary<IEnumerable, Runtime.AccessorSparse>> setProperties)
            {
                var properties = new List<Property>();
                var material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImageCube }
                    }
                };
                var meshPrimitive = MeshPrimitive.CreateCube();
                meshPrimitive.Material = material;
                var nodes = new List<Runtime.Node>
                {
                    new Runtime.Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new List<Runtime.MeshPrimitive>
                            {
                                meshPrimitive
                            }
                        }
                    },
                    new Runtime.Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new List<Runtime.MeshPrimitive>
                            {
                                meshPrimitive
                            }
                        }
                    }
                };
                var animation = new Runtime.Animation();
                var sparseDictionary = new Dictionary<IEnumerable, Runtime.AccessorSparse>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animation, nodes, sparseDictionary);

                // Create the gltf object.
                var model = new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF
                    (
                        () => new Runtime.Scene
                        {
                            Nodes = nodes
                        }, 
                        animations: new List<Runtime.Animation> { animation },
                        referenceToSparse: sparseDictionary
                    ),
                    Animated = true,
                };

                return model;
            }

            var SamplerInputLinear = new[]
            {
                0.0f,
                1.0f,
                2.0f,
                3.0f,
                4.0f,
            };

            var SamplerInputSparseValues = new[]
            {
                0.5f,
                1.0f,
            };

            var SamplerOutput = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
            };

            var BasicSampler = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var channels = new List<Runtime.AnimationChannel>
                    {
                        new Runtime.AnimationChannel
                        {
                            Target = new Runtime.AnimationChannelTarget
                            {
                                Node = nodes[0],
                                Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION
                            },
                            Sampler = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput)
                        },
                        new Runtime.AnimationChannel
                        {
                            Target = new Runtime.AnimationChannelTarget
                            {
                                Node = nodes[1],
                                Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION
                            },
                            Sampler = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputSparseValues, SamplerOutput)
                        },
                    };
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>(new List<int> { 1, 2 },
                        Runtime.AccessorSparse.IndicesComponentTypeEnum.UNSIGNED_INT, channels[1].Sampler.InputKeys, Runtime.AccessorSparse.ValuesComponentTypeEnum.FLOAT, channels[0].Sampler.InputKeys, false);
                    sparseDictionary.Add(SamplerInputSparseValues, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                // CreateModel((properties) =>
                // {
                //     properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                //     properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                // }),
                // CreateModel((properties) =>
                // {
                //     properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                //     properties.Add(new Property(PropertyName.Description, "Does not have a bufferView."));
                // }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
