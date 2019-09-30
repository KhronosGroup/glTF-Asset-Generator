using System.Numerics;

namespace AssetGenerator.Conversion
{
    internal static class NumericExtensions
    {
        public static float[] ToArray(this Vector2 vec2)
        {
            return new[] { vec2.X, vec2.Y };
        }
        public static float[] ToArray(this Vector3 vec3)
        {
            return new[] { vec3.X, vec3.Y, vec3.Z };
        }
        public static float[] ToArray(this Vector4 vec4)
        {
            return new[] { vec4.X, vec4.Y, vec4.Z, vec4.W };
        }
        public static float[] ToArray(this Quaternion quaternion)
        {
            return new[] { quaternion.X, quaternion.Y, quaternion.Z, quaternion.W };
        }
        public static float[] ToArray(this Matrix4x4 mat)
        {
            return new[]
            {
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44,
            };
        }
    }
}
