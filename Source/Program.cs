using glTFLoader.Schema;
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
            var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");

            foreach (var type in executingAssembly.GetTypes())
            {
                var assetGroupAttribute = type.GetCustomAttribute<AssetGroupAttribute>();
                if (assetGroupAttribute != null)
                {
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        var assetAttribute = method.GetCustomAttribute<AssetAttribute>();
                        var imageAttributes = method.GetCustomAttributes<ImageAttribute>();

                        if (assetAttribute != null)
                        {
                            var gltf = new Gltf
                            {
                                Asset = new Asset
                                {
                                    Generator = "glTF Asset Generator " + Assembly.GetExecutingAssembly().GetName().Version,
                                    Version = "2.0",
                                }
                            };

                            var dataList = new List<Data>();

                            method.Invoke(null, new object[] { assetAttribute.Name, gltf, dataList });

                            var assetFolder = Path.Combine(executingAssemblyFolder, assetGroupAttribute.Folder);
                            Directory.CreateDirectory(assetFolder);

                            foreach(var image in imageAttributes)
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

                            var assetFile = Path.Combine(assetFolder, assetAttribute.Name + ".gltf");
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
    }
}
