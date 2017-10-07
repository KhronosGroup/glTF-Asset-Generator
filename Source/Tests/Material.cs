using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Material : Test
    {
        public Material()
        {
            testType = TestName.Material;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image emissiveTexture = new Runtime.Image
            {
                Uri = texture_Emissive
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image occlusionTexture = new Runtime.Image
            {
                Uri = texture_OcclusionRoughnessMetallic
            };
            usedImages.Add(emissiveTexture);
            usedImages.Add(normalTexture);
            usedImages.Add(occlusionTexture);
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.MetallicFactor, 0.0f),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f)),
                new Property(Propertyname.EmissiveTexture, emissiveTexture),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.Scale, 2.0f, Propertyname.NormalTexture),
                new Property(Propertyname.OcclusionTexture, occlusionTexture),
                new Property(Propertyname.Strength, 0.5f, Propertyname.OcclusionTexture)
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.EmissiveFactor),
                properties.Find(e => e.name == Propertyname.EmissiveTexture)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.EmissiveTexture)));
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.MetallicFactor)
                {
                    material.MetallicRoughnessMaterial.MetallicFactor = req.value;
                }
            }

            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.EmissiveFactor:
                        {
                            material.EmissiveFactor = property.value;
                            break;
                        }
                    case Propertyname.NormalTexture:
                        {
                            material.NormalTexture = new Runtime.Texture();
                            material.NormalTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.Scale:
                        {
                            material.NormalScale = property.value;
                            break;
                        }
                    case Propertyname.OcclusionTexture:
                        {
                            material.OcclusionTexture = new Runtime.Texture();
                            material.OcclusionTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.Strength:
                        {
                            material.OcclusionStrength = property.value;
                            break;
                        }
                    case Propertyname.EmissiveTexture:
                        {
                            material.EmissiveTexture = new Runtime.Texture();
                            material.EmissiveTexture.Source = property.value;
                            break;
                        }
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
