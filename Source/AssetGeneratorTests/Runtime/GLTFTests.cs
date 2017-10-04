using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime.Tests
{
    [TestClass()]
    public class GLTFTests
    {
        [TestMethod()]
        public void GLTFTest()
        {
            Runtime.GLTF gltf = new GLTF();
            Assert.IsTrue(gltf != null);
        }

        [TestMethod()]
        public void BuildGLTFTest()
        {
            glTFLoader.Schema.Gltf gltf = new glTFLoader.Schema.Gltf();
            var asset = new Asset
            {
                Generator = "Unit Test",
                Copyright = "Unit Tester",
            };
            gltf.Asset = asset.ConvertToAsset();
            Data geometryData = new Data("test.bin");
            Runtime.GLTF wrapper = new GLTF();
            wrapper.BuildGLTF(ref gltf, geometryData);
        }
    }
}