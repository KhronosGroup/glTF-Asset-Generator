using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    class Texture_Sampler : TestValues
    {
        public Texture_Sampler()
        {
            // The base glTF spec does not support mipmapping, so the MagFilter and MinFilter 
            // attributes will have no visible affect unless mipmapping is implemented by the client
            onlyBinaryAttributes = false;
            noPrerequisite = false;
            imageAttributes = new ImageAttribute[]
            {
                new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            requiredAttributes = new List<Attribute>
            {
                new Attribute(AttributeName.BaseColorTexture, image)
            };
            attributes = new List<Attribute>
            {
                new Attribute(AttributeName.MagFilter_Nearest, glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST, group:1),
                new Attribute(AttributeName.MagFilter_Linear, glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR, group:1),
                new Attribute(AttributeName.MinFilter_Nearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST, group:2),
                new Attribute(AttributeName.MinFilter_Linear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR, group:2),
                new Attribute(AttributeName.MinFilter_NearestMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST, group:2),
                new Attribute(AttributeName.MinFilter_LinearMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST, group:2),
                new Attribute(AttributeName.MinFilter_NearestMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR, group:2),
                new Attribute(AttributeName.MinFilter_LinearMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR, group:2),
                new Attribute(AttributeName.WrapS_ClampToEdge, glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE, group:3),
                new Attribute(AttributeName.WrapS_MirroredRepeat, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT, group:3),
                new Attribute(AttributeName.WrapT_ClampToEdge, glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE, group:4),
                new Attribute(AttributeName.WrapT_MirroredRepeat, glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT, group:4)
            };
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Attribute> combo)
        {
            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
            material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();

            foreach (Attribute req in requiredAttributes)
            {
                if (req.name == AttributeName.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                }
            }

            foreach (Attribute attribute in combo)
            {
                if (attribute.name == AttributeName.MagFilter_Nearest ||
                    attribute.name == AttributeName.MagFilter_Linear)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MagFilter = attribute.value;
                }
                else if (attribute.name == AttributeName.MinFilter_Nearest ||
                         attribute.name == AttributeName.MinFilter_Linear ||
                         attribute.name == AttributeName.MinFilter_NearestMipmapNearest ||
                         attribute.name == AttributeName.MinFilter_LinearMipmapNearest ||
                         attribute.name == AttributeName.MinFilter_NearestMipmapLinear ||
                         attribute.name == AttributeName.MinFilter_LinearMipmapLinear)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MinFilter = attribute.value;
                }
                else if (attribute.name == AttributeName.WrapS_ClampToEdge ||
                         attribute.name == AttributeName.WrapS_MirroredRepeat ||
                         attribute.name == AttributeName.WrapS_Repeat)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapS = attribute.value;
                }
                else if (attribute.name == AttributeName.WrapT_ClampToEdge ||
                         attribute.name == AttributeName.WrapT_MirroredRepeat ||
                         attribute.name == AttributeName.WrapT_Repeat)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapT = attribute.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
