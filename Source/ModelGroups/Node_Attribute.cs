using System;
using System.Numerics;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Node_Attribute : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Node_Attribute;

        public Node_Attribute(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.Node> setProperties)
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
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                        {
                            BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                        },
                    };
                    node.Mesh.MeshPrimitives[0].Normals = null;
                    node.Mesh.MeshPrimitives[0].Tangents = null;
                }

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes[0]);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => gltf.Scenes[0]),
                };
            }

            void SetTranslation(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(-2, 2, -2);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetTranslationX(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(-2, 0, 0);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetTranslationY(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(0, 2, 0);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetTranslationZ(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(0, 0, -2);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetRotation(List<Property> properties, Runtime.Node node)
            {
                var rotation = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)Math.PI);
                rotation.W = (float)Math.Round(rotation.W);
                node.Rotation = rotation;
                properties.Add(new Property(PropertyName.Rotation, rotation));
            }

            void SetScale(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(1.2f, 1.2f, 1.2f);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetMatrix(List<Property> properties, Runtime.Node node)
            {
                var matrixT = Matrix4x4.CreateTranslation(new Vector3(-2, 2, -2));
                var matrixR = Matrix4x4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)(Math.PI));
                var matrixS = Matrix4x4.CreateScale(1.2f);
                var matrixTRS = Matrix4x4.Multiply(Matrix4x4.Multiply(matrixS, matrixR), matrixT);
                node.Matrix = matrixTRS;
                properties.Add(new Property(PropertyName.Matrix, matrixTRS));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, node) => {

                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslationX(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslationY(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslationZ(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetRotation(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                    SetRotation(properties, node);
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetMatrix(properties, node);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}


//using System.Collections.Generic;
//using System.Numerics;
//using System;

//namespace AssetGenerator.ModelGroups
//{
//    [ModelGroupAttribute]
//    class Node_Attribute : ModelGroup
//    {
//        public Node_Attribute(List<string> imageList) : base(imageList)
//        {
//            modelGroupName = ModelGroupName.Node_Attribute;
//            onlyBinaryProperties = false;

//            Runtime.Image baseColorTexture = new Runtime.Image
//            {
//                Uri = imageList.Find(e => e.Contains("BaseColor_Nodes"))
//            };
//            usedTextures.Add(baseColorTexture);

//            var matrixT = Matrix4x4.CreateTranslation(new Vector3(-2, 2, -2));
//            var matrixR = Matrix4x4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)(Math.PI));
//            var matrixS = Matrix4x4.CreateScale(1.2f);
//            var matrixTRS = Matrix4x4.Multiply(Matrix4x4.Multiply(matrixS, matrixR), matrixT);
//            var rotation = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)Math.PI);
//            rotation.W = (float)Math.Round(rotation.W);

//            requiredProperty = new List<Property>
//            {
//                new Property(Propertyname.BaseColorTexture, baseColorTexture),
//            };
//            properties = new List<Property>
//            {
//                new Property(Propertyname.Translation, new Vector3(-2, 2, -2), group: 1),
//                new Property(Propertyname.Translation_X, new Vector3(-2, 0, 0), group: 1),
//                new Property(Propertyname.Translation_Y, new Vector3(0, 2, 0), group: 1),
//                new Property(Propertyname.Translation_Z, new Vector3(0, 0, -2), group: 1),
//                new Property(Propertyname.Rotation, rotation),
//                new Property(Propertyname.Scale, new Vector3(1.2f, 1.2f, 1.2f), group: 2),
//                new Property(Propertyname.Matrix, matrixTRS),
//            };
//            specialProperties = new List<Property>
//            {
//                new Property(Propertyname.Matrix, matrixTRS),
//                new Property(Propertyname.Rotation, rotation),
//            };

//            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
//            var translation = properties.Find(e => e.name == Propertyname.Translation);
//            var rot = properties.Find(e => e.name == Propertyname.Rotation);
//            var scale = properties.Find(e => e.name == Propertyname.Scale);
//            specialCombos.Add(new List<Property>()
//            {
//                translation,
//                rot,
//                scale,
//            });
//        }

//        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
//        {
//            combos.RemoveAt(1); // Remove the full set combo

//            // Sort the combos by complexity
//            combos.Sort(delegate (List<Property> x, List<Property> y)
//            {
//                if (x.Count == 0) return -1; // Empty Set
//                else if (y.Count == 0) return 1; // Empty Set
//                else if (x.Find(e => e.name == Propertyname.Matrix) != null) return 1; // Matrix
//                else if (y.Find(e => e.name == Propertyname.Matrix) != null) return -1; // Matrix
//                else if (x.Count > y.Count) return 1;
//                else if (x.Count < y.Count) return -1;
//                else if (x.Count == y.Count)
//                {
//                    // Tie goes to the combo with the left-most property on the table
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
//            // Switch from the flat plane to a model with multiple nodes
//            wrapper = Common.MultiNode();
//            var nodeList = new List<Runtime.Node>();
//            nodeList.Add(wrapper.Scenes[0].Nodes[0]);
//            nodeList.Add(wrapper.Scenes[0].Nodes[0].Children[0]);

//            // Add a new child node that will inherit the transformations
//            //nodeList.Add((DeepCopy.CloneObject(wrapper.Scenes[0].Nodes[0])));
//            //nodeList[2].Name = "Node1";
//            //nodeList[2].Children = null;
//            //nodeList[1].Children = new List<Runtime.Node>();
//            //nodeList[1].Children.Add(nodeList[2]);

//            // Clear the vertex normal and tangent values already in the model
//            foreach (var node in nodeList)
//            {
//                node.Mesh.MeshPrimitives[0].Normals = null;
//                node.Mesh.MeshPrimitives[0].Tangents = null;
//            }

//            // Texture the model
//            foreach (Property req in requiredProperty)
//            {
//                if (req.name == Propertyname.BaseColorTexture)
//                {
//                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
//                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
//                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
//                }
//            }

//            foreach (Property property in combo)
//            {
//                if (property.name == Propertyname.Matrix)
//                {
//                    nodeList[1].Matrix = specialProperties[0].value;
//                }
//                else if (property.name == Propertyname.Translation ||
//                         property.name == Propertyname.Translation_X ||
//                         property.name == Propertyname.Translation_Y ||
//                         property.name == Propertyname.Translation_Z)
//                {
//                    nodeList[1].Translation = property.value;
//                }
//                else if (property.name == Propertyname.Rotation)
//                {
//                    nodeList[1].Rotation = specialProperties[1].value;
//                }
//                else if (property.name == Propertyname.Scale)
//                {
//                    nodeList[1].Scale = property.value;
//                }
//            }

//            // Apply the material to each node
//            foreach (var node in nodeList)
//            {
//                node.Mesh.MeshPrimitives[0].Material = material;
//            }

//            return wrapper;
//        }
//    }
//}
