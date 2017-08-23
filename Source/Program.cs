﻿using glTFLoader.Schema;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AssetGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyFolder = Path.GetDirectoryName(executingAssembly.Location);

            TestValues makeTest = new TestValues(Tests.material);
            var combos = makeTest.ParameterCombos();

            foreach (var combo in combos)
            {
                string name = makeTest.GenerateName(combo);

                var gltf = new Gltf
                {
                    Asset = new Asset
                    {
                        Generator = "glTF Asset Generator " + name,
                        Version = "2.0",
                    }
                };

                var dataList = new List<Data>();

                var geometryData = new Data(name + ".bin");
                dataList.Add(geometryData);

                Common.SingleTriangle(gltf, geometryData, makeTest.testArea);

                if (makeTest.testArea == Tests.material)
                {
                    foreach (Parameter param in combo)
                    {
                        if (param.name == ParameterName.BaseColorFactor)
                        {
                            gltf.Materials[0].PbrMetallicRoughness.BaseColorFactor = param.value;
                        }
                        if (param.name == ParameterName.MetallicFactor)
                        {
                            gltf.Materials[0].PbrMetallicRoughness.MetallicFactor = param.value;
                        }
                        if (param.name == ParameterName.RoughnessFactor)
                        {
                            gltf.Materials[0].PbrMetallicRoughness.RoughnessFactor = param.value;
                        }
                    }

                    gltf.Meshes[0].Primitives[0].Material = 0;
                }

                var assetFolder = Path.Combine(executingAssemblyFolder, "Models");
                Directory.CreateDirectory(assetFolder);

                var assetFile = Path.Combine(assetFolder, name + ".gltf");
                glTFLoader.Interface.SaveModel(gltf, assetFile);

                foreach (var data in dataList)
                {
                    data.Writer.Flush();

                    var dataFile = Path.Combine(assetFolder, data.Name);
                    File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                }
            }
        }
    }
}
