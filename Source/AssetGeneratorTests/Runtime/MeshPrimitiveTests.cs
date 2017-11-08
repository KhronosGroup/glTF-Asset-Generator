﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void ConvertToMeshPrimitiveTest()
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
            int bufferIndex = 0;

            MeshPrimitive meshPrim = new MeshPrimitive();
            meshPrim.ConvertToSchema(gltf, bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, bufferIndex);
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
            Runtime.GLTF gltf = new GLTF();
            List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
            List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
            List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
            glTFLoader.Schema.Buffer buffer = new glTFLoader.Schema.Buffer();
            Data geometryData = new Data("test.bin");
            int bufferIndex = 0;
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
            glTFLoader.Schema.Mesh m = mesh.ConvertToSchema(gltf, bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, bufferIndex);
            Assert.IsTrue(m.Primitives[0].Targets.Count() > 0);
            Assert.IsTrue(m.Weights.Count() > 0);
        }
        [TestMethod()]
        public void ColorAttributeEnumTest()
        {
            MeshPrimitive meshPrimitive = new MeshPrimitive();

            meshPrimitive.ColorComponentType = MeshPrimitive.ColorComponentTypeEnum.FLOAT;
            meshPrimitive.ColorType = MeshPrimitive.ColorTypeEnum.VEC3;
            Assert.AreEqual(meshPrimitive.ColorComponentType, MeshPrimitive.ColorComponentTypeEnum.FLOAT);
            Assert.AreEqual(meshPrimitive.ColorType, MeshPrimitive.ColorTypeEnum.VEC3);
        }

        [TestMethod()]
        public void IndicesTest()
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
            int bufferIndex = 0;

            MeshPrimitive meshPrimitive = new MeshPrimitive();
            meshPrimitive.Positions = new List<Vector3>
            {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };
            meshPrimitive.Normals = new List<Vector3>
            {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };
            meshPrimitive.Indices = new List<int>
            {
                0, 1, 3, 1, 2, 3
            };
            meshPrimitive.TextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(0.0f, 0.0f)
                }
            };
            glTFLoader.Schema.MeshPrimitive sMeshPrimitive = meshPrimitive.ConvertToSchema(gltf, bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, bufferIndex);
            Assert.AreEqual(sMeshPrimitive.Indices, 2); // indices is third bufferview, or index 2
            Assert.AreEqual(accessors[2].Count, 6); // should be siz index values
            
            
        }
    }
}