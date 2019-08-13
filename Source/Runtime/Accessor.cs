using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class Accessor
    {
        //object AccessorSparse;

        public IEnumerable Values { get; set; }

        public ComponentTypeEnum? ComponentType { get; set; }

        public TypeEnum? Type { get; set; }

        public enum ComponentTypeEnum { FLOAT, BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, UNSIGNED_INT }
        public enum TypeEnum { SCALAR, VEC2, VEC3, VEC4, MAT2, MAT3, MAT4 }

        public Accessor()
        {

        }

        public Accessor(IEnumerable values, ComponentTypeEnum? componentType = null, TypeEnum? type = null)
        {
            // Checks if only one of the types is null.
            if ((componentType == null && type != null) || (componentType != null && type == null))
            {
                throw new ArgumentException($"Only one type is set. Both ComponentType and Type need to be set or neither.");
            }

            Values = values;
            
            // Assigns default types when they are not explicitly set.
            if (componentType == null && type == null)
            {
                Type valuesType = values.GetType();
                Type valuesElementType;
                if (valuesType.IsGenericType)
                {
                    valuesElementType = valuesType.GetGenericArguments()[0];
                }
                else if (valuesType.IsArray)
                {
                    valuesElementType = valuesType.GetElementType();
                }
                else
                {
                    throw new Exception($"Bad type {valuesType}");
                }
                
                if (valuesElementType == typeof(int))
                {
                    // Indicies
                    ComponentType = ComponentTypeEnum.UNSIGNED_INT;
                    Type = TypeEnum.SCALAR;
                }
                else if (valuesElementType == typeof(List<Vector2>) || valuesElementType == typeof(Vector2[]))
                {
                    // Texture Coords
                    ComponentType = ComponentTypeEnum.FLOAT;
                    Type = TypeEnum.VEC2;
                }
                else if (valuesElementType == typeof(Vector3))
                {
                    // Positions, Normals, Colors
                    ComponentType = ComponentTypeEnum.FLOAT;
                    Type = TypeEnum.VEC3;
                }
                else if (valuesElementType == typeof(Vector4))
                {
                    // Quaternion, Tangent, Colors
                    ComponentType = ComponentTypeEnum.FLOAT;
                    Type = TypeEnum.VEC4;
                }
                else
                {
                    throw new Exception($"Unable to assign an accessor default component type default for {valuesElementType}");
                }
            }
            else
            {
                ComponentType = componentType;
                Type = type;
            }
        }
    }
}
