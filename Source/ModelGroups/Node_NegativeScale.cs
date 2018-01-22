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
            Runtime.Image figureNodes = new Runtime.Image
            {
                Uri = figure_Nodes
            };
            usedTextures.Add(normalTexture);
            usedTextures.Add(baseColorTexture);
            usedTextures.Add(metallicRoughnessTexture);
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
            // Moves the model with only textures next to the empty set
            var textureControl = combos[5];
            combos.Insert(1, textureControl);
            combos.RemoveAt(6);

            // Move the two matrix combos to the end
            var matrix = combos[2];
            var matrixTextured = combos[3];
            combos.Insert(6, matrixTextured);
            combos.Insert(6, matrix);
            combos.RemoveAt(2);
            combos.RemoveAt(2);


            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Switch to a model with multiple nodes
            wrapper = Common.MultiNode();
            var nodeList = new List<Runtime.Node>();
            nodeList = wrapper.Scenes[0].Nodes;

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.ChildNodes)
                {

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

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Matrix)
                {
                    nodeList[0].Matrix = specialProperties[0].value;
                }
                else if (property.name == Propertyname.Scale)
                {
                    nodeList[0].Scale = property.value;
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
