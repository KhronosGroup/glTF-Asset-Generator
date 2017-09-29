using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    class Material_MetallicRoughness : TestValues
    {
        public Material_MetallicRoughness()
        {
            onlyBinaryProperties = false;
            imageAttributes = new ImageAttribute[]
            {
                new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            properties = new List<Property>
            {
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                new Property(Propertyname.BaseColorTexture, image),
                new Property(Propertyname.MetallicFactor, 0.5f),
                new Property(Propertyname.RoughnessFactor, 0.5f),
                new Property(Propertyname.MetallicRoughnessTexture, image)
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture),
                properties.Find(e => e.name == Propertyname.BaseColorFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.RoughnessFactor),
                properties.Find(e => e.name == Propertyname.MetallicFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.MetallicFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.RoughnessFactor)));
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            foreach (Property property in combo)
            {
                if (material.MetallicRoughnessMaterial == null)
                {
                    material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                }

                if (property.name == Propertyname.BaseColorFactor)
                {
                    material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                }
                else if (property.name == Propertyname.MetallicFactor)
                {
                    material.MetallicRoughnessMaterial.MetallicFactor = property.value;
                }
                else if (property.name == Propertyname.RoughnessFactor)
                {
                    material.MetallicRoughnessMaterial.RoughnessFactor = property.value;
                }
                else if (property.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                }
                else if (property.name == Propertyname.MetallicRoughnessTexture)
                {
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = property.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
