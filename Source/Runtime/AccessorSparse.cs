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
        public ComponentTypeEnum IndicesType { get; protected set; }

        /// <summary>
        /// Stores the displaced accessor attributes pointed by Indices.
        ///Must have the same componentType and number of components as the base accessor.
        /// </summary>
        public IEnumerable Values { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable BaseValues { get; protected set; }

        public enum ComponentTypeEnum { UNSIGNED_BYTE, UNSIGNED_SHORT, UNSIGNED_INT}

        public AccessorSparse(List<int> indices, ComponentTypeEnum indicesType, IEnumerable values, IEnumerable baseValues, bool omitBufferView)
        {
            OmitBufferView = omitBufferView;
            Count = indices.Count;
            IndicesType = indicesType;
            Indices = indices;
            Values = values;
            BaseValues = baseValues;
        }
    }

    internal class AccessorSparse<T> : AccessorSparse
    {
        public new IEnumerable<T> Values { get; protected set; }
        public new IEnumerable<T> BaseValues { get; protected set; }

        public AccessorSparse(List<int> indices, ComponentTypeEnum indicesType, IEnumerable<T> values, IEnumerable<T> baseValues, bool omitBufferView)
            : base(indices, indicesType, values, baseValues, omitBufferView)
        {
            Values = values;
            BaseValues = baseValues;
        }
    }
}
