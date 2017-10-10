using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Material_Alpha : Test
    { 
        public Material_Alpha()
        {
            testType = TestName.Material_Alpha;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(baseColorTexture);
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                new Property(Propertyname.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                new Property(Propertyname.AlphaCutoff, 0.2f),
                new Property(Propertyname.DoubleSided, true),
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 1.0f, 1.0f, 0.6f)),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.Color_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Short, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Short, colorCoord, group:3),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.Color_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Short, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Short, colorCoord, group:3),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.AlphaCutoff)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.DoubleSided),
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.DoubleSided),
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.BaseColorFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture),
                properties.Find(e => e.name == Propertyname.BaseColorFactor),
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend),
                properties.Find(e => e.name == Propertyname.Color_Vector4_Float)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaCutoff)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorFactor)));
            foreach (var y in specialProperties)
            {
                removeCombos.Add(ComboHelper.CustomComboCreation(y));
            }
            // Test color against blend. Don't apply a texture or factor for this.
            var blend = properties.Find(e => e.name == Propertyname.AlphaMode_Blend);
            for (int y = 1; y <= 6; y++)
            {
                specialCombos.Add(ComboHelper.CustomComboCreation(
                specialProperties[y],
                blend));
            }
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // BaseColorTexture is used everywhere except in the empty set
            var baseColorTexture = specialProperties.Find(e => e.name == Propertyname.BaseColorTexture);
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo
                if ((y.Find(e => e.name == baseColorTexture.name)) == null &&
                    (y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.Color_Vector3_Float.ToString()))) == null)
                {
                    // Skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.AlphaMode_Opaque ||
                    property.name == Propertyname.AlphaMode_Mask ||
                    property.name == Propertyname.AlphaMode_Blend)
                {
                    material.AlphaMode = property.value;
                }
                else if (property.name == Propertyname.AlphaCutoff)
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
                        material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                    }
                    material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                }
                else if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                }
                else if (property.name == Propertyname.Color_Vector3_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.Color_Vector4_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.Color_Vector3_Byte)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.Color_Vector4_Byte)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.Color_Vector3_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.Color_Vector4_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
