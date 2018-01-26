using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_NegativeScale : ModelGroup
    {
        public Node_NegativeScale()
        {
            modelGroupName = ModelGroupName.Node_NegativeScale;
            onlyBinaryProperties = false;

            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal_Nodes
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor_Nodes
            };
            Runtime.Image metallicRoughnessTexture = new Runtime.Image
            {
                Uri = texture_MetallicRoughness_Nodes
            };
            usedTextures.Add(normalTexture);
            usedTextures.Add(baseColorTexture);
            usedTextures.Add(metallicRoughnessTexture);

            Runtime.GLTF defaultModel = Common.MultiNode(); // Only used to get the default tangent and normal values
            List<Vector3> normals = new List<Vector3>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals);
            List<Vector4> tangents = new List<Vector4>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents);

            var matrixNegScale = Matrix4x4.CreateScale(-2);

            properties = new List<Property>
            {
                new Property(Propertyname.Scale, new Vector3(-2, -2, -2)),
                new Property(Propertyname.Matrix, matrixNegScale),
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.MetallicRoughnessTexture, metallicRoughnessTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Matrix, matrixNegScale),
            };

            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            var scale = properties.Find(e => e.name == Propertyname.Scale);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            var metallicRoughTex = properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture);
            specialCombos.Add(new List<Property>()
            {
                normal,
                tangent,
                normTex,
                colorTex,
                metallicRoughTex
            });
            specialCombos.Add(new List<Property>()
            {
                scale,
                normal,
                tangent,
                normTex,
                colorTex,
                metallicRoughTex
            });
            specialCombos.Add(new List<Property>()
            {
                matrix,
                normal,
                tangent,
                normTex,
                colorTex,
                metallicRoughTex
            });
            specialCombos.Add(new List<Property>()
            {
                normal,
                normTex,
                colorTex,
                metallicRoughTex
            });
            specialCombos.Add(new List<Property>()
            {
                scale,
                normal,
                normTex,
                colorTex,
                metallicRoughTex
            });
            specialCombos.Add(new List<Property>()
            {
                matrix,
                normal,
                normTex,
                colorTex,
                metallicRoughTex
            });
            removeCombos.Add(new List<Property>()
            {
                normal
            });
            removeCombos.Add(new List<Property>()
            {
                tangent
            });
            removeCombos.Add(new List<Property>()
            {
                normTex
            });
            removeCombos.Add(new List<Property>()
            {
                colorTex
            });
            removeCombos.Add(new List<Property>()
            {
                metallicRoughTex
            });
            removeCombos.Add(new List<Property>()
            {
                matrix,
                scale,
                normal,
                tangent,
                normTex,
                colorTex,
                metallicRoughTex
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

            // Apply non-transforming attributes first, so they're copied to the control nodes
            foreach (Property property in combo)
            {
                if (property.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = property.value;
                }
                else if (property.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                }
                else if (property.name == Propertyname.MetallicRoughnessTexture)
                {
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = property.value;
                }
                else
                {
                    foreach (var node in nodeList)
                    {
                        if (property.name == Propertyname.VertexNormal)
                        {
                            node.Mesh.MeshPrimitives[0].Normals = property.value;
                        }
                        else if (property.name == Propertyname.VertexTangent)
                        {
                            node.Mesh.MeshPrimitives[0].Tangents = property.value;
                        }
                    }
                }
            }

            foreach (Property property in combo)
            {
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
