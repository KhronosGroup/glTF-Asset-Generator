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

            Model CreateModel(Action<List<Property>, Runtime.GLTF> setProperties)
            {
                var properties = new List<Property>();

                // Apply the common properties to the gltf.


                // Create the gltf object
                var gltf = new Runtime.GLTF();

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf);

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
            }

            void SetCommonGltf(Runtime.GLTF sourceGltf, Runtime.GLTF destinationGltf)
            {
                destinationGltf.Asset = sourceGltf.Asset;
                destinationGltf.Scenes = sourceGltf.Scenes;
            }

            void SetBasicSkin(Runtime.GLTF gltf)
            {
                var planeSkinScene = Scene.CreatePlaneWithSkin();
                Runtime.GLTF tempGltf = CreateGLTF(() => planeSkinScene);
                SetCommonGltf(tempGltf, gltf);
            }

            void SetFiveJointSkin(Runtime.GLTF gltf)
            {
                var planeSkinScene = Scene.CreateComplexPlaneWithSkin();
                Runtime.GLTF tempGltf = CreateGLTF(() => planeSkinScene);
                SetCommonGltf(tempGltf, gltf);
            }

            void AnimateWithRotation(Runtime.GLTF gltf, Runtime.Node node)
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
                                    Node = node,
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            }
                        }
                    }
                };
                var quarterTurn = (FloatMath.Pi / 2);
                gltf.Animations.First().Channels.First().Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    new[]
                    {
                        0.0f,
                        1.0f,
                        2.0f,
                    },
                    new[]
                    {
                        Quaternion.Identity,
                        Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                        Quaternion.Identity,
                    });
            }

            void AnimateFiveJointsWIthRotation(Runtime.GLTF gltf)
            {
                var rootJoint = gltf.Scenes.First().Nodes.ElementAt(1);
                var rootMidJoint = rootJoint.Children.First();
                var midJoint = rootMidJoint.Children.First();
                var midTopJoint = midJoint.Children.First();
                var TopJoint = midTopJoint.Children.First();

                AnimateWithRotation(gltf, rootJoint);
                AnimateWithRotation(gltf, rootMidJoint);
                AnimateWithRotation(gltf, midJoint);
                AnimateWithRotation(gltf, midTopJoint);
                AnimateWithRotation(gltf, TopJoint);
            }

            void SetInverseBindMatrix(Runtime.GLTF gltf)
            {
                var rootJoint = gltf.Scenes.First().Nodes.ElementAt(1);
                var midJoint = gltf.Scenes.First().Nodes.ElementAt(1).Children.First();
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints."));
                }),
                CreateModel((properties, gltf) => {
                    SetBasicSkin(gltf);
                    AnimateWithRotation(gltf, gltf.Scenes.First().Nodes.ElementAt(1).Children.First());
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints, one of which is animated with a rotation."));
                }),
                CreateModel((properties, gltf) => {
                    SetFiveJointSkin(gltf);
                    AnimateFiveJointsWIthRotation(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with five joints."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
