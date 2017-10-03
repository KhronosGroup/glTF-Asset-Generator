﻿using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute(TestNames.Texture_Sampler)]
    class Texture_Sampler : TestValues
    {
        public Texture_Sampler()
        {
            // The base glTF spec does not support mipmapping, so the MagFilter and MinFilter 
            // attributes will have no visible affect unless mipmapping is implemented by the client
            testType = TestNames.Texture_Sampler;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            imageAttributes = new ImageAttribute[]
            {
                new ImageAttribute(texture)
            };
            Runtime.Image image = new Runtime.Image
            {
                Uri = texture
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, image)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.MagFilter_Nearest, glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST, group:1),
                new Property(Propertyname.MagFilter_Linear, glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR, group:1),
                new Property(Propertyname.MinFilter_Nearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST, group:2),
                new Property(Propertyname.MinFilter_Linear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR, group:2),
                new Property(Propertyname.MinFilter_NearestMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST, group:2),
                new Property(Propertyname.MinFilter_LinearMipmapNearest, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST, group:2),
                new Property(Propertyname.MinFilter_NearestMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR, group:2),
                new Property(Propertyname.MinFilter_LinearMipmapLinear, glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR, group:2),
                new Property(Propertyname.WrapS_ClampToEdge, glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE, group:3),
                new Property(Propertyname.WrapS_MirroredRepeat, glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT, group:3),
                new Property(Propertyname.WrapT_ClampToEdge, glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE, group:4),
                new Property(Propertyname.WrapT_MirroredRepeat, glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT, group:4)
            };
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            material.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
            material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                }
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.MagFilter_Nearest ||
                    property.name == Propertyname.MagFilter_Linear)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MagFilter = property.value;
                }
                else if (property.name == Propertyname.MinFilter_Nearest ||
                         property.name == Propertyname.MinFilter_Linear ||
                         property.name == Propertyname.MinFilter_NearestMipmapNearest ||
                         property.name == Propertyname.MinFilter_LinearMipmapNearest ||
                         property.name == Propertyname.MinFilter_NearestMipmapLinear ||
                         property.name == Propertyname.MinFilter_LinearMipmapLinear)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MinFilter = property.value;
                }
                else if (property.name == Propertyname.WrapS_ClampToEdge ||
                         property.name == Propertyname.WrapS_MirroredRepeat ||
                         property.name == Propertyname.WrapS_Repeat)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapS = property.value;
                }
                else if (property.name == Propertyname.WrapT_ClampToEdge ||
                         property.name == Propertyname.WrapT_MirroredRepeat ||
                         property.name == Propertyname.WrapT_Repeat)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapT = property.value;
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
