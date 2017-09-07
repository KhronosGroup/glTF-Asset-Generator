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
    public class Material
    {
        /// <summary>
        /// The user-defined name of this object
        /// </summary>
        public string name;
        /// <summary>
        /// A set of parameter values that are used to define the metallic-roughness material model from Physically-Based Rendering methodology
        /// </summary>
        public Runtime.MetallicRoughnessMaterial metallicRoughnessMaterial;
        /// <summary>
        /// Texture that contains tangent-space normal information
        /// </summary>
        public Runtime.Texture normalTexture;
        /// <summary>
        /// Scaling factor for the normal texture
        /// </summary>
        public float? normalScale;
        /// <summary>
        /// Texture that defines areas of the surface that are occluded from light, and thus rendered darker.  This information is contained in the "red" channel.
        /// </summary>
        public Runtime.Texture occlusionTexture;
        /// <summary>
        /// Scaling factor for the occlusion texture
        /// </summary>
        public float? occlusionStrength;
        /// <summary>
        /// Texture that may be used to illuminate parts of the object surface. It defines the color of the light that is emitted from the surface
        /// </summary>
        public Runtime.Texture emissiveTexture;
        /// <summary>
        /// Contains scaling factors for the "red", "green" and "blue" components of the emissive texture
        /// </summary>
        public Vector3? emissiveFactor;

        /// <summary>
        /// Specifies whether the material is double sided
        /// </summary>
        public bool? doubleSided;

        /// <summary>
        /// The alpha rendering mode of the material
        /// </summary>
        public glTFLoader.Schema.Material.AlphaModeEnum? alphaMode;
        /// <summary>
        /// The alpha cutoff value of the material
        /// </summary>
        public float? alphaCutoff;
        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="gTexture"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="material"></param>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        public int?[] addTexture(Runtime.Texture gTexture, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, glTFLoader.Schema.Material material)
        {
            List<int> indices = new List<int>();
            int? sampler_index = null;
            int? image_index = null;

            if (gTexture != null)
            {
                if (gTexture.sampler != null)
                {
                    // If a similar sampler is already being used in the list, reuse that index instead of creating a new sampler object
                    if (samplers.Count > 0)
                    {
                        int find_index;
                        ObjectSearch<glTFLoader.Schema.Sampler> samplerSearch = new ObjectSearch<glTFLoader.Schema.Sampler>(gTexture.sampler.convertToSampler());
                        find_index = samplers.FindIndex(0, samplers.Count, samplerSearch.Equals);
                        if (find_index != -1)
                            sampler_index = find_index;
                    }
                    if (!sampler_index.HasValue)
                    {
                        glTFLoader.Schema.Sampler sampler = gTexture.sampler.convertToSampler();
                        samplers.Add(sampler);
                        sampler_index = samplers.Count() - 1;
                    }
                }
                if (gTexture.source != null)
                {
                    // If an equivalent image object has already been created, reuse its index instead of creating a new image object
                    glTFLoader.Schema.Image image = gTexture.source.convertToImage();
                    ObjectSearch<glTFLoader.Schema.Image> imageSearch = new ObjectSearch<glTFLoader.Schema.Image>(image);
                    int find_image_index = images.FindIndex(0, images.Count, imageSearch.Equals);

                    if (find_image_index != -1)
                    {
                        image_index = find_image_index;
                    }

                    if (!image_index.HasValue)
                    {
                        images.Add(image);
                        image_index = images.Count() - 1;
                    }
                }
                glTFLoader.Schema.Texture texture = new glTFLoader.Schema.Texture();
                if (sampler_index.HasValue)
                {
                    texture.Sampler = sampler_index.Value;
                }
                if (image_index.HasValue)
                {
                    texture.Source = image_index.Value;
                }
                if (gTexture.name != null)
                {
                    texture.Name = gTexture.name;
                }
                // If an equivalent texture has already been created, re-use that texture's index instead of creating a new texture
                int find_texture_index = -1;
                if (textures.Count > 0)
                {
                    ObjectSearch<glTFLoader.Schema.Texture> textureSearch = new ObjectSearch<glTFLoader.Schema.Texture>(texture);
                    find_texture_index = textures.FindIndex(textureSearch.Equals);
                }
                if (find_texture_index > -1)
                {
                    indices.Add(find_texture_index);
                }
                else
                {
                    textures.Add(texture);
                    indices.Add(textures.Count() - 1);
                }

                if (gTexture.texCoordIndex.HasValue)
                {
                    indices.Add(gTexture.texCoordIndex.Value);
                }
            }
            int?[] result = { sampler_index, image_index };
            return result;
        }
        /// <summary>
        /// Creates a Material object and updates the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <returns>Returns a Material object, and updates the properties of the GLTFWrapper</returns>
        public glTFLoader.Schema.Material createMaterial(List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures)
        {
            glTFLoader.Schema.Material material = new glTFLoader.Schema.Material();
            material.PbrMetallicRoughness = new glTFLoader.Schema.MaterialPbrMetallicRoughness();

            if (metallicRoughnessMaterial != null)
            {
                if (metallicRoughnessMaterial.baseColorFactor != null)
                {
                    material.PbrMetallicRoughness.BaseColorFactor = new[]
                    {
                            metallicRoughnessMaterial.baseColorFactor.Value.x,
                            metallicRoughnessMaterial.baseColorFactor.Value.y,
                            metallicRoughnessMaterial.baseColorFactor.Value.z,
                            metallicRoughnessMaterial.baseColorFactor.Value.w
                        };
                }

                if (metallicRoughnessMaterial.baseColorTexture != null)
                {
                    int?[] baseColorIndices = addTexture(metallicRoughnessMaterial.baseColorTexture, samplers, images, textures, material);

                    material.PbrMetallicRoughness.BaseColorTexture = new glTFLoader.Schema.TextureInfo();
                    if (baseColorIndices[0].HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture.Index = baseColorIndices[0].Value;
                    }
                    if (baseColorIndices[1].HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture.TexCoord = baseColorIndices[1].Value;
                    };

                }
                if (metallicRoughnessMaterial.metallicRoughnessTexture != null)
                {
                    int?[] metallicRoughnessIndices = addTexture(metallicRoughnessMaterial.metallicRoughnessTexture, samplers, images, textures, material);

                    material.PbrMetallicRoughness.MetallicRoughnessTexture = new glTFLoader.Schema.TextureInfo();
                    if (metallicRoughnessIndices[0].HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture.Index = metallicRoughnessIndices[0].Value;
                    }
                    if (metallicRoughnessIndices[1].HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture.TexCoord = metallicRoughnessIndices[1].Value;
                    }
                }
                if (metallicRoughnessMaterial.metallicFactor.HasValue)
                {
                    material.PbrMetallicRoughness.MetallicFactor = metallicRoughnessMaterial.metallicFactor.Value;
                }
                if (metallicRoughnessMaterial.roughnessFactor.HasValue)
                {
                    material.PbrMetallicRoughness.RoughnessFactor = metallicRoughnessMaterial.roughnessFactor.Value;
                }
            }
            if (emissiveFactor != null)
            {
                material.EmissiveFactor = new[]
                {
                        emissiveFactor.Value.x,
                        emissiveFactor.Value.y,
                        emissiveFactor.Value.z
                    };

            }
            if (normalTexture != null)
            {
                int?[] normalIndicies = addTexture(normalTexture, samplers, images, textures, material);
                material.NormalTexture = new glTFLoader.Schema.MaterialNormalTextureInfo();

                if (normalIndicies[0].HasValue)
                {
                    material.NormalTexture.Index = normalIndicies[0].Value;

                }
                if (normalIndicies[1].HasValue)
                {
                    material.NormalTexture.TexCoord = normalIndicies[1].Value;
                }
            }
            if (occlusionTexture != null)
            {
                int?[] occlusionIndicies = addTexture(occlusionTexture, samplers, images, textures, material);
                material.OcclusionTexture = new glTFLoader.Schema.MaterialOcclusionTextureInfo();
                if (occlusionIndicies[0].HasValue)
                {
                    material.OcclusionTexture.Index = occlusionIndicies[0].Value;

                };
                if (occlusionIndicies[1].HasValue)
                {
                    material.OcclusionTexture.TexCoord = occlusionIndicies[1].Value;
                }
            }
            if (emissiveTexture != null)
            {
                int?[] emissiveIndicies = addTexture(emissiveTexture, samplers, images, textures, material);
                material.EmissiveTexture = new glTFLoader.Schema.TextureInfo();
                if (emissiveIndicies[0].HasValue)
                {
                    material.EmissiveTexture.Index = emissiveIndicies[0].Value;
                }
                if (emissiveIndicies[1].HasValue)
                {
                    material.EmissiveTexture.TexCoord = emissiveIndicies[1].Value;
                }
            }
            if (alphaMode.HasValue)
            {
                material.AlphaMode = alphaMode.Value;
            }
            if (alphaCutoff.HasValue)
            {
                material.AlphaCutoff = alphaCutoff.Value;
            }
            if (name != null)
            {
                material.Name = name;
            }
            if (doubleSided.HasValue)
            {
                material.DoubleSided = doubleSided.Value;
            }
            return material;
        }

    }
}
