﻿using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Convert Runtime Abstraction to Schema.
    /// </summary>
    internal class GLTFConverter
    {
        private List<glTFLoader.Schema.Buffer> buffers = new List<glTFLoader.Schema.Buffer>();
        private List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
        private List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
        private List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
        private List<glTFLoader.Schema.Node> nodes = new List<glTFLoader.Schema.Node>();
        private List<glTFLoader.Schema.Scene> scenes = new List<glTFLoader.Schema.Scene>();
        private List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
        private List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
        private List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
        private List<glTFLoader.Schema.Mesh> meshes = new List<glTFLoader.Schema.Mesh>();
        private List<glTFLoader.Schema.Animation> animations = new List<glTFLoader.Schema.Animation>();
        private List<glTFLoader.Schema.Skin> skins = new List<glTFLoader.Schema.Skin>();

        private Dictionary<Node, int> nodeToIndexCache = new Dictionary<Node, int>();
        private Dictionary<Texture, TextureIndices> textureToTextureIndicesCache = new Dictionary<Texture, TextureIndices>();
        private Dictionary<Image, int> imageToIndexCache = new Dictionary<Image, int>();
        private Dictionary<Mesh, glTFLoader.Schema.Mesh> meshToSchemaCache = new Dictionary<Mesh, glTFLoader.Schema.Mesh>();
        private Dictionary<Image, glTFLoader.Schema.Image> imageToSchemaCache = new Dictionary<Image, glTFLoader.Schema.Image>();
        private Dictionary<Sampler, glTFLoader.Schema.Sampler> samplerToSchemaCache = new Dictionary<Sampler, glTFLoader.Schema.Sampler>();
        private Dictionary<Animation, glTFLoader.Schema.Animation> animationToSchemaCache = new Dictionary<Animation, glTFLoader.Schema.Animation>();
        private Dictionary<MeshPrimitive, glTFLoader.Schema.MeshPrimitive> meshPrimitiveToSchemaCache = new Dictionary<MeshPrimitive, glTFLoader.Schema.MeshPrimitive>();
        private Dictionary<Material, glTFLoader.Schema.Material> materialToSchemaCache = new Dictionary<Material, glTFLoader.Schema.Material>();
        private Dictionary<Skin, int> skinsToIndexCache = new Dictionary<Skin, int>();
        private enum AttributeEnum { POSITION, NORMAL, TANGENT, COLOR, TEXCOORDS_0, TEXCOORDS_1, JOINTS_0, WEIGHTS_0 };

        /// <summary>
        /// Set this property to allow creating custom types.
        /// </summary>
        public Func<Type, object> CreateInstanceOverride = type => Activator.CreateInstance(type);

        /// <summary>
        /// Utility struct for holding sampler, image and texture coord indices
        /// </summary>
        private struct TextureIndices
        {
            public int? SamplerIndex;
            public int? ImageIndex;
            public int? TextureCoordIndex;
        }

        /// <summary>
        /// Converts Runtime GLTF to Schema GLTF object.
        /// </summary>
        public glTFLoader.Schema.Gltf ConvertRuntimeToSchema(GLTF runtimeGLTF, Data geometryData)
        {
            var gltf = this.CreateInstance<glTFLoader.Schema.Gltf>();

            if (runtimeGLTF.Asset != null)
            {
                gltf.Asset = ConvertAssetToSchema(runtimeGLTF.Asset);
            }

            var buffer = CreateInstance<glTFLoader.Schema.Buffer>();
            buffer.Uri = geometryData.Name;

            // for each scene, create a node for each mesh and compute the indices for the scene object
            foreach (var runtimeScene in runtimeGLTF.Scenes)
            {
                var sceneIndicesSet = new List<int>();
                // loops through each mesh and converts it into a Node, with optional transformation info if available
                foreach (var node in runtimeScene.Nodes)
                {
                    sceneIndicesSet.Add(ConvertNodeToSchema(node, runtimeGLTF, buffer, geometryData, bufferIndex: 0));
                }

                var scene = CreateInstance<glTFLoader.Schema.Scene>();
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
                var animations = new List<glTFLoader.Schema.Animation>();
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
            return (T)this.CreateInstanceOverride(typeof(T));
        }

        private int AddToSchemaSamplers(glTFLoader.Schema.Sampler sampler)
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
        private int AddToSchemaImages(glTFLoader.Schema.Image image)
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
        /// converts the Runtime image to a glTF Image
        /// </summary>
        /// <returns>Returns a gltf Image object</returns>
        private glTFLoader.Schema.Image ConvertImageToSchema(Image runtimeImage)
        {
            if (this.imageToSchemaCache.TryGetValue(runtimeImage, out glTFLoader.Schema.Image schemaImage))
            {
                return schemaImage;
            }
            schemaImage = CreateInstance<glTFLoader.Schema.Image>();

            schemaImage.Uri = runtimeImage.Uri;

            if (runtimeImage.MimeType.HasValue)
            {
                schemaImage.MimeType = runtimeImage.MimeType.Value;
            }

            if (runtimeImage.Name != null)
            {
                schemaImage.Name = runtimeImage.Name;
            }
            this.imageToSchemaCache.Add(runtimeImage, schemaImage);

            return schemaImage;
        }

        private glTFLoader.Schema.Sampler ConvertSamplerToSchema(Sampler runtimeSampler)
        {
            if (this.samplerToSchemaCache.TryGetValue(runtimeSampler, out glTFLoader.Schema.Sampler schemaSampler))
            {
                return schemaSampler;
            }
            schemaSampler = CreateInstance<glTFLoader.Schema.Sampler>();

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
            this.samplerToSchemaCache.Add(runtimeSampler, schemaSampler);

            return schemaSampler;
        }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <returns>Returns the indices of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        private TextureIndices AddTexture(Texture runtimeTexture)
        {
            if (this.textureToTextureIndicesCache.TryGetValue(runtimeTexture, out TextureIndices textureIndices))
            {
                return textureIndices;
            }

            var indices = new List<int>();
            int? samplerIndex = null;
            int? imageIndex = null;
            int? textureCoordIndex = null;

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

                var texture = CreateInstance<glTFLoader.Schema.Texture>();
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
                // If an equivalent texture has already been created, re-use that texture's index instead of creating a new texture
                int findTextureIndex = -1;
                if (textures.Count > 0)
                {
                    for (int i = 0; i < textures.Count(); ++i)
                    {
                        if (textures[i].TexturesEqual(texture))
                        {
                            findTextureIndex = i;
                            break;
                        }
                    }
                }
                
                if (findTextureIndex > -1)
                {
                    indices.Add(findTextureIndex);
                }
                else
                {
                    textures.Add(texture);
                    indices.Add(textures.Count() - 1);
                }

                if (runtimeTexture.TexCoordIndex.HasValue)
                {
                    indices.Add(runtimeTexture.TexCoordIndex.Value);
                    textureCoordIndex = runtimeTexture.TexCoordIndex.Value;
                }
            }

            textureIndices = new TextureIndices
            {
                SamplerIndex = samplerIndex,
                ImageIndex = imageIndex,
                TextureCoordIndex = textureCoordIndex
            };

            textureToTextureIndicesCache.Add(runtimeTexture, textureIndices);

            return textureIndices;
        }

        private int GetPaddedSize(int value, int size)
        {
            var remainder = value % size;
            return (remainder == 0 ? value : checked(value + size - remainder));
        }

        /// <summary>
        /// Pads a value to ensure it is a multiple of size
        /// </summary>
        private int Align(Data geometryData, int value, int size)
        {
            var paddedValue = GetPaddedSize(value, size);

            int additionalPaddedBytes = paddedValue - value;
            for (int i = 0; i < additionalPaddedBytes; ++i)
            {
                geometryData.Writer.Write((byte)0);
            }
            value += additionalPaddedBytes;

            return value;
        }

        /// <summary>
        /// Creates a bufferview schema object
        /// </summary>
        private glTFLoader.Schema.BufferView CreateBufferView(int bufferIndex, string name, int byteLength, int byteOffset, int? byteStride)
        {
            var bufferView = CreateInstance<glTFLoader.Schema.BufferView>();

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
        /// Creates an accessor schema object
        /// </summary>
        private glTFLoader.Schema.Accessor CreateAccessor(int bufferviewIndex, int? byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, glTFLoader.Schema.Accessor.TypeEnum? type, bool? normalized)
        {
            var accessor = CreateInstance<glTFLoader.Schema.Accessor>();

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
        private glTFLoader.Schema.Asset ConvertAssetToSchema(Asset runtimeAsset)
        {
            var extras = CreateInstance<glTFLoader.Schema.Extras>();
            var schemaAsset = CreateInstance<glTFLoader.Schema.Asset>();

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
        private int ConvertNodeToSchema(Node runtimeNode, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            if (this.nodeToIndexCache.TryGetValue(runtimeNode, out int nodeIndex))
            {
                return nodeIndex;
            }

            var node = CreateInstance<glTFLoader.Schema.Node>();
            nodes.Add(node);
            nodeIndex = nodes.Count() - 1;
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
                meshes.Add(schemaMesh);
                node.Mesh = meshes.Count() - 1;
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
                    var schemaChildIndex = ConvertNodeToSchema(childNode, gltf, buffer, geometryData, bufferIndex);
                    childrenIndices.Add(schemaChildIndex);
                }
                node.Children = childrenIndices.ToArray();
            }
            if (runtimeNode.Skin?.SkinJoints?.Any() == true)
            {
                int skinIndex;
                if (this.skinsToIndexCache.TryGetValue(runtimeNode.Skin, out skinIndex))
                {
                    node.Skin = skinIndex;
                }
                else
                {
                    var inverseBindMatrices = runtimeNode.Skin.SkinJoints.Select(skinJoint => skinJoint.InverseBindMatrix);

                    int? inverseBindMatricesAccessorIndex = null;
                    if (inverseBindMatrices.Any(inverseBindMatrix => !inverseBindMatrix.IsIdentity))
                    {
                        int inverseBindMatricesByteOffset = (int)geometryData.Writer.BaseStream.Position;
                        geometryData.Writer.Write(inverseBindMatrices);
                        int inverseBindMatricesByteLength = (int)geometryData.Writer.BaseStream.Position - inverseBindMatricesByteOffset;

                        // create bufferview
                        var inverseBindMatricesBufferView = CreateBufferView(bufferIndex, "Inverse Bind Matrix", inverseBindMatricesByteLength, inverseBindMatricesByteOffset, null);
                        bufferViews.Add(inverseBindMatricesBufferView);

                        // create accessor
                        var inverseBindMatricesAccessor = CreateAccessor(bufferViews.Count() - 1, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, inverseBindMatrices.Count(), "IBM", null, null, glTFLoader.Schema.Accessor.TypeEnum.MAT4, null);
                        accessors.Add(inverseBindMatricesAccessor);
                        inverseBindMatricesAccessorIndex = accessors.Count() - 1;
                    }

                    var jointIndices = runtimeNode.Skin.SkinJoints.Select(SkinJoint => ConvertNodeToSchema(SkinJoint.Node, gltf, buffer, geometryData, bufferIndex));

                    var skin = new glTFLoader.Schema.Skin
                    {
                        Name = runtimeNode.Skin.Name,
                        Joints = jointIndices.ToArray(),
                        InverseBindMatrices = inverseBindMatricesAccessorIndex,
                    };
                    skins.Add(skin);
                    this.skinsToIndexCache.Add(runtimeNode.Skin, skins.Count() - 1);
                    node.Skin = this.skinsToIndexCache[runtimeNode.Skin];
                }
            }
            nodeToIndexCache.Add(runtimeNode, nodeIndex);

            return nodeIndex;
        }

        /// <summary>
        /// Converts the morph target list of dictionaries into Morph Target
        /// </summary>
        private IEnumerable<Dictionary<string, int>> GetMeshPrimitiveMorphTargets(MeshPrimitive meshPrimitive, List<float> weights, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
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
                            int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                            var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);

                            bufferViews.Add(bufferView);
                            int bufferviewIndex = bufferViews.Count() - 1;

                            // Create an accessor for the bufferView
                            var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Positions.Count(), "Positions Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                            accessors.Add(accessor);
                            geometryData.Writer.Write(morphTarget.Positions.ToArray());
                            morphTargetAttributes.Add("POSITION", accessors.Count() - 1);
                        }
                    }
                    if (morphTarget.Normals != null && morphTarget.Normals.Any())
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Normals.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Normals.ToArray());
                        morphTargetAttributes.Add("NORMAL", accessors.Count() - 1);
                    }
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Any())
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Tangents.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Tangents.ToArray());
                        morphTargetAttributes.Add("TANGENT", accessors.Count() - 1);
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
        private glTFLoader.Schema.Mesh ConvertMeshToSchema(Node runtimeNode, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            if (this.meshToSchemaCache.TryGetValue(runtimeNode.Mesh, out glTFLoader.Schema.Mesh schemaMesh))
            {
                return schemaMesh;
            }
            var runtimeMesh = runtimeNode.Mesh;
            schemaMesh = CreateInstance<glTFLoader.Schema.Mesh>();
            var primitives = new List<glTFLoader.Schema.MeshPrimitive>(runtimeMesh.MeshPrimitives.Count());
            var weights = new List<float>();
            // Loops through each wrapped mesh primitive within the mesh and converts them to mesh primitives, as well as updating the
            // indices in the lists
            foreach (var gPrimitive in runtimeMesh.MeshPrimitives)
            {
                glTFLoader.Schema.MeshPrimitive mPrimitive = ConvertMeshPrimitiveToSchema(runtimeNode, gPrimitive, gltf, buffer, geometryData, bufferIndex);
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
            this.meshToSchemaCache.Add(runtimeNode.Mesh, schemaMesh);

            return schemaMesh;
        }

        /// <summary>
        /// Converts runtime material to schema.
        /// </summary>
        private glTFLoader.Schema.Material ConvertMaterialToSchema(Material runtimeMaterial, GLTF gltf)
        {
            if (this.materialToSchemaCache.TryGetValue(runtimeMaterial, out glTFLoader.Schema.Material schemaMaterial))
            {
                return schemaMaterial;
            }
            schemaMaterial = CreateInstance<glTFLoader.Schema.Material>();

            if (runtimeMaterial.MetallicRoughnessMaterial != null)
            {
                schemaMaterial.PbrMetallicRoughness = CreateInstance<glTFLoader.Schema.MaterialPbrMetallicRoughness>();
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
                    var baseColorIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture);

                    schemaMaterial.PbrMetallicRoughness.BaseColorTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                    if (baseColorIndices.ImageIndex.HasValue)
                    {
                        schemaMaterial.PbrMetallicRoughness.BaseColorTexture.Index = baseColorIndices.ImageIndex.Value;
                    }
                    if (baseColorIndices.TextureCoordIndex.HasValue)
                    {
                        schemaMaterial.PbrMetallicRoughness.BaseColorTexture.TexCoord = baseColorIndices.TextureCoordIndex.Value;
                    }
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture != null)
                {
                    var metallicRoughnessIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture);

                    schemaMaterial.PbrMetallicRoughness.MetallicRoughnessTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                    if (metallicRoughnessIndices.ImageIndex.HasValue)
                    {
                        schemaMaterial.PbrMetallicRoughness.MetallicRoughnessTexture.Index = metallicRoughnessIndices.ImageIndex.Value;
                    }
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
                var normalIndices = AddTexture(runtimeMaterial.NormalTexture);
                schemaMaterial.NormalTexture = CreateInstance<glTFLoader.Schema.MaterialNormalTextureInfo>();

                if (normalIndices.ImageIndex.HasValue)
                {
                    schemaMaterial.NormalTexture.Index = normalIndices.ImageIndex.Value;

                }
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
                var occlusionIndices = AddTexture(runtimeMaterial.OcclusionTexture);
                schemaMaterial.OcclusionTexture = CreateInstance<glTFLoader.Schema.MaterialOcclusionTextureInfo>();
                if (occlusionIndices.ImageIndex.HasValue)
                {
                    schemaMaterial.OcclusionTexture.Index = occlusionIndices.ImageIndex.Value;
                }
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
                var emissiveIndices = AddTexture(runtimeMaterial.EmissiveTexture);
                schemaMaterial.EmissiveTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (emissiveIndices.ImageIndex.HasValue)
                {
                    schemaMaterial.EmissiveTexture.Index = emissiveIndices.ImageIndex.Value;
                }
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
                            throw new NotImplementedException("Extension schema conversion not implemented for " + runtimeExtension.Name);
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
        private glTFLoader.Schema.MaterialPbrSpecularGlossiness ConvertPbrSpecularGlossinessExtensionToSchema(Extensions.KHR_materials_pbrSpecularGlossiness specGloss, GLTF gltf)
        {
            var materialPbrSpecularGlossiness = CreateInstance<glTFLoader.Schema.MaterialPbrSpecularGlossiness>();

            if (specGloss.DiffuseFactor.HasValue)
            {
                materialPbrSpecularGlossiness.DiffuseFactor = specGloss.DiffuseFactor.Value.ToArray();
            }
            if (specGloss.DiffuseTexture != null)
            {
                TextureIndices textureIndices = AddTexture(specGloss.DiffuseTexture);
                materialPbrSpecularGlossiness.DiffuseTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.DiffuseTexture.Index = textureIndices.ImageIndex.Value;
                }
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
                TextureIndices textureIndices = AddTexture(specGloss.SpecularGlossinessTexture);
                materialPbrSpecularGlossiness.SpecularGlossinessTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.SpecularGlossinessTexture.Index = textureIndices.ImageIndex.Value;
                }
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
        /// Converts Runtime Quantum Rendering to Schema (Not an actual glTF feature) 
        /// </summary>
        private glTFLoader.Schema.FAKE_materials_quantumRendering ConvertExtQuantumRenderingToSchema(Extensions.FAKE_materials_quantumRendering quantumRendering, GLTF gltf)
        {
            var materialEXT_QuantumRendering = CreateInstance<glTFLoader.Schema.FAKE_materials_quantumRendering>();

            if (quantumRendering.PlanckFactor.HasValue)
            {
                materialEXT_QuantumRendering.PlanckFactor = quantumRendering.PlanckFactor.Value.ToArray();
            }
            if (quantumRendering.CopenhagenTexture != null)
            {
                TextureIndices textureIndices = AddTexture(quantumRendering.CopenhagenTexture);
                materialEXT_QuantumRendering.CopenhagenTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialEXT_QuantumRendering.CopenhagenTexture.Index = textureIndices.ImageIndex.Value;
                }
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
                TextureIndices textureIndices = AddTexture(quantumRendering.SuperpositionCollapseTexture);
                materialEXT_QuantumRendering.SuperpositionCollapseTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialEXT_QuantumRendering.SuperpositionCollapseTexture.Index = textureIndices.ImageIndex.Value;
                }
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
        /// Interleaves the primitive attributes to a single bufferview
        /// </summary>
        private Dictionary<string, int> InterleaveMeshPrimitiveAttributes(MeshPrimitive meshPrimitive, Data geometryData, int bufferIndex)
        {
            var attributes = new Dictionary<string, int>();
            var availableAttributes = new HashSet<AttributeEnum>();
            int vertexCount = 0;

            // create bufferview
            var bufferView = CreateBufferView(bufferIndex, "Interleaved attributes", 1, 0, null);
            bufferViews.Add(bufferView);
            int bufferviewIndex = bufferViews.Count() - 1;

            int byteOffset = 0;

            if (meshPrimitive.Positions != null && meshPrimitive.Positions.Any())
            {
                vertexCount = meshPrimitive.Positions.Count();
                //get the max and min values
                Vector3[] minMaxPositions = GetMinMaxPositions(meshPrimitive);
                var min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                var max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                var positionAccessor = CreateAccessor(bufferviewIndex, byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, meshPrimitive.Positions.Count(), "Position Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                accessors.Add(positionAccessor);
                attributes.Add("POSITION", accessors.Count() - 1);
                availableAttributes.Add(AttributeEnum.POSITION);
                byteOffset += sizeof(float) * 3;
            }
            if (meshPrimitive.Normals != null && meshPrimitive.Normals.Any())
            {
                var normalAccessor = CreateAccessor(bufferviewIndex, byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, meshPrimitive.Normals.Count(), "Normal Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                accessors.Add(normalAccessor);
                attributes.Add("NORMAL", accessors.Count() - 1);
                availableAttributes.Add(AttributeEnum.NORMAL);
                byteOffset += sizeof(float) * 3;
            }
            if (meshPrimitive.Tangents != null && meshPrimitive.Tangents.Any())
            {
                var tangentAccessor = CreateAccessor(bufferviewIndex, byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, meshPrimitive.Tangents.Count(), "Tangent Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                accessors.Add(tangentAccessor);
                attributes.Add("TANGENT", accessors.Count() - 1);
                availableAttributes.Add(AttributeEnum.TANGENT);
                byteOffset += sizeof(float) * 4;
            }
            if (meshPrimitive.TextureCoordSets != null && meshPrimitive.TextureCoordSets.Any())
            {
                int textureCoordSetIndex = 0;
                foreach(var textureCoordSet in meshPrimitive.TextureCoordSets)
                {
                    bool normalized = false;
                    glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                    int offset = 0;
                    switch (meshPrimitive.TextureCoordsComponentType)
                    {
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            offset = sizeof(float) * 2;
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                            normalized = true;
                            offset = sizeof(byte) * 2;
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            normalized = true;
                            offset = sizeof(ushort) * 2;
                            break;
                        default:
                            throw new NotImplementedException("Accessor component type " + meshPrimitive.TextureCoordsComponentType + " not supported!");
                    }
                    var textureCoordAccessor = CreateAccessor(bufferviewIndex, byteOffset, accessorComponentType, textureCoordSet.Count(), "Texture Coord " + textureCoordSetIndex, null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, normalized);
                    accessors.Add(textureCoordAccessor);
                    attributes.Add("TEXCOORD_" + textureCoordSetIndex, accessors.Count() - 1);
                    availableAttributes.Add(textureCoordSetIndex == 0 ? AttributeEnum.TEXCOORDS_0 : AttributeEnum.TEXCOORDS_1);
                    offset = GetPaddedSize(offset, 4);
                    byteOffset += offset;
                    ++textureCoordSetIndex;
                }
            }
            if (meshPrimitive.Colors != null && meshPrimitive.Colors.Any())
            {
                bool normalized = false;
                glTFLoader.Schema.Accessor.TypeEnum vectorType;
                int offset;
                if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3)
                {
                    offset = 3;
                    vectorType = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
                }
                else if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                {
                    offset = 4;
                    vectorType = glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                }
                else
                {
                    throw new NotImplementedException("Color of type " + meshPrimitive.ColorType + " not supported!");
                }
                glTFLoader.Schema.Accessor.ComponentTypeEnum colorAccessorComponentType;
                switch (meshPrimitive.ColorComponentType)
                {
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        offset *= sizeof(byte);
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        offset *= sizeof(ushort);
                        normalized = true;
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        offset *= sizeof(float);
                        break;
                    default:
                        throw new NotImplementedException("Color component type " + meshPrimitive.ColorComponentType + " not supported!");

                }
                int totalByteLength = (int)geometryData.Writer.BaseStream.Position;
                offset = GetPaddedSize(offset, 4);
                var colorAccessor = CreateAccessor(bufferviewIndex, byteOffset, colorAccessorComponentType, meshPrimitive.Colors.Count(), "Color Accessor", null, null, vectorType, normalized);
                accessors.Add(colorAccessor);
                attributes.Add("COLOR_0", accessors.Count() - 1);
                availableAttributes.Add(AttributeEnum.COLOR);
                byteOffset += offset;
            }
            bufferView.ByteStride = byteOffset;

            for (int i = 0; i < vertexCount; ++i)
            {
                foreach(var availableAttribute in availableAttributes)
                {
                    switch(availableAttribute)
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
                    int totalByteLength = (int)geometryData.Writer.BaseStream.Position;
                    Align(geometryData, totalByteLength, 4);
                }
            }
            bufferView.ByteLength = (int)geometryData.Writer.BaseStream.Position;

            return attributes;
        }

        private int WriteTextureCoords(MeshPrimitive meshPrimitive, IEnumerable<Vector2> textureCoordSet, int min, int max, Data geometryData)
        {
            int byteLength = 0;
            int offset = (int)geometryData.Writer.BaseStream.Position;

            int count = max - min + 1;
            Vector2[] tcs = textureCoordSet.ToArray();
            byteLength = 8 * count;
            switch (meshPrimitive.TextureCoordsComponentType)
            {
                case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                    for (int i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(tcs[i]);
                    }
                    break;
                case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                    for (int i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(tcs[i].X * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(tcs[i].Y * byte.MaxValue)));
                        Align(geometryData, 2, 4);
                    }
                    break;
                case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                    for (int i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(tcs[i].X * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(tcs[i].Y * ushort.MaxValue)));
                    }
                    break;
                default:
                    throw new NotImplementedException("Byte length calculation not implemented for TextureCoordsComponentType: " + meshPrimitive.TextureCoordsComponentType);
            }
            byteLength = (int)geometryData.Writer.BaseStream.Position - offset;

            return byteLength;
        }

        private int WriteColors(MeshPrimitive meshPrimitive, int min, int max, Data geometryData)
        {
            int byteLength = 0;
            int count = max - min + 1;
            int vectorSize = meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;
            byteLength = 0;

            switch (meshPrimitive.ColorComponentType)
            {
                case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                    for (int i = min; i <= max; ++i)
                    {
                        var color = meshPrimitive.Colors.ElementAt(i);
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(color.X * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(color.Y * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(color.Z * byte.MaxValue)));
                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(Convert.ToByte(Math.Round(color.W * byte.MaxValue)));
                        }
                        byteLength += Align(geometryData, vectorSize, 4);
                    }
                    break;
                case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                    for (int i = min; i <= max; ++i)
                    {
                        var color = meshPrimitive.Colors.ElementAt(i);
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.X * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.Y * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.Z * ushort.MaxValue)));

                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.W * ushort.MaxValue)));
                        }
                        byteLength += Align(geometryData, 2 * vectorSize, 4);
                    }
                    break;
                case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                    for (int i = min; i <= max; ++i)
                    {
                        var color = meshPrimitive.Colors.ElementAt(i);
                        geometryData.Writer.Write(color.X);
                        geometryData.Writer.Write(color.Y);
                        geometryData.Writer.Write(color.Z);

                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(color.W);
                        }
                        byteLength += Align(geometryData, 4 * vectorSize, 4);
                    }
                    break;
            }

            return byteLength;
        }

        /// <summary>
        /// Converts runtime animation to schema.
        /// </summary>
        private glTFLoader.Schema.Animation ConvertAnimationToSchema(Animation runtimeAnimation, glTFLoader.Schema.Buffer buffer, GLTF gltf, Data geometryData, int bufferIndex)
        {
            if (this.animationToSchemaCache.TryGetValue(runtimeAnimation, out glTFLoader.Schema.Animation schemaAnimation))
            {
                return schemaAnimation;
            }

            schemaAnimation = CreateInstance<glTFLoader.Schema.Animation>();
            var animationChannels = new List<glTFLoader.Schema.AnimationChannel>();
            var animationSamplers = new List<glTFLoader.Schema.AnimationSampler>();

            foreach (var runtimeAnimationChannel in runtimeAnimation.Channels)
            {
                var animationChannel = new glTFLoader.Schema.AnimationChannel();
                var targetNode = runtimeAnimationChannel.Target.Node;
                var sceneIndex = 0;
                if (gltf.Scene.HasValue)
                {
                    sceneIndex = gltf.Scene.Value;
                }

                var targetNodeIndex = this.ConvertNodeToSchema(targetNode, gltf, buffer, geometryData, bufferIndex);
              
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

            schemaAnimation.Channels = animationChannels.ToArray();
            schemaAnimation.Samplers = animationSamplers.ToArray();

            this.animationToSchemaCache.Add(runtimeAnimation, schemaAnimation);

            return schemaAnimation;
        }

        /// <summary>
        /// Converts runtime mesh primitive to schema.
        /// </summary>
        private glTFLoader.Schema.MeshPrimitive ConvertMeshPrimitiveToSchema(Node runtimeNode, MeshPrimitive runtimeMeshPrimitive, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            if (this.meshPrimitiveToSchemaCache.TryGetValue(runtimeMeshPrimitive, out glTFLoader.Schema.MeshPrimitive schemaMeshPrimitive))
            {
                return schemaMeshPrimitive;
            }
            schemaMeshPrimitive = CreateInstance<glTFLoader.Schema.MeshPrimitive>();
            var attributes = new Dictionary<string, int>();
            if (runtimeMeshPrimitive.Interleave != null && runtimeMeshPrimitive.Interleave == true)
            {
                attributes = InterleaveMeshPrimitiveAttributes(runtimeMeshPrimitive, geometryData, bufferIndex);
            }
            else
            {
                if (runtimeMeshPrimitive.Positions != null)
                {
                    //Create BufferView for the position
                    int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Positions.Count();
                    float[] min = new float[] { };
                    float[] max = new float[] { };

                    //get the max and min values
                    Vector3[] minMaxPositions = GetMinMaxPositions(runtimeMeshPrimitive);
                    min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                    max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                    var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                    accessors.Add(accessor);
                    geometryData.Writer.Write(runtimeMeshPrimitive.Positions.ToArray());
                    attributes.Add("POSITION", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.Normals != null)
                {
                    // Create BufferView
                    int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Normals.Count();
                    // Create a bufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);

                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                    accessors.Add(accessor);
                    geometryData.Writer.Write(runtimeMeshPrimitive.Normals.ToArray());
                    attributes.Add("NORMAL", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.Tangents != null && runtimeMeshPrimitive.Tangents.Any())
                {
                    // Create BufferView
                    int byteLength = sizeof(float) * 4 * runtimeMeshPrimitive.Tangents.Count();
                    // Create a bufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);


                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                    accessors.Add(accessor);
                    geometryData.Writer.Write(runtimeMeshPrimitive.Tangents.ToArray());
                    attributes.Add("TANGENT", accessors.Count() - 1);
                }
                if (runtimeMeshPrimitive.Colors != null)
                {
                    var colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                    var colorAccessorType = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? glTFLoader.Schema.Accessor.TypeEnum.VEC3 : glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                    int vectorSize = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;

                    // Create BufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                    int byteLength = WriteColors(runtimeMeshPrimitive, 0, runtimeMeshPrimitive.Colors.Count() - 1, geometryData);
                    int? byteStride = null;
                    switch (runtimeMeshPrimitive.ColorComponentType)
                    {
                        case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                            colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                            if (vectorSize == 3)
                            {
                                byteStride = 4;
                            }
                            break;
                        case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                            colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            if (vectorSize == 3)
                            {
                                byteStride = 8;
                            }
                            break;
                        default: //Default to ColorComponentTypeEnum.FLOAT:
                            colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            break;
                    }

                    var bufferView = CreateBufferView(bufferIndex, "Colors", byteLength, byteOffset, byteStride);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    // we normalize if the color accessor mode is not set to FLOAT
                    bool normalized = runtimeMeshPrimitive.ColorComponentType != MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    var accessor = CreateAccessor(bufferviewIndex, 0, colorAccessorComponentType, runtimeMeshPrimitive.Colors.Count(), "Colors Accessor", null, null, colorAccessorType, normalized);
                    accessors.Add(accessor);
                    attributes.Add("COLOR_0", accessors.Count() - 1);
                    if (normalized)
                    {
                        // Pad any additional bytes if byteLength is not a multiple of 4
                        Align(geometryData, byteLength, 4);
                    }
                }
                if (runtimeMeshPrimitive.TextureCoordSets != null)
                {
                    int i = 0;
                    foreach (var textureCoordSet in runtimeMeshPrimitive.TextureCoordSets)
                    {
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        int byteLength = WriteTextureCoords(runtimeMeshPrimitive, textureCoordSet, 0, runtimeMeshPrimitive.TextureCoordSets.ElementAt(i).Count() - 1, geometryData);

                        glTFLoader.Schema.Accessor accessor;
                        glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                        // we normalize only if the texture cood accessor type is not float
                        bool normalized = runtimeMeshPrimitive.TextureCoordsComponentType != MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                        int? byteStride = null;
                        switch (runtimeMeshPrimitive.TextureCoordsComponentType)
                        {
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                                break;
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                                byteStride = 4;
                                break;
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                                break;
                            default: // Default to Float
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                                break;
                        }

                        var bufferView = CreateBufferView(bufferIndex, "Texture Coords " + i, byteLength, byteOffset, byteStride);
                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;
                        // Create Accessor
                        accessor = CreateAccessor(bufferviewIndex, 0, accessorComponentType, textureCoordSet.Count(), "UV Accessor " + i, null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, normalized);

                        accessors.Add(accessor);

                        // Add any additional bytes if the data is normalized
                        if (normalized)
                        {
                            // Pad any additional bytes if byteLength is not a multiple of 4
                            Align(geometryData, byteLength, 4);
                        }
                        attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);
                        ++i;
                    }
                }

            }
            if (runtimeMeshPrimitive.Indices != null && runtimeMeshPrimitive.Indices.Any())
            {
                int byteLength;
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                glTFLoader.Schema.Accessor.ComponentTypeEnum indexComponentType;

                switch (runtimeMeshPrimitive.IndexComponentType)
                {
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE:
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        byteLength = sizeof(byte) * runtimeMeshPrimitive.Indices.Count();
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT:
                        byteLength = sizeof(ushort) * runtimeMeshPrimitive.Indices.Count();
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT:
                        byteLength = sizeof(uint) * runtimeMeshPrimitive.Indices.Count();
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT;
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unrecognized Index Component Type Enum " + runtimeMeshPrimitive.IndexComponentType);
                }
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Indices", byteLength, byteOffset, null);
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                var accessor = CreateAccessor(bufferviewIndex, 0, indexComponentType, runtimeMeshPrimitive.Indices.Count(), "Indices Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                accessors.Add(accessor);
                switch (indexComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToUInt32(index));
                        }
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToByte(index));
                        }
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(index));
                        }
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported Index Component Type");
                }

                schemaMeshPrimitive.Indices = accessors.Count() - 1;
            }

            var vertexJointWeights = runtimeMeshPrimitive.VertexJointWeights;
            if (vertexJointWeights != null && vertexJointWeights.Any())
            {
                var numJointWeights = vertexJointWeights.First().Count();
                if (!vertexJointWeights.All(jointWeights => jointWeights.Count() == numJointWeights))
                {
                    throw new ArgumentException("All vertex joint weights must have equal count");
                }

                glTFLoader.Schema.Accessor.ComponentTypeEnum weightAccessorComponentType;
                bool weightAccessorNormalized;
                Action<float> writeWeight;
                switch (runtimeMeshPrimitive.WeightComponentType)
                {
                    case MeshPrimitive.WeightComponentTypeEnum.FLOAT:
                    {
                        weightAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        weightAccessorNormalized = false;
                        writeWeight = value => geometryData.Writer.Write(value);
                        break;
                    }
                    case MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_BYTE:
                    {
                        weightAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        weightAccessorNormalized = true;
                        writeWeight = value => geometryData.Writer.Write(Convert.ToByte(Math.Round(value * byte.MaxValue)));
                        break;
                    }
                    case MeshPrimitive.WeightComponentTypeEnum.NORMALIZED_UNSIGNED_SHORT:
                    {
                        weightAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        weightAccessorNormalized = true;
                        writeWeight = value => geometryData.Writer.Write(Convert.ToUInt16(Math.Round(value * ushort.MaxValue)));
                        break;
                    }
                    default:
                    {
                        throw new InvalidEnumArgumentException();
                    }
                }

                glTFLoader.Schema.Accessor.ComponentTypeEnum jointAccessorComponentType;
                Action<int> writeJointIndex;
                switch (runtimeMeshPrimitive.JointComponentType)
                {
                    case MeshPrimitive.JointComponentTypeEnum.UNSIGNED_BYTE:
                        jointAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        writeJointIndex = jointIndex => geometryData.Writer.Write(Convert.ToByte(jointIndex));
                        break;
                    case MeshPrimitive.JointComponentTypeEnum.UNSIGNED_SHORT:
                        jointAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
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
                    var jointWeightsSet = vertexJointWeights
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

                        // Pad any additional bytes if byteLength is not a multiple of 4
                        Align(geometryData, byteLength, 4);

                        var accessorIndex = accessors.Count();
                        accessors.Add(CreateAccessor(bufferViewIndex, 0, weightAccessorComponentType, numVertices, "weights accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, weightAccessorNormalized));
                        attributes.Add($"WEIGHTS_{set}", accessorIndex);
                    }

                    // Joints
                    {
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                        foreach (var jointWeight in jointWeightsSet)
                        {
                            var jointIndex = 0;
                            if (jointWeight != null)
                            {
                                jointIndex = runtimeNode.Skin.SkinJoints.IndexOf(jointWeight.Joint);
                                if (jointIndex == -1)
                                {
                                    throw new Exception("joint cannot be found in skin joints!");
                                }
                            }

                            writeJointIndex(jointIndex);
                        }

                        var byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;

                        var bufferViewIndex = bufferViews.Count();
                        bufferViews.Add(CreateBufferView(bufferIndex, "joint indices buffer view", byteLength, byteOffset, null));

                        // Pad any additional bytes if byteLength is not a multiple of 4
                        Align(geometryData, byteLength, 4);

                        var accessorIndex = accessors.Count();
                        accessors.Add(CreateAccessor(bufferViewIndex, 0, jointAccessorComponentType, numVertices, "joint indices accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, false));
                        attributes.Add($"JOINTS_{set}", accessorIndex);
                    }
                }
            }

            schemaMeshPrimitive.Attributes = attributes;
            if (runtimeMeshPrimitive.Material != null)
            {
                var nMaterial = ConvertMaterialToSchema(runtimeMeshPrimitive.Material, gltf);
                materials.Add(nMaterial);
                schemaMeshPrimitive.Material = materials.Count() - 1;
            }

            switch (runtimeMeshPrimitive.Mode)
            {
                case MeshPrimitive.ModeEnum.TRIANGLES:
                    //glTF defaults to triangles
                    break;
                case MeshPrimitive.ModeEnum.POINTS:
                    schemaMeshPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.POINTS;
                    break;
                case MeshPrimitive.ModeEnum.LINES:
                    schemaMeshPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINES;
                    break;
                case MeshPrimitive.ModeEnum.LINE_LOOP:
                    schemaMeshPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_LOOP;
                    break;
                case MeshPrimitive.ModeEnum.LINE_STRIP:
                    schemaMeshPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_STRIP;
                    break;
                case MeshPrimitive.ModeEnum.TRIANGLE_FAN:
                    schemaMeshPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_FAN;
                    break;
                case MeshPrimitive.ModeEnum.TRIANGLE_STRIP:
                    schemaMeshPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_STRIP;
                    break;
            }

            this.meshPrimitiveToSchemaCache.Add(runtimeMeshPrimitive, schemaMeshPrimitive);

            return schemaMeshPrimitive;
        }

        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as an array of two vectors, minimum and maximum respectively</returns>
        private Vector3[] GetMinMaxPositions(MeshPrimitive meshPrimitive)
        {
            //get the max and min values
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
