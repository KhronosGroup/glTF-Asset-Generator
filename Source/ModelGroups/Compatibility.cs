using System;
using System.Numerics;
using System.Collections.Generic;
using glTFLoader.Shared;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Converters;

namespace AssetGenerator
{
    internal class Compatibility : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Compatibility;

        public Compatibility(List<string> imageList)
        {
            ApplyPostRuntimeChanges = true;
            NoSampleImages = true;

            // There are no common properties in this model group.

            Model CreateModel(Action<List<Property>, Runtime.GLTF> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);
                var gltf = CreateGLTF(() => new Runtime.Scene()
                {
                    Nodes = new List<Runtime.Node>
                    {
                        new Runtime.Node
                        {
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new List<Runtime.MeshPrimitive>
                                {
                                    meshPrimitive
                                }
                            },
                        },
                    },
                });

                // There are no common properties in this model group.

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                };
            }

            void SetMinVersion(List<Property> properties, Runtime.GLTF gltf)
            {
                gltf.Asset.MinVersion = "2.1";
                properties.Add(new Property(PropertyName.MinVersion, gltf.Asset.MinVersion));
            }

            void SetVersionCurrent(List<Property> properties, Runtime.GLTF gltf)
            {
                gltf.Asset.Version = "2.0";
                properties.Add(new Property(PropertyName.Version, gltf.Asset.Version));
            }

            void SetVersionFuture(List<Property> properties, Runtime.GLTF gltf)
            {
                gltf.Asset.Version = "2.1";
                properties.Add(new Property(PropertyName.Version, gltf.Asset.Version));
            }

            void SetDescription(List<Property> properties, Runtime.GLTF gltf, string description)
            {
                if (description == "Extension required")
                {
                    gltf.ExtensionsRequired = new List<string>() { "EXT_QuantumRendering" };
                    gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = new Runtime.Material()
                    {
                        Extensions = new List<Runtime.Extensions.Extension>()
                        {
                            new Runtime.Extensions.EXT_QuantumRendering()
                            {
                                PlanckFactor = new Vector4(0.2f, 0.2f, 0.2f, 0.8f),
                                CopenhagenTexture = new Runtime.Texture(),
                                EntanglementFactor = new Vector3(0.4f, 0.4f, 0.4f),
                                ProbabilisticFactor = 0.3f,
                                SuperpositionCollapseTexture = new Runtime.Texture(),
                            }
                        }
                    };
                }

                properties.Add(new Property(PropertyName.Description, description));
            }

            void SetModelShouldLoad(List<Property> properties, string loadableStatus = ":white_check_mark:")
            {
                properties.Add(new Property(PropertyName.ModelShouldLoad, loadableStatus));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf) => {
                    SetVersionCurrent(properties, gltf);
                    SetModelShouldLoad(properties);
                }),
                CreateModel((properties, gltf) => {
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, gltf, "Light object added at root");
                    SetModelShouldLoad(properties);
                }),
                CreateModel((properties, gltf) => {
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, gltf, "Light property added to node object");
                    SetModelShouldLoad(properties);
                }),
                CreateModel((properties, gltf) => {
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, gltf, "Alpha mode updated with a new enum value, and a fallback value");
                    SetModelShouldLoad(properties);
                }),
                CreateModel((properties, gltf) => {
                    SetMinVersion(properties, gltf);
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, gltf, "Requires a specific version or higher");
                    SetModelShouldLoad(properties, "Only in version 2.1 or higher");
                }),
                CreateModel((properties, gltf) => {
                    SetVersionCurrent(properties, gltf);
                    SetDescription(properties, gltf, "Extension required");
                    SetModelShouldLoad(properties, ":x:");
                }),
            };

            GenerateUsedPropertiesList();
        }

        /// <summary>
        /// These properties are added to the model after the Runtime layer generates the gltf.
        /// </summary>
        public override void PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (Property property in combo)
            {
                if (property.Name == PropertyName.Description)
                {
                    switch (property.ReadmeValue)
                    {
                        case "Light object added at root":
                            {
                                // Add an simulated feature at the root level
                                gltf = new ExperimentalGltf1(gltf)
                                {
                                    lights = new ExperimentalGltf1.Light
                                    {
                                        Color = new float[] { 0.3f, 0.4f, 0.5f }
                                    }
                                };
                                break;
                            }
                        case "Light property added to node object":
                            {
                                // Add an simulated feature into an existing property
                                gltf.Nodes[0] = new ExperimentalGltf1.Node(gltf.Nodes[0])
                                {
                                    Light = 0.5f
                                };
                                break;
                            }
                        case "Alpha mode updated with a new enum value, and a fallback value":
                            {
                                // Add an simulated feature with a fallback option
                                gltf = new ExperimentalGltf2(gltf)
                                {
                                    Materials = new ExperimentalGltf2.Material[] 
                                    {
                                        new ExperimentalGltf2.Material(new glTFLoader.Schema.Material())
                                        {
                                            AlphaMode = glTFLoader.Schema.Material.AlphaModeEnum.BLEND,
                                            AlphaMode2 = ExperimentalGltf2.Material.AlphaModeEnum.QUANTUM,
                                        }
                                    }
                                };
                                break;
                            }
                    }
                }
            }
        }

        // Used to add a property to the root level, or into an existing property
        private class ExperimentalGltf1 : glTFLoader.Schema.Gltf
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
        private class ExperimentalGltf2 : glTFLoader.Schema.Gltf
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
}
