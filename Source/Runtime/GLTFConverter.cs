﻿using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using static glTFLoader.Schema.Accessor;
using static glTFLoader.Schema.MeshPrimitive;
using Loader = glTFLoader.Schema;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Convert Runtime Abstraction to Schema.
    /// </summary>
    internal class GLTFConverter
    {
        private List<Loader.Buffer> buffers = new List<Loader.Buffer>();
        private List<Loader.BufferView> bufferViews = new List<Loader.BufferView>();
        private List<Loader.Accessor> accessors = new List<Loader.Accessor>();
        private List<Loader.Material> materials = new List<Loader.Material>();
        private List<Loader.Node> nodes = new List<Loader.Node>();
        private List<Loader.Scene> scenes = new List<Loader.Scene>();
        private List<Loader.Image> images = new List<Loader.Image>();
        private List<Loader.Sampler> samplers = new List<Loader.Sampler>();
        private List<Loader.Texture> textures = new List<Loader.Texture>();
        private List<Loader.Mesh> meshes = new List<Loader.Mesh>();
        private List<Loader.Animation> animations = new List<Loader.Animation>();
        private List<Loader.Skin> skins = new List<Loader.Skin>();

        private Dictionary<Texture, TextureInfo> textureToTextureIndicesCache = new Dictionary<Texture, TextureInfo>();
        private Dictionary<Mesh, Loader.Mesh> meshToSchemaCache = new Dictionary<Mesh, Loader.Mesh>();
        private Dictionary<Image, Loader.Image> imageToSchemaCache = new Dictionary<Image, Loader.Image>();
        private Dictionary<Sampler, Loader.Sampler> samplerToSchemaCache = new Dictionary<Sampler, Loader.Sampler>();
        private Dictionary<MeshPrimitive, Loader.MeshPrimitive> meshPrimitiveToSchemaCache = new Dictionary<MeshPrimitive, Loader.MeshPrimitive>();
        private Dictionary<Material, Loader.Material> materialToSchemaCache = new Dictionary<Material, Loader.Material>();
        private Dictionary<Node, int> nodeToIndexCache = new Dictionary<Node, int>();
        private Dictionary<AnimationSampler, int> animationSamplerToIndexCache = new Dictionary<AnimationSampler, int>();
        private Dictionary<Skin, int> skinToIndexCache = new Dictionary<Skin, int>();
        private Dictionary<IEnumerable, int> enumerableToIndexCache = new Dictionary<IEnumerable, int>();
        private enum AttributeEnum { POSITION, NORMAL, TANGENT, COLOR, TEXCOORDS_0, TEXCOORDS_1, JOINTS_0, WEIGHTS_0 };

        /// <summary>
        /// Set this property to allow creating custom types.
        /// </summary>
        public Func<Type, object> CreateInstanceOverride = type => Activator.CreateInstance(type);

        /// <summary>
        /// Utility struct for holding texture information.
        /// </summary>
        private struct TextureInfo
        {
            public int Index;
            public int? SamplerIndex;
            public int? ImageIndex;
            public int? TextureCoordIndex;
        }

        /// <summary>
        /// Converts Runtime GLTF to Schema GLTF object.
        /// </summary>
        public Loader.Gltf ConvertRuntimeToSchema(GLTF runtimeGLTF, Data geometryData)
        {
            var gltf = CreateInstance<Loader.Gltf>();

            if (runtimeGLTF.Asset != null)
            {
                gltf.Asset = ConvertAssetToSchema(runtimeGLTF.Asset);
            }

            var buffer = CreateInstance<Loader.Buffer>();
            buffer.Uri = geometryData.Name;

            // For each scene, create a node for each mesh and compute the indices for the scene object.
            foreach (var runtimeScene in runtimeGLTF.Scenes)
            {
                var sceneIndicesSet = new List<int>();
                // Loops through each mesh and converts it into a Node, with optional transformation info if available.
                foreach (var node in runtimeScene.Nodes)
                {
                    sceneIndicesSet.Add(ConvertNodeToSchema(node, runtimeGLTF, buffer, geometryData, bufferIndex: 0));
                }

                var scene = CreateInstance<Loader.Scene>();
                scene.Nodes = sceneIndicesSet.ToArray();
                scenes.Add(scene);
            }
            if (scenes != null && scenes.Any())
            {
                gltf.Scenes = scenes.ToArray();
                gltf.Scene = 0;
            }
            if (runtimeGLTF.Animations != null && runtimeGLTF.Animations.Any())
            {
                var animations = new List<Loader.Animation>();
                foreach (var runtimeAnimation in runtimeGLTF.Animations)
                {
                    var animation = ConvertAnimationToSchema(runtimeAnimation, buffer, runtimeGLTF, geometryData, bufferIndex: 0);
                    if (!animations.Contains(animation))
                    {
                        animations.Add(animation);
                    }
                }
                gltf.Animations = animations.ToArray();
            }

            if (meshes != null && meshes.Any())
            {
                gltf.Meshes = meshes.ToArray();
            }
            if (materials != null && materials.Any())
            {
                gltf.Materials = materials.ToArray();
            }
            if (accessors != null && accessors.Any())
            {
                gltf.Accessors = accessors.ToArray();
            }
            if (bufferViews != null && bufferViews.Any())
            {
                gltf.BufferViews = bufferViews.ToArray();
            }

            gltf.Buffers = new[] { buffer };
            if (nodes != null && nodes.Any())
            {
                gltf.Nodes = nodes.ToArray();
            }

            if (images.Any())
            {
                gltf.Images = images.ToArray();
            }
            if (textures.Any())
            {
                gltf.Textures = textures.ToArray();
            }
            if (skins.Any())
            {
                gltf.Skins = skins.ToArray();
            }
            if (samplers.Any())
            {
                gltf.Samplers = samplers.ToArray();
            }
            if (animations.Any())
            {
                gltf.Animations = animations.ToArray();
            }
            if (runtimeGLTF.Scene.HasValue)
            {
                gltf.Scene = runtimeGLTF.Scene.Value;
            }
            if (runtimeGLTF.ExtensionsUsed != null && runtimeGLTF.ExtensionsUsed.Any())
            {
                gltf.ExtensionsUsed = runtimeGLTF.ExtensionsUsed.ToArray();
            }
            if (runtimeGLTF.ExtensionsRequired != null && runtimeGLTF.ExtensionsRequired.Any())
            {
                gltf.ExtensionsRequired = runtimeGLTF.ExtensionsRequired.ToArray();
            }
            buffer.ByteLength = (int)geometryData.Writer.BaseStream.Position;

            return gltf;
        }

        private T CreateInstance<T>()
        {
            return (T)CreateInstanceOverride(typeof(T));
        }

        private int AddToSchemaSamplers(Loader.Sampler sampler)
        {
            if (samplers.Contains(sampler))
            {
                return samplers.IndexOf(sampler);
            }
            else
            {
                samplers.Add(sampler);
                return samplers.Count() - 1;
            }
        }
        private int AddToSchemaImages(Loader.Image image)
        {
            if (images.Contains(image))
            {
                return images.IndexOf(image);
            }
            else
            {
                images.Add(image);
                return images.Count() - 1;
            }
        }

        /// <summary>
        /// Converts the Runtime image to a glTF Image.
        /// </summary>
        /// <returns>Returns a gltf Image object.</returns>
        private Loader.Image ConvertImageToSchema(Image runtimeImage)
        {
            if (imageToSchemaCache.TryGetValue(runtimeImage, out Loader.Image schemaImage))
            {
                return schemaImage;
            }
            schemaImage = CreateInstance<Loader.Image>();

            schemaImage.Uri = runtimeImage.Uri;

            if (runtimeImage.MimeType.HasValue)
            {
                schemaImage.MimeType = runtimeImage.MimeType.Value;
            }

            if (runtimeImage.Name != null)
            {
                schemaImage.Name = runtimeImage.Name;
            }
            imageToSchemaCache.Add(runtimeImage, schemaImage);

            return schemaImage;
        }

        private Loader.Sampler ConvertSamplerToSchema(Sampler runtimeSampler)
        {
            if (samplerToSchemaCache.TryGetValue(runtimeSampler, out Loader.Sampler schemaSampler))
            {
                return schemaSampler;
            }
            schemaSampler = CreateInstance<Loader.Sampler>();

            if (runtimeSampler.MagFilter.HasValue)
            {
                schemaSampler.MagFilter = runtimeSampler.MagFilter.Value;
            }

            if (runtimeSampler.MinFilter.HasValue)
            {
                schemaSampler.MinFilter = runtimeSampler.MinFilter.Value;
            }

            if (runtimeSampler.WrapS.HasValue)
            {
                schemaSampler.WrapS = runtimeSampler.WrapS.Value;
            }

            if (runtimeSampler.WrapT.HasValue)
            {
                schemaSampler.WrapT = runtimeSampler.WrapT.Value;
            }

            if (runtimeSampler.Name != null)
            {
                schemaSampler.Name = runtimeSampler.Name;
            }
            samplerToSchemaCache.Add(runtimeSampler, schemaSampler);

            return schemaSampler;
        }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <returns>Returns the indices of the texture and the texture coordinate as an array of two integers if created. Can also return null if the index is not defined.</returns>
        private TextureInfo AddTexture(Texture runtimeTexture)
        {
            if (textureToTextureIndicesCache.TryGetValue(runtimeTexture, out TextureInfo textureIndices))
            {
                return textureIndices;
            }

            var indices = new List<int>();
            int? samplerIndex = null;
            int? imageIndex = null;
            int? textureCoordIndex = null;
            int index = -1;

            if (runtimeTexture != null)
            {
                if (runtimeTexture.Sampler != null)
                {
                    samplerIndex = AddToSchemaSamplers(ConvertSamplerToSchema(runtimeTexture.Sampler));
                }
                if (runtimeTexture.Source != null)
                {
                    imageIndex = AddToSchemaImages(ConvertImageToSchema(runtimeTexture.Source));
                }

                var texture = CreateInstance<Loader.Texture>();
                if (samplerIndex.HasValue)
                {
                    texture.Sampler = samplerIndex.Value;
                }
                if (imageIndex.HasValue)
                {
                    texture.Source = imageIndex.Value;
                }
                if (runtimeTexture.Name != null)
                {
                    texture.Name = runtimeTexture.Name;
                }
                // If an equivalent texture has already been created, re-use that texture's index instead of creating a new texture.
                if (textures.Count > 0)
                {
                    for (var i = 0; i < textures.Count; ++i)
                    {
                        if (textures[i].TexturesEqual(texture))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                if (index == -1)
                {
                    index = textures.Count;
                    textures.Add(texture);
                }
                indices.Add(index);

                if (runtimeTexture.TexCoordIndex.HasValue)
                {
                    indices.Add(runtimeTexture.TexCoordIndex.Value);
                    textureCoordIndex = runtimeTexture.TexCoordIndex.Value;
                }
            }

            textureIndices = new TextureInfo
            {
                SamplerIndex = samplerIndex,
                ImageIndex = imageIndex,
                TextureCoordIndex = textureCoordIndex,
                Index = index
            };

            textureToTextureIndicesCache.Add(runtimeTexture, textureIndices);

            return textureIndices;
        }

        private static int GetPaddedSize(int value, int size)
        {
            var remainder = value % size;
            return (remainder == 0 ? value : checked(value + size - remainder));
        }

        /// <summary>
        /// Pads a value to ensure it is a multiple of 4.
        /// </summary>
        private static void Align(BinaryWriter writer)
        {
            var rem = (4 - (writer.BaseStream.Position & 3)) & 3;
            writer.Write(new byte[rem]);
        }

        /// <summary>
        /// Creates a bufferview schema object.
        /// </summary>
        private Loader.BufferView CreateBufferView(int bufferIndex, string name, int byteLength, int byteOffset, int? byteStride)
        {
            var bufferView = CreateInstance<Loader.BufferView>();

            bufferView.Name = name;
            bufferView.ByteLength = byteLength;
            bufferView.ByteOffset = byteOffset;
            bufferView.Buffer = bufferIndex;

            if (byteStride.HasValue)
            {
                bufferView.ByteStride = byteStride;
            }

            return bufferView;
        }

        /// <summary>
        /// Creates an accessor schema object.
        /// </summary>
        private Loader.Accessor CreateAccessor(int bufferviewIndex, int? byteOffset, ComponentTypeEnum? componentType, int? count, string name, TypeEnum? type, bool? normalized = null, float[] max = null, float[] min = null)
        {
            var accessor = CreateInstance<Loader.Accessor>();

            accessor.BufferView = bufferviewIndex;
            accessor.Name = name;

            if (min != null && min.Any())
            {
                accessor.Min = min;
            }

            if (max != null && max.Any())
            {
                accessor.Max = max;
            }

            if (componentType.HasValue)
            {
                accessor.ComponentType = componentType.Value;
            }

            if (byteOffset.HasValue)
            {
                accessor.ByteOffset = byteOffset.Value;
            }

            if (count.HasValue)
            {
                accessor.Count = count.Value;
            }

            if (type.HasValue)
            {
                accessor.Type = type.Value;
            }

            if (normalized.HasValue && normalized.Value == true)
            {
                accessor.Normalized = normalized.Value;
            }

            return accessor;
        }

        /// <summary>
        /// Converts runtime asset to Schema.
        /// </summary>
        private Loader.Asset ConvertAssetToSchema(Asset runtimeAsset)
        {
            var extras = CreateInstance<Loader.Extras>();
            var schemaAsset = CreateInstance<Loader.Asset>();

            if (runtimeAsset.Generator != null)
            {
                schemaAsset.Generator = runtimeAsset.Generator;
            }

            if (runtimeAsset.Version != null)
            {
                schemaAsset.Version = runtimeAsset.Version;
            }

            if (runtimeAsset.Extras != null)
            {
                schemaAsset.Extras = runtimeAsset.Extras;
            }

            if (runtimeAsset.Copyright != null)
            {
                schemaAsset.Copyright = runtimeAsset.Copyright;
            }

            if (runtimeAsset.MinVersion != null)
            {
                schemaAsset.MinVersion = runtimeAsset.MinVersion;
            }

            return schemaAsset;
        }

        /// <summary>
        /// Converts runtime node to schema.
        /// </summary>
        private int ConvertNodeToSchema(Node runtimeNode, GLTF gltf, Loader.Buffer buffer, Data geometryData, int bufferIndex)
        {
            if (nodeToIndexCache.TryGetValue(runtimeNode, out int nodeIndex))
            {
                return nodeIndex;
            }

            var node = CreateInstance<Loader.Node>();
            nodeIndex = nodes.Count;
            nodes.Add(node);
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
                var schemaMesh = ConvertMeshToSchema(runtimeNode, gltf, buffer, geometryData, bufferIndex);

                int meshIndex = meshes.IndexOf(schemaMesh);
                if (meshIndex == -1)
                {
                    node.Mesh = meshes.Count();
                    meshes.Add(schemaMesh);
                }
                else
                {
                    node.Mesh = meshIndex;
                }
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
                var childrenIndices = new List<int>();
                foreach (var childNode in runtimeNode.Children)
                {
                    int schemaChildIndex = ConvertNodeToSchema(childNode, gltf, buffer, geometryData, bufferIndex);
                    childrenIndices.Add(schemaChildIndex);
                }
                node.Children = childrenIndices.ToArray();
            }
            if (runtimeNode.Skin?.Joints?.Any() == true)
            {
                if (skinToIndexCache.TryGetValue(runtimeNode.Skin, out int skinIndex))
                {
                    node.Skin = skinIndex;
                }
                else
                {
                    // Verify that the Joints and InverseBindMatrices lists are the same length. Different lenghts break our assumtion of a 1:1 correlation.
                    if (runtimeNode.Skin.Joints.Count() != runtimeNode.Skin.InverseBindMatrices.Count())
                    {
                        throw new InvalidEnumArgumentException("Mismatch between the number of joints and number of inverseBindMatrices!");
                    }

                    int? inverseBindMatricesAccessorIndex = null;
                    if (enumerableToIndexCache.TryGetValue(runtimeNode.Skin.InverseBindMatrices, out int index))
                    {
                        inverseBindMatricesAccessorIndex = index;
                    }
                    else
                    {
                        if (runtimeNode.Skin.InverseBindMatrices.Any(inverseBindMatrix => !inverseBindMatrix.IsIdentity))
                        {
                            var inverseBindMatricesByteOffset = (int)geometryData.Writer.BaseStream.Position;
                            geometryData.Writer.Write(runtimeNode.Skin.InverseBindMatrices);
                            var inverseBindMatricesByteLength = (int)geometryData.Writer.BaseStream.Position - inverseBindMatricesByteOffset;

                            // Create accessor
                            var inverseBindMatricesAccessor = CreateAccessor(bufferViews.Count(), 0, ComponentTypeEnum.FLOAT, runtimeNode.Skin.InverseBindMatrices.Count(), "IBM", TypeEnum.MAT4);
                            inverseBindMatricesAccessorIndex = accessors.Count();
                            accessors.Add(inverseBindMatricesAccessor);
                            enumerableToIndexCache.Add(runtimeNode.Skin.InverseBindMatrices, inverseBindMatricesAccessorIndex.Value);

                            // Create bufferview
                            var inverseBindMatricesBufferView = CreateBufferView(bufferIndex, "Inverse Bind Matrix", inverseBindMatricesByteLength, inverseBindMatricesByteOffset, null);
                            bufferViews.Add(inverseBindMatricesBufferView);
                        }
                    }

                    var jointIndices = runtimeNode.Skin.Joints.Select(Joint => ConvertNodeToSchema(Joint, gltf, buffer, geometryData, bufferIndex));

                    var skin = new Loader.Skin
                    {
                        Name = runtimeNode.Skin.Name,
                        Joints = jointIndices.ToArray(),
                        InverseBindMatrices = inverseBindMatricesAccessorIndex,
                    };
                    node.Skin = skins.Count();
                    skins.Add(skin);
                    skinToIndexCache.Add(runtimeNode.Skin, node.Skin.Value);
                }
            }
            nodeToIndexCache.Add(runtimeNode, nodeIndex);

            return nodeIndex;
        }

        /// <summary>
        /// Converts the morph target list of dictionaries into Morph Target
        /// </summary>
        private IEnumerable<Dictionary<string, int>> GetMeshPrimitiveMorphTargets(MeshPrimitive meshPrimitive, List<float> weights, Loader.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var morphTargetDicts = new List<Dictionary<string, int>>();
            if (meshPrimitive.MorphTargets != null)
            {
                foreach (MeshPrimitive morphTarget in meshPrimitive.MorphTargets)
                {
                    var morphTargetAttributes = new Dictionary<string, int>();

                    if (morphTarget.Positions != null && morphTarget.Positions.Any())
                    {
                        if (morphTarget.Positions != null)
                        {
                            //Create BufferView for the position
                            int byteLength = sizeof(float) * 3 * morphTarget.Positions.Count();
                            var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                            var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);

                            var bufferviewIndex = bufferViews.Count;
                            bufferViews.Add(bufferView);

                            // Create an accessor for the bufferView
                            var accessor = CreateAccessor(bufferviewIndex, 0, ComponentTypeEnum.FLOAT, morphTarget.Positions.Count(), "Positions Accessor", TypeEnum.VEC3);
                            geometryData.Writer.Write(morphTarget.Positions.ToArray());
                            morphTargetAttributes.Add("POSITION", accessors.Count());
                            accessors.Add(accessor);
                        }
                    }
                    if (morphTarget.Normals != null && morphTarget.Normals.Any())
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Normals.Count();
                        // Create a bufferView
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);

                        int bufferviewIndex = bufferViews.Count;
                        bufferViews.Add(bufferView);

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, ComponentTypeEnum.FLOAT, morphTarget.Normals.Count(), "Normals Accessor", TypeEnum.VEC3);

                        geometryData.Writer.Write(morphTarget.Normals.ToArray());
                        morphTargetAttributes.Add("NORMAL", accessors.Count());
                        accessors.Add(accessor);
                    }
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Any())
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Tangents.Count();
                        // Create a bufferView
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);

                        int bufferviewIndex = bufferViews.Count;
                        bufferViews.Add(bufferView);

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, ComponentTypeEnum.FLOAT, morphTarget.Tangents.Count(), "Tangents Accessor", TypeEnum.VEC3);

                        geometryData.Writer.Write(morphTarget.Tangents.ToArray());
                        morphTargetAttributes.Add("TANGENT", accessors.Count);
                        accessors.Add(accessor);
                    }
                    morphTargetDicts.Add(new Dictionary<string, int>(morphTargetAttributes));
                    weights.Add(meshPrimitive.MorphTargetWeight);
                }
            }
            return morphTargetDicts;
        }

        /// <summary>
        /// Converts runtime mesh to schema.
        /// </summary>
        private Loader.Mesh ConvertMeshToSchema(Node runtimeNode, GLTF gltf, Loader.Buffer buffer, Data geometryData, int bufferIndex)
        {
            if (meshToSchemaCache.TryGetValue(runtimeNode.Mesh, out Loader.Mesh schemaMesh))
            {
                return schemaMesh;
            }
            Mesh runtimeMesh = runtimeNode.Mesh;
            schemaMesh = CreateInstance<Loader.Mesh>();
            var primitives = new List<Loader.MeshPrimitive>(runtimeMesh.MeshPrimitives.Count());
            var weights = new List<float>();
            // Loops through each wrapped mesh primitive within the mesh and converts them to mesh primitives, as well as updating the
            // indices in the lists
            foreach (var gPrimitive in runtimeMesh.MeshPrimitives)
            {
                Loader.MeshPrimitive mPrimitive = ConvertMeshPrimitiveToSchema(runtimeNode, gPrimitive, gltf, buffer, geometryData, bufferIndex);
                if (gPrimitive.MorphTargets != null && gPrimitive.MorphTargets.Any())
                {
                    var morphTargetAttributes = GetMeshPrimitiveMorphTargets(gPrimitive, weights, buffer, geometryData, bufferIndex);
                    mPrimitive.Targets = morphTargetAttributes.ToArray();
                }
                primitives.Add(mPrimitive);
            }
            if (runtimeMesh.Name != null)
            {
                schemaMesh.Name = runtimeMesh.Name;
            }
            if (runtimeMesh.MeshPrimitives != null && primitives.Count > 0)
            {
                schemaMesh.Primitives = primitives.ToArray();
            }
            if (weights.Count > 0)
            {
                schemaMesh.Weights = weights.ToArray();
            }
            meshToSchemaCache.Add(runtimeNode.Mesh, schemaMesh);

            return schemaMesh;
        }

        /// <summary>
        /// Converts runtime material to schema.
        /// </summary>
        private Loader.Material ConvertMaterialToSchema(Material runtimeMaterial, GLTF gltf)
        {
            if (materialToSchemaCache.TryGetValue(runtimeMaterial, out Loader.Material schemaMaterial))
            {
                return schemaMaterial;
            }
            schemaMaterial = CreateInstance<Loader.Material>();

            if (runtimeMaterial.MetallicRoughnessMaterial != null)
            {
                schemaMaterial.PbrMetallicRoughness = CreateInstance<Loader.MaterialPbrMetallicRoughness>();
                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.HasValue)
                {
                    schemaMaterial.PbrMetallicRoughness.BaseColorFactor = new[]
                    {
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.X,
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.Y,
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.Z,
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.W
                    };
                }

                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    TextureInfo baseColorIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture);

                    schemaMaterial.PbrMetallicRoughness.BaseColorTexture = CreateInstance<Loader.TextureInfo>();
                    schemaMaterial.PbrMetallicRoughness.BaseColorTexture.Index = baseColorIndices.Index;
                    if (baseColorIndices.TextureCoordIndex.HasValue)
                    {
                        schemaMaterial.PbrMetallicRoughness.BaseColorTexture.TexCoord = baseColorIndices.TextureCoordIndex.Value;
                    }
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture != null)
                {
                    TextureInfo metallicRoughnessIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture);

                    schemaMaterial.PbrMetallicRoughness.MetallicRoughnessTexture = CreateInstance<Loader.TextureInfo>();
                    schemaMaterial.PbrMetallicRoughness.MetallicRoughnessTexture.Index = metallicRoughnessIndices.Index;
                    if (metallicRoughnessIndices.TextureCoordIndex.HasValue)
                    {
                        schemaMaterial.PbrMetallicRoughness.MetallicRoughnessTexture.TexCoord = metallicRoughnessIndices.TextureCoordIndex.Value;
                    }
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicFactor.HasValue)
                {
                    schemaMaterial.PbrMetallicRoughness.MetallicFactor = runtimeMaterial.MetallicRoughnessMaterial.MetallicFactor.Value;
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.RoughnessFactor.HasValue)
                {
                    schemaMaterial.PbrMetallicRoughness.RoughnessFactor = runtimeMaterial.MetallicRoughnessMaterial.RoughnessFactor.Value;
                }
            }
            if (runtimeMaterial.EmissiveFactor != null)
            {
                schemaMaterial.EmissiveFactor = new[]
                {
                    runtimeMaterial.EmissiveFactor.Value.X,
                    runtimeMaterial.EmissiveFactor.Value.Y,
                    runtimeMaterial.EmissiveFactor.Value.Z
                };
            }
            if (runtimeMaterial.NormalTexture != null)
            {
                TextureInfo normalIndices = AddTexture(runtimeMaterial.NormalTexture);
                schemaMaterial.NormalTexture = CreateInstance<Loader.MaterialNormalTextureInfo>();
                schemaMaterial.NormalTexture.Index = normalIndices.Index;
                if (normalIndices.TextureCoordIndex.HasValue)
                {
                    schemaMaterial.NormalTexture.TexCoord = normalIndices.TextureCoordIndex.Value;
                }
                if (runtimeMaterial.NormalScale.HasValue)
                {
                    schemaMaterial.NormalTexture.Scale = runtimeMaterial.NormalScale.Value;
                }
            }
            if (runtimeMaterial.OcclusionTexture != null)
            {
                TextureInfo occlusionIndices = AddTexture(runtimeMaterial.OcclusionTexture);
                schemaMaterial.OcclusionTexture = CreateInstance<Loader.MaterialOcclusionTextureInfo>();
                schemaMaterial.OcclusionTexture.Index = occlusionIndices.Index;
                if (occlusionIndices.TextureCoordIndex.HasValue)
                {
                    schemaMaterial.OcclusionTexture.TexCoord = occlusionIndices.TextureCoordIndex.Value;
                }
                if (runtimeMaterial.OcclusionStrength.HasValue)
                {
                    schemaMaterial.OcclusionTexture.Strength = runtimeMaterial.OcclusionStrength.Value;
                }
            }
            if (runtimeMaterial.EmissiveTexture != null)
            {
                TextureInfo emissiveIndices = AddTexture(runtimeMaterial.EmissiveTexture);
                schemaMaterial.EmissiveTexture = CreateInstance<Loader.TextureInfo>();
                schemaMaterial.EmissiveTexture.Index = emissiveIndices.Index;
                if (emissiveIndices.TextureCoordIndex.HasValue)
                {
                    schemaMaterial.EmissiveTexture.TexCoord = emissiveIndices.TextureCoordIndex.Value;
                }
            }
            if (runtimeMaterial.AlphaMode.HasValue)
            {
                schemaMaterial.AlphaMode = runtimeMaterial.AlphaMode.Value;
            }
            if (runtimeMaterial.AlphaCutoff.HasValue)
            {
                schemaMaterial.AlphaCutoff = runtimeMaterial.AlphaCutoff.Value;
            }
            if (runtimeMaterial.Name != null)
            {
                schemaMaterial.Name = runtimeMaterial.Name;
            }
            if (runtimeMaterial.DoubleSided.HasValue)
            {
                schemaMaterial.DoubleSided = runtimeMaterial.DoubleSided.Value;
            }
            if (runtimeMaterial.Extensions != null)
            {
                var extensionsUsed = new List<string>();
                if (schemaMaterial.Extensions == null)
                {
                    schemaMaterial.Extensions = new Dictionary<string, object>();
                }
                if (gltf.ExtensionsUsed == null)
                {
                    gltf.ExtensionsUsed = new List<string>();
                }
                foreach (var runtimeExtension in runtimeMaterial.Extensions)
                {
                    object extension;
                    switch (runtimeExtension.Name)
                    {
                        case nameof(Extensions.KHR_materials_pbrSpecularGlossiness):
                            extension = ConvertPbrSpecularGlossinessExtensionToSchema((Extensions.KHR_materials_pbrSpecularGlossiness)runtimeExtension, gltf);
                            break;
                        case nameof(Extensions.FAKE_materials_quantumRendering):
                            extension = ConvertExtQuantumRenderingToSchema((Extensions.FAKE_materials_quantumRendering)runtimeExtension, gltf);
                            break;
                        default:
                            throw new NotImplementedException($"Extension schema conversion not implemented for {runtimeExtension.Name}");
                    }

                    schemaMaterial.Extensions.Add(runtimeExtension.Name, extension);

                    if (!extensionsUsed.Contains(runtimeExtension.Name))
                    {
                        extensionsUsed.Add(runtimeExtension.Name);
                    }
                }
                gltf.ExtensionsUsed = extensionsUsed;
            }
            materialToSchemaCache.Add(runtimeMaterial, schemaMaterial);

            return schemaMaterial;
        }

        /// <summary>
        /// Converts Runtime PbrSpecularGlossiness to Schema.
        /// </summary>
        private Loader.MaterialPbrSpecularGlossiness ConvertPbrSpecularGlossinessExtensionToSchema(Extensions.KHR_materials_pbrSpecularGlossiness specGloss, GLTF gltf)
        {
            var materialPbrSpecularGlossiness = CreateInstance<Loader.MaterialPbrSpecularGlossiness>();

            if (specGloss.DiffuseFactor.HasValue)
            {
                materialPbrSpecularGlossiness.DiffuseFactor = specGloss.DiffuseFactor.Value.ToArray();
            }
            if (specGloss.DiffuseTexture != null)
            {
                TextureInfo textureIndices = AddTexture(specGloss.DiffuseTexture);
                materialPbrSpecularGlossiness.DiffuseTexture = CreateInstance<Loader.TextureInfo>();
                materialPbrSpecularGlossiness.DiffuseTexture.Index = textureIndices.Index;
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.DiffuseTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (specGloss.SpecularFactor.HasValue)
            {
                materialPbrSpecularGlossiness.SpecularFactor = specGloss.SpecularFactor.Value.ToArray();
            }
            if (specGloss.GlossinessFactor.HasValue)
            {
                materialPbrSpecularGlossiness.GlossinessFactor = specGloss.GlossinessFactor.Value;
            }
            if (specGloss.SpecularGlossinessTexture != null)
            {
                TextureInfo textureIndices = AddTexture(specGloss.SpecularGlossinessTexture);
                materialPbrSpecularGlossiness.SpecularGlossinessTexture = CreateInstance<Loader.TextureInfo>();
                materialPbrSpecularGlossiness.SpecularGlossinessTexture.Index = textureIndices.Index;
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.SpecularGlossinessTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (specGloss.GlossinessFactor.HasValue)
            {
                materialPbrSpecularGlossiness.GlossinessFactor = specGloss.GlossinessFactor.Value;
            }

            return materialPbrSpecularGlossiness;
        }

        /// <summary>
        /// Converts Runtime Quantum Rendering to Schema. (Not an actual glTF feature) 
        /// </summary>
        private Loader.FAKE_materials_quantumRendering ConvertExtQuantumRenderingToSchema(Extensions.FAKE_materials_quantumRendering quantumRendering, GLTF gltf)
        {
            var materialEXT_QuantumRendering = CreateInstance<Loader.FAKE_materials_quantumRendering>();

            if (quantumRendering.PlanckFactor.HasValue)
            {
                materialEXT_QuantumRendering.PlanckFactor = quantumRendering.PlanckFactor.Value.ToArray();
            }
            if (quantumRendering.CopenhagenTexture != null)
            {
                TextureInfo textureIndices = AddTexture(quantumRendering.CopenhagenTexture);
                materialEXT_QuantumRendering.CopenhagenTexture = CreateInstance<Loader.TextureInfo>();
                materialEXT_QuantumRendering.CopenhagenTexture.Index = textureIndices.Index;
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialEXT_QuantumRendering.CopenhagenTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (quantumRendering.EntanglementFactor.HasValue)
            {
                materialEXT_QuantumRendering.EntanglementFactor = quantumRendering.EntanglementFactor.Value.ToArray();
            }
            if (quantumRendering.ProbabilisticFactor.HasValue)
            {
                materialEXT_QuantumRendering.ProbabilisticFactor = quantumRendering.ProbabilisticFactor.Value;
            }
            if (quantumRendering.SuperpositionCollapseTexture != null)
            {
                TextureInfo textureIndices = AddTexture(quantumRendering.SuperpositionCollapseTexture);
                materialEXT_QuantumRendering.SuperpositionCollapseTexture = CreateInstance<Loader.TextureInfo>();
                materialEXT_QuantumRendering.SuperpositionCollapseTexture.Index = textureIndices.Index;
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialEXT_QuantumRendering.SuperpositionCollapseTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (quantumRendering.ProbabilisticFactor.HasValue)
            {
                materialEXT_QuantumRendering.ProbabilisticFactor = quantumRendering.ProbabilisticFactor.Value;
            }

            return materialEXT_QuantumRendering;
        }


        /// <summary>
        /// Interleaves the primitive attributes to a single bufferview.
        /// </summary>
        private Dictionary<string, int> InterleaveMeshPrimitiveAttributes(MeshPrimitive meshPrimitive, Data geometryData, int bufferIndex)
        {
            var attributes = new Dictionary<string, int>();
            var availableAttributes = new HashSet<AttributeEnum>();
            var vertexCount = 0;

            // Create bufferview
            var bufferView = CreateBufferView(bufferIndex, "Interleaved attributes", 1, 0, null);
            int bufferviewIndex = bufferViews.Count;
            bufferViews.Add(bufferView);

            var byteOffset = 0;

            if (meshPrimitive.Positions != null && meshPrimitive.Positions.Any())
            {
                vertexCount = meshPrimitive.Positions.Count();
                // Get the max and min values
                Vector3[] minMaxPositions = GetMinMaxPositions(meshPrimitive);
                var min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                var max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                var positionAccessor = CreateAccessor(bufferviewIndex, byteOffset, ComponentTypeEnum.FLOAT, meshPrimitive.Positions.Count(), "Position Accessor", TypeEnum.VEC3, null, max, min);
                attributes.Add("POSITION", accessors.Count);
                accessors.Add(positionAccessor);
                availableAttributes.Add(AttributeEnum.POSITION);
                byteOffset += sizeof(float) * 3;
            }
            if (meshPrimitive.Normals != null && meshPrimitive.Normals.Any())
            {
                var normalAccessor = CreateAccessor(bufferviewIndex, byteOffset, ComponentTypeEnum.FLOAT, meshPrimitive.Normals.Count(), "Normal Accessor", TypeEnum.VEC3);
                attributes.Add("NORMAL", accessors.Count);
                accessors.Add(normalAccessor);
                availableAttributes.Add(AttributeEnum.NORMAL);
                byteOffset += sizeof(float) * 3;
            }
            if (meshPrimitive.Tangents != null && meshPrimitive.Tangents.Any())
            {
                var tangentAccessor = CreateAccessor(bufferviewIndex, byteOffset, ComponentTypeEnum.FLOAT, meshPrimitive.Tangents.Count(), "Tangent Accessor", TypeEnum.VEC4);
                attributes.Add("TANGENT", accessors.Count);
                accessors.Add(tangentAccessor);
                availableAttributes.Add(AttributeEnum.TANGENT);
                byteOffset += sizeof(float) * 4;
            }
            if (meshPrimitive.Colors != null && meshPrimitive.Colors.Any())
            {
                var normalized = false;
                TypeEnum vectorType;
                int offset;
                if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3)
                {
                    offset = 3;
                    vectorType = TypeEnum.VEC3;
                }
                else if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                {
                    offset = 4;
                    vectorType = TypeEnum.VEC4;
                }
                else
                {
                    throw new NotImplementedException($"Color of type {meshPrimitive.ColorType} not supported!");
                }
                ComponentTypeEnum colorAccessorComponentType;
                switch (meshPrimitive.ColorComponentType)
                {
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                        colorAccessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                        offset *= sizeof(byte);
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                        colorAccessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                        offset *= sizeof(ushort);
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                        colorAccessorComponentType = ComponentTypeEnum.FLOAT;
                        offset *= sizeof(float);
                        break;
                    default:
                        throw new NotImplementedException($"Color component type {meshPrimitive.ColorComponentType} not supported!");

                }
                var totalByteLength = (int)geometryData.Writer.BaseStream.Position;
                offset = GetPaddedSize(offset, 4);
                var colorAccessor = CreateAccessor(bufferviewIndex, byteOffset, colorAccessorComponentType, meshPrimitive.Colors.Count(), "Color Accessor", vectorType, normalized);
                attributes.Add("COLOR_0", accessors.Count);
                accessors.Add(colorAccessor);
                availableAttributes.Add(AttributeEnum.COLOR);
                byteOffset += offset;
            }
            if (meshPrimitive.TextureCoordSets != null && meshPrimitive.TextureCoordSets.Any())
            {
                int textureCoordSetIndex = 0;
                foreach (var textureCoordSet in meshPrimitive.TextureCoordSets)
                {
                    bool normalized = false;
                    ComponentTypeEnum accessorComponentType;
                    int offset = 0;
                    switch (meshPrimitive.TextureCoordsComponentType)
                    {
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                            accessorComponentType = ComponentTypeEnum.FLOAT;
                            offset = sizeof(float) * 2;
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                            accessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                            normalized = true;
                            offset = sizeof(byte) * 2;
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                            accessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                            normalized = true;
                            offset = sizeof(ushort) * 2;
                            break;
                        default:
                            throw new NotImplementedException($"Accessor component type {meshPrimitive.TextureCoordsComponentType} not supported!");
                    }
                    var textureCoordAccessor = CreateAccessor(bufferviewIndex, byteOffset, accessorComponentType, textureCoordSet.Count(), $"Texture Coord {textureCoordSetIndex}", TypeEnum.VEC2, normalized);
                    attributes.Add($"TEXCOORD_{textureCoordSetIndex}", accessors.Count);
                    accessors.Add(textureCoordAccessor);
                    availableAttributes.Add(textureCoordSetIndex == 0 ? AttributeEnum.TEXCOORDS_0 : AttributeEnum.TEXCOORDS_1);
                    offset = GetPaddedSize(offset, 4);
                    byteOffset += offset;
                    ++textureCoordSetIndex;
                }
            }
            bufferView.ByteStride = byteOffset;

            for (var i = 0; i < vertexCount; ++i)
            {
                foreach (var availableAttribute in availableAttributes)
                {
                    switch (availableAttribute)
                    {
                        case AttributeEnum.POSITION:
                            geometryData.Writer.Write(meshPrimitive.Positions.ElementAt(i));
                            break;
                        case AttributeEnum.NORMAL:
                            geometryData.Writer.Write(meshPrimitive.Normals.ElementAt(i));
                            break;
                        case AttributeEnum.TANGENT:
                            geometryData.Writer.Write(meshPrimitive.Tangents.ElementAt(i));
                            break;
                        case AttributeEnum.COLOR:
                            WriteColors(meshPrimitive, i, i, geometryData);
                            break;
                        case AttributeEnum.TEXCOORDS_0:
                            WriteTextureCoords(meshPrimitive, meshPrimitive.TextureCoordSets.First(), i, i, geometryData);
                            break;
                        case AttributeEnum.TEXCOORDS_1:
                            WriteTextureCoords(meshPrimitive, meshPrimitive.TextureCoordSets.ElementAt(1), i, i, geometryData);
                            break;
                        default:
                            throw new NotSupportedException($"The attribute {availableAttribute} is not currently supported to be interleaved!");
                    }
                    var totalByteLength = (int)geometryData.Writer.BaseStream.Position;
                    Align(geometryData.Writer);
                }
            }
            bufferView.ByteLength = (int)geometryData.Writer.BaseStream.Position;

            return attributes;
        }

        private int WriteTextureCoords(MeshPrimitive meshPrimitive, IEnumerable<Vector2> textureCoordSet, int min, int max, Data geometryData)
        {
            var byteLength = 0;
            var offset = (int)geometryData.Writer.BaseStream.Position;

            var count = max - min + 1;
            Vector2[] tcs = textureCoordSet.ToArray();
            byteLength = 8 * count;
            switch (meshPrimitive.TextureCoordsComponentType)
            {
                case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                    for (var i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(tcs[i]);
                    }
                    break;
                case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                    for (var i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(tcs[i].X * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(tcs[i].Y * byte.MaxValue)));
                        Align(geometryData.Writer);
                    }
                    break;
                case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                    for (var i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(tcs[i].X * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(tcs[i].Y * ushort.MaxValue)));
                    }
                    break;
                default:
                    throw new NotImplementedException($"Byte length calculation not implemented for TextureCoordsComponentType: {meshPrimitive.TextureCoordsComponentType}");
            }
            byteLength = (int)geometryData.Writer.BaseStream.Position - offset;

            return byteLength;
        }

        private int WriteColors(MeshPrimitive meshPrimitive, int min, int max, Data geometryData)
        {
            var byteLength = 0;
            var offset = (int)geometryData.Writer.BaseStream.Position;
            var count = max - min + 1;
            int vectorSize = meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;
            byteLength = 0;

            switch (meshPrimitive.ColorComponentType)
            {
                case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                    for (var i = min; i <= max; ++i)
                    {
                        Vector4 color = meshPrimitive.Colors.ElementAt(i);
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(color.X * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(color.Y * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(color.Z * byte.MaxValue)));
                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(Convert.ToByte(Math.Round(color.W * byte.MaxValue)));
                        }
                        Align(geometryData.Writer);
                    }
                    break;
                case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                    for (var i = min; i <= max; ++i)
                    {
                        Vector4 color = meshPrimitive.Colors.ElementAt(i);
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.X * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.Y * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.Z * ushort.MaxValue)));

                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.W * ushort.MaxValue)));
                        }
                        Align(geometryData.Writer);
                    }
                    break;
                case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                    for (var i = min; i <= max; ++i)
                    {
                        Vector4 color = meshPrimitive.Colors.ElementAt(i);
                        geometryData.Writer.Write(color.X);
                        geometryData.Writer.Write(color.Y);
                        geometryData.Writer.Write(color.Z);

                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(color.W);
                        }
                        Align(geometryData.Writer);
                    }
                    break;
            }

            byteLength = (int)geometryData.Writer.BaseStream.Position - offset;
            return byteLength;
        }

        /// <summary>
        /// Converts runtime animation to schema.
        /// </summary>
        private Loader.Animation ConvertAnimationToSchema(Animation runtimeAnimation, Loader.Buffer buffer, GLTF gltf, Data geometryData, int bufferIndex)
        {
            var schemaAnimation = CreateInstance<Loader.Animation>();
            var animationChannels = new List<Loader.AnimationChannel>();
            var animationSamplers = new List<Loader.AnimationSampler>();

            foreach (var runtimeAnimationChannel in runtimeAnimation.Channels)
            {
                var animationChannel = new Loader.AnimationChannel();
                Node targetNode = runtimeAnimationChannel.Target.Node;
                int sceneIndex = 0;
                if (gltf.Scene.HasValue)
                {
                    sceneIndex = gltf.Scene.Value;
                }

                // Create Animation Channel.
                animationChannel.Target = new Loader.AnimationChannelTarget();

                if (targetNode != null)
                {
                    animationChannel.Target.Node = ConvertNodeToSchema(targetNode, gltf, buffer, geometryData, bufferIndex);
                }

                switch (runtimeAnimationChannel.Target.Path)
                {
                    case AnimationChannelTarget.PathEnum.TRANSLATION:
                        animationChannel.Target.Path = Loader.AnimationChannelTarget.PathEnum.translation;
                        break;
                    case AnimationChannelTarget.PathEnum.ROTATION:
                        animationChannel.Target.Path = Loader.AnimationChannelTarget.PathEnum.rotation;
                        break;
                    case AnimationChannelTarget.PathEnum.SCALE:
                        animationChannel.Target.Path = Loader.AnimationChannelTarget.PathEnum.scale;
                        break;
                    case AnimationChannelTarget.PathEnum.WEIGHT:
                        animationChannel.Target.Path = Loader.AnimationChannelTarget.PathEnum.weights;
                        break;
                    default:
                        throw new NotSupportedException($"Animation target path {runtimeAnimationChannel.Target.Path} not supported!");
                }
                animationChannels.Add(animationChannel);

                if (animationSamplerToIndexCache.TryGetValue(runtimeAnimationChannel.Sampler, out int animationSamplerIndex))
                {
                    animationChannel.Sampler = animationSamplerIndex;
                }
                else
                {
                    // Create Animation Channel Sampler.
                    AnimationSampler runtimeSampler = runtimeAnimationChannel.Sampler;
                    var animationSampler = new Loader.AnimationSampler();

                    if (enumerableToIndexCache.TryGetValue(runtimeSampler.InputKeys, out int animationSamplerInputIndex))
                    {
                        animationSampler.Input = animationSamplerInputIndex;
                    }
                    else
                    {
                        // Write Input Key frames
                        var min = new[] { runtimeSampler.InputKeys.Min() };
                        var max = new[] { runtimeSampler.InputKeys.Max() };
                        var inputAccessor = CreateAccessor(bufferViews.Count, 0, ComponentTypeEnum.FLOAT, runtimeSampler.InputKeys.Count(), "Animation Sampler Input", TypeEnum.SCALAR, null, max, min);

                        var inputByteOffset = (int)geometryData.Writer.BaseStream.Position;
                        geometryData.Writer.Write(runtimeSampler.InputKeys);
                        var inputByteLength = (int)geometryData.Writer.BaseStream.Position - inputByteOffset;
                        var inputBufferView = CreateBufferView(bufferIndex, "Animation Sampler Input", inputByteLength, inputByteOffset, null);
                        bufferViews.Add(inputBufferView);

                        animationSampler.Input = accessors.Count;
                        accessors.Add(inputAccessor);
                        enumerableToIndexCache.Add(runtimeSampler.InputKeys, animationSampler.Input);
                    }

                    if (enumerableToIndexCache.TryGetValue(runtimeSampler.OutputKeys, out int animationSamplerOutputIndex))
                    {
                        animationSampler.Output = animationSamplerOutputIndex;
                    }
                    else
                    {
                        // Write the output key frame data
                        var outputByteOffset = (int)geometryData.Writer.BaseStream.Position;

                        Type runtimeSamplerType = runtimeSampler.GetType();
                        Type runtimeSamplerGenericTypeDefinition = runtimeSamplerType.GetGenericTypeDefinition();
                        Type runtimeSamplerGenericTypeArgument = runtimeSamplerType.GenericTypeArguments[0];

                        TypeEnum outputAccessorType;
                        if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                        {
                            outputAccessorType = TypeEnum.VEC3;
                        }
                        else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                        {
                            outputAccessorType = TypeEnum.VEC4;
                        }
                        else
                        {
                            throw new ArgumentException("Unsupported animation accessor type!");
                        }

                        // We need to align if the texture coord accessor type is not float.
                        bool normalized = runtimeSampler.OutputComponentType != AnimationSampler.ComponentTypeEnum.FLOAT;
                        ComponentTypeEnum accessorComponentType;
                        Action<float> writeKeys;
                        switch (runtimeSampler.OutputComponentType)
                        {
                            case AnimationSampler.ComponentTypeEnum.FLOAT:
                                accessorComponentType = ComponentTypeEnum.FLOAT;
                                writeKeys = value => geometryData.Writer.Write(value);
                                break;
                            case AnimationSampler.ComponentTypeEnum.NORMALIZED_BYTE:
                                accessorComponentType = ComponentTypeEnum.BYTE;
                                writeKeys = value => geometryData.Writer.Write(Convert.ToSByte(Math.Round(value * sbyte.MaxValue)));
                                break;
                            case AnimationSampler.ComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE:
                                // Unsigned is valid per the spec, but won't work except with positive rotation values.
                                accessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                                writeKeys = value => geometryData.Writer.Write(Convert.ToByte(Math.Round(value * byte.MaxValue)));
                                break;
                            case AnimationSampler.ComponentTypeEnum.NORMALIZED_SHORT:
                                accessorComponentType = ComponentTypeEnum.SHORT;
                                writeKeys = value => geometryData.Writer.Write(Convert.ToInt16(Math.Round(value * Int16.MaxValue)));
                                break;
                            case AnimationSampler.ComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT:
                                // Unsigned is valid per the spec, but won't work except with positive rotation values.
                                accessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                                writeKeys = value => geometryData.Writer.Write(Convert.ToUInt16(Math.Round(value * UInt16.MaxValue)));
                                break;
                            default: // Default to Float
                                throw new ArgumentException("Unsupported accessor component type!");
                        }

                        Loader.AnimationSampler.InterpolationEnum samplerInterpolation;
                        if (runtimeSamplerGenericTypeDefinition == typeof(StepAnimationSampler<>))
                        {
                            samplerInterpolation = Loader.AnimationSampler.InterpolationEnum.STEP;

                            if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                            {
                                geometryData.Writer.Write(((StepAnimationSampler<Vector3>)runtimeSampler).OutputKeys);
                            }
                            else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                            {
                                geometryData.Writer.Write(((StepAnimationSampler<Quaternion>)runtimeSampler).OutputKeys);
                            }
                            else
                            {
                                throw new ArgumentException("Unsupported animation sampler component type!");
                            }
                        }
                        else if (runtimeSamplerGenericTypeDefinition == typeof(LinearAnimationSampler<>))
                        {
                            samplerInterpolation = Loader.AnimationSampler.InterpolationEnum.LINEAR;

                            if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                            {
                                foreach (var value in ((LinearAnimationSampler<Vector3>)runtimeSampler).OutputKeys)
                                {
                                    writeKeys(value.X);
                                    writeKeys(value.Y);
                                    writeKeys(value.Z);
                                }
                            }
                            else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                            {
                                foreach (var value in ((LinearAnimationSampler<Quaternion>)runtimeSampler).OutputKeys)
                                {
                                    writeKeys(value.X);
                                    writeKeys(value.Y);
                                    writeKeys(value.Z);
                                    writeKeys(value.W);
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Unsupported animation sampler component type!");
                            }
                        }
                        else if (runtimeSamplerGenericTypeDefinition == typeof(CubicSplineAnimationSampler<>))
                        {
                            samplerInterpolation = Loader.AnimationSampler.InterpolationEnum.CUBICSPLINE;

                            if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                            {
                                ((CubicSplineAnimationSampler<Vector3>)runtimeSampler).OutputKeys.ForEach(key =>
                                {
                                    geometryData.Writer.Write(key.InTangent);
                                    geometryData.Writer.Write(key.Value);
                                    geometryData.Writer.Write(key.OutTangent);
                                });
                            }
                            else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                            {
                                ((CubicSplineAnimationSampler<Quaternion>)runtimeSampler).OutputKeys.ForEach(key =>
                                {
                                    geometryData.Writer.Write(key.InTangent);
                                    geometryData.Writer.Write(key.Value);
                                    geometryData.Writer.Write(key.OutTangent);
                                });
                            }
                            else
                            {
                                throw new ArgumentException("Unsupported animation sampler component type!");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }

                        if (normalized)
                        {
                            Align(geometryData.Writer);
                        }

                        int outputCount = samplerInterpolation == Loader.AnimationSampler.InterpolationEnum.CUBICSPLINE ? runtimeSampler.InputKeys.Count() * 3 : runtimeSampler.InputKeys.Count();
                        var outputAccessor = CreateAccessor(bufferViews.Count, 0, accessorComponentType, outputCount, "Animation Sampler Output", outputAccessorType, normalized);
                        animationSampler.Interpolation = samplerInterpolation;
                        animationSampler.Output = accessors.Count;
                        accessors.Add(outputAccessor);

                        var outputByteLength = (int)geometryData.Writer.BaseStream.Position - outputByteOffset;
                        var outputBufferView = CreateBufferView(bufferIndex, "Animation Sampler Output", outputByteLength, outputByteOffset, null);
                        bufferViews.Add(outputBufferView);

                        if (runtimeSampler is LinearAnimationSampler<System.Numerics.Quaternion>)
                        {
                            enumerableToIndexCache.Add(runtimeSampler.OutputKeys, animationSampler.Output);
                        }
                    }

                    animationChannel.Sampler = animationSamplers.Count;
                    animationSamplers.Add(animationSampler);
                    animationSamplerToIndexCache.Add(runtimeAnimationChannel.Sampler, animationChannel.Sampler);
                }
            }

            schemaAnimation.Channels = animationChannels.ToArray();
            schemaAnimation.Samplers = animationSamplers.ToArray();

            return schemaAnimation;
        }

        /// <summary>
        /// Converts runtime mesh primitive to schema.
        /// </summary>
        private Loader.MeshPrimitive ConvertMeshPrimitiveToSchema(Node runtimeNode, MeshPrimitive runtimeMeshPrimitive, GLTF gltf, Loader.Buffer buffer, Data geometryData, int bufferIndex)
        {
            if (meshPrimitiveToSchemaCache.TryGetValue(runtimeMeshPrimitive, out Loader.MeshPrimitive schemaMeshPrimitive))
            {
                return schemaMeshPrimitive;
            }
            schemaMeshPrimitive = CreateInstance<Loader.MeshPrimitive>();
            var attributes = new Dictionary<string, int>();
            if (runtimeMeshPrimitive.Interleave != null && runtimeMeshPrimitive.Interleave == true)
            {
                attributes = InterleaveMeshPrimitiveAttributes(runtimeMeshPrimitive, geometryData, bufferIndex);
            }
            else
            {
                if (runtimeMeshPrimitive.Positions != null)
                {
                    if (enumerableToIndexCache.TryGetValue(runtimeMeshPrimitive.Positions, out int positionAccessorIndex))
                    {
                        attributes.Add("POSITION", positionAccessorIndex);
                    }
                    else
                    {
                        // Get the max and min values
                        var minMaxPositions = GetMinMaxPositions(runtimeMeshPrimitive);
                        float[] min = { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                        float[] max = { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };

                        // Create BufferView for the position
                        Align(geometryData.Writer);
                        int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Positions.Count();
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);
                        var bufferViewIndex = bufferViews.Count;
                        bufferViews.Add(bufferView);

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferViewIndex, 0, ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Positions.Count(), "Positions Accessor", TypeEnum.VEC3, null, max, min);
                        geometryData.Writer.Write(runtimeMeshPrimitive.Positions.ToArray());
                        attributes.Add("POSITION", accessors.Count);
                        enumerableToIndexCache.Add(runtimeMeshPrimitive.Positions, accessors.Count);
                        accessors.Add(accessor);
                    }
                }
                if (runtimeMeshPrimitive.Normals != null)
                {
                    if (enumerableToIndexCache.TryGetValue(runtimeMeshPrimitive.Normals, out int normalAccessorIndex))
                    {
                        attributes.Add("NORMAL", normalAccessorIndex);
                    }
                    else
                    {
                        // Write to Buffer and create BufferView
                        Align(geometryData.Writer);
                        int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Normals.Count();
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);
                        int bufferViewIndex = bufferViews.Count;
                        bufferViews.Add(bufferView);

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferViewIndex, 0, ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Normals.Count(), "Normals Accessor", TypeEnum.VEC3);
                        geometryData.Writer.Write(runtimeMeshPrimitive.Normals.ToArray());
                        attributes.Add("NORMAL", accessors.Count);
                        enumerableToIndexCache.Add(runtimeMeshPrimitive.Normals, accessors.Count);
                        accessors.Add(accessor);
                    }
                }
                if (runtimeMeshPrimitive.Tangents != null && runtimeMeshPrimitive.Tangents.Any())
                {
                    if (enumerableToIndexCache.TryGetValue(runtimeMeshPrimitive.Tangents, out int tangentAccessorIndex))
                    {
                        attributes.Add("TANGENT", tangentAccessorIndex);
                    }
                    else
                    {
                        // Create BufferView
                        Align(geometryData.Writer);
                        int byteLength = sizeof(float) * 4 * runtimeMeshPrimitive.Tangents.Count();
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);
                        var bufferViewIndex = bufferViews.Count;
                        bufferViews.Add(bufferView);

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferViewIndex, 0, ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Tangents.Count(), "Tangents Accessor", TypeEnum.VEC4);
                        geometryData.Writer.Write(runtimeMeshPrimitive.Tangents.ToArray());
                        attributes.Add("TANGENT", accessors.Count);
                        enumerableToIndexCache.Add(runtimeMeshPrimitive.Tangents, accessors.Count);
                        accessors.Add(accessor);
                    }
                }
                if (runtimeMeshPrimitive.Colors != null)
                {
                    if (enumerableToIndexCache.TryGetValue(runtimeMeshPrimitive.Colors, out int colorsAccessorIndex))
                    {
                        attributes.Add("COLOR_0", colorsAccessorIndex);
                    }
                    else
                    {
                        var colorAccessorComponentType = ComponentTypeEnum.FLOAT;
                        var colorAccessorType = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? TypeEnum.VEC3 : TypeEnum.VEC4;
                        int vectorSize = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;

                        // Create BufferView
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;

                        int byteLength = WriteColors(runtimeMeshPrimitive, 0, runtimeMeshPrimitive.Colors.Count() - 1, geometryData);
                        int? byteStride = null;
                        switch (runtimeMeshPrimitive.ColorComponentType)
                        {
                            case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                                colorAccessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                                if (vectorSize == 3)
                                {
                                    byteStride = 4;
                                }
                                break;
                            case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                                colorAccessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                                if (vectorSize == 3)
                                {
                                    byteStride = 8;
                                }
                                break;
                            default: // Default to ColorComponentTypeEnum.FLOAT:
                                colorAccessorComponentType = ComponentTypeEnum.FLOAT;
                                break;
                        }

                        var bufferView = CreateBufferView(bufferIndex, "Colors", byteLength, byteOffset, byteStride);
                        var bufferviewIndex = bufferViews.Count;
                        bufferViews.Add(bufferView);

                        // Create an accessor for the bufferView
                        // We normalize if the color accessor mode is not set to FLOAT.
                        bool normalized = runtimeMeshPrimitive.ColorComponentType != MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                        var accessor = CreateAccessor(bufferviewIndex, 0, colorAccessorComponentType, runtimeMeshPrimitive.Colors.Count(), "Colors Accessor", colorAccessorType, normalized);
                        attributes.Add("COLOR_0", accessors.Count);
                        enumerableToIndexCache.Add(runtimeMeshPrimitive.Colors, accessors.Count);
                        accessors.Add(accessor);
                        if (normalized)
                        {
                            Align(geometryData.Writer);
                        }
                    }
                }
                if (runtimeMeshPrimitive.TextureCoordSets != null)
                {
                    var i = 0;
                    foreach (var textureCoordSet in runtimeMeshPrimitive.TextureCoordSets)
                    {
                        if (enumerableToIndexCache.TryGetValue(textureCoordSet, out int textureCoordsAccessorIndex))
                        {
                            attributes.Add($"TEXCOORD_{i}", textureCoordsAccessorIndex);
                        }
                        else
                        {
                            var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                            int byteLength = WriteTextureCoords(runtimeMeshPrimitive, textureCoordSet, 0, runtimeMeshPrimitive.TextureCoordSets.ElementAt(i).Count() - 1, geometryData);

                            Loader.Accessor accessor;
                            ComponentTypeEnum accessorComponentType;
                            // We normalize only if the texture coord accessor type is not float.
                            bool normalized = runtimeMeshPrimitive.TextureCoordsComponentType != MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                            int? byteStride = null;
                            switch (runtimeMeshPrimitive.TextureCoordsComponentType)
                            {
                                case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                                    accessorComponentType = ComponentTypeEnum.FLOAT;
                                    break;
                                case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                                    accessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                                    byteStride = 4;
                                    break;
                                case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                                    accessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                                    break;
                                default: // Default to Float
                                    accessorComponentType = ComponentTypeEnum.FLOAT;
                                    break;
                            }

                            var bufferView = CreateBufferView(bufferIndex, $"Texture Coords {i}", byteLength, byteOffset, byteStride);
                            var bufferviewIndex = bufferViews.Count;
                            bufferViews.Add(bufferView);

                            // Create Accessor
                            attributes.Add($"TEXCOORD_{i}", accessors.Count);
                            enumerableToIndexCache.Add(textureCoordSet, accessors.Count);
                            accessor = CreateAccessor(bufferviewIndex, 0, accessorComponentType, textureCoordSet.Count(), $"UV Accessor {i}", TypeEnum.VEC2, normalized);
                            accessors.Add(accessor);

                            // Add any additional bytes if the data is normalized
                            if (normalized)
                            {
                                Align(geometryData.Writer);
                            }
                        }
                        ++i;
                    }
                }

            }
            if (runtimeMeshPrimitive.Indices != null && runtimeMeshPrimitive.Indices.Any())
            {
                if (enumerableToIndexCache.TryGetValue(runtimeMeshPrimitive.Indices, out int indicesAccessorIndex))
                {
                    schemaMeshPrimitive.Indices = indicesAccessorIndex;
                }
                else
                {
                    int byteLength;
                    var byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    ComponentTypeEnum indexComponentType;

                    switch (runtimeMeshPrimitive.IndexComponentType)
                    {
                        case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE:
                            indexComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                            byteLength = sizeof(byte) * runtimeMeshPrimitive.Indices.Count();
                            break;
                        case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT:
                            byteLength = sizeof(ushort) * runtimeMeshPrimitive.Indices.Count();
                            indexComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                            break;
                        case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT:
                            byteLength = sizeof(uint) * runtimeMeshPrimitive.Indices.Count();
                            indexComponentType = ComponentTypeEnum.UNSIGNED_INT;
                            break;
                        default:
                            throw new InvalidEnumArgumentException($"Unrecognized Index Component Type Enum {runtimeMeshPrimitive.IndexComponentType}");
                    }
                    var bufferView = CreateBufferView(bufferIndex, "Indices", byteLength, byteOffset, null);
                    var bufferviewIndex = bufferViews.Count;
                    bufferViews.Add(bufferView);

                    var accessor = CreateAccessor(bufferviewIndex, 0, indexComponentType, runtimeMeshPrimitive.Indices.Count(), "Indices Accessor", TypeEnum.SCALAR);
                    switch (indexComponentType)
                    {
                        case ComponentTypeEnum.UNSIGNED_INT:
                            foreach (var index in runtimeMeshPrimitive.Indices)
                            {
                                geometryData.Writer.Write(Convert.ToUInt32(index));
                            }
                            break;
                        case ComponentTypeEnum.UNSIGNED_BYTE:
                            foreach (var index in runtimeMeshPrimitive.Indices)
                            {
                                geometryData.Writer.Write(Convert.ToByte(index));
                            }
                            break;
                        case ComponentTypeEnum.UNSIGNED_SHORT:
                            foreach (var index in runtimeMeshPrimitive.Indices)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(index));
                            }
                            break;
                        default:
                            throw new InvalidEnumArgumentException("Unsupported Index Component Type");
                    }

                    schemaMeshPrimitive.Indices = accessors.Count;
                    enumerableToIndexCache.Add(runtimeMeshPrimitive.Indices, accessors.Count);
                    accessors.Add(accessor);
                }
            }

            var vertexJointWeights = runtimeMeshPrimitive.VertexJointWeights;
            if (vertexJointWeights != null && vertexJointWeights.Any())
            {
                var numJointWeights = vertexJointWeights.First().Count();
                if (!vertexJointWeights.All(jointWeights => jointWeights.Count() == numJointWeights))
                {
                    throw new ArgumentException("All vertex joint weights must have equal count");
                }

                ComponentTypeEnum weightAccessorComponentType;
                bool weightAccessorNormalized;
                Action<float> writeWeight;
                switch (runtimeMeshPrimitive.WeightComponentType)
                {
                    case MeshPrimitive.WeightComponentTypeEnum.FLOAT:
                        {
                            weightAccessorComponentType = ComponentTypeEnum.FLOAT;
                            weightAccessorNormalized = false;
                            writeWeight = value => geometryData.Writer.Write(value);
                            break;
                        }
                    case MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE:
                        {
                            weightAccessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                            weightAccessorNormalized = true;
                            writeWeight = value => geometryData.Writer.Write(Convert.ToByte(Math.Round(value * byte.MaxValue)));
                            break;
                        }
                    case MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT:
                        {
                            weightAccessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                            weightAccessorNormalized = true;
                            writeWeight = value => geometryData.Writer.Write(Convert.ToUInt16(Math.Round(value * ushort.MaxValue)));
                            break;
                        }
                    default:
                        {
                            throw new InvalidEnumArgumentException();
                        }
                }

                ComponentTypeEnum jointAccessorComponentType;
                Action<int> writeJointIndex;
                switch (runtimeMeshPrimitive.JointComponentType)
                {
                    case MeshPrimitive.JointComponentTypeEnum.UNSIGNED_BYTE:
                        jointAccessorComponentType = ComponentTypeEnum.UNSIGNED_BYTE;
                        writeJointIndex = jointIndex => geometryData.Writer.Write(Convert.ToByte(jointIndex));
                        break;
                    case MeshPrimitive.JointComponentTypeEnum.UNSIGNED_SHORT:
                        jointAccessorComponentType = ComponentTypeEnum.UNSIGNED_SHORT;
                        writeJointIndex = jointIndex => geometryData.Writer.Write(Convert.ToUInt16(jointIndex));
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }

                var numVertices = vertexJointWeights.Count();
                var numSets = (numJointWeights + 3) / 4;

                for (var set = 0; set < numSets; set++)
                {
                    var start = set * 4;
                    IEnumerable<JointWeight> jointWeightsSet = vertexJointWeights
                        .Select(jointWeights => jointWeights.Where((_, index) => start <= index && index < start + 4))
                        .Select(jointWeights => jointWeights.Concat(new JointWeight[4 - jointWeights.Count()]))
                        .SelectMany(weights => weights);

                    // Weights
                    {
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;

                        foreach (var jointWeight in jointWeightsSet)
                        {
                            writeWeight(jointWeight == null ? 0 : jointWeight.Weight);
                        }

                        var byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;

                        var bufferViewIndex = bufferViews.Count();
                        bufferViews.Add(CreateBufferView(bufferIndex, "weights buffer view", byteLength, byteOffset, null));

                        Align(geometryData.Writer);

                        var accessorIndex = accessors.Count();
                        accessors.Add(CreateAccessor(bufferViewIndex, 0, weightAccessorComponentType, numVertices, "weights accessor", TypeEnum.VEC4, weightAccessorNormalized));
                        attributes.Add($"WEIGHTS_{set}", accessorIndex);
                    }

                    // Joints
                    {
                        var byteOffset = (int)geometryData.Writer.BaseStream.Position;

                        foreach (JointWeight jointWeight in jointWeightsSet)
                        {
                            writeJointIndex(jointWeight?.JointIndex ?? 0);
                        }

                        var byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;

                        var bufferViewIndex = bufferViews.Count();
                        bufferViews.Add(CreateBufferView(bufferIndex, "joint indices buffer view", byteLength, byteOffset, null));

                        Align(geometryData.Writer);

                        var accessorIndex = accessors.Count();
                        accessors.Add(CreateAccessor(bufferViewIndex, 0, jointAccessorComponentType, numVertices, "joint indices accessor", TypeEnum.VEC4, false));
                        attributes.Add($"JOINTS_{set}", accessorIndex);
                    }
                }
            }

            schemaMeshPrimitive.Attributes = attributes;

            if (runtimeMeshPrimitive.Material != null)
            {
                var nMaterial = ConvertMaterialToSchema(runtimeMeshPrimitive.Material, gltf);
                // If an equivalent material has already been created, re-use that material's index instead of creating a new material.
                var findMaterialIndex = materials.IndexOf(nMaterial);
                if (findMaterialIndex > -1)
                {
                    schemaMeshPrimitive.Material = findMaterialIndex;
                }
                else
                {
                    schemaMeshPrimitive.Material = materials.Count;
                    materials.Add(nMaterial);
                }
            }

            switch (runtimeMeshPrimitive.Mode)
            {
                case MeshPrimitive.ModeEnum.TRIANGLES:
                    // glTF defaults to triangles
                    break;
                case MeshPrimitive.ModeEnum.POINTS:
                    schemaMeshPrimitive.Mode = ModeEnum.POINTS;
                    break;
                case MeshPrimitive.ModeEnum.LINES:
                    schemaMeshPrimitive.Mode = ModeEnum.LINES;
                    break;
                case MeshPrimitive.ModeEnum.LINE_LOOP:
                    schemaMeshPrimitive.Mode = ModeEnum.LINE_LOOP;
                    break;
                case MeshPrimitive.ModeEnum.LINE_STRIP:
                    schemaMeshPrimitive.Mode = ModeEnum.LINE_STRIP;
                    break;
                case MeshPrimitive.ModeEnum.TRIANGLE_FAN:
                    schemaMeshPrimitive.Mode = ModeEnum.TRIANGLE_FAN;
                    break;
                case MeshPrimitive.ModeEnum.TRIANGLE_STRIP:
                    schemaMeshPrimitive.Mode = ModeEnum.TRIANGLE_STRIP;
                    break;
            }

            meshPrimitiveToSchemaCache.Add(runtimeMeshPrimitive, schemaMeshPrimitive);

            return schemaMeshPrimitive;
        }

        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as an array of two vectors, minimum and maximum respectively.</returns>
        private Vector3[] GetMinMaxPositions(MeshPrimitive meshPrimitive)
        {
            // Get the max and min values
            Vector3 minVal = new Vector3
            {
                X = float.MaxValue,
                Y = float.MaxValue,
                Z = float.MaxValue
            };
            Vector3 maxVal = new Vector3
            {
                X = float.MinValue,
                Y = float.MinValue,
                Z = float.MinValue
            };
            foreach (Vector3 position in meshPrimitive.Positions)
            {
                maxVal.X = Math.Max(position.X, maxVal.X);
                maxVal.Y = Math.Max(position.Y, maxVal.Y);
                maxVal.Z = Math.Max(position.Z, maxVal.Z);

                minVal.X = Math.Min(position.X, minVal.X);
                minVal.Y = Math.Min(position.Y, minVal.Y);
                minVal.Z = Math.Min(position.Z, minVal.Z);
            }
            Vector3[] results = { minVal, maxVal };
            return results;
        }
    }
}
