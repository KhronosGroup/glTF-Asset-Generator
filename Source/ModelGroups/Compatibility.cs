using AssetGenerator.Runtime;
using AssetGenerator.Runtime.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Schema = glTFLoader.Schema;

namespace AssetGenerator.ModelGroups
{
    internal class Compatibility : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Compatibility;

        public Compatibility(List<string> imageList)
        {
            NoSampleImages = true;

            // There are no common properties in this model group.

            Model CreateModel(Action<List<Property>, Asset, List<string>, List<string>, Runtime.MeshPrimitive> setProperties,
                Action<Schema.Gltf> postRuntimeChanges = null, Dictionary<Type, Type> schemaTypeMapping = null, bool? setLoadableTag = true)
            {
                var properties = new List<Property>();

                var gltf = CreateGLTF(() => new Scene());
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);
                var extensionsUsed = new List<string>();
                var extensionsRequired = new List<string>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf.Asset, extensionsUsed, extensionsRequired, meshPrimitive);

                // Create the gltf object.
                if (extensionsUsed.Any())
                {
                    gltf.ExtensionsUsed = extensionsUsed;
                }
                if (extensionsRequired.Any())
                {
                    gltf.ExtensionsRequired = extensionsRequired;
                }

                gltf.Scenes.First().Nodes = new List<Node>
                {
                    new Node
                    {
                        Mesh = new Runtime.Mesh
                        {
                            MeshPrimitives = new List<Runtime.MeshPrimitive>
                            {
                                meshPrimitive
                            }
                        },
                    },
                };

                var model = new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };

                model.Loadable = setLoadableTag;
                model.PostRuntimeChanges = postRuntimeChanges;

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

            Property SetMinVersion(Asset asset)
            {
                return new Property(PropertyName.MinVersion, asset.MinVersion = "2.1");
            }

            Property SetVersionCurrent(Asset asset)
            {
                return new Property(PropertyName.Version, asset.Version = "2.0");
            }

            Property SetVersionFuture(Asset asset)
            {
                return new Property(PropertyName.Version, asset.Version = "2.1");
            }

            void SetPostRuntimeAtRoot(Schema.Gltf gltf)
            {
                // Add an simulated feature at the root level.
                var experimentalGltf = (ExperimentalGltf)gltf;
                experimentalGltf.Lights = new[]
                {
                    new ExperimentalLight
                    {
                        Color = new[] { 0.3f, 0.4f, 0.5f }
                    }
                };
            }

            void SetPostRuntimeInProperty(Schema.Gltf gltf)
            {
                // Add an simulated feature into an existing property.
                var experimentalNode = (ExperimentalNode)gltf.Nodes[0];
                experimentalNode.Light = 0;
            }

            void SetPostRuntimeWithFallback(Schema.Gltf gltf)
            {
                // Add an simulated feature with a fallback option.
                gltf.Materials = new Schema.Material[]
                {
                    new ExperimentalMaterial
                    {
                        AlphaMode = Schema.Material.AlphaModeEnum.BLEND,
                        AlphaMode2 = ExperimentalAlphaMode2.QUANTUM,
                    }
                };
            }

            var experimentalSchemaTypeMapping = new Dictionary<Type, Type>
            {
                { typeof(Schema.Gltf), typeof(ExperimentalGltf) },
                { typeof(Schema.Node), typeof(ExperimentalNode) },
                { typeof(Schema.Material), typeof(ExperimentalMaterial) },
            };

            var shouldLoad = ":white_check_mark:";

            Models = new List<Model>
            {
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetVersionCurrent(asset));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, shouldLoad));
                }),
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetVersionFuture(asset));
                    properties.Add(new Property(PropertyName.Description, "Light object added at root"));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, shouldLoad));
                }, SetPostRuntimeAtRoot, experimentalSchemaTypeMapping),
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetVersionFuture(asset));
                    properties.Add(new Property(PropertyName.Description, "Light property added to node object"));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, shouldLoad));
                }, SetPostRuntimeInProperty, experimentalSchemaTypeMapping),
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetVersionFuture(asset));
                    properties.Add(new Property(PropertyName.Description, "Alpha mode updated with a new enum value, and a fallback value"));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, shouldLoad));
                }, SetPostRuntimeWithFallback, experimentalSchemaTypeMapping),
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetMinVersion(asset));
                    properties.Add(SetVersionFuture(asset));
                    properties.Add(new Property(PropertyName.Description, "Requires a specific version or higher"));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, "Only in version 2.1 or higher"));
                }, null, null, setLoadableTag: false),
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetVersionCurrent(asset));

                    var emptyTexture = new Texture();
                    var extension = new FAKE_materials_quantumRendering
                    {
                        PlanckFactor = new Vector4(0.2f, 0.2f, 0.2f, 0.8f),
                        CopenhagenTexture = new TextureInfo
                        {
                            Texture = emptyTexture
                        },
                        EntanglementFactor = new Vector3(0.4f, 0.4f, 0.4f),
                        ProbabilisticFactor = 0.3f,
                        SuperpositionCollapseTexture = new TextureInfo
                        {
                            Texture = emptyTexture
                        },
                    };

                    meshPrimitive.Material = new Runtime.Material
                    {
                        Extensions = new List<Extension>
                        {
                            extension
                        }
                    };

                    extensionsUsed.Add(extension.Name);
                    extensionsRequired.Add(extension.Name);

                    properties.Add(new Property(PropertyName.Description, "Extension required"));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, ":x:"));
                }, null, null, setLoadableTag: false),
                CreateModel((properties, asset, extensionsUsed, extensionsRequired, meshPrimitive) =>
                {
                    properties.Add(SetVersionCurrent(asset));

                    var extension = new KHR_materials_pbrSpecularGlossiness
                    {
                        SpecularFactor = new Vector3(0.04f, 0.04f, 0.04f),
                        GlossinessFactor = 0.0f,
                    };

                    meshPrimitive.Material = new Runtime.Material
                    {
                        // Metallic-Roughness
                        PbrMetallicRoughness = new PbrMetallicRoughness
                        {
                            MetallicFactor = 0.0f
                        },
                        // Specular-Glossiness
                        Extensions = new[] { extension },
                    };

                    extensionsUsed.Add(extension.Name);

                    properties.Add(new Property(PropertyName.Description, "Specular Glossiness extension used but not required"));
                    properties.Add(new Property(PropertyName.ModelShouldLoad, shouldLoad));
                }),
            };

            GenerateUsedPropertiesList();
        }

        private class ExperimentalNode : Schema.Node
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

        // Used to add a property to the root level, or into an existing property.
        private class ExperimentalGltf : Schema.Gltf
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

        // Used to add a new enum into an existing property with a fallback option.
        private class ExperimentalMaterial : Schema.Material
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("alphaMode2")]
            public ExperimentalAlphaMode2 AlphaMode2 { get; set; }
        }
    }
}
