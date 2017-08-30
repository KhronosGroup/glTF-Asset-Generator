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

                int numCombos = combos.Length;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    string name = makeTest.GenerateName(combos[comboIndex]);

                    var gltf = new Gltf
                    {
                        Asset = new Asset
                        {
                            Generator = "glTF Asset Generator : " + name,
                            Version = "2.0",
                        }
                    };

                    var dataList = new List<Data>();

                    var geometryData = new Data(test.ToString() + "_" + comboIndex + ".bin");
                    dataList.Add(geometryData);
                    GLTFWrapper wrapper = Common.SingleTriangleMultipleUVSetsWrapper(gltf, geometryData);
                    GLTFMaterial mat = new GLTFMaterial(); ;

                    if (makeTest.testArea == Tests.materials)
                    {
                        foreach (Parameter param in combos[comboIndex])
                        {
                            if (param.name == ParameterName.Name)
                            {
                                mat.name = param.value;
                            }
                            else if (param.name == ParameterName.EmissiveFactor)
                            {
                                mat.emissiveFactor = param.value;
                            }
                            else if (param.name == ParameterName.AlphaMode_MASK || param.name == ParameterName.AlphaMode_BLEND)
                            {
                                mat.alphaMode = param.value;
                            }
                            else if (param.name == ParameterName.AlphaCutoff)
                            {
                                mat.alphaCutoff = param.value;
                            }
                            else if (param.name == ParameterName.DoubleSided)
                            {
                                mat.doubleSided = param.value;
                            }
                            else if (param.name == ParameterName.NormalTexture)
                            {
                                mat.normalTexture = new GLTFTexture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.normalTexture.source = param.value;
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.normalTexture.texCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Scale && param.prerequisite == ParameterName.NormalTexture)
                            {
                                mat.normalScale = param.value;
                            }
                            else if (param.name == ParameterName.OcclusionTexture)
                            {
                                mat.occlusionTexture = new GLTFTexture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.occlusionTexture.source = param.value;
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.occlusionTexture.texCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Scale && param.prerequisite == ParameterName.OcclusionTexture)
                            {
                                mat.occlusionStrength = param.value;
                            }
                            else if (param.name == ParameterName.EmissiveTexture)
                            {
                                mat.emissiveTexture = new GLTFTexture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.EmissiveTexture)
                            {
                                mat.emissiveTexture.source = param.value;
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.EmissiveTexture)
                            {
                                mat.emissiveTexture.texCoordIndex = param.value;
                            }
                        }

                        wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
                        wrapper.buildGLTF(gltf, geometryData);
                    }

                    else if (makeTest.testArea == Tests.pbrMetallicRoughness)
                    {
                        mat.metallicRoughnessMaterial = new GLTFMetallicRoughnessMaterial();
                        foreach (Parameter param in combos[comboIndex])
                        {
                            if (param.name == ParameterName.BaseColorFactor)
                            {
                                mat.metallicRoughnessMaterial.baseColorFactor = param.value;
                            }
                            else if (param.name == ParameterName.MetallicFactor)
                            {
                                mat.metallicRoughnessMaterial.metallicFactor = param.value;
                            }
                            else if (param.name == ParameterName.RoughnessFactor)
                            {
                                mat.metallicRoughnessMaterial.roughnessFactor = param.value;
                            }
                            else if (param.name == ParameterName.BaseColorTexture)
                            {
                                mat.metallicRoughnessMaterial.baseColorTexture = new GLTFTexture();
                            }
                            else if (param.name == ParameterName.Source && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.metallicRoughnessMaterial.baseColorTexture.source = param.value;
                            }
                            else if (param.name == ParameterName.Sampler && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.metallicRoughnessMaterial.baseColorTexture.sampler = new GLTFSampler();
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.metallicRoughnessMaterial.baseColorTexture.texCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Name && param.prerequisite == ParameterName.BaseColorTexture)
                            {
                                mat.metallicRoughnessMaterial.baseColorTexture.name = param.value;
                            }
                            else if (param.name == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.metallicRoughnessMaterial.metallicRoughnessTexture = new GLTFTexture();
                            }
                            else if (param.name == ParameterName.Sampler && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.metallicRoughnessMaterial.metallicRoughnessTexture.sampler = new GLTFSampler();
                            }
                            else if (param.name == ParameterName.TexCoord && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.metallicRoughnessMaterial.metallicRoughnessTexture.texCoordIndex = param.value;
                            }
                            else if (param.name == ParameterName.Name && param.prerequisite == ParameterName.MetallicRoughnessTexture)
                            {
                                mat.metallicRoughnessMaterial.metallicRoughnessTexture.name = param.value;
                            }
                        }

                        wrapper.scenes[0].meshes[0].meshPrimitives[0].material = mat;
                        wrapper.buildGLTF(gltf, geometryData);
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

                    var assetFile = Path.Combine(assetFolder, test.ToString() + "_" + comboIndex + ".gltf");
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
}
