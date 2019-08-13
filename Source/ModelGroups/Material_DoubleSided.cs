using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Material_DoubleSided : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_DoubleSided;

        public Material_DoubleSided(List<string> imageList)
        {
            Runtime.Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Plane");
            Runtime.Image normalImage = UseTexture(imageList, "Normal_Plane");

            // Track the common properties for use in the readme.
            var doubleSidedValue = true;
            CommonProperties.Add(new Property(PropertyName.DoubleSided, doubleSidedValue.ToString()));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.DoubleSided = doubleSidedValue;
                meshPrimitive.Material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture { Source = baseColorTextureImage };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

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

            void SetVertexNormal(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeNormalsValue = MeshPrimitive.GetSinglePlaneNormals();
                meshPrimitive.Normals = planeNormalsValue;
                properties.Add(new Property(PropertyName.VertexNormal, ((IEnumerable<Vector3>)planeNormalsValue.Values).ToReadmeString()));
            }

            void SetVertexTangent(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeTangentValue = MeshPrimitive.GetSinglePlaneTangents();
                meshPrimitive.Tangents = planeTangentValue;
                properties.Add(new Property(PropertyName.VertexTangent, ((IEnumerable<Vector4>)planeTangentValue.Values).ToReadmeString()));
            }

            void SetNormalTexture(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.NormalTexture = new Runtime.Texture { Source = normalImage };
                properties.Add(new Property(PropertyName.NormalTexture, normalImage.ToReadmeString()));
            }

            Models = new List<Model>
            {
                CreateModel((properties, meshPrimitive) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexNormal(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexNormal(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetVertexNormal(properties, meshPrimitive);
                    SetVertexTangent(properties, meshPrimitive);
                    SetNormalTexture(properties, meshPrimitive);
                }),
                CreateModel((properties, meshPrimitive) =>
                {
                    SetNormalTexture(properties, meshPrimitive);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
