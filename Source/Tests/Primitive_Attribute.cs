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
            Runtime.Image OcclusionRoughnessMetallicTexture = new Runtime.Image
            {
                Uri = texture_OcclusionRoughnessMetallic
            };
            usedImages.Add(normalTexture);
            usedImages.Add(baseColorTexture);
            usedImages.Add(OcclusionRoughnessMetallicTexture);
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f)
            };
            List<Vector2> uvCoord2 = new List<Vector2>()
            {
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f)
            };
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f)
            };
            List<Vector4> tanCoord = new List<Vector4>()
            {
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f)
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.MetallicRoughnessTexture, OcclusionRoughnessMetallicTexture),
                new Property(Propertyname.TexCoord, uvCoord2),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Normal, planeNormals),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.Tangent, tanCoord),
                new Property(Propertyname.TexCoord0_Float, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, group:1),
                new Property(Propertyname.TexCoord0_Byte, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, group:1),
                new Property(Propertyname.TexCoord0_Short, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, group:1),
                new Property(Propertyname.TexCoord1_Float, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, Propertyname.TexCoord0_Float, 2),
                new Property(Propertyname.TexCoord1_Byte, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, Propertyname.TexCoord0_Byte, 2),
                new Property(Propertyname.TexCoord1_Short, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, Propertyname.TexCoord0_Short, 2),
                new Property(Propertyname.Color_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Short, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Short, colorCoord, group:3),
            };
            addToCombos.Add(
                properties.Find(e => e.name == Propertyname.TexCoord0_Float));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Normal),
                properties.Find(e => e.name == Propertyname.NormalTexture),
                properties.Find(e => e.name == Propertyname.Tangent)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Normal),
                properties.Find(e => e.name == Propertyname.NormalTexture)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.TexCoord0_Byte),
                properties.Find(e => e.name == Propertyname.TexCoord1_Byte),
                properties.Find(e => e.name == Propertyname.Color_Vector4_Byte)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.TexCoord0_Byte),
                properties.Find(e => e.name == Propertyname.TexCoord1_Byte),
                properties.Find(e => e.name == Propertyname.Color_Vector3_Byte)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.TexCoord0_Short),
                properties.Find(e => e.name == Propertyname.TexCoord1_Short),
                properties.Find(e => e.name == Propertyname.Color_Vector4_Short)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.TexCoord0_Short),
                properties.Find(e => e.name == Propertyname.TexCoord1_Short),
                properties.Find(e => e.name == Propertyname.Color_Vector3_Short)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Tangent)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.NormalTexture)));
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            // Clear values from the default model, so we can test those values not being set
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Normals = null;

            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
            material.NormalTexture = new Runtime.Texture();

            // BaseColor is set for every model, while the normal texture and second UV cord is only
            // used when a second UV is being set.
            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 0;
                }
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Normal)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Normals = property.value;
                }
                else if (property.name == Propertyname.Tangent)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Tangents = property.value;
                }
                else if (property.name == Propertyname.TexCoord0_Float ||
                         property.name == Propertyname.TexCoord0_Byte ||
                         property.name == Propertyname.TexCoord0_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType = property.value;
                }
                else if (property.name == Propertyname.TexCoord1_Float ||
                         property.name == Propertyname.TexCoord1_Byte ||
                         property.name == Propertyname.TexCoord1_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType = property.value;

                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(
                        requiredProperty.Find(e => e.name == Propertyname.TexCoord).value);

                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = 
                        requiredProperty.Find(e => e.name == Propertyname.MetallicRoughnessTexture).value;
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.TexCoordIndex = 1;
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
