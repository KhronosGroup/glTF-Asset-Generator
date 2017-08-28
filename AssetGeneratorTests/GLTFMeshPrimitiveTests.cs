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
        public class GLTFMeshPrimitiveTests
        {
            GLTFMeshPrimitive primitive;

            [TestInitialize]
            public void Initialize()
            {
                List<Vector3> trianglePositions = new List<Vector3>()
                {
                    new Vector3(1.0f, 0.0f, 0.0f),
                    new Vector3(-1.0f, 0.0f, 0.0f),
                    new Vector3(0.0f, 1.0f, 0.0f)
                };
                List<Vector3> triangleNormals = new List<Vector3>()
                {
                    new Vector3(0.0f, 0.0f, -1.0f),
                    new Vector3(0.0f, 0.0f, -1.0f),
                    new Vector3(0.0f, 0.0f, -1.0f)
                };
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
                primitive = new GLTFMeshPrimitive
                {
                    positions = trianglePositions,
                    normals = triangleNormals,
                    textureCoordSets = triangleTextureCoordSets
                };
            }

            [TestMethod()]
            public void getMinMaxNormalsTest()
            {
                Vector3[] minMaxNormals = primitive.getMinMaxNormals();

                Assert.AreEqual(minMaxNormals[0], new Vector3(0.0f, 0.0f, -1.0f));
                Assert.AreEqual(minMaxNormals[1], new Vector3(0.0f, 0.0f, -1.0f));

            }

            [TestMethod()]
            public void getMinMaxPositionsTest()
            {
                Vector3[] minMaxPositions = primitive.getMinMaxPositions();

                Assert.AreEqual(minMaxPositions[0], new Vector3(-1.0f, 0.0f, 0.0f));
                Assert.AreEqual(minMaxPositions[1], new Vector3(1.0f, 1.0f, 0.0f));
            }

            [TestMethod()]
            public void getMinMaxTextureCoordsTest()
            {
                List<Vector2[]> minMaxTextureCoordsSet = primitive.getMinMaxTextureCoords();

                Assert.AreEqual(minMaxTextureCoordsSet[0][0], new Vector2(0.0f, 0.0f));
                Assert.AreEqual(minMaxTextureCoordsSet[0][1], new Vector2(0.5f, 1.0f));

                Assert.AreEqual(minMaxTextureCoordsSet[1][0], new Vector2(0.5f, 0.0f));
                Assert.AreEqual(minMaxTextureCoordsSet[1][1], new Vector2(1.0f, 1.0f));
            }
        }
    }

}
