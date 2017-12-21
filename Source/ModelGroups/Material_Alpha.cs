using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_Alpha : ModelGroup
    { 
        public Material_Alpha()
        {
            modelGroupName = ModelGroupName.Material_Alpha;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(baseColorTexture);
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.3f, 0.3f, 0.3f, 0.4f),
                new Vector4( 0.3f, 0.3f, 0.3f, 0.2f),
                new Vector4( 0.3f, 0.3f, 0.3f, 0.8f),
                new Vector4( 0.3f, 0.3f, 0.3f, 0.6f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                new Property(Propertyname.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                new Property(Propertyname.AlphaCutoff_Low, 0.6f,  group:3),
                new Property(Propertyname.AlphaCutoff_High, 0.8f,  group:3),
                new Property(Propertyname.AlphaCutoff_Equal, 0.7f,  group:3),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:2),
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 1.0f, 1.0f, 0.7f)),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            var mask = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var blend = properties.Find(e => e.name == Propertyname.AlphaMode_Blend);
            var alphaCutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var alphaCutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var alphaCutoffEqual = properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffLow,
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffEqual
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffHigh
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffLow,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffEqual,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffHigh,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                blend,
                color,
                baseColorFactor,
                colorTex,
            });
            removeCombos.Add(new List<Property>()
            {
                 alphaCutoffLow
            });
            removeCombos.Add(new List<Property>()
            {
                 alphaCutoffEqual
            });
            removeCombos.Add(new List<Property>()
            {
                 alphaCutoffHigh
            });
            removeCombos.Add(new List<Property>()
            {
                 baseColorFactor
            });
            removeCombos.Add(new List<Property>()
            {
                 color
            });
            removeCombos.Add(new List<Property>()
            {
                 colorTex
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            var blend = properties.Find(e => e.name == Propertyname.AlphaMode_Blend);
            var mask = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var alphaCutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var alphaCutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var alphaCutoffEqual = properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);

            // BaseColorTexture is used everywhere except in the empty set
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo, or vertexcolor
                if ((y.Find(e => e.name == baseColorTexture.name)) == null &&
                    (y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector4_Float.ToString()))) == null)
                {
                    // Skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            // Add AlphaMode_Blend/Alphamode_Mask + VertexColor combos to the bottom, so BaseColorTexture isn't split up
            combos.Add(new List<Property>()
            {
                blend,
                color
            });
            combos.Add(new List<Property>()
            {
                mask,
                color
            });

            // Add more combos last, so they don't have a base color texture
            combos.Add(new List<Property>()
            {
                blend,
                baseColorFactor
            });
            combos.Add(new List<Property>()
            {
                mask,
                baseColorFactor
            });
            combos.Add(new List<Property>()
            {
                mask,
                alphaCutoffLow,
                baseColorFactor,
            });
            combos.Add(new List<Property>()
            {
                mask,
                alphaCutoffEqual,
                baseColorFactor
            });
            combos.Add(new List<Property>()
            {
                mask,
                alphaCutoffHigh,
                baseColorFactor,
            });

            //// Sort the combos by complexity
            //combos.Sort(delegate (List<Property> x, List<Property> y)
            //{
            //    if (x.Count == 0) return -1; // Empty Set
            //    else if (y.Count == 0) return 1; // Empty Set
            //    else if (x.Count == 5) return -1; // Full set
            //    else if (x[0].propertyGroup == 1 &&
            //             x[0].name != y[0].name)// Sort by Mask/Blend
            //    {
            //        int xPropertyIndex = properties.FindIndex(e => e.name == x[0].name);
            //        int yPropertyIndex = properties.FindIndex(e => e.name == y[0].name);
            //        if (xPropertyIndex > yPropertyIndex) return 1;
            //        else if (xPropertyIndex < yPropertyIndex) return -1;
            //        else return 0;
            //    }
            //    else if (x.Count > y.Count) return 1;
            //    else if (x.Count < y.Count) return -1;
            //    else if (x.Count == y.Count)
            //    {
            //        // Tie goes to the combo with the left-most property on the table
            //        for (int p = 0; p < x.Count; p++)
            //        {
            //            if (x[p].propertyGroup != y[p].propertyGroup ||
            //                x[p].propertyGroup == 0)
            //            {
            //                int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
            //                int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
            //                if (xPropertyIndex > yPropertyIndex) return 1;
            //                else if (xPropertyIndex < yPropertyIndex) return -1;
            //            }
            //        }
            //        for (int p = 0; p < x.Count; p++)
            //        {
            //            int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
            //            int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
            //            if (xPropertyIndex > yPropertyIndex) return 1;
            //            else if (xPropertyIndex < yPropertyIndex) return -1;
            //        }
            //        return 0;
            //    }
            //    else return 0;
            //});

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            foreach (Property property in combo)
            {
                if (property.propertyGroup == 1) // Alpha Mode
                {
                    material.AlphaMode = property.value;
                }
                else if (property.propertyGroup == 3) // Alpha Cutoff
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
