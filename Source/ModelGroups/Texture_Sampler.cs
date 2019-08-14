using System;
using System.Collections.Generic;
using System.Numerics;
using static glTFLoader.Schema.Sampler;

namespace AssetGenerator
{
    internal class Texture_Sampler : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Texture_Sampler;

        public Texture_Sampler(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.Sampler> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture
                        {
                            Source = baseColorTextureImage,
                            Sampler = new Runtime.Sampler(),
                        },
                    },
                };

                // Apply the common properties to the gltf.
                meshPrimitive.TextureCoordSets = new Runtime.Accessor
                (
                    new[]
                    {
                        new[]
                        {
                            new Vector2( 1.3f,  1.3f),
                            new Vector2(-0.3f,  1.3f),
                            new Vector2(-0.3f, -0.3f),
                            new Vector2( 1.3f, -0.3f)
                        },
                    }
                );

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene
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
                    }),
                };
            }

            void SetWrapT(List<Property> properties, Runtime.Sampler sampler, WrapTEnum enumValue)
            {
                sampler.WrapT = enumValue;
                properties.Add(new Property(PropertyName.WrapT, enumValue.ToReadmeString()));
            }

            void SetWrapS(List<Property> properties, Runtime.Sampler sampler, WrapSEnum enumValue)
            {
                sampler.WrapS = enumValue;
                properties.Add(new Property(PropertyName.WrapS, sampler.WrapS.ToReadmeString()));
            }

            void SetMagFilter(List<Property> properties, Runtime.Sampler sampler, MagFilterEnum enumValue)
            {
                sampler.MagFilter = enumValue;
                properties.Add(new Property(PropertyName.MagFilter, enumValue.ToReadmeString()));
            }

            void SetMinFilter(List<Property> properties, Runtime.Sampler sampler, MinFilterEnum enumValue)
            {
                sampler.MinFilter = enumValue;
                properties.Add(new Property(PropertyName.MinFilter, enumValue.ToReadmeString()));
            }

            Models = new List<Model>
            {
                CreateModel((properties, sampler) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapT(properties, sampler, WrapTEnum.CLAMP_TO_EDGE);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapT(properties, sampler, WrapTEnum.MIRRORED_REPEAT);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapS(properties, sampler, WrapSEnum.CLAMP_TO_EDGE);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapS(properties, sampler, WrapSEnum.MIRRORED_REPEAT);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMagFilter(properties, sampler, MagFilterEnum.NEAREST);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMagFilter(properties, sampler, MagFilterEnum.LINEAR);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, MinFilterEnum.NEAREST);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, MinFilterEnum.LINEAR);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, MinFilterEnum.NEAREST_MIPMAP_NEAREST);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, MinFilterEnum.LINEAR_MIPMAP_NEAREST);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, MinFilterEnum.NEAREST_MIPMAP_LINEAR);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, MinFilterEnum.LINEAR_MIPMAP_LINEAR);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapT(properties, sampler, WrapTEnum.CLAMP_TO_EDGE);
                    SetWrapS(properties, sampler, WrapSEnum.CLAMP_TO_EDGE);
                    SetMagFilter(properties, sampler, MagFilterEnum.NEAREST);
                    SetMinFilter(properties, sampler, MinFilterEnum.NEAREST);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
