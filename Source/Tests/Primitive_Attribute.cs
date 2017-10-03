using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute(TestNames.Primitive_Attribute),
        ImageAttribute(texture_BaseColor),
        ImageAttribute(texture_Normal)]
    class Primitive_Attribute : TestValues
    {
        public Primitive_Attribute()
        {
            testType = TestNames.Primitive_Attribute;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.NormalTexture, normalTexture)
            };
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f)
            };
            List<Vector2> uvCoord1 = new List<Vector2>()
            {
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 0.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 0.5f, 0.0f)
            };
            List<Vector2> uvCoord2 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 0.5f, 1.0f),
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 0.0f),
                new Vector2( 0.0f, 0.0f)
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
            properties = new List<Property>
            {
                new Property(Propertyname.Normal, planeNormals),
                new Property(Propertyname.Tangent, tanCoord),
                new Property(Propertyname.TexCoord0_Float, uvCoord1, group:1),
                new Property(Propertyname.TexCoord0_Byte, uvCoord1, group:1),
                new Property(Propertyname.TexCoord0_Short, uvCoord1, group:1),
                new Property(Propertyname.TexCoord1_Float, uvCoord2, Propertyname.TexCoord0_Float, 2),
                new Property(Propertyname.TexCoord1_Byte, uvCoord2, Propertyname.TexCoord0_Byte, 2),
                new Property(Propertyname.TexCoord1_Short, uvCoord2, Propertyname.TexCoord0_Short, 2),
                new Property(Propertyname.Color_Vector3_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector3_Short, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Byte, colorCoord, group:3),
                new Property(Propertyname.Color_Vector4_Short, colorCoord, group:3),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Normal),
                properties.Find(e => e.name == Propertyname.Tangent)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Tangent)));
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
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            // Clear values from the default model, so we can test those values not being set
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Normals = null;

            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
            material.NormalTexture = new Runtime.Texture();

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
                else if (property.name == Propertyname.TexCoord0_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                        Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets[0] = property.value;
                }
                else if (property.name == Propertyname.TexCoord0_Byte)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                        Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets[0] = property.value;
                }
                else if (property.name == Propertyname.TexCoord0_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                        Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets[0] = property.value;
                }
                else if (property.name == Propertyname.TexCoord1_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                        Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(property.value);

                    var NormText = requiredProperty.Find(e => e.name == Propertyname.NormalTexture);
                    material.NormalTexture.Source = NormText.value;
                    material.NormalTexture.TexCoordIndex = 1;
                }
                else if (property.name == Propertyname.TexCoord1_Byte)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                        Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(property.value);

                    var NormText = requiredProperty.Find(e => e.name == Propertyname.NormalTexture);
                    material.NormalTexture.Source = NormText.value;
                    material.NormalTexture.TexCoordIndex = 1;
                }
                else if (property.name == Propertyname.TexCoord1_Short)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                        Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(property.value);

                    var NormText = requiredProperty.Find(e => e.name == Propertyname.NormalTexture);
                    material.NormalTexture.Source = NormText.value;
                    material.NormalTexture.TexCoordIndex = 1;
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
