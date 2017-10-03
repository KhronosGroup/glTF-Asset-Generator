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
        public const string texture_BaseColor = "TexturePlane_BaseColor.png";
        public const string texture_Roughness = "TexturePlane_Roughness.png";
        public const string texture_Emissive = "TexturePlane_Emissive.png";
        public const string texture_Metallic = "TexturePlane_Metallic.png";
        public const string texture_Normal = "TexturePlane_Normal.png";        
        public const string texture_OcclusionRoughnessMetallic = "TexturePlane_OcclusionRoughnessMetallic.png";

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
