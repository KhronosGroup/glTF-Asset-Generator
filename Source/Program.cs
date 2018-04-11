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
                new ModelGroup(new Material(imageList)),
            };

            // Uses Reflection to create a list containing one instance of each group of models 
            //List<dynamic> allModelGroups = new List<dynamic>();
            //foreach (var type in executingAssembly.GetTypes())
            //{
            //    var modelGroupAttribute = type.GetCustomAttribute<ModelGroupAttribute>();
            //    if (modelGroupAttribute != null)
            //    {
            //        ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(List<string>) });
            //        dynamic modelGroup = ctor.Invoke(new dynamic[] { imageList });
            //        allModelGroups.Add(modelGroup);
            //    }
            //}

            var modelGroupIndex = 0;
            foreach (var modelGroup in allModelGroups)
            {
                // Creates the combos if the model group is still using automatic combos
                //if (modelGroup.combos.Count < 1)
                //{
                //    modelGroup.combos = ComboHelper.AttributeCombos(modelGroup);
                //}

                ReadmeBuilder readme = new ReadmeBuilder();
                modelGroup.id = modelGroupIndex++;
                Manifest manifest = new Manifest(modelGroup.modelGroupName);
              
                string modelGroupFolder = Path.Combine(outputFolder, modelGroup.modelGroupName.ToString());

                //FileHelper.ClearOldFiles(outputFolder, modelGroupFolder);
                Directory.CreateDirectory(modelGroupFolder);

                // Copy all of the images used by the model group into that model group's output directory
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.usedFigures);
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.usedTextures, useThumbnails: true);

                readme.SetupHeader(modelGroup);

                int numCombos = modelGroup.combos.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    string[] name = ReadmeStringHelper.GenerateName(modelGroup.combos[comboIndex]);

                    var asset = new Runtime.Asset
                    {
                        Generator = "glTF Asset Generator",
                        Version = "2.0",
                        //Extras = new Runtime.Extras
                        //{
                        //    // Inserts a string into the .gltf containing the properties that are set for a given model, for debug.
                        //    Attributes = String.Join(" - ", name)
                        //}
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
                    wrapper = modelGroup.SetModelAttributes(wrapper, mat, modelGroup.combos[comboIndex], ref gltf);

                    // Passes the desired properties to the runtime layer, which then coverts that data into
                    // a gltf loader object, ready to create the model
                    Runtime.GLTFConverter.ConvertRuntimeToSchema(wrapper, ref gltf, geometryData);

                    // Makes last second changes to the model that bypass the runtime layer
                    // in order to add 'features that don't really exist otherwise
                    modelGroup.PostRuntimeChanges(modelGroup.combos[comboIndex], ref gltf);

                    // Creates the .gltf file and writes the model's data to it
                    var filename = modelGroup.modelGroupName.ToString() + "_" + comboIndex.ToString("00") + ".gltf";
                    var assetFile = Path.Combine(modelGroupFolder, filename);
                    glTFLoader.Interface.SaveModel(gltf, assetFile);

                    // Creates the .bin file and writes the model's data to it
                    foreach (var data in dataList)
                    {
                        data.Writer.Flush();

                        var dataFile = Path.Combine(modelGroupFolder, data.Name);
                        File.WriteAllBytes(dataFile, ((MemoryStream)data.Writer.BaseStream).ToArray());
                    }

                    readme.SetupTable(modelGroup, comboIndex, modelGroup.combos);
                    manifest.models.Add(
                        new Manifest.Model(filename, modelGroup.modelGroupName, modelGroup.noSampleImages));
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
