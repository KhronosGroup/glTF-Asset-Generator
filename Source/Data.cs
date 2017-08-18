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
    internal struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
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
    internal struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    internal static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Vector4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static void Write(this BinaryWriter writer, IEnumerable<Vector4> values)
        {
            values.ForEach(value => writer.Write(value));
        }
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
        public static void Write(this BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(this BinaryWriter writer, IEnumerable<Vector2> values)
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
