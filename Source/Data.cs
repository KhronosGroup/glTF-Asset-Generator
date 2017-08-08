using System;
using System.Collections.Generic;
using System.IO;

namespace AssetGenerator
{
    internal class Data
    {
        public string Name { get; private set; }
        public BinaryWriter Writer { get; private set; }

        public Data(string name)
        {
            this.Name = name;
            this.Writer = new BinaryWriter(new MemoryStream());
        }
    }

    internal struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    internal static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(this BinaryWriter writer, IEnumerable<Vector3> values)
        {
            values.ForEach(value => writer.Write(value));
        }
    }

    internal static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }
    }
}
