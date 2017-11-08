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
                FileHelper.CopyImageFiles(executingAssembly, executingAssemblyFolder, assetFolder, test.usedImages);
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
                            // Inserts a string into the .gltf containing the properties that are set for a given model, for debug.
                            Attributes = String.Join(" - ", name)
                        }
                    };

                    var gltf = new glTFLoader.Schema.Gltf
                    {
                        Asset = asset.ConvertToSchema()
                    };

                    var dataList = new List<Data>();

                    var geometryData = new Data(test.testType.ToString() + "_" + comboIndex.ToString("00") + ".bin");
                    dataList.Add(geometryData);

                    Runtime.GLTF wrapper = Common.SinglePlane();

                    wrapper.Asset = asset;

                    Runtime.Material mat = new Runtime.Material();

                    // Takes the current combo and uses it to bundle together the data for the desired properties 
                    wrapper = test.SetModelAttributes(wrapper, mat, combos[comboIndex], ref gltf);

                    // Passes the desired properties to the runtime layer, which then coverts that data into
                    // a gltf loader object, ready to create the model
                    wrapper.BuildGLTF(ref gltf, geometryData);

                    // Makes last second changes to the model that bypass the runtime layer
                    // in order to add 'features that don't really exist otherwise
                    test.PostRuntimeChanges(combos[comboIndex], ref gltf);

                    // Creates the .gltf file and writes the model's data to it
                    var assetFile = Path.Combine(assetFolder, test.testType.ToString() + "_" + comboIndex.ToString("00") + ".gltf");
                    glTFLoader.Interface.SaveModel(gltf, assetFile);

                    // Creates the .bin file and writes the model's data to it
                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(assetFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    logs.SetupTable(test, comboIndex, combos);
                }

                logs.WriteOut(executingAssembly, test, assetFolder);
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
