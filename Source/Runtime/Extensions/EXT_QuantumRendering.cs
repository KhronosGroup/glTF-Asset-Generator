﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator.Runtime.Extensions
{
    /// <summary>
    /// This is a fake extension. We've created it to verify the behavior of clients attempting
    /// to load a model which contains an extension that they do not support. By creating an
    /// extension which does not exist, we guarantee consistent results regardless of which
    /// client attempts to load a model requiring this extension.
    /// </summary>
    internal class EXT_QuantumRendering : Extension
    {
        /// <summary>
        /// The name of the extension
        /// </summary>
        public override string Name
        {
            get
            {
                return "EXT_QuantumRendering";
            }
        }
        private struct TextureIndices
        {
            public int? SamplerIndex;
            public int? ImageIndex;
            public int? TextureCoordIndex;
        }
        /// <summary>
        /// The reflected diffuse factor of the material
        /// </summary>
        public Vector4? PlanckFactor { get; set; }
        /// <summary>
        /// The diffuse texture
        /// </summary>
        public Texture CopenhagenTexture { get; set; }
        /// <summary>
        /// The specular RGB color of the material
        /// </summary>
        public Vector3? EntanglementFactor { get; set; }
        /// <summary>
        /// The glossiness or smoothness of the material
        /// </summary>
        public float? ProbabilisticFactor { get; set; }
        /// <summary>
        /// The specular-glossiness texture
        /// </summary>
        public Texture SuperpositionCollapseTexture { get; set; }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="gTexture"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="material"></param>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        private TextureIndices AddTexture(Runtime.Texture gTexture, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures)
        {
            List<int> indices = new List<int>();
            int? samplerIndex = null;
            int? imageIndex = null;
            int? textureCoordIndex = null;

            if (gTexture != null)
            {
                if (gTexture.Sampler != null)
                {
                    // If a similar sampler is already being used in the list, reuse that index instead of creating a new sampler object
                    if (samplers.Count > 0)
                    {
                        int findIndex;
                        ObjectSearch<glTFLoader.Schema.Sampler> samplerSearch = new ObjectSearch<glTFLoader.Schema.Sampler>(gTexture.Sampler.ConvertToSchema());
                        findIndex = samplers.FindIndex(0, samplers.Count, samplerSearch.Equals);
                        if (findIndex != -1)
                            samplerIndex = findIndex;
                    }
                    if (!samplerIndex.HasValue)
                    {
                        glTFLoader.Schema.Sampler sampler = gTexture.Sampler.ConvertToSchema();
                        samplers.Add(sampler);
                        samplerIndex = samplers.Count() - 1;
                    }
                }
                if (gTexture.Source != null)
                {
                    // If an equivalent image object has already been created, reuse its index instead of creating a new image object
                    glTFLoader.Schema.Image image = gTexture.Source.ConvertToSchema();
                    ObjectSearch<glTFLoader.Schema.Image> imageSearch = new ObjectSearch<glTFLoader.Schema.Image>(image);
                    int findImageIndex = images.FindIndex(0, images.Count, imageSearch.Equals);

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
                glTFLoader.Schema.Texture texture = new glTFLoader.Schema.Texture();
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
                if (textures.Count > 0)
                {
                    ObjectSearch<glTFLoader.Schema.Texture> textureSearch = new ObjectSearch<glTFLoader.Schema.Texture>(texture);
                    findTextureIndex = textures.FindIndex(textureSearch.Equals);
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
        /// Converts the material to schema
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <returns></returns>
        public override Object ConvertToSchema(Runtime.GLTF gltf, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures)
        {
            glTFLoader.Schema.MaterialEXT_QuantumRendering materialEXT_QuantumRendering = new glTFLoader.Schema.MaterialEXT_QuantumRendering();

            if (PlanckFactor.HasValue)
            {
                materialEXT_QuantumRendering.PlanckFactor = PlanckFactor.Value.ToArray();
            }
            if (CopenhagenTexture != null)
            {
                TextureIndices textureIndices = AddTexture(CopenhagenTexture, samplers, images, textures);
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
            if (EntanglementFactor.HasValue)
            {
                materialEXT_QuantumRendering.EntanglementFactor = EntanglementFactor.Value.ToArray();
            }
            if (ProbabilisticFactor.HasValue)
            {
                materialEXT_QuantumRendering.ProbabilisticFactor = ProbabilisticFactor.Value;
            }
            if (SuperpositionCollapseTexture != null)
            {
                TextureIndices textureIndices = AddTexture(SuperpositionCollapseTexture, samplers, images, textures);
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
            if (ProbabilisticFactor.HasValue)
            {
                materialEXT_QuantumRendering.ProbabilisticFactor = ProbabilisticFactor.Value;
            }

            return materialEXT_QuantumRendering;
        }
    }
}
