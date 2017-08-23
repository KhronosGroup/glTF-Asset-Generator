using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetGenerator;
using glTFLoader.Schema;
using static AssetGenerator.GLTFWrapper;
using System.Reflection;
using System.IO;

namespace AssetGenerator
{
    
    class Program
    {
        
        static void Main(string[] args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyFolder = Path.GetDirectoryName(executingAssembly.Location);

            foreach (var type in executingAssembly.GetTypes())
            {
                System.Diagnostics.Debug.WriteLine(type.Name);
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
                                    Generator = "glTF Asset Generator Examples" + Assembly.GetExecutingAssembly().GetName().Version,
                                    Version = "2.0",
                                }
                            };

                            var dataList = new List<Data>();
                            System.Diagnostics.Debug.WriteLine(method.Name);

                            method.Invoke(null, new object[] { assetAttribute.Name, gltf, dataList });

                            var assetFolder = Path.Combine(executingAssemblyFolder, assetGroupAttribute.Folder, assetAttribute.Name);
                            Directory.CreateDirectory(assetFolder);
                            var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");
                            System.Diagnostics.Debug.WriteLine(imageFolder);


                            foreach(var image in imageAttributes)
                            {
                                System.Diagnostics.Debug.WriteLine(image.Name);
                                System.Diagnostics.Debug.WriteLine(image.Name);
                                System.Diagnostics.Debug.WriteLine(Path.Combine(imageFolder, image.Name));
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
