using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Node_Attribute : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Node_Attribute;

        public Node_Attribute(List<string> imageList)
        {
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.Node> setProperties)
            {
                var properties = new List<Property>();
                var gltf = Gltf.CreateMultiNode();
                var nodes = new List<Runtime.Node>()
                {
                    gltf.Scenes[0].Nodes[0],
                    gltf.Scenes[0].Nodes[0].Children[0],
                };

                // Apply the common properties to the gltf.
                foreach (var node in nodes)
                {
                    node.Mesh.MeshPrimitives[0].Material = new Runtime.Material()
                    {
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                        {
                            BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                        },
                    };
                    node.Mesh.MeshPrimitives[0].Normals = null;
                    node.Mesh.MeshPrimitives[0].Tangents = null;
                }

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes[1]);

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => gltf.Scenes[0]),
                };
            }

            void SetTranslation(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(-2, 2, -2);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetTranslationX(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(-2, 0, 0);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetTranslationY(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(0, 2, 0);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetTranslationZ(List<Property> properties, Runtime.Node node)
            {
                node.Translation = new Vector3(0, 0, -2);
                properties.Add(new Property(PropertyName.Translation, node.Translation));
            }

            void SetRotation(List<Property> properties, Runtime.Node node)
            {
                var rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
                node.Rotation = rotation;
                properties.Add(new Property(PropertyName.Rotation, rotation));
            }

            void SetScale(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(1.2f, 1.2f, 1.2f);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetMatrix(List<Property> properties, Runtime.Node node)
            {
                var matrixT = Matrix4x4.CreateTranslation(new Vector3(-2, 2, -2));
                var matrixR = Matrix4x4.CreateRotationY(FloatMath.Pi);
                var matrixS = Matrix4x4.CreateScale(1.2f);
                var matrixTRS = Matrix4x4.Multiply(Matrix4x4.Multiply(matrixS, matrixR), matrixT);
                node.Matrix = matrixTRS;
                properties.Add(new Property(PropertyName.Matrix, matrixTRS));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, node) => {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslationX(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslationY(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslationZ(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetRotation(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                    SetRotation(properties, node);
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetMatrix(properties, node);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
