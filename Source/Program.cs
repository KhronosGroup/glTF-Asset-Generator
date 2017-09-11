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
                Tests.Sampler
            };

            foreach (var test in testBatch)
            {
                TestValues makeTest = new TestValues(test);
                var combos = makeTest.ParameterCombos();
                var csv = new StringBuilder();
                var md = new StringBuilder();
                List<List<string>> mdLog = new List<List<string>>();

                // Setup the log file
                mdLog.Add(new List<string>()); // First line must be blank
                mdLog.Add(new List<string>
                {
                    "Model Name" // First cell is empty
                });
                mdLog.Add(new List<string>
                {
                    "---" // Hyphens after header 
                });
                foreach (var param in makeTest.parameters)
                {
                    mdLog[1].Add(param.name.ToString());
                    mdLog[2].Add(":---:");
                }                

                var assetFolder = Path.Combine(executingAssemblyFolder, test.ToString());
                Directory.CreateDirectory(assetFolder);

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
                    Runtime.GLTF wrapper = Common.SingleTriangleMultipleUVSetsWrapper(gltf, geometryData);
                    Runtime.Material mat = new Runtime.Material(); ;

                    if (makeTest.testArea == Tests.Materials)
                    {
                        foreach (Parameter param in combos[comboIndex])
                        {
                            if (param.name == ParameterName.Name)
                            {
                                mat.Name = param.value;
                            }
                            else if (param.name == ParameterName.EmissiveFactor)
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
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.NormalTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.NormalTexture.TexCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Scale && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.NormalScale = param.value;
                            }
                            else if (param.name == ParameterName.OcclusionTexture)
                            {
                                mat.OcclusionTexture = new Runtime.Texture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.OcclusionTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.OcclusionTexture.TexCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Scale && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.OcclusionStrength = param.value;
                            }
                            else if (param.name == ParameterName.EmissiveTexture)
                            {
                                mat.EmissiveTexture = new Runtime.Texture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.EmissiveTexture)
                            {
                                mat.EmissiveTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.EmissiveTexture)
                            {
                                mat.EmissiveTexture.TexCoordIndex = param.value;
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
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.Sampler && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Name && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Name = param.value;
                            }
                            else if (param.name == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = param.value;
                            }
                            else if (param.name == ParameterName.Sampler && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture.Sampler = new Runtime.Sampler();
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture.TexCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Name && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture.Name = param.value;
                            }
                        }
                    }

                    else if (makeTest.testArea == Tests.Sampler)
                    {
                        mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                        mat.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                        mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();

                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>
                        {
                            new List<Vector2>
                            {
                                new Vector2(-2.0f, 0.0f),
                                new Vector2(-1.0f, 1.0f),
                                new Vector2(0.0f, 0.0f)
                            }
                        };

                        foreach (Parameter req in makeTest.requiredParameters)
                        {
                            if (req.name == ParameterName.Source)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                            }
                            else if (req.name == ParameterName.TexCoord)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = req.value;
                            }
                            else if (req.name == ParameterName.Name)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Name = req.value;
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
                                System.Diagnostics.Debug.WriteLine(imageFolder + " does not exist");
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
                    foreach (var possibleParam in makeTest.parameters)
                    {
                        var match = combos[comboIndex].Exists(e =>
                            e.name == possibleParam.name &&
                            e.prerequisite == possibleParam.prerequisite);
                        if (match == true)
                        {
                            mdLog[logIndex].Add(":white_check_mark:");
                        }
                        else
                        {
                            mdLog[logIndex].Add(":x:");
                        }
                    }

                    var writeToLog = string.Format("{0},{1}", comboIndex, String.Join(" - ", name));
                    csv.AppendLine(writeToLog);
                }

                foreach (var line in mdLog)
                {
                    md.AppendLine(String.Join("| ", line));
                }
                
                var logFile = Path.Combine(assetFolder, test.ToString() + "_log.csv");
                File.WriteAllText(logFile, csv.ToString());
                var mdLogFile = Path.Combine(assetFolder, test.ToString() + "_log.md");
                File.WriteAllText(mdLogFile, md.ToString());
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
            Console.ReadKey();
        }
    }
}
