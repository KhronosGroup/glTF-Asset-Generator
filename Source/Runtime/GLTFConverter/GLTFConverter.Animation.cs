using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal partial class GLTFConverter
    {
        /// <summary>
        /// Converts runtime animation to schema.
        /// </summary>
        private glTFLoader.Schema.Animation ConvertAnimationToSchema(Animation runtimeAnimation)
        {
            var animation = CreateInstance<glTFLoader.Schema.Animation>();
            var animationChannels = new List<glTFLoader.Schema.AnimationChannel>();
            var animationSamplers = new List<glTFLoader.Schema.AnimationSampler>();

            foreach (var runtimeAnimationChannel in runtimeAnimation.Channels)
            {
                var animationChannel = new glTFLoader.Schema.AnimationChannel();
                var targetNode = runtimeAnimationChannel.Target.Node;
                var sceneIndex = 0;
                if (runtimeGLTF.MainScene.HasValue)
                {
                    sceneIndex = runtimeGLTF.MainScene.Value;
                }

                var targetNodeIndex = runtimeGLTF.Scenes.ElementAt(sceneIndex).Nodes.FindIndex(x => x.Equals(targetNode));
                var runtimeSampler = runtimeAnimationChannel.Sampler;

                // Create Animation Channel

                // Write Input Key frames
                var inputBufferView = CreateBufferView(bufferIndex, "Animation Sampler Input", runtimeSampler.InputKeys.Count() * 4, (int)geometryData.Writer.BaseStream.Position, null);
                bufferViews.Add(inputBufferView);

                geometryData.Writer.Write(runtimeSampler.InputKeys);

                var min = new[] { runtimeSampler.InputKeys.Min() };
                var max = new[] { runtimeSampler.InputKeys.Max() };
                var inputAccessor = CreateAccessor(bufferViews.Count - 1, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeSampler.InputKeys.Count(), "Animation Sampler Input", max, min, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                accessors.Add(inputAccessor);

                var inputAccessorIndex = accessors.Count - 1;

                animationChannel.Target = new glTFLoader.Schema.AnimationChannelTarget
                {
                    Node = targetNodeIndex
                };

                switch (runtimeAnimationChannel.Target.Path)
                {
                    case AnimationChannelTarget.PathEnum.TRANSLATION:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.translation;
                        break;
                    case AnimationChannelTarget.PathEnum.ROTATION:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.rotation;
                        break;
                    case AnimationChannelTarget.PathEnum.SCALE:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.scale;
                        break;
                    case AnimationChannelTarget.PathEnum.WEIGHT:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.weights;
                        break;
                    default:
                        throw new NotSupportedException($"Animation target path {runtimeAnimationChannel.Target.Path} not supported!");
                }

                // Write the output key frame data
                var outputByteOffset = (int)geometryData.Writer.BaseStream.Position;

                var runtimeSamplerType = runtimeSampler.GetType();
                var runtimeSamplerGenericTypeDefinition = runtimeSamplerType.GetGenericTypeDefinition();
                var runtimeSamplerGenericTypeArgument = runtimeSamplerType.GenericTypeArguments[0];

                glTFLoader.Schema.Accessor.TypeEnum outputAccessorType;
                if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                {
                    outputAccessorType = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
                }
                else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                {
                    outputAccessorType = glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                }
                else
                {
                    throw new ArgumentException("Unsupported animation accessor type!");
                }

                var outputAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;

                glTFLoader.Schema.AnimationSampler.InterpolationEnum samplerInterpolation;
                if (runtimeSamplerGenericTypeDefinition == typeof(StepAnimationSampler<>))
                {
                    samplerInterpolation = glTFLoader.Schema.AnimationSampler.InterpolationEnum.STEP;

                    if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                    {
                        var specificRuntimeSampler = (StepAnimationSampler<Vector3>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                    {
                        var specificRuntimeSampler = (StepAnimationSampler<Quaternion>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported animation sampler component type!");
                    }
                }
                else if (runtimeSamplerGenericTypeDefinition == typeof(LinearAnimationSampler<>))
                {
                    samplerInterpolation = glTFLoader.Schema.AnimationSampler.InterpolationEnum.LINEAR;

                    if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                    {
                        var specificRuntimeSampler = (LinearAnimationSampler<Vector3>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                    {
                        var specificRuntimeSampler = (LinearAnimationSampler<Quaternion>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported animation sampler type!");
                    }
                }
                else if (runtimeSamplerGenericTypeDefinition == typeof(CubicSplineAnimationSampler<>))
                {
                    samplerInterpolation = glTFLoader.Schema.AnimationSampler.InterpolationEnum.CUBICSPLINE;

                    if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                    {
                        var specificRuntimeSampler = (CubicSplineAnimationSampler<Vector3>)runtimeSampler;
                        specificRuntimeSampler.OutputKeys.ForEach(key =>
                        {
                            geometryData.Writer.Write(key.InTangent);
                            geometryData.Writer.Write(key.Value);
                            geometryData.Writer.Write(key.OutTangent);
                        });
                    }
                    else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                    {
                        var specificRuntimeSampler = (CubicSplineAnimationSampler<Quaternion>)runtimeSampler;
                        specificRuntimeSampler.OutputKeys.ForEach(key =>
                        {
                            geometryData.Writer.Write(key.InTangent);
                            geometryData.Writer.Write(key.Value);
                            geometryData.Writer.Write(key.OutTangent);
                        });
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                var outputCount = samplerInterpolation == glTFLoader.Schema.AnimationSampler.InterpolationEnum.CUBICSPLINE ? inputAccessor.Count * 3 : inputAccessor.Count;
                var outputByteLength = (int)geometryData.Writer.BaseStream.Position - outputByteOffset;
                var outputBufferView = CreateBufferView(bufferIndex, "Animation Sampler Output", outputByteLength, outputByteOffset, null);
                bufferViews.Add(outputBufferView);

                var outputAccessor = CreateAccessor(bufferViews.Count - 1, 0, outputAccessorComponentType, outputCount, "Animation Sampler Output", null, null, outputAccessorType, null);
                accessors.Add(outputAccessor);
                var outputAccessorIndex = accessors.Count - 1;

                // Create Animation Sampler
                var animationSampler = new glTFLoader.Schema.AnimationSampler
                {
                    Interpolation = samplerInterpolation,
                    Input = inputAccessorIndex,
                    Output = outputAccessorIndex
                };

                animationChannels.Add(animationChannel);
                animationSamplers.Add(animationSampler);

                // This needs to be improved to support instancing
                animationChannel.Sampler = animationSamplers.Count() - 1;
            }

            animation.Channels = animationChannels.ToArray();
            animation.Samplers = animationSamplers.ToArray();

            return animation;
        }
    }
}
