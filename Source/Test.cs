using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Test
    {
        public TestName testType;
        public List<Property> properties;
        public List<Property> requiredProperty = null;
        public List<Runtime.Image> usedImages = new List<Runtime.Image>();
        public List<List<Property>> specialCombos = new List<List<Property>>();
        public List<List<Property>> removeCombos = new List<List<Property>>();
        public List<Property> specialProperties = new List<Property>();
        public bool onlyBinaryProperties = true;
        public bool noPrerequisite = true;
        public const string texture_Normal = "lambert2_normal.png";
        public const string texture_Emissive = "lambert2_emissive.png";
        public const string texture_BaseColor = "lambert2_baseColor.png";
        public const string texture_OcclusionRoughnessMetallic = "lambert2_occlusionRoughnessMetallic.png";
        public const string texture_AlbedoTransparency = "lambert2_AlbedoTransparency.png";
        public const string texture_SpecularSmoothness = "lambert2_SpecularSmoothness.png";

        public Test()
        {

        }

        public virtual List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            return combos;
        }
    }
    public enum TestName
    {
        Undefined,
        Material,
        Material_Alpha,
        Material_MetallicRoughness,
        Material_SpecularGlossiness,
        Texture_Sampler,
        Primitive_Attribute,
    }
}
