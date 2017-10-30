using System.Collections.Generic;
using glTFLoader.Shared;
using Newtonsoft.Json;
using System.Reflection;

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
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(baseColorTexture);
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
                new Property(Propertyname.ExperimentalFeature_AtRoot, "At Root", group:2),
                new Property(Propertyname.ExperimentalFeature_InProperty, "In Property", group:2),
                new Property(Propertyname.ExperimentalFeature_WithFallback, "With FallBack", group:2),
                new Property(Propertyname.ExperimentalFeature_RequiresVersion, "Requires Version", group:2),
                new Property(Propertyname.ExtensionRequired, "Experimental Extension"),
                new Property(Propertyname.Sampler, "Fallback to Repeat"),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.ModelShouldLoad_Yes, ":white_check_mark:", group:3),
                new Property(Propertyname.ModelShouldLoad_No, ":x:", group:2),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Version_Current, "2.0", group:1),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_RequiresVersion),
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.MinVersion)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_WithFallback)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_InProperty)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version_Current)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_AtRoot)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_InProperty)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_WithFallback)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_RequiresVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
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
            combos[5].Add(currentVersion);

            // Replace the full set with the 'Version + Fake Feature' set
            var setToAdd = ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.ExperimentalFeature_AtRoot));
            combos[1] = (setToAdd);

            // Show if each model is expected to load or not
            var willLoad = properties.Find(e => e.name == Propertyname.ModelShouldLoad_Yes);
            var wontLoad = properties.Find(e => e.name == Propertyname.ModelShouldLoad_No);
            combos[0].Add(willLoad);
            combos[1].Add(willLoad);
            combos[2].Add(willLoad);
            combos[3].Add(willLoad);
            combos[4].Add(wontLoad);
            combos[5].Add(wontLoad);
            combos[5].Add(wontLoad);

            return combos;
        }
        //glTFLoader.Schema.Sampler.WrapSEnum.REPEAT
        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
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
                else if (property.name == Propertyname.ExtensionRequired)
                {
                    wrapper.ExtensionsRequired = new List<string>();
                    wrapper.ExtensionsRequired.Add("MicrosoftQuantumRendering");
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

        public override void PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.ExperimentalFeature_AtRoot:
                        {
                            // Add an experimental feature at the root level
                            ExperimentalGltf1 experimentalGltf = new ExperimentalGltf1(gltf);
                            experimentalGltf.Lights = new ExperimentalGltf1.Light { Color = new float[] { 0.3f, 0.4f, 0.5f } };
                            gltf = experimentalGltf;
                            break;
                        }
                    case Propertyname.ExperimentalFeature_InProperty:
                        {
                            // Add an experimental feature into an existing property
                            ExperimentalGltf1.Node experimentalNode = new ExperimentalGltf1.Node(gltf.Nodes[0]);
                            experimentalNode.Light = 0.5f;
                            gltf.Nodes[0] = experimentalNode;
                            break;
                        }
                    case Propertyname.ExperimentalFeature_WithFallback:
                        {
                            // Add an experimental feature with a fallback option
                            ExperimentalGltf2 experimentalGltf = new ExperimentalGltf2(gltf);
                            ExperimentalGltf2.Sampler fallbackSampler = new ExperimentalGltf2.Sampler(gltf.Samplers[0]);
                            ExperimentalGltf2.Sampler experimentalSampler = new ExperimentalGltf2.Sampler(gltf.Samplers[0]);
                            experimentalSampler.WrapS = ExperimentalGltf2.Sampler.WrapSEnum.QUANTUM_REPEAT;
                            experimentalGltf.Samplers = new ExperimentalGltf2.Sampler[2] {
                                fallbackSampler,
                                experimentalSampler };
                            gltf = experimentalGltf;
                            break;
                        }
                }
            }
        }
    }

    public class ExperimentalGltf1 : glTFLoader.Schema.Gltf
    {
        public ExperimentalGltf1() { }
        public ExperimentalGltf1(glTFLoader.Schema.Gltf parent)
        {
            foreach (PropertyInfo property in parent.GetType().GetProperties())
            {
                var parentProperty = property.GetValue(parent);
                if (parentProperty != null)
                {
                    property.SetValue(this, parentProperty);
                }
            }
        }
        public Light Lights { get; set; }
        public class Light
        {
            public Light()
            {

            }

            [JsonConverter(typeof(ArrayConverter))]
            [JsonProperty("color")]
            public float[] Color { get; set; }
        }

        public class Node : glTFLoader.Schema.Node
        {
            public Node(glTFLoader.Schema.Node parent)
            {
                foreach (PropertyInfo property in parent.GetType().GetProperties())
                {
                    var parentProperty = property.GetValue(parent);
                    if (parentProperty != null)
                    {
                        property.SetValue(this, parentProperty);
                    }
                }
            }

            [JsonConverter(typeof(ArrayConverter))]
            [JsonProperty("light")]
            public float Light { get; set; }
        }
    }

    public class ExperimentalGltf2 : glTFLoader.Schema.Gltf
    {
        public ExperimentalGltf2() { }
        public ExperimentalGltf2(glTFLoader.Schema.Gltf parent)
        {
            foreach (PropertyInfo property in parent.GetType().GetProperties())
            {
                var parentProperty = property.GetValue(parent);
                if (parentProperty != null)
                {
                    property.SetValue(this, parentProperty);
                }
            }
        }

        public class Sampler : glTFLoader.Schema.Sampler
        {
            public Sampler(glTFLoader.Schema.Sampler parent)
            {
                foreach (PropertyInfo property in parent.GetType().GetProperties())
                {
                    var parentProperty = property.GetValue(parent);
                    if (parentProperty != null)
                    {
                        property.SetValue(this, parentProperty);
                    }
                }
            }

            new public WrapSEnum WrapS { get; set; }

            new public enum WrapSEnum
            {
                REPEAT = 10497,
                CLAMP_TO_EDGE = 33071,
                MIRRORED_REPEAT = 33648,
                QUANTUM_REPEAT = 34225
            }
        }
    }
}
