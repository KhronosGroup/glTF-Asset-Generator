using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    class Material_MetallicRoughness : TestValues
    {
        public Material_MetallicRoughness()
        {
            onlyBinaryAttributes = false;
            imageAttributes = new ImageAttribute[]
            {
                            new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                            new Attribute(AttributeName.BaseColorTexture, image),
                            new Attribute(AttributeName.MetallicFactor, 0.5f),
                            new Attribute(AttributeName.RoughnessFactor, 0.5f),
                            new Attribute(AttributeName.MetallicRoughnessTexture, image)
                        };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.BaseColorTexture),
                attributes.Find(e => e.name == AttributeName.BaseColorFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                attributes.Find(e => e.name == AttributeName.RoughnessFactor),
                attributes.Find(e => e.name == AttributeName.MetallicFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                attributes.Find(e => e.name == AttributeName.MetallicFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.MetallicRoughnessTexture),
                attributes.Find(e => e.name == AttributeName.RoughnessFactor)));
        }
    }
}
