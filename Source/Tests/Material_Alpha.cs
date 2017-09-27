using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    class Material_Alpha : TestValues
    {
        public Material_Alpha()
        {
            onlyBinaryAttributes = false;
            noPrerequisite = false;
            imageAttributes = new ImageAttribute[]
            {
                            new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            requiredAttributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.NormalTexture, image),
                            new Attribute(AttributeName.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                        };
            attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                            new Attribute(AttributeName.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                            new Attribute(AttributeName.AlphaCutoff, 0.2f),
                            new Attribute(AttributeName.DoubleSided, true),
                        };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.AlphaMode_Mask),
                attributes.Find(e => e.name == AttributeName.AlphaCutoff)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.AlphaMode_Mask),
                attributes.Find(e => e.name == AttributeName.DoubleSided)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.AlphaMode_Blend),
                attributes.Find(e => e.name == AttributeName.DoubleSided)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.AlphaCutoff)));
        }
    }
}
