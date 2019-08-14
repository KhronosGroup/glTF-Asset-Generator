﻿using System;
using System.Reflection;

namespace AssetGenerator
{
    /// <summary>
    /// A property is a value that can be set on a glTF model.
    /// This class tracks the value of the property and related metadata.
    /// </summary>
    internal class Property
    {
        public PropertyName Name;
        public string ReadmeColumnName;
        public string ReadmeValue;
        public Func<object> Value { get; set; }

        public Property(PropertyName enumName, string displayValue)
        {
            Name = enumName;
            ReadmeColumnName = ReadmeExtensionMethods.GenerateNameWithSpaces(enumName.ToString());
            ReadmeValue = displayValue;
        }

        public override bool Equals(object obj)
        {
            var otherProperty = obj as Property;
            if (Name == otherProperty.Name)
            {
                return ReadmeValue == otherProperty.ReadmeValue;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    /// <summary>
    /// Pass an object to CloneObject, and it returns a copy of that object that isn't just a reference.
    /// </summary>
    internal static class DeepCopy
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
                Type elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                // Catch for types in System.Numerics.
                if (elementType == null)
                {
                    elementType = Type.GetType(type.AssemblyQualifiedName.Replace("[]", string.Empty));
                }

                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                    {
                        continue;
                    }
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

    internal enum PropertyName
    {
        Mode,
        IndicesValues,
        IndicesComponentType,
        IndicesType,
        ValueType,
        AlphaMode,
        AlphaCutoff,
        DoubleSided,
        VertexUV0,
        VertexPosition,
        VertexNormal,
        VertexTangent,
        VertexColor,
        NormalTexture,
        Normals,
        NormalTextureScale,
        OcclusionTexture,
        OcclusionTextureStrength,
        EmissiveTexture,
        EmissiveFactor,
        ExtensionUsed,
        ExtensionRequired,
        SpecularGlossinessOnMaterial0,
        SpecularGlossinessOnMaterial1,
        BaseColorTexture,
        BaseColorFactor,
        MetallicRoughnessTexture,
        MetallicFactor,
        RoughnessFactor,
        DiffuseTexture,
        DiffuseFactor,
        SpecularGlossinessTexture,
        SpecularFactor,
        GlossinessFactor,
        Primitive0,
        Primitive1,
        Material0WithBaseColorFactor,
        Material1WithBaseColorFactor,
        Primitive0VertexUV0,
        Primitive1VertexUV0,
        Primitive0VertexUV1,
        Primitive1VertexUV1,
        Matrix,
        Translation,
        Rotation,
        Scale,
        Target,
        Interpolation,
        WrapT,
        WrapS,
        MagFilter,
        MinFilter,
        Version,
        MinVersion,
        SparseAccessor,
        Path,
        BufferView,
        Description,
        ModelShouldLoad,
        JointsComponentType,
        WeightComponentType,
        SamplerOutputComponentType,
        LeftPrimitiveIndices,
        RightPrimitiveIndices,
        Difference,
    }
}
