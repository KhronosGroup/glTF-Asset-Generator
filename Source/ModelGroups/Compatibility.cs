using glTFLoader.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace AssetGenerator
{
    internal class Compatibility : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Compatibility;

        public Compatibility(List<string> imageList)
        {
            NoSampleImages = true;

            // There are no common properties in this model group.

            Model CreateModel(Action<List<Property>, Runtime.GLTF> setProperties, Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null, Dictionary<Type, Type> schemaTypeMapping = null)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);
                var gltf = CreateGLTF(() => new Runtime.Scene
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
                var model = new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };

                if (postRuntimeChanges != null)
                {
                    model.PostRuntimeChanges = postRuntimeChanges;
                }

                if (schemaTypeMapping != null)
                {
                    model.CreateSchemaInstance = type =>
                    {
                        if (schemaTypeMapping.TryGetValue(type, out Type mappedType))
                        {
                            type = mappedType;
                        }

                        return Activator.CreateInstance(type);
                    };
                }

                return model;
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

            void SetDescription(List<Property> properties, string description)
            {
                properties.Add(new Property(PropertyName.Description, description));
            }

            void SetDescriptionExtensionRequired(List<Property> properties, Runtime.GLTF gltf)
            {
                var extension = new Runtime.Extensions.FAKE_materials_quantumRendering
                {
                    PlanckFactor = new Vector4(0.2f, 0.2f, 0.2f, 0.8f),
                    CopenhagenTexture = new Runtime.Texture(),
                    EntanglementFactor = new Vector3(0.4f, 0.4f, 0.4f),
                    ProbabilisticFactor = 0.3f,
                    SuperpositionCollapseTexture = new Runtime.Texture(),
                };

                gltf.Scenes.ElementAt(0).Nodes[0].Mesh.MeshPrimitives.ElementAt(0).Material = new Runtime.Material()
                {
                    Extensions = new List<Runtime.Extensions.Extension>()
                    {
                        extension
                    }
                };

                gltf.ExtensionsRequired = new List<string>() { extension.Name };

                properties.Add(new Property(PropertyName.Description, "Extension required"));
            }

            void SetModelShouldLoad(List<Property> properties, string loadableStatus = ":white_check_mark:")
            {
                properties.Add(new Property(PropertyName.ModelShouldLoad, loadableStatus));
            }

            void SetPostRuntimeAtRoot(glTFLoader.Schema.Gltf gltf)
            {
                // Add an simulated feature at the root level
                var experimentalGltf = (ExperimentalGltf)gltf;
                experimentalGltf.Lights = new[]
                {
                    new ExperimentalLight
                    {
                        Color = new float[] { 0.3f, 0.4f, 0.5f }
                    }
                };
            }

            void SetPostRuntimeInProperty(glTFLoader.Schema.Gltf gltf)
            {
                // Add an simulated feature into an existing property
                var experimentalNode = (ExperimentalNode)gltf.Nodes[0];
                experimentalNode.Light = 0;
            }

            void SetPostRuntimeWithFallback(glTFLoader.Schema.Gltf gltf)
            {
                // Add an simulated feature with a fallback option
                gltf.Materials = new[]
                {
                    new ExperimentalMaterial
                    {
                        AlphaMode = glTFLoader.Schema.Material.AlphaModeEnum.BLEND,
                        AlphaMode2 = ExperimentalAlphaMode2.QUANTUM,
                    }
                };
            }

            var experimentalSchemaTypeMapping = new Dictionary<Type, Type>
            {
                { typeof(glTFLoader.Schema.Gltf), typeof(ExperimentalGltf) },
                { typeof(glTFLoader.Schema.Node), typeof(ExperimentalNode) },
                { typeof(glTFLoader.Schema.Material), typeof(ExperimentalMaterial) },
            };

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf) => {
                    SetVersionCurrent(properties, gltf);
                    SetModelShouldLoad(properties);
                }),
                CreateModel((properties, gltf) => {
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, "Light object added at root");
                    SetModelShouldLoad(properties);
                }, SetPostRuntimeAtRoot, experimentalSchemaTypeMapping),
                CreateModel((properties, gltf) => {
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, "Light property added to node object");
                    SetModelShouldLoad(properties);
                }, SetPostRuntimeInProperty, experimentalSchemaTypeMapping),
                CreateModel((properties, gltf) => {
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, "Alpha mode updated with a new enum value, and a fallback value");
                    SetModelShouldLoad(properties);
                }, SetPostRuntimeWithFallback, experimentalSchemaTypeMapping),
                CreateModel((properties, gltf) => {
                    SetMinVersion(properties, gltf);
                    SetVersionFuture(properties, gltf);
                    SetDescription(properties, "Requires a specific version or higher");
                    SetModelShouldLoad(properties, "Only in version 2.1 or higher");
                }),
                CreateModel((properties, gltf) => {
                    SetVersionCurrent(properties, gltf);
                    SetDescriptionExtensionRequired(properties, gltf);
                    SetModelShouldLoad(properties, ":x:");
                }),
            };

            GenerateUsedPropertiesList();
        }

        private class ExperimentalNode : glTFLoader.Schema.Node
        {
            [JsonProperty("light")]
            public int? Light { get; set; }

            public bool ShouldSerializeLight()
            {
                return Light.HasValue;
            }
        }

        private class ExperimentalLight
        {
            [JsonProperty("color")]
            public float[] Color { get; set; }
        }

        // Used to add a property to the root level, or into an existing property
        private class ExperimentalGltf : glTFLoader.Schema.Gltf
        {
            // Creates a new root level property
            [JsonProperty("lights")]
            public ExperimentalLight[] Lights { get; set; }

            public bool ShouldSerializeLights()
            {
                return Lights != null;
            }
        }

        private enum ExperimentalAlphaMode2
        {
            OPAQUE = 0,
            MASK = 1,
            BLEND = 2,
            QUANTUM = 3,
        }

        // Used to add a new enum into an existing property with a fallback option
        private class ExperimentalMaterial : glTFLoader.Schema.Material
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("alphaMode2")]
            public ExperimentalAlphaMode2 AlphaMode2 { get; set; }
        }
    }
}
