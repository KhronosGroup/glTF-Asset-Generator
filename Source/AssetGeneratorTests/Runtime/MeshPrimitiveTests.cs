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
    public class MeshPrimitiveTests
    {
        [TestMethod()]
        public void GetMinMaxNormalsTest()
        {
            List<Vector3> normals = new List<Vector3>
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };
            MeshPrimitive meshPrim = new MeshPrimitive();
            meshPrim.Normals = normals;
            Vector3[] minMaxNormals = meshPrim.GetMinMaxNormals();
            Assert.AreEqual(new Vector3(0.0f, 0.0f, -1.0f), minMaxNormals[0]);
            Assert.AreEqual(new Vector3(0.0f, 0.0f, 1.0f), minMaxNormals[1]);
        }

        [TestMethod()]
        public void GetMinMaxPositionsTest()
        {
            List<Vector3> positions = new List<Vector3>
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };
            MeshPrimitive meshPrim = new MeshPrimitive();
            meshPrim.Positions = positions;
            Vector3[] minMaxPositions = meshPrim.GetMinMaxPositions();
            Assert.AreEqual(new Vector3(0.0f, 0.0f, -1.0f), minMaxPositions[0]);
            Assert.AreEqual(new Vector3(0.0f, 0.0f, 1.0f), minMaxPositions[1]);
        }

        [TestMethod()]
        public void GetMinMaxTextureCoordsTest()
        {
            List<List<Vector2>> triangleTextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(0.5f, 1.0f),
                    new Vector2(0.25f, 0.0f)
                },
                new List<Vector2>
                {
                    new Vector2(0.5f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.75f, 0.0f)
                }

            };

            MeshPrimitive meshPrim = new MeshPrimitive();
            meshPrim.TextureCoordSets = triangleTextureCoordSets;
            List<Vector2[]> minMaxTextureCoordSets = meshPrim.GetMinMaxTextureCoords();

            Assert.AreEqual(minMaxTextureCoordSets[0][0], new Vector2(0.0f, 0.0f));
            Assert.AreEqual(minMaxTextureCoordSets[0][1], new Vector2(0.5f, 1.0f));

            Assert.AreEqual(minMaxTextureCoordSets[1][0], new Vector2(0.5f, 0.0f));
            Assert.AreEqual(minMaxTextureCoordSets[1][1], new Vector2(1.0f, 1.0f));
        }

        [TestMethod()]
        public void ConvertToMeshPrimitiveTest()
        {
            List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
            List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
            List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
            glTFLoader.Schema.Buffer buffer = new glTFLoader.Schema.Buffer();
            Data geometryData = new Data("test.bin");
            int buffer_index = 0;

            MeshPrimitive meshPrim = new MeshPrimitive();
            meshPrim.ConvertToMeshPrimitive(bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, buffer_index, true, false, false, false);
        }

        [TestMethod()]
        public void GetMorphTargetsTest()
        {
            var positions = new List<Vector3>
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            };

            var positions2 = new List<Vector3>
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
            };

            var normals = new List<Vector3>
            {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };

            List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
            List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
            List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
            glTFLoader.Schema.Buffer buffer = new glTFLoader.Schema.Buffer();
            Data geometryData = new Data("test.bin");
            int buffer_index = 0;


            MeshPrimitive meshPrim = new MeshPrimitive
            {
                Positions = positions,
                Normals = normals
            };
            MeshPrimitive morphTarget = new MeshPrimitive
            {
                Positions = positions2,
                Normals = normals
            };
            List<MeshPrimitive> morphTargets = new List<MeshPrimitive>();
            morphTargets.Add(morphTarget);

            meshPrim.MorphTargets = morphTargets;
            meshPrim.morphTargetWeight = 0;
            Mesh mesh = new Mesh();
            mesh.AddPrimitive(meshPrim);
            glTFLoader.Schema.Mesh m = mesh.ConvertToMesh(bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, buffer_index);
            Assert.IsTrue(m.Primitives[0].Targets.Count() > 0);
            Assert.IsTrue(m.Weights.Count() > 0);
        }

    }
}