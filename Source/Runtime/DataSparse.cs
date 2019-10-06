using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    internal static class DataSparse
    {
        public static DataSparse<T> Create<T>(IDictionary<int, T> map)
        {
            return Create(DataType.UnsignedInt, map);
        }

        public static DataSparse<T> Create<T>(DataType indicesOutputType, IDictionary<int, T> map)
        {
            return new DataSparse<T>
            {
                Map = map,
                IndicesOutputType = indicesOutputType,
            };
        }
    }

    internal class DataSparse<T>
    {
        public IDictionary<int, T> Map { get; set; }
        public DataType IndicesOutputType { get; set; }
    }
}
