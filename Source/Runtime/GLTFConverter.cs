using AssetGenerator.Runtime.ExtensionMethods;
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
                for (int index = 0; index < runtimeScene.Nodes.Count(); ++index)
                {
                    var nodeIndex = ConvertNodeToSchema(runtimeScene.Nodes[index], runtimeGLTF, buffer, geometryData, bufferIndex: 0);
                    sceneIndicesSet.Add(nodeIndex);
                }

                var scene = CreateInstance<glTFLoader.Schema.Scene>();
                scene.Nodes = sceneIndicesSet.ToArray();
                scenes.Add(scene);
            }
            if (scenes != null && scenes.Count > 0)
            {
                gltf.Scenes = scenes.ToArray();
                gltf.Scene = 0;
            }

            if (meshes != null && meshes.Count > 0)
            {
                gltf.Meshes = meshes.ToArray();
            }
            if (materials != null && materials.Count > 0)
            {
                gltf.Materials = materials.ToArray();
            }
            if (accessors != null && accessors.Count > 0)
            {
                gltf.Accessors = accessors.ToArray();
            }
            if (bufferViews != null && bufferViews.Count > 0)
            {
                gltf.BufferViews = bufferViews.ToArray();
            }

            gltf.Buffers = new[] { buffer };
            if (nodes != null && nodes.Count > 0)
            {
                gltf.Nodes = nodes.ToArray();
            }

            if (images.Count > 0)
            {
                gltf.Images = images.ToArray();

            }
            if (textures.Count > 0)
            {
                gltf.Textures = textures.ToArray();
            }
            if (samplers.Count > 0)
            {
                gltf.Samplers = samplers.ToArray();
            }
            if (runtimeGLTF.MainScene.HasValue)
            {
                gltf.Scene = runtimeGLTF.MainScene.Value;
            }
            if (runtimeGLTF.ExtensionsUsed != null)
            {
                gltf.ExtensionsUsed = runtimeGLTF.ExtensionsUsed.ToArray();
            }
            if (runtimeGLTF.ExtensionsRequired != null)
            {
                gltf.ExtensionsRequired = runtimeGLTF.ExtensionsRequired.ToArray();
            }

            return gltf;
        }

        private T CreateInstance<T>()
        {
            return (T)this.CreateInstanceOverride(typeof(T));
        }

        /// <summary>
        /// converts the Runtime image to a glTF Image
        /// </summary>
        /// <returns>Returns a gltf Image object</returns>
        private glTFLoader.Schema.Image ConvertImageToSchema(Image runtimeImage)
        {
            var image = CreateInstance<glTFLoader.Schema.Image>();

            image.Uri = runtimeImage.Uri;

            if (runtimeImage.MimeType.HasValue)
            {
                image.MimeType = runtimeImage.MimeType.Value;
            }

            if (runtimeImage.Name != null)
            {
                image.Name = runtimeImage.Name;
            }

            return image;
        }

        private glTFLoader.Schema.Sampler ConvertSamplerToSchema(Sampler runtimeSampler)
        {
            var sampler = CreateInstance<glTFLoader.Schema.Sampler>();

            if (runtimeSampler.MagFilter.HasValue)
            {
                sampler.MagFilter = runtimeSampler.MagFilter.Value;
            }

            if (runtimeSampler.MinFilter.HasValue)
            {
                sampler.MinFilter = runtimeSampler.MinFilter.Value;
            }

            if (runtimeSampler.WrapS.HasValue)
            {
                sampler.WrapS = runtimeSampler.WrapS.Value;
            }

            if (runtimeSampler.WrapT.HasValue)
            {
                sampler.WrapT = runtimeSampler.WrapT.Value;
            }

            if (runtimeSampler.Name != null)
            {
                sampler.Name = runtimeSampler.Name;
            }

            return sampler;
        }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        private TextureIndices AddTexture(Texture runtimeTexture)
        {
            var indices = new List<int>();
            int? samplerIndex = null;
            int? imageIndex = null;
            int? textureCoordIndex = null;

            if (runtimeTexture != null)
            {
                if (runtimeTexture.Sampler != null)
                {
                    // If a similar sampler is already being used in the list, reuse that index instead of creating a new sampler object
                    if (samplers.Count > 0)
                    {
                        int findIndex = -1;
                        for (int i = 0; i < samplers.Count(); ++i)
                        {
                            if (samplers[i].SamplersEqual(ConvertSamplerToSchema(runtimeTexture.Sampler)))
                            {
                                findIndex = i;
                                break;
                            }
                        }
                    }
                    if (!samplerIndex.HasValue)
                    {
                        var sampler = ConvertSamplerToSchema(runtimeTexture.Sampler);
                        samplers.Add(sampler);
                        samplerIndex = samplers.Count() - 1;
                    }
                }
                if (runtimeTexture.Source != null)
                {
                    // If an equivalent image object has already been created, reuse its index instead of creating a new image object
                    var image = ConvertImageToSchema(runtimeTexture.Source);
                    int findImageIndex = -1;

                    if (images.Count() > 0)
                    {
                        for (int i = 0; i < images.Count(); ++i)
                        {
                            if (images[i].ImagesEqual(image))
                            {
                                findImageIndex = i;
                                break;
                            }
                        }
                    }

                    if (findImageIndex != -1)
                    {
                        imageIndex = findImageIndex;
                    }

                    if (!imageIndex.HasValue)
                    {
                        images.Add(image);
                        imageIndex = images.Count() - 1;
                    }
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

            TextureIndices textureIndices = new TextureIndices
            {
                SamplerIndex = samplerIndex,
                ImageIndex = imageIndex,
                TextureCoordIndex = textureCoordIndex
            };

            return textureIndices;
        }

        /// <summary>
        /// Pads a value to ensure it is a multiple of size
        /// </summary>
        private int Align(Data geometryData, int value, int size)
        {
            var remainder = value % size;
            var paddedValue = (remainder == 0 ? value : checked(value + size - remainder));

            int additionalPaddedBytes = paddedValue - value;
            Enumerable.Range(0, additionalPaddedBytes).ForEach(arg => geometryData.Writer.Write((byte)0));
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

            if (min != null && min.Count() > 0)
            {
                accessor.Min = min;
            }

            if (max != null && max.Count() > 0)
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
            var node = CreateInstance<glTFLoader.Schema.Node>();
            nodes.Add(node);
            int nodeIndex = nodes.Count() - 1;
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
                var schemaMesh = ConvertMeshToSchema(runtimeNode.Mesh, gltf, buffer, geometryData, bufferIndex);
                // If an equivalent mesh has already been created, re-use that mesh's index instead of creating a new material
                node.Mesh = AddInstanceOfObject(meshes, schemaMesh);
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

            return nodeIndex;
        }

        /// <summary>
        /// Converts the morph target list of dictionaries into Morph Target
        /// </summary>
        private List<Dictionary<string, int>> GetMeshPrimitiveMorphTargets(MeshPrimitive meshPrimitive, List<float> weights, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var morphTargetDicts = new List<Dictionary<string, int>>();
            if (meshPrimitive.MorphTargets != null)
            {
                foreach (MeshPrimitive morphTarget in meshPrimitive.MorphTargets)
                {
                    var morphTargetAttributes = new Dictionary<string, int>();

                    if (morphTarget.Positions != null && morphTarget.Positions.Count > 0)
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
                    if (morphTarget.Normals != null && morphTarget.Normals.Count > 0)
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
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Count > 0)
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Tangents.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        buffer.ByteLength += (int)geometryData.Writer.BaseStream.Position;
                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Tangents.ToArray());
                        morphTargetAttributes.Add("TANGENT", accessors.Count() - 1);
                    }
                    morphTargetDicts.Add(new Dictionary<string, int>(morphTargetAttributes));
                    weights.Add(meshPrimitive.morphTargetWeight);
                }
            }
            return morphTargetDicts;
        }

        /// <summary>
        /// Converts runtime mesh to schema.
        /// </summary>
        private glTFLoader.Schema.Mesh ConvertMeshToSchema(Mesh runtimeMesh, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var schemaMesh = CreateInstance<glTFLoader.Schema.Mesh>();
            var primitives = new List<glTFLoader.Schema.MeshPrimitive>(runtimeMesh.MeshPrimitives.Count);
            var weights = new List<float>();
            // Loops through each wrapped mesh primitive within the mesh and converts them to mesh primitives, as well as updating the
            // indices in the lists
            foreach (var gPrimitive in runtimeMesh.MeshPrimitives)
            {
                glTFLoader.Schema.MeshPrimitive mPrimitive = ConvertMeshPrimitiveToSchema(gPrimitive, gltf, buffer, geometryData, bufferIndex);
                if (gPrimitive.MorphTargets != null && gPrimitive.MorphTargets.Count() > 0)
                {
                    List<Dictionary<string, int>> morphTargetAttributes = GetMeshPrimitiveMorphTargets(gPrimitive, weights, buffer, geometryData, bufferIndex);
                    //List<Dictionary<string, int>> morphTargetAttributes = gPrimitive.GetMorphTargets(bufferViews, accessors, ref buffer, geometryData, ref weights, bufferIndex);
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

            return schemaMesh;
        }

        /// <summary>
        /// Converts runtime material to schema.
        /// </summary>
        private glTFLoader.Schema.Material ConvertMaterialToSchema(Material runtimeMaterial, GLTF gltf)
        {
            var material = CreateInstance<glTFLoader.Schema.Material>();

            if (runtimeMaterial.MetallicRoughnessMaterial != null)
            {
                material.PbrMetallicRoughness = CreateInstance<glTFLoader.Schema.MaterialPbrMetallicRoughness>();
                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.HasValue)
                {
                    material.PbrMetallicRoughness.BaseColorFactor = new[]
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

                    material.PbrMetallicRoughness.BaseColorTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                    if (baseColorIndices.ImageIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture.Index = baseColorIndices.ImageIndex.Value;
                    }
                    if (baseColorIndices.TextureCoordIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture.TexCoord = baseColorIndices.TextureCoordIndex.Value;
                    };
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture != null)
                {
                    var metallicRoughnessIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture);

                    material.PbrMetallicRoughness.MetallicRoughnessTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                    if (metallicRoughnessIndices.ImageIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture.Index = metallicRoughnessIndices.ImageIndex.Value;
                    }
                    if (metallicRoughnessIndices.TextureCoordIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture.TexCoord = metallicRoughnessIndices.TextureCoordIndex.Value;
                    }
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicFactor.HasValue)
                {
                    material.PbrMetallicRoughness.MetallicFactor = runtimeMaterial.MetallicRoughnessMaterial.MetallicFactor.Value;
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.RoughnessFactor.HasValue)
                {
                    material.PbrMetallicRoughness.RoughnessFactor = runtimeMaterial.MetallicRoughnessMaterial.RoughnessFactor.Value;
                }
            }
            if (runtimeMaterial.EmissiveFactor != null)
            {
                material.EmissiveFactor = new[]
                {
                    runtimeMaterial.EmissiveFactor.Value.X,
                    runtimeMaterial.EmissiveFactor.Value.Y,
                    runtimeMaterial.EmissiveFactor.Value.Z
                };
            }
            if (runtimeMaterial.NormalTexture != null)
            {
                var normalIndicies = AddTexture(runtimeMaterial.NormalTexture);
                material.NormalTexture = CreateInstance<glTFLoader.Schema.MaterialNormalTextureInfo>();

                if (normalIndicies.ImageIndex.HasValue)
                {
                    material.NormalTexture.Index = normalIndicies.ImageIndex.Value;

                }
                if (normalIndicies.TextureCoordIndex.HasValue)
                {
                    material.NormalTexture.TexCoord = normalIndicies.TextureCoordIndex.Value;
                }
                if (runtimeMaterial.NormalScale.HasValue)
                {
                    material.NormalTexture.Scale = runtimeMaterial.NormalScale.Value;
                }
            }
            if (runtimeMaterial.OcclusionTexture != null)
            {
                var occlusionIndicies = AddTexture(runtimeMaterial.OcclusionTexture);
                material.OcclusionTexture = CreateInstance<glTFLoader.Schema.MaterialOcclusionTextureInfo>();
                if (occlusionIndicies.ImageIndex.HasValue)
                {
                    material.OcclusionTexture.Index = occlusionIndicies.ImageIndex.Value;
                };
                if (occlusionIndicies.TextureCoordIndex.HasValue)
                {
                    material.OcclusionTexture.TexCoord = occlusionIndicies.TextureCoordIndex.Value;
                }
                if (runtimeMaterial.OcclusionStrength.HasValue)
                {
                    material.OcclusionTexture.Strength = runtimeMaterial.OcclusionStrength.Value;
                }
            }
            if (runtimeMaterial.EmissiveTexture != null)
            {
                var emissiveIndicies = AddTexture(runtimeMaterial.EmissiveTexture);
                material.EmissiveTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (emissiveIndicies.ImageIndex.HasValue)
                {
                    material.EmissiveTexture.Index = emissiveIndicies.ImageIndex.Value;
                }
                if (emissiveIndicies.TextureCoordIndex.HasValue)
                {
                    material.EmissiveTexture.TexCoord = emissiveIndicies.TextureCoordIndex.Value;
                }
            }
            if (runtimeMaterial.AlphaMode.HasValue)
            {
                material.AlphaMode = runtimeMaterial.AlphaMode.Value;
            }
            if (runtimeMaterial.AlphaCutoff.HasValue)
            {
                material.AlphaCutoff = runtimeMaterial.AlphaCutoff.Value;
            }
            if (runtimeMaterial.Name != null)
            {
                material.Name = runtimeMaterial.Name;
            }
            if (runtimeMaterial.DoubleSided.HasValue)
            {
                material.DoubleSided = runtimeMaterial.DoubleSided.Value;
            }
            if (runtimeMaterial.Extensions != null)
            {
                if (material.Extensions == null)
                {
                    material.Extensions = new Dictionary<string, object>();
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

                    material.Extensions.Add(runtimeExtension.Name, extension);

                    if (!gltf.ExtensionsUsed.Contains(runtimeExtension.Name))
                    {
                        gltf.ExtensionsUsed.Add(runtimeExtension.Name);
                    }
                }
            }

            return material;
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
            int byteOffset = (int)geometryData.Writer.BaseStream.Position;
            if (meshPrimitive.Positions != null)
            {
                int count = meshPrimitive.Positions.Count();

                int totalByteLength = 0;

                // create bufferview
                var bufferView = CreateBufferView(bufferIndex, "Interleaved attributes", 1, 0, null);
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                for (int i = 0; i < count; ++i)
                {
                    if (i == 0)
                    {
                        //get the max and min values
                        Vector3[] minMaxPositions = GetMinMaxPositions(meshPrimitive);
                        var min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                        var max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                        var positionAccessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, count, "Position Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                        accessors.Add(positionAccessor);
                        attributes.Add("POSITION", accessors.Count() - 1);
                    }
                    totalByteLength = Align(geometryData, totalByteLength, 4);
                    geometryData.Writer.Write(meshPrimitive.Positions[i]);
                    totalByteLength += sizeof(float) * 3;
                    if (meshPrimitive.Normals != null)
                    {
                        if (i == 0)
                        {
                            int normalOffset = totalByteLength;
                            var normalAccessor = CreateAccessor(bufferviewIndex, normalOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, count, "Normal Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                            accessors.Add(normalAccessor);
                            attributes.Add("NORMAL", accessors.Count() - 1);
                        }
                        geometryData.Writer.Write(meshPrimitive.Normals[i]);
                        totalByteLength += sizeof(float) * 3;
                    }
                    if (meshPrimitive.Tangents != null)
                    {
                        if (i == 0)
                        {
                            int tangentOffset = totalByteLength;
                            var tangentAccessor = CreateAccessor(bufferviewIndex, tangentOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, count, "Tangent Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                            accessors.Add(tangentAccessor);
                            attributes.Add("TANGENT", accessors.Count() - 1);
                        }
                        geometryData.Writer.Write(meshPrimitive.Tangents[i]);
                        totalByteLength += sizeof(float) * 4;
                    }
                    if (meshPrimitive.TextureCoordSets != null)
                    {
                        bool normalized = false;
                        int[] textureCoordOffset = new int[meshPrimitive.TextureCoordSets.Count()];
                        for (int j = 0; j < meshPrimitive.TextureCoordSets.Count(); ++j)
                        {

                            // if not multiple of 4, add padding
                            totalByteLength = Align(geometryData, totalByteLength, 4);
                            textureCoordOffset[j] = totalByteLength;
                            if (i == 0)
                            {
                                glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;

                                switch (meshPrimitive.TextureCoordsComponentType)
                                {
                                    case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                                        accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                                        break;
                                    case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                                        accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                                        normalized = true;
                                        break;
                                    case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                                        accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                                        normalized = true;
                                        break;
                                    default:
                                        throw new NotImplementedException("Accessor component type " + meshPrimitive.TextureCoordsComponentType + " not supported!");
                                }
                                var textureCoordAccessor = CreateAccessor(bufferviewIndex, textureCoordOffset[j], accessorComponentType, count, "Texture Coord " + j, null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, normalized);
                                accessors.Add(textureCoordAccessor);
                                attributes.Add("TEXCOORD_" + j, accessors.Count() - 1);
                            }
                            totalByteLength += WriteTextureCoords(meshPrimitive, meshPrimitive.TextureCoordSets[j], i, i, geometryData);
                        }
                    }
                    if (meshPrimitive.Colors != null)
                    {
                        // if not multiple of 4, add padding
                        totalByteLength = Align(geometryData, totalByteLength, 4);
                        int colorOffset = totalByteLength;
                        if (i == 0)
                        {
                            bool normalized = false;
                            glTFLoader.Schema.Accessor.TypeEnum vectorType;
                            if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3)
                            {
                                vectorType = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
                            }
                            else if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                            {
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
                                    normalized = true;
                                    break;
                                case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                                    colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                                    normalized = true;
                                    break;
                                case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                                    colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                                    break;
                                default:
                                    throw new NotImplementedException("Color component type " + meshPrimitive.ColorComponentType + " not supported!");

                            }
                            var colorAccessor = CreateAccessor(bufferviewIndex, colorOffset, colorAccessorComponentType, count, "Color Accessor", null, null, vectorType, normalized);
                            accessors.Add(colorAccessor);
                            attributes.Add("COLOR_0", accessors.Count() - 1);
                        }
                        totalByteLength += WriteColors(meshPrimitive, i, i, geometryData);
                    }
                    // if not multiple of 4, add padding
                    totalByteLength = Align(geometryData, totalByteLength, 4);

                    if (i == 0)
                    {
                        bufferView.ByteStride = totalByteLength;
                    }
                }
                bufferView.ByteLength = totalByteLength;
            }

            return attributes;
        }

        private int WriteTextureCoords(MeshPrimitive meshPrimitive, List<Vector2> textureCoordSet, int min, int max, Data geometryData)
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
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(meshPrimitive.Colors[i].X * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(meshPrimitive.Colors[i].Y * byte.MaxValue)));
                        geometryData.Writer.Write(Convert.ToByte(Math.Round(meshPrimitive.Colors[i].Z * byte.MaxValue)));
                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(Convert.ToByte(Math.Round(meshPrimitive.Colors[i].W * byte.MaxValue)));
                        }
                        byteLength += Align(geometryData, vectorSize, 4);
                    }
                    break;
                case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                    for (int i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(meshPrimitive.Colors[i].X * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(meshPrimitive.Colors[i].Y * ushort.MaxValue)));
                        geometryData.Writer.Write(Convert.ToUInt16(Math.Round(meshPrimitive.Colors[i].Z * ushort.MaxValue)));

                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(Math.Round(meshPrimitive.Colors[i].W * ushort.MaxValue)));
                        }
                        byteLength += Align(geometryData, 2 * vectorSize, 4);
                    }
                    break;
                case MeshPrimitive.ColorComponentTypeEnum.FLOAT:
                    for (int i = min; i <= max; ++i)
                    {
                        geometryData.Writer.Write(meshPrimitive.Colors[i].X);
                        geometryData.Writer.Write(meshPrimitive.Colors[i].Y);
                        geometryData.Writer.Write(meshPrimitive.Colors[i].Z);

                        if (meshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC4)
                        {
                            geometryData.Writer.Write(meshPrimitive.Colors[i].W);
                        }
                        byteLength += Align(geometryData, 4 * vectorSize, 4);
                    }
                    break;
            }

            return byteLength;
        }

        /// <summary>
        /// Converts runtime mesh primitive to schema.
        /// </summary>
        private glTFLoader.Schema.MeshPrimitive ConvertMeshPrimitiveToSchema(MeshPrimitive runtimeMeshPrimitive, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var mPrimitive = CreateInstance<glTFLoader.Schema.MeshPrimitive>();
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
                if (runtimeMeshPrimitive.Tangents != null && runtimeMeshPrimitive.Tangents.Count > 0)
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
                    for (int i = 0; i < runtimeMeshPrimitive.TextureCoordSets.Count; ++i)
                    {
                        List<Vector2> textureCoordSet = runtimeMeshPrimitive.TextureCoordSets[i];
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        int byteLength = WriteTextureCoords(runtimeMeshPrimitive, textureCoordSet, 0, runtimeMeshPrimitive.TextureCoordSets[i].Count() - 1, geometryData);

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
                    }
                }

            }
            if (runtimeMeshPrimitive.Indices != null && runtimeMeshPrimitive.Indices.Count() > 0)
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

                mPrimitive.Indices = accessors.Count() - 1;
            }

            mPrimitive.Attributes = attributes;
            if (runtimeMeshPrimitive.Material != null)
            {
                var nMaterial = ConvertMaterialToSchema(runtimeMeshPrimitive.Material, gltf);

                // If an equivalent material has already been created, re-use that material's index instead of creating a new material
                mPrimitive.Material = AddInstanceOfObject(materials, nMaterial);
            }
            int totalByteLength = (int)geometryData.Writer.BaseStream.Position;
            if (totalByteLength > 0)
            {
                buffer.ByteLength = totalByteLength;
            }
            if (runtimeMeshPrimitive.Mode.HasValue)
            {
                switch (runtimeMeshPrimitive.Mode)
                {
                    case MeshPrimitive.ModeEnum.POINTS:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.POINTS;
                        break;
                    case MeshPrimitive.ModeEnum.LINES:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINES;
                        break;
                    case MeshPrimitive.ModeEnum.LINE_LOOP:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_LOOP;
                        break;
                    case MeshPrimitive.ModeEnum.LINE_STRIP:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_STRIP;
                        break;
                    case MeshPrimitive.ModeEnum.TRIANGLES:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLES;
                        break;
                    case MeshPrimitive.ModeEnum.TRIANGLE_FAN:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_FAN;
                        break;
                    case MeshPrimitive.ModeEnum.TRIANGLE_STRIP:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_STRIP;
                        break;
                }
            }

            return mPrimitive;
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

        /// <summary>
        /// Takes an object and adds it to a list if there isn't already an equivalent in that list. The index of either the newly added item or the existing one is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="existingInstances">List of instances that the object will be added to, if it doesn't already exist</param>
        /// <param name="instance">Object that is being checked for uniqueness</param>
        /// <returns>Index of the item in the list</returns>
        private int AddInstanceOfObject<T>(List<T> existingInstances, T instance)
        {
            int instanceIndex = -1;
            int indexUsed;

            for (int i = 0; i < existingInstances.Count(); ++i)
            {
                if (existingInstances[i].ObjectsEqual(instance))
                {
                    instanceIndex = i;
                    break;
                }
            }

            if (instanceIndex > -1)
            {
                indexUsed = instanceIndex;
            }
            else
            {
                existingInstances.Add(instance);
                indexUsed = existingInstances.Count() - 1;
            }

            return indexUsed;
        }
    }
}
