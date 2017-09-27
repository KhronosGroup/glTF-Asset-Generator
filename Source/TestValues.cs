using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public List<Attribute> attributes;
        public List<Attribute> requiredAttributes = null;
        public ImageAttribute[] imageAttributes;
        public List<List<Attribute>> specialCombos = new List<List<Attribute>>();
        public List<List<Attribute>> removeCombos = new List<List<Attribute>>();
        public bool onlyBinaryAttributes = true;
        public bool noPrerequisite = true;
        public string texture = "UVmap2017.png";

        public TestValues(Tests testType)
        {
            testArea = testType;

            switch (testArea)
            {
                case Tests.Material:
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
                            new Attribute(AttributeName.MetallicFactor, 0.0f),
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f)),
                            new Attribute(AttributeName.EmissiveTexture, image),
                            new Attribute(AttributeName.NormalTexture, image),
                            new Attribute(AttributeName.Scale, 2.0f, AttributeName.NormalTexture),
                            new Attribute(AttributeName.OcclusionTexture, image),
                            new Attribute(AttributeName.Strength, 0.5f, AttributeName.OcclusionTexture)
                        };
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.EmissiveFactor),
                            attributes.Find(e => e.name == AttributeName.EmissiveTexture)));
                        break;
                    }
                case Tests.Material_Alpha:
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
                            new Attribute(AttributeName.NormalTexture, image),
                            new Attribute(AttributeName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                            new Attribute(AttributeName.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                            new Attribute(AttributeName.AlphaCutoff, 0.2f),
                            new Attribute(AttributeName.DoubleSided, true),
                        };
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaMode_Mask),
                            attributes.Find(e => e.name == AttributeName.AlphaCutoff)));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaMode_Mask),
                            attributes.Find(e => e.name == AttributeName.DoubleSided)));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaMode_Blend),
                            attributes.Find(e => e.name == AttributeName.DoubleSided)));
                        removeCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.AlphaCutoff)));
                        break;
                    }
                case Tests.Material_MetallicRoughness:
                    {
                        onlyBinaryAttributes = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                            new Attribute(AttributeName.BaseColorTexture, image),
                            new Attribute(AttributeName.MetallicFactor, 0.5f),
                            new Attribute(AttributeName.RoughnessFactor, 0.5f),
                            new Attribute(AttributeName.MetallicRoughnessTexture, image)
                        };
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.BaseColorTexture),
                            attributes.Find(e => e.name == AttributeName.BaseColorFactor)));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                            attributes.Find(e => e.name == AttributeName.RoughnessFactor),
                            attributes.Find(e => e.name == AttributeName.MetallicFactor)));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                            attributes.Find(e => e.name == AttributeName.MetallicFactor)));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                            attributes.Find(e => e.name == AttributeName.RoughnessFactor)));
                        break;
                    }
                case Tests.Texture_Sampler:
                    {
                        // The base glTF spec does not support mipmapping, so the MagFilter and MinFilter 
                        // attributes will have no visible affect unless mipmapping is implemented by the client
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
                            new Attribute(AttributeName.BaseColorTexture, image)
                        };
                        attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.MagFilter_Nearest, glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST, group:1),
                            new Attribute(AttributeName.MagFilter_Linear, glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR, group:1),
                            new Attribute(AttributeName.MinFilter_Nearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST, group:2),
                            new Attribute(AttributeName.MinFilter_Linear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR, group:2),
                            new Attribute(AttributeName.MinFilter_NearestMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST, group:2),
                            new Attribute(AttributeName.MinFilter_LinearMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST, group:2),
                            new Attribute(AttributeName.MinFilter_NearestMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR, group:2),
                            new Attribute(AttributeName.MinFilter_LinearMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR, group:2),
                            new Attribute(AttributeName.WrapS_ClampToEdge, glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE, group:3),
                            new Attribute(AttributeName.WrapS_MirroredRepeat, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT, group:3),
                            new Attribute(AttributeName.WrapT_ClampToEdge, glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE, group:4),
                            new Attribute(AttributeName.WrapT_MirroredRepeat, glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT, group:4)
                        };
                        break;
                    }
                case Tests.Primitive_Attribute:
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
                        
                        break;
                    }
            }
        }
    }
}
