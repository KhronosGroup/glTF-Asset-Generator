using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
            var jsonSerializer = new Newtonsoft.Json.JsonSerializer
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            // Make an inventory of what images there are
            var imageList = FileHelper.FindImageFiles(Path.Combine(executingAssemblyFolder, "Resources"));

            // Create a list containing each model group and their initial values
            List<ModelGroup> allModelGroups = new List<ModelGroup>()
            {
                new Buffer_Interleaved(imageList),
                new Compatibility(imageList),
                new Material(imageList),
                new Material_AlphaBlend(imageList),
                new Material_AlphaMask(imageList),
                new Material_DoubleSided(imageList),
                new Material_MetallicRoughness(imageList),
                new Material_Mixed(imageList),
                new Material_SpecularGlossiness(imageList),
                new Mesh_PrimitiveAttribute(imageList),
                new Mesh_PrimitiveMode(imageList),
                new Mesh_PrimitiveVertexColor(imageList),
                new Mesh_Primitives(imageList),
                new Mesh_PrimitivesUV(imageList),
                new Node_Attribute(imageList),
                new Node_NegativeScale(imageList),
                new Texture_Sampler(imageList),
            };

            var modelGroupIndex = 0;
            foreach (var modelGroup in allModelGroups)
            {
                ReadmeBuilder readme = new ReadmeBuilder();
                modelGroup.Id = modelGroupIndex++;
                Manifest manifest = new Manifest(modelGroup.Name);
              
                string modelGroupFolder = Path.Combine(outputFolder, modelGroup.Name.ToString());

                Directory.CreateDirectory(modelGroupFolder);

                // Copy all of the images used by the model group into that model group's output directory
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.UsedFigures);
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.UsedTextures, useThumbnails: true);

                readme.SetupHeader(modelGroup);

                int numCombos = modelGroup.Models.Count;
                for (int comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    var model = modelGroup.Models[comboIndex];

                    Runtime.GLTF gltf = model.GLTF;

                    var dataList = new List<Data>();

                    var geometryData = new Data(modelGroup.Name.ToString() + "_" + comboIndex.ToString("00") + ".bin");
                    dataList.Add(geometryData);

                    // Passes the desired properties to the runtime layer, which then coverts that data into
                    // a gltf loader object, ready to create the model
                    var converter = new Runtime.GLTFConverter { CreateInstanceOverride = model.CreateSchemaInstance };
                    var schemaGltf = converter.ConvertRuntimeToSchema(gltf, geometryData);

                    // Makes last second changes to the model that bypass the runtime layer
                    // in order to add features that don't really exist otherwise
                    model.PostRuntimeChanges(schemaGltf);

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

                    readme.SetupTable(modelGroup, comboIndex, model.Properties);
                    manifest.Models.Add(new Manifest.Model(filename, modelGroup.Name, modelGroup.NoSampleImages));
                }

                readme.WriteOut(executingAssembly, modelGroup, modelGroupFolder);
                manifestMaster.Add(manifest);

                // Write out the manifest JSON specific to this model group
                using (StreamWriter writeModelGroupManifest = new StreamWriter(Path.Combine(modelGroupFolder, "Manifest.json")))
                    jsonSerializer.Serialize(writeModelGroupManifest, manifest);
            }

            // Write out the master manifest JSON containing all of the model groups
            using (StreamWriter writeManifest = new StreamWriter(Path.Combine(outputFolder, "Manifest.json")))
                jsonSerializer.Serialize(writeManifest, manifestMaster.ToArray());

            // Update the main readme
            ReadmeBuilder.UpdateMainReadme(executingAssembly, outputFolder, manifestMaster);

            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine("Completed in : " + TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString());
        }
    }
}
