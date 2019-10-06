using AssetGenerator.Runtime;
using AssetGenerator.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static glTFLoader.Schema.AnimationChannelTarget;
using static glTFLoader.Schema.AnimationSampler;
using static glTFLoader.Schema.Material;
using static glTFLoader.Schema.MeshPrimitive;
using static glTFLoader.Schema.Sampler;
using Schema = glTFLoader.Schema;

namespace AssetGenerator.Conversion
{
    internal partial class Converter
    {
        private readonly List<Schema.Accessor> accessors = new List<Schema.Accessor>();
        private readonly List<Schema.Animation> animations = new List<Schema.Animation>();
        private readonly List<Schema.Buffer> buffers = new List<Schema.Buffer>();
        private readonly List<Schema.BufferView> bufferViews = new List<Schema.BufferView>();
        private readonly List<Schema.Image> images = new List<Schema.Image>();
        private readonly List<Schema.Material> materials = new List<Schema.Material>();
        private readonly List<Schema.Mesh> meshes = new List<Schema.Mesh>();
        private readonly List<Schema.Node> nodes = new List<Schema.Node>();
        private readonly List<Schema.Sampler> samplers = new List<Schema.Sampler>();
        private readonly List<Schema.Scene> scenes = new List<Schema.Scene>();
        private readonly List<Schema.Skin> skins = new List<Schema.Skin>();
        private readonly List<Schema.Texture> textures = new List<Schema.Texture>();

        private readonly Dictionary<Data, int> accessorToIndexCache = new Dictionary<Data, int>();
        private readonly Dictionary<Image, int> imageToIndexCache = new Dictionary<Image, int>();
        private readonly Dictionary<Material, int> materialToIndexCache = new Dictionary<Material, int>();
        private readonly Dictionary<Mesh, int> meshToIndexCache = new Dictionary<Mesh, int>();
        private readonly Dictionary<Node, int> nodeToIndexCache = new Dictionary<Node, int>();
        private readonly Dictionary<Sampler, int> samplerToIndexCache = new Dictionary<Sampler, int>();
        private readonly Dictionary<Skin, int> skinToIndexCache = new Dictionary<Skin, int>();
        private readonly Dictionary<Texture, int> textureToIndexCache = new Dictionary<Texture, int>();

        private readonly Func<Type, object> createInstance;

        private readonly Func<BinaryDataType, BinaryData> getBinaryData;
        private readonly Dictionary<BinaryData, int> binaryDataToBufferIndex = new Dictionary<BinaryData, int>();

        private class DataConvertArgs
        {
            public string AccessorName { get; }
            public string BufferViewName { get; }
            public Data Data { get; }
            public bool MinMax { get; }

            public DataConvertArgs(string accessorName, string bufferViewName, Data data, bool minMax = false)
            {
                AccessorName = accessorName;
                BufferViewName = bufferViewName;
                Data = data;
                MinMax = minMax;
            }
        }

        public Converter(Func<BinaryDataType, BinaryData> getBinaryData, Func<Type, object> createInstance = null)
        {
            this.getBinaryData = getBinaryData;
            this.createInstance = createInstance;
        }

        private T CreateInstance<T>()
        {
            return (T)createInstance(typeof(T));
        }

        private int Convert<T, RuntimeT>(RuntimeT runtimeInstance, List<T> objectsList, Dictionary<RuntimeT, int> objectToIndexCache, Action<T> action)
        {
            if (objectToIndexCache.TryGetValue(runtimeInstance, out int index))
            {
                return index;
            }

            var instance = CreateInstance<T>();
            index = objectsList.Count;
            objectToIndexCache.Add(runtimeInstance, index);
            objectsList.Add(instance);
            action(instance);
            return index;
        }

        private BinaryData GetBinaryData(BinaryDataType type)
        {
            var binaryData = this.getBinaryData(type);
            if (!binaryDataToBufferIndex.ContainsKey(binaryData))
            {
                var buffer = CreateInstance<Schema.Buffer>();
                buffer.Uri = binaryData.Name;
                var bufferIndex = buffers.Count;
                buffers.Add(buffer);
                binaryDataToBufferIndex.Add(binaryData, bufferIndex);
            }
            return binaryData;
        }

