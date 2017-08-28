using System;
using System.Collections.Generic;
using System.Linq;
namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public Parameter[] parameters;
        public ImageAttribute[] imageAttributes;
        bool onlyBinaryParams = true;
        bool noRequiredParams = true;

        public TestValues(Tests testType)
        {
            testArea = testType;

            switch (testArea)
            {
                case Tests.materials:
                    {
                        onlyBinaryParams = false;
                        parameters = new Parameter[]
                        {
                            new Parameter(ParameterName.Name, "name", false),
                            new Parameter(ParameterName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f), false),
                            new Parameter(ParameterName.AlphaMode_MASK, glTFLoader.Schema.Material.AlphaModeEnum.MASK, false, 1),
                            new Parameter(ParameterName.AlphaMode_BLEND, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, false, 1),
                            new Parameter(ParameterName.AlphaCutoff, 0.2f, false),
                            new Parameter(ParameterName.DoubleSided, true, false)
                        };
                        break;
                    }
                case Tests.pbrMetallicRoughness:
                    {
                        parameters = new Parameter[]
                        {
                        new Parameter(ParameterName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.0f), false),
                        new Parameter(ParameterName.MetallicFactor, 0.5f, false),
                        new Parameter(ParameterName.RoughnessFactor, 0.5f, false)
                        };
                        break;
                    }
                case Tests.BaseColorTexture:
                    {
                        parameters = new Parameter[]
                        {
                            new Parameter(ParameterName.Index, 0, true),
                            new Parameter(ParameterName.Source, 0, false),
                            new Parameter(ParameterName.Sampler, 0, false),
                            new Parameter(ParameterName.TexCoord, 0, false),
                            new Parameter(ParameterName.Name, "name", false),                            
                        };
                    }
                    break;
            }
        }

        public Parameter[][] ParameterCombos()
        {
            Parameter[][] finalResult;
            List<Parameter[]> removeTheseCombos = new List<Parameter[]>();
            List<Parameter[]> keepTheseCombos = new List<Parameter[]>();
            List<Parameter> requiredParameters = new List<Parameter>();
            bool reqParam;
            var combos = PowerSet<Parameter>(parameters);

            // Removes sets that exclude a required parameter
            // Removes sets that duplicate binary entries for a single parameter (e.g. alphaMode)
            if (onlyBinaryParams == false || noRequiredParams == false)
            {
                // Makes a list of required parameters
                foreach (var param in parameters)
                {
                    if (param.isRequired == true)
                    {
                        requiredParameters.Add(param);
                    }
                }

                // Are there any required parameters?
                reqParam = requiredParameters.Any();

                // Makes a list of combos to remove
                foreach (var combo in combos)
                {
                    List<int> binarySets = new List<int>();
                    int reqParamCount = 0;
                    foreach (var param in combo)
                    {
                        if (param.binarySet > 0)
                        {
                            if (binarySets.Contains(param.binarySet))
                            {
                                removeTheseCombos.Add(combo);
                                break;
                            }
                            else
                            {
                                binarySets.Add(param.binarySet);
                            }
                        }
                        if (reqParam == true && param.isRequired == true)
                        {
                            reqParamCount++;
                        }
                    }
                    if (reqParam == true && combo.Any() == true && reqParamCount < requiredParameters.Count())
                    {
                        removeTheseCombos.Add(combo);
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
                name += paramSet[i].name;
            }

            if (name == null)
            {
                name = "NoParametersSet";
            }

            return name;
        }
    }

    public class Parameter
    {
        public ParameterName name { get; }
        public dynamic value; // Could be a float, array of floats, or string
        public bool isRequired;
        public int binarySet;

        public Parameter(ParameterName parmName, dynamic parameterValue, bool required)
        {
            name = parmName;
            value = parameterValue;
            isRequired = required;
            binarySet = 0;
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
        materials,
        BaseColorTexture,
        pbrMetallicRoughness,
        pbrTextures,
        texture
    }

    public enum ParameterName
    {
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
        Source,
        NormalTexture,
        OcclusionTexture,
        EmissiveTexture,
        Index,
        TexCoord,
        Scale,
        Strength
    }
}
