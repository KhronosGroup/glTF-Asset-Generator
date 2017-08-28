using glTFLoader.Schema;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static AssetGenerator.GLTFWrapper;

namespace AssetGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyFolder = Path.GetDirectoryName(executingAssembly.Location);
            var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");

            Tests[] testBatch = new Tests[]
            {
                Tests.materials,
                Tests.pbrMetallicRoughness
            };

            foreach (var test in testBatch)
            {
                TestValues makeTest = new TestValues(test);
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
                    GLTFWrapper wrapper = Common.SingleTriangleMultipleUVSetsWrapper(gltf, geometryData);
                    GLTFMaterial mat = new GLTFMaterial(); ;

                    if (makeTest.testArea == Tests.materials)
                    {
                        foreach (Parameter param in combo)
                        {
                            //if (param.name == ParameterName.Name)
                            //{
                            //    gltf.Materials[0].Name = param.value;
                            //}
                            if (param.name == ParameterName.EmissiveFactor)
                            {
                                mat.emissiveFactor = param.value;
                                //gltf.Materials[0].EmissiveFactor = param.value;
                            }
                            else if (param.name == ParameterName.AlphaMode_MASK || param.name == ParameterName.AlphaMode_BLEND)
                            {
                                mat.alphaMode = param.value;
                                //gltf.Materials[0].AlphaMode = param.value;
                            }
                            //else if (param.name == ParameterName.AlphaCutoff)
                            //{
                            //    gltf.Materials[0].AlphaCutoff = param.value;
                            //}
                            //else if (param.name == ParameterName.DoubleSided)
                            //{
                            //    gltf.Materials[0].DoubleSided = param.value;
                            //}
                        }

                        wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
                        wrapper.buildGLTF(gltf, geometryData);
                        //gltf.Meshes[0].Primitives[0].Material = 0;
                    }

                    if (makeTest.testArea == Tests.pbrMetallicRoughness)
                    {
                        mat.metallicRoughnessMaterial = new GLTFMetallicRoughnessMaterial();
                        foreach (Parameter param in combo)
                        {
                            if (param.name == ParameterName.BaseColorFactor)
                            {
                                mat.metallicRoughnessMaterial.baseColorFactor = param.value;
                                //gltf.Materials[0].PbrMetallicRoughness.BaseColorFactor = param.value;
                            }
                            else if (param.name == ParameterName.MetallicFactor)
                            {
                                mat.metallicRoughnessMaterial.metallicFactor = param.value;
                                //gltf.Materials[0].PbrMetallicRoughness.MetallicFactor = param.value;
                            }
                            else if (param.name == ParameterName.RoughnessFactor)
                            {
                                mat.metallicRoughnessMaterial.roughnessFactor = param.value;
                                //gltf.Materials[0].PbrMetallicRoughness.RoughnessFactor = param.value;
                            }
                        }

                        wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
                        wrapper.buildGLTF(gltf, geometryData);
                        //gltf.Meshes[0].Primitives[0].Material = 0;
                    }

                    var assetFolder = Path.Combine(executingAssemblyFolder, test.ToString());
                    Directory.CreateDirectory(assetFolder);

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

                    var assetFile = Path.Combine(assetFolder, name + ".gltf");
                    //glTFLoader.Interface.SaveModel(gltf, assetFile);

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
}
