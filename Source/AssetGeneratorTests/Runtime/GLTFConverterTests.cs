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
    public class GLTFConverterTests
    {
        [TestMethod()]
        public void ConvertRuntimeToSchemaTest()
        {
            var bufferName = "GLTFConverterTest.bin";
            var runtimeGLTF = new GLTF();
            var schemaGLTF = new glTFLoader.Schema.Gltf();
            var geometryData = new Data(bufferName);
            GLTFConverter.ConvertRuntimeToSchema(runtimeGLTF, ref schemaGLTF, geometryData);
            Assert.AreEqual(schemaGLTF.Scenes, null);
            Assert.AreEqual(schemaGLTF.Buffers[0].Uri, bufferName);
        }
    }
}