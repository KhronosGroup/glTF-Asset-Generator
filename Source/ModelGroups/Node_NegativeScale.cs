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
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image figureNodes = new Runtime.Image
            {
                Uri = figure_Nodes
            };
            usedTextures.Add(normalTexture);
            usedTextures.Add(baseColorTexture);
            usedFigures.Add(figureNodes);

            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
            };
            for (int x = 0; x < 23; x++)
            {
                tangents.Add(tangents[0]);
            }
            var matrixNegScale = Matrix4x4.CreateScale(-2);

            requiredProperty = new List<Property>
            {
                new Property(Propertyname.ChildNodes, figureNodes)
            };
            properties = new List<Property>
            {
                //new Property(Propertyname.Matrix, "T : [3, 3, 3]<br>R : [0.6, 0.6, 0.6]<br>S : [-2, -2, -2]"),
                new Property(Propertyname.Matrix, matrixNegScale),
                new Property(Propertyname.Scale, new Vector3(-2, 1, 1)),
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
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
            specialCombos.Add(new List<Property>()
            {
                matrix,
                normal,
                tangent,
                normTex,
                colorTex
            });
            specialCombos.Add(new List<Property>()
            {
                scale,
                normal,
                tangent,
                normTex,
                colorTex
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
                matrix,
                scale,
                normal,
                tangent,
                normTex,
                colorTex
            });
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Switch from the flat plane to the cube
            wrapper = Common.SingleCube();
            //Create the nodes by using the same properties as the original node and then changing positions slightly
            var nodeList = new List<Runtime.Node>();
            nodeList.Add(wrapper.Scenes[0].Nodes[0]);
            for (int x = 0; x < 3; x++)
            {
                nodeList.Add((DeepCopy.CloneObject(wrapper.Scenes[0].Nodes[0])));
            }

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.ChildNodes)
                {
                    // Builds the child/parent hierarchy of nodes
                    nodeList[0].Children = new List<Runtime.Node>();
                    nodeList[0].Children.Add(nodeList[1]);
                    nodeList[0].Children.Add(nodeList[2]);

                    nodeList[2].Children = new List<Runtime.Node>();
                    nodeList[2].Children.Add(nodeList[3]);

                    // Changes the new node's positions slightly
                    var originPosition = nodeList[0].Mesh.MeshPrimitives[0].Positions;
                    List<Vector3> Pos1 = new List<Vector3>();
                    List<Vector3> Pos2 = new List<Vector3>();
                    List<Vector3> Pos3 = new List<Vector3>();
                    foreach (var vec in originPosition)
                    {
                        Pos1.Add(new Vector3(vec.X - 1.2f, vec.Y - 1.2f, vec.Z));
                        Pos2.Add(new Vector3(vec.X + 1.2f, vec.Y - 1.2f, vec.Z));
                        Pos3.Add(new Vector3(vec.X + 2.4f, vec.Y - 2.4f, vec.Z));
                    }
                    nodeList[1].Mesh.MeshPrimitives[0].Positions = Pos1;
                    nodeList[2].Mesh.MeshPrimitives[0].Positions = Pos2;
                    nodeList[3].Mesh.MeshPrimitives[0].Positions = Pos3;

                    // Name the nodes for debug reasons
                    nodeList[0].Name = "Node_0";
                    nodeList[1].Name = "Node_1";
                    nodeList[2].Name = "Node_2";
                    nodeList[3].Name = "Node_3";
                }
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
                else
                {
                    foreach (var node in nodeList)
                    {
                        if (property.name == Propertyname.VertexNormal)
                        {
                            // Already set in Common.cs
                            //node.Mesh.MeshPrimitives[0].Normals = req.value;
                        }
                        else if (property.name == Propertyname.VertexTangent)
                        {
                            node.Mesh.MeshPrimitives[0].Tangents = property.value;
                        }
                    }
                }
            }

            // Creates a duplicate set of the nodes, which will retain their original positions
            var nodeListAtOrigin = DeepCopy.CloneObject(nodeList);
            wrapper.Scenes[0].Nodes.Add(nodeListAtOrigin[0]);
            nodeListAtOrigin[0].Children = new List<Runtime.Node>();
            nodeListAtOrigin[0].Children.Add(nodeListAtOrigin[1]);
            nodeListAtOrigin[0].Children.Add(nodeListAtOrigin[2]);

            nodeListAtOrigin[2].Children = new List<Runtime.Node>();
            nodeListAtOrigin[2].Children.Add(nodeListAtOrigin[3]);

            nodeListAtOrigin[0].Name = "NodeControl_0";
            nodeListAtOrigin[1].Name = "NodeControl_1";
            nodeListAtOrigin[2].Name = "NodeControl_2";
            nodeListAtOrigin[3].Name = "NodeControl_3";

            foreach (Property property in combo)
            {
                foreach (var node in nodeList)
                {
                    if (property.name == Propertyname.Matrix)
                    {
                        node.Matrix = specialProperties[0].value;
                    }
                    else if (property.name == Propertyname.Scale)
                    {
                        node.Scale = property.value;
                    }
                }
            }

            // Apply the material to each node
            foreach (var node in nodeList)
            {
                node.Mesh.MeshPrimitives[0].Material = material;
            }

            foreach (var node in nodeListAtOrigin)
            {
                node.Mesh.MeshPrimitives[0].Material = material;
            }

            return wrapper;
        }
    }
}
