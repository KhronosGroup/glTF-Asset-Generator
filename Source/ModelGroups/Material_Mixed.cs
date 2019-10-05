using System;
using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    internal class Material_Mixed : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_Mixed;

        public Material_Mixed(List<string> imageList)
        {
            var baseColorTexture = new Runtime.Texture { Source = UseTexture(imageList, "BaseColor_X") };
            UseFigure(imageList, "UVSpace2");
            UseFigure(imageList, "UVSpace3");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.ExtensionUsed, "Specular Glossiness"));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.Material, Runtime.Material> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane();
                var baseColorTextureInfo = new Runtime.TextureInfo { Texture = baseColorTexture };
                meshPrimitives[0].Material = new Runtime.Material();
                meshPrimitives[0].Material.PbrMetallicRoughness = new Runtime.PbrMetallicRoughness();
                meshPrimitives[1].Material = new Runtime.Material();
                meshPrimitives[1].Material.PbrMetallicRoughness = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitives[0].Material.PbrMetallicRoughness.BaseColorTexture = baseColorTextureInfo;
                meshPrimitives[1].Material.PbrMetallicRoughness.BaseColorTexture = baseColorTextureInfo;

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitives[0].Material, meshPrimitives[1].Material);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene
                    {
                        Nodes = new[]
                        {
                            new Runtime.Node
                            {
                                Mesh = new Runtime.Mesh
                                {
                                    MeshPrimitives = meshPrimitives
                                },
                            },
                        },
                    }, extensionsUsed: new List<string>() { "KHR_materials_pbrSpecularGlossiness" }),
                };
            }

            void SetSpecularGlossiness0(List<Property> properties, Runtime.Material material0)
            {
                material0.Extensions = new List<Runtime.Extensions.Extension> { new Runtime.Extensions.KHR_materials_pbrSpecularGlossiness() };
                properties.Add(new Property(PropertyName.SpecularGlossinessOnMaterial0, ":white_check_mark:"));
            }

            void SetSpecularGlossiness1(List<Property> properties, Runtime.Material material1)
            {
                material1.Extensions = new List<Runtime.Extensions.Extension> { new Runtime.Extensions.KHR_materials_pbrSpecularGlossiness() };
                properties.Add(new Property(PropertyName.SpecularGlossinessOnMaterial1, ":white_check_mark:"));
            }

            void NoSpecularGlossiness0(List<Property> properties)
            {
                properties.Add(new Property(PropertyName.SpecularGlossinessOnMaterial0, ":x:"));
            }

            void NoSpecularGlossiness1(List<Property> properties)
            {
                properties.Add(new Property(PropertyName.SpecularGlossinessOnMaterial1, ":x:"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, material0, material1) =>
                {
                    SetSpecularGlossiness0(properties, material0);
                    SetSpecularGlossiness1(properties, material1);
                }),
                CreateModel((properties, material0, material1) =>
                {
                    NoSpecularGlossiness0(properties);
                    NoSpecularGlossiness1(properties);
                }),
                CreateModel((properties, material0, material1) =>
                {
                    SetSpecularGlossiness0(properties, material0);
                    NoSpecularGlossiness1(properties);

                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
