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
            var diffuseTextureImage = UseTexture(imageList, "Diffuse_Plane");
            var specularGlossinessTextureImage = UseTexture(imageList, "SpecularGlossiness_Plane");
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_X");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.ExtensionUsed, "Specular Glossiness"));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.Material, Runtime.Extensions.PbrSpecularGlossiness> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                var extension = new Runtime.Extensions.PbrSpecularGlossiness();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.Extensions = new List<Runtime.Extensions.Extension>() { extension };
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material, extension);

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
                    }, new List<string>() { "KHR_materials_pbrSpecularGlossiness" }),
                };
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
                properties.Add(new Property(PropertyName.SpecularFactor, specularFactorValue, group: 1));
            }

            void SetSpecularFactorToZero(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                var specularFactorValue = new Vector3(0.0f, 0.0f, 0.0f);
                extension.SpecularFactor = specularFactorValue;
                properties.Add(new Property(PropertyName.SpecularFactor, specularFactorValue, group: 1));
            }

            void SetGlossinessFactor(List<Property> properties, Runtime.Extensions.PbrSpecularGlossiness extension)
            {
                extension.GlossinessFactor = 0.3f;
                properties.Add(new Property(PropertyName.GlossinessFactor, extension.GlossinessFactor));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, material, extension) => {

                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetDiffuseFactor(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetSpecularGlossinessTexture(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetSpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetVertexColor(properties, meshPrimitive);
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetDiffuseTexture(properties, extension);
                    SetDiffuseFactor(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetDiffuseTexture(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetSpecularGlossinessTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetSpecularGlossinessTexture(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) => {
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
