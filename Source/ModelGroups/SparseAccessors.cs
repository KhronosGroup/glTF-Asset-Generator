using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using static glTFLoader.Schema.Sampler;
using static AssetGenerator.Runtime.AccessorSparse;

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
                var animated = true;
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
                var animations = new List<Runtime.Animation>
                {
                    animation
                };
                var sparseDictionary = new Dictionary<IEnumerable, Runtime.AccessorSparse>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animation, nodes, sparseDictionary);

                // If no animations are used, null out that property.
                if (animation.Channels == null)
                {
                    animations = null;
                    animated = false;
                }

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
                        animations: animations,
                        referenceToSparse: sparseDictionary
                    ),
                    Animated = animated,
                };

                return model;
            }

            var SamplerInputLinear = new[]
            {
                0.0f,
                0.5f,
                1.0f,
                1.5f,
                2.0f,
                2.5f,
                3.0f,
                3.5f,
                4.0f,
            };

            var SamplerInputSparse = new[]
            {
                1.25f,
                1.5f,
                1.75f,
            };

            var SamplerOutput = new[]
            {
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(90.0f), 0.0f, 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(-90.0f), 0.0f, 0.0f),
                Quaternion.Identity,
            };

            var SamplerOutputSparse = new[]
            {
                Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(45.0f), FloatMath.ToRadians(45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(FloatMath.ToRadians(-45.0f), FloatMath.ToRadians(-45.0f), 0.0f),
            };

            var BasicSampler = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);

            List<Runtime.AnimationChannel> CreateChannels(List<Runtime.Node> nodes, Runtime.AnimationSampler sampler0, Runtime.AnimationSampler sampler1)
            {
                return new List<Runtime.AnimationChannel>
                {
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = nodes[0],
                            Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION
                        },
                        Sampler = sampler0
                    },
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = nodes[1],
                            Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION
                        },
                        Sampler = sampler1
                    },
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputSparse, SamplerOutput);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1, 2, 3 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys,
                        false
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputSparse, SamplerOutput);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1, 2, 3 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys,
                        false
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputSparse, SamplerOutput);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1, 2, 3 },
                        IndicesComponentTypeEnum.UNSIGNED_SHORT,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys,
                        false
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Quaternion>
                    (
                        new List<int> { 2, 4 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[0].Sampler).OutputKeys,
                        false
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Quaternion>
                    (
                        new List<int> { 2, 4 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[0].Sampler).OutputKeys,
                        false
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Quaternion>
                    (
                        new List<int> { 2, 4 },
                        IndicesComponentTypeEnum.UNSIGNED_SHORT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[0].Sampler).OutputKeys,
                        false
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                    nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                // CreateModel((properties, animation, nodes, sparseDictionary) =>
                // {
                //     nodes[1].Mesh.MeshPrimitives.First().Positions = new List<Vector3>
                //     {
                //         new Vector3(-0.25f, -0.25f, 0.0f),
                //     };

                //     var sparse = new Runtime.AccessorSparse<Vector3>(new List<int> { 1 }, IndicesComponentTypeEnum.UNSIGNED_INT,
                //         nodes[1].Mesh.MeshPrimitives.First().Positions, ValuesComponentTypeEnum.FLOAT,
                //         nodes[0].Mesh.MeshPrimitives.First().Positions, false);
                //     sparseDictionary.Add(nodes[1].Mesh.MeshPrimitives.First().Positions, sparse);

                //     nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);
                //     nodes[1].Translation = new Vector3(0.5f, 0.0f, 0.0f);

                //     properties.Add(new Property(PropertyName.SparseAccessor, "Position"));
                //     properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                // }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
