﻿using System;
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
            var baseColorTextureImage = UseTexture(imageList, "BaseColor_Nodes");
            var normalImage = UseTexture(imageList, "Normal_Nodes");
            var metallicRoughnessTextureImage = UseTexture(imageList, "MetallicRoughness_Nodes");

            // Track the common properties for use in the readme.
            var translationValue = new Vector3(0, 2, 0);
            var matrixTranslationValue = Matrix4x4.CreateTranslation(translationValue); 
            CommonProperties.Add(new Property(PropertyName.Translation, translationValue));
            CommonProperties.Add(new Property(PropertyName.BaseColorTexture, baseColorTextureImage));
            CommonProperties.Add(new Property(PropertyName.NormalTexture, normalImage));
            CommonProperties.Add(new Property(PropertyName.MetallicRoughnessTexture, metallicRoughnessTextureImage));

            Model CreateModel(Action<List<Property>, Runtime.Node, Runtime.Node> setProperties)
            {
                var properties = new List<Property>();
                var gltf = Gltf.CreateMultiNode();
                var nodes = new[]
                {
                    gltf.Scenes.First().Nodes.First(),
                    gltf.Scenes.First().Nodes.First().Children.First(),
                };

                // Apply the common properties to the gltf.
                foreach (var node in nodes)
                {
                    node.Mesh.MeshPrimitives.First().Material = new Runtime.Material()
                    {
                        NormalTexture = new Runtime.Texture() { Source = normalImage },
                        MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness()
                        {
                            BaseColorTexture = new Runtime.Texture() { Source = baseColorTextureImage },
                            MetallicRoughnessTexture = new Runtime.Texture() { Source = metallicRoughnessTextureImage },
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

                // Create the gltf object
                return new Model
                {
                    Properties = properties,
                    GLTF = CreateGLTF(() => gltf.Scenes.First()),
                };
            }

            void SetMatrixScaleX(List<Property> properties, Runtime.Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, 1, 1)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix));
            }

            void SetMatrixScaleXY(List<Property> properties, Runtime.Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, -1, 1)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix));
            }

            void SetMatrixScaleXYZ(List<Property> properties, Runtime.Node node)
            {
                node.Matrix = Matrix4x4.Multiply(Matrix4x4.CreateScale(new Vector3(-1, -1, -1)), matrixTranslationValue);
                properties.Add(new Property(PropertyName.Matrix, node.Matrix));
            }

            void SetScaleX(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(-1, 1, 1);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetScaleXY(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(-1, -1, 1);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetScaleXYZ(List<Property> properties, Runtime.Node node)
            {
                node.Scale = new Vector3(-1, -1, -1);
                properties.Add(new Property(PropertyName.Scale, node.Scale));
            }

            void SetVertexNormal(List<Property> properties, Runtime.Node nodeZero, Runtime.Node nodeOne)
            {
                var normals = Gltf.GetMultiNodeNormals();
                nodeZero.Mesh.MeshPrimitives.First().Normals = normals;
                nodeOne.Mesh.MeshPrimitives.First().Normals = normals;
                properties.Add(new Property(PropertyName.VertexNormal, ":white_check_mark:"));
            }

            void SetVertexTangent(List<Property> properties, Runtime.Node nodeZero, Runtime.Node nodeOne)
            {
                var tangents = Gltf.GetMultiNodeTangents();
                nodeZero.Mesh.MeshPrimitives.First().Tangents = tangents;
                nodeOne.Mesh.MeshPrimitives.First().Tangents = tangents;
                properties.Add(new Property(PropertyName.VertexTangent, ":white_check_mark:"));
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, nodeZero, nodeOne) => {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetMatrixScaleX(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetMatrixScaleXY(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetMatrixScaleXYZ(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleX(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXY(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXYZ(properties, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleX(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXY(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXYZ(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleX(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXY(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
                CreateModel((properties, nodeZero, nodeOne) => {
                    SetScaleXYZ(properties, nodeOne);
                    SetVertexNormal(properties, nodeZero, nodeOne);
                    SetVertexTangent(properties, nodeZero, nodeOne);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
