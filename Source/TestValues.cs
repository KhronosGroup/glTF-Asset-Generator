using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using glTFLoader.Schema;
using static AssetGenerator.GLTFWrapper;

namespace AssetGenerator
{
    public class TestValues
    {
        public TestValues()
        {

        }

        public Parameter[][] ParameterCombos()
        {
            //List<Parameter> parameters = new List<Parameter>
            Parameter[] parameters =
            {
                new Parameter("BaseColorFactor", new[] { 1.0f, 0.0f, 0.0f, 0.0f }),
                new Parameter("BetallicFactor", 0.5f),
                new Parameter("RoughnessFactor", 0.5f)
            };

            var temp = PowerSet<Parameter>(parameters);

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
                name = "None";
            }

            return name;
        }
    }

    public class Parameter
    {
        public string name { get; }
        public dynamic value; // Could be a float, array of floats, or string
        public bool required;

        public Parameter(string parmName, dynamic parameterValue)
        {
            name = parmName;
            value = parameterValue;
            required = false;
        }
    }
}
