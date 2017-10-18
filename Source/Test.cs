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
        public const string texture_Normal = "panel_normal.png";
        public const string texture_Emissive = "panel_emissive.png";
        public const string texture_BaseColor = "panel_baseColor.png";
        public const string texture_MetallicRoughness = "panel_metallicRoughness.png";
        public const string texture_Occlusion = "panel_occlusion.png";
        public const string texture_Diffuse = "panel_diffuse.png";
        public const string texture_SpecularGlossiness = "panel_specularGlossiness.png";
        public const string icon_UVspace0 = "UVspaceIcon-0.png";
        public const string icon_UVspace1 = "UVspaceIcon-1.png";

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
