using System;
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

            Model CreateModel(Action<List<Property>, List<Runtime.MeshPrimitive>, List<Runtime.Node>, List<Runtime.Animation>> setProperties)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();
                var animations = new List<Runtime.Animation>();
                var animated = true;

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material()
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                    {
                        BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImagePlane },
                    },
                };

                var nodes = new List<Runtime.Node>();
                var meshPrimitives = new List<Runtime.MeshPrimitive>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitives, nodes, animations);

                // If no animations are used, null out that property.
                if (!animations.Any())
                {
                    animations = null;
                    animated = false;
                }

                // Create the gltf object.
                Runtime.GLTF gltf = CreateGLTF(() => new Runtime.Scene()
                {
                    Nodes = nodes
                }, animations: animations);

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                    Animated = animated
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    meshPrimitives.AddRange(MeshPrimitive.CreateMultiPrimitivePlane());
                    var samplerAttributes = new List<glTFLoader.Schema.Sampler.WrapTEnum>()
                    {
                        WrapTEnum.CLAMP_TO_EDGE,
                        WrapTEnum.MIRRORED_REPEAT
                    };
                    for (int i = 0; i < meshPrimitives.Count; i++)
                    { 
                        meshPrimitives[i].Material = new Runtime.Material
                        {
                            MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                            {
                                BaseColorTexture = new Runtime.Texture
                                {
                                    Source = baseColorTextureImagePlane,
                                    Sampler = new Runtime.Sampler()
                                    {
                                        WrapT = samplerAttributes[i]
                                    }
                                }
                            }
                        };
                    }

                    nodes.Add(
                        new Runtime.Node
                        {
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = meshPrimitives
                            }
                        }
                    );

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same image."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    meshPrimitives.AddRange(MeshPrimitive.CreateMultiPrimitivePlane());
                    var texture = new Runtime.Texture { Source = baseColorTextureImagePlane };

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = new Runtime.Material
                        {
                           MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                            {
                               BaseColorTexture = texture
                            }
                        };
                    }

                    nodes.Add(
                        new Runtime.Node
                        {
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = meshPrimitives
                            }
                        }
                    );

                   properties.Add(new Property(PropertyName.Description, "Two materials using the same texture."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    meshPrimitives.AddRange(MeshPrimitive.CreateMultiPrimitivePlane());
                    var material = new Runtime.Material
                    {
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                        {
                            BaseColorTexture = new Runtime.Texture
                            {
                                Source = baseColorTextureImagePlane,
                            }
                        }
                    };

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = material;
                    }

                    nodes.Add(
                        new Runtime.Node
                        {
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = meshPrimitives
                            }
                        }
                    );

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same material."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    meshPrimitives.Add(MeshPrimitive.CreateSinglePlane(includeTextureCoords: false));
                    meshPrimitives.Add(MeshPrimitive.CreateSinglePlane(includeTextureCoords: false));
                    meshPrimitives[0].TextureCoordSets = meshPrimitives[1].TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                    meshPrimitives[0].Normals = meshPrimitives[1].Normals = MeshPrimitive.GetSinglePlaneNormals();

                   foreach (var meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = new Runtime.Material
                        {
                            MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                            {
                               BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImagePlane }
                            }
                        };
                    }
                    nodes.Add(
                       new Runtime.Node
                       {
                           Mesh = new Runtime.Mesh
                           {
                               MeshPrimitives = meshPrimitives
                           }
                        }
                    );

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the attributes `NORMAL` and `TEXTCOORD`."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    meshPrimitives.Add(MeshPrimitive.CreateSinglePlane(includeIndices: false));
                    meshPrimitives.Add(MeshPrimitive.CreateSinglePlane(includeIndices: false));
                    meshPrimitives[0].Indices = meshPrimitives[1].Indices = MeshPrimitive.GetSinglePlaneIndices();

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                       meshPrimitive.Material = new Runtime.Material
                        {
                           MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                            {
                               BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImagePlane }
                            }
                        };
                    }
                    nodes.Add(
                       new Runtime.Node
                       {
                           Mesh = new Runtime.Mesh
                           {
                               MeshPrimitives = meshPrimitives
                           }
                        }
                    );

                    properties.Add(new Property(PropertyName.Description, "Two primitives indices using the same accessors."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    meshPrimitives.Add(MeshPrimitive.CreateSinglePlane());
                    var mesh = new Runtime.Mesh()
                    {
                        MeshPrimitives = meshPrimitives
                    };

                    foreach (var meshPrimitive in meshPrimitives)
                    {
                       meshPrimitive.Material = new Runtime.Material
                        {
                           MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                            {
                               BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImagePlane }
                            }
                        };
                    }

                    nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.5f, 0.0f, 0.0f),
                            Mesh = mesh
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.5f, 0.0f, 0.0f),
                            Mesh = mesh
                        }
                    });

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same mesh."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);

                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Translation = new Vector3(0.5f, 0.0f, 0.0f);
                    nodes[2].Skin = nodes[0].Skin;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same skin."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    nodes.AddRange(Nodes.CreatePlaneWithSkinB());

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same skeleton."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    nodes.AddRange(Nodes.CreatePlaneWithSkinB());
                    foreach (var node in nodes)
                    {
                        if (node.Skin != null)
                        {
                            node.Skin.InverseBindMatrixInstanced = true;
                        }
                    }

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same inverseBindMatrices."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    var meshPrimitive = MeshPrimitive.CreateCube();
                    meshPrimitive.Material = new Runtime.Material()
                    {
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                        {
                            BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImageCube },
                        },
                    };
                    nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.2f, 0.0f, 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh()
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>()
                                {
                                    meshPrimitive
                                }
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.2f, 0.0f, 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh()
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>()
                                {
                                    meshPrimitive
                                }
                            }
                        }
                    });

                    var quarterTurn = (FloatMath.Pi / 2.0f);
                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                        new[]
                        {
                            0.0f,
                            1.0f,
                            2.0f,
                            3.0f,
                            4.0f,
                        },
                        new[]
                        {
                            Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                            Quaternion.Identity,
                            Quaternion.CreateFromYawPitchRoll(0.0f, -quarterTurn, 0.0f),
                            Quaternion.Identity,
                            Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                        });
                    animations.Add( new Runtime.Animation
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
                                Sampler = sampler
                            },
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = nodes[1],
                                    Path = ROTATION,
                                },
                                Sampler = sampler
                            },
                        }
                    });

                    properties.Add(new Property(PropertyName.Description, "Two animation channels using the same samplers."));
                }),
                CreateModel((properties, meshPrimitives, nodes, animations) => {
                    var meshPrimitive = MeshPrimitive.CreateCube();
                    meshPrimitive.Material = new Runtime.Material()
                    {
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                        {
                            BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImageCube },
                        },
                    };
                    nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.2f, 0.0f, 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh()
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>()
                                {
                                    meshPrimitive
                                }
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.2f, 0.0f, 0.0f),
                            Scale = new Vector3(0.5f, 0.5f, 0.5f),
                            Mesh = new Runtime.Mesh()
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>()
                                {
                                    meshPrimitive
                                }
                            }
                        }
                    });

                    var quarterTurn = (FloatMath.Pi / 2.0f);
                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                        new[]
                        {
                            0.0f,
                            1.0f,
                            2.0f,
                            3.0f,
                            4.0f,
                        },
                        new[]
                        {
                            Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                            Quaternion.Identity,
                            Quaternion.CreateFromYawPitchRoll(0.0f, -quarterTurn, 0.0f),
                            Quaternion.Identity,
                            Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                        });
                    animations.Add( new Runtime.Animation
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
                                Sampler = sampler,
                                SamplerInstanced = false
                            },
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = nodes[1],
                                    Path = ROTATION,
                                },
                                Sampler = sampler,
                                SamplerInstanced = false
                            },
                        }
                    });

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same accessors."));
                }),
                // CreateModel((properties, meshPrimitives, nodes, animations) => {
                //     properties.Add(new Property(PropertyName.Description, "Two buffer views using the same buffers."));
                // }),
                // Morph NYI
                // CreateModel((properties, meshPrimitives, node, animations) => {
                //     properties.Add(new Property(PropertyName.Description, "Two morph target attributes using the same accessors."));
                // }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
