using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_Attribute : ModelGroup
    {
        public Node_Attribute()
        {
            modelGroupName = ModelGroupName.Node_Attribute;
            onlyBinaryProperties = false;

            var matrixT = Matrix4x4.CreateTranslation(new Vector3(3, 3, 3));
            var matrixR = Matrix4x4.CreateFromYawPitchRoll(0.6f, 0.6f, 0.6f);
            var matrixS = Matrix4x4.CreateScale(2);
            var matrixTRS = Matrix4x4.Multiply(Matrix4x4.Multiply(matrixT, matrixR), matrixS);
            var rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3(0.6f, 0.6f, 0.6f)), 42));

            properties = new List<Property>
            {
                new Property(Propertyname.Matrix, matrixTRS),
                new Property(Propertyname.Translation, new Vector3(3, 3, 3), group: 1),
                new Property(Propertyname.Translation_X, new Vector3(3, 0, 0), group: 1),
                new Property(Propertyname.Translation_Y, new Vector3(0, 3, 0), group: 1),
                new Property(Propertyname.Translation_Z, new Vector3(0, 0, 3), group: 1),
                new Property(Propertyname.Rotation, rotation),
                new Property(Propertyname.Scale, new Vector3(2, 2, 2), group: 2),
                new Property(Propertyname.Scale_X, new Vector3(2, 1, 1), group: 2),
                new Property(Propertyname.Scale_Y, new Vector3(1, 2, 1), group: 2),
                new Property(Propertyname.Scale_Z, new Vector3(1, 1, 2), group: 2),
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
                else if (property.name == Propertyname.Scale ||
                         property.name == Propertyname.Scale_X ||
                         property.name == Propertyname.Scale_Y ||
                         property.name == Propertyname.Scale_Z)
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
