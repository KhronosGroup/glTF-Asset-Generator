using System.Collections;
using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Sparse, for use in creating a Sparse Accessor.
    /// </summary>
    internal abstract class AccessorSparse
    {
        /// <summary>
        /// Number of attributes encoded in this sparse accessor.
        /// </summary>
        public int SparseCount { get; protected set; }

        /// <summary>
        /// Number of attributes encoded in the target accessor.
        /// </summary>
        public int BaseCount { get; protected set; }

        /// <summary>
        /// Points to those accessor attributes that deviate from their initialization value.
        /// </summary>
        public IEnumerable<int> Indices { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IndicesComponentTypeEnum IndicesComponentType { get; protected set; }

        /// <summary>
        /// Stores the displaced accessor attributes pointed by Indices.
        /// Must have the same componentType and number of components as the base accessor.
        /// </summary>
        public IEnumerable Values { get; protected set; }

        /// <summary>
        /// Values encoded in the target accessor.
        /// </summary>
        public IEnumerable BaseValues { get; protected set; }

        /// <summary>
        /// Component type of the values encoded in the target accessor.
        /// </summary>
        public ValuesComponentTypeEnum ValuesComponentType { get; protected set; }

        /// <summary>
        /// Name for the sparse accessor if it does not reference a buffer view.
        /// </summary>
        public string Name { get; protected set; }

        public enum IndicesComponentTypeEnum { UNSIGNED_BYTE, UNSIGNED_SHORT, UNSIGNED_INT }
        public enum ValuesComponentTypeEnum { FLOAT, NORMALIZED_BYTE, NORMALIZED_UNSIGNED_BYTE, NORMALIZED_SHORT, NORMALIZED_UNSIGNED_SHORT }

        public AccessorSparse(List<int> indices, IndicesComponentTypeEnum indicesComponentType, ValuesComponentTypeEnum valuesComponentType, IEnumerable values, IEnumerable baseValues, int baseCount = 0, string name = "")
        {
            SparseCount = indices.Count;
            IndicesComponentType = indicesComponentType;
            Indices = indices;
            ValuesComponentType = valuesComponentType;
            Values = values;
            BaseValues = baseValues;
            BaseCount = baseCount;
            Name = name;
        }
    }

    internal class AccessorSparse<T> : AccessorSparse
    {
        public new IEnumerable<T> Values { get; protected set; }
        public new IEnumerable<T> BaseValues { get; protected set; }

        public AccessorSparse(List<int> indices, IndicesComponentTypeEnum indicesComponentType, ValuesComponentTypeEnum valuesComponentType, IEnumerable<T> values, IEnumerable<T> baseValues, int baseCount = 0, string name = "")
            : base(indices, indicesComponentType, valuesComponentType, values, baseValues, baseCount, name)
        {
            Values = values;
            BaseValues = baseValues;
        }
    }
}
