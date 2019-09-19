using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class Accessor
    {
        /// <summary>
        /// Data referenced by the accessor.
        /// </summary>
        public IEnumerable Values { get; set; }

        /// <summary>
        /// Component type of the Accessor's value.
        /// </summary>
        public ComponentTypeEnum ComponentType { get; set; }

        /// <summary>
        /// Type of the accessor's value.
        /// </summary>
        public TypeEnum Type { get; set; }

        /// <summary>
        /// Used for creating a sparse accessor. The accessor values are used as the initialization values for the sparse accessor.
        /// </summary>
        public AccessorSparse Sparse { get; set; }

        public enum ComponentTypeEnum { FLOAT, BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, UNSIGNED_INT }
        public enum TypeEnum { SCALAR, VEC2, VEC3, VEC4, MAT2, MAT3, MAT4 }

        public int Count
        {
            get
            {
                return Values.Cast<object>().Count();
            }
        }

        public Accessor()
        {
        }

        public Accessor(IEnumerable values)
        {
            Values = values;

            Type elementType = values.Cast<object>().First().GetType();
            if (elementType == typeof(int))
            {
                ComponentType = ComponentTypeEnum.UNSIGNED_INT;
                Type = TypeEnum.SCALAR;
            }
            else if (elementType == typeof(float))
            {
                ComponentType = ComponentTypeEnum.FLOAT;
                Type = TypeEnum.SCALAR;
            }
            else if (elementType == typeof(List<Vector2>) || elementType == typeof(Vector2[]))
            {
                ComponentType = ComponentTypeEnum.FLOAT;
                Type = TypeEnum.VEC2;
            }
            else if (elementType == typeof(Vector3))
            {
                ComponentType = ComponentTypeEnum.FLOAT;
                Type = TypeEnum.VEC3;
            }
            else if (elementType == typeof(Vector4) || elementType == typeof(Quaternion))
            {
                ComponentType = ComponentTypeEnum.FLOAT;
                Type = TypeEnum.VEC4;
            }
            else
            {
                throw new Exception($"Unable to assign a default accessor component type for {elementType}");
            }
        }

        public Accessor(IEnumerable values, ComponentTypeEnum componentType, TypeEnum type)
        {
            Values = values;
            ComponentType = componentType;
            Type = type;
        }
    }
}
