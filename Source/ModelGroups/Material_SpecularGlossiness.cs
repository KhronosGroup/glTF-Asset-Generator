using AssetGenerator.Runtime;
using AssetGenerator.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Material_SpecularGlossiness : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_SpecularGlossiness;

        public Material_SpecularGlossiness(List<string> imageList)
        {
            Image diffuseTextureImage = UseTexture(imageList, "Diffuse_Plane");
            Image specularGlossinessTextureImage = UseTexture(imageList, "SpecularGlossiness_Plane");
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_X");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.ExtensionUsed, "Specular Glossiness"));
            CommonProperties.Add(new Property(PropertyName.ExtensionRequired, "Specular Glossiness"));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive, Runtime.Material, KHR_materials_pbrSpecularGlossiness> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                var extension = new KHR_materials_pbrSpecularGlossiness();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.Extensions = new List<Extension> { extension };
                meshPrimitive.Material.MetallicRoughnessMaterial = new PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Texture { Source = baseColorTextureImage };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive, meshPrimitive.Material, extension);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = new[]
                        {
                            new Node
                            {
                                Mesh = new Runtime.Mesh
                                {
                                    MeshPrimitives = new[]
                                    {
                                        meshPrimitive
                                    }
                                },
                            },
                        },
                    },
                    extensionsUsed: new List<string> { "KHR_materials_pbrSpecularGlossiness" },
                    extensionsRequired: new List<string> { "KHR_materials_pbrSpecularGlossiness" }),
                };
            }

            void SetVertexColor(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors = new Accessor
                   (
                       new[]
                       {
                        new Vector4(0.0f, 0.0f, 1.0f, 0.8f),
                        new Vector4(1.0f, 0.0f, 0.0f, 0.8f),
                        new Vector4(0.0f, 0.0f, 1.0f, 0.8f),
                        new Vector4(1.0f, 0.0f, 0.0f, 0.8f),
                       },
                       Accessor.ComponentTypeEnum.FLOAT,
                       Accessor.TypeEnum.VEC3
                   );

                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.ComponentType.ToReadmeString()}"));
            }

            void SetDiffuseTexture(List<Property> properties, KHR_materials_pbrSpecularGlossiness extension)
            {
                extension.DiffuseTexture = new Texture { Source = diffuseTextureImage };
                properties.Add(new Property(PropertyName.DiffuseTexture, diffuseTextureImage.ToReadmeString()));
            }

            void SetDiffuseFactor(List<Property> properties, KHR_materials_pbrSpecularGlossiness extension)
            {
                var diffuseFactorValue = new Vector4(0.2f, 0.2f, 0.2f, 0.8f);
                extension.DiffuseFactor = diffuseFactorValue;
                properties.Add(new Property(PropertyName.DiffuseFactor, diffuseFactorValue.ToReadmeString()));
            }

            void SetSpecularGlossinessTexture(List<Property> properties, KHR_materials_pbrSpecularGlossiness extension)
            {
                extension.SpecularGlossinessTexture = new Texture { Source = specularGlossinessTextureImage };
                properties.Add(new Property(PropertyName.SpecularGlossinessTexture, specularGlossinessTextureImage.ToReadmeString()));
            }

            void SetSpecularFactor(List<Property> properties, KHR_materials_pbrSpecularGlossiness extension)
            {
                var specularFactorValue = new Vector3(0.4f, 0.4f, 0.4f);
                extension.SpecularFactor = specularFactorValue;
                properties.Add(new Property(PropertyName.SpecularFactor, specularFactorValue.ToReadmeString()));
            }

            void SetSpecularFactorToZero(List<Property> properties, KHR_materials_pbrSpecularGlossiness extension)
            {
                var specularFactorValue = new Vector3(0.0f, 0.0f, 0.0f);
                extension.SpecularFactor = specularFactorValue;
                properties.Add(new Property(PropertyName.SpecularFactor, specularFactorValue.ToReadmeString()));
            }

            void SetGlossinessFactor(List<Property> properties, KHR_materials_pbrSpecularGlossiness extension)
            {
                extension.GlossinessFactor = 0.3f;
                properties.Add(new Property(PropertyName.GlossinessFactor, extension.GlossinessFactor.ToReadmeString()));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetVertexColor(properties, meshPrimitive);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetDiffuseFactor(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetSpecularGlossinessTexture(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetSpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetVertexColor(properties, meshPrimitive);
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetDiffuseTexture(properties, extension);
                    SetDiffuseFactor(properties, extension);
                    SetSpecularFactorToZero(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetDiffuseTexture(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetSpecularGlossinessTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetSpecularGlossinessTexture(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetDiffuseTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
                CreateModel((properties, meshPrimitive, material, extension) =>
                {
                    SetVertexColor(properties, meshPrimitive);
                    SetDiffuseTexture(properties, extension);
                    SetDiffuseFactor(properties, extension);
                    SetSpecularGlossinessTexture(properties, extension);
                    SetSpecularFactor(properties, extension);
                    SetGlossinessFactor(properties, extension);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
