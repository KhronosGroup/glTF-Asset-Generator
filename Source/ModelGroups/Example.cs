using System;
using System.Collections.Generic;
using static glTFLoader.Schema.Material;

namespace AssetGenerator.ModelGroups
{
    // Name the class but inherit from ModelGroup
    internal class /*Example*/ : ModelGroup
    {
        // Set the ModelGroupID. The value used will need to be added to the ModelGroupId enum in ModelGroup.cs
        public override ModelGroupId Id => ModelGroupId./*Example*/;

        public /*Example*/(List<string> imageList)
        {
            UseFigure(imageList, "Indices_Primitive0");

            // Track the common properties for use in the readme.
            var doubleSided = true;
            CommonProperties.Add(new Property(PropertyName.DoubleSided, doubleSided));

            Model CreateModel(Action<List<Property>, Runtime.Material> setProperties)
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane();

                // Apply the common properties to this group's models.
                meshPrimitive.Material = new Runtime.Material
                {
                    DoubleSided = doubleSided
                };

                // Apply the properties that are specific to this model.
                setProperties(properties, meshPrimitive.Material);

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

            void SetAlphaModeBlend(List<Property> properties, Runtime.Material material)
            {
                var alphaMode = AlphaModeEnum.BLEND;
                material.AlphaMode = alphaMode;

                properties.Add(new Property(PropertyName.AlphaMode, alphaMode));
            }

            Models = new List<Model>
            {
                CreateModel((properties, material) =>
                {
                    // There are no properties set on this model.
                }),
                CreateModel((properties, material) =>
                {
                    SetAlphaModeBlend(properties, material);
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
