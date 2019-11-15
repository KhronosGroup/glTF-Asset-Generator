using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Instancing : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Instancing;

        public Instancing(List<string> imageList)
        {
            var baseColorImageA = UseTexture(imageList, "BaseColor_A");
            var baseColorImageB = UseTexture(imageList, "BaseColor_B");
            var baseColorImageCube = UseTexture(imageList, "BaseColor_Cube");
            var distantCamera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.7f));

            // There are no common properties in this model group that are reported in the readme.

            TextureInfo CreateTextureInfo(Image source)
            {
                return new TextureInfo
                {
                    Texture = new Texture
                    {
                        Source = source,
                    }
                };
            }

            Model CreateModel(Action<List<Property>, List<Node>, List<Animation>> setProperties, Action<Model> setCamera)
            {
                var properties = new List<Property>();
                var animations = new List<Animation>();
                var animated = true;
                var nodes = new List<Node>();

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
                    GLTF = CreateGLTF(() => new Scene { Nodes = nodes }, animations: animations),
                    Animated = animated,
                };

                setCamera(model);

                return model;
            }

            var samplerInputLinear = Data.Create(new[]
            {
                0.0f,
                1.0f,
                2.0f,
                3.0f,
                4.0f,
            });

            var samplerInputCurve = Data.Create(new[]
            {
                0.0f,
                0.5f,
                1.0f,
                2.0f,
                4.0f,
            });

            var samplerOutput = Data.Create(new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
            });

            var samplerOutputReverse = Data.Create(new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(90.0f), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ToRadians(-90.0f), 0.0f),
            });

            Runtime.Material CreateMaterial(TextureInfo textureInfo)
            {
                return new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = textureInfo
                    }
                };
            }

            void AddMeshPrimitivesToSingleNode(List<Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives)
            {
                // If there are multiple mesh primitives, offset their position so they don't overlap.
                if (meshPrimitives.Count > 1)
                {
                    meshPrimitives[0].Positions.Values = meshPrimitives[0].Positions.Values.Select(position => { return new Vector3(position.X - 0.6f, position.Y, position.Z); });
                    meshPrimitives[1].Positions.Values = meshPrimitives[1].Positions.Values.Select(position => { return new Vector3(position.X + 0.6f, position.Y, position.Z); });
                }

                nodes.Add(
                    new Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = meshPrimitives
                        }
                    }
                );
            }

            void AddMeshPrimitivesToMultipleNodes(List<Node> nodes, Runtime.MeshPrimitive meshPrimitives0, Runtime.MeshPrimitive meshPrimitives1)
            {
                nodes.AddRange(new[]
                    {
                        new Node
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
                        new Node
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

            void AddAnimation(List<Animation> animations, List<Node> nodes, AnimationSampler sampler0, AnimationSampler sampler1, bool samplerInstanced)
            {
                animations.Add(new Animation
                {
                    Channels = new List<AnimationChannel>
                    {
                        new AnimationChannel
                        {
                            Target = new AnimationChannelTarget
                            {
                                Node = nodes[0],
                                Path = AnimationChannelTargetPath.Rotation,
                            },
                            Sampler = sampler0
                        },
                        new AnimationChannel
                        {
                            Target = new AnimationChannelTarget
                            {
                                Node = nodes[1],
                                Path = AnimationChannelTargetPath.Rotation,
                            },
                            Sampler = sampler1
                        },
                    }
                });
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
                        meshPrimitive.TexCoords0 = Data.Create<Vector2>
                        (
                            new[]
                            {
                                new Vector2( 1.3f,  1.3f),
                                new Vector2(-0.3f,  1.3f),
                                new Vector2(-0.3f, -0.3f),
                                new Vector2( 1.3f, -0.3f),
                            }
                        );
                    }

                    meshPrimitives[0].Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    meshPrimitives[1].Material = CreateMaterial(CreateTextureInfo(baseColorImageA));

                    meshPrimitives[0].Material.PbrMetallicRoughness.BaseColorTexture.Texture.Sampler = new Sampler
                    {
                        WrapT = SamplerWrap.ClampToEdge,
                        WrapS = SamplerWrap.ClampToEdge
                    };
                    meshPrimitives[1].Material.PbrMetallicRoughness.BaseColorTexture.Texture.Sampler = new Sampler
                    {
                        WrapT = SamplerWrap.MirroredRepeat,
                        WrapS = SamplerWrap.MirroredRepeat
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

                    meshPrimitives[0].Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    meshPrimitives[1].Material = CreateMaterial(CreateTextureInfo(baseColorImageB));

                    var sampler = new Sampler
                    {
                        WrapT = SamplerWrap.ClampToEdge,
                        WrapS = SamplerWrap.ClampToEdge
                    };
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material.PbrMetallicRoughness.BaseColorTexture.Texture.Sampler = sampler;
                        // This non-standard set of texture coordinates is larger than the texture but not an exact multiple, so it allows texture sampler settings to be visible.
                        meshPrimitive.TexCoords0 = Data.Create<Vector2>
                        (
                            new[]
                            {
                                new Vector2( 1.3f,  1.3f),
                                new Vector2(-0.3f,  1.3f),
                                new Vector2(-0.3f, -0.3f),
                                new Vector2( 1.3f, -0.3f),
                            }
                        );
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

                    var texture = CreateTextureInfo(baseColorImageA);
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateMaterial(texture);
                    }
                    meshPrimitives[0].Material.PbrMetallicRoughness.BaseColorTexture = meshPrimitives[1].Material.PbrMetallicRoughness.BaseColorTexture;
                    meshPrimitives[1].Material.PbrMetallicRoughness.BaseColorFactor = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);

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
                    var material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = material;
                    }

                    // One of the primitives has a 'zoomed in' texture coordinate set.
                    meshPrimitives[1].TexCoords0 = Data.Create<Vector2>
                    (
                        new[]
                        {
                            new Vector2(0.9f, 0.9f),
                            new Vector2(0.1f, 0.9f),
                            new Vector2(0.1f, 0.1f),
                            new Vector2(0.9f, 0.1f),
                        }
                    );

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same material."));
                    properties.Add(new Property(PropertyName.Difference, "One primitive has texture coordinates that displays all of texture A, while the other primitive has textures coordinates that don't display the border."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane();
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    meshPrimitive1.Material = CreateMaterial(CreateTextureInfo(baseColorImageB));

                    meshPrimitive0.Positions = meshPrimitive1.Positions;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the `POSITION` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "One primitive uses texture A while the other primitive uses texture B."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane(includeIndices: false);
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    meshPrimitive1.Material = CreateMaterial(CreateTextureInfo(baseColorImageB));

                    meshPrimitive0.Indices = meshPrimitive1.Indices;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for indices."));
                    properties.Add(new Property(PropertyName.Difference, "One primitive uses texture A while the other primitive uses texture B."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive.Material = CreateMaterial(CreateTextureInfo(baseColorImageA));

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive, meshPrimitive);
                    nodes[1].Mesh = nodes[0].Mesh;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same mesh."));
                    properties.Add(new Property(PropertyName.Difference, "The two nodes have different translations."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive.Material = CreateMaterial(CreateTextureInfo(baseColorImageA));

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive, meshPrimitive);
                    nodes[1].Mesh = nodes[0].Mesh;

                    // Adds just the node containing the negative scale.
                    nodes[0].Scale = new Vector3(1.0f, -1.0f, 1.0f);

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same mesh."));
                    properties.Add(new Property(PropertyName.Difference, "One node has a [1.0, -1.0, 1.0] scale and the other has a default scale."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TexCoords0 = Data.Create(Nodes.GetSkinATexCoords());

                    // Adds just the node containing the mesh, dropping the data for a second set of joints.
                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTextureInfo(baseColorImageB));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TexCoords0 = Data.Create(Nodes.GetSkinATexCoords());
                    nodes[2].Skin = nodes[0].Skin;

                    // Offsets the position of both meshes so they don't overlap.
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Values = nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Values.Select(position => { return new Vector3(position.X - 0.3f, position.Y, position.Z); } );
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Values = nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Values.Select(position => { return new Vector3(position.X + 0.3f, position.Y, position.Z); } );

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same skin."));
                    properties.Add(new Property(PropertyName.Difference, "The two mesh primitives have different `POSITION` values."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) =>
                {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TexCoords0 = Data.Create(Nodes.GetSkinATexCoords());

                    // Adds just the node containing the mesh, dropping the data for a second set of joints.
                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTextureInfo(baseColorImageB));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TexCoords0 = Data.Create(Nodes.GetSkinATexCoords());
                    nodes[2].Skin.Joints = nodes[0].Skin.Joints;

                    // Creates new inverseBindMatrices for the second skin, rotating the flap further than the default value would.
                    nodes[2].Skin.InverseBindMatrices = Data.Create(new[]
                    {
                        nodes[2].Skin.InverseBindMatrices.Values.First(),
                        Matrix4x4.Multiply(nodes[2].Skin.InverseBindMatrices.Values.ElementAt(1), Matrix4x4.CreateRotationX(FloatMath.ToRadians(-30))),
                    });

                    // Offsets the position of both meshes so they don't overlap.
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Values = nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Values.Select(position => new Vector3(position.X - 0.3f, position.Y, position.Z));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Values = nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Values.Select(position => new Vector3(position.X + 0.3f, position.Y, position.Z));

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same joints."));
                    properties.Add(new Property(PropertyName.Difference, "The skin with texture B has inverseBindMatrices that fold twice as far as the skin with texture A."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) =>
                {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTextureInfo(baseColorImageA));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TexCoords0 = Data.Create(Nodes.GetSkinATexCoords());
                    nodes[1].Translation = Vector3.Add((Vector3)nodes[1].Translation, new Vector3(-0.3f, 0.0f, 0.0f));

                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTextureInfo(baseColorImageB));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TexCoords0 = Data.Create(Nodes.GetSkinATexCoords());
                    nodes[3].Translation = Vector3.Add((Vector3)nodes[3].Translation, new Vector3(0.3f, 0.0f, 0.0f));

                    nodes[2].Skin.InverseBindMatrices = nodes[0].Skin.InverseBindMatrices;

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same inverseBindMatrices."));
                    properties.Add(new Property(PropertyName.Difference, "The base joint for the two skins have different translations."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    var textureInfo = CreateTextureInfo(baseColorImageCube);
                    meshPrimitive0.Material = CreateMaterial(textureInfo);
                    meshPrimitive1.Material = CreateMaterial(textureInfo);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var sampler = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = samplerInputLinear,
                        Output = samplerOutput,
                    };

                    AddAnimation(animations, nodes, sampler, sampler, true);

                    properties.Add(new Property(PropertyName.Description, "Two animation channels using the same sampler."));
                    properties.Add(new Property(PropertyName.Difference, "The two animation channels target different nodes."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    var textureInfo = CreateTextureInfo(baseColorImageCube);
                    meshPrimitive0.Material = CreateMaterial(textureInfo);
                    meshPrimitive1.Material = CreateMaterial(textureInfo);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = samplerInputLinear,
                        Output = samplerOutput,
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = samplerInputLinear,
                        Output = samplerOutputReverse,
                    };

                    AddAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same input accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The two animation samplers have different output values."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) =>
                {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    var textureInfo = CreateTextureInfo(baseColorImageCube);
                    meshPrimitive0.Material = CreateMaterial(textureInfo);
                    meshPrimitive1.Material = CreateMaterial(textureInfo);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var sampler0 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = samplerInputLinear,
                        Output = samplerOutput,
                    };

                    var sampler1 = new AnimationSampler
                    {
                        Interpolation = AnimationSamplerInterpolation.Linear,
                        Input = samplerInputCurve,
                        Output = samplerOutput,
                    };

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
                //    meshPrimitives[0].TexCoords0 = meshPrimitives[1].TexCoords0 = MeshPrimitive.GetSinglePlaneTextureCoordSets();
                //    meshPrimitives[0].Normals = meshPrimitives[1].Normals = MeshPrimitive.GetSinglePlaneNormals();

                //   foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                //    {
                //        meshPrimitive.BufferViewsInstanced = true;
                //        meshPrimitive.Material = CreateMaterial();
                //    }
                //    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                //    properties.Add(new Property(PropertyName.Description, "Two accessors using the same buffer view."));
                //}, (model) => { model.Camera = null; }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
