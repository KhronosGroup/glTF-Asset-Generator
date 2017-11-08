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
    public class MeshTests
    {
        [TestMethod()]
        public void MeshTest()
        {
            Mesh m = new Mesh();
            Assert.IsNotNull(m);
        }

        [TestMethod()]
        public void AddPrimitiveTest()
        {
            Mesh m = new Mesh();
            m.AddPrimitive(new MeshPrimitive());
        }

        [TestMethod()]
        public void ConvertToMeshTest()
        {
            Runtime.GLTF gltf = new GLTF();
            List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
            List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
            List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
            glTFLoader.Schema.Buffer buffer = new glTFLoader.Schema.Buffer();
            Data geometryData = new Data("test.bin");
            Mesh m = new Mesh();

            int buffer_index = 0;

            m.ConvertToSchema(gltf, bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, buffer_index);
        }
    }
}