using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Node_NegativeScale : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Node_NegativeScale;

        public Node_NegativeScale(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");
            var normalImage = UseTexture(imageList, "Normal_Nodes");
            var metallicRoughnessTextureImage = UseTexture(imageList, "MetallicRoughness_Nodes");

            // Track the common properties for use in the readme.
            var translationValue = new Vector3(0, 2, 0);
            var matrixTranslationValue = Matrix4x4.CreateTranslation(translationValue); 
            CommonProperties.Add(new Property(PropertyName.Translation, translationValue));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalImage));
            CommonProperties.Add(new Property(PropertyName.MetallicRoughnessTexture, metallicRoughnessTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.Node, Runtime.Node> setProperties)
            {
                var properties = new List<Property>();
                var gltf = Gltf.CreateMultiNode();
                var nodes = new List<Runtime.Node>()
                {
                    gltf.Scenes[0].Nodes[0],
                    gltf.Scenes[0].Nodes[0].Children[0],
                };

                // Apply the common properties to the gltf.
                foreach (var node in nodes)
                {
                    node.Mesh.MeshPrimitives[0].Material = new Runtime.Material()
                    {
                        NormalTexture = new Runtime.Texture() { Source = normalImage },
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                        {
                            BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                            MetallicRoughnessTexture = new Runtime.Texture() { Source = metallicRoughnessTextureImage },
                        },
                    };
                }

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes[0], nodes[1]);

                // Applies a translation to avoid clippine the other node. 
                // Models with a matrix applied have the translation applied in that matrix.
                if (properties.Find(e => e.Name == PropertyName.Matrix) == null)
                {
                    nodes[1].Translation = translationValue;
                }

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => gltf.Scenes[0]),
                };
            }

            void SetMatrixScaleX(List<Property> properties, Runtime.Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, 1, 1)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix));
            }

            void SetMatrixScaleXY(List<Property> properties, Runtime.Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, -1, 1)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix));
            }

            void SetMatrixScaleXYZ(List<Property> properties, Runtime.Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, -1, -1)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix));
            }

            void SetScaleX(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(-1, 1, 1);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetScaleXY(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(-1, -1, 1);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetScaleXYZ(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(-1, -1, -1);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetVertexNormal(List<Property> properties, Runtime.Node nodeZero, Runtime.Node nodeOne)
            {
                var normals = Gltf.GetMultiNodeNormals();
                nodeZero.Mesh.MeshPrimitives[0].Normals = normals;
                nodeOne.Mesh.MeshPrimitives[0].Normals = normals;
                properties.Add(new Property(PropertyName.VertexNormal, ":white_check_mark:"));
            }

            void SetVertexTangent(List<Property> properties, Runtime.Node nodeZero, Runtime.Node nodeOne)
            {
                var tangents = Gltf.GetMultiNodeTangents();
                nodeZero.Mesh.MeshPrimitives[0].Tangents = tangents;
                nodeOne.Mesh.MeshPrimitives[0].Tangents = tangents;
                properties.Add(new Property(PropertyName.VertexTangent, ":white_check_mark:"));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, nodeZero, nodeOne) => {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetMatrixScaleX(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetMatrixScaleXY(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetMatrixScaleXYZ(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleX(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXY(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXYZ(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleX(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXY(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXYZ(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleX(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXY(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXYZ(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}



//using System.Collections.Generic;
//using System.Numerics;

//namespace AssetGenerator.ModelGroups
//{
//    [ModelGroupAttribute]
//    class Node_NegativeScale : ModelGroup
//    {
//        public Node_NegativeScale(List<string> imageList) : base(imageList)
//        {
//            modelGroupName = ModelGroupName.Node_NegativeScale;
//            onlyBinaryProperties = false;

//            Runtime.Image normalTexture = new Runtime.Image
//            {
//                Uri = imageList.Find(e => e.Contains("Normal_Nodes"))
//            };
//            Runtime.Image baseColorTexture = new Runtime.Image
//            {
//                Uri = imageList.Find(e => e.Contains("BaseColor_Nodes"))
//            };
//            Runtime.Image metallicRoughnessTexture = new Runtime.Image
//            {
//                Uri = imageList.Find(e => e.Contains("MetallicRoughness_Nodes"))
//            };
//            usedTextures.Add(normalTexture);
//            usedTextures.Add(baseColorTexture);
//            usedTextures.Add(metallicRoughnessTexture);

//            Runtime.GLTF defaultModel = Common.MultiNode(); // Only used to get the default tangent and normal values
//            List<Vector3> normals = new List<Vector3>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals);
//            List<Vector4> tangents = new List<Vector4>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents);

//            var matrixT = Matrix4x4.CreateTranslation(new Vector3(0, 2, 0));
//            var matrixNegScale_X = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, 1, 1)), matrixT);
//            var matrixNegScale_XY = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, -1, 1)), matrixT);
//            var matrixNegScale_XYZ = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, -1, -1)), matrixT);

//            requiredProperty = new List<Property>
//            {
//                new Property(Propertyname.Translation_Y, new Vector3(0, 2, 0), group: 1),
//                new Property(Propertyname.BaseColorTexture, baseColorTexture),
//                new Property(Propertyname.NormalTexture, normalTexture),
//                new Property(Propertyname.MetallicRoughnessTexture, metallicRoughnessTexture),
//            };
//            properties = new List<Property>
//            {
//                new Property(Propertyname.Matrix_X, matrixNegScale_X, group: 1),
//                new Property(Propertyname.Matrix_XY, matrixNegScale_XY, group: 1),
//                new Property(Propertyname.Matrix_XYZ, matrixNegScale_XYZ, group: 1),
//                new Property(Propertyname.Scale_X, new Vector3(-1, 1, 1), group: 2),
//                new Property(Propertyname.Scale_XY, new Vector3(-1, -1, 1), group: 2),
//                new Property(Propertyname.Scale_XYZ, new Vector3(-1, -1, -1), group: 2),
//                new Property(Propertyname.VertexNormal, normals),
//                new Property(Propertyname.VertexTangent, tangents),
//            };

//            var scale_X = properties.Find(e => e.name == Propertyname.Scale_X);
//            var scale_XY = properties.Find(e => e.name == Propertyname.Scale_XY);
//            var scale_XYZ = properties.Find(e => e.name == Propertyname.Scale_XYZ);
//            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
//            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);

//            Makes combos with both normals, and normals with tangents for each of the following scale values

//           var scaleSets = new List<Property>()
//           {
//                scale_X,
//                scale_XY,
//                scale_XYZ,
//           };
//            foreach (var scale in scaleSets)
//            {
//                specialCombos.Add(new List<Property>()
//                {
//                    scale,
//                    normal,
//                });
//                specialCombos.Add(new List<Property>()
//                {
//                    scale,
//                    normal,
//                    tangent,
//                });
//            }

//            removeCombos.Add(new List<Property>()
//            {
//                normal,
//            });
//            removeCombos.Add(new List<Property>()
//            {
//                tangent,
//            });
//        }

//        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
//        {
//            combos.RemoveAt(1); // Remove the full set combo

//            Sort the combos by complexity
//            combos.Sort(delegate (List<Property> x, List<Property> y)
//            {
//                if (x.Count == 0) return -1; // Empty Set
//                else if (y.Count == 0) return 1; // Empty Set
//                else if (x.Count > y.Count) return 1;
//                else if (x.Count < y.Count) return -1;
//                else if (x.Count == y.Count)
//                {
//                    Tie goes to the combo with the left-most property on the table
//                    for (int p = 0; p < x.Count; p++)
//                    {
//                        if (x[p].propertyGroup != y[p].propertyGroup ||
//                            x[p].propertyGroup == 0)
//                        {
//                            int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
//                            int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
//                            if (xPropertyIndex > yPropertyIndex) return 1;
//                            else if (xPropertyIndex < yPropertyIndex) return -1;
//                        }
//                    }
//                    for (int p = 0; p < x.Count; p++)
//                    {
//                        int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
//                        int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
//                        if (xPropertyIndex > yPropertyIndex) return 1;
//                        else if (xPropertyIndex < yPropertyIndex) return -1;
//                    }
//                    return 0;
//                }
//                else return 0;
//            });

//            return combos;
//        }

//        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
//        {
//            Switch from the flat plane to a model with multiple nodes
//           wrapper = Common.MultiNode();
//            var nodeList = new List<Runtime.Node>();
//            nodeList.Add(wrapper.Scenes[0].Nodes[0]);
//            nodeList.Add(wrapper.Scenes[0].Nodes[0].Children[0]);

//            Clear the vertex normal and tangent values already in the model
//            foreach (var node in nodeList)
//            {
//                node.Mesh.MeshPrimitives[0].Normals = null;
//                node.Mesh.MeshPrimitives[0].Tangents = null;
//            }

//            foreach (Property req in requiredProperty)
//            {
//                if (req.name == Propertyname.NormalTexture)
//                {
//                    material.NormalTexture = new Runtime.Texture();
//                    material.NormalTexture.Source = req.value;
//                }
//                else if (req.name == Propertyname.BaseColorTexture)
//                {
//                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
//                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
//                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
//                }
//                else if (req.name == Propertyname.MetallicRoughnessTexture)
//                {
//                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
//                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = req.value;
//                }
//                else if (req.name == Propertyname.Translation_Y)
//                {
//                    if (combo.Find(e => e.name == Propertyname.Matrix_X) == null &&
//                        combo.Find(e => e.name == Propertyname.Matrix_XY) == null &&
//                        combo.Find(e => e.name == Propertyname.Matrix_XYZ) == null) // The matrixs have their own translation
//                    {
//                        nodeList[1].Translation = req.value;
//                    }
//                }
//            }

//            foreach (Property property in combo)
//            {
//                foreach (var node in nodeList)
//                {
//                    if (property.name == Propertyname.VertexNormal)
//                    {
//                        node.Mesh.MeshPrimitives[0].Normals = property.value;
//                    }
//                    else if (property.name == Propertyname.VertexTangent)
//                    {
//                        node.Mesh.MeshPrimitives[0].Tangents = property.value;
//                    }
//                }

//                if (property.name == Propertyname.Matrix_X ||
//                    property.name == Propertyname.Matrix_XY ||
//                    property.name == Propertyname.Matrix_XYZ)
//                {
//                    nodeList[1].Matrix = property.value;
//                }
//                else if (property.name == Propertyname.Scale_X ||
//                        property.name == Propertyname.Scale_XY ||
//                        property.name == Propertyname.Scale_XYZ)
//                {
//                    nodeList[1].Scale = property.value;
//                }
//            }

//            Apply the material to each node
//            foreach (var node in nodeList)
//            {
//                node.Mesh.MeshPrimitives[0].Material = material;
//            }

//            return wrapper;
//        }
//    }
//}
