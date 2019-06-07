using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Instantiation : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Instantiation;

        public Instantiation(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, List<Runtime.MeshPrimitive>, List<Runtime.Node>> setProperties)
            {
                var properties = new List<Property>();
                var cubeMeshPrimitive = MeshPrimitive.CreateCube();

                // Apply the common properties to the gltf.
                cubeMeshPrimitive.Material = new Runtime.Material()
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                    {
                        BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                    },
                };

                var nodes = new List<Runtime.Node>();
                var meshPrimitives = new List<Runtime.MeshPrimitive>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitives, nodes);

                // Create the gltf object.
                Runtime.GLTF gltf = CreateGLTF(() => new Runtime.Scene()
                {
                    Nodes = nodes
                });

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                    Animated = true,
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitives, nodes) => {
                   meshPrimitives.AddRange(MeshPrimitive.CreateMultiPrimitivePlane());

                   for (int i = 0; i < meshPrimitives.Count; i++)
                   {
                       meshPrimitives[i].Material = new Runtime.Material
                       {
                           MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                           {
                               BaseColorTexture = new Runtime.Texture
                               {
                                   Source = baseColorTextureImage,
                                   Name = $"texture_{i}"
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
                CreateModel((properties, meshPrimitives, nodes) => {
                   meshPrimitives.AddRange(MeshPrimitive.CreateMultiPrimitivePlane());
                   var texture = new Runtime.Texture { Source = baseColorTextureImage };

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
                CreateModel((properties, meshPrimitives, nodes) => {
                    // DEBUG - NYI in Runtime!
                    meshPrimitives.AddRange(MeshPrimitive.CreateMultiPrimitivePlane());
                    var material = new Runtime.Material
                    {
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                        {
                            BaseColorTexture = new Runtime.Texture
                            {
                                Source = baseColorTextureImage,
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
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     meshPrimitives.Add(MeshPrimitive.CreateSinglePlane(includeIndices: false));
                //     meshPrimitives.Add(MeshPrimitive.CreateSinglePlane(includeIndices: false));

                //     nodes.Add(
                //        new Runtime.Node
                //        {
                //            Mesh = new Runtime.Mesh
                //            {
                //                MeshPrimitives = meshPrimitives
                //            }
                //         }
                //     );

                //     properties.Add(new Property(PropertyName.Description, "Two primitives attributes using the same accessors (POSITION, NORMAL, TANGENT, TEXTCOORD)."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     meshPrimitives.Add(MeshPrimitive.CreateSinglePlane());
                //     meshPrimitives.Add(MeshPrimitive.CreateSinglePlane());

                //     nodes.Add(
                //        new Runtime.Node
                //        {
                //            Mesh = new Runtime.Mesh
                //            {
                //                MeshPrimitives = meshPrimitives
                //            }
                //         }
                //     );

                //     properties.Add(new Property(PropertyName.Description, "Two primitives indices using the same accessors."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     meshPrimitives.Add(MeshPrimitive.CreateSinglePlane());

                //     nodes.AddRange(new[]
                //     {
                //        new Runtime.Node
                //        {
                //            Translation = new Vector3(-0.5f, 0.0f, 0.0f),
                //            Mesh = new Runtime.Mesh
                //            {
                //                MeshPrimitives = meshPrimitives
                //            }
                //        },
                //        new Runtime.Node
                //        {
                //            Translation = new Vector3(0.5f, 0.0f, 0.0f),
                //            Mesh = new Runtime.Mesh
                //            {
                //                MeshPrimitives = meshPrimitives
                //            }
                //        }
                //     });

                //     properties.Add(new Property(PropertyName.Description, "Two nodes using the same mesh."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                //     nodes[0].Name = "plane0";
                //     nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);

                //     nodes.Add(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3)[0]);
                //     nodes[2].Name = "plane1";
                //     nodes[2].Translation = new Vector3(0.5f, 0.0f, 0.0f);
                //     nodes[2].Skin = nodes[0].Skin;

                //     properties.Add(new Property(PropertyName.Description, "Two nodes using the same skin."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                //     nodes[0].Name = "plane0";
                //     nodes[0].Translation = new Vector3(-0.5f, 0.0f, 0.0f);

                //     nodes.AddRange(Nodes.CreateFoldingPlaneSkin("skinA", 2, 3));
                //     nodes[2].Name = "plane1";
                //     nodes[2].Translation = new Vector3(0.5f, 0.0f, 0.0f);
                //     nodes[2].Skin.SkinJoints = nodes[0].Skin.SkinJoints;

                //     properties.Add(new Property(PropertyName.Description, "Two skins using the same skeleton."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     properties.Add(new Property(PropertyName.Description, "Two skins using the same inverseBindMatrices."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     properties.Add(new Property(PropertyName.Description, "Two animation samplers using the same accessors."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     properties.Add(new Property(PropertyName.Description, "Two animation channels using the same samplers."));
                // }),
                // CreateModel((properties, meshPrimitives, nodes) => {
                //     properties.Add(new Property(PropertyName.Description, "Two buffer views using the same buffers."));
                // }),
                // Morph NYI
                // CreateModel((properties, channels, node) => {
                //     properties.Add(new Property(PropertyName.Description, "Two morph target attributes using the same accessors."));
                // }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
