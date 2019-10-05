using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal enum DataType
    {
        Default,

        // Integral
        UnsignedByte,
        UnsignedShort,
        UnsignedInt,

        // Floating
        Float,
        NormalizedUnsignedByte,
        NormalizedUnsignedShort,
        NormalizedByte,
        NormalizedShort,
    }

    internal class Data
    {
        public DataType OutputType { get; set; }

        public static Data<T> Create<T>(IEnumerable<T> values)
        {
            return new Data<T>
            {
                Values = values,
            };
        }

        public static Data<T> Create<T>(IEnumerable<T> values, DataType outputType)
        {
            return new Data<T>
            {
                Values = values,
                OutputType = outputType,
            };
        }

        protected Data()
        {
        }
    }

    internal class Data<T> : Data
    {
        /// T must be one of the following types:
        /// - int
        /// - float
        /// - Vector2
        /// - Vector3
        /// - Vector4
        /// - Quaternion
        /// - Matrix4x4
        /// - JointVector
        /// - WeightVector
        public IEnumerable<T> Values { get; set; }
    }
}
