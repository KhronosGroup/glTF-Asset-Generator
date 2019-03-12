using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            char pathSeparator = Path.DirectorySeparatorChar;
            string outputFolder = Path.GetFullPath(Path.Combine(executingAssemblyFolder, string.Format(@"..{0}..{0}..{0}..{0}Output", pathSeparator)));
            string positiveTestsFolder = Path.Combine(outputFolder, "Positive");
            string negativeTestsFolder = Path.Combine(outputFolder, "Negative");
            var jsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Make an inventory of what images there are.
            var imageList = FileHelper.FindImageFiles(Path.Combine(executingAssemblyFolder, "Resources"));

            // Create a list containing each model group and their initial values.
            var positiveTests = new List<ModelGroup>
            {
                new Animation_Node(imageList),
                new Animation_NodeMisc(imageList),
                new Animation_Skin(imageList),
                new Animation_SkinType(imageList),
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
            var negativeTests = new List<ModelGroup>
            {
                new Mesh_PrimitiveRestart(imageList),
            };

            ProcessModelGroups(positiveTests, positiveTestsFolder, "AssetGenerator.ReadmeTemplates.Page_PositiveTests.md");
            ProcessModelGroups(negativeTests, negativeTestsFolder, "AssetGenerator.ReadmeTemplates.Page_NegativeTests.md");

            using (var newReadme = new FileStream(Path.Combine(outputFolder, "README.md"), FileMode.Create))
            {
                executingAssembly.GetManifestResourceStream("AssetGenerator.ReadmeTemplates.Page_OutputMain.md").CopyTo(newReadme);
            }

            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine($"Completed in : {TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString()}");

            /// <summary>
            /// </summary>
            void ProcessModelGroups(List<ModelGroup> modelGroupList, string savePath, string readmeTemplate)
            {
                var manifests = new List<Manifest>();
                foreach (var modelGroup in modelGroupList)
                {
                    manifests.Add(GenerateModels(modelGroup, savePath));
                }

                using (var writeManifest = new StreamWriter(Path.Combine(savePath, "Manifest.json")))
                {
                    jsonSerializer.Serialize(writeManifest, manifests.ToArray());
                }
                ReadmeBuilder.CreateTestIndexReadme(executingAssembly, savePath, manifests, readmeTemplate);
            }

            /// <summary>
            /// </summary>
            /// <returns></returns>
            Manifest GenerateModels(ModelGroup modelGroup, string savePath)
            {
                var readme = new ReadmeBuilder();
                var manifest = new Manifest(modelGroup.Id);

                string modelGroupFolder = Path.Combine(savePath, modelGroup.Id.ToString());

                Directory.CreateDirectory(modelGroupFolder);

                // Copy all of the images used by the model group into that model group's output directory.
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.UsedFigures);
                FileHelper.CopyImageFiles(executingAssemblyFolder, modelGroupFolder, modelGroup.UsedTextures, useThumbnails: true);

                readme.SetupHeader(modelGroup);

                var numCombos = modelGroup.Models.Count;
                for (var comboIndex = 0; comboIndex < numCombos; comboIndex++)
                {
                    var model = modelGroup.Models[comboIndex];
                    var filename = $"{modelGroup.Id}_{comboIndex:00}.gltf";

                    using (var data = new Data($"{modelGroup.Id}_{comboIndex:00}.bin"))
                    {
                        // Pass the desired properties to the runtime layer, which then coverts that data into
                        // a gltf loader object, ready to create the model.
                        var converter = new Runtime.GLTFConverter { CreateInstanceOverride = model.CreateSchemaInstance };
                        glTFLoader.Schema.Gltf gltf = converter.ConvertRuntimeToSchema(model.GLTF, data);

                        // Makes last second changes to the model that bypass the runtime layer.
                        model.PostRuntimeChanges?.Invoke(gltf);

                        // Create the .gltf file and writes the model's data to it.
                        string assetFile = Path.Combine(modelGroupFolder, filename);
                        glTFLoader.Interface.SaveModel(gltf, assetFile);

                        // Create the .bin file and writes the model's data to it.
                        string dataFile = Path.Combine(modelGroupFolder, data.Name);
                        File.WriteAllBytes(dataFile, data.ToArray());
                    }

                    readme.SetupTable(modelGroup, comboIndex, model.Properties);
                    manifest.Models.Add(new Manifest.Model(filename, modelGroup.Id, modelGroup.NoSampleImages, model.Camera, model.Valid));
                }

                // Write the readme and manifest specific to this model group.
                readme.WriteOut(executingAssembly, modelGroup, modelGroupFolder);
                using (var writeModelGroupManifest = new StreamWriter(Path.Combine(modelGroupFolder, "Manifest.json")))
                {
                    jsonSerializer.Serialize(writeModelGroupManifest, manifest);
                }

                return manifest;
            }
        }
    }
}
