using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Buffer_Interleaved : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Buffer_Interleaved;

        public Buffer_Interleaved(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Grey") };

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane();

                // Apply the common properties to the gltf.
                meshPrimitive.Interleave = true;
                meshPrimitive.Colors = Data.Create
                (
                    new[]
                    {
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(1.0f, 0.0f, 0.0f),
                        new Vector3(1.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                    }
                );
                meshPrimitive.Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorTexture = new TextureInfo { Texture = baseColorTexture },
                    },
                };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

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
                    }),
                };
            }

            void SetUvTypeFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TexCoords0.OutputType = DataType.Float;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TexCoords0.OutputType.ToReadmeString()));
            }

            void SetUvTypeByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TexCoords0.OutputType = DataType.NormalizedUnsignedByte;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TexCoords0.OutputType.ToReadmeString()));
            }

            void SetUvTypeShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TexCoords0.OutputType = DataType.NormalizedUnsignedShort;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TexCoords0.OutputType.ToReadmeString()));
            }

            void SetColorTypeFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.OutputType = DataType.Float;
                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
            }

            void SetColorTypeByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.OutputType = DataType.NormalizedUnsignedByte;
                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
            }

            void SetColorTypeShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Colors.OutputType = DataType.NormalizedUnsignedShort;
                properties.Add(new Property(PropertyName.VertexColor, $"Vector3 {meshPrimitive.Colors.OutputType.ToReadmeString()}"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeByte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeFloat(properties, meshPrimitive);
                    SetColorTypeShort(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeByte(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetUvTypeShort(properties, meshPrimitive);
                    SetColorTypeFloat(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