        public Schema.Gltf Convert(GLTF runtimeGltf)
        {
            var gltf = CreateInstance<Schema.Gltf>();

            if (runtimeGltf.Asset != null)
            {
                gltf.Asset = ConvertAsset(runtimeGltf.Asset);
            }

            if (runtimeGltf.Scenes != null)
            {
                foreach (var runtimeScene in runtimeGltf.Scenes)
                {
                    var scene = CreateInstance<Schema.Scene>();
                    scene.Nodes = runtimeScene.Nodes.Select(runtimeNode => ConvertNode(runtimeNode)).ToArray();
                    scenes.Add(scene);
                }
            }

            if (runtimeGltf.Scene != null)
            {
                gltf.Scene = runtimeGltf.Scenes.ToList().IndexOf(runtimeGltf.Scene);
            }

            if (runtimeGltf.Animations != null)
            {
                var binaryData = GetBinaryData(BinaryDataType.Animation);
                foreach (var runtimeAnimation in runtimeGltf.Animations)
                {
                    animations.Add(ConvertAnimation(runtimeAnimation, binaryData));
                }
            }

            if (runtimeGltf.ExtensionsUsed != null)
            {
                gltf.ExtensionsUsed = runtimeGltf.ExtensionsUsed.ToArray();
            }

            if (runtimeGltf.ExtensionsRequired != null)
            {
                gltf.ExtensionsRequired = runtimeGltf.ExtensionsRequired.ToArray();
            }

            foreach (var pair in binaryDataToBufferIndex)
            {
                buffers[pair.Value].ByteLength = checked((int)pair.Key.Writer.BaseStream.Length);
            }

            if (accessors.Any()) { gltf.Accessors = accessors.ToArray(); }
            if (animations.Any()) { gltf.Animations = animations.ToArray(); }
            if (buffers.Any()) { gltf.Buffers = buffers.ToArray(); }
            if (bufferViews.Any()) { gltf.BufferViews = bufferViews.ToArray(); }
            if (images.Any()) { gltf.Images = images.ToArray(); }
            if (materials.Any()) { gltf.Materials = materials.ToArray(); }
            if (meshes.Any()) { gltf.Meshes = meshes.ToArray(); }
            if (nodes.Any()) { gltf.Nodes = nodes.ToArray(); }
            if (samplers.Any()) { gltf.Samplers = samplers.ToArray(); }
            if (scenes.Any()) { gltf.Scenes = scenes.ToArray(); }
            if (skins.Any()) { gltf.Skins = skins.ToArray(); }
            if (textures.Any()) { gltf.Textures = textures.ToArray(); }

            return gltf;
        }

        private Schema.Asset ConvertAsset(Asset runtimeAsset)
        {
            var schemaAsset = CreateInstance<Schema.Asset>();

            if (runtimeAsset.Generator != null)
            {
                schemaAsset.Generator = runtimeAsset.Generator;
            }

            if (runtimeAsset.Version != null)
            {
                schemaAsset.Version = runtimeAsset.Version;
            }

            if (runtimeAsset.MinVersion != null)
            {
                schemaAsset.MinVersion = runtimeAsset.MinVersion;
            }

            return schemaAsset;
        }

        private int ConvertNode(Node runtimeNode)
        {
            return Convert(runtimeNode, nodes, nodeToIndexCache, node =>
            {
                if (runtimeNode.Name != null)
                {
                    node.Name = runtimeNode.Name;
                }

                if (runtimeNode.Matrix.HasValue)
                {
                    node.Matrix = runtimeNode.Matrix.Value.ToArray();
                }

                if (runtimeNode.Mesh != null)
                {
                    node.Mesh = ConvertMesh(runtimeNode.Mesh, GetBinaryData(BinaryDataType.Mesh));
                }

                if (runtimeNode.Skin != null)
                {
                    node.Skin = ConvertSkin(runtimeNode.Skin, GetBinaryData(BinaryDataType.Skin));
                }

                if (runtimeNode.Rotation.HasValue)
                {
                    node.Rotation = runtimeNode.Rotation.Value.ToArray();
                }

                if (runtimeNode.Scale.HasValue)
                {
                    node.Scale = runtimeNode.Scale.Value.ToArray();
                }

                if (runtimeNode.Translation.HasValue)
                {
                    node.Translation = runtimeNode.Translation.Value.ToArray();
                }

                if (runtimeNode.Children != null)
                {
                    node.Children = runtimeNode.Children.Select(runtimeChildNode => ConvertNode(runtimeChildNode)).ToArray();
                }
            });
        }

