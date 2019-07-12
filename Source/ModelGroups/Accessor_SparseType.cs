using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static AssetGenerator.Runtime.AccessorSparse;
using static AssetGenerator.Runtime.AnimationChannelTarget;
using static AssetGenerator.Runtime.AnimationSampler;

namespace AssetGenerator
{
    internal class Accessor_SparseType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Accessor_SparseType;

        public Accessor_SparseType(List<string> imageList)
        {
            Runtime.Image baseColorTextureImageCube = UseTexture(imageList, "BaseColor_Cube");
            UseFigure(imageList, "SparseAccessor_Input-Base");
            UseFigure(imageList, "SparseAccessor_Input-Sparse");
            UseFigure(imageList, "SparseAccessor_Input-Modified");
            UseFigure(imageList, "SparseAccessor_OutputTransform-Base");
            UseFigure(imageList, "SparseAccessor_OutputTransform-Sparse");
            UseFigure(imageList, "SparseAccessor_OutputTransform-Modified");
            UseFigure(imageList, "SparseAccessor_OutputRotation-Base");
            UseFigure(imageList, "SparseAccessor_OutputRotation-Sparse");
            UseFigure(imageList, "SparseAccessor_OutputRotation-Modified");
            UseFigure(imageList, "SparseAccessor_NoBufferView-Base");
            UseFigure(imageList, "SparseAccessor_NoBufferView-Sparse");
            UseFigure(imageList, "SparseAccessor_NoBufferView-Modified");

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
                meshPrimitive0.Positions = meshPrimitive0.Positions.Select(position => { return new Vector3(position.X - 0.4f, position.Y, position.Z); });
                meshPrimitive1.Positions = meshPrimitive1.Positions.Select(position => { return new Vector3(position.X + 0.4f, position.Y, position.Z); });

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

                // Create the gltf object.
                return new Model
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
                    Camera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.5f)) 
                };
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

            var SamplerOutputTranslation = new[]
            {
                new Vector3(0.0f,  0.2f, 0.0f),
                new Vector3(0.0f, -0.2f, 0.0f),
                new Vector3(0.0f,  0.2f, 0.0f),
            };

            var SamplerOutputTranslationSparse = new[]
            {
                new Vector3(0.0f, 0.1f, 0.0f),
            };

            var SamplerOutputRotation = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(45.0f), 0.0f),
            };

            var SamplerOutputRotationSparse = new Quaternion[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
            };

            List<Runtime.AnimationChannel> CreateChannels(List<Runtime.Node> nodes, Runtime.AnimationSampler sampler0, Runtime.AnimationSampler sampler1)
            {
                return new List<Runtime.AnimationChannel>
                {
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = nodes[0],
                        },
                        Sampler = sampler0
                    },
                    new Runtime.AnimationChannel
                    {
                        Target = new Runtime.AnimationChannelTarget
                        {
                            Node = nodes[1],
                        },
                        Sampler = sampler1
                    },
                };
            }

            void SetAnimationPaths(List<Runtime.AnimationChannel> channels, PathEnum path, List<Property> properties)
            {
                foreach (var channel in channels)
                {
                    channel.Target.Path = path;
                }
            }

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslation);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputSparse, SamplerOutputTranslation);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.TRANSLATION, properties);

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.ValueType, sparse.ValuesComponentType));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslation);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputSparse, SamplerOutputTranslation);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.TRANSLATION, properties);

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.ValueType, sparse.ValuesComponentType));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslation);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputSparse, SamplerOutputTranslation);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.TRANSLATION, properties);

                    var sparse = new Runtime.AccessorSparse<float>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_SHORT,
                        ValuesComponentTypeEnum.FLOAT,
                        channels[1].Sampler.InputKeys,
                        channels[0].Sampler.InputKeys
                    );
                    sparseDictionary.Add(SamplerInputSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.ValueType, sparse.ValuesComponentType));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputRotation, ComponentTypeEnum.NORMALIZED_BYTE);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputRotationSparse, ComponentTypeEnum.NORMALIZED_BYTE);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    var sparse = new Runtime.AccessorSparse<Quaternion>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.NORMALIZED_BYTE,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputRotationSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.ValueType, sparse.ValuesComponentType));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputRotation, ComponentTypeEnum.NORMALIZED_SHORT);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputRotationSparse, ComponentTypeEnum.NORMALIZED_SHORT);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    var sparse = new Runtime.AccessorSparse<Quaternion>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.NORMALIZED_SHORT,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Quaternion>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputRotationSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.ValueType, sparse.ValuesComponentType));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    nodes.RemoveAt(0);
                    var sampler = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslationSparse);
                    var channels = new List<Runtime.AnimationChannel>
                    {
                        new Runtime.AnimationChannel
                        {
                            Target = new Runtime.AnimationChannelTarget
                            {
                                Node = nodes[0],
                                Path = PathEnum.TRANSLATION
                            },
                            Sampler = sampler
                        },
                    };
                    animation.Channels = channels;

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[0].Sampler).OutputKeys,
                        null,
                        SamplerOutputTranslation.Count(),
                        "Sparse Animation Sampler Output"
                    );
                    sparseDictionary.Add(SamplerOutputTranslationSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, sparse.IndicesComponentType));
                    properties.Add(new Property(PropertyName.ValueType, sparse.ValuesComponentType));
                    properties.Add(new Property(PropertyName.BufferView, ""));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
