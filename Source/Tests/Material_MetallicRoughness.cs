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

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Attribute> combo)
        {
            foreach (Attribute attribute in combo)
            {
                if (material.MetallicRoughnessMaterial == null)
                {
                    material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                }

                if (attribute.name == AttributeName.BaseColorFactor)
                {
                    material.MetallicRoughnessMaterial.BaseColorFactor = attribute.value;
                }
                else if (attribute.name == AttributeName.MetallicFactor)
                {
                    material.MetallicRoughnessMaterial.MetallicFactor = attribute.value;
                }
                else if (attribute.name == AttributeName.RoughnessFactor)
                {
                    material.MetallicRoughnessMaterial.RoughnessFactor = attribute.value;
                }
                else if (attribute.name == AttributeName.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = attribute.value;
                }
                else if (attribute.name == AttributeName.MetallicRoughnessTexture)
                {
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = attribute.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
