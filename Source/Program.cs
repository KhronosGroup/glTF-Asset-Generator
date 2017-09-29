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
            var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");

            TestNames[] testBatch = new TestNames[]
            {
                TestNames.Material,
                TestNames.Material_Alpha,
                TestNames.Material_MetallicRoughness,
                TestNames.Texture_Sampler,
                TestNames.Primitive_Attribute
            };

            foreach (var test in testBatch)
            {
                TestValues makeTest = new TestValues();
                var currentTest = makeTest.InitializeTestValues(test);
                var combos = ComboHelper.AttributeCombos(currentTest);
                LogBuilder logs = new LogBuilder();

                // Delete any preexisting files in the output directories, then create those directories if needed
                var assetFolder = Path.Combine(executingAssemblyFolder, test.ToString());
                var trashFolder = Path.Combine(executingAssemblyFolder, "Delete");
                bool tryAgain = true;
                while (tryAgain)
                {
                    try
                    {
                        Directory.Move(assetFolder, trashFolder);
                        Directory.Delete(trashFolder, true);
                        tryAgain = false;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // Do nothing
                        tryAgain = false;
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Unable to delete the directory.");
                        Console.WriteLine("Verify that there are no open files and that the current user has write permission to that directory.");
                        Console.WriteLine("Press any key to try again.");
                        Console.ReadKey();
                        tryAgain = true;
                    }
                }

                Directory.CreateDirectory(assetFolder);

                logs.SetupHeader(currentTest);

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

                    var geometryData = new Data(test.ToString() + "_" + comboIndex + ".bin");
                    dataList.Add(geometryData);
                    Runtime.GLTF wrapper = Common.SinglePlaneWrapper(gltf, geometryData);
                    Runtime.Material mat = new Runtime.Material();

                    wrapper = currentTest.SetModelAttributes(wrapper, mat, combos[comboIndex]);

                    wrapper.BuildGLTF(ref gltf, geometryData);

                    if (currentTest.imageAttributes != null)
                    {
                        foreach (var image in currentTest.imageAttributes)
                        {
                            if (File.Exists(Path.Combine(imageFolder, image.Name)))
                            {
                                File.Copy(Path.Combine(imageFolder, image.Name), Path.Combine(assetFolder, image.Name), true);
                            }
                            else
                            {
                                Debug.WriteLine(imageFolder + " does not exist");
                            }
                        }
                    }

                    var assetFile = Path.Combine(assetFolder, test.ToString() + "_" + comboIndex + ".gltf");
                    glTFLoader.Interface.SaveModel(gltf, assetFile);

                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(assetFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    logs.SetupTable(currentTest, comboIndex, combos);
                }

                logs.WriteOut(currentTest, assetFolder);
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
