namespace AssetGenerator
{
    public class Property
    {
        public Propertyname name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public Propertyname prerequisite = Propertyname.Undefined;
        public int attributeGroup;

        public Property(Propertyname attributeName, dynamic attributeValue, Propertyname ParentAttribute = Propertyname.Undefined, int group = 0)
        {
            name = attributeName;
            value = attributeValue;
            prerequisite = ParentAttribute;
            attributeGroup = group;
        }
    }
    public enum Propertyname
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
        VertexColor_Vector3_Float,
        VertexColor_Vector4_Float,
        VertexColor_Vector3_Byte,
        VertexColor_Vector4_Byte,
        VertexColor_Vector3_Short,
        VertexColor_Vector4_Short,
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
        VertexNormal,
        Position,
        VertexTangent,
        VertexUV0_Float,
        VertexUV0_Byte,
        VertexUV0_Short,
        VertexUV1_Float,
        VertexUV1_Byte,
        VertexUV1_Short,
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
