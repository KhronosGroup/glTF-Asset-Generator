using AssetGenerator.Conversion;
using AssetGenerator.ModelGroups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;

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
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Make an inventory of what images there are.
            var imageList = FileHelper.FindImageFiles(Path.Combine(executingAssemblyFolder, "Resources"));

            // Create an ordered list initializing each model group.
            var positiveTests = new List<ModelGroup>
            {
                new Accessor_Sparse(imageList),
                new Accessor_SparseType(imageList),
                new Animation_Node(imageList),
                new Animation_NodeMisc(imageList),
                new Animation_SamplerType(imageList),
                new Animation_Skin(imageList),
                new Animation_SkinType(imageList),
                new Buffer_Interleaved(imageList),
                new Buffer_Misc(imageList),
                new Compatibility(imageList),
                new Instancing(imageList),
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
                new Mesh_NoPosition(imageList),
            };

            // Retains the manifest from each test type for use in updating the main readme table.
            var mainManifests = new List<List<Manifest>>
            {
                ProcessModelGroups(positiveTests, positiveTestsFolder),
                ProcessModelGroups(negativeTests, negativeTestsFolder),
            };
            ReadmeBuilder.UpdateMainReadme(executingAssembly, mainManifests, Directory.GetParent(outputFolder).ToString(), new string[] { "Positive", "Negative" });

            // Writes a readme explaining the different test types.
            using (var newReadme = new FileStream(Path.Combine(outputFolder, "README.md"), FileMode.Create))
            {
                executingAssembly.GetManifestResourceStream("AssetGenerator.ReadmeTemplates.Page_Output.md").CopyTo(newReadme);
            }

            Console.WriteLine("Model Creation Complete!");
            Console.WriteLine($"Completed in : {TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString()}");

            /// <summary>
            /// Saves each model group to a master manifest as it is created, and writes that manifest to file.
            /// </summary>
            /// <returns>Manifest containing all of the model groups and created.</returns>
            List<Manifest> ProcessModelGroups(List<ModelGroup> modelGroupList, string savePath)
            {
                // Generates the models and saves the model group manifest in with the master manifest.
                var manifests = new List<Manifest>();
                foreach (var modelGroup in modelGroupList)
                {
                    manifests.Add(GenerateModels(modelGroup, savePath));
                }

                // Writes the master manifest to file.
                using (var writeManifest = new StreamWriter(Path.Combine(savePath, "Manifest.json")))
                {
                    writeManifest.Write(JsonSerializer.Serialize(manifests.ToArray(), jsonSerializerOptions));
                }

                return manifests;
            }

            /// <summary>
            /// Create and write all models contained in a model group. Writes a mini-manifest for just the model group to file.
            /// Generates a readme describing the models created. Also copies over required textures and figures.
            /// </summary>
            /// <returns>Manifest containing the model group's models.</returns>
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

                    void Convert(Func<BinaryDataType, BinaryData> getBinaryData)
                    {
                        // Pass the desired properties to the runtime layer, which then coverts that data into
                        // a gltf loader object, ready to create the model.
                        var converter = new Converter(getBinaryData, model.CreateSchemaInstance);
                        glTFLoader.Schema.Gltf gltf = converter.Convert(model.GLTF);

                        // Makes last second changes to the model that bypass the runtime layer.
                        model.PostRuntimeChanges?.Invoke(gltf);

                        // Create the .gltf file and writes the model's data to it.
                        string assetFile = Path.Combine(modelGroupFolder, filename);
                        glTFLoader.Interface.SaveModel(gltf, assetFile);
                    }

                    void WriteBinaryDataFiles(params BinaryData[] bins)
                    {
                        foreach (var bin in bins)
                        {
                            // Write model data to the .bin file.
                            File.WriteAllBytes(Path.Combine(modelGroupFolder, bin.Name), bin.Bytes);
                        }
                    }

                    if (!model.SeparateBuffers)
                    {
                        using (var binaryData = new BinaryData($"{modelGroup.Id}_{comboIndex:00}.bin"))
                        {
                            Convert(type => binaryData);
                            WriteBinaryDataFiles(binaryData);
                        }
                    }
                    else
                    {
                        using (var binaryData = new BinaryData($"{modelGroup.Id}_{comboIndex:00}.bin"))
                        using (var binaryDataAnimation = new BinaryData($"{modelGroup.Id}_Animation_{comboIndex:00}.bin"))
                        {
                            Convert(type => type == BinaryDataType.Animation ? binaryDataAnimation : binaryData);
                            WriteBinaryDataFiles(binaryData, binaryDataAnimation);
                        }
                    }

                    readme.SetupTable(modelGroup, comboIndex, model, Path.GetFileName(savePath));
                    manifest.Models.Add(new Manifest.Model(filename, modelGroup.Id, modelGroup.NoSampleImages, model.Camera, model.Animated, model.Loadable));
                }

                // Write the readme and manifest specific to this model group.
                readme.WriteOut(executingAssembly, modelGroup, modelGroupFolder);
                using (var writeModelGroupManifest = new StreamWriter(Path.Combine(modelGroupFolder, "Manifest.json")))
                {
                    writeModelGroupManifest.Write(JsonSerializer.Serialize(manifest, jsonSerializerOptions));
                }

                return manifest;
            }
        }
    }
}
