using System;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Node_Animation : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Node_Animation;

        public Node_Animation(List<string> imageList)
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
                node. = new Vector3(-2, 2, -2);
                properties.Add(new Property(PropertyName.Translation, node.));
            }

            void SetRotation(List<Property> properties, Runtime.Node node)
            {

                node. = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
                properties.Add(new Property(PropertyName.Rotation, ));
            }

            void SetScale(List<Property> properties, Runtime.Node node)
            {
                node. = new Vector3(1.2f, 1.2f, 1.2f);
                properties.Add(new Property(PropertyName.Scale, node.));
            }

            void SetInterpolation(List<Property> properties, Runtime.Node node, var interpolationType)
            {

                node.
                properties.Add(new Property(PropertyName.Interpolation, matrixTRS));
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
                    SetRotation(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetScale(properties, node);
                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                    SetInterpolation(properties, node, interpolationType step);
                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                    SetInterpolation(properties, node, interpolationType cubicspline);
                }),
                CreateModel((properties, node) => {
                    SetRotation(properties, node);
                    SetInterpolation(properties, node, interpolationType cubicspline);
                }),
                CreateModel((properties, node) => {
                    // Curve that doesn't start at zero?
                }),
                CreateModel((properties, node) => {
                    SetTranslation(properties, node);
                    SetRotation(properties, node);
                    SetScale(properties, node);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
