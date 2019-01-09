using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static AssetGenerator.Runtime.MeshPrimitive;

namespace AssetGenerator
{
    internal static class ReadmeStringHelper
    {
        /// <summary>
        /// Takes a value and reformats it for use in the model group readme table.
        /// Not all values automatically convert to string in a desirable way, so this function handles those cases.
        /// </summary>
        /// <returns>String intended to be displayed in a readme formatted with markdown.</returns>
        public static string ConvertValueToString(dynamic value)
        {
            string output = "ERROR";
            if (value == null)
            {
                output = ":white_check_mark:";
            }
            else
            {
                Type valueType = value.GetType();

                if (valueType.Equals(typeof(Vector2)) ||
                    valueType.Equals(typeof(Vector3)) ||
                    valueType.Equals(typeof(Vector4)))
                {
                    output = value.ToString("N1", CultureInfo.InvariantCulture).Replace('<', '[').Replace('>', ']').Replace(" ", "&nbsp;");
                }
                else if (valueType.Equals(typeof(List<int>)))
                {
                    var intArray = value.ToArray();
                    var stringArray = new string[intArray.Length];
                    for (var i = 0; i < intArray.Length; i++)
                    {
                        stringArray[i] = intArray[i].ToString();
                    }
                    output = string.Join(", ", stringArray);
                    output = $"[{output}]";
                }
                else if (valueType.Equals(typeof(List<Vector2>)) ||
                         valueType.Equals(typeof(List<Vector3>)) ||
                         valueType.Equals(typeof(List<Vector4>)))
                {
                    output = ":white_check_mark:";
                }
                else if (valueType.Equals(typeof(Runtime.Image)))
                {
                    // 18 is normal cell height. Use height=\"72\" width=\"72\" to clamp the size, but currently removed
                    // due to stretching when the table is too wide. Using thumbnails of the intended size for now.
                    Regex changePath = new Regex(@"(.*)(?=\/)");
                    string thumbnailPath = changePath.Replace(value.Uri, "Figures/Thumbnails", 1);
                    output = $"[<img src=\"{thumbnailPath}\" align=\"middle\">]({value.Uri})";
                }
                else if (valueType.Equals(typeof(Matrix4x4)))
                {
                    List<List<float>> matrixFloat = new List<List<float>>();                    
                    matrixFloat.Add(new List<float>() { value.M11, value.M12, value.M13, value.M14 });
                    matrixFloat.Add(new List<float>() { value.M21, value.M22, value.M23, value.M24 });
                    matrixFloat.Add(new List<float>() { value.M31, value.M32, value.M33, value.M34 });
                    matrixFloat.Add(new List<float>() { value.M41, value.M42, value.M43, value.M44 });

                    List<List<string>> matrixString = new List<List<string>>();
                    foreach (var row in matrixFloat)
                    {
                        matrixString.Add(new List<string>());
                        foreach (var num in row)
                        {
                            matrixString.Last().Add(num.ToString("N1", CultureInfo.InvariantCulture));
                        }
                    }

                    output = "";
                    foreach (var row in matrixString)
                    {
                        output += $"[{string.Join(",&nbsp;", row)}]<br>";
                    }
                }
                else if (valueType.Equals(typeof(Quaternion)))
                {
                    output = string.Format(CultureInfo.InvariantCulture, "[{0:N1}, {1:N1}, {2:N1}, {3:N1}]",
                        value.X, value.Y, value.Z, value.W).Replace(" ", "&nbsp;");
                }
                else if (valueType.Equals(typeof(TextureCoordsComponentTypeEnum)))
                {
                    if (value == TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE)
                    {
                        output = "Byte";
                    }
                    else if (value == TextureCoordsComponentTypeEnum.NORMALIZED_USHORT)
                    {
                        output = "Short";
                    }
                    else
                    {
                        output = "Float";
                    }
                }
                else if (valueType.Equals(typeof(IndexComponentTypeEnum)))
                {
                    if (value == IndexComponentTypeEnum.UNSIGNED_BYTE)
                    {
                        output = "Byte";
                    }
                    else if (value == IndexComponentTypeEnum.UNSIGNED_SHORT)
                    {
                        output = "Short";
                    }
                    else
                    {
                        output = "Int";
                    }
                }
                else
                {
                    if (valueType.Equals(typeof(float)))
                    {
                        // Displays two digits for floats.
                        output = value.ToString("0.0", CultureInfo.InvariantCulture); 
                    }
                    else if (valueType.BaseType.Equals(typeof(Enum)))
                    {
                        output = GenerateNameWithSpaces(value.ToString(), fullName: true);
                    }
                    else
                    {
                        output = value.ToString();
                    }
                }

                if (output != "ERROR")
                {
                    return output;
                }
                else
                {
                    Console.WriteLine("Unable to convert the value for an attribute into a format that can be added to the log.");
                    return output;
                }
            }

            return output;
        }

        /// <summary>
        /// Takes a string and puts spaces before capitals to make it more human readable.
        /// </summary>
        /// <returns>String with added spaces</returns>
        public static string GenerateNameWithSpaces(string sourceName, bool fullName = false)
        {
            var name = new StringBuilder();
            name.Append(sourceName[0]);
            for (var i = 1; i < sourceName.Length; i++)
            {
                if (Equals(sourceName[i], '_') && !fullName)
                {
                    break;
                }
                else if (char.IsUpper(sourceName[i]) &&
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
