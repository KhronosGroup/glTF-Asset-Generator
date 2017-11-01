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
                new Property(Propertyname.SimulatedFeature_AtRoot, "Light Object added At Root", group:2),
                new Property(Propertyname.SimulatedFeature_InProperty, "Light Property Added to Node Object", group:2),
                new Property(Propertyname.SimulatedFeature_WithFallback, "Alpha Mode updated with a new Enum value, and a FallBack value", group:2),
                new Property(Propertyname.SimulatedFeature_RequiresVersion, "Requires a specific Version or higher", group:2),
                new Property(Propertyname.SimulatedFeature_ExtensionRequired, "Extension Required", group:2),
                new Property(Propertyname.ModelShouldLoad_Yes, ":white_check_mark:", group:3),
                new Property(Propertyname.ModelShouldLoad_No, ":x:", group:2),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Version_Current, "2.0", group:1),
                new Property(Propertyname.Sampler, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SimulatedFeature_RequiresVersion),
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.MinVersion)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.SimulatedFeature_WithFallback),
                specialProperties.Find(e => e.name == Propertyname.Sampler),
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.SimulatedFeature_InProperty)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version_Current)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SimulatedFeature_AtRoot)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SimulatedFeature_InProperty)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SimulatedFeature_WithFallback)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SimulatedFeature_RequiresVersion)));
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
                properties.Find(e => e.name == Propertyname.SimulatedFeature_AtRoot));
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

            return combos;
        }

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
                else if (property.name == Propertyname.SimulatedFeature_ExtensionRequired)
                {
                    wrapper.ExtensionsRequired = new List<string>();
                    wrapper.ExtensionsRequired.Add("EXT_QuantumRendering");
                    material.MetallicRoughnessMaterial = null;
                    material.Extensions = new List<Runtime.Extensions.Extension>();
                    material.Extensions.Add(new Runtime.Extensions.EXT_QuantumRendering());
                    var extension = material.Extensions[0] as Runtime.Extensions.EXT_QuantumRendering;
                    extension.PlanckFactor = new Vector4(0.2f, 0.2f, 0.2f, 0.8f);
                    extension.CopenhagenTexture = new Runtime.Texture();
                    extension.EntanglementFactor = new Vector3(0.4f, 0.4f, 0.4f);
                    extension.ProbabilisticFactor = 0.3f;
                    extension.SuperpositionCollapseTexture = new Runtime.Texture();
                }
                else if (property.name == Propertyname.SimulatedFeature_WithFallback)
                {
                    // Fallback sampler
                    var sampler = specialProperties.Find(e => e.name == Propertyname.Sampler);
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapS = sampler.value;

                    // BaseColorTexture
                    var baseColorTexture = specialProperties.Find(e => e.name == Propertyname.BaseColorTexture);
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = baseColorTexture.value;
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
                    case Propertyname.SimulatedFeature_AtRoot:
                        {
                            // Add an experimental feature at the root level
                            ExperimentalGltf1 experimentalGltf = new ExperimentalGltf1(gltf);
                            experimentalGltf.lights = new ExperimentalGltf1.Light { Color = new float[] { 0.3f, 0.4f, 0.5f } };
                            gltf = experimentalGltf;
                            break;
                        }
                    case Propertyname.SimulatedFeature_InProperty:
                        {
                            // Add an experimental feature into an existing property
                            ExperimentalGltf1.Node experimentalNode = new ExperimentalGltf1.Node(gltf.Nodes[0]);
                            experimentalNode.Light = 0.5f;
                            gltf.Nodes[0] = experimentalNode;
                            break;
                        }
                    case Propertyname.SimulatedFeature_WithFallback:
                        {
                            // Add an experimental feature
                            ExperimentalGltf2 experimentalGltf = new ExperimentalGltf2(gltf);
                            ExperimentalGltf2.Sampler fallbackSampler = new ExperimentalGltf2.Sampler(gltf.Samplers[0]);
                            ExperimentalGltf2.Sampler experimentalSampler = new ExperimentalGltf2.Sampler(gltf.Samplers[0]);
                            experimentalSampler.WrapS = ExperimentalGltf2.Sampler.WrapSEnum.QUANTUM_REPEAT;
                            experimentalSampler.WrapT = ExperimentalGltf2.Sampler.WrapTEnum.REPEAT;
                            fallbackSampler.WrapS = ExperimentalGltf2.Sampler.WrapSEnum.MIRRORED_REPEAT;
                            fallbackSampler.WrapT = ExperimentalGltf2.Sampler.WrapTEnum.REPEAT;

                            experimentalGltf.Samplers = new ExperimentalGltf2.Sampler[2] {
                                fallbackSampler,
                                experimentalSampler };

                            // Fallback option
                            ExperimentalGltf2.Texture experimentalTexture = new ExperimentalGltf2.Texture(gltf.Textures[0]);
                            experimentalTexture.AdditionalSamplers = new int[] { 1 };
                            experimentalGltf.Textures[0] = experimentalTexture;

                            gltf = experimentalGltf;
                            break;
                        }
                }
            }
        }
    }

    // Used to add a property to the root level, or into an existing property
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

        // Creates a new root level property
        public Light lights { get; set; }
        public class Light
        {
            public Light()
            {

            }

            [JsonConverter(typeof(ArrayConverter))]
            [JsonProperty("color")]
            public float[] Color { get; set; }
        }

        // Insert a feature into an existing property
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

    // Used to add a new enum into an existing property with a fallback option
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

        // Experimental enum
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

            [JsonConverter(typeof(ArrayConverter))]
            [JsonProperty("wrapS")]
            new public WrapSEnum WrapS { get; set; }

            new public enum WrapSEnum
            {
                REPEAT = 10497,
                CLAMP_TO_EDGE = 33071,
                MIRRORED_REPEAT = 33648,
                QUANTUM_REPEAT = 34225
            }
        }

        // Fallback option
        public class Texture : glTFLoader.Schema.Texture
        {
            public Texture(glTFLoader.Schema.Texture parent)
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
            [JsonProperty("additionalSamplers")]
            public int[] AdditionalSamplers { get; set; }
        }
    }
}
