﻿using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_NegativeScale : ModelGroup
    {
        public Node_NegativeScale(List<string> textures, List<string> figures) : base(textures, figures)
        {
            modelGroupName = ModelGroupName.Node_NegativeScale;
            onlyBinaryProperties = false;

            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("Normal_Nodes"))
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("BaseColor_Nodes"))
            };
            Runtime.Image metallicRoughnessTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("MetallicRoughness_Nodes"))
            };
            usedTextures.Add(normalTexture);
            usedTextures.Add(baseColorTexture);
            usedTextures.Add(metallicRoughnessTexture);

            Runtime.GLTF defaultModel = Common.MultiNode(); // Only used to get the default tangent and normal values
            List<Vector3> normals = new List<Vector3>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals);
            List<Vector4> tangents = new List<Vector4>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents);

            var matrixNegScale = Matrix4x4.CreateScale(new Vector3(-2, 1, 1));

            requiredProperty = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.MetallicRoughnessTexture, metallicRoughnessTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Scale, new Vector3(-2, 1, 1)),
                new Property(Propertyname.Matrix, matrixNegScale),
                new Property(Propertyname.VertexTangent, tangents),
                
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Matrix, matrixNegScale),
            };

            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            var scale = properties.Find(e => e.name == Propertyname.Scale);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);

            specialCombos.Add(new List<Property>()
            {
                scale,
                tangent,
            });
            specialCombos.Add(new List<Property>()
            {
                matrix,
                tangent,
            });

            removeCombos.Add(new List<Property>()
            {
                tangent,
            });
            removeCombos.Add(new List<Property>()
            {
                matrix,
                scale,
                tangent,
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Sort the combos by complexity
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                if (x.Count == 0) return -1; // Empty Set
                else if (y.Count == 0) return 1; // Empty Set
                else if ((x[0].name != Propertyname.Scale && x[0].name != Propertyname.Matrix) &&
                         (y[0].name == Propertyname.Scale || y[0].name == Propertyname.Matrix)) return -1; // Empty Set
                else if (x.Count > y.Count) return 1;
                else if (x.Count < y.Count) return -1;
                else if (x.Count == y.Count)
                {
                    // Tie goes to the combo with the left-most property on the table
                    for (int p = 0; p < x.Count; p++)
                    {
                        if (x[p].propertyGroup != y[p].propertyGroup ||
                            x[p].propertyGroup == 0)
                        {
                            int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
                            int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
                            if (xPropertyIndex > yPropertyIndex) return 1;
                            else if (xPropertyIndex < yPropertyIndex) return -1;
                        }
                    }
                    for (int p = 0; p < x.Count; p++)
                    {
                        int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
                        int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
                        if (xPropertyIndex > yPropertyIndex) return 1;
                        else if (xPropertyIndex < yPropertyIndex) return -1;
                    }
                    return 0;
                }
                else return 0;
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Switch from the flat plane to a model with multiple nodes
            wrapper = Common.MultiNode();
            var nodeList = new List<Runtime.Node>();
            nodeList.Add(wrapper.Scenes[0].Nodes[0]);
            nodeList.Add(wrapper.Scenes[0].Nodes[0].Children[0]);

            // Clear the vertex normal and tangent values already in the model
            foreach (var node in nodeList)
            {
                node.Mesh.MeshPrimitives[0].Normals = null;
                node.Mesh.MeshPrimitives[0].Tangents = null;
            }

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = req.value;
                }
                else if (req.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                }
                else if (req.name == Propertyname.MetallicRoughnessTexture)
                {
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = req.value;
                }
                else
                {
                    foreach (var node in nodeList)
                    {
                        if (req.name == Propertyname.VertexNormal)
                        {
                            node.Mesh.MeshPrimitives[0].Normals = req.value;
                        }
                    }
                }
            }

            foreach (Property property in combo)
            {
                foreach (var node in nodeList)
                {
                    if (property.name == Propertyname.VertexTangent)
                    {
                        node.Mesh.MeshPrimitives[0].Tangents = property.value;
                    }
                }

                if (property.name == Propertyname.Matrix)
                {
                    nodeList[1].Matrix = specialProperties[0].value;
                }
                else if (property.name == Propertyname.Scale)
                {
                    nodeList[1].Scale = property.value;
                }
            }

            // Apply the material to each node
            foreach (var node in nodeList)
            {
                node.Mesh.MeshPrimitives[0].Material = material;
            }

            return wrapper;
        }
    }
}
