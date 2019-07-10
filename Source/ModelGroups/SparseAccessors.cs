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
                var meshPrimitive0 = MeshPrimitive.CreateCube();
                var meshPrimitive1 = MeshPrimitive.CreateCube();

                var material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImageCube }
                    }
                };
                meshPrimitive0.Material = material;
                meshPrimitive1.Material = material;

                // Offsets the positions of the mesh primitives so they don't overlap. This is done because animation translations override node translations.
                meshPrimitive0.Positions = meshPrimitive0.Positions.Select(position => { return new Vector3(position.X - 0.4f, position.Y, position.Z); } );
                meshPrimitive1.Positions = meshPrimitive1.Positions.Select(position => { return new Vector3(position.X + 0.4f, position.Y, position.Z); } );

                var nodes = new List<Runtime.Node>
                {
                    new Runtime.Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new List<Runtime.MeshPrimitive>
                            {
                                meshPrimitive0
                            }
                        }
                    },
                    new Runtime.Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new List<Runtime.MeshPrimitive>
                            {
                                meshPrimitive1
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
                1.0f,
                2.0f,
            };

            var SamplerInputSparse = new[]
            {
                1.5f,
            };

            var SamplerOutput = new[]
            {
                new Vector3(0.0f,  0.2f, 0.0f),
                new Vector3(0.0f, -0.2f, 0.0f),
                new Vector3(0.0f,  0.2f, 0.0f),
            };

            var SamplerOutputSparse = new[]
            {
                new Vector3(0.2f, -0.2f, 0.0f),
            };

            var BasicSampler = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);

            List<Runtime.AnimationChannel> CreateChannels(List<Runtime.Node> nodes, Runtime.AnimationSampler sampler0, Runtime.AnimationSampler sampler1)
            {
                return new List<Runtime.AnimationChannel>
                {
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = nodes[0],
                            Path = Runtime.AnimationChannelTarget.PathEnum.TRANSLATION
                        },
                        Sampler = sampler0
                    },
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = nodes[1],
                            Path = Runtime.AnimationChannelTarget.PathEnum.TRANSLATION
                        },
                        Sampler = sampler1
                    },
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputSparse, SamplerOutput);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputSparse, SamplerOutput);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputSparse, SamplerOutput);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_SHORT,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_SHORT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    // DEBUG - NYI. Which bufferview is omitted? How does that work?
                    // Guess: No base is used, so it's assuming origin is the base.
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[1].Sampler).OutputKeys,
                        null,
                        SamplerOutput.Count()
                    );
                    sparseDictionary.Add(SamplerOutputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.IndicesComponentType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.Description, "Does not have a bufferView."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
