using System;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Material_Mixed : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Material_Mixed;

        public Material_Mixed(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_X");
            UseFigure(imageList, "UVSpace2");
            UseFigure(imageList, "UVSpace3");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.ExtensionUsed, "Specular Glossiness"));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.Material, Runtime.Material> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitives = MeshPrimitive.CreateMultiPrimitivePlane();
                var baseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };
                meshPrimitives[0].Material = new Runtime.Material();
                meshPrimitives[0].Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                meshPrimitives[1].Material = new Runtime.Material();
                meshPrimitives[1].Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitives[0].Material.MetallicRoughnessMaterial.BaseColorTexture = baseColorTexture;
                meshPrimitives[1].Material.MetallicRoughnessMaterial.BaseColorTexture = baseColorTexture;

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitives[0].Material, meshPrimitives[1].Material);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene()
                    {
                        Nodes = new List<Runtime.Node>
                        {
                            new Runtime.Node
                            {
                                Mesh = new Runtime.Mesh
                                {
                                    MeshPrimitives = meshPrimitives
                                },
                            },
                        },
                    }, new List<string>() { "KHR_materials_pbrSpecularGlossiness" }),
                };
            }

            void SetSpecularGlossiness0(List<Property> properties, Runtime.Material material0)
            {
                material0.Extensions = new List<Runtime.Extensions.Extension>() { new Runtime.Extensions.KHR_materials_pbrSpecularGlossiness() };
                properties.Add(new Property(PropertyName.SpecularGlossinessOnMaterial0, ":white_check_mark:"));
            }

            void SetSpecularGlossiness1(List<Property> properties, Runtime.Material material1)
            {
                material1.Extensions = new List<Runtime.Extensions.Extension>() { new Runtime.Extensions.KHR_materials_pbrSpecularGlossiness() };
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

            this.Models = new List<Model>
            {
                CreateModel((properties, material0, material1) => {
                    SetSpecularGlossiness0(properties, material0);
                    SetSpecularGlossiness1(properties, material1);
                }),
                CreateModel((properties, material0, material1) => {
                    NoSpecularGlossiness0(properties);
                    NoSpecularGlossiness1(properties);
                }),
                CreateModel((properties, material0, material1) => {
                    SetSpecularGlossiness0(properties, material0);
                    NoSpecularGlossiness1(properties);

                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
