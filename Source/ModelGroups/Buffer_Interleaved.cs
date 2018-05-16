using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Buffer_Interleaved : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Buffer_Interleaved;

        public Buffer_Interleaved(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Grey");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();

                // Apply the common properties to the gltf.
                meshPrimitive.Interleave = true;
                meshPrimitive.Colors = new List<Vector4>()
                {
                    new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                    new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                    new Vector4( 1.0f, 1.0f, 0.0f, 0.2f),
                    new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
                }; 
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

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

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

            void SetUvTypeFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordsComponentType = Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                properties.Add(new Property(PropertyName.VertexUV0, "Float"));
            }

            void SetUvTypeTypeByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordsComponentType = Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE;
                properties.Add(new Property(PropertyName.VertexUV0, "Byte"));
            }

            void SetUvTypeTypeShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordsComponentType = Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT;
                properties.Add(new Property(PropertyName.VertexUV0, "Short"));
            }

            void SetColorTypeFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                meshPrimitive.ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                properties.Add(new Property(PropertyName.VertexColor, "Vector3 Float"));
            }

            void SetColorTypeByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                meshPrimitive.ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                properties.Add(new Property(PropertyName.VertexColor, "Vector3 Byte"));
            }

            void SetColorTypeShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                meshPrimitive.ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                properties.Add(new Property(PropertyName.VertexColor, "Vector3 Short"));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) => {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeByte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeShort(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetUvTypeTypeByte(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetUvTypeTypeShort(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
