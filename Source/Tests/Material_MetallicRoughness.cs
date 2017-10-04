using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute()]
    class Material_MetallicRoughness : Test
    {
        public Material_MetallicRoughness()
        {
            testType = TestName.Material_MetallicRoughness;
            onlyBinaryProperties = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image occlusionRoughnessMetallicTexture = new Runtime.Image
            {
                Uri = texture_OcclusionRoughnessMetallic
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(occlusionRoughnessMetallicTexture);
            properties = new List<Property>
            {
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 0.0f, 0.0f, 0.8f)),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.MetallicFactor, 0.5f),
                new Property(Propertyname.RoughnessFactor, 0.5f),
                new Property(Propertyname.MetallicRoughnessTexture, occlusionRoughnessMetallicTexture)
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

                switch (property.name)
                {
                    case Propertyname.BaseColorFactor:
                        {
                            material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            break;
                        }
                    case Propertyname.MetallicFactor:
                        {
                            material.MetallicRoughnessMaterial.MetallicFactor = property.value;
                            break;
                        }
                    case Propertyname.RoughnessFactor:
                        {
                            material.MetallicRoughnessMaterial.RoughnessFactor = property.value;
                            break;
                        }
                    case Propertyname.BaseColorTexture:
                        {
                            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                            material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.MetallicRoughnessTexture:
                        {
                            material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                            material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = property.value;
                            break;
                        }
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
