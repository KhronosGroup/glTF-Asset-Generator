namespace AssetGenerator
{
    public class Property
    {
        public Propertyname name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public Propertyname prerequisite = Propertyname.Undefined;
        public int propertyGroup;

        public Property(Propertyname propertyName, dynamic propertyValue, Propertyname ParentProperty = Propertyname.Undefined, int group = 0)
        {
            name = propertyName;
            value = propertyValue;
            prerequisite = ParentProperty;
            propertyGroup = group;
        }
    }
    public enum Propertyname
    {
        Undefined,
        AlphaCutoff,
        AlphaMode_Blend,
        AlphaMode_Mask,
        AlphaMode_Opaque,
        BaseColorFactor,
        BaseColorTexture,
        DiffuseFactor,
        DiffuseTexture,
        DoubleSided,
        EmissiveFactor,
        EmissiveTexture,
        ExtensionRequired,
        ExperimentalFeature_AtRoot,
        ExperimentalFeature_InProperty,
        ExperimentalFeature_WithFallback,
        ExperimentalFeature_RequiresVersion,
        GlossinessFactor,
        MagFilter_Linear,
        MagFilter_Nearest,
        MetallicFactor,
        MetallicRoughnessTexture,
        MinFilter_Linear,
        MinFilter_LinearMipmapLinear,
        MinFilter_LinearMipmapNearest,
        MinFilter_Nearest,
        MinFilter_NearestMipmapLinear,
        MinFilter_NearestMipmapNearest,
        MinVersion,
        ModelShouldLoad_No,
        ModelShouldLoad_Yes,
        Name,
        NormalTexture,
        OcclusionTexture,
        PbrTextures,
        Position,
        RoughnessFactor,
        Sampler,
        Scale,
        Source,
        SpecularFactor,
        SpecularFactor_Override,
        SpecularGlossinessTexture,
        Strength,
        TexCoord,
        Version,
        Version_Current,
        VertexColor_Vector3_Byte,
        VertexColor_Vector3_Float,
        VertexColor_Vector3_Short,
        VertexColor_Vector4_Byte,
        VertexColor_Vector4_Float,
        VertexColor_Vector4_Short,
        VertexNormal,
        VertexTangent,
        VertexUV0_Byte,
        VertexUV0_Float,
        VertexUV0_Short,
        VertexUV1_Byte,
        VertexUV1_Float,
        VertexUV1_Short,
        WrapS_ClampToEdge,
        WrapS_MirroredRepeat,
        WrapS_Repeat,
        WrapT_ClampToEdge,
        WrapT_MirroredRepeat,
        WrapT_Repeat,
    }
}
