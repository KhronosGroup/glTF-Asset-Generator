using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetGenerator.Runtime.ExtensionMethods;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Convert Runtime Abstraction to Schema.
    /// </summary>
    internal class GLTFConverter
    {
        private static List<glTFLoader.Schema.Buffer> Buffers { get; set; } 
        private static List<glTFLoader.Schema.BufferView> BufferViews  { get; set; } 
        private static List<glTFLoader.Schema.Accessor> Accessors { get; set; } 
        private static List<glTFLoader.Schema.Material> Materials { get; set; } 
        private static List<glTFLoader.Schema.Node> Nodes { get; set; } 
        private static List<glTFLoader.Schema.Scene> Scenes { get; set; }
        private static List<glTFLoader.Schema.Image> Images { get; set; } 
        private static List<glTFLoader.Schema.Sampler> Samplers { get; set; } 
        private static List<glTFLoader.Schema.Texture> Textures { get; set; } 
        private static List<glTFLoader.Schema.Mesh> Meshes { get; set; }

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
        /// Clears the static properties.
        /// </summary>
        private static void initBuffers()
        {
            Buffers = new List<glTFLoader.Schema.Buffer>();
            BufferViews = new List<glTFLoader.Schema.BufferView>();
            Accessors = new List<glTFLoader.Schema.Accessor>();
            Materials = new List<glTFLoader.Schema.Material>();
            Nodes = new List<glTFLoader.Schema.Node>();
            Scenes = new List<glTFLoader.Schema.Scene>();
            Images = new List<glTFLoader.Schema.Image>();
            Samplers = new List<glTFLoader.Schema.Sampler>();
            Textures = new List<glTFLoader.Schema.Texture>();
            Meshes = new List<glTFLoader.Schema.Mesh>();
    }

        /// <summary>
        /// Converts Runtime GLTF to Schema GLTF object.
        /// </summary>
        /// <param name="runtimeGLTF"></param>
        /// <param name="gltf"></param>
        /// <param name="geometryData"></param>
        public static void ConvertRuntimeToSchema(Runtime.GLTF runtimeGLTF, ref glTFLoader.Schema.Gltf gltf, Data geometryData)
        {
            initBuffers();
            if (runtimeGLTF.Asset != null)
            {
                gltf.Asset = ConvertAssetToSchema(runtimeGLTF.Asset);
            }

            glTFLoader.Schema.Buffer gBuffer = new glTFLoader.Schema.Buffer
            {
                Uri = geometryData.Name,
            };
            int bufferIndex = 0;


            // for each scene, create a node for each mesh and compute the indices for the scene object
            foreach (var gscene in runtimeGLTF.Scenes)
            {
                var sceneIndicesSet = new List<int>();
                // loops through each mesh and converts it into a Node, with optional transformation info if available
                for (int index = 0; index < gscene.Nodes.Count(); ++index)
                {
                    var nodeIndex = ConvertNodeToSchema(gscene.Nodes[index], runtimeGLTF, gBuffer, geometryData, bufferIndex);
                    sceneIndicesSet.Add(nodeIndex);
                }

                Scenes.Add(new glTFLoader.Schema.Scene
                {
                    Nodes = sceneIndicesSet.ToArray()
                });
            }
            if (Scenes != null && Scenes.Count > 0)
            {
                gltf.Scenes = Scenes.ToArray();
                gltf.Scene = 0;
            }

            if (Meshes != null && Meshes.Count > 0)
            {
                gltf.Meshes = Meshes.ToArray();
            }
            if (Materials != null && Materials.Count > 0)
            {
                gltf.Materials = Materials.ToArray();
            }
            if (Accessors != null && Accessors.Count > 0)
            {
                gltf.Accessors = Accessors.ToArray();
            }
            if (BufferViews != null && BufferViews.Count > 0)
            {
                gltf.BufferViews = BufferViews.ToArray();
            }

            gltf.Buffers = new[] { gBuffer };
            if (Nodes != null && Nodes.Count > 0)
            {
                gltf.Nodes = Nodes.ToArray();
            }

            if (Images.Count > 0)
            {
                gltf.Images = Images.ToArray();

            }
            if (Textures.Count > 0)
            {
                gltf.Textures = Textures.ToArray();
            }
            if (Samplers.Count > 0)
            {
                gltf.Samplers = Samplers.ToArray();
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
        }

        /// <summary>
        /// converts the Runtime image to a glTF Image
        /// </summary>
        /// <returns>Returns a gltf Image object</returns>
        private static glTFLoader.Schema.Image ConvertImageToSchema(Runtime.Image runtimeImage)
        {
            glTFLoader.Schema.Image image = new glTFLoader.Schema.Image
            {
                Uri = runtimeImage.Uri
            };
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

        private static glTFLoader.Schema.Sampler ConvertSamplerToSchema(Runtime.Sampler runtimeSampler)
        {
            glTFLoader.Schema.Sampler schemaSampler = new glTFLoader.Schema.Sampler();
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
            return schemaSampler;

        }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="gTexture"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="material"></param>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        private static TextureIndices AddTexture(Runtime.Texture gTexture)
        {
            var indices = new List<int>();
            int? samplerIndex = null;
            int? imageIndex = null;
            int? textureCoordIndex = null;

            if (gTexture != null)
            {
                if (gTexture.Sampler != null)
                {
                    // If a similar sampler is already being used in the list, reuse that index instead of creating a new sampler object
                    if (Samplers.Count > 0)
                    {
                        int findIndex = -1;
                        for (int i = 0; i < Samplers.Count(); ++i)
                        {
                            if (Samplers[i].SamplersEqual(ConvertSamplerToSchema(gTexture.Sampler)))
                            {
                                findIndex = i;
                                break;
                            }
                        }
                    }
                    if (!samplerIndex.HasValue)
                    {
                        var sampler = ConvertSamplerToSchema(gTexture.Sampler);
                        Samplers.Add(sampler);
                        samplerIndex = Samplers.Count() - 1;
                    }
                }
                if (gTexture.Source != null)
                {
                    // If an equivalent image object has already been created, reuse its index instead of creating a new image object
                    var image = ConvertImageToSchema(gTexture.Source);
                    int findImageIndex = -1;

                    if (Images.Count() > 0)
                    {
                        for (int i = 0; i < Images.Count(); ++i)
                        {
                            if (Images[i].ImagesEqual(image))
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
                        Images.Add(image);
                        imageIndex = Images.Count() - 1;
                    }
                }
                var texture = new glTFLoader.Schema.Texture();
                if (samplerIndex.HasValue)
                {
                    texture.Sampler = samplerIndex.Value;
                }
                if (imageIndex.HasValue)
                {
                    texture.Source = imageIndex.Value;
                }
                if (gTexture.Name != null)
                {
                    texture.Name = gTexture.Name;
                }
                // If an equivalent texture has already been created, re-use that texture's index instead of creating a new texture
                int findTextureIndex = -1;
                if (Textures.Count > 0)
                {
                    for (int i = 0; i < Textures.Count(); ++i)
                    {
                        if (Textures[i].TexturesEqual(texture))
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
                    Textures.Add(texture);
                    indices.Add(Textures.Count() - 1);
                }

                if (gTexture.TexCoordIndex.HasValue)
                {
                    indices.Add(gTexture.TexCoordIndex.Value);
                    textureCoordIndex = gTexture.TexCoordIndex.Value;
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
        /// Pads the value of the values to ensure it is a multiple of size
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static int Align(int value, int size)
        {
            var remainder = value % size;
            return (remainder == 0 ? value : checked(value + size - remainder));
        }

        /// <summary>
        /// Creates a bufferview schema object
        /// </summary>
        /// <param name="bufferIndex"></param>
        /// <param name="name"></param>
        /// <param name="byteLength"></param>
        /// <param name="byteOffset"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.BufferView CreateBufferView(int bufferIndex, string name, int byteLength, int byteOffset)
        {
            var bufferView = new glTFLoader.Schema.BufferView
            {
                Name = name,
                ByteLength = byteLength,
                ByteOffset = byteOffset,
                Buffer = bufferIndex
            };
            return bufferView;
        }

        /// <summary>
        /// Creates an accessor schema object
        /// </summary>
        /// <param name="bufferviewIndex"></param>
        /// <param name="byteOffset"></param>
        /// <param name="componentType"></param>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="type"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.Accessor CreateAccessor(int bufferviewIndex, int? byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, glTFLoader.Schema.Accessor.TypeEnum? type, bool? normalized)
        {
            var accessor = new glTFLoader.Schema.Accessor
            {
                BufferView = bufferviewIndex,
                Name = name,
            };
            if (min != null && min.Count() > 0)
            {
                accessor.Min = min;
            };
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
            if (normalized.HasValue)
            {
                accessor.Normalized = normalized.Value;
            }
            return accessor;
        }

        /// <summary>
        /// Converts runtime asset to Schema.
        /// </summary>
        /// <param name="runtimeAsset"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.Asset ConvertAssetToSchema(Runtime.Asset runtimeAsset)
        {
            var extras = new glTFLoader.Schema.Extras();
            var schemaAsset = new glTFLoader.Schema.Asset();

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
        /// <param name="runtimeNode"></param>
        /// <param name="gltf"></param>
        /// <param name="buffer"></param>
        /// <param name="geometryData"></param>
        /// <param name="bufferIndex"></param>
        /// <returns></returns>
        private static int ConvertNodeToSchema(Runtime.Node runtimeNode, Runtime.GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var node = new glTFLoader.Schema.Node();
            Nodes.Add(node);
            int nodeIndex = Nodes.Count() - 1;
            if (runtimeNode.Name != null)
            {
                node.Name = runtimeNode.Name;
            }
            if (runtimeNode.Matrix != null)
            {
                node.Matrix = runtimeNode.Matrix.ToArray();
            }
            if (runtimeNode.Mesh != null)
            {
                var schemaMesh = ConvertMeshToSchema(runtimeNode.Mesh, gltf, buffer, geometryData, bufferIndex);
                Meshes.Add(schemaMesh);
                node.Mesh = Meshes.Count() - 1;
            }
            if (runtimeNode.Rotation != null)
            {
                node.Rotation = runtimeNode.Rotation.ToArray();
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
        /// <param name="meshPrimitive"></param>
        /// <param name="weights"></param>
        /// <param name="buffer"></param>
        /// <param name="geometryData"></param>
        /// <param name="bufferIndex"></param>
        /// <returns></returns>
        private static List<Dictionary<string, int>> GetMeshPrimitiveMorphTargets(Runtime.MeshPrimitive meshPrimitive, List<float> weights, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
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
                            var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset);

                            BufferViews.Add(bufferView);
                            int bufferviewIndex = BufferViews.Count() - 1;

                            // Create an accessor for the bufferView
                            var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Positions.Count(), "Positions Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                            Accessors.Add(accessor);
                            geometryData.Writer.Write(morphTarget.Positions.ToArray());
                            morphTargetAttributes.Add("POSITION", Accessors.Count() - 1);
                        }
                    }
                    if (morphTarget.Normals != null && morphTarget.Normals.Count > 0)
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Normals.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset);

                        BufferViews.Add(bufferView);
                        int bufferviewIndex = BufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        Accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Normals.ToArray());
                        morphTargetAttributes.Add("NORMAL", Accessors.Count() - 1);
                    }
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Count > 0)
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Tangents.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset);

                        BufferViews.Add(bufferView);
                        int bufferviewIndex = BufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        buffer.ByteLength += (int)geometryData.Writer.BaseStream.Position;
                        Accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Tangents.ToArray());
                        morphTargetAttributes.Add("TANGENT", Accessors.Count() - 1);
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
        /// <param name="runtimeMesh"></param>
        /// <param name="gltf"></param>
        /// <param name="buffer"></param>
        /// <param name="geometryData"></param>
        /// <param name="bufferIndex"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.Mesh ConvertMeshToSchema(Runtime.Mesh runtimeMesh, Runtime.GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var schemaMesh = new glTFLoader.Schema.Mesh();
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
        /// <param name="runtimeMaterial"></param>
        /// <param name="gltf"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.Material ConvertMaterialToSchema(Runtime.Material runtimeMaterial, Runtime.GLTF gltf)
        {
            var material = new glTFLoader.Schema.Material();

            if (runtimeMaterial.MetallicRoughnessMaterial != null)
            {
                material.PbrMetallicRoughness = new glTFLoader.Schema.MaterialPbrMetallicRoughness();
                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor != null)
                {
                    material.PbrMetallicRoughness.BaseColorFactor = new[]
                    {
                            runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.x,
                            runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.y,
                            runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.z,
                            runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.w
                        };
                }

                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    var baseColorIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture);

                    material.PbrMetallicRoughness.BaseColorTexture = new glTFLoader.Schema.TextureInfo();
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

                    material.PbrMetallicRoughness.MetallicRoughnessTexture = new glTFLoader.Schema.TextureInfo();
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
                        runtimeMaterial.EmissiveFactor.Value.x,
                        runtimeMaterial.EmissiveFactor.Value.y,
                        runtimeMaterial.EmissiveFactor.Value.z
                    };

            }
            if (runtimeMaterial.NormalTexture != null)
            {
                var normalIndicies = AddTexture(runtimeMaterial.NormalTexture);
                material.NormalTexture = new glTFLoader.Schema.MaterialNormalTextureInfo();

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
                material.OcclusionTexture = new glTFLoader.Schema.MaterialOcclusionTextureInfo();
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
                material.EmissiveTexture = new glTFLoader.Schema.TextureInfo();
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
                if (gltf.ExtensionsUsed != null)
                {
                    gltf.ExtensionsUsed = new List<string>();
                }
                foreach (var extension in runtimeMaterial.Extensions)
                {
                    switch (extension.Name)
                    {
                        case "KHR_materials_pbrSpecularGlossiness":
                            material.Extensions.Add(extension.Name, ConvertPbrSpecularGlossinessExtensionToSchema(extension as Runtime.Extensions.PbrSpecularGlossiness, gltf));
                            break;
                        case "EXT_QuantumRendering":
                            material.Extensions.Add(extension.Name, ConvertExtQuantumRenderingToSchema(extension as Runtime.Extensions.EXT_QuantumRendering, gltf));
                            break;
                        default:
                            throw new NotImplementedException("Extension schema conversion not implemented for " + extension.Name);
                    }
                    
                    if (gltf.ExtensionsUsed == null)
                    {
                        gltf.ExtensionsUsed = new List<string>(new[] { extension.Name });

                    }
                    else if (!gltf.ExtensionsUsed.Contains(extension.Name))
                    {
                        gltf.ExtensionsUsed.Add(extension.Name);
                    }

                }
            }

            return material;
        }

        /// <summary>
        /// Converts Runtime PbrSpecularGlossiness to Schema.
        /// </summary>
        /// <param name="specGloss"></param>
        /// <param name="gltf"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.MaterialPbrSpecularGlossiness ConvertPbrSpecularGlossinessExtensionToSchema(Runtime.Extensions.PbrSpecularGlossiness specGloss, Runtime.GLTF gltf)
        {
            var materialPbrSpecularGlossiness = new glTFLoader.Schema.MaterialPbrSpecularGlossiness();

            if (specGloss.DiffuseFactor.HasValue)
            {
                materialPbrSpecularGlossiness.DiffuseFactor = specGloss.DiffuseFactor.Value.ToArray();
            }
            if (specGloss.DiffuseTexture != null)
            {
                TextureIndices textureIndices = AddTexture(specGloss.DiffuseTexture);
                materialPbrSpecularGlossiness.DiffuseTexture = new glTFLoader.Schema.TextureInfo();
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
                materialPbrSpecularGlossiness.SpecularGlossinessTexture = new glTFLoader.Schema.TextureInfo();
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
        /// <param name="quantumRendering"></param>
        /// <param name="gltf"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.MaterialEXT_QuantumRendering ConvertExtQuantumRenderingToSchema(Runtime.Extensions.EXT_QuantumRendering quantumRendering, Runtime.GLTF gltf)
        {
            var materialEXT_QuantumRendering = new glTFLoader.Schema.MaterialEXT_QuantumRendering();

            if (quantumRendering.PlanckFactor.HasValue)
            {
                materialEXT_QuantumRendering.PlanckFactor = quantumRendering.PlanckFactor.Value.ToArray();
            }
            if (quantumRendering.CopenhagenTexture != null)
            {
                TextureIndices textureIndices = AddTexture(quantumRendering.CopenhagenTexture);
                materialEXT_QuantumRendering.CopenhagenTexture = new glTFLoader.Schema.TextureInfo();
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
                materialEXT_QuantumRendering.SuperpositionCollapseTexture = new glTFLoader.Schema.TextureInfo();
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
        /// Converts runtime mesh primitive to schema.
        /// </summary>
        /// <param name="runtimeMeshPrimitive"></param>
        /// <param name="gltf"></param>
        /// <param name="buffer"></param>
        /// <param name="geometryData"></param>
        /// <param name="bufferIndex"></param>
        /// <returns></returns>
        private static glTFLoader.Schema.MeshPrimitive ConvertMeshPrimitiveToSchema(Runtime.MeshPrimitive runtimeMeshPrimitive, Runtime.GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var attributes = new Dictionary<string, int>();
            var mPrimitive = new glTFLoader.Schema.MeshPrimitive();

            if (runtimeMeshPrimitive.Positions != null)
            {
                //Create BufferView for the position
                int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Positions.Count();
                float[] min = new float[] { };
                float[] max = new float[] { };

                //get the max and min values
                Vector3[] minMaxPositions = GetMinMaxPositions(runtimeMeshPrimitive);
                max = new[] { minMaxPositions[0].x, minMaxPositions[0].y, minMaxPositions[0].z };
                min = new[] { minMaxPositions[1].x, minMaxPositions[1].y, minMaxPositions[1].z };
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset);
                BufferViews.Add(bufferView);
                int bufferviewIndex = BufferViews.Count() - 1;

                // Create an accessor for the bufferView
                var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                Accessors.Add(accessor);
                geometryData.Writer.Write(runtimeMeshPrimitive.Positions.ToArray());
                attributes.Add("POSITION", Accessors.Count() - 1);
            }
            if (runtimeMeshPrimitive.Normals != null)
            {
                // Create BufferView
                int byteLength = sizeof(float) * 3 * runtimeMeshPrimitive.Normals.Count();
                // Create a bufferView
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset);

                BufferViews.Add(bufferView);
                int bufferviewIndex = BufferViews.Count() - 1;

                // Create an accessor for the bufferView
                var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                Accessors.Add(accessor);
                geometryData.Writer.Write(runtimeMeshPrimitive.Normals.ToArray());
                attributes.Add("NORMAL", Accessors.Count() - 1);
            }
            if (runtimeMeshPrimitive.Tangents != null && runtimeMeshPrimitive.Tangents.Count > 0)
            {
                // Create BufferView
                int byteLength = sizeof(float) * 4 * runtimeMeshPrimitive.Tangents.Count();
                // Create a bufferView
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset);


                BufferViews.Add(bufferView);
                int bufferviewIndex = BufferViews.Count() - 1;

                // Create an accessor for the bufferView
                var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeMeshPrimitive.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                Accessors.Add(accessor);
                geometryData.Writer.Write(runtimeMeshPrimitive.Tangents.ToArray());
                attributes.Add("TANGENT", Accessors.Count() - 1);
            }
            if (runtimeMeshPrimitive.Indices != null && runtimeMeshPrimitive.Indices.Count() > 0)
            {
                int byteLength = sizeof(int) * runtimeMeshPrimitive.Indices.Count();
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Indices", byteLength, byteOffset);

                BufferViews.Add(bufferView);
                int bufferviewIndex = BufferViews.Count() - 1;

                var indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT;

                switch (runtimeMeshPrimitive.IndexComponentType)
                {
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE:
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT:
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT:
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT;
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unrecognized Index Component Type Enum " + runtimeMeshPrimitive.IndexComponentType);
                }

                var accessor = CreateAccessor(bufferviewIndex, 0, indexComponentType, runtimeMeshPrimitive.Indices.Count(), "Indices Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                Accessors.Add(accessor);
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

                mPrimitive.Indices = Accessors.Count() - 1;
            }
            if (runtimeMeshPrimitive.Colors != null)
            {
                var colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                var colorAccessorType = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? glTFLoader.Schema.Accessor.TypeEnum.VEC3 : glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                int vectorSize = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;
                int byteLength = 0;

                // Create BufferView
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                switch (runtimeMeshPrimitive.ColorComponentType)
                {
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        byteLength = sizeof(byte) * vectorSize * runtimeMeshPrimitive.Colors.Count();

                        foreach (Vector4 color in runtimeMeshPrimitive.Colors)
                        {
                            geometryData.Writer.Write(Convert.ToByte(Math.Round(color.x * byte.MaxValue)));
                            geometryData.Writer.Write(Convert.ToByte(Math.Round(color.y * byte.MaxValue)));
                            geometryData.Writer.Write(Convert.ToByte(Math.Round(color.z * byte.MaxValue)));
                            if (colorAccessorType == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                            {
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(color.w * byte.MaxValue)));
                            }
                        }
                        break;
                    case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        byteLength = sizeof(ushort) * vectorSize * runtimeMeshPrimitive.Colors.Count();

                        foreach (Vector4 color in runtimeMeshPrimitive.Colors)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.x * ushort.MaxValue)));
                            geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.y * ushort.MaxValue)));
                            geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.z * ushort.MaxValue)));
                            if (colorAccessorType == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(color.w * ushort.MaxValue)));
                            }
                        }
                        break;
                    default: //Default to ColorComponentTypeEnum.FLOAT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                        byteLength = sizeof(float) * vectorSize * runtimeMeshPrimitive.Colors.Count();

                        foreach (Vector4 color in runtimeMeshPrimitive.Colors)
                        {
                            geometryData.Writer.Write(color.x);
                            geometryData.Writer.Write(color.y);
                            geometryData.Writer.Write(color.z);
                            if (colorAccessorType == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                            {
                                geometryData.Writer.Write(color.w);
                            }
                        }
                        break;
                }
                var bufferView = CreateBufferView(bufferIndex, "Colors", byteLength, byteOffset);
                BufferViews.Add(bufferView);
                int bufferviewIndex = BufferViews.Count() - 1;

                // Create an accessor for the bufferView
                // we normalize if the color accessor mode is not set to FLOAT
                bool normalized = runtimeMeshPrimitive.ColorComponentType != MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                var accessor = CreateAccessor(bufferviewIndex, 0, colorAccessorComponentType, runtimeMeshPrimitive.Colors.Count(), "Colors Accessor", null, null, colorAccessorType, normalized);
                Accessors.Add(accessor);
                attributes.Add("COLOR_0", Accessors.Count() - 1);
                if (normalized)
                {
                    // Pad any additional bytes if byteLength is not a multiple of 4
                    int additionalPaddedBytes = Align(byteLength, 4) - byteLength;
                    Enumerable.Range(0, additionalPaddedBytes).ForEach(arg => geometryData.Writer.Write((byte)0));
                }
            }
            if (runtimeMeshPrimitive.TextureCoordSets != null)
            {
                for (int i = 0; i < runtimeMeshPrimitive.TextureCoordSets.Count; ++i)
                {
                    List<Vector2> textureCoordSet = runtimeMeshPrimitive.TextureCoordSets[i];
                    int byteLength;

                    glTFLoader.Schema.Accessor accessor;
                    glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                    // we normalize only if the texture cood accessor type is not float
                    bool normalized = runtimeMeshPrimitive.TextureCoordsComponentType != MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                    switch (runtimeMeshPrimitive.TextureCoordsComponentType)
                    {
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            byteLength = sizeof(float) * 2 * textureCoordSet.Count();
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                            byteLength = sizeof(byte) * 2 * textureCoordSet.Count();
                            break;
                        case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            byteLength = sizeof(ushort) * 2 * textureCoordSet.Count();
                            break;
                        default: // Default to Float
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            byteLength = sizeof(float) * 2 * textureCoordSet.Count();
                            break;
                    }
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Texture Coords " + i, byteLength, byteOffset);
                    BufferViews.Add(bufferView);
                    int bufferviewIndex = BufferViews.Count() - 1;
                    // Create Accessor
                    accessor = CreateAccessor(bufferviewIndex, 0, accessorComponentType, textureCoordSet.Count(), "UV Accessor " + i, null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, normalized);

                    Accessors.Add(accessor);
                    Vector2[] textureCoordSetArr = textureCoordSet.ToArray();
                    if (accessor.Normalized && !accessor.ComponentType.Equals(glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT))
                    {
                        if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE)
                        {
                            foreach (Vector2 tcs in textureCoordSetArr)
                            {
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(tcs.x * byte.MaxValue)));
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(tcs.y * byte.MaxValue)));
                            }
                        }
                        else if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
                        {
                            foreach (Vector2 tcs in textureCoordSetArr)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(tcs.x * ushort.MaxValue)));
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(tcs.y * ushort.MaxValue)));
                            }
                        }
                    }
                    else // write float
                    {
                        geometryData.Writer.Write(textureCoordSetArr);
                    }
                    // Add any additional bytes if the data is normalized
                    if (normalized)
                    {
                        // Pad any additional bytes if byteLength is not a multiple of 4
                        int additionalPaddedBytes = Align(byteLength, 4) - byteLength;
                        Enumerable.Range(0, additionalPaddedBytes).ForEach(arg => geometryData.Writer.Write((byte)0));
                    }
                    attributes.Add("TEXCOORD_" + i, Accessors.Count() - 1);
                }
            }

            mPrimitive.Attributes = attributes;
            if (runtimeMeshPrimitive.Material != null)
            {
                var nMaterial = ConvertMaterialToSchema(runtimeMeshPrimitive.Material, gltf);
                Materials.Add(nMaterial);
                mPrimitive.Material = Materials.Count() - 1;
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
        private static Vector3[] GetMinMaxPositions(Runtime.MeshPrimitive meshPrimitive)
        {

            //get the max and min values
            Vector3 minVal = new Vector3
            {
                x = float.MaxValue,
                y = float.MaxValue,
                z = float.MaxValue
            };
            Vector3 maxVal = new Vector3
            {
                x = float.MinValue,
                y = float.MinValue,
                z = float.MinValue
            };
            foreach (Vector3 position in meshPrimitive.Positions)
            {
                maxVal.x = Math.Max(position.x, maxVal.x);
                maxVal.y = Math.Max(position.y, maxVal.y);
                maxVal.z = Math.Max(position.z, maxVal.z);

                minVal.x = Math.Min(position.x, minVal.x);
                minVal.y = Math.Min(position.y, minVal.y);
                minVal.z = Math.Min(position.z, minVal.z);
            }
            Vector3[] results = { minVal, maxVal };
            return results;
        }
    }
}
