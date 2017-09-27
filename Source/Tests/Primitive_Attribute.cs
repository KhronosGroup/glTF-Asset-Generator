using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    class Primitive_Attribute : TestValues
    {
        public Primitive_Attribute()
        {
            onlyBinaryAttributes = false;
            noPrerequisite = false;
            imageAttributes = new ImageAttribute[]
            {
                            new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            requiredAttributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.BaseColorTexture, image),
                            new Attribute(AttributeName.NormalTexture, image)
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
            attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.Normal, planeNormals),
                            new Attribute(AttributeName.Tangent, tanCoord),
                            new Attribute(AttributeName.TexCoord0_Float, uvCoord1, group:1),
                            new Attribute(AttributeName.TexCoord0_Byte, uvCoord1, group:1),
                            new Attribute(AttributeName.TexCoord0_Short, uvCoord1, group:1),
                            new Attribute(AttributeName.TexCoord1_Float, uvCoord2, AttributeName.TexCoord0_Float, 2),
                            new Attribute(AttributeName.TexCoord1_Byte, uvCoord2, AttributeName.TexCoord0_Byte, 2),
                            new Attribute(AttributeName.TexCoord1_Short, uvCoord2, AttributeName.TexCoord0_Short, 2),
                            new Attribute(AttributeName.Color_Vector3_Float, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector3_Byte, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector3_Short, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector4_Float, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector4_Byte, colorCoord, group:3),
                            new Attribute(AttributeName.Color_Vector4_Short, colorCoord, group:3),
                        };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.Normal),
                attributes.Find(e => e.name == AttributeName.Tangent)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.Tangent)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.TexCoord0_Byte),
                attributes.Find(e => e.name == AttributeName.TexCoord1_Byte),
                attributes.Find(e => e.name == AttributeName.Color_Vector4_Byte)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.TexCoord0_Byte),
                attributes.Find(e => e.name == AttributeName.TexCoord1_Byte),
                attributes.Find(e => e.name == AttributeName.Color_Vector3_Byte)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.TexCoord0_Short),
                attributes.Find(e => e.name == AttributeName.TexCoord1_Short),
                attributes.Find(e => e.name == AttributeName.Color_Vector4_Short)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.TexCoord0_Short),
                attributes.Find(e => e.name == AttributeName.TexCoord1_Short),
                attributes.Find(e => e.name == AttributeName.Color_Vector3_Short)));
        }
    }
}
