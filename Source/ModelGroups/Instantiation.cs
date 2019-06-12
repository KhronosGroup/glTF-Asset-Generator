﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using static glTFLoader.Schema.Sampler;
using static AssetGenerator.Runtime.AnimationChannelTarget.PathEnum;

namespace AssetGenerator
{
    internal class Instantiation : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Instantiation;

        public Instantiation(List<string> imageList)
        {
            Runtime.Image baseColorTextureImagePlane = UseTexture(imageList, "BaseColor_Plane");
            Runtime.Image baseColorTextureImageCube = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Node>, List<Runtime.Animation>> setProperties)
            {
                var properties = new List<Property>();
                var animations = new List<Runtime.Animation>();
                var animated = true;
                var nodes = new List<Runtime.Node>();

                // There are no common properties in this model group that are reported in the readme.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes, animations);

                // If no animations are used, null out that property.
                if (!animations.Any())
                {
                    animations = null;
                    animated = false;
                }

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene() { Nodes = nodes }, animations: animations),
                    Animated = animated
                };
            }

            List<Vector4> CreateVertexColors(IEnumerable<Vector3> positions, bool useAlternateColor = false)
            {
                var colors = new List<Vector4>();
                var color = useAlternateColor ? new Vector4(0.0f, 1.0f, 0.0f, 1.0f) : new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
                foreach (var vertex in positions)
                {
                    colors.Add(color);
                }
                return colors;
            }

            Runtime.Material CreateMaterial(bool useCubeTexture = false)
            {
                return new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = useCubeTexture ? baseColorTextureImageCube : baseColorTextureImagePlane }
                    }
                };
            }

            void AddMeshPrimitivesToSingleNode(List<Runtime.Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives)
            {
                nodes.Add(
                    new Runtime.Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = meshPrimitives
                        }
                    }
                );
            }

            float[] GetAnimationSamplerInputKeys()
            {
                return new[]
                {
                    0.0f,
                    1.0f,
                    2.0f,
                    3.0f,
                    4.0f,
                };
            }

            Quaternion[] GetAnimationSamplerOutputKeys()
            {
                var quarterTurn = (FloatMath.Pi / 2.0f);
                return new[]
                {
                    Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, -quarterTurn, 0.0f),
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                };
            }

            void AddMeshPrimitivesToMultipleNodes(List<Runtime.Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives)
            {
                nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.27f, 0.0f, 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh()
                            {
                                MeshPrimitives = meshPrimitives
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.27f, 0.0f, 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh()
                            {
                                MeshPrimitives = meshPrimitives
                            }
                        }
                    }
                );
            }

            void BuildAnimation(List<Runtime.Animation> animations, List<Runtime.Node> nodes, Runtime.AnimationSampler sampler0, Runtime.AnimationSampler sampler1, bool samplerInstanced)
            {
                animations.Add(new Runtime.Animation
                    {
                        Channels = new List<Runtime.AnimationChannel>
                        {
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = nodes[0],
                                    Path = ROTATION,
                                },
                                Sampler = sampler0,
                                SamplerInstanced = samplerInstanced
                            },
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = nodes[1],
                                    Path = ROTATION,
                                },
                                Sampler = sampler1,
                                SamplerInstanced = samplerInstanced
                            },
                        }
                    }
                );
            }

            Models = new List<Model>
            {
                CreateModel((properties, nodes, animations) => {
                    List<Runtime.MeshPrimitive> meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane();
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateMaterial();
                    }
                    meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler() { WrapT = WrapTEnum.CLAMP_TO_EDGE };
                    meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler() { WrapT = WrapTEnum.MIRRORED_REPEAT };

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same image."));
                }),
                CreateModel((properties, nodes, animations) => {
                    List<Runtime.MeshPrimitive> meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane();
                    var texture = new Runtime.Texture { Source = baseColorTextureImagePlane };

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateMaterial();
                    }

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                   properties.Add(new Property(PropertyName.Description, "Two materials using the same texture."));
                }),
                CreateModel((properties, nodes, animations) => {
                    List<Runtime.MeshPrimitive> meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane();
                    var material = CreateMaterial();

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = material;
                    }

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same material."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    meshPrimitives[0].TextureCoordSets = meshPrimitives[1].TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    meshPrimitives[0].Normals = meshPrimitives[1].Normals = MeshPrimitive.GetSinglePlaneNormals();

                   foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateMaterial();
                    }
                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the attributes `NORMAL` and `TEXTCOORD`."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane()
                    };
                    meshPrimitives[0].Indices = meshPrimitives[1].Indices = MeshPrimitive.GetSinglePlaneIndices();

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                       meshPrimitive.Material = CreateMaterial();
                    }
                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two primitives indices using the same accessors."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateSinglePlane() };
                    meshPrimitives[0].Colors = CreateVertexColors(meshPrimitives[0].Positions);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives);
                    nodes[1].Mesh = nodes[0].Mesh;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same mesh."));
                }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions);

                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    var positions = (List<Vector3>)nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions;
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(positions, true);

                    // Offsets the position of the second node so they don't overlap.
                    for (int i = 0; i < positions.Count(); i++)
                    {
                        positions[i] = new Vector3(positions[i].X + 0.6f, positions[i].Y, positions[i].Z);
                    }

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same skin."));
                }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreatePlaneWithSkinB());

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same skeleton."));
                }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreatePlaneWithSkinB());
                    foreach (Runtime.Node node in nodes)
                    {
                        if (node.Skin != null)
                        {
                            node.Skin.InverseBindMatrixInstanced = true;
                        }
                    }

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same inverseBindMatrices."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateCube() };
                    meshPrimitives[0].Material = CreateMaterial(useCubeTexture: true);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives);

                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(GetAnimationSamplerInputKeys(), GetAnimationSamplerOutputKeys());
                    BuildAnimation(animations, nodes, sampler, sampler, true);

                    properties.Add(new Property(PropertyName.Description, "Two animation channels using the same samplers."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateCube() };
                    meshPrimitives[0].Material = CreateMaterial(useCubeTexture: true);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives);

                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(GetAnimationSamplerInputKeys(), GetAnimationSamplerOutputKeys());
                    BuildAnimation(animations, nodes, sampler, sampler, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same accessors."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    meshPrimitives[0].TextureCoordSets = meshPrimitives[1].TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    meshPrimitives[0].Normals = meshPrimitives[1].Normals = MeshPrimitive.GetSinglePlaneNormals();

                   foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.BufferViewsInstanced = true;
                        meshPrimitive.Material = CreateMaterial();
                    }
                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two accessors using the same buffer view."));
                }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateCube() };
                    meshPrimitives[0].Material = CreateMaterial(useCubeTexture: true);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives);

                    var output = GetAnimationSamplerOutputKeys();
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(GetAnimationSamplerInputKeys(), output);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(
                        new[]
                        {
                            0.0f,
                            2.0f,
                            4.0f,
                            6.0f,
                            8.0f,
                        },
                        output
                    );
                    BuildAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers sharing the same output accessors."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
