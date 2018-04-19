using System;
using System.Reflection;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    public class Property
    {
        internal PropertyName name;
        internal string readmeColumnName;
        internal string readmeValue;
        internal Func<object> value { get; set; }
        internal int propertyGroup;

        internal Property()
        {
            value = null;
            propertyGroup = 0;
        }
        internal Property(PropertyName enumName, string columnName, object displayValue, int group = 0)
        {
            name = enumName;
            readmeColumnName = columnName;
            readmeValue = ReadmeStringHelper.ConvertValueToString(displayValue);
            propertyGroup = group;
        }
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
