using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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
        [TestMethod()]
        public void InterleaveAttributesTest()
        {
            var bufferName = "GLTFInterleaveTest.bin";
            var runtimeGLTF = new GLTF();

            List<Vector3> planePositions = new List<Vector3>()
            {
                new Vector3( 0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };

            // 1:1 UV mapping
            List<List<Vector2>> planeTextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 1.0f),
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f)
                },
            };

            List<int> PlaneIndices = new List<int>
            {
                1, 0, 3, 1, 3, 2
            };

            var meshPrimitive = new Runtime.MeshPrimitive
            {
                Positions = planePositions,
                TextureCoordSets = planeTextureCoordSets,
                Indices = PlaneIndices
            };
            meshPrimitive.ColorComponentType = MeshPrimitive.ColorComponentTypeEnum.FLOAT;
            meshPrimitive.IndexComponentType = MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT;
            meshPrimitive.Mode = MeshPrimitive.ModeEnum.TRIANGLES;
            meshPrimitive.TextureCoordsComponentType = MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;

            meshPrimitive.Interleave = true;
            var node = new Runtime.Node
            {
                Mesh = new Runtime.Mesh
                {
                    MeshPrimitives = new List<MeshPrimitive> { meshPrimitive }
                }
            };
            var scene = new Runtime.Scene
            {
                Nodes = new List<Node> { node }
            };
            runtimeGLTF.Scenes = new List<Scene> { scene };

            var schemaGLTF = new glTFLoader.Schema.Gltf();
            var geometryData = new Data(bufferName);
            GLTFConverter.ConvertRuntimeToSchema(runtimeGLTF, ref schemaGLTF, geometryData);
            Assert.IsTrue(schemaGLTF.BufferViews.Count() == 2);
            Assert.IsTrue(schemaGLTF.BufferViews[0].ByteStride == 20);
        }

    }
}