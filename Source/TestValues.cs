using System;
using System.Collections.Generic;
using System.Linq;
using static AssetGenerator.GLTFWrapper;

namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public Parameter[] parameters;
        public Parameter[] requiredParameters;
        public ImageAttribute[] imageAttributes;
        bool onlyBinaryParams = true;
        //bool noRequiredParams = true;
        bool noPrerequisite = true;

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
                            new ImageAttribute("green.png")
};
                        GLTFImage image = new GLTFImage
                        {
                            uri = "green.png"
                        };
                        parameters = new Parameter[]
                        {
                            new Parameter(ParameterName.Name, "name", false),
                            new Parameter(ParameterName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f), false),
                            new Parameter(ParameterName.AlphaMode_MASK, glTFLoader.Schema.Material.AlphaModeEnum.MASK, false, 1),
                            new Parameter(ParameterName.AlphaMode_BLEND, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, false, 1),
                            new Parameter(ParameterName.AlphaCutoff, 0.2f, false),
                            new Parameter(ParameterName.DoubleSided, true, false),
                            new Parameter(ParameterName.NormalTexture, null, false),
                            new Parameter(ParameterName.Source, image, false, ParameterName.NormalTexture),
                            new Parameter(ParameterName.TexCoord, 0, false, ParameterName.NormalTexture),
                            new Parameter(ParameterName.Scale, 2.0f, false, ParameterName.NormalTexture),
                            new Parameter(ParameterName.OcclusionTexture, null, false),
                            new Parameter(ParameterName.Source, image, false, ParameterName.OcclusionTexture),
                            new Parameter(ParameterName.TexCoord, 0, false, ParameterName.OcclusionTexture),
                            new Parameter(ParameterName.Strength, 0.5f, false, ParameterName.OcclusionTexture),
                            new Parameter(ParameterName.EmissiveTexture, null, false),
                            new Parameter(ParameterName.Source, image, false, ParameterName.EmissiveTexture),
                            new Parameter(ParameterName.TexCoord, 0, false, ParameterName.EmissiveTexture)
                        };
                        break;
                    }
                case Tests.PbrMetallicRoughness:
                    {
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute("green.png")
                        };
                        GLTFImage image = new GLTFImage
                        {
                            uri = "green.png"
                        };
                        parameters = new Parameter[]
                        {
                            new Parameter(ParameterName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.0f), false),
                            new Parameter(ParameterName.MetallicFactor, 0.5f, false),
                            new Parameter(ParameterName.RoughnessFactor, 0.5f, false),
                            new Parameter(ParameterName.BaseColorTexture, null, false),
                            new Parameter(ParameterName.Source, image, false, ParameterName.BaseColorTexture),
                            new Parameter(ParameterName.Sampler, 0, false, ParameterName.BaseColorTexture),
                            new Parameter(ParameterName.TexCoord, 0, false, ParameterName.BaseColorTexture),
                            new Parameter(ParameterName.Name, "name", false, ParameterName.BaseColorTexture),
                            new Parameter(ParameterName.MetallicRoughnessTexture, null, false),
                            new Parameter(ParameterName.Source, image, false, ParameterName.MetallicRoughnessTexture),
                            new Parameter(ParameterName.Sampler, 0, false, ParameterName.MetallicRoughnessTexture),
                            new Parameter(ParameterName.TexCoord, 0, false, ParameterName.MetallicRoughnessTexture),
                            new Parameter(ParameterName.Name, "name", false, ParameterName.MetallicRoughnessTexture)
                        };
                        break;
                    }
                case Tests.Sampler:
                    {
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute("green.png")
                        };
                        GLTFImage image = new GLTFImage
                        {
                            uri = "green.png"
                        };
                        requiredParameters = new Parameter[]
                        {
                            new Parameter(ParameterName.Source, image, true),
                            new Parameter(ParameterName.TexCoord, 0, true),
                            new Parameter(ParameterName.Name, "name", true),
                            new Parameter(ParameterName.Sampler, 0, true),
                        };
                        parameters = new Parameter[]
                        {
                            new Parameter(ParameterName.MagFilter_NEAREST, 9728, false, 1),
                            new Parameter(ParameterName.MagFilter_LINEAR, 9729, false, 1),
                            new Parameter(ParameterName.MinFilter_NEAREST, 9728, false, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR, 9729, false, 2),
                            new Parameter(ParameterName.MinFilter_NEAREST_MIPMAP_NEAREST, 9984, false, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR_MIPMAP_NEAREST, 9985, false, 2),
                            new Parameter(ParameterName.MinFilter_NEAREST_MIPMAP_LINEAR, 9986, false, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR_MIPMAP_LINEAR, 9987, false, 2),
                            new Parameter(ParameterName.WrapS_CLAMP_TO_EDGE, 33071, false, 3),
                            new Parameter(ParameterName.WrapS_MIRRORED_REPEAT, 33648, false, 3),
                            new Parameter(ParameterName.WrapS_REPEAT, 10497, false, 3),
                            new Parameter(ParameterName.WrapT_CLAMP_TO_EDGE, 33071, false, 3),
                            new Parameter(ParameterName.WrapT_MIRRORED_REPEAT, 33648, false, 3),
                            new Parameter(ParameterName.WrapT_REPEAT, 10497, false, 3),
                        };
                        break;
                    }
            }
        }

        public Parameter[][] ParameterCombos()
        {
            Parameter[][] finalResult;
            List<Parameter[]> removeTheseCombos = new List<Parameter[]>();
            List<Parameter[]> keepTheseCombos = new List<Parameter[]>();
            List<Parameter> isRequired = new List<Parameter>();
            List<ParameterName> isPrerequisite = new List<ParameterName>();
            bool reqParam;
            bool prereqParam;
            var combos = PowerSet<Parameter>(parameters);

            // Removes sets that exclude a required parameter
            // Removes sets that duplicate binary entries for a single parameter (e.g. alphaMode)
            if (onlyBinaryParams == false || noPrerequisite == false )//|| noRequiredParams == false)
            {
                // Makes a list of required parameters
                //foreach (var param in parameters)
                //{
                //    if (param.prerequisite != ParameterName.Undefined)
                //    {
                //        if (!isPrerequisite.Contains(param.prerequisite))
                //        {
                //            isPrerequisite.Add(param.prerequisite);
                //        }
                //    }
                //    else if (param.isRequired == true)
                //    {
                //        isRequired.Add(param);
                //    }
                //}

                // Are there any prerequisite or required parameters? 
                prereqParam = isPrerequisite.Any();
                //reqParam = isRequired.Any();

                // Makes a list of combos to remove
                foreach (var combo in combos)
                {
                   // int reqParamCount = 0;
                    bool usedPrereq = false;
                    List<int> binarySets = new List<int>();
                    List<ParameterName> usedPrerequisite = new List<ParameterName>();

                    // Makes a list of each prerequisite parameter in the current combo
                    if (prereqParam == true)
                    {
                        foreach (var prereq in isPrerequisite)
                        {
                            foreach (var param in combo)
                            {
                                if (param.name == prereq)
                                {
                                    usedPrerequisite.Add(prereq);
                                }
                            }
                        }
                        usedPrereq = usedPrerequisite.Any();
                    }

                    foreach (var param in combo)
                    {
                        if (param.binarySet > 0)
                        {
                            if (binarySets.Contains(param.binarySet)) // Remove combos that have multiple of the same binary combo
                            {
                                removeTheseCombos.Add(combo);
                                break;
                            }
                            else
                            {
                                binarySets.Add(param.binarySet);
                            }
                        }
                        if (usedPrereq == true && param.prerequisite != ParameterName.Undefined) // Removes combos that have a parameter missing a prerequisite
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
                                removeTheseCombos.Add(combo);
                                break;
                            }
                        }
                        else if (usedPrereq == false && param.prerequisite != ParameterName.Undefined)
                        {
                            removeTheseCombos.Add(combo);
                            break;
                        }
                        //if (reqParam == true && param.isRequired == true)
                        //{
                        //    reqParamCount++;
                        //}
                    }
                    //if (reqParam == true && combo.Any() == true && reqParamCount < isRequired.Count()) // Remove combos if they are missing a required parameter
                    //{
                    //    removeTheseCombos.Add(combo);
                    //}
                }

                // Uses the list of bad combos to trim down the original power set
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
                finalResult = keepTheseCombos.ToArray();
            }
            else
            {
                // If there are only binary parameters, we don't need to check for duplicates
                finalResult = combos;
            }

            return finalResult;
        }

        //https://stackoverflow.com/questions/19890781/creating-a-power-set-of-a-sequence
        public static T[][] PowerSet<T>(T[] seq)
        {
            var powerSet = new T[1 << seq.Length][];
            powerSet[0] = new T[0]; // starting only with empty setL
            for (int i = 0; i < seq.Length; i++)
            {
                var cur = seq[i];
                int count = 1 << i; // doubling list each time
                for (int j = 0; j < count; j++)
                {
                    var source = powerSet[j];
                    var destination = powerSet[count + j] = new T[source.Length + 1];
                    for (int q = 0; q < source.Length; q++)
                        destination[q] = source[q];
                    destination[source.Length] = cur;
                }
            }
            return powerSet;
        }

        public string GenerateName(Parameter[] paramSet)
        {
            string name = null;

            for (int i = 0; i < paramSet.Length; i++)
            {
                if (name == null)
                {
                    name += paramSet[i].name;
                }
                else
                {
                    name += " - " + paramSet[i].name;
                }
            }
            if (name == null)
            {
                name = "NoParametersSet";
            }

            return name;
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
        public dynamic value; // Could be a float, array of floats, or string
        public bool isRequired;
        public ParameterName prerequisite = ParameterName.Undefined;
        public int binarySet;

        public Parameter(ParameterName parmName, dynamic parameterValue, bool required)
        {
            name = parmName;
            value = parameterValue;
            isRequired = required;
            binarySet = 0;
        }

        public Parameter(ParameterName parmName, dynamic parameterValue, bool required, ParameterName ParentParam)
        {
            name = parmName;
            value = parameterValue;
            isRequired = required;
            binarySet = 0;
            prerequisite = ParentParam;
        }

        public Parameter(ParameterName parmName, dynamic parameterValue, bool required, int belongsToBinarySet)
        {
            name = parmName;
            value = parameterValue;
            isRequired = required;
            binarySet = belongsToBinarySet;
        }
    }

    public enum Tests
    {
        Undefined,
        Materials,
        PbrMetallicRoughness,
        Sampler
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
        AlphaCutoff,
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
