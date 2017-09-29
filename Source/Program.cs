using glTFLoader.Schema;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Linq;

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
                makeTest.InitializeTestValues(test);
                var combos = ComboHelper.AttributeCombos(makeTest);
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

                logs.SetupHeader(makeTest);

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

                    if (makeTest.testType == TestNames.Material)
                    {
                        mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();

                        foreach (Attribute req in makeTest.requiredAttributes)
                        {
                            if (req.name == AttributeName.MetallicFactor)
                            {
                                mat.MetallicRoughnessMaterial.MetallicFactor = req.value;
                            }
                        }

                        foreach (Attribute attribute in combos[comboIndex])
                        {
                            if (attribute.name == AttributeName.EmissiveFactor)
                            {
                                mat.EmissiveFactor = attribute.value;
                            }
                            else if (attribute.name == AttributeName.NormalTexture)
                            {
                                mat.NormalTexture = new Runtime.Texture();
                                mat.NormalTexture.Source = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Scale && attribute.prerequisite == AttributeName.NormalTexture)
                            {
                                mat.NormalScale = attribute.value;
                            }
                            else if (attribute.name == AttributeName.OcclusionTexture)
                            {
                                mat.OcclusionTexture = new Runtime.Texture();
                                mat.OcclusionTexture.Source = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Strength && attribute.prerequisite == AttributeName.OcclusionTexture)
                            {
                                mat.OcclusionStrength = attribute.value;
                            }
                            else if (attribute.name == AttributeName.EmissiveTexture)
                            {
                                mat.EmissiveTexture = new Runtime.Texture();
                                mat.EmissiveTexture.Source = attribute.value;
                            }
                        }
                    }

                    if (makeTest.testType == TestNames.Material_Alpha)
                    {
                        mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                        mat.NormalTexture = new Runtime.Texture();

                        foreach (Attribute req in makeTest.requiredAttributes)
                        {
                            if (req.name == AttributeName.BaseColorFactor)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorFactor = req.value;
                            }
                            else if (req.name == AttributeName.NormalTexture)
                            {
                                mat.NormalTexture.Source = req.value;
                            }
                        }

                        foreach (Attribute attribute in combos[comboIndex])
                        {
                            if (attribute.name == AttributeName.AlphaMode_Opaque ||
                                attribute.name == AttributeName.AlphaMode_Mask ||
                                attribute.name == AttributeName.AlphaMode_Blend)
                            {
                                mat.AlphaMode = attribute.value;
                            }
                            else if (attribute.name == AttributeName.AlphaCutoff)
                            {
                                mat.AlphaCutoff = attribute.value;
                            }
                            else if (attribute.name == AttributeName.DoubleSided)
                            {
                                mat.DoubleSided = attribute.value;
                            }
                        }
                    }

                    else if (makeTest.testType == TestNames.Material_MetallicRoughness)
                    {
                        foreach (Attribute attribute in combos[comboIndex])
                        {
                            if (mat.MetallicRoughnessMaterial == null)
                            {
                                mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                            }

                            if (attribute.name == AttributeName.BaseColorFactor)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorFactor = attribute.value;
                            }
                            else if (attribute.name == AttributeName.MetallicFactor)
                            {
                                mat.MetallicRoughnessMaterial.MetallicFactor = attribute.value;
                            }
                            else if (attribute.name == AttributeName.RoughnessFactor)
                            {
                                mat.MetallicRoughnessMaterial.RoughnessFactor = attribute.value;
                            }
                            else if (attribute.name == AttributeName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = attribute.value;
                            }
                            else if (attribute.name == AttributeName.MetallicRoughnessTexture)
                            {
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                                mat.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = attribute.value;
                            }
                        }
                    }

                    else if (makeTest.testType == TestNames.Texture_Sampler)
                    {
                        mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                        mat.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                        mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();

                        foreach (Attribute req in makeTest.requiredAttributes)
                        {
                            if (req.name == AttributeName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                            }
                        }

                        foreach (Attribute attribute in combos[comboIndex])
                        {
                            if (attribute.name == AttributeName.MagFilter_Nearest ||
                                attribute.name == AttributeName.MagFilter_Linear)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MagFilter = attribute.value;
                            }
                            else if (attribute.name == AttributeName.MinFilter_Nearest ||
                                     attribute.name == AttributeName.MinFilter_Linear ||
                                     attribute.name == AttributeName.MinFilter_NearestMipmapNearest ||
                                     attribute.name == AttributeName.MinFilter_LinearMipmapNearest ||
                                     attribute.name == AttributeName.MinFilter_NearestMipmapLinear ||
                                     attribute.name == AttributeName.MinFilter_LinearMipmapLinear)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.MinFilter = attribute.value;
                            }
                            else if (attribute.name == AttributeName.WrapS_ClampToEdge ||
                                     attribute.name == AttributeName.WrapS_MirroredRepeat ||
                                     attribute.name == AttributeName.WrapS_Repeat)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapS = attribute.value;
                            }
                            else if (attribute.name == AttributeName.WrapT_ClampToEdge ||
                                     attribute.name == AttributeName.WrapT_MirroredRepeat ||
                                     attribute.name == AttributeName.WrapT_Repeat)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Sampler.WrapT = attribute.value;
                            }
                        }
                    }

                    else if (makeTest.testType == TestNames.Primitive_Attribute)
                    {
                        // Clear values from the default model, so we can test those values not being set
                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Normals = null;

                        mat.MetallicRoughnessMaterial = new Runtime.MetallicRoughnessMaterial();
                        mat.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                        mat.NormalTexture = new Runtime.Texture();

                        foreach (Attribute req in makeTest.requiredAttributes)
                        {
                            if (req.name == AttributeName.BaseColorTexture)
                            {
                                mat.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                                mat.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 0;
                            }
                        }

                        foreach (Attribute attribute in combos[comboIndex])
                        {
                            if (attribute.name == AttributeName.Normal)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Normals = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Tangent)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Tangents = attribute.value;
                            }
                            else if (attribute.name == AttributeName.TexCoord0_Float)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets[0] = attribute.value;
                            }
                            else if (attribute.name == AttributeName.TexCoord0_Byte)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets[0] = attribute.value;
                            }
                            else if (attribute.name == AttributeName.TexCoord0_Short)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets[0] = attribute.value;
                            }
                            else if (attribute.name == AttributeName.TexCoord1_Float)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(attribute.value);

                                var NormText = makeTest.requiredAttributes.Find(e => e.name == AttributeName.NormalTexture);
                                mat.NormalTexture.Source = NormText.value;
                                mat.NormalTexture.TexCoordIndex = 1;
                            }
                            else if (attribute.name == AttributeName.TexCoord1_Byte)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(attribute.value);

                                var NormText = makeTest.requiredAttributes.Find(e => e.name == AttributeName.NormalTexture);
                                mat.NormalTexture.Source = NormText.value;
                                mat.NormalTexture.TexCoordIndex = 1;
                            }
                            else if (attribute.name == AttributeName.TexCoord1_Short)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordsComponentType =
                                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].TextureCoordSets.Add(attribute.value);

                                var NormText = makeTest.requiredAttributes.Find(e => e.name == AttributeName.NormalTexture);
                                mat.NormalTexture.Source = NormText.value;
                                mat.NormalTexture.TexCoordIndex = 1;
                            }
                            else if (attribute.name == AttributeName.Color_Vector3_Float)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Color_Vector4_Float)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Color_Vector3_Byte)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Color_Vector4_Byte)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Color_Vector3_Short)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = attribute.value;
                            }
                            else if (attribute.name == AttributeName.Color_Vector4_Short)
                            {
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                                wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = attribute.value;
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

                    logs.SetupTable(makeTest,comboIndex, combos);
                }

                logs.WriteOut(makeTest, assetFolder);
            }
            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
