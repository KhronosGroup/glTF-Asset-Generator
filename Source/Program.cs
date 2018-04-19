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
            var pathSeparator = Path.DirectorySeparatorChar;
            string outputFolder = Path.GetFullPath(Path.Combine(executingAssemblyFolder, String.Format(@"..{0}..{0}..{0}..{0}Output", pathSeparator)));
            List<Manifest> manifestMaster = new List<Manifest>();

            // Make an inventory of what images there are
            var imageList = FileHelper.FindImageFiles(Path.Combine(executingAssemblyFolder, "Resources"));

            // Create a list containing each model group and their initial values
            List<ModelGroup> allModelGroups = new List<ModelGroup>()
            {
                new Material(imageList),
            };

            var modelGroupIndex = 0;
            foreach (var modelGroup in allModelGroups)
            {
                ReadmeBuilder readme = new ReadmeBuilder();
                modelGroup.Id = modelGroupIndex++;
                Manifest manifest = new Manifest(modelGroup.Name);
              
                string modelGroupFolder = Path.Combine(outputFolder, modelGroup.Name.ToString());

                //FileHelper.ClearOldFiles(outputFolder, modelGroupFolder);
                Directory.CreateDirectory(modelGroupFolder);

                // Copy all of the images used by the model group into that model group's output directory
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.UsedFigures);
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.UsedTextures, useThumbnails: true);

                readme.SetupHeader(modelGroup);

                int numCombos = modelGroup.Models.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    Runtime.GLTF gltf = modelGroup.Models[comboIndex].GLTF;
                    //var asset = new Runtime.Asset
                    //{
                    //    Generator = "glTF Asset Generator",
                    //    Version = "2.0",
                    //};

                    var schemaGltf = new glTFLoader.Schema.Gltf
                    {
                        Asset = gltf.Asset.ConvertToSchema()
                    };

                    var dataList = new List<Data>();

                    var geometryData = new Data(modelGroup.Name.ToString() + "_" + comboIndex.ToString("00") + ".bin");
                    dataList.Add(geometryData);

                    // Takes the current combo and uses it to bundle together the data for the desired properties 
                    //Runtime.GLTF gltf = modelGroup.Models[comboIndex].CreateModel();
                    //gltf.Asset = asset;

                    // Passes the desired properties to the runtime layer, which then coverts that data into
                    // a gltf loader object, ready to create the model
                    Runtime.GLTFConverter.ConvertRuntimeToSchema(gltf, ref schemaGltf, geometryData);

                    // Makes last second changes to the model that bypass the runtime layer
                    // in order to add 'features that don't really exist otherwise
                    //modelGroup.PostRuntimeChanges(modelGroup.combos[comboIndex], ref schemaGltf);

                    // Creates the .gltf file and writes the model's data to it
                    var filename = modelGroup.Name.ToString() + "_" + comboIndex.ToString("00") + ".gltf";
                    var assetFile = Path.Combine(modelGroupFolder, filename);
                    glTFLoader.Interface.SaveModel(schemaGltf, assetFile);

                    // Creates the .bin file and writes the model's data to it
                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(modelGroupFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    //readme.SetupTable(modelGroup, comboIndex, modelGroup.models);
                    manifest.models.Add(
                        new Manifest.Model(filename, modelGroup.Name, modelGroup.NoSampleImages));
                }

                readme.WriteOut(executingAssembly, modelGroup, modelGroupFolder);
                manifestMaster.Add(manifest);

                // Write out the manifest JSON specific to this model group
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(manifest, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path.Combine(modelGroupFolder, "Manifest.json"), json);
            }

            // Write out the master manifest JSON containing all of the model groups
            string jsonMaster = Newtonsoft.Json.JsonConvert.SerializeObject(manifestMaster.ToArray(), Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(outputFolder, "Manifest.json"), jsonMaster);

            // Update the main readme
            ReadmeBuilder.UpdateMainReadme(executingAssembly, outputFolder, manifestMaster);

            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
