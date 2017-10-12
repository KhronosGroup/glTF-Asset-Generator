using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    public class MaterialSpecularGlossinessExtension
    {
        public static string Name
        {
            get
            {
                return "KHR_materials_pbrSpecularGlossiness";
            }
        }
        public struct TextureIndices
        {
            public int? SamplerIndex;
            public int? ImageIndex;
            public int? TextureCoordIndex;
        }
        /// <summary>
        /// The reflected diffuse factor of the material
        /// </summary>
        public Vector4? DiffuseFactor { get; set; }
        /// <summary>
        /// The diffuse texture
        /// </summary>
        public Texture DiffuseTexture { get; set; }
        /// <summary>
        /// The specular RGB color of the material
        /// </summary>
        public Vector3? SpecularFactor { get; set; }
        /// <summary>
        /// The glossiness or smoothness of the material
        /// </summary>
        public float? GlossinessFactor { get; set; }
        /// <summary>
        /// The specular-glossiness texture
        /// </summary>
        public Texture SpecularGlossinessTexture { get; set; }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <param name="gTexture"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="material"></param>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        public TextureIndices AddTexture(Runtime.Texture gTexture, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures)
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
                        ObjectSearch<glTFLoader.Schema.Sampler> samplerSearch = new ObjectSearch<glTFLoader.Schema.Sampler>(gTexture.Sampler.ConvertToSampler());
                        findIndex = samplers.FindIndex(0, samplers.Count, samplerSearch.Equals);
                        if (findIndex != -1)
                            samplerIndex = findIndex;
                    }
                    if (!samplerIndex.HasValue)
                    {
                        glTFLoader.Schema.Sampler sampler = gTexture.Sampler.ConvertToSampler();
                        samplers.Add(sampler);
                        samplerIndex = samplers.Count() - 1;
                    }
                }
                if (gTexture.Source != null)
                {
                    // If an equivalent image object has already been created, reuse its index instead of creating a new image object
                    glTFLoader.Schema.Image image = gTexture.Source.ConvertToImage();
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
        public glTFLoader.Schema.MaterialPbrSpecularGlossiness ConvertToSpecularGlossiness(List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures)
        {
            glTFLoader.Schema.MaterialPbrSpecularGlossiness materialPbrSpecularGlossiness = new glTFLoader.Schema.MaterialPbrSpecularGlossiness();
            
            if (DiffuseFactor.HasValue)
            {
                materialPbrSpecularGlossiness.DiffuseFactor = DiffuseFactor.Value.ToArray();
            }
            if (DiffuseTexture != null)
            {
                TextureIndices textureIndices = AddTexture(DiffuseTexture, samplers, images, textures);
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
            if (SpecularFactor.HasValue)
            {
                materialPbrSpecularGlossiness.SpecularFactor = SpecularFactor.Value.ToArray();
            }
            if (GlossinessFactor.HasValue)
            {
                materialPbrSpecularGlossiness.GlossinessFactor = GlossinessFactor.Value;
            }
            if (SpecularGlossinessTexture != null)
            {
                TextureIndices textureIndices = AddTexture(SpecularGlossinessTexture, samplers, images, textures);
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
            if (GlossinessFactor.HasValue)
            {
                materialPbrSpecularGlossiness.GlossinessFactor = GlossinessFactor.Value;
            }

            return materialPbrSpecularGlossiness;
        }
    }
}
