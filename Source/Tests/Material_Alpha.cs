using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute(TestNames.Material_Alpha),
        ImageAttribute(texture_Normal)]
    class Material_Alpha : TestValues
    { 
        public Material_Alpha()
        {
            testType = TestNames.Material_Alpha;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                new Property(Propertyname.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                new Property(Propertyname.AlphaCutoff, 0.2f),
                new Property(Propertyname.DoubleSided, true),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.AlphaCutoff)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.DoubleSided)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend),
                properties.Find(e => e.name == Propertyname.DoubleSided)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaCutoff)));
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
            material.NormalTexture = new Runtime.Texture();

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.BaseColorFactor)
                {
                    material.MetallicRoughnessMaterial.BaseColorFactor = req.value;
                }
                else if (req.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture.Source = req.value;
                }
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.AlphaMode_Opaque ||
                    property.name == Propertyname.AlphaMode_Mask ||
                    property.name == Propertyname.AlphaMode_Blend)
                {
                    material.AlphaMode = property.value;
                }
                else if (property.name == Propertyname.AlphaCutoff)
                {
                    material.AlphaCutoff = property.value;
                }
                else if (property.name == Propertyname.DoubleSided)
                {
                    material.DoubleSided = property.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
