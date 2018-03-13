using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Mesh_Primitives : ModelGroup
    {
        public Mesh_Primitives(List<string> textures, List<string> figures) : base(textures, figures)
        {
            modelGroupName = ModelGroupName.Mesh_Primitives;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image figureIndicesPrimitive0 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("Indices_Primitive0"))
            };
            Runtime.Image figureIndicesPrimitive1 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("Indices_Primitive1"))
            };
            usedFigures.Add(figureIndicesPrimitive0);
            usedFigures.Add(figureIndicesPrimitive1);

            List<Vector3> primitive0Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f)
            };
            List<Vector3> primitive1Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };
            List<int> primitiveTriangleIndices = new List<int>
            {
                0, 1, 2,
            };
            Runtime.MeshPrimitive primitive0Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive0Positions,
                Indices = primitiveTriangleIndices,
            };
            Runtime.MeshPrimitive primitive1Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive1Positions,
                Indices = primitiveTriangleIndices,
            };
            Vector4 colors0 = new Vector4(0.2f, 0.8f, 0.2f, 0.8f);
            Vector4 colors1 = new Vector4(0.2f, 0.2f, 0.8f, 0.8f);

            properties = new List<Property>
            {
                new Property(Propertyname.Primitive0Material0BaseColorFactor, colors0),
                new Property(Propertyname.Primitive1Material0BaseColorFactor, colors0),
                new Property(Propertyname.Primitive0Material1BaseColorFactor, colors1),
                new Property(Propertyname.Primitive1Material1BaseColorFactor, colors1),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split1, primitive0Mesh, group: 1),
                new Property(Propertyname.Primitives_Split2, primitive1Mesh, group: 1),
            };

            var pri0mat0 = properties.Find(e => e.name == Propertyname.Primitive0Material0BaseColorFactor);
            var pri0mat1 = properties.Find(e => e.name == Propertyname.Primitive0Material1BaseColorFactor);
            var pri1mat0 = properties.Find(e => e.name == Propertyname.Primitive1Material0BaseColorFactor);
            var pri1mat1 = properties.Find(e => e.name == Propertyname.Primitive1Material1BaseColorFactor);
            specialCombos.Add(new List<Property>()
            {
                pri0mat0,
                pri1mat0
            });
            specialCombos.Add(new List<Property>()
            {
                pri0mat1,
                pri1mat1
            });
            specialCombos.Add(new List<Property>()
            {
                pri0mat1,
                pri1mat0
            });
            specialCombos.Add(new List<Property>()
            {
                pri0mat0,
                pri1mat1
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            combos.RemoveAt(1); // Removes the full set

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

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material0, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Same plane, but split into two triangle primitives
            var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split1);
            var primitive2 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split2);
            Runtime.MeshPrimitive prim0 = new Runtime.MeshPrimitive
            {
                Positions = primitive1.value.Positions,
                Indices = primitive1.value.Indices,
            };
            Runtime.MeshPrimitive prim2 = new Runtime.MeshPrimitive
            {
                Positions = primitive2.value.Positions,
                Indices = primitive2.value.Indices,
            };
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
            {
                prim0,
                prim2
            };

            // Make a second material
            var material1 = DeepCopy.CloneObject(material0);

            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.Primitive0Material0BaseColorFactor:
                        {
                            material0.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                            material0.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material0;
                            break;
                        }
                    case Propertyname.Primitive0Material1BaseColorFactor:
                        {
                            material1.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                            material1.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material1;
                            break;
                        }
                    case Propertyname.Primitive1Material0BaseColorFactor:
                        {
                            material0.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                            material0.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = material0;
                            break;
                        }
                    case Propertyname.Primitive1Material1BaseColorFactor:
                        {
                            material1.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                            material1.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = material1;
                            break;
                        }
                }
            }

            return wrapper;
        }
    }
}
