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

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Attribute> combo)
        {
            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();

            foreach (Attribute req in requiredAttributes)
            {
                if (req.name == AttributeName.MetallicFactor)
                {
                    material.MetallicRoughnessMaterial.MetallicFactor = req.value;
                }
            }

            foreach (Attribute attribute in combo)
            {
                if (attribute.name == AttributeName.EmissiveFactor)
                {
                    material.EmissiveFactor = attribute.value;
                }
                else if (attribute.name == AttributeName.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = attribute.value;
                }
                else if (attribute.name == AttributeName.Scale && attribute.prerequisite == AttributeName.NormalTexture)
                {
                    material.NormalScale = attribute.value;
                }
                else if (attribute.name == AttributeName.OcclusionTexture)
                {
                    material.OcclusionTexture = new Runtime.Texture();
                    material.OcclusionTexture.Source = attribute.value;
                }
                else if (attribute.name == AttributeName.Strength && attribute.prerequisite == AttributeName.OcclusionTexture)
                {
                    material.OcclusionStrength = attribute.value;
                }
                else if (attribute.name == AttributeName.EmissiveTexture)
                {
                    material.EmissiveTexture = new Runtime.Texture();
                    material.EmissiveTexture.Source = attribute.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
