using glTFLoader.Schema;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace AssetGenerator
{
    class ExtraData: Extras
    {
        public string Attributes { get; set; }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            Stopwatch.StartNew();

            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyFolder = Path.GetDirectoryName(executingAssembly.Location);
            //var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");

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
                TestValues makeTest = new TestValues();
                List<List<Property>> combos = ComboHelper.AttributeCombos(test);
                LogBuilder logs = new LogBuilder();
                string assetFolder = Path.Combine(executingAssemblyFolder, test.testType.ToString());

                FileHelper.ClearOldFiles(executingAssemblyFolder, assetFolder);
                Directory.CreateDirectory(assetFolder);
                FileHelper.CopyImageFiles(executingAssemblyFolder, assetFolder, test.GetType());
                logs.SetupHeader(test);

                int numCombos = combos.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    string[] name = LogStringHelper.GenerateName(combos[comboIndex]);

                    var gltf = new Gltf
                    {
                        Asset = new Asset
                        {
                            Generator = "glTF Asset Generator",
                            Version = "2.0",
                            Extras = new ExtraData
                            {
                                Attributes = String.Join(" - ", name)
                            }
                        }
                    };

                    var dataList = new List<Data>();

                    var geometryData = new Data(test.testType.ToString() + "_" + comboIndex + ".bin");
                    dataList.Add(geometryData);
                    Runtime.GLTF wrapper = Common.SinglePlaneWrapper(gltf, geometryData);
                    Runtime.Material mat = new Runtime.Material();

                    wrapper = test.SetModelAttributes(wrapper, mat, combos[comboIndex]);

                    wrapper.BuildGLTF(ref gltf, geometryData);

                    var assetFile = Path.Combine(assetFolder, test.testType.ToString() + "_" + comboIndex + ".gltf");
                    glTFLoader.Interface.SaveModel(gltf, assetFile);

                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(assetFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    logs.SetupTable(test, comboIndex, combos);
                }

                logs.WriteOut(test, assetFolder);
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
