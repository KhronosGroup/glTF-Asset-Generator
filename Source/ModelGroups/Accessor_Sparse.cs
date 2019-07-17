using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static AssetGenerator.Runtime.AccessorSparse;
using static AssetGenerator.Runtime.AnimationChannelTarget;

namespace AssetGenerator
{
    internal class Accessor_Sparse : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Accessor_Sparse;

        public Accessor_Sparse(List<string> imageList)
        {
            Runtime.Image baseColorTextureImageA = UseTexture(imageList, "BaseColor_A");
            Runtime.Image baseColorTextureImageB = UseTexture(imageList, "BaseColor_B");
            UseFigure(imageList, "SparseAccessor_Input");
            UseFigure(imageList, "SparseAccessor_Output-Translation");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.Animation, List<Runtime.Node>, Dictionary<IEnumerable, Runtime.AccessorSparse>> setProperties)
            {
                var properties = new List<Property>();
                var animated = true;
                var meshPrimitive0 = MeshPrimitive.CreateSinglePlane();
                var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();

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
                SetTexture(nodes);
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
                    Camera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.75f))
                };
            }

            var ColorWhite = new Vector4[]
            {
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            };

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
                new Vector3(0.0f,  0.3f, 0.0f),
                new Vector3(0.0f, -0.3f, 0.0f),
                new Vector3(0.0f,  0.3f, 0.0f),
            };

            var SamplerOutputTranslationSparse = new[]
            {
                new Vector3(0.0f, 0.2f, 0.0f),
            };

            void SetTexture(List<Runtime.Node> nodes)
            {
                var textureA = new Runtime.Texture { Source = baseColorTextureImageA };
                var textureB = new Runtime.Texture { Source = baseColorTextureImageB };

                nodes[0].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImageA }
                    }
                };
                nodes[1].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImageB }
                    }
                };
            }

            void OffsetPositions(List<Runtime.Node> nodes)
            {
                // Offsets the positions of the mesh primitives so they don't overlap. This is done because animation translations override node translations.
                nodes[0].Mesh.MeshPrimitives.First().Positions = nodes[0].Mesh.MeshPrimitives.First().Positions.Select(position => { return new Vector3(position.X - 0.6f, position.Y, position.Z); });
                nodes[1].Mesh.MeshPrimitives.First().Positions = nodes[1].Mesh.MeshPrimitives.First().Positions.Select(position => { return new Vector3(position.X + 0.6f, position.Y, position.Z); });
            }

            void OffsetNodeTranslations(List<Runtime.Node> nodes)
            {
                // Gives each node a translation so they don't overlap, but can have the same values for position.
                nodes[0].Translation = new Vector3(-0.6f, 0.0f, 0.0f);
                nodes[1].Translation = new Vector3(0.6f, 0.0f, 0.0f);
            }

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
                    OffsetPositions(nodes);
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

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    OffsetPositions(nodes);
                    var sampler0 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslation);
                    var sampler1 = new Runtime.LinearAnimationSampler<Vector3>(SamplerInputLinear, SamplerOutputTranslationSparse);
                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.TRANSLATION, properties);

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 1 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[1].Sampler).OutputKeys,
                        ((Runtime.LinearAnimationSampler<Vector3>)channels[0].Sampler).OutputKeys
                    );
                    sparseDictionary.Add(SamplerOutputTranslationSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 2"));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    OffsetNodeTranslations(nodes);

                    var PositionsSparse = new List<Vector3>
                    {
                        new Vector3( 0.25f, -0.5f, 0.0f),
                        new Vector3(-0.25f,  0.5f, 0.0f),
                    };
                    nodes[1].Mesh.MeshPrimitives.First().Positions = PositionsSparse;

                    var sparse = new Runtime.AccessorSparse<Vector3>
                    (
                        new List<int> { 0, 2},
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.FLOAT,
                        nodes[1].Mesh.MeshPrimitives.First().Positions,
                        nodes[0].Mesh.MeshPrimitives.First().Positions
                    );
                    sparseDictionary.Add(PositionsSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Positions"));
                    properties.Add(new Property(PropertyName.Description, "Mesh B's'sparse accessor overwrites the values of the top left and bottom right vertexes."));
                }),
                CreateModel((properties, animation, nodes, sparseDictionary) =>
                {
                    // Add extra vertexes that will be used by the sparse accessor.
                    var positions = MeshPrimitive.GetSinglePlanePositions();
                    positions.Add(new Vector3( 0.25f, -0.5f, 0.0f));
                    positions.Add(new Vector3(-0.25f,  0.5f, 0.0f));
                    var textureCoords = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    textureCoords[0].Add(textureCoords[0][0]);
                    textureCoords[0].Add(textureCoords[0][2]);
                    foreach(var node in nodes)
                    {
                        node.Mesh.MeshPrimitives.First().Positions = positions;
                        node.Mesh.MeshPrimitives.First().TextureCoordSets = textureCoords;
                    }
                    OffsetNodeTranslations(nodes);

                    var indicesSparse = new List<int> { 4, 5 };
                    nodes[1].Mesh.MeshPrimitives.First().Indices = indicesSparse;
                    var sparse = new Runtime.AccessorSparse<int>
                    (
                        new List<int> { 1, 5 },
                        IndicesComponentTypeEnum.UNSIGNED_BYTE,
                        ValuesComponentTypeEnum.UNSIGNED_INT,
                        nodes[1].Mesh.MeshPrimitives.First().Indices,
                        nodes[0].Mesh.MeshPrimitives.First().Indices
                    );
                    sparseDictionary.Add(indicesSparse, sparse);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Mesh Primitive Indices"));
                    properties.Add(new Property(PropertyName.Description, "Both meshes have six vertexes, but only four are used to make the visible mesh. " + 
                        "Mesh B's sparse accessor replaces the indices pointing at the top left and bottom right vertexes with ones pointing at the unused vertexes."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
