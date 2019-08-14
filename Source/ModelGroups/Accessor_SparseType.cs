using AssetGenerator.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static AssetGenerator.Runtime.Accessor;
using static AssetGenerator.Runtime.AnimationChannelTarget;
using static AssetGenerator.Runtime.AnimationSampler;

namespace AssetGenerator
{
    internal class Accessor_SparseType : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Accessor_SparseType;

        public Accessor_SparseType(List<string> imageList)
        {
            Image baseColorTextureImageA = UseTexture(imageList, "BaseColor_A");
            Image baseColorTextureImageB = UseTexture(imageList, "BaseColor_B");
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
                var sparseDictionary = new Dictionary<IEnumerable, Runtime.AccessorSparse>();

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

            var SamplerOutputRotation = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians( 45.0f), 0.0f),
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-45.0f), 0.0f),
            };

            var SamplerOutputRotationSparse = new Quaternion[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
            };

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

            void SetAnimationPaths(List<AnimationChannel> channels, PathEnum path, List<Property> properties)
            {
                foreach (var channel in channels)
                {
                    channel.Target.Path = path;
                }
            }

            void OffsetPositions(List<Node> nodes)
            {
                // Offsets the positions of the mesh primitives so they don't overlap. This is done because animation translations override node translations.
                nodes[0].Mesh.MeshPrimitives.First().Positions.Values = ((Vector3[])nodes[0].Mesh.MeshPrimitives.First().Positions.Values).Select(position => { return new Vector3(position.X - 0.6f, position.Y, position.Z); });
                nodes[1].Mesh.MeshPrimitives.First().Positions.Values = ((Vector3[])nodes[1].Mesh.MeshPrimitives.First().Positions.Values).Select(position => { return new Vector3(position.X + 0.6f, position.Y, position.Z); });
            }

            void SetTexture(List<Node> nodes)
            {
                var textureA = new Texture { Source = baseColorTextureImageA };
                var textureB = new Texture { Source = baseColorTextureImageB };

                nodes[0].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new Texture { Source = baseColorTextureImageA }
                    }
                };
                nodes[1].Mesh.MeshPrimitives.First().Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new Texture { Source = baseColorTextureImageB }
                    }
                };

                nodes[0].Mesh.MeshPrimitives.First().TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
                nodes[1].Mesh.MeshPrimitives.First().TextureCoordSets = new Accessor(MeshPrimitive.GetSinglePlaneTextureCoordSets());
            }

            Models = new List<Model>
            {
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation);
                    var sampler1 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation);

                    sampler1.InputKeys.Sparse = new AccessorSparse
                    (
                        new[] { 1 },
                        ComponentTypeEnum.UNSIGNED_INT,
                        ComponentTypeEnum.FLOAT,
                        SamplerInputSparse
                    );

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.InputKeys.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.InputKeys.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation);
                    var sampler1 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation);

                    sampler1.InputKeys.Sparse = new AccessorSparse
                    (
                        new[] { 1 },
                        ComponentTypeEnum.UNSIGNED_BYTE,
                        ComponentTypeEnum.FLOAT,
                        SamplerInputSparse
                    );

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.InputKeys.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.InputKeys.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation);
                    var sampler1 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation);

                    sampler1.InputKeys.Sparse = new AccessorSparse
                    (
                        new[] { 1 },
                        ComponentTypeEnum.UNSIGNED_SHORT,
                        ComponentTypeEnum.FLOAT,
                        SamplerInputSparse
                    );

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Input"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.InputKeys.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.InputKeys.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 1"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation, ComponentTypeEnum.BYTE, TypeEnum.VEC4, InterpolationEnum.LINEAR);
                    var sampler1 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation, ComponentTypeEnum.BYTE, TypeEnum.VEC4, InterpolationEnum.LINEAR);

                    sampler1.OutputKeys.Sparse = new AccessorSparse
                    (
                        new[] { 1 },
                        ComponentTypeEnum.UNSIGNED_INT,
                        ComponentTypeEnum.BYTE,
                        SamplerOutputRotationSparse
                    );

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.OutputKeys.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.OutputKeys.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 2"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    OffsetPositions(nodes);

                    var sampler0 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation, ComponentTypeEnum.SHORT, TypeEnum.VEC4, InterpolationEnum.LINEAR);
                    var sampler1 = new AnimationSampler(SamplerInputLinear, SamplerOutputRotation, ComponentTypeEnum.SHORT, TypeEnum.VEC4, InterpolationEnum.LINEAR);

                    sampler1.OutputKeys.Sparse = new AccessorSparse
                    (
                        new[] { 1 },
                        ComponentTypeEnum.UNSIGNED_INT,
                        ComponentTypeEnum.SHORT,
                        SamplerOutputRotationSparse
                    );

                    var channels = CreateChannels(nodes, sampler0, sampler1);
                    animation.Channels = channels;
                    SetAnimationPaths(channels, PathEnum.ROTATION, properties);

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler1.OutputKeys.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler1.OutputKeys.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See Figure 2"));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    // Add extra vertexes that will be used by the sparse accessor.
                    SetTexture(nodes);
                    var positions = MeshPrimitive.GetSinglePlanePositions().ToList();
                    positions.Add(new Vector3( 0.25f, -0.5f, 0.0f));
                    positions.Add(new Vector3(-0.25f,  0.5f, 0.0f));
                    var textureCoords = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    textureCoords[0] = textureCoords[0].Concat
                    (
                        new[]
                        {
                            textureCoords[0][0],
                            textureCoords[0][2]
                        }
                    ).ToArray();
                    foreach(var node in nodes)
                    {
                        node.Mesh.MeshPrimitives.First().Positions.Values = positions.ToArray();
                        node.Mesh.MeshPrimitives.First().TextureCoordSets.Values = textureCoords;
                    }
                    OffsetPositions(nodes);

                    nodes[1].Mesh.MeshPrimitives.First().Indices.Values = nodes[0].Mesh.MeshPrimitives.First().Indices.Values;

                    nodes[1].Mesh.MeshPrimitives.First().Indices.Sparse = new AccessorSparse
                    (
                        new[] { 1, 5 },
                        ComponentTypeEnum.UNSIGNED_BYTE,
                        ComponentTypeEnum.UNSIGNED_INT,
                        new List<int> { 4, 5 }
                    );

                    properties.Add(new Property(PropertyName.SparseAccessor, "Mesh Primitive Indices"));
                    properties.Add(new Property(PropertyName.IndicesType, nodes[1].Mesh.MeshPrimitives.First().Indices.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, nodes[1].Mesh.MeshPrimitives.First().Indices.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ":white_check_mark:"));
                    properties.Add(new Property(PropertyName.Description, "See the description for the Mesh Primitive Indices model in [Accessor_Sparse](../Accessor_Sparse/README.md)."));
                }),
                CreateModel((properties, animation, nodes) =>
                {
                    SetTexture(nodes);
                    nodes.RemoveAt(0);
                    var sampler = new AnimationSampler(SamplerInputLinear, null, ComponentTypeEnum.FLOAT, TypeEnum.VEC4, InterpolationEnum.LINEAR);

                    sampler.OutputKeys.Sparse = new AccessorSparse
                    (
                        new[] { 1 },
                        ComponentTypeEnum.UNSIGNED_BYTE,
                        ComponentTypeEnum.FLOAT,
                        SamplerOutputTranslationSparse,
                        SamplerOutputTranslation.Count(),
                        "Sparse Animation Sampler Output"
                    );

                    var channels = new List<AnimationChannel>
                    {
                        new AnimationChannel
                        {
                            Target = new AnimationChannelTarget
                            {
                                Node = nodes[0],
                                Path = PathEnum.TRANSLATION
                            },
                            Sampler = sampler
                        },
                    };
                    animation.Channels = channels;

                    properties.Add(new Property(PropertyName.SparseAccessor, "Output"));
                    properties.Add(new Property(PropertyName.IndicesType, sampler.OutputKeys.Sparse.IndicesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.ValueType, sampler.OutputKeys.Sparse.ValuesComponentType.ToReadmeString()));
                    properties.Add(new Property(PropertyName.BufferView, ""));
                    properties.Add(new Property(PropertyName.Description, "See Figure 3"));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
