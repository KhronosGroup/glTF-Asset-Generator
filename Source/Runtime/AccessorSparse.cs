using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator.Runtime
{
    internal class AccessorSparse
    {
        /// <summary>
        /// Points to those accessor attributes that deviate from their initialization value.
        /// </summary>
        public IEnumerable<int> Indices { get; set; }

        /// <summary>
        /// Component type of the sparse accessor's indices. Note that invalid values can be set.
        /// </summary>
        public Accessor.ComponentTypeEnum IndicesComponentType { get; set; }

        /// <summary>
        /// Stores the displaced accessor attributes pointed by Indices.
        /// Must have the same component type and number of components as the base accessor.
        /// </summary>
        public IEnumerable Values { get; set; }

        /// <summary>
        /// Component type of the values encoded in the target accessor. Note that invalid values can be set.
        /// </summary>
        public Accessor.ComponentTypeEnum ValuesComponentType { get; protected set; }

        /// <summary>
        /// Name for the sparse accessor if it does not reference a buffer view.
        /// </summary>
        public string Name { get; protected set; }

        public int InitializationArraySize { get; protected set; }

        public int Count
        {
            get
            {
                return Indices.Count();
            }
        }

        /// <summary>
        /// Create a Sparse Accessor. Set a name if there is no base accessor to be initialized from.
        /// </summary>
        public AccessorSparse(IEnumerable<int> indices, Accessor.ComponentTypeEnum indicesComponentType, Accessor.ComponentTypeEnum valuesComponentType, IEnumerable values, int initializationArraySize = 0, string name = "")
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
