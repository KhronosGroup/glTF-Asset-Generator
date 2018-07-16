using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class Animation_Skin : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Skin;

        public Animation_Skin(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.GLTF, IEnumerable<Runtime.Node>> setProperties)
            {
                var properties = new List<Property>();
                var planeSkinScene = Scene.CreatePlaneWithSkin();

                // Apply the common properties to the gltf.


                // Create the gltf object
                Runtime.GLTF gltf = CreateGLTF(() => planeSkinScene);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf, planeSkinScene.Nodes);

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
            }

            void AnimateWithRotation(Runtime.GLTF gltf)
            {
                gltf.Animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = new List<Runtime.AnimationChannel>
                        {
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = gltf.Scenes.First().Nodes.ElementAt(1).Children.First(),
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            }
                        }
                    }
                };
                gltf.Animations.First().Channels.First().Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
                    {
                        new Quaternion(0.0f, 0.0f, -0.70711f, 0.70711f),
                        new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                        new Quaternion(0.0f, 0.0f, -0.70711f, 0.70711f),
                    });
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf, nodes) => {
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints."));
                }),
                CreateModel((properties, gltf, nodes) => {
                    AnimateWithRotation(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints, one of which is animated with a rotation."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
