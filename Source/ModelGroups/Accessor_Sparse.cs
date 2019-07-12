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
    internal class Accessor_Sparse : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Accessor_Sparse;

        public Accessor_Sparse(List<string> imageList)
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

                // If no animations are used, null out that property.
                if (animation.Channels == null)
                {
                    animations = null;
                    animated = false;
                }

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
                new Vector3(0.2f, -0.2f, 0.0f),
            };

            var SamplerOutputRotation = new[]
            {
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
            };

            var SamplerOutputRotationSparse = new Quaternion[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(30.0f), 0.0f),
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

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslation);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslationSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.TRANSLATION, properties);

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_INT,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputTranslationSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {

                    properties.Add(new Property(PropertyName.SparseAccessor, "Positions"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {

                    properties.Add(new Property(PropertyName.SparseAccessor, "Mesh Primitive Indices"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
