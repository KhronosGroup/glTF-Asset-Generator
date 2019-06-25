﻿using System;
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
        private readonly Vector4 ColorGreen;
        private readonly Vector4 ColorBlue;
        public Instancing(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage1 = UseTexture(imageList, "BaseColor_1");
            Runtime.Image baseColorTextureImage2 = UseTexture(imageList, "BaseColor_2");
            Runtime.Image baseColorTextureImagePlane = UseTexture(imageList, "BaseColor_Plane");
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
                    GLTF = CreateGLTF(() => new Runtime.Scene() { Nodes = nodes }, animations: animations),
                    Animated = animated,
                };

                setCamera(model);

                return model;
            }

            IList<float> SamplerInputLinear = Array.AsReadOnly(new[]
            {
                0.0f,
                1.0f,
                2.0f,
                3.0f,
                4.0f,
            });

            IList<float> SamplerInputCurve = Array.AsReadOnly(new[]
            {
                0.0f,
                0.5f,
                1.0f,
                2.0f,
                4.0f,
            });

            IList<Quaternion> SamplerOutputForward = Array.AsReadOnly(new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(90), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(90), 0.0f),
            });

            IList<Quaternion> SamplerOutputReverse = Array.AsReadOnly(new[]
            {
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(90), 0.0f),
                Quaternion.Identity,
                Quaternion.CreateFromYawPitchRoll(0.0f, FloatMath.ConvertDegreesToRadians(-90), 0.0f),
            });

            ColorGreen = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            ColorBlue = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            List<Vector4> CreateVertexColors(int vertexEndIndex, Vector4 color)
            {
                var colors = new List<Vector4>();
                for (int i = 0; i < vertexEndIndex; i++)
                {
                    colors.Add(color);
                }
                return colors;
            }

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
                    meshPrimitives[0].Positions = meshPrimitives[0].Positions.Select(vec => { return new Vector3(vec.X - 0.6f, vec.Y, vec.Z); } ).ToArray();
                    meshPrimitives[1].Positions = meshPrimitives[1].Positions.Select(vec => { return new Vector3(vec.X + 0.6f, vec.Y, vec.Z); } ).ToArray();
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

                    meshPrimitives[0].Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    meshPrimitives[1].Material = CreateMaterial(CreateTexture(baseColorTextureImage1));

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

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same image for the `source` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The texture sampler attributes `WrapT` and `WrapS` viewed on the left are set as `CLAMP_TO_EDGE`.<br><br>The values viewed on the right are `MIRRORED_REPEAT`."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };

                    meshPrimitives[0].Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    meshPrimitives[1].Material = CreateMaterial(CreateTexture(baseColorTextureImage2));

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

                    properties.Add(new Property(PropertyName.Description, "Two textures using the same value for the `sampler` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The `image` viewed on the left is the #1 textures.<br><br>Viewed on the right is the #2 texture."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane()
                    };

                    var texture = CreateTexture(baseColorTextureImagePlane);
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = CreateMaterial(texture);
                    }
                    meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture = meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture;
                    meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorFactor = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);

                    AddMeshPrimitivesToSingleNode(nodes, meshPrimitives);

                   properties.Add(new Property(PropertyName.Description, "Two materials using the same texture `index`."));
                   properties.Add(new Property(PropertyName.Difference, "The material viewed on the left does not have a metallic-roughness `baseColorFactor` set.<br><br>The material viewed on the right has a blueish `baseColorFactor`."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        MeshPrimitive.CreateSinglePlane(),
                        MeshPrimitive.CreateSinglePlane(includeTextureCoords: false)
                    };
                    var material = CreateMaterial(CreateTexture(baseColorTextureImagePlane));
                    foreach (Runtime.MeshPrimitive meshPrimitive in meshPrimitives)
                    {
                        meshPrimitive.Material = material;
                    }
                    // One of the primitives has a 'zoomed in' texture coordinate set.
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
                    properties.Add(new Property(PropertyName.Difference, "The primitive viewed on the left has a `TEXCOORD` value that displays the entire texture.<br><br>The value viewed on the right is zoomed in on a corner of the texture."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane();
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImage2));

                    meshPrimitive0.Positions = meshPrimitive1.Positions;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the `POSITION` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The primitive viewed on the left has a `material` with the #1 texture.<br><br>The primitive viewed to the right has a material with the #2 texture."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive0 = MeshPrimitive.CreateSinglePlane(includeIndices: false);
                    var meshPrimitive1 = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImage2));

                    meshPrimitive0.Indices = meshPrimitive1.Indices;

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    properties.Add(new Property(PropertyName.Description, "Two primitives using the same accessors for the `indices` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The primitive viewed to the left has a `material` with the #1 texture.<br><br>The primitive viewed to the right has a material with the #2 texture."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                    meshPrimitive.Material = CreateMaterial(CreateTexture(baseColorTextureImage1));

                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive, meshPrimitive);
                    nodes[1].Mesh = nodes[0].Mesh;

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same `mesh` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The node viewed on the left has a `translation` to the left on the X axis.<br><br>The node viewed on the right has a `translation` to the right on the X axis."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();

                    // Adds just the node containing the mesh, dropping the data for a second set of joints.
                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImage2));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[2].Skin = nodes[0].Skin;

                    // Offsets the position of both meshes so they don't overlap.
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(vec => { return new Vector3(vec.X - 0.3f, vec.Y, vec.Z); } ).ToArray();
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(vec => { return new Vector3(vec.X + 0.3f, vec.Y, vec.Z); } ).ToArray();

                    properties.Add(new Property(PropertyName.Description, "Two nodes using the same `skin` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The mesh viewed on the left has `POSITION` values further on the left of the X axis.<br><br>The mesh viewed to the right has values further on the right of the X axis."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();

                    // Adds just the node containing the mesh, dropping the data for a second set of joints.
                    nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImage2));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[2].Skin.Joints = nodes[0].Skin.Joints;

                    // Creates new inverseBindMatrices for the second skin, rotating the flap further than the default value would.
                    nodes[2].Skin.InverseBindMatrices = new[]
                    {
                        nodes[2].Skin.InverseBindMatrices.First(),
                        Matrix4x4.Multiply(nodes[2].Skin.InverseBindMatrices.ElementAt(1), Matrix4x4.CreateRotationX(FloatMath.ConvertDegreesToRadians(-30))),
                    };

                    // Offsets the position of both meshes so they don't overlap.
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[0].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(vec => { return new Vector3(vec.X - 0.3f, vec.Y, vec.Z); } ).ToArray();
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions = nodes[2].Mesh.MeshPrimitives.ElementAt(0).Positions.Select(vec => { return new Vector3(vec.X + 0.3f, vec.Y, vec.Z); } ).ToArray();

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same `joints` attributes."));
                    properties.Add(new Property(PropertyName.Difference, "The skin viewed on the left has `inverseBindMatrices` that stand the model up and fold it slightly.<br><br>The values viewed on the right are similar, but fold the model twice as much."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) => {
                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[0].Name = "plane0";
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImage1));
                    nodes[0].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[1].Translation = Vector3.Add((Vector3)nodes[1].Translation, new Vector3(-0.3f, 0.0f, 0.0f));

                    nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                    nodes[2].Name = "plane1";
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).Material = CreateMaterial(CreateTexture(baseColorTextureImage2));
                    nodes[2].Mesh.MeshPrimitives.ElementAt(0).TextureCoordSets = Nodes.GetSkinATextureCoordSets();
                    nodes[3].Translation = Vector3.Add((Vector3)nodes[3].Translation, new Vector3(0.3f, 0.0f, 0.0f));

                    nodes[2].Skin.InverseBindMatrices = nodes[0].Skin.InverseBindMatrices;

                    properties.Add(new Property(PropertyName.Description, "Two skins using the same `inverseBindMatrices` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The base joint for the skin viewed on the left has a `translation` to the left along the X axis.<br><br>The base joint viewed to the right has a `translation` to the right along the X axis."));
                }, (model) => { model.Camera = null; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var sampler = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, SamplerOutputForward);
                    AddAnimation(animations, nodes, sampler, sampler, true);

                    properties.Add(new Property(PropertyName.Description, "Two animation channels using the same `sampler` attribute."));
                    properties.Add(new Property(PropertyName.Difference, "The first animation channel targets the node viewed on the left.<br><br>The second targets the node viewed on the right."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var inputKeys = SamplerInputLinear;
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(inputKeys, SamplerOutputForward);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(inputKeys, SamplerOutputReverse);
                    AddAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same `input` accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The animation sampler viewed on the left has an output that rotates the model back and forth.<br><br>The values viewed on the right also rotates the model, but in the opposite directions."));
                }, (model) => { model.Camera = distantCamera; }),
                CreateModel((properties, nodes, animations) => {
                    var meshPrimitive0 = MeshPrimitive.CreateCube();
                    var meshPrimitive1 = MeshPrimitive.CreateCube();
                    meshPrimitive0.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    meshPrimitive1.Material = CreateMaterial(CreateTexture(baseColorTextureImageCube));
                    AddMeshPrimitivesToMultipleNodes(nodes, meshPrimitive0, meshPrimitive1);

                    var output = SamplerOutputForward;
                    var sampler0 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputLinear, output);
                    var sampler1 = new Runtime.LinearAnimationSampler<Quaternion>(SamplerInputCurve, output);
                    AddAnimation(animations, nodes, sampler0, sampler1, false);

                    properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same `output` accessors."));
                    properties.Add(new Property(PropertyName.Difference, "The animation sampler viewed on the left has an input that increaces in a linear fashion.<br><br>The animation sampler viewed on the right increaces at a variable rate."));
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
