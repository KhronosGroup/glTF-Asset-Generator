using System.Collections.Generic;

namespace AssetGenerator
{
    public class TestValues
    {
        public TestNames testType;
        public List<Property> properties;
        public List<Property> requiredProperty = null;
        public ImageAttribute[] imageAttributes;
        public List<List<Property>> specialCombos = new List<List<Property>>();
        public List<List<Property>> removeCombos = new List<List<Property>>();
        public bool onlyBinaryProperties = true;
        public bool noPrerequisite = true;
        public string texture = "UVmap2017.png";

        public TestValues()
        {

        }

        public dynamic InitializeTestValues(TestNames nameOfTest)
        {
            testType = nameOfTest;
            dynamic test = null;

            if (testType == TestNames.Material)
            {
                test = new Tests.Material();
            }
            else if (testType == TestNames.Material_Alpha)
            {
                test = new Tests.Material_Alpha();
            }
            else if (testType == TestNames.Material_MetallicRoughness)
            {
                test = new Tests.Material_MetallicRoughness();
            }
            else if (testType == TestNames.Primitive_Attribute)
            {
                test = new Tests.Primitive_Attribute();
            }
            else if (testType == TestNames.Texture_Sampler)
            {
                test = new Tests.Texture_Sampler();
            }

            return test;
        }
    }
    public enum TestNames
    {
        Undefined,
        Material,
        Material_Alpha,
        Material_MetallicRoughness,
        Texture_Sampler,
        Primitive_Attribute,
    }
}
