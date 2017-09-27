using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    class Material : TestValues
    {
        public Material()
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
                            new Attribute(AttributeName.MetallicFactor, 0.0f),
                        };
            attributes = new List<Attribute>
                        {
                            new Attribute(AttributeName.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f)),
                            new Attribute(AttributeName.EmissiveTexture, image),
                            new Attribute(AttributeName.NormalTexture, image),
                            new Attribute(AttributeName.Scale, 2.0f, AttributeName.NormalTexture),
                            new Attribute(AttributeName.OcclusionTexture, image),
                            new Attribute(AttributeName.Strength, 0.5f, AttributeName.OcclusionTexture)
                        };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                attributes.Find(e => e.name == AttributeName.EmissiveFactor),
                attributes.Find(e => e.name == AttributeName.EmissiveTexture)));
        }
    }
}
