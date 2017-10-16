using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Material_MetallicRoughness : Test
    {
        public Material_MetallicRoughness()
        {
            testType = TestName.Material_MetallicRoughness;
            onlyBinaryProperties = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image occlusionRoughnessMetallicTexture = new Runtime.Image
            {
                Uri = texture_OcclusionRoughnessMetallic
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(occlusionRoughnessMetallicTexture);
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.BaseColorFactor, new Vector4(0.2f, 0.2f, 0.2f, 0.8f)),
                new Property(Propertyname.VertexColor_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector4_Short, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Short, colorCoord, group:3),
                new Property(Propertyname.MetallicFactor, 0.0f),
                new Property(Propertyname.RoughnessFactor, 0.0f),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.MetallicRoughnessTexture, occlusionRoughnessMetallicTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector4_Short, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Short, colorCoord, group:3),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorFactor),
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.MetallicFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.RoughnessFactor)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Test the VertexColor in combo with BaseColorTexture
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            string vertexColorName = LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector3_Float.ToString());
            string baseColorTextureName = LogStringHelper.GenerateNameWithSpaces(Propertyname.BaseColorTexture.ToString());
            foreach (var y in combos)
            {
                // Checks if combos contain the vertexcolor property
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == vertexColorName)) != null)
                {
                    // Makes sure that BaseColorTexture isn't already in that combo
                    if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == baseColorTextureName)) == null)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            // Test the VertexColor in combo with BaseColorFactor
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            string baseColorFactorName = LogStringHelper.GenerateNameWithSpaces(Propertyname.BaseColorFactor.ToString());
            foreach (var y in combos)
            {
                // Checks if combos contain the vertexcolor property
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == vertexColorName)) != null)
                {
                    // Makes sure that BaseColorTexture isn't already in that combo
                    if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == baseColorFactorName)) == null)
                    {
                        y.Add(baseColorFactor);
                    }
                }
            }

            // Inserts the solo BaseColorTexture model next to the other models that use the texture
            combos.Insert(3, ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            foreach (Property property in combo)
            {
                if (material.MetallicRoughnessMaterial == null)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                }

                switch (property.name)
                {
                    case Propertyname.BaseColorFactor:
                        {
                            material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            break;
                        }
                    case Propertyname.MetallicFactor:
                        {
                            material.MetallicRoughnessMaterial.MetallicFactor = property.value;
                            break;
                        }
                    case Propertyname.RoughnessFactor:
                        {
                            material.MetallicRoughnessMaterial.RoughnessFactor = property.value;
                            break;
                        }
                    case Propertyname.BaseColorTexture:
                        {
                            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                            material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.MetallicRoughnessTexture:
                        {
                            material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                            material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = property.value;
                            break;
                        }
                }

                if (property.name == Propertyname.VertexColor_Vector3_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector3_Byte)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Byte)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector3_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Short)
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
