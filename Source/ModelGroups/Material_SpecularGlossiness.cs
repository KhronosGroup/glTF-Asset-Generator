using System;
using System.Numerics;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Material_SpecularGlossiness : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Material_SpecularGlossiness;

        public Material_SpecularGlossiness(List<string> imageList)
        {
            var diffuseTextureImage = GetImage(imageList, "Diffuse_Plane");
            var specularGlossinessTextureImage = GetImage(imageList, "SpecularGlossiness_Plane");
            var baseColorTextureImage = GetImage(imageList, "BaseColor_X");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.ExtensionUsed, "Specular Glossiness"));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.Material, Runtime.Extensions.PbrSpecularGlossiness, List<string>> setProperties)
            {
                var properties = new List<Property>();
                List<string> extensionsUsed = new List<string> { "KHR_materials_pbrSpecularGlossiness" };
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                var extension = new Runtime.Extensions.PbrSpecularGlossiness();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.Extensions = new List<Runtime.Extensions.Extension>() { extension };
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material, extension, extensionsUsed);

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
                                    MeshPrimitives = new List<Runtime.MeshPrimitive>
                                    {
                                        meshPrimitive
                                    }
                                },
                            },
                        },
                    },
                    extensionsUsed
                    ),
                };
            }

            void NoSpecularGlossiness(Runtime.Material material, List< string> extensionsUsed)
            {
                material.Extensions = null;
                extensionsUsed = null;
            }

            void SetVertexColor(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                List<Vector4> vertexColors = new List<Vector4>()
                {
                    new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                    new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                    new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                    new Vector4( 1.0f, 0.0f, 0.0f, 0.8f)
                };
                meshPrimitive.ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                meshPrimitive.ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                meshPrimitive.Colors = vertexColors;

                var vertexColorsValue = new VertexColor(meshPrimitive.ColorComponentType, meshPrimitive.ColorType, meshPrimitive.Colors);
                properties.Add(new Property(PropertyName.VertexColor, vertexColorsValue));
            }

            void SetDiffuseTexture(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                extension.DiffuseTexture = new Runtime.Texture { Source = diffuseTextureImage };
                properties.Add(new Property(PropertyName.DiffuseTexture, diffuseTextureImage));
            }

            void SetDiffuseFactor(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                var diffuseFactorValue = new Vector4(0.2f, 0.2f, 0.2f, 0.8f);
                extension.DiffuseFactor = diffuseFactorValue;
                properties.Add(new Property(PropertyName.DiffuseFactor, diffuseFactorValue));
            }

            void SetSpecularGlossinessTexture(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                extension.SpecularGlossinessTexture = new Runtime.Texture { Source = specularGlossinessTextureImage };
                properties.Add(new Property(PropertyName.SpecularGlossinessTexture, specularGlossinessTextureImage));
            }

            void SetSpecularFactor(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                var specularFactorValue = new Vector3(0.4f, 0.4f, 0.4f);
                extension.SpecularFactor = specularFactorValue;
                properties.Add(new Property(PropertyName.SpecularFactor, specularFactorValue));
            }

            void Set0SpecularFactor(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                var specularFactorValue = new Vector3(0.0f, 0.0f, 0.0f);
                extension.SpecularFactor = specularFactorValue;
                properties.Add(new Property(PropertyName.SpecularFactor, specularFactorValue));
            }

            void SetGlossinessFactor(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                extension.GlossinessFactor = 0.3f;
                properties.Add(new Property(PropertyName.GlossinessFactor, extension.GlossinessFactor));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    NoSpecularGlossiness(material, extensionsUsed);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetVertexColor(properties, meshPrimitive);
                    Set0SpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetDiffuseTexture(properties, extension);
                    Set0SpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetDiffuseFactor(properties, extension);
                    Set0SpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetSpecularGlossinessTexture(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetSpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetDiffuseTexture(properties, extension);
                    Set0SpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetDiffuseTexture(properties, extension);
                    SetDiffuseFactor(properties, extension);
                    Set0SpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetDiffuseTexture(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetSpecularGlossinessTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetSpecularGlossinessTexture(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension, extensionsUsed) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetDiffuseTexture(properties, extension);
                    SetDiffuseFactor(properties, extension);
                    SetSpecularGlossinessTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                    SetSpecularFactor(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
