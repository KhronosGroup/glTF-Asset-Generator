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
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:2),
                new Property(Propertyname.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                new Property(Propertyname.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                new Property(Propertyname.AlphaCutoff_Low, 0.6f,  group:3),
                new Property(Propertyname.AlphaCutoff_High, 0.8f,  group:3),
                new Property(Propertyname.AlphaCutoff_Equal, 0.7f,  group:3),
                new Property(Propertyname.DoubleSided, true),
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 1.0f, 1.0f, 0.7f)),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:2),
            };
            var mask = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var alphaCutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var alphaCutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var alphaCutoffEqual = properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.AlphaCutoff_Low)));
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
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.DoubleSided),
                properties.Find(e => e.name == Propertyname.AlphaCutoff_Low),
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.DoubleSided),
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.BaseColorFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorFactor),
                properties.Find(e => e.name == Propertyname.BaseColorTexture),
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend),
                properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaCutoff_Low)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaCutoff_High)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorFactor)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            var blend = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var mask = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var alphaCutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var alphaCutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var alphaCutoffEqual = properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);

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
            combos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend),
                properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float)));
            combos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float)));

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

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.AlphaMode_Opaque ||
                    property.name == Propertyname.AlphaMode_Mask ||
                    property.name == Propertyname.AlphaMode_Blend)
                {
                    material.AlphaMode = property.value;
                }
                else if (property.propertyGroup == 3) // Alpha Cutoff
                {
                    material.AlphaCutoff = property.value;
                }
                else if (property.name == Propertyname.DoubleSided)
                {
                    material.DoubleSided = property.value;
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
