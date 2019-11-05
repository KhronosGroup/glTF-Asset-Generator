using System;
using System.Numerics;
using AssetGenerator.Runtime;
using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    internal class Buffer_Misc : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Buffer_Misc;

        public Buffer_Misc(List<string> imageList)
        {
            UseFigure(imageList, "NYI");

            // Track the common properties for use in the readme.
            var baseColorFactorGreen = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            CommonProperties.Add(new Property(PropertyName.BaseColorFactor, baseColorFactorGreen.ToReadmeString()));

            Model CreateModel(Action<List<Property>, AnimationChannel, Node> setProperties) 
            {
                var properties = new List<Property>();
                Runtime.MeshPrimitive meshPrimitive = MeshPrimitive.CreateSinglePlane(includeTextureCoords: false);
                meshPrimitive.Material = new Runtime.Material
                {
                    PbrMetallicRoughness = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorFactor = baseColorFactorGreen
                    }
                };

                // Apply the common properties to the glTF.
                var node = new Node
                {
                    Mesh = new Runtime.Mesh
                    {
                        MeshPrimitives = new[]
                        {
                            meshPrimitive
                        }
                    }
                };
                var channel = new AnimationChannel();

                // Apply the proerties that are specific to this glTF
                setProperties(properties, channel, node);

                // Create the glTF object
                GLTF gltf = CreateGLTF(() => new Scene
                {
                    Nodes = new[]
                    {
                        node
                    },
                });
                gltf.Animations = new[]
                {
                    new Animation
                    {
                        Channels = new List<AnimationChannel>
                        {
                            channel
                        }
                    }
                };
                return new Model
                {
                    Properties = properties,
                    GLTF = gltf,
                    Animated = true,
                    SeparateBuffers = true,
                };
            }

            void setTranslationChanneltarget(AnimationChannel channel, Node node)
            {
                channel.Target = new AnimationChannelTarget
                {
                    Node = node,
                    Path = AnimationChannelTargetPath.Translation,
                };
            }

            void SetLinearSamplerForTranslation(AnimationChannel channel)
            {
                channel.Sampler = new AnimationSampler
                {
                    Interpolation = AnimationSamplerInterpolation.Linear,
                    Input = Data.Create(new[]
                    {
                        0.0f,
                        2.0f,
                        4.0f,
                    }),
                    Output = Data.Create(new[]
                    {
                        new Vector3(-0.1f, 0.0f, 0.0f),
                        new Vector3(0.1f, 0.0f, 0.0f),
                        new Vector3(-0.1f, 0.0f, 0.0f),
                    }),
                };
            }

            Models = new List<Model>
            {
                CreateModel((properties, channel, node) =>
                {
                    setTranslationChanneltarget(channel, node);
                    SetLinearSamplerForTranslation(channel);
                    properties.Add(new Property(PropertyName.Description, "The mesh primitive and animation data are stored in separate buffers."));
                })
            };

            GenerateUsedPropertiesList();
        }
    }
}