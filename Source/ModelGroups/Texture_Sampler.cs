using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Texture_Sampler : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Texture_Sampler;

        public Texture_Sampler(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.Sampler> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material()
                {
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                    {
                        BaseColorTexture = new Runtime.Texture()
                        {
                            Source = baseColorTextureImage,
                            Sampler = new Runtime.Sampler(),
                        },
                    },
                };

                // Apply the common properties to the gltf.
                meshPrimitive.TextureCoordSets = new List<List<Vector2>>()
                {
                    new List<Vector2>()
                    {
                        new Vector2( 1.3f, 1.3f),
                        new Vector2(-0.3f, 1.3f),
                        new Vector2(-0.3f,-0.3f),
                        new Vector2( 1.3f,-0.3f)
                    }
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture.Sampler);

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
                    }),
                };
            }

            void SetWrapTtoClampToEdge(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.WrapT = glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE;
                properties.Add(new Property(PropertyName.WrapT, sampler.WrapT));
            }

            void SetWrapTtoMirroredRepeat(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.WrapT = glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT;
                properties.Add(new Property(PropertyName.WrapT, sampler.WrapT));
            }

            void SetWrapStoClampToEdge(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.WrapS = glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE;
                properties.Add(new Property(PropertyName.WrapS, sampler.WrapS));
            }

            void SetWrapStoMirroredRepeat(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.WrapS = glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT;
                properties.Add(new Property(PropertyName.WrapS, sampler.WrapS));
            }

            void SetMagFilterToNearest(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MagFilter = glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST;
                properties.Add(new Property(PropertyName.MagFilter, sampler.MagFilter));
            }

            void SetMagFilterToLinear(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MagFilter = glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR;
                properties.Add(new Property(PropertyName.MagFilter, sampler.MagFilter));
            }

            void SetMinFilterToNearest(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MinFilter = glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST;
                properties.Add(new Property(PropertyName.MinFilter, sampler.MinFilter));
            }

            void SetMinFilterToLinear(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MinFilter = glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR;
                properties.Add(new Property(PropertyName.MinFilter, sampler.MinFilter));
            }
            void 
                SetMinFilterToNearestMipmapNearest(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MinFilter = glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST;
                properties.Add(new Property(PropertyName.MinFilter, sampler.MinFilter));
            }

            void SetMinFilterToLinearMipmapNearest(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MinFilter = glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST;
                properties.Add(new Property(PropertyName.MinFilter, sampler.MinFilter));
            }

            void SetMinFilterToNearestMipmapLinear(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MinFilter = glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR;
                properties.Add(new Property(PropertyName.MinFilter, sampler.MinFilter));
            }

            void SetMinFilterToLinearMipmapLinear(List<Property> properties, Runtime.Sampler sampler)
            {
                sampler.MinFilter = glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR;
                properties.Add(new Property(PropertyName.MinFilter, sampler.MinFilter));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, sampler) => {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, sampler) => {
                    SetWrapTtoClampToEdge(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetWrapTtoMirroredRepeat(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetWrapStoClampToEdge(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetWrapStoMirroredRepeat(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMagFilterToNearest(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMagFilterToLinear(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMinFilterToNearest(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMinFilterToLinear(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMinFilterToNearestMipmapNearest(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMinFilterToLinearMipmapNearest(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMinFilterToNearestMipmapLinear(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetMinFilterToLinearMipmapLinear(properties, sampler);
                }),
                CreateModel((properties, sampler) => {
                    SetWrapTtoClampToEdge(properties, sampler);
                    SetWrapStoClampToEdge(properties, sampler);
                    SetMagFilterToNearest(properties, sampler);
                    SetMinFilterToNearest(properties, sampler);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
