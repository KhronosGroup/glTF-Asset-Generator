using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Compatibility : Test
    {
        public Compatibility()
        {
            testType = TestName.Compatibility;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image emissiveTexture = new Runtime.Image
            {
                Uri = texture_Emissive
            };
            usedImages.Add(normalTexture);
            usedImages.Add(emissiveTexture);
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f),
                new Vector3( 0.0f, 0.0f,-1.0f)
            };
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f),
                new Vector4( 1.0f, 1.0f, 0.0f, 0.2f)
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.VertexNormal, planeNormals),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.VertexColor_Vector4_Float, colorCoord, group:3),
                new Property(Propertyname.EmissiveTexture, emissiveTexture),
                new Property(Propertyname.EmissiveFactor, new Vector3(1.0f, 1.0f, 1.0f)),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.MinVersion, "2.1"),
                new Property(Propertyname.Version, "2.1", group:1),
                new Property(Propertyname.Version_Current, "2.0", group:1),
                new Property(Propertyname.FakeFeature, null),
                new Property(Propertyname.ExtensionRequired, "SpecularGlossiness"),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Version_Current, "2.0", group:1),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion),
                properties.Find(e => e.name == Propertyname.Version)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version_Current)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.FakeFeature)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Adding a line to show the version being set in the empty set model
            var currentVersion = specialProperties.Find(e => e.name == Propertyname.Version_Current);
            combos[0].Add(currentVersion);

            // Replace the full set with the 'Version + Fake Feature' set
            var setToAdd = ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.FakeFeature));
            combos[1] = (setToAdd);

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            foreach (Property required in requiredProperty)
            {
                if (required.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = required.value;
                }
                else if (required.name == Propertyname.VertexColor_Vector4_Float)
                {
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = required.value;
                }
                else if (required.name == Propertyname.EmissiveTexture)
                {
                    material.EmissiveTexture = new Runtime.Texture();
                    material.EmissiveTexture.Source = required.value;
                }
                else if (required.name == Propertyname.EmissiveFactor)
                {
                    material.EmissiveFactor = required.value;
                }
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.MinVersion)
                {
                    wrapper.Asset.MinVersion = property.value;
                }
                else if (property.name == Propertyname.Version ||
                         property.name == Propertyname.Version_Current)
                {
                    wrapper.Asset.Version = property.value;
                }
                else if (property.name == Propertyname.VertexNormal)
                {
                    // Need to set a value in the .gltf somehow
                }
                else if (property.name == Propertyname.ExtensionRequired)
                {
                    material.MetallicRoughnessMaterial = null;
                    material.Extensions = new List<Runtime.Extensions.Extension>();
                    material.Extensions.Add(new Runtime.Extensions.PbrSpecularGlossiness());
                }
            }
            if (combo.Count > 0) // Don't set the material on the empty set
            {
                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;
            }

            return wrapper;
        }
    }
}
