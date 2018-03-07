using System.Collections.Generic;
using System.Numerics;
using System;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_Attribute : ModelGroup
    {
        public Node_Attribute(List<string> textures, List<string> figures) : base(textures, figures)
        {
            modelGroupName = ModelGroupName.Node_Attribute;
            onlyBinaryProperties = false;

            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("BaseColor_Nodes"))
            };
            usedTextures.Add(baseColorTexture);

            var matrixT = Matrix4x4.CreateTranslation(new Vector3(-2, 2, -2));
            var matrixR = Matrix4x4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)(Math.PI));
            var matrixS = Matrix4x4.CreateScale(1.2f);
            var matrixTRS = Matrix4x4.Multiply(Matrix4x4.Multiply(matrixS, matrixR), matrixT);
            var rotation = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)Math.PI);
            rotation.W = (float)Math.Round(rotation.W);

            requiredProperty = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Translation, new Vector3(-2, 2, -2), group: 1),
                new Property(Propertyname.Translation_X, new Vector3(-2, 0, 0), group: 1),
                new Property(Propertyname.Translation_Y, new Vector3(0, 2, 0), group: 1),
                new Property(Propertyname.Translation_Z, new Vector3(0, 0, -2), group: 1),
                new Property(Propertyname.Rotation, rotation),
                new Property(Propertyname.Scale, new Vector3(1.2f, 1.2f, 1.2f), group: 2),
                new Property(Propertyname.Matrix, matrixTRS),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Matrix, matrixTRS),
                new Property(Propertyname.Rotation, rotation),
            };

            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            var translation = properties.Find(e => e.name == Propertyname.Translation);
            var rot = properties.Find(e => e.name == Propertyname.Rotation);
            var scale = properties.Find(e => e.name == Propertyname.Scale);
            specialCombos.Add(new List<Property>()
            {
                translation,
                rot,
                scale,
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            combos.RemoveAt(1); // Remove the full set combo

            // Sort the combos by complexity
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                if (x.Count == 0) return -1; // Empty Set
                else if (y.Count == 0) return 1; // Empty Set
                else if (x.Find(e => e.name == Propertyname.Matrix) != null) return 1; // Matrix
                else if (y.Find(e => e.name == Propertyname.Matrix) != null) return -1; // Matrix
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

            // Add a new child node that will inherit the transformations
            //nodeList.Add((DeepCopy.CloneObject(wrapper.Scenes[0].Nodes[0])));
            //nodeList[2].Name = "Node1";
            //nodeList[2].Children = null;
            //nodeList[1].Children = new List<Runtime.Node>();
            //nodeList[1].Children.Add(nodeList[2]);

            // Clear the vertex normal and tangent values already in the model
            foreach (var node in nodeList)
            {
                node.Mesh.MeshPrimitives[0].Normals = null;
                node.Mesh.MeshPrimitives[0].Tangents = null;
            }

            // Texture the model
            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                }
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Matrix)
                {
                    nodeList[1].Matrix = specialProperties[0].value;
                }
                else if (property.name == Propertyname.Translation ||
                         property.name == Propertyname.Translation_X ||
                         property.name == Propertyname.Translation_Y ||
                         property.name == Propertyname.Translation_Z)
                {
                    nodeList[1].Translation = property.value;
                }
                else if (property.name == Propertyname.Rotation)
                {
                    nodeList[1].Rotation = specialProperties[1].value;
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
