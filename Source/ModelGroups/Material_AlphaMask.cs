using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_AlphaMask : ModelGroup
    { 
        public Material_AlphaMask()
        {
            modelGroupName = ModelGroupName.Material_AlphaMask;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(baseColorTexture);
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.AlphaCutoff_Low, 0.4f,  group:3),
                new Property(Propertyname.AlphaCutoff_High, 0.7f,  group:3),
                new Property(Propertyname.AlphaCutoff_Multiplied, 0.5f,  group:3),
                new Property(Propertyname.AlphaCutoff_All, 1.1f,  group:3),
                new Property(Propertyname.AlphaCutoff_None, 0f,  group:3),
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 1.0f, 1.0f, 0.7f)),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            var cutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var cutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var cutoffMultiplied = properties.Find(e => e.name == Propertyname.AlphaCutoff_Multiplied);
            var cutoffAll = properties.Find(e => e.name == Propertyname.AlphaCutoff_All);
            var cutoffNone = properties.Find(e => e.name == Propertyname.AlphaCutoff_None);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            specialCombos.Add(new List<Property>()
            {
                cutoffMultiplied,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                cutoffHigh,
                baseColorFactor
            });
            removeCombos.Add(new List<Property>()
            {
                baseColorFactor
            });
            removeCombos.Add(new List<Property>()
            {
                cutoffMultiplied
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);

            // BaseColorTexture is used everywhere except in the empty set
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo
                if ((y.Find(e => e.name == baseColorTexture.name)) == null)
                {
                    // Skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            //// Sort the combos by complexity
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
            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.AlphaMode_Mask)
                {
                    material.AlphaMode = req.value;
                }
            }

            foreach (Property property in combo)
            {
                if (property.propertyGroup == 3) // Alpha Cutoff
                {
                    material.AlphaCutoff = property.value;
                }
                else if (property.name == Propertyname.BaseColorFactor)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }
                    material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                }
                else if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Float)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
