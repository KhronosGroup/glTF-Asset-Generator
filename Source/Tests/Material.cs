﻿using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute(TestNames.Material)]
    class Material : TestValues
    {
        
        public Material()
        {
            testType = TestNames.Material;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            imageAttributes = new ImageAttribute[]
            {
                new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.MetallicFactor, 0.0f),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.EmissiveFactor, new Vector3(0.0f, 0.0f, 1.0f)),
                new Property(Propertyname.EmissiveTexture, image),
                new Property(Propertyname.NormalTexture, image),
                new Property(Propertyname.Scale, 2.0f, Propertyname.NormalTexture),
                new Property(Propertyname.OcclusionTexture, image),
                new Property(Propertyname.Strength, 0.5f, Propertyname.OcclusionTexture)
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.EmissiveFactor),
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

            foreach (Property attribute in combo)
            {
                if (attribute.name == Propertyname.EmissiveFactor)
                {
                    material.EmissiveFactor = attribute.value;
                }
                else if (attribute.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = attribute.value;
                }
                else if (attribute.name == Propertyname.Scale && attribute.prerequisite == Propertyname.NormalTexture)
                {
                    material.NormalScale = attribute.value;
                }
                else if (attribute.name == Propertyname.OcclusionTexture)
                {
                    material.OcclusionTexture = new Runtime.Texture();
                    material.OcclusionTexture.Source = attribute.value;
                }
                else if (attribute.name == Propertyname.Strength && attribute.prerequisite == Propertyname.OcclusionTexture)
                {
                    material.OcclusionStrength = attribute.value;
                }
                else if (attribute.name == Propertyname.EmissiveTexture)
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
