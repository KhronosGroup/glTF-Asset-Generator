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
