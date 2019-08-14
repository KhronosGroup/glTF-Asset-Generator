using System.Collections;
using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Sparse, for use in creating a Sparse Accessor.
    /// </summary>
    internal class AccessorSparse
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
        /// Component type of the sparse accessor's indices. Note that invalid values can be set.
        /// </summary>
        public Accessor.ComponentTypeEnum IndicesComponentType { get; protected set; }

        private IEnumerable _values;
        /// <summary>
        /// Stores the displaced accessor attributes pointed by Indices.
        /// Must have the same component type and number of components as the base accessor.
        /// </summary>
        public IEnumerable Values
        {
            get
            {
                return _values;
            }
            set
            {
                // Store the element count to avoid casting.
                _values = value;
                int count = 0;
                if (value != null)
                {
                    foreach (var i in value)
                    {
                        count++;
                    }
                    ValuesCount = count;
                }
                else
                {
                    ValuesCount = 0;
                }
            }
        }

        /// <summary>
        /// Component type of the values encoded in the target accessor. Note that invalid values can be set.
        /// </summary>
        public Accessor.ComponentTypeEnum ValuesComponentType { get; protected set; }

        /// <summary>
        /// Name for the sparse accessor if it does not reference a buffer view.
        /// </summary>
        public string Name { get; protected set; }

        public int InitializationArraySize { get; protected set; }

        /// <summary>
        /// Create a Sparse Accessor. Set a name if there is no base accessor to be initialized from.
        /// </summary>
        public AccessorSparse(int[] indices, Accessor.ComponentTypeEnum indicesComponentType, Accessor.ComponentTypeEnum valuesComponentType, IEnumerable values, int initializationArraySize = 0, string name = "")
        {
            IndicesComponentType = indicesComponentType;
            Indices = indices;
            ValuesComponentType = valuesComponentType;
            Values = values;
            Name = name;
            InitializationArraySize = initializationArraySize;
        }
    }
}
