using System;
using System.Reflection;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    public class Property
    {
        public Propertyname name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public Propertyname prerequisite = Propertyname.Undefined;
        public int propertyGroup;

        public Property()
        {
            name = Propertyname.Undefined;
            value = null;
            prerequisite = Propertyname.Undefined;
            propertyGroup = 0;
        }
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
        AlphaCutoff_Low,
        AlphaCutoff_High,
        AlphaCutoff_Multiplied,
        AlphaCutoff_All,
        AlphaCutoff_None,
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
        ExtensionUsed_SpecularGlossiness,
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
        IndicesValues_None,
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
        Primitive_0,
        Primitive_1,
        Primitive0VertexUV0,
        Primitive1VertexUV0,
        Primitive0VertexUV1,
        Primitive1VertexUV1,
        Primitive_NoUV0,
        RoughnessFactor,
        Sampler,
        Scale,
        Source,
        SpecularFactor,
        SpecularFactor_Override,
        SpecularGlossinessTexture,
        SpecularGlossinessOnMaterial0_Yes,
        SpecularGlossinessOnMaterial0_No,
        SpecularGlossinessOnMaterial1_Yes,
        SpecularGlossinessOnMaterial1_No,
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

    /// <summary>
    /// Pass an object to CloneObject, and it returns a deep copy of that object.
    /// </summary>
    public static class DeepCopy
    {
        public static T CloneObject<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null");
            }
            return (T)Process(obj);
        }
        static object Process(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                    type.FullName.Replace("[]", string.Empty));
                if (elementType == null) // Catch for types in System.Numerics
                {
                    elementType = Type.GetType(
                        type.AssemblyQualifiedName.ToString().Replace("[]", string.Empty));
                }
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, Process(fieldValue));
                }
                return toret;
            }
            else
            {
                throw new ArgumentException("Unknown type");
            }
        }
    }

    class VertexColor
    {
        public Runtime.MeshPrimitive.ColorComponentTypeEnum componentType;
        public Runtime.MeshPrimitive.ColorTypeEnum type;
        public List<Vector4> colors;

        public VertexColor(Runtime.MeshPrimitive.ColorComponentTypeEnum colorComponentType,
                           Runtime.MeshPrimitive.ColorTypeEnum colorType, List<Vector4> vertexColors)
        {
            componentType = colorComponentType;
            type = colorType;
            colors = vertexColors;
        }
    }
}
