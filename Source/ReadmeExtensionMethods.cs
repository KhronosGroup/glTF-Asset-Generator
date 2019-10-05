using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace AssetGenerator
{
    /// <summary>
    /// Adds an alternative to the ToString method on several values. Intended for use in printing nicely formatted human readable values.
    /// </summary>
    internal static class ReadmeExtensionMethods
    {
        public static string ToReadmeString(this Vector3 value)
        {
            return FormatVectorString(value.ToString("N1", CultureInfo.InvariantCulture));
        }

        public static string ToReadmeString(this Vector3? value)
        {
            return value != null ? FormatVectorString(value.Value.ToString("N1", CultureInfo.InvariantCulture)) : string.Empty;
        }

        public static string ToReadmeString(this Vector4 value)
        {
            return FormatVectorString(value.ToString("N1", CultureInfo.InvariantCulture));
        }

        private static string FormatVectorString(string value)
        {
            return value.Replace('<', '[').Replace('>', ']').Replace(" ", "&nbsp;");
        }

        public static string ToReadmeString(this IEnumerable<int> value)
        {
            var intArray = value.ToArray();
            var stringArray = new string[intArray.Length];
            for (var i = 0; i < intArray.Length; i++)
            {
                stringArray[i] = intArray[i].ToString();
            }
            return $"[{string.Join(", ", stringArray)}]";
        }

        public static string ToReadmeString(this IEnumerable<Vector3> value)
        {
            return ":white_check_mark:";
        }

        public static string ToReadmeString(this IEnumerable<Vector4> value)
        {
            return ":white_check_mark:";
        }

        public static string ToReadmeString(this Runtime.Image value)
        {
            // 18 is normal cell height. Use height=\"72\" width=\"72\" to clamp the size, but currently removed
            // due to stretching when the table is too wide. Using thumbnails of the intended size for now.
            Regex changePath = new Regex(@"(.*)(?=\/)");
            string thumbnailPath = changePath.Replace(value.Uri, "Figures/Thumbnails", 1);
            return $"[<img src=\"{thumbnailPath}\" align=\"middle\">]({value.Uri})";
        }

        public static string ToReadmeString(this Matrix4x4 value)
        {
            return Matrix4x4ToString(value);
        }

        public static string ToReadmeString(this Matrix4x4? value)
        {
            return value != null ? Matrix4x4ToString(value.Value) : "";
        }

        private static string Matrix4x4ToString(Matrix4x4 value)
        {
            var matrixString = new string[][]
            {
                new [] { value.M11.ToReadmeString(), value.M12.ToReadmeString(), value.M13.ToReadmeString(), value.M14.ToReadmeString() },
                new [] { value.M21.ToReadmeString(), value.M22.ToReadmeString(), value.M23.ToReadmeString(), value.M24.ToReadmeString() },
                new [] { value.M31.ToReadmeString(), value.M32.ToReadmeString(), value.M33.ToReadmeString(), value.M34.ToReadmeString() },
                new [] { value.M41.ToReadmeString(), value.M42.ToReadmeString(), value.M43.ToReadmeString(), value.M44.ToReadmeString() },
            };

            var output = new StringBuilder();
            foreach (var row in matrixString)
            {
                output.Append($"[{string.Join(",&nbsp;", row)}]<br>");
            }
            return output.ToString();
        }

        public static string ToReadmeString(this Quaternion value)
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:N1}, {1:N1}, {2:N1}, {3:N1}]", value.X, value.Y, value.Z, value.W).Replace(" ", "&nbsp;");
        }

        public static string ToReadmeString(this Enum value)
        {
            return GenerateNameWithSpaces(value.ToString());
        }

        public static string ToReadmeString(this float value)
        {
            // Displays two digits for floats.
            return value.ToString("0.0", CultureInfo.InvariantCulture);
        }

        public static string ToReadmeString(this float? value)
        {
            // Displays two digits for floats.
            return value != null ? value.Value.ToString("0.0", CultureInfo.InvariantCulture) : "";
        }

        /// <summary>
        /// Takes a string and puts spaces before capitals to make it more human readable.
        /// </summary>
        /// <returns>String with added spaces</returns>
        public static string GenerateNameWithSpaces(string sourceName)
        {
            var name = new StringBuilder();
            name.Append(sourceName[0]);
            for (var i = 1; i < sourceName.Length; i++)
            {
                if (char.IsUpper(sourceName[i]) &&
                    sourceName[i - 1] != ' ' &&
                    !char.IsUpper(sourceName[i - 1]))
                {
                    name.Append(' ');
                }
                else if (char.IsNumber(sourceName[i]))
                {
                    name.Append(' ');
                }

                if (!Equals(sourceName[i], '_'))
                {
                    if (char.IsUpper(sourceName[i]) &&
                        name.Length > 0 &&
                        char.IsUpper(sourceName[i - 1]))
                    {
                        name.Append(char.ToLower(sourceName[i]));
                    }
                    else
                    {
                        name.Append(sourceName[i]);
                    }
                }
            }

            var output = name.ToString().Replace("Uv", "UV");

            return output;
        }
    }
}
