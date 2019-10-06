using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Accessor_SparseType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Accessor_SparseType;

        public Accessor_SparseType(List<string> imageList)
        {
            var baseColorTextureA = new Texture { Source = UseTexture(imageList, "BaseColor_A") };
            var baseColorTextureB = new Texture { Source = UseTexture(imageList, "BaseColor_B") };
            UseFigure(imageList, "SparseAccessor_Input");
            UseFigure(imageList, "SparseAccessor_Output-Rotation");
            UseFigure(imageList, "SparseAccessor_NoBufferView");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Animation, List<Node>> setProperties)
            {
                var properties = new List<Property>();
                var animated = true;
                var meshPrimitive0 = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);
                var meshPrimitive1 = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);

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

            var samplerInputSparse = 1.5f;

            var samplerOutputTranslationSparse = new Vector3(0.0f, 0.2f, 0.0f);

            var samplerOutputRotation = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians( 45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-45.0f), 0.0f),
            };

            var samplerOutputRotationSparse = Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f);

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

            void OffsetPositions(List<Node> nodes)
            {
                // Offsets the positions of the mesh primitives so they don't overlap. This is done because animation translations override node translations.
                nodes[0].Mesh.MeshPrimitives.First().Positions.Values = nodes[0].Mesh.MeshPrimitives.First().Positions.Values.Select(position => { return new Vector3(position.X - 0.6f, position.Y, position.Z); });
                nodes[1].Mesh.MeshPrimitives.First().Positions.Values = nodes[1].Mesh.MeshPrimitives.First().Positions.Values.Select(position => { return new Vector3(position.X + 0.6f, position.Y, position.Z); });
            }

            void SetTexture(List<Node> nodes)
            {
                nodes[0].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTextureA }
                    }
                };
                nodes[1].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTextureB }
                    }
                };

                nodes[0].Mesh.MeshPrimitives.First().TexCoords0 = Data.Create(MeshPrimitive.GetSinglePlaneTexCoords());
                nodes[1].Mesh.MeshPrimitives.First().TexCoords0 = Data.Create(MeshPrimitive.GetSinglePlaneTexCoords());
            }

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear, DataSparse.Create
                        (
                            DataType.UnsignedByte,
                            new Dictionary<int, float>
                            {
                                { 1, samplerInputSparse }
                            }
                        )),
                        Output = Data.Create(samplerOutputRotation),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Rotation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.Input.Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.Input.OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear, DataSparse.Create
                        (
                            DataType.UnsignedShort,
                            new Dictionary<int, float>
                            {
                                { 1, samplerInputSparse }
                            }
                        )),
                        Output = Data.Create(samplerOutputRotation),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Rotation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.Input.Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.Input.OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear, DataSparse.Create
                        (
                            DataType.UnsignedInt,
                            new Dictionary<int, float>
                            {
                                { 1, samplerInputSparse }
                            }
                        )),
                        Output = Data.Create(samplerOutputRotation),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Rotation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.Input.Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.Input.OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation, DataType.NormalizedByte),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation, DataType.NormalizedByte, DataSparse.Create
                        (
                            DataType.UnsignedByte,
                            new Dictionary<int, Quaternion>
                            {
                                { 1, samplerOutputRotationSparse }
                            }
                        )),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Rotation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, ((Data<Quaternion>)sampler1.Output).Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.Output.OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 2"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation, DataType.NormalizedShort),
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(samplerOutputRotation, DataType.NormalizedShort, DataSparse.Create
                        (
                            DataType.UnsignedByte,
                            new Dictionary<int, Quaternion>
                            {
                                { 1, samplerOutputRotationSparse }
                            }
                        )),
                    };

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, AnimationChannelTargetPath.Rotation, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, ((Data<Quaternion>)sampler1.Output).Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.Output.OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 2"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    // Add extra vertexes that will be used by the sparse accessor.
                    SetTexture(nodes);
                    var positions = MeshPrimitive.GetSinglePlanePositions().Concat(new[]
                    {
                        new Vector3( 0.25f, -0.5f, 0.0f),
                        new Vector3(-0.25f,  0.5f, 0.0f),
                    });
                    var texCoords = MeshPrimitive.GetSinglePlaneTexCoords().Concat(new[]
                    {
                        new Vector2(1.0f, 1.0f),
                        new Vector2(0.0f, 0.0f),
                    });

                    foreach (var node in nodes)
                    {
                        node.Mesh.MeshPrimitives.First().Positions.Values = positions;
                        node.Mesh.MeshPrimitives.First().TexCoords0.Values = texCoords;
                    }
                    OffsetPositions(nodes);

                    var meshPrimitiveIndices = nodes[1].Mesh.MeshPrimitives.First().Indices;
                    meshPrimitiveIndices.Values = nodes[0].Mesh.MeshPrimitives.First().Indices.Values;
                    meshPrimitiveIndices.Sparse = DataSparse.Create
                    (
                        DataType.UnsignedByte,
                        new Dictionary<int, int>
                        {
                            { 1, 4 },
                            { 5, 5 },
                        }
                    );

                    properties.Add(new Property(PropertyName.SparseAccessor, "Mesh Primitive Indices"));
                    properties.Add(new Property(PropertyName.IndicesType, meshPrimitiveIndices.Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, meshPrimitiveIndices.OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See the description for the Mesh Primitive Indices model in [Accessor_Sparse](../Accessor_Sparse/README.md)."));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    nodes.RemoveAt(0);

                    var sampler = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = Data.Create(samplerInputLinear),
                        Output = Data.Create(Enumerable.Repeat(default(Vector3), 3), DataSparse.Create
                        (
                            DataType.UnsignedByte,
                            new Dictionary<int, Vector3>
                            {
                                { 1, samplerOutputTranslationSparse }
                            }
                        )),
                    };

                    var channels = new List<AnimationChannel>
                    {
                        new AnimationChannel
                        {
                            Target = new AnimationChannelTarget
                            {
                                Node = nodes[0],
                                Path = AnimationChannelTargetPath.Translation
                            },
                            Sampler = sampler
                        },
                    };
                    animation.Channels = channels;

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, ((Data<Vector3>)sampler.Output).Sparse.IndicesOutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, ((Data<Vector3>)sampler.Output).OutputType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ""));
                    properties.Add(new Property(PropertyName.Description, "See Figure 3"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
