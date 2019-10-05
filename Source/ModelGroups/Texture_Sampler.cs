using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Texture_Sampler : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Texture_Sampler;

        public Texture_Sampler(List<string> imageList)
        {
            var baseColorImage = UseTexture(imageList, "BaseColor_Plane");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.Sampler> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.TextureInfo
                        {
                            Texture = new Runtime.Texture
                            {
                                Source = baseColorImage,
                                Sampler = new Runtime.Sampler()
                            }
                        },
                    },
                };

                // Apply the common properties to the gltf.
                meshPrimitive.TexCoords0 = Runtime.Data.Create
                (
                    new[]
                    {
                        new Vector2( 1.3f,  1.3f),
                        new Vector2(-0.3f,  1.3f),
                        new Vector2(-0.3f, -0.3f),
                        new Vector2( 1.3f, -0.3f)
                    }
                );

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive.Material.PbrMetallicRoughness.BaseColorTexture.Texture.Sampler);

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

            void SetWrapT(List<Property> properties, Runtime.Sampler sampler, Runtime.SamplerWrap enumValue)
            {
                sampler.WrapT = enumValue;
                properties.Add(new Property(PropertyName.WrapT, enumValue.ToReadmeString()));
            }

            void SetWrapS(List<Property> properties, Runtime.Sampler sampler, Runtime.SamplerWrap enumValue)
            {
                sampler.WrapS = enumValue;
                properties.Add(new Property(PropertyName.WrapS, sampler.WrapS.ToReadmeString()));
            }

            void SetMagFilter(List<Property> properties, Runtime.Sampler sampler, Runtime.SamplerMagFilter enumValue)
            {
                sampler.MagFilter = enumValue;
                properties.Add(new Property(PropertyName.MagFilter, enumValue.ToReadmeString()));
            }

            void SetMinFilter(List<Property> properties, Runtime.Sampler sampler, Runtime.SamplerMinFilter enumValue)
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
                    SetWrapT(properties, sampler, Runtime.SamplerWrap.ClampToEdge);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapT(properties, sampler, Runtime.SamplerWrap.MirroredRepeat);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapS(properties, sampler, Runtime.SamplerWrap.ClampToEdge);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapS(properties, sampler, Runtime.SamplerWrap.MirroredRepeat);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMagFilter(properties, sampler, Runtime.SamplerMagFilter.Nearest);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMagFilter(properties, sampler, Runtime.SamplerMagFilter.Linear);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.Nearest);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.Linear);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.NearestMipmapNearest);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.LinearMipmapNearest);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.NearestMipmapLinear);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.LinearMipmapLinear);
                }),
                CreateModel((properties, sampler) =>
                {
                    SetWrapT(properties, sampler, Runtime.SamplerWrap.ClampToEdge);
                    SetWrapS(properties, sampler, Runtime.SamplerWrap.ClampToEdge);
                    SetMagFilter(properties, sampler, Runtime.SamplerMagFilter.Nearest);
                    SetMinFilter(properties, sampler, Runtime.SamplerMinFilter.Nearest);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
