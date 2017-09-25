using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public List<Parameter> parameters;
        public List<Parameter> requiredParameters = null;
        public ImageAttribute[] imageAttributes;
        private List<List<Parameter>> specialCombos = new List<List<Parameter>>();
        private List<List<Parameter>> removeCombos = new List<List<Parameter>>();
        bool onlyBinaryParams = true;
        bool noPrerequisite = true;
        string texture = "UVmap2017.png";

        public TestValues(Tests testType)
        {
            testArea = testType;

            switch (testArea)
            {
                case Tests.Materials:
                    {
                        onlyBinaryParams = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredParameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.MetallicFactor, 0.0f),
                        };
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f)),
                            new Parameter(ParameterName.EmissiveTexture, image),
                            new Parameter(ParameterName.NormalTexture, image),
                            new Parameter(ParameterName.Scale, 2.0f, ParameterName.NormalTexture),
                            new Parameter(ParameterName.OcclusionTexture, image),
                            new Parameter(ParameterName.Strength, 0.5f, ParameterName.OcclusionTexture)
                        };
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.EmissiveFactor),
                            parameters.Find(e => e.name == ParameterName.EmissiveTexture)));
                        break;
                    }
                case Tests.Alphas:
                    {
                        onlyBinaryParams = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredParameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.NormalTexture, image),
                            new Parameter(ParameterName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                        };
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.AlphaMode_MASK, glTFLoader.Schema.Material.AlphaModeEnum.MASK, 1),
                            new Parameter(ParameterName.AlphaMode_BLEND, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, 1),
                            new Parameter(ParameterName.AlphaCutoff, 0.2f),
                            new Parameter(ParameterName.DoubleSided, true),
                        };
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.AlphaMode_MASK),
                            parameters.Find(e => e.name == ParameterName.AlphaCutoff)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.AlphaMode_MASK),
                            parameters.Find(e => e.name == ParameterName.DoubleSided)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.AlphaMode_BLEND),
                            parameters.Find(e => e.name == ParameterName.DoubleSided)));
                        removeCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.AlphaCutoff)));
                        break;
                    }
                case Tests.PBRs:
                    {
                        onlyBinaryParams = false;
                        //noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        //requiredParameters = new List<Parameter>
                        //{
                        //    new Parameter(ParameterName.Undefined, "None"),
                        //};
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                            new Parameter(ParameterName.BaseColorTexture, image),
                            new Parameter(ParameterName.MetallicFactor, 0.5f),
                            new Parameter(ParameterName.RoughnessFactor, 0.5f),
                            new Parameter(ParameterName.MetallicRoughnessTexture, image)
                        };
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.BaseColorTexture),
                            parameters.Find(e => e.name == ParameterName.BaseColorFactor)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.MetallicRoughnessTexture),
                            parameters.Find(e => e.name == ParameterName.RoughnessFactor),
                            parameters.Find(e => e.name == ParameterName.MetallicFactor)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.MetallicRoughnessTexture),
                            parameters.Find(e => e.name == ParameterName.MetallicFactor)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.MetallicRoughnessTexture),
                            parameters.Find(e => e.name == ParameterName.RoughnessFactor)));
                        break;
                    }
                case Tests.Samplers:
                    {
                        // The base glTF spec does not support mipmapping, so the MagFilter and MinFilter 
                        // attributes will have no visible affect unless mipmapping is implemented by the client
                        onlyBinaryParams = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredParameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.BaseColorTexture, image)
                        };
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.MagFilter_NEAREST, glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST, 1),
                            new Parameter(ParameterName.MagFilter_LINEAR, glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR, 1),
                            new Parameter(ParameterName.MinFilter_NEAREST, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR, 2),
                            new Parameter(ParameterName.MinFilter_NEAREST_MIPMAP_NEAREST, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR_MIPMAP_NEAREST, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST, 2),
                            new Parameter(ParameterName.MinFilter_NEAREST_MIPMAP_LINEAR, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR_MIPMAP_LINEAR, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR, 2),
                            new Parameter(ParameterName.WrapS_CLAMP_TO_EDGE, glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE, 3),
                            new Parameter(ParameterName.WrapS_MIRRORED_REPEAT, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT, 3),
                            new Parameter(ParameterName.WrapT_CLAMP_TO_EDGE, glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE, 4),
                            new Parameter(ParameterName.WrapT_MIRRORED_REPEAT, glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT, 4)
                        };
                        break;
                    }
                case Tests.PrimitiveAttributes:
                    {
                        onlyBinaryParams = false;
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute(texture)
                        };
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = texture
                        };
                        requiredParameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.BaseColorTexture, image),
                            new Parameter(ParameterName.OcclusionTexture, image)
                        };
                        List<Vector3> planeNormals = new List<Vector3>()
                        {
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f),
                            new Vector3( 0.0f, 0.0f,-1.0f)
                        };
                        List<Vector2> uvCoord1 = new List<Vector2>()
                        {
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 1.0f, 1.0f),
                            new Vector2( 1.0f, 0.0f),
                            new Vector2( 0.5f, 0.0f)
                        };
                        List<Vector2> uvCoord2 = new List<Vector2>()
                        {
                            new Vector2( 0.0f, 1.0f),
                            new Vector2( 0.5f, 1.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 0.0f, 0.0f)
                        };
                        List<Vector4> colorCoord = new List<Vector4>()
                        {
                            new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                            new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                            new Vector4( 0.0f, 0.0f, 1.0f, 0.8f)
                        };
                        List<Vector4> tanCoord = new List<Vector4>()
                        {
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                            new Vector4( -1.0f, 0.0f, 0.0f, 1.0f)
                        };
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.Normal, planeNormals),
                            new Parameter(ParameterName.Tangent, tanCoord),
                            new Parameter(ParameterName.TexCoord0_FLOAT, uvCoord1, 1),
                            new Parameter(ParameterName.TexCoord0_BYTE, uvCoord1, 1),
                            new Parameter(ParameterName.TexCoord0_SHORT, uvCoord1, 1),
                            new Parameter(ParameterName.TexCoord1_FLOAT, uvCoord2, ParameterName.TexCoord0_FLOAT, 2),
                            new Parameter(ParameterName.TexCoord1_BYTE, uvCoord2, ParameterName.TexCoord0_BYTE, 2),
                            new Parameter(ParameterName.TexCoord1_SHORT, uvCoord2, ParameterName.TexCoord0_SHORT, 2),
                            new Parameter(ParameterName.Color_VEC3_FLOAT, colorCoord, 3),
                            new Parameter(ParameterName.Color_VEC3_BYTE, colorCoord, 3),
                            new Parameter(ParameterName.Color_VEC4_FLOAT, colorCoord, 3),
                            new Parameter(ParameterName.Color_VEC3_SHORT, colorCoord, 3),
                            new Parameter(ParameterName.Color_VEC4_BYTE, colorCoord, 3),
                            new Parameter(ParameterName.Color_VEC4_SHORT, colorCoord, 3),
                        };
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.Normal),
                            parameters.Find(e => e.name == ParameterName.Tangent)));
                        removeCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.Tangent)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.TexCoord0_BYTE),
                            parameters.Find(e => e.name == ParameterName.TexCoord1_BYTE),
                            parameters.Find(e => e.name == ParameterName.Color_VEC3_BYTE)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.TexCoord0_BYTE),
                            parameters.Find(e => e.name == ParameterName.TexCoord1_BYTE),
                            parameters.Find(e => e.name == ParameterName.Color_VEC4_BYTE)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.TexCoord0_SHORT),
                            parameters.Find(e => e.name == ParameterName.TexCoord1_SHORT),
                            parameters.Find(e => e.name == ParameterName.Color_VEC3_SHORT)));
                        specialCombos.Add(ComboCreation(
                            parameters.Find(e => e.name == ParameterName.TexCoord0_SHORT),
                            parameters.Find(e => e.name == ParameterName.TexCoord1_SHORT),
                            parameters.Find(e => e.name == ParameterName.Color_VEC4_SHORT)));
                        break;
                    }
            }
        }

        public List<List<Parameter>> ParameterCombos()
        {
            List<List<Parameter>> finalResult;
            List<List<Parameter>> removeTheseCombos = new List<List<Parameter>>();
            List<List<Parameter>> keepTheseCombos = new List<List<Parameter>>();
            List<Parameter> isRequired = new List<Parameter>();
            List<Parameter> isPrerequisite = new List<Parameter>();
            bool prereqParam;

            //var combos = PowerSet<Parameter>(parameters);
            var combos = BasicSet<Parameter>(parameters);

            // Include any special combos
            if (specialCombos.Any())
            {
                foreach (var x in specialCombos)
                {
                    var comboIndex = combos.FindIndex(e => e.Any() && e[0].name == x[0].name && e.Count() == 1);
                    combos.Insert(comboIndex + 1, x);
                }
            }

            // Remove the explicitly excluded combos
            if (removeCombos.Any())
            {
                foreach (var x in removeCombos)
                {
                    combos.RemoveAll(e => e.Count == 1 && e[0].name == x[0].name);
                }
            }

            if (noPrerequisite == false)
            {
                // Makes a list of names of possible prerequisites
                List<ParameterName> Prerequisites = new List<ParameterName>();
                foreach (var x in parameters)
                {
                    if (x.prerequisite != ParameterName.Undefined)
                    {
                        if (Prerequisites.Any())
                        {
                            bool isNew = true;
                            foreach (var y in Prerequisites)
                            {
                                if (y == x.prerequisite)
                                {
                                    isNew = false;
                                    break;
                                }
                            }
                            if (isNew == true)
                            {
                                Prerequisites.Add(x.prerequisite);
                            }
                        }
                        else
                        {
                            Prerequisites.Add(x.prerequisite);
                        }
                    }
                }
                // Convert the name list into a list of the actual prerequisites
                foreach (var x in Prerequisites)
                {
                    foreach (var y in parameters)
                    {
                        if (x == y.name)
                        {
                            isPrerequisite.Add(y);
                            break;
                        }
                    }
                }
                // Add combos where prerequisite attributes have all dependant attributes set
                foreach (var x in isPrerequisite)
                {
                    // Start a list with the prerequisite attribute 
                    var addList = new List<Parameter>
                    {
                        x
                    };

                    // Populate that list will all of the required attributes
                    foreach (var y in parameters)
                    {
                        if (y.prerequisite == x.name)
                        {
                            addList.Add(y);
                        }
                    }
                    // Then include the combo with the rest
                    var comboIndex = combos.FindIndex(e => e.Any() && e[0].name == addList[0].name && e.Count() == 1);
                    combos.Insert(comboIndex + 1, addList);
                }
            }

            // Handle non-binary attributes in the first combo
            if (onlyBinaryParams == false)
            {
                List<Parameter> keep = new List<Parameter>();
                foreach (var x in combos[1])
                {
                    // Keep attribute if it is the first found or is binary
                    if (x.binarySet == 0 || (x.binarySet > 0 && !keep.Any()))
                    {
                        keep.Add(x);
                    }
                    else if (x.binarySet > 0)
                    {
                        bool alreadyKept = false;
                        foreach (var y in keep)
                        {
                            // Don't keep the nonbinary attribute if there is already one of that set on the list
                            if (y.binarySet == x.binarySet)
                            {
                                alreadyKept = true;
                                break;
                            }
                        }
                        if (alreadyKept == false) // Keep nonbinary attribute 
                        {
                            keep.Add(x);
                        }
                    }
                }
                // Remove the extra nonbinary attributes
                combos[1] = keep;
            }

            // Removes sets that duplicate binary entries for a single parameter (e.g. alphaMode)
            // Removes sets where an attribute is missing a required parameter
            if (onlyBinaryParams == false || noPrerequisite == false)
            {
                // Are there any prerequisite attributes? 
                prereqParam = isPrerequisite.Any();

                // Makes a list of combos to remove
                int combosCount = combos.Count();
                for (int x = 2; x < combosCount; x++) // The first two combos are already taken care of
                {
                    bool usedPrereq = false;
                    List<int> binarySets = new List<int>();
                    List<ParameterName> usedPrerequisite = new List<ParameterName>();

                    // Makes a list of each prerequisite parameter in the current combo
                    if (prereqParam == true)
                    {
                        foreach (var prereq in isPrerequisite)
                        {
                            foreach (var param in combos[x])
                            {
                                if (param.name == prereq.name)
                                {
                                    usedPrerequisite.Add(prereq.name);
                                }
                            }
                        }
                        usedPrereq = usedPrerequisite.Any();
                    }

                    foreach (var param in combos[x])
                    {
                        // Remove combos that have multiple of the same binary combo
                        if (param.binarySet > 0)
                        {
                            if (binarySets.Contains(param.binarySet))
                            {
                                removeTheseCombos.Add(combos[x]);
                                break;
                            }
                            else
                            {
                                binarySets.Add(param.binarySet);
                            }
                        }
                        // Removes combos that have a parameter missing a prerequisite
                        if (usedPrereq == true && param.prerequisite != ParameterName.Undefined)
                        {
                            bool prereqNotFound = true;
                            foreach (var prereq in usedPrerequisite)
                            {
                                if (param.prerequisite == prereq)
                                {
                                    prereqNotFound = false;
                                    break;
                                }
                            }
                            if (prereqNotFound != false)
                            {
                                removeTheseCombos.Add(combos[x]);
                                break;
                            }
                        }
                        else if (usedPrereq == false && param.prerequisite != ParameterName.Undefined)
                        {
                            removeTheseCombos.Add(combos[x]);
                            break;
                        }
                    }
                }

                // Uses the list of bad combos to trim down the original set
                int numCombos = combos.Count();
                int numRemoveTheseCombos = removeTheseCombos.Count();
                for (int x = 0; x < numCombos; x++)
                {
                    bool excludeCombo = false;
                    for (int y = 0; y < numRemoveTheseCombos; y++)
                    {
                        if (combos[x] == removeTheseCombos[y])
                        {
                            excludeCombo = true;
                            break;
                        }
                    }
                    if (excludeCombo == false)
                    {
                        keepTheseCombos.Add(combos[x]);
                    }
                }
                finalResult = keepTheseCombos;
            }
            else
            {
                // If there are only binary parameters, we don't need to check for duplicates
                finalResult = combos;
            }

            return finalResult;
        }

        private List<Parameter> ComboCreation(Parameter paramA)
        {
            List<Parameter> newCombo = new List<Parameter>();

            newCombo.Add(paramA);

            return newCombo;
        }

        private List<Parameter> ComboCreation(Parameter paramA, Parameter paramB)
        {
            List<Parameter> newCombo = new List<Parameter>();

            newCombo.Add(paramA);
            newCombo.Add(paramB);

            return newCombo;
        }

        private List<Parameter> ComboCreation(Parameter paramA, Parameter paramB, Parameter paramC)
        {
            List<Parameter> newCombo = new List<Parameter>();

            newCombo.Add(paramA);
            newCombo.Add(paramB);
            newCombo.Add(paramC);

            return newCombo;
        }

        /// <summary>
        /// Given a,b,c this returns [a,b,c], [a], [b], [c]
        /// </summary>
        /// <param name="seq"></param>
        /// <returns>List of lists containing a basic set of combinations.</returns>
        public static List<List<T>> BasicSet<T>(List<T> seq)
        {
            var basicSet = new List<List<T>>();
            basicSet.Add(new List<T>()); // Will contain the empty set
            basicSet.Add(new List<T>()); // Will contain the full set
            foreach (var x in seq)
            {
                basicSet[1].Add(x);
                var addList = new List<T>
                {
                    x
                };
                basicSet.Add(addList);
            }
            return basicSet;
        }

        /// <summary>
        /// Given a,b,c this returns all possible combinations including a full and empty set.
        /// </summary>
        /// <param name="seq"></param>
        /// <returns>List of lists containing a powerset.</returns>
        //https://stackoverflow.com/questions/19890781/creating-a-power-set-of-a-sequence
        public static List<List<T>> PowerSet<T>(List<T> seq)
        {
            var powerSet = new List<List<T>>();
            powerSet.Add(new List<T>()); // starting only with empty set
            for (int i = 0; i < seq.Count; i++)
            {
                var cur = seq[i];
                int count = 1 << i; // doubling list each time
                for (int j = 0; j < count; j++)
                {
                    var source = powerSet[j];
                    powerSet.Add(new List<T>());
                    powerSet[count + j] = new List<T>();
                    var destination = powerSet[count + j];
                    for (int q = 0; q < source.Count; q++)
                        destination.Add(source[q]);
                    destination.Add(cur);
                }
            }
            return powerSet;
        }

        public string ConvertValueToString(dynamic value)
        {
            string output = "ERROR";
            Type valueType = value.GetType();

            if (valueType.Equals(typeof(Vector2)) ||
                valueType.Equals(typeof(Vector3)) ||
                valueType.Equals(typeof(Vector4)))
            {
                output = String.Join(", ", value.ToArray());
                output = "[" + output + "]";
            }
            else if (valueType.Equals(typeof(Runtime.Image)))
            {
                output = String.Format("<img src=\"./{0}\" height=\"50\">", value.Uri);
            }
            else // It is a type that is easy to convert
            {
                output = value.ToString();
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

        public string[] GenerateName(List<Parameter> paramSet)
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
        /// Also drops '_' character and the text following it.
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns>String with added spaces</returns>
        //https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
        public string GenerateNameWithSpaces(string sourceName)
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
                else if(char.IsNumber(sourceName[i]))
                {
                    name.Append(' ');
                }
                name.Append(sourceName[i]);
            }
            return name.ToString();
        }

        public string GenerateNonbinaryName(string sourceName)
        {
            StringBuilder name = new StringBuilder();
            bool beginningFound = false;
            for (int i = 0; i < sourceName.Length; i++)
            {
                if (beginningFound)
                {
                    name.Append(sourceName[i]);
                }
                if (Equals(sourceName[i], '_'))
                {
                    beginningFound = true;
                }
            }
            return name.ToString();
        }

        /// <summary>
        /// Compares two sets of model attributes and returns whether they are equal or not, regardless of list order
        /// </summary>
        /// <param name="comboToCheck"></param>
        /// <param name="comboToFind"></param>
        /// <returns>Returns a bool, true if they contain the exact same parameters in any order</returns>
        bool FindCombo(Parameter[] comboToCheck, Parameter[] comboToFind)
        {
            if (comboToCheck.Count() == comboToFind.Count())
            {
                bool isEqual = true;
                foreach (var x in comboToCheck)
                {
                    bool containsElement = false;
                    foreach (var y in comboToFind)
                    {
                        if (x.name == y.name)
                        {
                            containsElement = true;
                            break;
                        }
                    }
                    if (containsElement == false)
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual == true)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Parameter
    {
        public ParameterName name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public ParameterName prerequisite = ParameterName.Undefined;
        public int binarySet;

        public Parameter(ParameterName parmName, dynamic parameterValue)
        {
            name = parmName;
            value = parameterValue;
            binarySet = 0;
        }

        public Parameter(ParameterName parmName, dynamic parameterValue, ParameterName ParentParam)
        {
            name = parmName;
            value = parameterValue;
            binarySet = 0;
            prerequisite = ParentParam;
        }

        public Parameter(ParameterName parmName, dynamic parameterValue, int belongsToBinarySet)
        {
            name = parmName;
            value = parameterValue;
            binarySet = belongsToBinarySet;
        }

        public Parameter(ParameterName parmName, dynamic parameterValue, ParameterName ParentParam, int belongsToBinarySet)
        {
            name = parmName;
            value = parameterValue;
            binarySet = belongsToBinarySet;
            prerequisite = ParentParam;
        }
    }

    public enum Tests
    {
        Undefined,
        Materials,
        Alphas,
        PBRs,
        Samplers,
        PrimitiveAttributes
    }

    public enum ParameterName
    {
        Undefined,
        Name,
        BaseColorFactor,
        BaseColorTexture,
        MetallicFactor,
        RoughnessFactor,
        MetallicRoughnessTexture,
        PbrTextures,
        EmissiveFactor,
        AlphaMode_MASK,
        AlphaMode_BLEND,
        AlphaMode_OPAQUE,
        AlphaCutoff,
        Color_VEC3_FLOAT,
        Color_VEC4_FLOAT,
        Color_VEC3_BYTE,
        Color_VEC4_BYTE,
        Color_VEC3_SHORT,
        Color_VEC4_SHORT,
        DoubleSided,
        Sampler,
        MagFilter_NEAREST,
        MagFilter_LINEAR,
        MinFilter_NEAREST,
        MinFilter_LINEAR,
        MinFilter_NEAREST_MIPMAP_NEAREST,
        MinFilter_LINEAR_MIPMAP_NEAREST,
        MinFilter_NEAREST_MIPMAP_LINEAR,
        MinFilter_LINEAR_MIPMAP_LINEAR,
        Normal,
        Position,
        Tangent,
        TexCoord0_FLOAT,
        TexCoord0_BYTE,
        TexCoord0_SHORT,
        TexCoord1_FLOAT,
        TexCoord1_BYTE,
        TexCoord1_SHORT,
        WrapS_CLAMP_TO_EDGE,
        WrapS_MIRRORED_REPEAT,
        WrapS_REPEAT,
        WrapT_CLAMP_TO_EDGE,
        WrapT_MIRRORED_REPEAT,
        WrapT_REPEAT,
        Source,
        TexCoord,
        NormalTexture,
        OcclusionTexture,
        EmissiveTexture,
        Scale,
        Strength
    }
}
