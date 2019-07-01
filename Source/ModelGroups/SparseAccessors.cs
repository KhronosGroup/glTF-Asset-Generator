using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using static glTFLoader.Schema.Sampler;
using static AssetGenerator.Runtime.AnimationChannelTarget.PathEnum;

namespace AssetGenerator
{
    internal class SparseAccessors : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.SparseAccessors;

        public SparseAccessors(List<string> imageList)
        {
            Runtime.Image baseColorTextureImageA = UseTexture(imageList, "BaseColor_A");
            Runtime.Image baseColorTextureImageB = UseTexture(imageList, "BaseColor_B");
            Runtime.Image baseColorTextureImageCube = UseTexture(imageList, "BaseColor_Cube");

            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>> setProperties)
            {
                var properties = new List<Property>();
                var meshPrimitive = MeshPrimitive.CreateCube();
                var animations = new List<Runtime.Animation>();

                // Apply the properties that are specific to this gltf.
                setProperties(properties);

                // Create the gltf object.
                var model = new Model
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
                                }
                            }
                        } 
                    }, animations: animations),
                    Animated = true,
                };

                return model;
            }

            Models = new List<Model>
            {
                CreateModel((properties) =>
                {
                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.Description, "Has a bufferView."));
                }),
                CreateModel((properties) =>
                {
                    properties.Add(new Property(PropertyName.SparseAccessor, "Animation Sampler Output"));
                    properties.Add(new Property(PropertyName.Description, "Does not have a bufferView."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
