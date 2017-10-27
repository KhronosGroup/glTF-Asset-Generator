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
            properties = new List<Property>
            {
                new Property(Propertyname.MinVersion, "2.1"),
                new Property(Propertyname.Version, "2.1", group:1),
                new Property(Propertyname.Version_Current, "2.0", group:1),
                new Property(Propertyname.ExperimentalFeature, null),
                new Property(Propertyname.ExtensionRequired, "Experimental Extension"),
                new Property(Propertyname.ModelShouldLoad_Yes, ":white_check_mark:", group:2),
                new Property(Propertyname.ModelShouldLoad_No, ":x:", group:2),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Version_Current, "2.0", group:1),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion),
                properties.Find(e => e.name == Propertyname.Version)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExtensionRequired),
                properties.Find(e => e.name == Propertyname.GlossinessFactor)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version_Current)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExperimentalFeature)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ModelShouldLoad_Yes)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ModelShouldLoad_No)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Adding a line to show the version being set in the empty set model and the extension model
            var currentVersion = specialProperties.Find(e => e.name == Propertyname.Version_Current);
            combos[0].Add(currentVersion);
            combos[3].Add(currentVersion);

            // Replace the full set with the 'Version + Fake Feature' set
            var setToAdd = ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.ExperimentalFeature));
            combos[1] = (setToAdd);

            // Show if each model is expected to load or not
            var willLoad = properties.Find(e => e.name == Propertyname.ModelShouldLoad_Yes);
            var wontLoad = properties.Find(e => e.name == Propertyname.ModelShouldLoad_No);
            combos[0].Add(willLoad);
            combos[1].Add(willLoad);
            combos[2].Add(wontLoad);
            combos[3].Add(wontLoad);

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
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
                else if (property.name == Propertyname.ExperimentalFeature)
                {
                    // Need to set a value in the .gltf somehow
                }
                else if (property.name == Propertyname.ExtensionRequired)
                {
                    // Add line to make the extension required!
                    material.MetallicRoughnessMaterial = null;
                    material.Extensions = new List<Runtime.Extensions.Extension>();
                    material.Extensions.Add(new Runtime.Extensions.MicrosoftQuantumRendering());
                    var extension = material.Extensions[0] as Runtime.Extensions.MicrosoftQuantumRendering;
                    extension.PlanckFactor = new Vector4(0.2f, 0.2f, 0.2f, 0.8f);
                    extension.CopenhagenTexture = new Runtime.Texture();
                    extension.CopenhagenTexture.Source = new Runtime.Image() { Uri = texture_Diffuse };
                    extension.EntanglementFactor = new Vector3(0.4f, 0.4f, 0.4f);
                    extension.ProbabilisticFactor = 0.3f;
                    extension.SuperpositionCollapseTexture = new Runtime.Texture();
                    extension.SuperpositionCollapseTexture.Source = new Runtime.Image() { Uri = texture_SpecularGlossiness };
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
