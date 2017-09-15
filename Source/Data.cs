using System;
using System.Collections.Generic;
using System.IO;
using glTFLoader.Schema;

namespace AssetGenerator
{
    public class Data
    {
        public string Name { get; private set; }
        public BinaryWriter Writer { get; private set; }

        public Data(string name)
        {
            this.Name = name;
            this.Writer = new BinaryWriter(new MemoryStream());
        }
    }
    public struct Vector4<T>
    {
        public T x;
        public T y;
        public T z;
        public T w;

        public Vector4(T x, T y, T z, T w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            Vector4<T> curr = (Vector4<T>)this;
            Vector4<T> other = (Vector4<T>)obj;
            return EqualityComparer<T>.Default.Equals(curr.x, other.x) && EqualityComparer<T>.Default.Equals(curr.y, other.y) && EqualityComparer<T>.Default.Equals(curr.z, other.z) && EqualityComparer<T>.Default.Equals(curr.w, curr.w);
        }
        public override int GetHashCode()
        {
            return Tuple.Create(x, y, z, w).GetHashCode();
        }
        public static bool operator ==(Vector4<T> v1, Vector4<T> v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector4<T> v1, Vector4<T> v2)
        {
            return !v1.Equals(v2);
        }

        public T[] ToArray()
        {
            T[] result =  new T[4] {x, y, z, w};
            return result;
        }
    }

    public struct Vector3<T>
    {
        public T x;
        public T y;
        public T z;

        public Vector3(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            Vector3<T> curr = (Vector3<T>)this;
            Vector3<T> other = (Vector3<T>)obj;

            return EqualityComparer<T>.Default.Equals(curr.x, other.x) && EqualityComparer<T>.Default.Equals(curr.y, other.y) && EqualityComparer<T>.Default.Equals(curr.z, other.z);
        }
        public override int GetHashCode()
        {
            return Tuple.Create(x, y, z).GetHashCode();
        }
        public static bool operator ==(Vector3<T> v1, Vector3<T> v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector3<T> v1, Vector3<T> v2)
        {
            return !v1.Equals(v2);
        }
        public T[] ToArray()
        {
            T[] result = new T[3] { x, y, z};
            return result;
        }
    }
    public struct Vector2<T>
    {
        public T x;
        public T y;

        public Vector2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            Vector2<T> curr = (Vector2<T>)this;
            Vector2<T> other = (Vector2<T>)obj;

            return EqualityComparer<T>.Default.Equals(curr.x, other.x) && EqualityComparer<T>.Default.Equals(curr.y, other.y);
        }
        public override int GetHashCode()
        {
            return Tuple.Create(x,y).GetHashCode();
        }
        public static bool operator ==(Vector2<T> v1, Vector2<T> v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector2<T> v1, Vector2<T> v2)
        {
            return !v1.Equals(v2);
        }
        public T[] ToArray()
        {
            T[] result = new T[2] { x, y};
            return result;
        }
    }

    /// <summary>
    /// Matrix struct which represents a 4x4 matrix
    /// </summary>
    public class Matrix4x4
    {
        public Vector4<float>[] Rows { get; set; }

        /// <summary>
        /// Defines a 4x4 matrix by passing in Vector4 rows, from top to bottom.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <param name="r3"></param>
        /// <param name="r4"></param>
        public Matrix4x4(Vector4<float> r1, Vector4<float> r2, Vector4<float> r3, Vector4<float> r4)
        {
            Rows = new Vector4<float>[] { r1, r2, r3, r4 };
        }
        /// <summary>
        /// Returns the identity matrix
        /// </summary>
        /// <returns></returns>
        public static Matrix4x4 Identity()
        {
            return new Matrix4x4(
                new Vector4<float>(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector4<float>(0.0f, 1.0f, 0.0f, 0.0f),
                new Vector4<float>(0.0f, 0.0f, 1.0f, 0.0f),
                new Vector4<float>(0.0f, 0.0f, 0.0f, 1.0f)
               );
        }

        public float[] ToArray()
        {
            float[] result = {
                Rows[0].x, Rows[0].y, Rows[0].z, Rows[0].w,
                Rows[1].x, Rows[1].y, Rows[1].z, Rows[1].w,
                Rows[2].x, Rows[2].y, Rows[2].z, Rows[2].w,
                Rows[3].x, Rows[3].y, Rows[3].z, Rows[3].w
            };

            return result;

        }
        
    }

    /// <summary>
    /// Defines a Quaternion where x, y and z represent the axis, and w is the angle in radians
    /// </summary>
    public class Quaternion
    {
        public Vector4<float> Components { get; private set; }
        /// <summary>
        /// Create a quaternion with x, y and z the axis, and w the angle (in radians)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quaternion(float x, float y, float z, float w)
        {
            Components = new Vector4<float>(x, y, x, w);
        }
        /// <summary>
        /// Creates the identity quaternion
        /// </summary>
        /// <returns></returns>
        public static Quaternion Identity()
        {
            return new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        }
        public float[] ToArray() => Components.ToArray();
    }
    



    internal static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Vector4<float> value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static void Write(this BinaryWriter writer, IEnumerable<Vector4<float>> values)
        {
            values.ForEach(value => writer.Write(value));
        }
        public static void Write(this BinaryWriter writer, Vector3<float> value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(this BinaryWriter writer, IEnumerable<Vector3<float>> values)
        {
            values.ForEach(value => writer.Write(value));
        }
        public static void Write(this BinaryWriter writer, Vector2<float> value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(this BinaryWriter writer, IEnumerable<Vector2<float>> values)
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
