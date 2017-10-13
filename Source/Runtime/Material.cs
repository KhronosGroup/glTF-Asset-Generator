using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Material
    /// </summary>
    internal class Material
    {
        private struct TextureIndices
        {
            public int? SamplerIndex;
            public int? ImageIndex;
            public int? TextureCoordIndex;
        }
        /// <summary>
        /// The user-defined name of this object
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A set of parameter values that are used to define the metallic-roughness material model from Physically-Based Rendering methodology
        /// </summary>
        public Runtime.PbrMetallicRoughness MetallicRoughnessMaterial { get; set; }
        /// <summary>
        /// Texture that contains tangent-space normal information
        /// </summary>
        public Runtime.Texture NormalTexture { get; set; }
        /// <summary>
        /// Scaling factor for the normal texture
        /// </summary>
        public float? NormalScale { get; set; }
        /// <summary>
        /// Texture that defines areas of the surface that are occluded from light, and thus rendered darker.  This information is contained in the "red" channel.
        /// </summary>
        public Runtime.Texture OcclusionTexture { get; set; }
        /// <summary>
        /// Scaling factor for the occlusion texture
        /// </summary>
        public float? OcclusionStrength { get; set; }
        /// <summary>
        /// Texture that may be used to illuminate parts of the object surface. It defines the color of the light that is emitted from the surface
        /// </summary>
        public Runtime.Texture EmissiveTexture { get; set; }
        /// <summary>
        /// Contains scaling factors for the "red", "green" and "blue" components of the emissive texture
        /// </summary>
        public Vector3? EmissiveFactor { get; set; }

        /// <summary>
        /// Specifies whether the material is double sided
        /// </summary>
        public bool? DoubleSided { get; set; }

        /// <summary>
        /// The alpha rendering mode of the material
        /// </summary>
        public glTFLoader.Schema.Material.AlphaModeEnum? AlphaMode { get; set; }
        /// <summary>
        /// The alpha cutoff value of the material
        /// </summary>
        public float? AlphaCutoff { get; set; }

        public List<Runtime.Extensions.Extension> Extensions { get; set; }


        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="gTexture"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="material"></param>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        private TextureIndices AddTexture(Runtime.Texture gTexture, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, glTFLoader.Schema.Material material)
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
        /// Creates a Material object and updates the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <returns>Returns a Material object, and updates the properties of the GLTFWrapper</returns>
        public glTFLoader.Schema.Material ConvertToSchema(Runtime.GLTF gltf, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures)
        {
            glTFLoader.Schema.Material material = new glTFLoader.Schema.Material();
            
            if (MetallicRoughnessMaterial != null)
            {
                material.PbrMetallicRoughness = new glTFLoader.Schema.MaterialPbrMetallicRoughness();
                if (MetallicRoughnessMaterial.BaseColorFactor != null)
                {
                    material.PbrMetallicRoughness.BaseColorFactor = new[]
                    {
                            MetallicRoughnessMaterial.BaseColorFactor.Value.x,
                            MetallicRoughnessMaterial.BaseColorFactor.Value.y,
                            MetallicRoughnessMaterial.BaseColorFactor.Value.z,
                            MetallicRoughnessMaterial.BaseColorFactor.Value.w
                        };
                }

                if (MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    TextureIndices baseColorIndices = AddTexture(MetallicRoughnessMaterial.BaseColorTexture, samplers, images, textures, material);

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
                if (MetallicRoughnessMaterial.MetallicRoughnessTexture != null)
                {
                   TextureIndices metallicRoughnessIndices = AddTexture(MetallicRoughnessMaterial.MetallicRoughnessTexture, samplers, images, textures, material);

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
                if (MetallicRoughnessMaterial.MetallicFactor.HasValue)
                {
                    material.PbrMetallicRoughness.MetallicFactor = MetallicRoughnessMaterial.MetallicFactor.Value;
                }
                if (MetallicRoughnessMaterial.RoughnessFactor.HasValue)
                {
                    material.PbrMetallicRoughness.RoughnessFactor = MetallicRoughnessMaterial.RoughnessFactor.Value;
                }
            }
            if (EmissiveFactor != null)
            {
                material.EmissiveFactor = new[]
                {
                        EmissiveFactor.Value.x,
                        EmissiveFactor.Value.y,
                        EmissiveFactor.Value.z
                    };

            }
            if (NormalTexture != null)
            {
                TextureIndices normalIndicies = AddTexture(NormalTexture, samplers, images, textures, material);
                material.NormalTexture = new glTFLoader.Schema.MaterialNormalTextureInfo();

                if (normalIndicies.ImageIndex.HasValue)
                {
                    material.NormalTexture.Index = normalIndicies.ImageIndex.Value;

                }
                if (normalIndicies.TextureCoordIndex.HasValue)
                {
                    material.NormalTexture.TexCoord = normalIndicies.TextureCoordIndex.Value;
                }
                if (NormalScale.HasValue)
                {
                    material.NormalTexture.Scale = NormalScale.Value;
                }
            }
            if (OcclusionTexture != null)
            {
                TextureIndices occlusionIndicies = AddTexture(OcclusionTexture, samplers, images, textures, material);
                material.OcclusionTexture = new glTFLoader.Schema.MaterialOcclusionTextureInfo();
                if (occlusionIndicies.ImageIndex.HasValue)
                {
                    material.OcclusionTexture.Index = occlusionIndicies.ImageIndex.Value;

                };
                if (occlusionIndicies.TextureCoordIndex.HasValue)
                {
                    material.OcclusionTexture.TexCoord = occlusionIndicies.TextureCoordIndex.Value;
                }
                if (OcclusionStrength.HasValue)
                {
                    material.OcclusionTexture.Strength = OcclusionStrength.Value;
                }
            }
            if (EmissiveTexture != null)
            {
                TextureIndices emissiveIndicies = AddTexture(EmissiveTexture, samplers, images, textures, material);
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
            if (AlphaMode.HasValue)
            {
                material.AlphaMode = AlphaMode.Value;
            }
            if (AlphaCutoff.HasValue)
            {
                material.AlphaCutoff = AlphaCutoff.Value;
            }
            if (Name != null)
            {
                material.Name = Name;
            }
            if (DoubleSided.HasValue)
            {
                material.DoubleSided = DoubleSided.Value;
            }
            if (Extensions != null)
            {
                if (material.Extensions == null)
                {
                    material.Extensions = new Dictionary<string, object>();
                }
                foreach (var extension in Extensions)
                {
                    material.Extensions.Add(extension.Name, extension.ConvertToSchema(gltf, samplers, images, textures));
                }
            }

            return material;
        }
    }
}
