using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    internal class Material_DoubleSided : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Material_DoubleSided;

        public Material_DoubleSided(List<string> imageList)
        {
            var baseColorTexture = new Texture { Source = UseTexture(imageList, "BaseColor_Plane") };
            var normalTexture = new Texture { Source = UseTexture(imageList, "Normal_Plane") };

            // Track the common properties for use in the readme.
            var doubleSidedValue = true;
            CommonProperties.Add(new Property(PropertyName.DoubleSided, doubleSidedValue.ToString()));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.MeshPrimitive> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateSinglePlane();
                meshPrimitive.Material = new Runtime.Material();
                meshPrimitive.Material.PbrMetallicRoughness = new PbrMetallicRoughness();

                // Apply the common properties to the gltf.
                meshPrimitive.Material.DoubleSided = doubleSidedValue;
                meshPrimitive.Material.PbrMetallicRoughness.BaseColorTexture = new TextureInfo { Texture = baseColorTexture };

                // Apply the properties that are specific to this gltf.
                setProperties(properties, meshPrimitive);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = new List<Node>
                        {
                            new Node
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
                meshPrimitive.Normals = Data.Create(planeNormalsValue);
                properties.Add(new Property(PropertyName.VertexNormal, planeNormalsValue.ToReadmeString()));
            }

            void SetVertexTangent(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                var planeTangentValue = MeshPrimitive.GetSinglePlaneTangents();
                meshPrimitive.Tangents = Data.Create(planeTangentValue);
                properties.Add(new Property(PropertyName.VertexTangent, planeTangentValue.ToReadmeString()));
            }

            void SetNormalTexture(List<Property> properties, Runtime.MeshPrimitive meshPrimitive)
            {
                meshPrimitive.Material.NormalTexture = new NormalTextureInfo { Texture = normalTexture };
                properties.Add(new Property(PropertyName.NormalTexture, normalTexture.Source.ToReadmeString()));
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
