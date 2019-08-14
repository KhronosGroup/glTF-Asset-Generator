using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal class Node_NegativeScale : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Node_NegativeScale;

        public Node_NegativeScale(List<string> imageList)
        {
            Image baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");
            Image normalImage = UseTexture(imageList, "Normal_Nodes");
            Image metallicRoughnessTextureImage = UseTexture(imageList, "MetallicRoughness_Nodes");

            // Track the common properties for use in the readme.
            var translationValue = new Vector3(0, 2, 0);
            Matrix4x4 matrixTranslationValue = Matrix4x4.CreateTranslation(translationValue);
            CommonProperties.Add(new Property(PropertyName.Translation, translationValue.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalImage.ToReadmeString()));
            CommonProperties.Add(new Property(PropertyName.MetallicRoughnessTexture, metallicRoughnessTextureImage.ToReadmeString()));

            Model CreateModel(Action<List<Property>, Node, Node> setProperties)
            {
                var properties = new List<Property>();
                List<Node> nodes = Nodes.CreateMultiNode();

                // Apply the common properties to the gltf.
                foreach (var node in nodes)
                {
                    node.Mesh.MeshPrimitives.First().Material = new Runtime.Material
                    {
                        NormalTexture = new Texture { Source = normalImage },
                        MetallicRoughnessMaterial = new PbrMetallicRoughness
                        {
                            BaseColorTexture = new Texture { Source = baseColorTextureImage },
                            MetallicRoughnessTexture = new Texture { Source = metallicRoughnessTextureImage },
                        },
                    };
                }

                // Apply the properties that are specific to this gltf.
                setProperties(properties, nodes[0], nodes[1]);

                // Applies a translation to avoid clipping the other node. 
                // Models with a matrix applied have the translation applied in that matrix.
                if (properties.Find(e => e.Name == PropertyName.Matrix) == null)
                {
                    nodes[1].Translation = translationValue;
                }

                // Create the gltf object.
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => new Scene
                    {
                        Nodes = new[]
                        {
                            nodes[0]
                        }
                    })
                };
            }

            void SetMatrixScaleX(List<Property> properties, Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1.0f, 1.0f, 1.0f)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix.ToReadmeString()));
            }

            void SetMatrixScaleXY(List<Property> properties, Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1.0f, -1.0f, 1.0f)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix.ToReadmeString()));
            }

            void SetMatrixScaleXYZ(List<Property> properties, Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1.0f, -1.0f, -1.0f)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix.ToReadmeString()));
            }

            void SetScaleX(List<Property> properties, Node node)
            {
                node.Scale = new Vector3(-1.0f, 1.0f, 1.0f);
                properties.Add(new Property(PropertyName.Scale, node.Scale.ToReadmeString()));
            }

            void SetScaleXY(List<Property> properties, Node node)
            {
                node.Scale = new Vector3(-1.0f, -1.0f, 1.0f);
                properties.Add(new Property(PropertyName.Scale, node.Scale.ToReadmeString()));
            }

            void SetScaleXYZ(List<Property> properties, Node node)
            {
                node.Scale = new Vector3(-1.0f, -1.0f, -1.0f);
                properties.Add(new Property(PropertyName.Scale, node.Scale.ToReadmeString()));
            }

            void SetVertexNormal(List<Property> properties, Node nodeZero, Node nodeOne)
            {
                var normals = new Accessor(Nodes.GetMultiNodeNormals());
                nodeZero.Mesh.MeshPrimitives.First().Normals = normals;
                nodeOne.Mesh.MeshPrimitives.First().Normals = normals;
                properties.Add(new Property(PropertyName.VertexNormal, ":white_check_mark:"));
            }

            void SetVertexTangent(List<Property> properties, Node nodeZero, Node nodeOne)
            {
                var tangents = new Accessor(Nodes.GetMultiNodeTangents());
                nodeZero.Mesh.MeshPrimitives.First().Tangents = tangents;
                nodeOne.Mesh.MeshPrimitives.First().Tangents = tangents;
                properties.Add(new Property(PropertyName.VertexTangent, ":white_check_mark:"));
            }

            Models = new List<Model>
            {
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetMatrixScaleX(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetMatrixScaleXY(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetMatrixScaleXYZ(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleX(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleXY(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleXYZ(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleX(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleXY(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleXYZ(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleX(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleXY(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) =>
                {
                    SetScaleXYZ(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
