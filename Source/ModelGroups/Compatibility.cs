using System.Collections.Generic;
using glTFLoader.Shared;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Converters;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Compatibility : ModelGroup
    {
        public Compatibility(List<string> figures) : base(figures)
        {
            modelGroupName = ModelGroupName.Compatibility;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            properties = new List<Property>
            {
                new Property(Propertyname.MinVersion, "2.1"),
                new Property(Propertyname.Version, "2.1", group:1),
                new Property(Propertyname.Version_Current, "2.0", group:1),
                new Property(Propertyname.Description_AtRoot, "Light object added at root", group:2),
                new Property(Propertyname.Description_InProperty, "Light property added to node object", group:2),
                new Property(Propertyname.Description_WithFallback, "Alpha mode updated with a new enum value, and a fallback value", group:2),
                new Property(Propertyname.Description_RequiresVersion, "Requires a specific version or higher", group:2),
                new Property(Propertyname.Description_ExtensionRequired, "Extension required", group:2),
                new Property(Propertyname.ModelShouldLoad_InCurrent, ":white_check_mark:", group:3),
                new Property(Propertyname.ModelShouldLoad_InFuture, "Only in version 2.1 or higher", group:3),
                new Property(Propertyname.ModelShouldLoad_No, ":x:", group:3),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Description_RequiresVersion),
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.MinVersion)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.Description_WithFallback),
                specialProperties.Find(e => e.name == Propertyname.AlphaMode_Blend)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.Description_InProperty)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MinVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version_Current)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Description_AtRoot)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Description_InProperty)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Description_WithFallback)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Description_RequiresVersion)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ModelShouldLoad_InCurrent)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ModelShouldLoad_InFuture)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.ModelShouldLoad_No)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Adding a line to show the version being set in the empty set model and the extension model
            var currentVersion = properties.Find(e => e.name == Propertyname.Version_Current);
            combos[0].Add(currentVersion);
            combos[5].Add(currentVersion);

            // Replace the full set with the 'Version + Fake Feature' set
            var setToAdd = ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Version),
                properties.Find(e => e.name == Propertyname.Description_AtRoot));
            combos[1] = (setToAdd);

            // Show if each model is expected to load or not
            var willLoad = properties.Find(e => e.name == Propertyname.ModelShouldLoad_InCurrent);
            var willLoadInFuture = properties.Find(e => e.name == Propertyname.ModelShouldLoad_InFuture);
            var wontLoad = properties.Find(e => e.name == Propertyname.ModelShouldLoad_No);
            combos[0].Add(willLoad);
            combos[1].Add(willLoad);
            combos[2].Add(willLoad);
            combos[3].Add(willLoad);
            combos[4].Add(willLoadInFuture);
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
                else if (property.name == Propertyname.Description_ExtensionRequired)
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
                else if (property.name == Propertyname.Description_WithFallback)
                {
                    // Fallback alpha mode will be set in the PostRuntimeChanges function
                }
            }

            if (combo.Count > 0) // Don't set the material on the empty set
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
            }

            return wrapper;
        }

        public override void PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.Description_AtRoot:
                        {
                            // Add an simulated feature at the root level
                            ExperimentalGltf1 experimentalGltf = new ExperimentalGltf1(gltf);
                            experimentalGltf.lights = new ExperimentalGltf1.Light { Color = new float[] { 0.3f, 0.4f, 0.5f } };
                            gltf = experimentalGltf;
                            break;
                        }
                    case Propertyname.Description_InProperty:
                        {
                            // Add an simulated feature into an existing property
                            ExperimentalGltf1.Node experimentalNode = new ExperimentalGltf1.Node(gltf.Nodes[0]);
                            experimentalNode.Light = 0.5f;
                            gltf.Nodes[0] = experimentalNode;
                            break;
                        }
                    case Propertyname.Description_WithFallback:
                        {
                            // Add an simulated feature with a fallback option
                            ExperimentalGltf2 experimentalGltf = new ExperimentalGltf2(gltf);
                            ExperimentalGltf2.Material simulatedMaterial = new ExperimentalGltf2.Material(gltf.Materials[0]);
                            var alphaModeFallback = specialProperties.Find(e => e.name == Propertyname.AlphaMode_Blend);
                            simulatedMaterial.AlphaMode = alphaModeFallback.value;
                            simulatedMaterial.AlphaMode2 = ExperimentalGltf2.Material.AlphaModeEnum.QUANTUM;
                            experimentalGltf.Materials[0] = simulatedMaterial;

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

        // Simulated enum
        public class Material : glTFLoader.Schema.Material
        {
            public Material(glTFLoader.Schema.Material parent)
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

            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("alphaMode2")]
            public AlphaModeEnum AlphaMode2 { get; set; }

            new public enum AlphaModeEnum
            {
                OPAQUE = 0,
                MASK = 1,
                BLEND = 2,
                QUANTUM = 3,
            }
        }
    }
}
