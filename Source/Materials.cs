using glTFLoader.Schema;
using System.Collections.Generic;

namespace AssetGenerator
{
    [AssetGroup("Materials")]
    internal static class Materials
    {
        [Asset("Test")]
        public static void Test(string name, Gltf gltf, List<Data> dataList)
        {
            var geometryData = new Data(name + ".bin");
            dataList.Add(geometryData);

            Common.SingleTriangle(gltf, geometryData);

            gltf.Materials = new[]
            {
                new Material
                {
                    PbrMetallicRoughness = new MaterialPbrMetallicRoughness
                    {
                        BaseColorFactor = new[] { 1.0f, 0.0f, 0.0f, 0.0f },
                    }
                }
            };

            gltf.Meshes[0].Primitives[0].Material = 0;
        }
    }
}
