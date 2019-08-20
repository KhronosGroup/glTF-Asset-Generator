using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using static glTFLoader.Schema.Sampler;
using static AssetGenerator.Runtime.AnimationChannelTarget.PathEnum;

namespace AssetGenerator
{
    internal class Instancing : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Instancing;

        public Instancing(List<string> imageList)
        {
            Runtime.Image baseColorTextureImageA = UseTexture(imageList, "BaseColor_A");
            Runtime.Image baseColorTextureImageB = UseTexture(imageList, "BaseColor_B");
            Runtime.Image baseColorTextureImageCube = UseTexture(imageList, "BaseColor_Cube");
            var distantCamera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.7f));

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.Node>, List<Runtime.Animation>> setProperties, Action<Model> setCamera)
            {
                var properties = new List<Property>();
                var animations = new List<Runtime.Animation>();
                var animated = true;
                var nodes = new List<Runtime.Node>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes, animations);

                // If no animations are used, null out that property.
                if (!animations.Any())
                {
                    animations = null;
                    animated = false;
                }

                // Create the gltf object.
                var model = new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene { Nodes = nodes }, animations: animations),
                    Animated = animated,
                };

                setCamera(model);

                return model;
            }

            Model CreateModelwithMultipleScenes(Action<List<Property>, List<Runtime.Scene>, List<Runtime.Node>, List<Runtime.Animation>> setProperties, Action<Model> setModel)
            {
                var properties = new List<Property>();
                var animations = new List<Runtime.Animation>();
                var animated = true;
                var nodes = new List<Runtime.Node>();
                var scenes = new List<Runtime.Scene>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, scenes, nodes, animations);

                // If no animations are used, null out that property.
                if (!animations.Any())
                {
                    animations = null;
                    animated = false;
                }

                // Create the gltf object.
                var model = new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => scenes, animations: animations),
                    Animated = animated,
                };

                setModel(model);

                return model;
            }

            var SamplerInputLinear = new[]
            {
                0.0f,
                1.0f,
                2.0f,
                3.0f,
                4.0f,
            };

            var SamplerInputCurve = new[]
            {
                0.0f,
                0.5f,
                1.0f,
                2.0f,
                4.0f,
            };

            var SamplerOutput = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
            };

            var SamplerOutputReverse = new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
            };

            Runtime.Texture CreateTexture(Runtime.Image image)
            {
                return new Runtime.Texture { Source = image };
            }
            
            Runtime.Material CreateMaterial(Runtime.Texture texture)
            {
                return new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = texture
                    }
                };
            }

            void AddMeshPrimitivesToSingleNode(List<Runtime.Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives)
            {
                // If there are multiple mesh primitives, offset their position so they don't overlap.
                if (meshPrimitives.Count > 1)
                {
                    meshPrimitives[0].Positions = meshPrimitives[0].Positions.Select(position => { return new Vector3(position.X - 0.6f, position.Y, position.Z); } );
                    meshPrimitives[1].Positions = meshPrimitives[1].Positions.Select(position => { return new Vector3(position.X + 0.6f, position.Y, position.Z); } );
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
            }

            void AddMeshPrimitivesToMultipleNodes(List<Runtime.Node> nodes, Runtime.MeshPrimitive meshPrimitives0, Runtime.MeshPrimitive meshPrimitives1)
            {
                nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.6f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>
                                {
                                    meshPrimitives0
                                }
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.6f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>
                                {
                                    meshPrimitives1
                                }
                            }
                        }
                    }
                );
            }

            void AddAnimation(List<Runtime.Animation> animations, List<Runtime.Node> nodes, Runtime.AnimationSampler sampler0, Runtime.AnimationSampler sampler1, bool samplerInstanced)
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
                            Sampler = sampler0
                        },
                        new Runtime.AnimationChannel
                        {
                            Target = new Runtime.AnimationChannelTarget
                            {
                                Node = nodes[1],
                                Path = ROTATION,
                            },
                            Sampler = sampler1
                        },
                    }
                });
            }

            void AddMeshPrimitivesToThreeNodes(List<Runtime.Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives)
            {
                nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.0f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>
                                {
                                    meshPrimitives[0]
                                }
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.6f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>
                                {
                                    meshPrimitives[1]
                                }
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.6f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>
                                {
                                    meshPrimitives[2]
                                }
                            }
                        }
                    }
                );
            }

            Models = new List<Model>
            {
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };

                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        // This non-standard set of texture coordinates is larger than the texture but not an exact multiple, so it allows texture sampler settings to be visible.
                        meshPrimitive.TextureCoordSets = new List<List<Vector2>>
                        {
                            new List<Vector2>
                            {
                                new Vector2( 1.3f,  1.3f),
                                new Vector2(-0.3f,  1.3f),
                                new Vector2(-0.3f, -0.3f),
                                new Vector2( 1.3f, -0.3f),
                            }
                        };
                    }

                    meshPrimitives[0].Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    meshPrimitives[1].Material = CreateMaterial(CreateTexture(baseColorTextureImageA));

                    meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler 
                    {
                        WrapT = WrapTEnum.CLAMP_TO_EDGE,
                        WrapS = WrapSEnum.CLAMP_TO_EDGE
                    };
                    meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler
                    {
                        WrapT = WrapTEnum.MIRRORED_REPEAT,
                        WrapS = WrapSEnum.MIRRORED_REPEAT
                    };

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same image as their source."));
                    properties.Add(new Property(PropertyName.Difference, "The texture sampler `WrapT` and `WrapS` are set to `CLAMP_TO_EDGE` for one and `MIRRORED_REPEAT` for the other."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };

                    meshPrimitives[0].Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    meshPrimitives[1].Material = CreateMaterial(CreateTexture(baseColorTextureImageB));

                    var sampler = new Runtime.Sampler
                    {
                        WrapT = WrapTEnum.CLAMP_TO_EDGE,
                        WrapS = WrapSEnum.CLAMP_TO_EDGE
                    };
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = sampler;
                        // This non-standard set of texture coordinates is larger than the texture but not an exact multiple, so it allows texture sampler settings to be visible.
                        meshPrimitive.TextureCoordSets = new List<List<Vector2>>
                        {
                            new List<Vector2>
                            {
                                new Vector2( 1.3f,  1.3f),
                                new Vector2(-0.3f,  1.3f),
                                new Vector2(-0.3f, -0.3f),
                                new Vector2( 1.3f, -0.3f),
                            }
                        };
                    }

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same sampler."));
                    properties.Add(new Property(PropertyName.Difference, "One texture uses image A while the other uses image B."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane()
                    };

                    var texture = CreateTexture(baseColorTextureImageA);
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateMaterial(texture);
                    }
                    meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture = meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture;
                    meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorFactor = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                   properties.Add(new Property(PropertyName.Description, "Two textures using the same source image."));
                   properties.Add(new Property(PropertyName.Difference, "One material does not have a baseColorFactor and the other has a blue baseColorFactor."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    var material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = material;
                    }

                    // One of the primitives has a 'zoomed in' texture coordinate set.
                    meshPrimitives[1].TextureCoordSets = new List<List<Vector2>>
                    {
                        new List<Vector2>
                        {
                            new Vector2(0.9f, 0.9f),
                            new Vector2(0.1f, 0.9f),
                            new Vector2(0.1f, 0.1f),
                            new Vector2(0.9f, 0.1f),
                        }
                    };

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same material."));
                    properties.Add(new Property(PropertyName.Difference, "One primitive has texture coordinates that displays all of texture A, while the other primitive has textures coordinates that don't display the border."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane();
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageB));

                    meshPrimitive0.Positions = meshPrimitive1.Positions;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the `POSITION` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "One primitive uses texture A while the other primitive uses texture B."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane(includeIndices: false);
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageB));

                    meshPrimitive0.Indices = meshPrimitive1.Indices;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for indices."));
                    properties.Add(new Property(PropertyName.Difference, "One primitive uses texture A while the other primitive uses texture B."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive.Material = CreateMaterial(CreateTexture(baseColorTextureImageA));

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive, meshPrimitive);
                    nodes[1].Mesh = nodes[0].Mesh;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same mesh."));
                    properties.Add(new Property(PropertyName.Difference, "The two nodes have different translations."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();

                    // Adds just the node containing the mesh, dropping the data for a second set of joints.
                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImageB));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[2].Skin = nodes[0].Skin;

                    // Offsets the position of both meshes so they don't overlap.
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(position => { return new Vector3(position.X - 0.3f, position.Y, position.Z); } );
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(position => { return new Vector3(position.X + 0.3f, position.Y, position.Z); } );

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same skin."));
                    properties.Add(new Property(PropertyName.Difference, "The two mesh primitives have different `POSITION` values."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) =>
                {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();

                    // Adds just the node containing the mesh, dropping the data for a second set of joints.
                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImageB));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[2].Skin.Joints = nodes[0].Skin.Joints;

                    // Creates new inverseBindMatrices for the second skin, rotating the flap further than the default value would.
                    nodes[2].Skin.InverseBindMatrices = new[]
                    {
                        nodes[2].Skin.InverseBindMatrices.First(),
                        Matrix4x4.Multiply(nodes[2].Skin.InverseBindMatrices.ElementAt(1), Matrix4x4.CreateRotationX(FloatMath.ToRadians(-30))),
                    };

                    // Offsets the position of both meshes so they don't overlap.
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(position => { return new Vector3(position.X - 0.3f, position.Y, position.Z); } );
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(position => { return new Vector3(position.X + 0.3f, position.Y, position.Z); } );

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same joints."));
                    properties.Add(new Property(PropertyName.Difference, "The skin with texture B has inverseBindMatrices that fold twice as far as the skin with texture A."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) =>
                {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[1].Translation = Vector3.Add((Vector3)nodes[1].Translation, new Vector3(-0.3f, 0.0f, 0.0f));

                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImageB));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[3].Translation = Vector3.Add((Vector3)nodes[3].Translation, new Vector3(0.3f, 0.0f, 0.0f));

                    nodes[2].Skin.InverseBindMatrices = nodes[0].Skin.InverseBindMatrices;

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same inverseBindMatrices."));
                    properties.Add(new Property(PropertyName.Difference, "The base joint for the two skins have different translations."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutput);
                    AddAnimation(animations, nodes, sampler, sampler, true);

                    properties.Add(new Property(PropertyName.Description, "Two animation channels using the same sampler."));
                    properties.Add(new Property(PropertyName.Difference, "The two animation channels target different nodes."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var inputKeys = SamplerInputLinear;
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(inputKeys, SamplerOutput);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(inputKeys, SamplerOutputReverse);
                    AddAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same input accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The two animation samplers have different output values."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var output = SamplerOutput;
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, output);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputCurve, output);
                    AddAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same output accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The two animation samplers have different input values."));
                }, (model) => { model.Camera = distantCamera; }),
                // To be implemented later. Needs to work as a type of interleaving.
                //CreateModel((properties, nodes, animations) =>
                //{
                //    var meshPrimitives = new List<Runtime.MeshPrimitive>
                //    {
                //        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                //        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                //    };
                //    meshPrimitives[0].TextureCoordSets = meshPrimitives[1].TextureCoordSets = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                //    meshPrimitives[0].Normals = meshPrimitives[1].Normals = MeshPrimitive.GetSinglePlaneNormals();

                //   foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                //    {
                //        meshPrimitive.BufferViewsInstanced = true;
                //        meshPrimitive.Material = CreateMaterial();
                //    }
                //    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                //    properties.Add(new Property(PropertyName.Description, "Two accessors using the same buffer view."));
                //}, (model) => { model.Camera = null; }),
                CreateModelwithMultipleScenes((properties, scenes, nodes,  animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane();
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    var meshPrimitive2 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageA));
                    meshPrimitive2.Material = CreateMaterial(CreateTexture(baseColorTextureImageB));
                    AddMeshPrimitivesToThreeNodes(nodes, new List<Runtime.MeshPrimitive>{meshPrimitive0, meshPrimitive1, meshPrimitive2});

                    scenes.Add(new Runtime.Scene { Nodes = nodes.GetRange(0,2), Name="scene0" });
                    scenes.Add(new Runtime.Scene { Nodes = nodes.GetRange(1,2), Name="scene1" });

                    properties.Add(new Property(PropertyName.Description, "Two scenes has the same node."));
                    properties.Add(new Property(PropertyName.Difference, "Each scene has a unique node in addition to the shared node."));
                }, (model) => { model.Camera = distantCamera; model.GLTF.Scene = 1;}),
            };

            GenerateUsedPropertiesList();
        }
    }
}
