using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal enum DataType
    {
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

    internal abstract class Data
    {
        public DataType OutputType { get; set; }

        public static Data<T> Create<T>(IEnumerable<T> values, DataSparse<T> sparse = null)
        {
            var outputType = (values is IEnumerable<int> || values is IEnumerable<JointVector>) ? DataType.UnsignedInt : DataType.Float;
            return Create(values, outputType, sparse);
        }

        public static Data<T> Create<T>(IEnumerable<T> values, DataType outputType, DataSparse<T> sparse = null)
        {
            return new Data<T>
            {
                Values = values,
                OutputType = outputType,
                Sparse = sparse,
            };
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

        public DataSparse<T> Sparse { get; set; }
    }
}