        private int ConvertMesh(Mesh runtimeMesh, BinaryData binaryData)
        {
            return Convert(runtimeMesh, meshes, meshToIndexCache, mesh =>
            {
                if (runtimeMesh.Name != null)
                {
                    mesh.Name = runtimeMesh.Name;
                }
                if (runtimeMesh.MeshPrimitives != null)
                {
                    var primitives = new List<Schema.MeshPrimitive>();
                    foreach (var runtimePrimitive in runtimeMesh.MeshPrimitives)
                    {
                        primitives.Add(ConvertMeshPrimitive(runtimePrimitive, binaryData));
                    }
                    mesh.Primitives = primitives.ToArray();
                }
            });
        }

        private Dictionary<string, int> ConvertMeshPrimitiveAttributes(Dictionary<string, DataConvertArgs> runtimeAttributes, bool interleave, BinaryData binaryData)
        {
            if (interleave)
            {
                return ConvertData(runtimeAttributes, binaryData, attribute: true);
            }
            else
            {
                var attributes = new Dictionary<string, int>();

                foreach (var pair in runtimeAttributes)
                {
                    attributes[pair.Key] = ConvertData(pair.Value, binaryData, attribute: true);
                }

                return attributes;
            }
        }

        private Schema.MeshPrimitive ConvertMeshPrimitive(MeshPrimitive runtimeMeshPrimitive, BinaryData binaryData)
        {
            var meshPrimitive = CreateInstance<Schema.MeshPrimitive>();

            var runtimeAttributes = new Dictionary<string, DataConvertArgs>();
            void AddRuntimeAttribute(string key, DataConvertArgs args)
            {
                if (args.Data != null)
                {
                    runtimeAttributes.Add(key, args);
                }
            }

            AddRuntimeAttribute("POSITION", new DataConvertArgs("Positions Accessor", "Positions", runtimeMeshPrimitive.Positions, minMax: true));
            AddRuntimeAttribute("NORMAL", new DataConvertArgs("Normals Accessor", "Normals", runtimeMeshPrimitive.Normals));
            AddRuntimeAttribute("TANGENT", new DataConvertArgs("Tangents Accessor", "Tangents", runtimeMeshPrimitive.Tangents));
            AddRuntimeAttribute("COLOR_0", new DataConvertArgs("Colors Accessor", "Colors", runtimeMeshPrimitive.Colors));
            AddRuntimeAttribute("TEXCOORD_0", new DataConvertArgs("UV Accessor 0", "Texture Coords 0", runtimeMeshPrimitive.TexCoords0));
            AddRuntimeAttribute("TEXCOORD_1", new DataConvertArgs("UV Accessor 1", "Texture Coords 1", runtimeMeshPrimitive.TexCoords1));

            meshPrimitive.Attributes = ConvertMeshPrimitiveAttributes(runtimeAttributes, runtimeMeshPrimitive.Interleave ?? false, binaryData);

            if (runtimeMeshPrimitive.Indices != null)
            {
                var args = new DataConvertArgs("Indices Accessor", "Indices", runtimeMeshPrimitive.Indices);
                meshPrimitive.Indices = ConvertData(args, binaryData);
            }

            // TODO: moved here to match existing data layout
            {
                if (runtimeMeshPrimitive.Weights != null)
                {
                    var args = new DataConvertArgs("weights accessor", "weights buffer view", runtimeMeshPrimitive.Weights);
                    meshPrimitive.Attributes["WEIGHTS_0"] = ConvertData(args, binaryData, attribute: true);
                }

                if (runtimeMeshPrimitive.Joints != null)
                {
                    var args = new DataConvertArgs("joint indices accessor", "joint indices buffer view", runtimeMeshPrimitive.Joints);
                    meshPrimitive.Attributes["JOINTS_0"] = ConvertData(args, binaryData, attribute: true);
                }
            }

            if (runtimeMeshPrimitive.Material != null)
            {
                meshPrimitive.Material = ConvertMaterial(runtimeMeshPrimitive.Material);
            }

            meshPrimitive.Mode = (ModeEnum)runtimeMeshPrimitive.Mode;

            return meshPrimitive;
        }

