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
        /// 
        /// </summary>
        public bool OmitBufferView { get; protected set; }

        /// <summary>
        /// Number of entries stored in the sparse array.
        /// </summary>
        public int Count { get; protected set; }

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
        ///Must have the same componentType and number of components as the base accessor.
        /// </summary>
        public IEnumerable Values { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable BaseValues { get; protected set; }
        public ValuesComponentTypeEnum ValuesComponentType { get; protected set; }

        public enum IndicesComponentTypeEnum { UNSIGNED_BYTE, UNSIGNED_SHORT, UNSIGNED_INT}
        public enum ValuesComponentTypeEnum { BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, UNSIGNED_INT, FLOAT}

        public AccessorSparse(List<int> indices, IndicesComponentTypeEnum indicesComponentType, ValuesComponentTypeEnum valuesComponentType, IEnumerable values, IEnumerable baseValues, bool omitBufferView)
        {
            Count = indices.Count;
            IndicesComponentType = indicesComponentType;
            Indices = indices;
            ValuesComponentType = valuesComponentType;
            Values = values;
            BaseValues = baseValues;
            OmitBufferView = omitBufferView;
        }
    }

    internal class AccessorSparse<T> : AccessorSparse
    {
        public new IEnumerable<T> Values { get; protected set; }
        public new IEnumerable<T> BaseValues { get; protected set; }

        public AccessorSparse(List<int> indices, IndicesComponentTypeEnum indicesComponentType, ValuesComponentTypeEnum valuesComponentType, IEnumerable<T> values, IEnumerable<T> baseValues, bool omitBufferView)
            : base(indices, indicesComponentType, valuesComponentType, values, baseValues, omitBufferView)
        {
            Values = values;
            BaseValues = baseValues;
        }
    }
}
