using System;
using System.Reflection;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Property
    {
        public PropertyName Name;
        public string ReadmeColumnName;
        public string ReadmeValue;
        public Func<object> Value { get; set; }
        public int PropertyGroup;

        public Property()
        {
            Value = null;
            PropertyGroup = 0;
        }
        public Property(PropertyName enumName, object displayValue, int group = 0)
        {
            Name = enumName;
            ReadmeColumnName = ReadmeStringHelper.GenerateNameWithSpaces(enumName.ToString());
            ReadmeValue = ReadmeStringHelper.ConvertValueToString(displayValue);
            PropertyGroup = group;
        }
    }

    /// <summary>
    /// Pass an object to CloneObject, and it returns a deep copy of that object.
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

    internal class VertexColor
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
    internal enum PropertyName
    {
        NormalTexture,
        Normals,
        NormalTextureScale,
        OcclusionTexture,
        OcclusionTextureStrength,
        EmissiveTexture,
        EmissiveFactor,
        BaseColorFactor,
        MetallicFactor,
    }
}
