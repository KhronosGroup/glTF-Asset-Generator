using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    internal class Node_Attribute : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Node_Attribute;

        public Node_Attribute(List<string> imageList)
        {
            var baseColorTexture = new Runtime.Texture { Source = UseTexture(imageList, "BaseColor_Nodes") };
            var normalTexture = new Runtime.Texture { Source = UseTexture(imageList, "Normal_Nodes") };
            var metallicRoughnessTexture = new Runtime.Texture { Source = UseTexture(imageList, "MetallicRoughness_Nodes") };
            
            // Track the common properties for use in the readme.
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTexture.Source.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalTexture.Source.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.MetallicRoughnessTexture, metallicRoughnessTexture.Source.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Runtime.Node> setProperties)
            {
                var properties = new List<Property>();
                List<Runtime.Node> nodes = Nodes.CreateMultiNode();

                // Apply the common properties to the gltf.
                foreach (var node in nodes)
                {
                    node.Mesh.MeshPrimitives.First().Material = new Runtime.Material
                    {
                        NormalTexture = new Runtime.NormalTextureInfo { Texture = normalTexture },
                        PbrMetallicRoughness = new Runtime.PbrMetallicRoughness
                        {
                            BaseColorTexture = new Runtime.TextureInfo { Texture = baseColorTexture },
                            MetallicRoughnessTexture = new Runtime.TextureInfo { Texture = metallicRoughnessTexture },
                        },
                    };
                }

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes[1]);

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Runtime.Scene
                    {
                        Nodes = new[]
                        {
                            nodes[0]
                        }
                    })
                };
            }

            void SetTranslation(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(-2.0f, 2.0f, -2.0f);
                properties.Add(new Property(PropertyName.Translation, node.Translation.ToReadmeString()));
            }

            void SetTranslationX(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(-2.0f, 0.0f, 0.0f);
                properties.Add(new Property(PropertyName.Translation, node.Translation.ToReadmeString()));
            }

            void SetTranslationY(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(0.0f, 2.0f, 0.0f);
                properties.Add(new Property(PropertyName.Translation, node.Translation.ToReadmeString()));
            }

            void SetTranslationZ(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(0.0f, 0.0f, -2.0f);
                properties.Add(new Property(PropertyName.Translation, node.Translation.ToReadmeString()));
            }

            void SetRotation(List<Property> properties, Runtime.Node node)
            {
                var rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
                node.Rotation = rotation;
                properties.Add(new Property(PropertyName.Rotation, rotation.ToReadmeString()));
            }

            void SetScale(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(1.2f, 1.2f, 1.2f);
                properties.Add(new Property(PropertyName.Scale, node.Scale.ToReadmeString()));
            }

            void SetMatrix(List<Property> properties, Runtime.Node node)
            {
                Matrix4x4 matrixT = Matrix4x4.CreateTranslation(new Vector3(-2.0f, 2.0f, -2.0f));
                Matrix4x4 matrixR = Matrix4x4.CreateRotationY(FloatMath.Pi);
                Matrix4x4 matrixS = Matrix4x4.CreateScale(1.2f);
                Matrix4x4 matrixTRS = Matrix4x4.Multiply(Matrix4x4.Multiply(matrixS, matrixR), matrixT);
                node.Matrix = matrixTRS;
                properties.Add(new Property(PropertyName.Matrix, matrixTRS.ToReadmeString()));
            }

            Models = new List<Model>
            {
                CreateModel((properties, node) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, node) =>
                {
                    SetTranslation(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetTranslationX(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetTranslationY(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetTranslationZ(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetRotation(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetTranslation(properties, node);
                    SetRotation(properties, node);
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) =>
                {
                    SetMatrix(properties, node);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
