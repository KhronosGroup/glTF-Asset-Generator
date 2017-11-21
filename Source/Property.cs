using System;
using System.Reflection;

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
        Description_AtRoot,
        Description_ExtensionRequired,
        Description_InProperty,
        Description_RequiresVersion,
        Description_WithFallback,
        GlossinessFactor,
        IndicesLocation_SinglePrimitive,
        IndicesLocation_TwoPrimitives,
        IndicesComponentType_Byte,
        IndicesComponentType_Short,
        IndicesComponentType_Int,
        IndicesComponentType_None,
        IndicesValues_Points,
        IndicesValues_Lines,
        IndicesValues_LineLoop,
        IndicesValues_LineStrip,
        IndicesValues_TriangleStrip,
        IndicesValues_TriangleFan,
        IndicesValues_Triangles,
        IndicesValues_Triangle,
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
        Mode_Points,
        Mode_Lines,
        Mode_Line_Loop,
        Mode_Line_Strip,
        Mode_Triangles,
        Mode_Triangle_Strip,
        Mode_Triangle_Fan,
        ModelShouldLoad_InCurrent,
        ModelShouldLoad_InFuture,
        ModelShouldLoad_No,
        Name,
        NormalTexture,
        OcclusionTexture,
        PbrTextures,
        Position,
        Primitives_Single,
        Primitives_Split1,
        Primitives_Split2,
        Primitives_Split3,
        Primitives_Split4,
        Primitive0VertexUV0,
        Primitive1VertexUV0,
        Primitive0VertexUV1,
        Primitive1VertexUV1,
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
    public static class DeepCopy
    {
        public static object CloneObject(object objSource)
        {
            //step : 1 Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            object objTarget = Activator.CreateInstance(typeSource);

            //Step2 : Get all the properties of source object type
            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //Step : 3 Assign all source property to taget object 's properties
            foreach (PropertyInfo property in propertyInfo)
            {
                //Check whether property can be written to 
                if (property.CanWrite)
                {
                    //Step : 4 check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        object objPropertyValue = property.GetValue(objSource, null);
                        if (objPropertyValue == null)
                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, CloneObject(objPropertyValue), null);
                        }
                    }
                }
            }
            return objTarget;
        }
    }
}
