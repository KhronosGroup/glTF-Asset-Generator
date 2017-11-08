using System;
using System.Collections.Generic;
using System.IO;
using glTFLoader.Schema;

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
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            Vector4 other = (Vector4)obj;
            return (this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w);
        }
        public override int GetHashCode()
        {
            return (x + y + z + w).GetHashCode();
        }
        public static bool operator ==(Vector4 v1, Vector4 v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector4 v1, Vector4 v2)
        {
            return !v1.Equals(v2);
        }

        public float[] ToArray()
        {
            float[] result =  new float[4] {x, y, z, w};
            return result;
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
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            Vector3 other = (Vector3)obj;
            return (this.x == other.x && this.y == other.y && this.z == other.z );
        }
        public override int GetHashCode()
        {
            return (x + y + z).GetHashCode();
        }
        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return !v1.Equals(v2);
        }
        public float[] ToArray()
        {
            float[] result = new float[3] { x, y, z};
            return result;
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
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            Vector2 other = (Vector2)obj;
            return (this.x == other.x && this.y == other.y);
        }
        public override int GetHashCode()
        {
            return (x + y ).GetHashCode();
        }
        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !v1.Equals(v2);
        }
        public float[] ToArray()
        {
            float[] result = new float[2] { x, y};
            return result;
        }
    }

    /// <summary>
    /// Matrix struct which represents a 4x4 matrix
    /// </summary>
    internal class Matrix4x4
    {
        public Vector4[] Rows { get; set; }

        /// <summary>
        /// Defines a 4x4 matrix by passing in Vector4 rows, from top to bottom.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <param name="r3"></param>
        /// <param name="r4"></param>
        public Matrix4x4(Vector4 r1, Vector4 r2, Vector4 r3, Vector4 r4)
        {
            Rows = new Vector4[] { r1, r2, r3, r4 };
        }
        /// <summary>
        /// Returns the identity matrix
        /// </summary>
        /// <returns></returns>
        public static Matrix4x4 Identity()
        {
            return new Matrix4x4(
                new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
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
    internal class Quaternion
    {
        public Vector4 Components { get; private set; }
        /// <summary>
        /// Create a quaternion with x, y and z the axis, and w the angle (in radians)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quaternion(float x, float y, float z, float w)
        {
            Components = new Vector4(x, y, x, w);
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
