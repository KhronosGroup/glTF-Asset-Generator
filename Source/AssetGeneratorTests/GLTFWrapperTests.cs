using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using glTFLoader.Schema;

namespace AssetGenerator.Tests
{
    [TestClass()]
    public class GLTFWrapperTests
    {
        [TestMethod()]
        public void GLTFWrapperTest()
        {
            GLTFWrapper wrapper = new GLTFWrapper();
            Assert.IsNotNull(wrapper);
        }

        [TestMethod()]
        public void buildGLTFTest()
        {
            Gltf gltf = new Gltf();
            Data geometryData = new Data("testBuildTFTest");
            GLTFWrapper wrapper = new GLTFWrapper();
            wrapper.buildGLTF(gltf, geometryData);
            Assert.IsNotNull(wrapper);
        }
    }
}