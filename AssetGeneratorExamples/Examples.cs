using AssetGenerator;
using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssetGenerator.GLTFWrapper;

namespace AssetGeneratorExamples
{
    [AssetGroup("Examples")]
    class Examples
    {
        /// <summary>
        /// Demonstrates creation of a triangle with a red base color material
        /// </summary>
        /// <param name="name"></param>
        [Asset("Triangle")]
        public static void createTriangle(string name, Gltf gltf, List<Data> dataList)
        {
            var geometryData = new Data(name + ".bin");
            dataList.Add(geometryData);
            GLTFWrapper wrapper = Common.SingleTriangleMultipleUVSetsWrapper(gltf, geometryData);
            GLTFMaterial mat = new GLTFMaterial();

            mat.metallicRoughnessMaterial = new GLTFMetallicRoughnessMaterial
            {
                baseColorFactor = new Vector4(1.0f, 0.0f, 0.0f, 0.0f)
            };

            wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
            wrapper.buildGLTF(gltf, geometryData);

        }
        /// <summary>
        /// Demonstrates creation of a triangle with a base color texture, and using the Image attribute.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gltf"></param>
        /// <param name="dataList"></param>
        [Asset("TriangleWithTexture"), Image("green.png")]
        public static void createTriangleWithTexture(string name, Gltf gltf, List<Data> dataList)
        {
            var geometryData = new Data(name + ".bin");
            dataList.Add(geometryData);
            GLTFWrapper wrapper = Common.SingleTriangleMultipleUVSetsWrapper(gltf, geometryData);
            GLTFMaterial mat = new GLTFMaterial();

            mat.metallicRoughnessMaterial = new GLTFMetallicRoughnessMaterial
            {
                baseColorTexture = new GLTFTexture
                {
                    source = new GLTFImage
                    {
                        uri = "green.png"
                    },
                    texCoordIndex = 0,
                    sampler = new GLTFSampler()
                }
            };

            wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
            wrapper.buildGLTF(gltf, geometryData);

        }
        /// <summary>
        /// Demonstrates creation of a triangle with a base color texture, and using the metallic roughness texture, using multiple (two) image attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gltf"></param>
        /// <param name="dataList"></param>
        [Asset("TriangleWithBaseAndMetallicRoughnessTexture"), Image("green.png"), Image("blue.png")]
        public static void createTriangleWithBaseAndMetallicRoughnessTexture(string name, Gltf gltf, List<Data> dataList)
        {
            var geometryData = new Data(name + ".bin");
            dataList.Add(geometryData);
            GLTFWrapper wrapper = Common.SingleTriangleMultipleUVSetsWrapper(gltf, geometryData);
            GLTFMaterial mat = new GLTFMaterial();

            mat.metallicRoughnessMaterial = new GLTFMetallicRoughnessMaterial
            {
                baseColorTexture = new GLTFTexture
                {
                    source = new GLTFImage
                    {
                        uri = "green.png"
                    },
                    texCoordIndex = 0,
                    sampler = new GLTFSampler()
                },
                metallicRoughnessTexture = new GLTFTexture
                {
                    source = new GLTFImage
                    {
                        uri = "blue.png"
                    },
                    texCoordIndex = 1,
                    sampler = new GLTFSampler()
                }
            };

            wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
            wrapper.buildGLTF(gltf, geometryData);
        }
    }
}
