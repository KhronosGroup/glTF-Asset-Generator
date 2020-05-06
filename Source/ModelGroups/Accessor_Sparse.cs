using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Accessor_Sparse : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Accessor_Sparse;

        public Accessor_Sparse(List<string> imageList)
        {
            var baseColorTextureA = new Texture { Source = UseTexture(imageList, "BaseColor_A") };
            var baseColorTextureB = new Texture { Source = UseTexture(imageList, "BaseColor_B") };
            UseFigure(imageList, "SparseAccessor_Input");
            UseFigure(imageList, "SparseAccessor_Output-Translation");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Animation, List<Node>> setProperties)
            {
                var properties = new List<Property>();
                var animated = true;
                var meshPrimitive0 = MeshPrimitive.CreateSinglePlane();
                var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();

                var nodes = new List<Node>
                {
                    new Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new List<Runtime.MeshPrimitive>
                            {
                                meshPrimitive0
                            }
                        }
                    },
                    new Node
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
                var animation = new Animation();
                var animations = new List<Animation>
                {
                    animation
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, animation, nodes);

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
                        () => new Scene
                        {
                            Nodes = nodes
                        },
                        animations: animations
                    ),
                    Animated = animated,
                    Camera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.75f))
                };
            }

            var samplerInputLinear = new[]
            {
                0.0f,
                1.0f,
                2.0f,
            };

            var samplerOutputTranslation = new[]
            {
                new Vector3(0.0f,  0.3f, 0.0f),
                new Vector3(0.0f, -0.3f, 0.0f),
                new Vector3(0.0f,  0.3f, 0.0f),
            };

            void SetTexture(List<Node> nodes)
            {
                nodes[0].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTextureA },
                    }
                };
                nodes[1].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTextureB }
                    }
                };
            }

            void OffsetPositions(List<Node> nodes)
            {
                // Offsets the positions of the mesh primitives so they don't overlap. This is done because animation translations override node translations.
                nodes[0].Mesh.MeshPrimitives.First().Positions.Values = ((Vector3[])nodes[0].Mesh.MeshPrimitives.First().Positions.Values).Select(position => { return new Vector3(position.X - 0.6f, position.Y, position.Z); });
                nodes[1].Mesh.MeshPrimitives.First().Positions.Values = ((Vector3[])nodes[1].Mesh.MeshPrimitives.First().Positions.Values).Select(position => { return new Vector3(position.X + 0.6f, position.Y, position.Z); });
            }

            void OffsetNodeTranslations(List<Node> nodes)
            {
                // Gives each node a translation so they don't overlap, but can have the same values for position.
                nodes[0].Translation = new Vector3(-0.6f, 0.0f, 0.0f);
                nodes[1].Translation = new Vector3(0.6f, 0.0f, 0.0f);
            }

            List<AnimationChannel> CreateChannels(List<Node> nodes, AnimationSampler sampler0, AnimationSampler sampler1)
            {
                return new List<AnimationChannel>
                {
                    new AnimationChannel
                    {
                        Target = new AnimationChannelTarget
                        {
                            Node = nodes[0],
                        },
                        Sampler = sampler0
                    },
                    new AnimationChannel
                    {
                        Target = new AnimationChannelTarget
                        {
                            Node = nodes[1],
                        },
                        Sampler = sampler1
                    },
                };
            }

            void SetAnimationPaths(List<AnimationChannel> channels, AnimationChannelTargetPath path, List<Property> properties)
            {
                foreach (var channel in channels)
                {
                    channel.Target.Path = path;
                }
            }

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes) =>
                {
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputTranslation),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear, DataSparse.Create
                        (
                            new Dictionary<int, float>
                            {
                                { 1, 1.5f }
                            }
                        )),
                        Output = Data.Create(samplerOutputTranslation),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Translation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Input"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputTranslation),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputTranslation, DataSparse.Create
                        (
                            new Dictionary<int, Vector3>
                            {
                                { 1, new Vector3(0.0f, 0.2f, 0.0f) },
                            }
                        )),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Translation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 2"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    OffsetNodeTranslations(nodes);

                    nodes[1].Mesh.MeshPrimitives.First().Positions.Values = nodes[0].Mesh.MeshPrimitives.First().Positions.Values;

                    nodes[1].Mesh.MeshPrimitives.First().Positions.Sparse = DataSparse.Create
                    (
                        new Dictionary<int, Vector3>
                        {
                            { 0, new Vector3( 0.25f, -0.5f, 0.0f) },
                            { 2, new Vector3(-0.25f,  0.5f, 0.0f) },
                        }
                    );

                    properties.Add(new Property(PropertyName.SparseAccessor, "Positions"));
                    properties.Add(new Property(PropertyName.Description, "Mesh B's sparse accessor overwrites the values of the top left and bottom right vertexes."));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    // Add extra vertexes that will be used by the sparse accessor.
                    var positions = MeshPrimitive.GetSinglePlanePositions().Concat(new[]
                    {
                        new Vector3( 0.25f, -0.5f, 0.0f),
                        new Vector3(-0.25f,  0.5f, 0.0f),
                    });
                    var textureCoords = MeshPrimitive.GetSinglePlaneTexCoords().Concat(new[]
                    {
                        new Vector2(1.0f, 1.0f),
                        new Vector2(0.0f, 0.0f),
                    });
                    foreach (var node in nodes)
                    {
                        node.Mesh.MeshPrimitives.First().Positions.Values = positions;
                        node.Mesh.MeshPrimitives.First().TexCoords0.Values = textureCoords;
                    }
                    OffsetNodeTranslations(nodes);

                    nodes[1].Mesh.MeshPrimitives.First().Indices.Values = nodes[0].Mesh.MeshPrimitives.First().Indices.Values;

                    nodes[1].Mesh.MeshPrimitives.First().Indices.Sparse = DataSparse.Create
                    (
                        new Dictionary<int, int>
                        {
                            { 1, 4 },
                            { 5, 5 },
                        }
                    );

                    properties.Add(new Property(PropertyName.SparseAccessor, "Mesh Primitive Indices"));
                    properties.Add(new Property(PropertyName.Description, "Both meshes have six vertexes, but only four are used to make the visible mesh. " +
                        "Mesh B's sparse accessor replaces the indices pointing at the top left and bottom right vertexes with ones pointing at the unused vertexes."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
