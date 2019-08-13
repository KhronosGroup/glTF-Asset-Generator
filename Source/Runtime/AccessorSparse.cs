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
        public int ValuesCount { get; protected set; }

        /// <summary>
        /// Points to those accessor attributes that deviate from their initialization value.
        /// </summary>
        public IEnumerable<int> Indices { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        // Only assign values of UNSIGNED_BYTE, UNSIGNED_SHORT, or UNSIGNED_INT.
        public Accessor.ComponentTypeEnum IndicesComponentType { get; protected set; }

        /// <summary>
        /// Stores the displaced accessor attributes pointed by Indices.
        /// Must have the same componentType and number of components as the base accessor.
        /// </summary>
        private IEnumerable _values;
        public IEnumerable Values
        {
            get
            {
                return _values;
            }
            set
            {
                // Store the element count to avoid casting.
                Values = value;
                int count = 0;
                foreach (var i in value)
                {
                    count++;
                }
                ValuesCount = count;
            }
        }

        /// <summary>
        /// Component type of the values encoded in the target accessor.
        /// </summary>
        public Accessor.ComponentTypeEnum ValuesComponentType { get; protected set; }

        /// <summary>
        /// Name for the sparse accessor if it does not reference a buffer view.
        /// </summary>
        public string Name { get; protected set; }

        public AccessorSparse(List<int> indices, Accessor.ComponentTypeEnum indicesComponentType, Accessor.ComponentTypeEnum valuesComponentType, IEnumerable values, string name = "")
        {
            ValuesCount = indices.Count;
            IndicesComponentType = indicesComponentType;
            Indices = indices;
            ValuesComponentType = valuesComponentType;
            Values = values;
            Name = name;
        }
    }

    internal class AccessorSparse<T> : AccessorSparse
    {
        public new IEnumerable<T> Values { get; protected set; }

        public AccessorSparse(List<int> indices, Accessor.ComponentTypeEnum indicesComponentType, Accessor.ComponentTypeEnum valuesComponentType, IEnumerable<T> values, string name = "")
            : base(indices, indicesComponentType, valuesComponentType, values, name)
        {
            ValuesCount = indices.Count;
            IndicesComponentType = indicesComponentType;
            Indices = indices;
            ValuesComponentType = valuesComponentType;
            Values = values;
            Name = name;
        }
    }
}
