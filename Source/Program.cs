using glTFLoader.Schema;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace AssetGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Stopwatch.StartNew();

            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyFolder = Path.GetDirectoryName(executingAssembly.Location);

            // Uses Reflection to create a list containing one instance of each test class 
            List<dynamic> testBatch = new List<dynamic>();
            foreach (var type in executingAssembly.GetTypes())
            {
                var testAttribute = type.GetCustomAttribute<TestAttribute>();
                if (testAttribute != null)
                {
                    ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
                    dynamic test = ctor.Invoke(new dynamic[] { });
                    testBatch.Add(test);
                }
            }

            foreach (var test in testBatch)
            {
                Test makeTest = new Test();
                List<List<Property>> combos = ComboHelper.AttributeCombos(test);
                LogBuilder logs = new LogBuilder();
                string assetFolder = Path.Combine(executingAssemblyFolder, test.testType.ToString());

                FileHelper.ClearOldFiles(executingAssemblyFolder, assetFolder);
                Directory.CreateDirectory(assetFolder);
                FileHelper.CopyImageFiles(executingAssemblyFolder, assetFolder, test.usedImages);
                logs.SetupHeader(test);

                int numCombos = combos.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    string[] name = LogStringHelper.GenerateName(combos[comboIndex]);

                    var asset = new Runtime.Asset
                    {
                        Generator = "glTF Asset Generator",
                        Version = "2.0",
                        Extras = new Runtime.Extras
                        {
                            Attributes = String.Join(" - ", name)
                        }
                    };

                    var gltf = new glTFLoader.Schema.Gltf
                    {
                        Asset = asset.ConvertToAsset()
                    };

                    var dataList = new List<Data>();

                    var geometryData = new Data(test.testType.ToString() + "_" + comboIndex.ToString("00") + ".bin");
                    dataList.Add(geometryData);

                    Runtime.GLTF wrapper = Common.SinglePlane();

                    wrapper.Asset = asset;

                    Runtime.Material mat = new Runtime.Material();

                    wrapper = test.SetModelAttributes(wrapper, mat, combos[comboIndex]);

                    wrapper.BuildGLTF(ref gltf, geometryData);

                    var assetFile = Path.Combine(assetFolder, test.testType.ToString() + "_" + comboIndex.ToString("00") + ".gltf");
                    glTFLoader.Interface.SaveModel(gltf, assetFile);

                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(assetFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    logs.SetupTable(test, comboIndex, combos);
                }

                logs.WriteOut(executingAssembly, test, assetFolder, executingAssemblyFolder);
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
