using System;
using System.Collections.Generic;
using System.Linq;
using static AssetGenerator.GLTFWrapper;


namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public List<Parameter> parameters;
        public Parameter[] requiredParameters;
        public ImageAttribute[] imageAttributes;
        bool onlyBinaryParams = true;
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
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.Name, "name", false),
                            new Parameter(ParameterName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f), false),
                            new Parameter(ParameterName.AlphaMode_MASK, glTFLoader.Schema.Material.AlphaModeEnum.MASK, false, 1),
                            new Parameter(ParameterName.AlphaMode_BLEND, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, false, 1),
                            new Parameter(ParameterName.AlphaMode_OPAQUE, glTFLoader.Schema.Material.AlphaModeEnum.OPAQUE, false, 1),
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
                        parameters = new List<Parameter>
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
                        // The base glTF spec does not support mipmapping, so the MagFilter and MinFilter 
                        // attributes will have no visible affect unless mipmapping is implemented by the client
                        noPrerequisite = false;
                        imageAttributes = new ImageAttribute[]
                        {
                            new ImageAttribute("UVmap2017.png")
                        };
                        GLTFImage image = new GLTFImage
                        {
                            uri = "UVmap2017.png"
                        };
                        requiredParameters = new Parameter[]
                        {
                            new Parameter(ParameterName.Source, image, true),
                            new Parameter(ParameterName.TexCoord, 0, true),
                            new Parameter(ParameterName.Name, "name", true),
                            new Parameter(ParameterName.Sampler, 0, true),
                        };
                        parameters = new List<Parameter>
                        {
                            new Parameter(ParameterName.MagFilter_NEAREST, glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST, false, 1),
                            new Parameter(ParameterName.MagFilter_LINEAR, glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR, false, 1),
                            new Parameter(ParameterName.MinFilter_NEAREST, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST, false, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR, false, 2),
                            new Parameter(ParameterName.MinFilter_NEAREST_MIPMAP_NEAREST, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST, false, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR_MIPMAP_NEAREST, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST, false, 2),
                            new Parameter(ParameterName.MinFilter_NEAREST_MIPMAP_LINEAR, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR, false, 2),
                            new Parameter(ParameterName.MinFilter_LINEAR_MIPMAP_LINEAR, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR, false, 2),
                            new Parameter(ParameterName.WrapS_CLAMP_TO_EDGE, glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE, false, 3),
                            new Parameter(ParameterName.WrapS_MIRRORED_REPEAT, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT, false, 3),
                            new Parameter(ParameterName.WrapS_REPEAT, glTFLoader.Schema.Sampler.WrapSEnum.REPEAT, false, 3),
                            new Parameter(ParameterName.WrapT_CLAMP_TO_EDGE, glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE, false, 4),
                            new Parameter(ParameterName.WrapT_MIRRORED_REPEAT, glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT, false, 4),
                            new Parameter(ParameterName.WrapT_REPEAT, glTFLoader.Schema.Sampler.WrapTEnum.REPEAT, false, 4),
                        };
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
            List<ParameterName> isPrerequisite = new List<ParameterName>();
            bool prereqParam;

            //var combos = PowerSet<Parameter>(parameters);
            var combos = BasicSet<Parameter>(parameters);

            // Removes sets that duplicate binary entries for a single parameter (e.g. alphaMode)
            // Removes sets where an attribute is missing a required parameter
            if (onlyBinaryParams == false || noPrerequisite == false )
            {
                // Are there any prerequisite? 
                prereqParam = isPrerequisite.Any();

                // Makes a list of combos to remove
                //foreach (var combo in combos)
                int combosCount = combos.Count();
                for (int x = 1; x < combosCount; x++) // Skip the first combo
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
                                if (param.name == prereq)
                                {
                                    usedPrerequisite.Add(prereq);
                                }
                            }
                        }
                        usedPrereq = usedPrerequisite.Any();
                    }

                    foreach (var param in combos[x])
                    {
                        if (param.binarySet > 0)
                        {
                            if (binarySets.Contains(param.binarySet)) // Remove combos that have multiple of the same binary combo
                            {
                                removeTheseCombos.Add(combos[x]);
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
                //finalResult = keepTheseCombos.ToArray();
                finalResult = keepTheseCombos;
            }
            else
            {
                // If there are only binary parameters, we don't need to check for duplicates
                finalResult = combos;
            }

            return finalResult;
        }

        /// <summary>
        /// Given a,b,c this returns [a,b,c], [a], [b], [c]
        /// </summary>
        /// <param name="seq"></param>
        /// <returns>List of lists containing a basic set of combinations.</returns>
        public static List<List<T>> BasicSet<T>(List<T> seq)
        {
            var basicSet = new List<List<T>>();
            basicSet.Add(new List<T>()); // Will contain the full set
            foreach (var x in seq)
            {
                basicSet[0].Add(x);
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

        public string GenerateName(List<Parameter> paramSet)
        {
            string name = null;

            for (int i = 0; i < paramSet.Count; i++)
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
        public dynamic value; // Could be a float, array of floats, string, or enum
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
        AlphaMode_OPAQUE,
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
