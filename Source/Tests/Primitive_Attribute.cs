using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Primitive_Attribute : Test
    {
        public Primitive_Attribute()
        {
            testType = TestName.Primitive_Attribute;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image uvIcon0 = new Runtime.Image
            {
                Uri = icon_UVspace0
            };
            Runtime.Image uvIcon1 = new Runtime.Image
            {
                Uri = icon_UVspace1
            };
            usedImages.Add(normalTexture);
            usedImages.Add(baseColorTexture);
            usedImages.Add(uvIcon0);
            usedImages.Add(uvIcon1);
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f)
            };
            List<Vector2> uvCoord2 = new List<Vector2>()
            {
                new Vector2(0.5f, 0.5f),
                new Vector2(1.0f, 0.5f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.5f, 0.0f)
            };
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f),
                new Vector4( 1.0f, 1.0f, 0.0f, 0.2f)
            };
            List<Vector4> tanCoord = new List<Vector4>()
            {
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexNormal, planeNormals),
                new Property(Propertyname.VertexTangent, tanCoord),
                new Property(Propertyname.VertexUV0_Float, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, group:1),
                new Property(Propertyname.VertexUV0_Byte, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, group:1),
                new Property(Propertyname.VertexUV0_Short, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, group:1),
                new Property(Propertyname.VertexUV1_Float, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, Propertyname.VertexUV0_Float, 2),
                new Property(Propertyname.VertexUV1_Byte, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, Propertyname.VertexUV0_Byte, 2),
                new Property(Propertyname.VertexUV1_Short, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, Propertyname.VertexUV0_Short, 2),
                new Property(Propertyname.VertexColor_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector4_Short, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.VertexColor_Vector3_Short, colorCoord, group:3),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.TexCoord, uvCoord2),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.VertexUV0_Float,
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, group:1)
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.VertexUV0_Float),
                properties.Find(e => e.name == Propertyname.NormalTexture),
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.VertexNormal),
                properties.Find(e => e.name == Propertyname.NormalTexture),
                properties.Find(e => e.name == Propertyname.VertexTangent)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.VertexNormal),
                properties.Find(e => e.name == Propertyname.NormalTexture)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.VertexTangent)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.NormalTexture)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // BaseColorTexture is used everywhere except in the empty set and with vertexcolor
            var baseColorTexture = specialProperties.Find(e => e.name == Propertyname.BaseColorTexture);
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo, or vertexcolor
                if ((y.Find(e => e.name == baseColorTexture.name)) == null &&
                    (y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector3_Float.ToString()))) == null)
                {
                    // Skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            // TextCoord0 is used everywhere a base color texture is used, so include it in everything except the empty set
            var vertexUV0 = specialProperties.Find(e => e.name == Propertyname.VertexUV0_Float);
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(vertexUV0.name.ToString()))) == null)
                {
                    // If there are already values in the combo, just add this new property
                    // Otherwise skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(vertexUV0);
                    }
                }
            }

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Remove the base model's UV0 on the empty set
            if (combo.Count < 0)
            {
                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.RemoveAt(0);
                material.MetallicRoughnessMaterial = null;
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                        material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 0;
                }
                else if (property.name == Propertyname.VertexNormal)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Normals = property.value;
                }
                else if (property.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = property.value;
                    material.NormalTexture.TexCoordIndex = 0;
                }
                else if (property.name == Propertyname.VertexTangent)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Tangents = property.value;
                }
                else if (property.name == Propertyname.VertexUV0_Float ||
                         property.name == Propertyname.VertexUV0_Byte ||
                         property.name == Propertyname.VertexUV0_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType = property.value;
                }
                else if (property.name == Propertyname.VertexUV1_Float ||
                         property.name == Propertyname.VertexUV1_Byte ||
                         property.name == Propertyname.VertexUV1_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType = property.value;

                    if (wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Count < 2)
                    {
                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.TexCoord).value);
                    }
                }
                else if (property.name == Propertyname.VertexColor_Vector3_Float)
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
            if (combo.Count > 0) // Don't set the material on the empty set
            {
                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;
            }
            // Use the second UV if it has been set
            if (wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Count > 1)
            {
                if (material.NormalTexture != null)
                {
                    material.NormalTexture.TexCoordIndex = 1;
                }
                if (material.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                }
            }

            return wrapper;
        }
    }
}
