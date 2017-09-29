namespace AssetGenerator
{
    public class Attribute
    {
        public AttributeName name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public AttributeName prerequisite = AttributeName.Undefined;
        public int attributeGroup;

        public Attribute(AttributeName attributeName, dynamic attributeValue, AttributeName ParentAttribute = AttributeName.Undefined, int group = 0)
        {
            name = attributeName;
            value = attributeValue;
            prerequisite = ParentAttribute;
            attributeGroup = group;
        }
    }
    public enum AttributeName
    {
        Undefined,
        Name,
        BaseColorFactor,
        BaseColorTexture,
        MetallicFactor,
        RoughnessFactor,
        MetallicRoughnessTexture,
        PbrTextures,
        EmissiveFactor,
        AlphaMode_Mask,
        AlphaMode_Blend,
        AlphaMode_Opaque,
        AlphaCutoff,
        Color_Vector3_Float,
        Color_Vector4_Float,
        Color_Vector3_Byte,
        Color_Vector4_Byte,
        Color_Vector3_Short,
        Color_Vector4_Short,
        DoubleSided,
        Sampler,
        MagFilter_Nearest,
        MagFilter_Linear,
        MinFilter_Nearest,
        MinFilter_Linear,
        MinFilter_NearestMipmapNearest,
        MinFilter_LinearMipmapNearest,
        MinFilter_NearestMipmapLinear,
        MinFilter_LinearMipmapLinear,
        Normal,
        Position,
        Tangent,
        TexCoord0_Float,
        TexCoord0_Byte,
        TexCoord0_Short,
        TexCoord1_Float,
        TexCoord1_Byte,
        TexCoord1_Short,
        WrapS_ClampToEdge,
        WrapS_MirroredRepeat,
        WrapS_Repeat,
        WrapT_ClampToEdge,
        WrapT_MirroredRepeat,
        WrapT_Repeat,
        Source,
        TexCoord,
        NormalTexture,
        OcclusionTexture,
        EmissiveTexture,
        Scale,
        Strength
    }
}