        private int ConvertMaterial(Material runtimeMaterial)
        {
            return Convert(runtimeMaterial, materials, materialToIndexCache, material =>
            {
                if (runtimeMaterial.Name != null)
                {
                    material.Name = runtimeMaterial.Name;
                }

                if (runtimeMaterial.PbrMetallicRoughness != null)
                {
                    material.PbrMetallicRoughness = CreateInstance<Schema.MaterialPbrMetallicRoughness>();

                    if (runtimeMaterial.PbrMetallicRoughness.BaseColorFactor.HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorFactor = runtimeMaterial.PbrMetallicRoughness.BaseColorFactor.Value.ToArray();
                    }

                    if (runtimeMaterial.PbrMetallicRoughness.BaseColorTexture != null)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture = ConvertTextureInfo(runtimeMaterial.PbrMetallicRoughness.BaseColorTexture);
                    }

                    if (runtimeMaterial.PbrMetallicRoughness.MetallicRoughnessTexture != null)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture = ConvertTextureInfo(runtimeMaterial.PbrMetallicRoughness.MetallicRoughnessTexture);
                    }

                    if (runtimeMaterial.PbrMetallicRoughness.MetallicFactor.HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicFactor = runtimeMaterial.PbrMetallicRoughness.MetallicFactor.Value;
                    }

                    if (runtimeMaterial.PbrMetallicRoughness.RoughnessFactor.HasValue)
                    {
                        material.PbrMetallicRoughness.RoughnessFactor = runtimeMaterial.PbrMetallicRoughness.RoughnessFactor.Value;
                    }
                }

                if (runtimeMaterial.NormalTexture != null)
                {
                    material.NormalTexture = ConvertNormalTextureInfo(runtimeMaterial.NormalTexture);
                }

                if (runtimeMaterial.OcclusionTexture != null)
                {
                    material.OcclusionTexture = ConvertOcclusionTextureInfo(runtimeMaterial.OcclusionTexture);
                }

                if (runtimeMaterial.EmissiveTexture != null)
                {
                    material.EmissiveTexture = ConvertTextureInfo(runtimeMaterial.EmissiveTexture);
                }

                if (runtimeMaterial.EmissiveFactor.HasValue)
                {
                    material.EmissiveFactor = runtimeMaterial.EmissiveFactor.Value.ToArray();
                }

                if (runtimeMaterial.DoubleSided.HasValue)
                {
                    material.DoubleSided = runtimeMaterial.DoubleSided.Value;
                }

                if (runtimeMaterial.AlphaMode.HasValue)
                {
                    material.AlphaMode = (AlphaModeEnum)runtimeMaterial.AlphaMode.Value;
                }

                if (runtimeMaterial.AlphaCutoff.HasValue)
                {
                    material.AlphaCutoff = runtimeMaterial.AlphaCutoff.Value;
                }

                if (runtimeMaterial.Extensions != null)
                {
                    material.Extensions = new Dictionary<string, object>();

                    foreach (var runtimeExtension in runtimeMaterial.Extensions)
                    {
                        object extension;
                        switch (runtimeExtension.Name)
                        {
                            case nameof(KHR_materials_pbrSpecularGlossiness):
                                extension = ConvertMaterialPbrSpecularGlossiness((KHR_materials_pbrSpecularGlossiness)runtimeExtension);
                                break;
                            case nameof(FAKE_materials_quantumRendering):
                                extension = ConvertMaterialExtQuantumRendering((FAKE_materials_quantumRendering)runtimeExtension);
                                break;
                            default:
                                throw new NotImplementedException($"Extension schema conversion not implemented for {runtimeExtension.Name}");
                        }

                        material.Extensions.Add(runtimeExtension.Name, extension);
                    }
                }
            });
        }

        private Schema.MaterialPbrSpecularGlossiness ConvertMaterialPbrSpecularGlossiness(KHR_materials_pbrSpecularGlossiness runtimeMaterial)
        {
            var material = CreateInstance<Schema.MaterialPbrSpecularGlossiness>();

            if (runtimeMaterial.DiffuseFactor.HasValue)
            {
                material.DiffuseFactor = runtimeMaterial.DiffuseFactor.Value.ToArray();
            }

            if (runtimeMaterial.DiffuseTexture != null)
            {
                material.DiffuseTexture = ConvertTextureInfo(runtimeMaterial.DiffuseTexture);
            }

            if (runtimeMaterial.SpecularFactor.HasValue)
            {
                material.SpecularFactor = runtimeMaterial.SpecularFactor.Value.ToArray();
            }

            if (runtimeMaterial.GlossinessFactor.HasValue)
            {
                material.GlossinessFactor = runtimeMaterial.GlossinessFactor.Value;
            }

            if (runtimeMaterial.SpecularGlossinessTexture != null)
            {
                material.SpecularGlossinessTexture = ConvertTextureInfo(runtimeMaterial.SpecularGlossinessTexture);
            }

            return material;
        }

        private Schema.FAKE_materials_quantumRendering ConvertMaterialExtQuantumRendering(FAKE_materials_quantumRendering runtimeMaterial)
        {
            var material = CreateInstance<Schema.FAKE_materials_quantumRendering>();

            if (runtimeMaterial.PlanckFactor.HasValue)
            {
                material.PlanckFactor = runtimeMaterial.PlanckFactor.Value.ToArray();
            }

            if (runtimeMaterial.CopenhagenTexture != null)
            {
                material.CopenhagenTexture = ConvertTextureInfo(runtimeMaterial.CopenhagenTexture);
            }

            if (runtimeMaterial.EntanglementFactor.HasValue)
            {
                material.EntanglementFactor = runtimeMaterial.EntanglementFactor.Value.ToArray();
            }

            if (runtimeMaterial.ProbabilisticFactor.HasValue)
            {
                material.ProbabilisticFactor = runtimeMaterial.ProbabilisticFactor.Value;
            }

            if (runtimeMaterial.SuperpositionCollapseTexture != null)
            {
                material.SuperpositionCollapseTexture = ConvertTextureInfo(runtimeMaterial.SuperpositionCollapseTexture);
            }

            if (runtimeMaterial.ProbabilisticFactor.HasValue)
            {
                material.ProbabilisticFactor = runtimeMaterial.ProbabilisticFactor.Value;
            }

            return material;
        }

        private Schema.TextureInfo ConvertTextureInfo(TextureInfo runtimeTextureInfo)
        {
            var textureInfo = CreateInstance<Schema.TextureInfo>();

            textureInfo.Index = ConvertTexture(runtimeTextureInfo.Texture);

            if (runtimeTextureInfo.TexCoord.HasValue)
            {
                textureInfo.TexCoord = runtimeTextureInfo.TexCoord.Value;
            }

            return textureInfo;
        }

        private Schema.MaterialNormalTextureInfo ConvertNormalTextureInfo(NormalTextureInfo runtimeTextureInfo)
        {
            var textureInfo = CreateInstance<Schema.MaterialNormalTextureInfo>();

            textureInfo.Index = ConvertTexture(runtimeTextureInfo.Texture);

            if (runtimeTextureInfo.TexCoord.HasValue)
            {
                textureInfo.TexCoord = runtimeTextureInfo.TexCoord.Value;
            }

            if (runtimeTextureInfo.Scale.HasValue)
            {
                textureInfo.Scale = runtimeTextureInfo.Scale.Value;
            }

            return textureInfo;
        }

        private Schema.MaterialOcclusionTextureInfo ConvertOcclusionTextureInfo(OcclusionTextureInfo runtimeTextureInfo)
        {
            var textureInfo = CreateInstance<Schema.MaterialOcclusionTextureInfo>();

            textureInfo.Index = ConvertTexture(runtimeTextureInfo.Texture);

            if (runtimeTextureInfo.TexCoord.HasValue)
            {
                textureInfo.TexCoord = runtimeTextureInfo.TexCoord.Value;
            }

            if (runtimeTextureInfo.Strength.HasValue)
            {
                textureInfo.Strength = runtimeTextureInfo.Strength.Value;
            }

            return textureInfo;
        }

        private int ConvertTexture(Texture runtimeTexture)
        {
            return Convert(runtimeTexture, textures, textureToIndexCache, texture =>
            {
                texture.Name = runtimeTexture.Name;

                if (runtimeTexture.Sampler != null)
                {
                    texture.Sampler = ConvertSampler(runtimeTexture.Sampler);
                }

                if (runtimeTexture.Source != null)
                {
                    texture.Source = ConvertImage(runtimeTexture.Source);
                }
            });
        }

        private int ConvertSampler(Sampler runtimeSampler)
        {
            return Convert(runtimeSampler, samplers, samplerToIndexCache, sampler =>
            {
                sampler.Name = runtimeSampler.Name;

                if (runtimeSampler.MagFilter.HasValue)
                {
                    sampler.MagFilter = (MagFilterEnum)runtimeSampler.MagFilter.Value;
                }

                if (runtimeSampler.MinFilter.HasValue)
                {
                    sampler.MinFilter = (MinFilterEnum)runtimeSampler.MinFilter.Value;
                }

                if (runtimeSampler.WrapS.HasValue)
                {
                    sampler.WrapS = (WrapSEnum)runtimeSampler.WrapS.Value;
                }

                if (runtimeSampler.WrapT.HasValue)
                {
                    sampler.WrapT = (WrapTEnum)runtimeSampler.WrapT.Value;
                }
            });
        }

        private int ConvertImage(Image runtimeImage)
        {
            return Convert(runtimeImage, images, imageToIndexCache, image =>
            {
                image.Name = runtimeImage.Name;
                image.Uri = runtimeImage.Uri;
            });
        }

        private int ConvertSkin(Skin runtimeSkin, BinaryData binaryData)
        {
            return Convert(runtimeSkin, skins, skinToIndexCache, skin =>
            {
                skin.Name = runtimeSkin.Name;

                if (runtimeSkin.InverseBindMatrices != null)
                {
                    var args = new DataConvertArgs("IBM", "Inverse Bind Matrix", runtimeSkin.InverseBindMatrices);
                    skin.InverseBindMatrices = ConvertData(args, binaryData);
                }

                skin.Joints = runtimeSkin.Joints.Select(jointNode => ConvertNode(jointNode)).ToArray();
            });
        }

        private Schema.Animation ConvertAnimation(Animation runtimeAnimation, BinaryData binaryData)
        {
            var animation = CreateInstance<Schema.Animation>();

            var animationChannels = new List<Schema.AnimationChannel>();
            var animationSamplers = new List<Schema.AnimationSampler>();
            var animationSamplerToIndexCache = new Dictionary<AnimationSampler, int>();

            int ConvertAnimationSampler(AnimationSampler runtimeAnimationSampler)
            {
                return Convert(runtimeAnimationSampler, animationSamplers, animationSamplerToIndexCache, animationSampler =>
                {
                    var inputDataInfo = new DataConvertArgs("Animation Sampler Input", "Animation Sampler Input", runtimeAnimationSampler.Input, minMax: true);
                    var outputDataInfo = new DataConvertArgs("Animation Sampler Output", "Animation Sampler Output", runtimeAnimationSampler.Output);

                    animationSampler.Interpolation = (InterpolationEnum)runtimeAnimationSampler.Interpolation;
                    animationSampler.Input = ConvertData(inputDataInfo, binaryData);
                    animationSampler.Output = ConvertData(outputDataInfo, binaryData);
                });
            }

            foreach (var runtimeAnimationChannel in runtimeAnimation.Channels)
            {
                var animationChannel = CreateInstance<Schema.AnimationChannel>();

                animationChannel.Sampler = ConvertAnimationSampler(runtimeAnimationChannel.Sampler);

                animationChannel.Target = CreateInstance<Schema.AnimationChannelTarget>();

                if (runtimeAnimationChannel.Target.Node != null)
                {
                    animationChannel.Target.Node = ConvertNode(runtimeAnimationChannel.Target.Node);
                }

                animationChannel.Target.Path = (PathEnum)runtimeAnimationChannel.Target.Path;

                animationChannels.Add(animationChannel);
            }

            animation.Name = runtimeAnimation.Name;
            animation.Channels = animationChannels.ToArray();
            animation.Samplers = animationSamplers.ToArray();

            return animation;
        }

        private int CreateBufferView(string name, out Schema.BufferView bufferView)
        {
            bufferView = CreateInstance<Schema.BufferView>();
            bufferView.Name = name;
            var bufferViewIndex = bufferViews.Count();
            bufferViews.Add(bufferView);
            return bufferViewIndex;
        }

        private void WriteBufferView(Schema.BufferView bufferView, BinaryData binaryData, Action<int> writeData)
        {
            binaryData.Writer.Align(4);

            int byteOffset = (int)binaryData.Writer.BaseStream.Position;

            writeData(byteOffset);

            int byteLength = (int)binaryData.Writer.BaseStream.Position - byteOffset;

            bufferView.Buffer = binaryDataToBufferIndex[binaryData];
            bufferView.ByteOffset = byteOffset;
            bufferView.ByteLength = byteLength;
        }

        private class ConvertDataState
        {
            public Schema.Accessor Accessor;
            public IEnumerator<DataConverter.Element> ValuesEnumerator;
            public IEnumerable<DataConverter.Element> SparseIndices;
            public IEnumerable<DataConverter.Element> SparseValues;
        }

        private Dictionary<string, int> ConvertData(Dictionary<string, DataConvertArgs> argsMap, BinaryData binaryData, bool attribute)
        {
            Schema.BufferView bufferView = null;
            int? bufferViewIndex = null;

            var states = new List<ConvertDataState>();
            var accessorsMap = new Dictionary<string, int>();

            foreach (var pair in argsMap)
            {
                var args = pair.Value;

                var info = DataConverter.GetInfo(args.Data, binaryData.Writer);

                if (info.Values != null && bufferViewIndex == null)
                {
                    var bufferViewName = argsMap.Count == 1 ? argsMap.First().Value.BufferViewName : "Interleaved attributes";
                    bufferViewIndex = CreateBufferView(bufferViewName, out bufferView);
                }

                var accessor = CreateInstance<Schema.Accessor>();
                accessor.Name = args.AccessorName;
                accessor.BufferView = bufferViewIndex;
                info.SetCommon(accessor);
                if (args.MinMax)
                {
                    info.SetMinMax(accessor);
                }

                states.Add(new ConvertDataState
                {
                    Accessor = accessor,
                    ValuesEnumerator = info.Values?.GetEnumerator(),
                    SparseIndices = info.SparseIndices,
                    SparseValues = info.SparseValues,
                });

                accessorsMap.Add(pair.Key, accessors.Count);
                accessors.Add(accessor);
            }

            var counts = states.Select(state => state.Accessor.Count);
            if (counts.Skip(1).Any(count => count != counts.First()))
            {
                throw new InvalidOperationException("Data values cannot have different counts");
            }

            if (bufferView != null)
            {
                bool aligned = false;
                int byteStride = 0;

                WriteBufferView(bufferView, binaryData, byteOffset =>
                {
                    while (states.All(state => state.ValuesEnumerator.MoveNext()))
                    {
                        foreach (var state in states)
                        {
                            if (byteStride == 0)
                            {
                                aligned |= binaryData.Writer.Align(state.Accessor.ComponentType.GetSize());
                                state.Accessor.ByteOffset = (int)binaryData.Writer.BaseStream.Position - byteOffset;
                            }

                            state.ValuesEnumerator.Current.Write();
                        }

                        if (attribute)
                        {
                            aligned |= binaryData.Writer.Align(4);
                        }

                        if (byteStride == 0)
                        {
                            byteStride = (int)binaryData.Writer.BaseStream.Position - byteOffset;
                        }
                    }
                });

                if (aligned || argsMap.Count > 1)
                {
                    bufferView.ByteStride = byteStride;
                }
            }

            foreach (var state in states)
            {
                if (state.SparseIndices != null && state.SparseValues != null)
                {
                    state.Accessor.Sparse.Indices.BufferView = CreateBufferView("Sparse Indices", out var indicesBufferView);
                    WriteBufferView(indicesBufferView, binaryData, byteOffset =>
                    {
                        foreach (var index in state.SparseIndices)
                        {
                            index.Write();
                        }
                    });

                    state.Accessor.Sparse.Values.BufferView = CreateBufferView("Sparse Values", out var valuesBufferView);
                    WriteBufferView(valuesBufferView, binaryData, byteOffset =>
                    {
                        foreach (var index in state.SparseValues)
                        {
                            index.Write();
                        }
                    });
                }
            }

            return accessorsMap;
        }

        private int ConvertData(DataConvertArgs args, BinaryData binaryData, bool attribute = false)
        {
            if (accessorToIndexCache.TryGetValue(args.Data, out int accessorIndex))
            {
                return accessorIndex;
            }

            var argsMap = new Dictionary<string, DataConvertArgs>
            {
                { string.Empty, args }
            };

            accessorIndex = ConvertData(argsMap, binaryData, attribute)[string.Empty];
            accessorToIndexCache.Add(args.Data, accessorIndex);
            return accessorIndex;
        }
    }
}
