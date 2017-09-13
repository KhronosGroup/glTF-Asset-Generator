using glTFLoader.Schema;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text;

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

            Tests[] testBatch = new Tests[]
            {
                Tests.Materials,
                Tests.Samplers
            };

            foreach (var test in testBatch)
            {
                TestValues makeTest = new TestValues(test);
                var combos = makeTest.ParameterCombos();
                var csv = new StringBuilder();
                var md = new StringBuilder();
                List<List<string>> mdLog = new List<List<string>>();
                string lastName = null;

                // Delete any preexisting files in the output directories, then create those directories if needed
                var assetFolder = Path.Combine(executingAssemblyFolder, test.ToString());
                bool tryAgain = true;
                while (tryAgain)
                {
                    try
                    {
                        Directory.Delete(assetFolder, true);
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

                // Setup the log file
                mdLog.Add(new List<string>()); // First line must be blank
                mdLog.Add(new List<string>
                {
                    "Model Name" // First cell is empty
                });
                mdLog.Add(new List<string>
                {
                    "---" // Hyphens for roll after header 
                });
                for (int i = 0; i < makeTest.parameters.Count; i++)
                {
                    string attributeName;
                    if (makeTest.parameters[i].prerequisite != ParameterName.Undefined)
                    {
                        attributeName = makeTest.parameters[i].prerequisite.ToString() + makeTest.parameters[i].name.ToString();
                    }
                    else
                    {
                        attributeName = makeTest.parameters[i].name.ToString();
                    }
                    attributeName = makeTest.GenerateNameWithSpaces(attributeName);
                    if (attributeName != lastName) // Skip duplicate names caused by non-binary attributes
                    {
                        lastName = attributeName;
                        mdLog[1].Add(attributeName);
                        mdLog[2].Add(":---:");
                    }
                }

                int numCombos = combos.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    string[] name = makeTest.GenerateName(combos[comboIndex]);

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
                    Runtime.Material mat = new Runtime.Material(); ;

                    if (makeTest.testArea == Tests.Materials)
                    {
                        foreach (Parameter param in combos[comboIndex])
                        {
                            if (param.name == ParameterName.EmissiveFactor)
                            {
                                mat.EmissiveFactor = param.value;
                            }
                            else if (param.name == ParameterName.AlphaMode_OPAQUE ||
                                     param.name == ParameterName.AlphaMode_MASK ||
                                     param.name == ParameterName.AlphaMode_BLEND)
                            {
                                mat.AlphaMode = param.value;
                            }
                            else if (param.name == ParameterName.AlphaCutoff)
                            {
                                mat.AlphaCutoff = param.value;
                            }
                            else if (param.name == ParameterName.DoubleSided)
                            {
                                mat.DoubleSided = param.value;
                            }
                            else if (param.name == ParameterName.NormalTexture)
                            {
                                mat.NormalTexture = new Runtime.Texture();
                                mat.NormalTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.Scale && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.NormalScale = param.value;
                            }
                            else if (param.name == ParameterName.OcclusionTexture)
                            {
                                mat.OcclusionTexture = new Runtime.Texture();
                                mat.OcclusionTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.Strength && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.OcclusionStrength = param.value;
                            }
                            else if (param.name == ParameterName.EmissiveTexture)
                            {
                                mat.EmissiveTexture = new Runtime.Texture();
                                mat.EmissiveTexture.Source = param.value;
                            }

                            // Only set the MetallicRoughnessMaterial if one of it's attributes will be used
                            if (mat.MetallicRoughnessMaterial == null)
                            {
                                mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                            }

                            if (param.name == ParameterName.BaseColorFactor)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorFactor = param.value;
                            }
                            else if (param.name == ParameterName.MetallicFactor)
                            {
                                mat.MetallicRoughnessMaterial.MetallicFactor = param.value;
                            }
                            else if (param.name == ParameterName.RoughnessFactor)
                            {
                                mat.MetallicRoughnessMaterial.RoughnessFactor = param.value;
                            }
                            else if (param.name == ParameterName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = param.value;
                            }
                        }
                    }

                    else if (makeTest.testArea == Tests.Samplers)
                    {
                        mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                        mat.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                        mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();

                        foreach (Parameter req in makeTest.requiredParameters)
                        {
                            if (req.name == ParameterName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                            }
                        }

                        foreach (Parameter param in combos[comboIndex])
                        {
                            if (param.name == ParameterName.MagFilter_NEAREST ||
                                param.name == ParameterName.MagFilter_LINEAR)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MagFilter = param.value;
                            }
                            else if (param.name == ParameterName.MinFilter_NEAREST ||
                                     param.name == ParameterName.MinFilter_LINEAR ||
                                     param.name == ParameterName.MinFilter_NEAREST_MIPMAP_NEAREST ||
                                     param.name == ParameterName.MinFilter_LINEAR_MIPMAP_NEAREST ||
                                     param.name == ParameterName.MinFilter_NEAREST_MIPMAP_LINEAR ||
                                     param.name == ParameterName.MinFilter_LINEAR_MIPMAP_LINEAR)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MinFilter = param.value;
                            }
                            else if (param.name == ParameterName.WrapS_CLAMP_TO_EDGE ||
                                     param.name == ParameterName.WrapS_MIRRORED_REPEAT ||
                                     param.name == ParameterName.WrapS_REPEAT)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapS = param.value;
                            }
                            else if (param.name == ParameterName.WrapT_CLAMP_TO_EDGE ||
                                     param.name == ParameterName.WrapT_MIRRORED_REPEAT ||
                                     param.name == ParameterName.WrapT_REPEAT)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapT = param.value;
                            }
                        }
                    }

                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = mat;
                    wrapper.BuildGLTF(ref gltf, geometryData);

                    if (makeTest.imageAttributes != null)
                    {
                        foreach (var image in makeTest.imageAttributes)
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

                    mdLog.Add(new List<string> // New row for a new model
                    {
                        test.ToString() + "_" + comboIndex
                    }); 
                    int logIndex = mdLog.Count - 1;
                    List<int> nonBinaryUsed = new List<int>();
                    foreach (var possibleParam in makeTest.parameters)
                    {
                        var paramIndex = combos[comboIndex].FindIndex(e =>
                            e.name == possibleParam.name &&
                            e.prerequisite == possibleParam.prerequisite);
                        if (paramIndex != -1)
                        {
                            if (possibleParam.binarySet > 0)
                            {
                                var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleParam.binarySet);
                                if (alreadyUsed)
                                {
                                    mdLog[logIndex][mdLog[logIndex].Count - 1] = makeTest.GenerateNonbinaryName(possibleParam.name.ToString());
                                }
                                else
                                {
                                    mdLog[logIndex].Add(makeTest.GenerateNonbinaryName(possibleParam.name.ToString()));
                                    nonBinaryUsed.Add(possibleParam.binarySet);
                                }
                            }
                            else
                            {
                                mdLog[logIndex].Add(":white_check_mark:");
                            }
                        }
                        else
                        {
                            if (possibleParam.binarySet > 0)
                            {
                                var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleParam.binarySet);
                                if (!alreadyUsed)
                                {
                                    mdLog[logIndex].Add(" ");
                                    nonBinaryUsed.Add(possibleParam.binarySet);
                                }
                            }
                            else
                            {
                                mdLog[logIndex].Add(" ");
                            }
                        }
                    }

                    var writeToLog = string.Format("{0},{1}", comboIndex, String.Join(" - ", name));
                    csv.AppendLine(writeToLog);
                }

                foreach (var line in mdLog)
                {
                    md.AppendLine(String.Join(" | ", line));
                }
                
                var logFile = Path.Combine(assetFolder, test.ToString() + "_log.csv");
                File.WriteAllText(logFile, csv.ToString());
                var mdLogFile = Path.Combine(assetFolder, "README.md");
                File.WriteAllText(mdLogFile, md.ToString());
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
