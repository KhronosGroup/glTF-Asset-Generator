﻿using System;
using System.Numerics;
using System.Collections.Generic;

namespace AssetGenerator
{
    internal class Mesh_PrimitiveAttribute : ModelGroup
    {
        public override ModelGroupName Name => ModelGroupName.Mesh_PrimitiveAttribute;

        public Mesh_PrimitiveAttribute(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            var normalImage = UseTexture(imageList, "Normal_Plane");

            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };

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

            void SetVertexUVFloat(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordsComponentType = Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordsComponentType, group: 1));
            }

            void SetVertexUVByte(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordsComponentType = Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordsComponentType, group: 1));
            }

            void SetVertexUVShort(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.TextureCoordsComponentType = Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT;
                properties.Add(new Property(PropertyName.VertexUV0, meshPrimitive.TextureCoordsComponentType, group: 1));
            }

            void SetVertexNormal(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeNormalsValue = new List<Vector3>()
                {
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f),
                    new Vector3( 0.0f, 0.0f, 1.0f)
                };
                meshPrimitive.Normals = planeNormalsValue;
                properties.Add(new Property(PropertyName.VertexNormal, planeNormalsValue));
            }

            void SetVertexTangent(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeTangentValue = new List<Vector4>()
                {
                    new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, 0.0f, 0.0f, 1.0f)
                };
                meshPrimitive.Tangents = planeTangentValue;
                properties.Add(new Property(PropertyName.VertexTangent, planeTangentValue));
            }

            void SetNormalTexture(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.NormalTexture = new Runtime.Texture { Source = normalImage };
                properties.Add(new Property(PropertyName.NormalTexture, normalImage));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVFloat(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVByte(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVShort(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetVertexNormal(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetVertexNormal(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) => {
                    SetVertexUVFloat(properties, meshPrimitive);
                    SetVertexNormal(properties, meshPrimitive);
                    SetVertexTangent(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
