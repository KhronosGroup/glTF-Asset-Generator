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

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string executingAssemblyFolder = Path.GetDirectoryName(executingAssembly.Location);
            string outputFolder = Path.GetFullPath(Path.Combine(executingAssemblyFolder, @"..\..\..\..\Output"));
            List<Manifest> manifestMaster = new List<Manifest>();

            // Make an inventory of what images there are
            var textures = FileHelper.FindImageFiles(executingAssembly, "Textures");
            var figures = FileHelper.FindImageFiles(executingAssembly, "Figures");

            // Uses Reflection to create a list containing one instance of each group of models 
            List<dynamic> allModelGroups = new List<dynamic>();
            foreach (var type in executingAssembly.GetTypes())
            {
                var modelGroupAttribute = type.GetCustomAttribute<ModelGroupAttribute>();
                if (modelGroupAttribute != null)
                {
                    ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(List<string>), typeof(List<string>) });
                    dynamic modelGroup = ctor.Invoke(new dynamic[] { textures, figures });
                    allModelGroups.Add(modelGroup);
                }
            }

            foreach (var modelGroup in allModelGroups)
            {
                List<List<Property>> combos = ComboHelper.AttributeCombos(modelGroup);

                ReadmeBuilder readme = new ReadmeBuilder();
                Manifest manifest = new Manifest(modelGroup.modelGroupName);
              
                string assetFolder = Path.Combine(outputFolder, modelGroup.modelGroupName.ToString());

                FileHelper.ClearOldFiles(outputFolder, assetFolder);
                Directory.CreateDirectory(assetFolder);

                // Copy all of the images used by the model group into that model group's output directory
                FileHelper.CopyImageFiles(executingAssembly, assetFolder, modelGroup.usedTextures, useThumbnails: true);
                FileHelper.CopyImageFiles(executingAssembly, assetFolder, modelGroup.usedFigures);


                readme.SetupHeader(modelGroup);

                int numCombos = combos.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    string[] name = ReadmeStringHelper.GenerateName(combos[comboIndex]);

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

                    var geometryData = new Data(modelGroup.modelGroupName.ToString() + "_" + comboIndex.ToString("00") + ".bin");
                    dataList.Add(geometryData);

                    Runtime.GLTF wrapper = Common.SinglePlane();

                    wrapper.Asset = asset;

                    Runtime.Material mat = new Runtime.Material();

                    // Takes the current combo and uses it to bundle together the data for the desired properties 
                    wrapper = modelGroup.SetModelAttributes(wrapper, mat, combos[comboIndex], ref gltf);

                    // Passes the desired properties to the runtime layer, which then coverts that data into
                    // a gltf loader object, ready to create the model
                    Runtime.GLTFConverter.ConvertRuntimeToSchema(wrapper, ref gltf, geometryData);

                    // Makes last second changes to the model that bypass the runtime layer
                    // in order to add 'features that don't really exist otherwise
                    modelGroup.PostRuntimeChanges(combos[comboIndex], ref gltf);

                    // Creates the .gltf file and writes the model's data to it
                    var filename = modelGroup.modelGroupName.ToString() + "_" + comboIndex.ToString("00") + ".gltf";
                    var assetFile = Path.Combine(assetFolder, filename);
                    glTFLoader.Interface.SaveModel(gltf, assetFile);

                    // Creates the .bin file and writes the model's data to it
                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(assetFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    readme.SetupTable(modelGroup, comboIndex, combos);
                    manifest.files.Add(filename);
                }

                readme.WriteOut(executingAssembly, modelGroup, assetFolder);
                manifestMaster.Add(manifest);

                // Write out the manifest JSON specific to this model group
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(manifest, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path.Combine(assetFolder, "Manifest.json"), json);
            }

            // Write out the master manifest JSON containing all of the model groups
            string jsonMaster = Newtonsoft.Json.JsonConvert.SerializeObject(manifestMaster.ToArray(), Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(outputFolder, "Manifest.json"), jsonMaster);

            // Update the main readme
            ReadmeBuilder.UpdateMainReadme(executingAssembly, outputFolder, manifestMaster);

            // Create reference images
            ReferenceImages.Create(executingAssembly, outputFolder, manifestMaster);

            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
