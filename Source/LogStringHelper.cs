using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    public static class LogStringHelper
    {
        public static string ConvertTestValueToString(Property param)
        {
            string output = "ERROR";
            if (param.value == null)
            {
                output = ":white_check_mark:";
            }
            else
            {
                Type valueType = param.value.GetType();

                if (valueType.Equals(typeof(Vector2)) ||
                    valueType.Equals(typeof(Vector3)) ||
                    valueType.Equals(typeof(Vector4)))
                {
                    output = param.value.ToString().Replace('<', '[').Replace('>', ']').Replace(" ", "&nbsp;");
                }
                else if (valueType.Equals(typeof(List<int>)))
                {
                    var floatArray = param.value.ToArray();
                    string[] stringArray = new string[floatArray.Length];
                    for (int i = 0; i < floatArray.Length; i++)
                    {
                        stringArray[i] = floatArray[i].ToString();
                    }
                    output = String.Join(", ", stringArray);
                    output = "[" + output + "]";
                }
                else if (valueType.Equals(typeof(List<Vector2>)) ||
                         valueType.Equals(typeof(List<Vector3>)) ||
                         valueType.Equals(typeof(List<Vector4>)))
                {
                    // Generates a name for nonBinary attributes
                    if (param.propertyGroup > 0)
                    {
                        output = GenerateNonbinaryName(param.name.ToString());
                    }
                    else
                    {
                        output = ":white_check_mark:";
                    }
                }
                else if (valueType.Equals(typeof(Runtime.Image)))
                {
                    // 18 is normal cell height
                    output = String.Format("<img src=\"./{0}\" height=\"72\" width=\"72\" align=\"middle\">", param.value.Uri);
                }
                else if (valueType.Equals(typeof(Matrix4x4)))
                {
                    List<List<string>> matrix = new List<List<string>>();
                    matrix.Add(new List<string>(){
                        param.value.M11.ToString(), param.value.M12.ToString(), param.value.M13.ToString(), param.value.M14.ToString()
                    });
                    matrix.Add(new List<string>(){
                        param.value.M21.ToString(), param.value.M22.ToString(), param.value.M23.ToString(), param.value.M24.ToString()
                    });
                    matrix.Add(new List<string>(){
                        param.value.M31.ToString(), param.value.M32.ToString(), param.value.M33.ToString(), param.value.M34.ToString()
                    });
                    matrix.Add(new List<string>(){
                        param.value.M41.ToString(), param.value.M42.ToString(), param.value.M43.ToString(), param.value.M44.ToString()
                    });
                    output = "";
                    foreach (var row in matrix)
                    {
                        output += '[' + String.Join(", ", row) + "]<br>";
                    }
                }
                else // Likely a type that is easy to convert
                {
                    if (valueType.Equals(typeof(float)))
                    {
                        output = param.value.ToString("0.0"); // Displays two digits for floats
                    }
                    else if (valueType.BaseType.Equals(typeof(Enum)) ||
                             valueType.Equals(typeof(AssetGenerator.VertexColor)))
                    {
                        // Use the TestValue enum instead of the Runtime enum
                        output = GenerateNonbinaryName(param.name.ToString());
                    }
                    else
                    {
                        output = param.value.ToString();
                    }
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

        public static string[] GenerateName(List<Property> paramSet)
        {
            string[] name = new string[paramSet.Count()];

            for (int i = 0; i < paramSet.Count; i++)
            {
                name[i] = paramSet[i].name.ToString();
            }
            if (name == null)
            {
                name = new string[1]
                    {
                        "NoParametersSet"
                    };
            }
            return name;
        }

        /// <summary>
        /// Takes a string and puts spaces before capitals to make it more human readable.
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns>String with added spaces</returns>
        //https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
        public static string GenerateNameWithSpaces(string sourceName)
        {
            StringBuilder name = new StringBuilder();
            name.Append(sourceName[0]);
            for (int i = 1; i < sourceName.Length; i++)
            {
                if (Equals(sourceName[i], '_'))
                {
                    break;
                }
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
                name.Append(sourceName[i]);
            }
            return name.ToString();
        }

        static string GenerateNonbinaryName(string sourceName)
        {
            StringBuilder name = new StringBuilder();
            bool beginningFound = false;
            for (int i = 0; i < sourceName.Length; i++)
            {
                if (beginningFound)
                {
                    if (Equals(sourceName[i], '_'))
                    {
                        name.Append(' ');
                    }
                    else if (char.IsUpper(sourceName[i]))
                    {
                        name.Append(' ');
                        name.Append(sourceName[i]);
                    }
                    else
                    {
                        name.Append(sourceName[i]);
                    }
                }
                if (Equals(sourceName[i], '_'))
                {
                    beginningFound = true;
                    name.Append(sourceName[i + 1]); // Avoids starting with a space
                    i++;
                }
            }

            return name.ToString();
        }
    }
}
