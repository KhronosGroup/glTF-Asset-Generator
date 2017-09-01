using glTFLoader.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssetGenerator.GLTFWrapper;

namespace AssetGenerator
{
    namespace AssetGenerator.GLTFWrapper.Tests
    {
        [TestClass()]
        public class GLTFMaterialTests
        {
            [TestMethod()]
            public void addTextureTest()
            {
                List<Sampler> samplers = new List<Sampler>();
                List<Image> images = new List<Image>();
                List<Texture> textures = new List<Texture>();
                Material material = new Material();
                GLTFTexture gTexture = new GLTFTexture
                {
                    source = new GLTFImage
                    {
                        uri = "green.png",
                    },
                    sampler = new GLTFSampler()
                };
                GLTFMaterial gMaterial = new GLTFMaterial();
                int?[] indices = gMaterial.addTexture(gTexture, samplers, images, textures, material);
                
                Assert.AreEqual(indices[0].Value, 0);
                Assert.AreEqual(indices[1].Value, 0);

                indices = gMaterial.addTexture(gTexture, samplers, images, textures, material);

                Assert.AreEqual(indices[0].Value, 0);
                Assert.AreEqual(indices[1].Value, 0);
            }
        }
    }

}
