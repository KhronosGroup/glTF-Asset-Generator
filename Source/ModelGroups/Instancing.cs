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
            Runtime.Image baseColorTextureImagePlane = UseTexture(imageList, "BaseColor_Plane");
            Runtime.Image baseColorTextureImageCube = UseTexture(imageList, "BaseColor_Cube");
            var distantCamera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.7f));
            const float quarterTurn = (FloatMath.Pi / 2.0f);

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
                    GLTF = CreateGLTF(() => new Runtime.Scene() { Nodes = nodes }, animations: animations),
                    Animated = animated,
                    Camera = new Manifest.Camera(new Vector3(0.0f, 0.0f, 2.7f))
                };

                setCamera(model);

                return model;
            }

            List<Vector4> CreateVertexColors(IEnumerable<Vector3> positions, bool useAlternateColor = false)
            {
                var colors = new List<Vector4>();
                var color = useAlternateColor ? new Vector4(0.0f, 1.0f, 0.0f, 1.0f) : new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
                foreach (Vector3 vertex in positions)
                {
                    colors.Add(color);
                }
                return colors;
            }

            Runtime.Material CreateTexturedMaterial(bool useCubeTexture = false)
            {
                return new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture { Source = useCubeTexture ? baseColorTextureImageCube : baseColorTextureImagePlane }
                    }
                };
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

            Quaternion[] GetAnimationSamplerOutputKeys(bool reverseOrder = false)
            {
                var direction = reverseOrder ? -1.0f : 1.0f;
                return new[]
                {
                    Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn * direction, 0.0f),
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, -quarterTurn * direction, 0.0f),
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn * direction, 0.0f),
                };
            }

            void AddMeshPrimitivesToSingleNode(List<Runtime.Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives)
            {
                // If there are multiple mesh primitives, offset their position so they don't overlap.
                if (meshPrimitives.Count > 1)
                {
                    var positions0 = (List<Vector3>)meshPrimitives.First().Positions;
                    var positions1 = (List<Vector3>)meshPrimitives.ElementAt(1).Positions;
                    for (int i = 0; i < positions0.Count; i++)
                    {
                        positions0[i] = new Vector3(positions0[i].X - 0.6f, positions0[i].Y, positions0[i].Z);
                        positions1[i] = new Vector3(positions1[i].X + 0.6f, positions1[i].Y, positions1[i].Z);
                    }
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

            void AddMeshPrimitivesToMultipleNodes(List<Runtime.Node> nodes, List<Runtime.MeshPrimitive> meshPrimitives0, List<Runtime.MeshPrimitive> meshPrimitives1)
            {
                nodes.AddRange(new[]
                    {
                        new Runtime.Node
                        {
                            Translation = new Vector3(-0.6f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = meshPrimitives0
                            }
                        },
                        new Runtime.Node
                        {
                            Translation = new Vector3(0.6f, 0.0f, 0.0f),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = meshPrimitives1
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

            Models = new List<Model>
            {
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateTexturedMaterial();
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
                    meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler { WrapT = WrapTEnum.CLAMP_TO_EDGE };
                    meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler { WrapT = WrapTEnum.MIRRORED_REPEAT };

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same image for the `source` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The texture sampler attribute `WrapT` on the left has a value of `CLAMP_TO_EDGE`. The right has a value of `MIRRORED_REPEAT`."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane()
                    };

                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateTexturedMaterial();
                    }
                    meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture = meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture;
                    meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorFactor = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                   properties.Add(new Property(PropertyName.Description, "Two materials using the same texture `index`."));
                   properties.Add(new Property(PropertyName.Difference, "The material on the left does not have a metallic-roughness `baseColorFactor` set.<br>The right has a value of:<br>" +
                    $"{ReadmeStringHelper.ConvertValueToString(meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorFactor)}"));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    var material = CreateTexturedMaterial();
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = material;
                    }
                    meshPrimitives[1].TextureCoordSets = new List<List<Vector2>>
                    {
                        new List<Vector2>
                        {
                            new Vector2(1.0f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, 0.0f),
                            new Vector2(1.0f, 0.0f),
                        }
                    };

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same `material` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The primitive on the left has a `TEXCOORD` value of:<br>" +
                        $"{ReadmeStringHelper.ConvertValueToString(meshPrimitives[0].TextureCoordSets)}The right has a value of:<br>{ReadmeStringHelper.ConvertValueToString(meshPrimitives[1].TextureCoordSets)}"));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives0 = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    var meshPrimitives1 = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    meshPrimitives0[0].Colors = CreateVertexColors(meshPrimitives0[0].Positions);
                    meshPrimitives1[0].Colors = CreateVertexColors(meshPrimitives1[0].Positions, useAlternateColor: true);
                    meshPrimitives0[0].Positions = meshPrimitives1[0].Positions;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives0, meshPrimitives1);

                    var colorString0 = "";
                    var colorString1 = "";
                    for (int i = 0; i < meshPrimitives0[0].Colors.Count(); i++)
                    {
                        colorString0 += ReadmeStringHelper.ConvertValueToString(meshPrimitives0[0].Colors.ElementAt(i)) + "<br>";
                        colorString1 += ReadmeStringHelper.ConvertValueToString(meshPrimitives1[0].Colors.ElementAt(i)) + "<br>";
                    }

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the `POSITION` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The primitive on the left has a vertex `COLOR_0` value of:<br>" +
                        $"{colorString0}The right has a value of:<br>{colorString1}"));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives0 = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false, includeIndices: false)
                    };
                    var meshPrimitives1 = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };

                    meshPrimitives0[0].Colors = CreateVertexColors(meshPrimitives0[0].Positions);
                    meshPrimitives1[0].Colors = CreateVertexColors(meshPrimitives1[0].Positions, useAlternateColor: true);
                    meshPrimitives0[0].Indices = meshPrimitives1[0].Indices;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives0, meshPrimitives1);

                    var colorString0 = "";
                    var colorString1 = "";
                    for (int i = 0; i < meshPrimitives0[0].Colors.Count(); i++)
                    {
                        colorString0 += ReadmeStringHelper.ConvertValueToString(meshPrimitives0[0].Colors.ElementAt(i)) + "<br>";
                        colorString1 += ReadmeStringHelper.ConvertValueToString(meshPrimitives1[0].Colors.ElementAt(i)) + "<br>";
                    }

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the `indices` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The primitive on the left has a vertex `COLOR_0` value of:<br>" +
                        $"{colorString0}The right has a value of:<br>{colorString1}"));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateSinglePlane(includeTextureCoords: false) };
                    meshPrimitives[0].Colors = CreateVertexColors(meshPrimitives[0].Positions);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives, meshPrimitives);
                    nodes[1].Mesh = nodes[0].Mesh;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same `mesh` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The node on the left has a translation of:<br>" +
                        $"{ReadmeStringHelper.ConvertValueToString(nodes[0].Translation)}<br>The right has a value of:<br>{ReadmeStringHelper.ConvertValueToString(nodes[1].Translation)}"));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    var meshPositions0 = (List<Vector3>)nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions;
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(meshPositions0);

                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    var meshPositions1 = (List<Vector3>)nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions;
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(meshPositions1, useAlternateColor: true);

                    // Offsets the position of both meshes so they don't overlap.
                    // Also builds strings used to display the values in the readme.
                    var positionString0 = "";
                    var positionString1 = "";
                    var colorString0 = "";
                    var colorString1 = "";
                    for (int i = 0; i < meshPositions0.Count; i++)
                    {
                        meshPositions0[i] = new Vector3(meshPositions0[i].X - 0.3f, meshPositions0[i].Y, meshPositions0[i].Z);
                        meshPositions1[i] = new Vector3(meshPositions1[i].X + 0.3f, meshPositions1[i].Y, meshPositions1[i].Z);

                        positionString0 += ReadmeStringHelper.ConvertValueToString(meshPositions0[i]) + "<br>";
                        positionString1 += ReadmeStringHelper.ConvertValueToString(meshPositions1[i]) + "<br>";
                        colorString0 += ReadmeStringHelper.ConvertValueToString(meshPositions0[i]) + "<br>";
                        colorString1 += ReadmeStringHelper.ConvertValueToString(meshPositions1[i]) + "<br>";
                    }

                    nodes[2].Skin = nodes[0].Skin;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same `skin` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The mesh on the left has a `POSITION` value of:<br>" +
                        $"{positionString0}and a vertex `COLOR_0` value of:<br>{colorString0}" +
                        $"The right has a value of:<br>{positionString1}and<br>{colorString1}."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    var meshPositions0 = (List<Vector3>)nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions;
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(meshPositions0);

                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    var meshPositions1 = (List<Vector3>)nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions;
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(meshPositions1, useAlternateColor: true);

                    // Offsets the position of both meshes so they don't overlap.
                    for (int i = 0; i < meshPositions0.Count; i++)
                    {
                        meshPositions0[i] = new Vector3(meshPositions0[i].X - 0.3f, meshPositions0[i].Y, meshPositions0[i].Z);
                        meshPositions1[i] = new Vector3(meshPositions1[i].X + 0.3f, meshPositions1[i].Y, meshPositions1[i].Z);
                    }

                    var jointRotation = Matrix4x4.CreateFromQuaternion((Quaternion)nodes[1].Children.First().Rotation);
                    Matrix4x4.Invert(jointRotation, out var jointRotationInverted);
                    nodes[2].Skin.InverseBindMatrices = new List<Matrix4x4>
                    {
                        Matrix4x4.Multiply(nodes[2].Skin.InverseBindMatrices.First(), Matrix4x4.CreateRotationZ(quarterTurn / 2)),
                        Matrix4x4.Multiply(jointRotation, Matrix4x4.Multiply(Matrix4x4.CreateRotationZ(quarterTurn /2), jointRotationInverted))
                    };

                    nodes[2].Skin.Joints = nodes[0].Skin.Joints;

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same `joints` attributes."));
                    properties.Add(new Property(PropertyName.Difference, "The skin on the left has `inverseBindMatrices` of:<br>" +
                        $"{ReadmeStringHelper.ConvertValueToString(nodes[0].Skin.InverseBindMatrices.ToList())}The right has a values of:<br>{ReadmeStringHelper.ConvertValueToString(nodes[2].Skin.InverseBindMatrices)}"));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions);
                    nodes[1].Translation = Vector3.Add((Vector3)nodes[1].Translation, new Vector3(-0.3f, 0.0f, 0.0f));

                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Colors = CreateVertexColors(nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions, useAlternateColor: true);
                    nodes[3].Translation = Vector3.Add((Vector3)nodes[3].Translation, new Vector3(0.3f, 0.0f, 0.0f));

                    nodes[2].Skin.InverseBindMatrices = nodes[0].Skin.InverseBindMatrices;

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same `inverseBindMatrices` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The joints of the skin on the left has a translation of:<br>" +
                        $"{ReadmeStringHelper.ConvertValueToString(nodes[1].Translation)}<br>The right has a value of:<br>{ReadmeStringHelper.ConvertValueToString(nodes[3].Translation)}"));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateCube() };
                    meshPrimitives[0].Material = CreateTexturedMaterial(useCubeTexture: true);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives, meshPrimitives);

                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(GetAnimationSamplerInputKeys(), GetAnimationSamplerOutputKeys(reverseOrder: true));
                    BuildAnimation(animations, nodes, sampler, sampler, true);

                    properties.Add(new Property(PropertyName.Description, "Two animation channels using the same `sampler` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The first animation channel points at the node on the left.<br>The second points at the node on the right."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateCube() };
                    meshPrimitives[0].Material = CreateTexturedMaterial(useCubeTexture: true);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives, meshPrimitives);

                    var inputKeys = GetAnimationSamplerInputKeys();
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(inputKeys, GetAnimationSamplerOutputKeys());
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(inputKeys, GetAnimationSamplerOutputKeys(reverseOrder: true));
                    BuildAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same `input` accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The animation sampler on the left has an output of:<br>" +
                        $"{ReadmeStringHelper.ConvertValueToString(sampler0.OutputKeys.ToList())}The right has a value of:<br>{ReadmeStringHelper.ConvertValueToString(sampler1.OutputKeys.ToList())}"));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive> { MeshPrimitive.CreateCube() };
                    meshPrimitives[0].Material = CreateTexturedMaterial(useCubeTexture: true);
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitives, meshPrimitives);

                    Quaternion[] output = GetAnimationSamplerOutputKeys();
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(GetAnimationSamplerInputKeys(), output);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(
                        new[]
                        {
                            0.0f,
                            0.5f,
                            1.0f,
                            2.0f,
                            4.0f,
                        },
                        output
                    );
                    BuildAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same `output` accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The animation sampler on the left has an input of:<br>" +
                        $"{ReadmeStringHelper.ConvertValueToString(sampler0.InputKeys.ToList())}<br>The right has a value of:<br>{ReadmeStringHelper.ConvertValueToString(sampler1.InputKeys.ToList())}"));
                }, (model) => { model.Camera = distantCamera; }),
                // To be implemented later. Needs to work as a type of interleaving.
                //CreateModel((properties, nodes, animations) => {
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
            };

            GenerateUsedPropertiesList();
        }
    }
}
