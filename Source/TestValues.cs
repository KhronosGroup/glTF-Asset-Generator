using System;

namespace AssetGenerator
{
    public class TestValues
    {
        public Tests testArea;
        public Parameter[] parameters;

        public TestValues(Tests testType)
        {
            testArea = testType;

            switch (testArea)
            {
                case Tests.material:
                {
                    parameters = new Parameter[]
                    {
                        new Parameter(ParameterName.BaseColorFactor, new[] { 1.0f, 0.0f, 0.0f, 0.0f }, false),
                        new Parameter(ParameterName.MetallicFactor, 0.5f, false),
                        new Parameter(ParameterName.RoughnessFactor, 0.5f, false)
                    };
                        break;
                }
                case Tests.texture:
                {
                    parameters = new Parameter[]
                    {
                        //TODO: Add texture parameters
                    };
                }
                break;
            }
        }

        public Parameter[][] ParameterCombos()
        {
            var temp = PowerSet<Parameter>(parameters);
            //TODO: Handle parameters that have more than binary values (e.g. alphaMode)
            //TODO: Remove sets that exclude a required parameter

            return temp;
        }

        //https://stackoverflow.com/questions/19890781/creating-a-power-set-of-a-sequence
        public static T[][] PowerSet<T>(T[] seq)
        {
            var powerSet = new T[1 << seq.Length][];
            powerSet[0] = new T[0]; // starting only with empty set
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

        public Parameter(ParameterName parmName, dynamic parameterValue, bool required)
        {
            name = parmName;
            value = parameterValue;
            isRequired = required;
        }
    }

    public enum Tests
    {
        material,
        texture
    }

    public enum ParameterName
    {
        BaseColorFactor,
        MetallicFactor,
        RoughnessFactor
    }
}
