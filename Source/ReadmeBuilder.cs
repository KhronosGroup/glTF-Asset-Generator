﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace AssetGenerator
{
    internal class ReadmeBuilder
    {
        StringBuilder md = new StringBuilder();
        List<List<string>> readmePrereqs = new List<List<string>>();
        List<List<string>> readme = new List<List<string>>();
        string lastName = null;

        public ReadmeBuilder()
        {

        }

        public static void UpdateMainReadme(Assembly executingAssembly, string outputFolder, List<Manifest> manifests)
        {
            // Use the main manifest to build an updated table of contents
            StringBuilder newTableOfContents = new StringBuilder();
            foreach (var modelgroup in manifests)
            {
                newTableOfContents.AppendLine(string.Format("- [{0}](Output/{1}/README.md)", 
                    ReadmeStringHelper.GenerateNameWithSpaces(modelgroup.Folder, true), modelgroup.Folder));
            }

            // Reads the readme file template
            string template;
            string templatePath = "AssetGenerator.ReadmeTemplates.README.md";
            using (Stream stream = executingAssembly.GetManifestResourceStream(templatePath))
            using (var streamReader = new StreamReader(stream))
            {
                template = streamReader.ReadToEnd();
            }

            // Find and replace the table of contents section with the newly built one
            template = template.Replace("~~TableOfContents~~", newTableOfContents.ToString());

            // Write out the readme file
            string readmeFilePath = Path.Combine(Directory.GetParent(outputFolder).ToString(), "README.md");
            File.WriteAllText(readmeFilePath, template);
        }

        public void SetupHeader(ModelGroup test)
        {
            // Setup the log file header
            if (test.CommonProperties != null)
            {
                // List attributes that are set in every generated model (prerequisites)
                readmePrereqs.Add(new List<string>()); // First line of table must be blank
                readmePrereqs.Add(new List<string>
                    {
                    "Property", // First cells are a static label
                    "**Values**"
                    });
                readmePrereqs.Add(new List<string>
                    {
                    ":---:", // Hyphens for row after header
                    ":---:",
                    });
                for (int i = 0; i < test.CommonProperties.Count; i++)
                {
                    string attributeName;
                    attributeName = test.CommonProperties[i].ReadmeColumnName;
                    readmePrereqs.Add(new List<string>
                    {
                        attributeName,
                        test.CommonProperties[i].ReadmeValue
                    });
                }
            }

            // Now start the table for generated models
            readme.Add(new List<string>()); // First line of table must be blank

            List<string> firstLine = new List<string>
            {
                " " // First cell is empty
            };
            List<string> secondLine = new List<string>
            {
                ":---:" // Hyphens for rows after header 
            };
            if (test.NoSampleImages == false)
            {
                firstLine.Add("Sample Image"); // The second cell is a static header name
                secondLine.Add(":---:"); // Add another row for the header name
            }
            readme.Add(firstLine);
            readme.Add(secondLine);

            for (int i = 0; i < test.Properties.Count; i++)
            {
                string attributeName;
                attributeName = test.Properties[i].ReadmeColumnName;

                if (attributeName != lastName) // Skip duplicate names caused by non-binary attributes
                {
                    lastName = attributeName;
                    readme[1].Add(attributeName);
                    readme[2].Add(":---:");
                }
            }
        }

        public void SetupTable(ModelGroup test, int modelIndex, List<Property> model)
        {
            string modelGroupName = test.Name.ToString();
            string modelNumber = modelIndex.ToString("D2");
            string liveURL = string.Format("https://bghgary.github.io/glTF-Assets-Viewer/?folder={0}&model={1}",
                test.Id, modelIndex);

            // New row for a new model
            List<string> modelInfo = new List<string>
            {
                // Displays the number of the model and is a link to the model
                string.Format("[{1}]({0}_{1}.gltf)<br>[View]({2})", modelGroupName, modelNumber, liveURL)
            };
            if (test.NoSampleImages == false)
            {
                // Also a sample image in the second cell
                modelInfo.Add(string.Format("[<img src=\"Figures/Thumbnails/{0}_{1}.png\" align=\"middle\">](Figures/SampleImages/{0}_{1}.png)", modelGroupName, modelNumber));
            }
            readme.Add(modelInfo);

            int logIndex = readme.Count - 1;
            List<int> nonBinaryUsed = new List<int>();
            foreach (var possibleAttribute in test.Properties)
            {
                var attributeIndex = model.FindIndex(e => e.Equals(possibleAttribute));
                if (attributeIndex != -1)
                {
                    if (possibleAttribute.PropertyGroup > 0)
                    {
                        var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleAttribute.PropertyGroup);
                        if (alreadyUsed)
                        {
                            // Overwrites the empty cell if a nonbinary of the same time had already been encountered and not used
                            readme[logIndex][readme[logIndex].Count - 1] = possibleAttribute.ReadmeValue;
                        }
                        else
                        {
                            // Creates a new cell, since this nonbinary type had not been encountered before
                            readme[logIndex].Add(possibleAttribute.ReadmeValue);
                            nonBinaryUsed.Add(possibleAttribute.PropertyGroup);
                        }
                    }
                    else
                    {
                        readme[logIndex].Add(possibleAttribute.ReadmeValue);
                    }
                }
                else
                {
                    if (possibleAttribute.PropertyGroup > 0)
                    {
                        var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleAttribute.PropertyGroup);
                        if (!alreadyUsed)
                        {
                            readme[logIndex].Add(" ");
                            nonBinaryUsed.Add(possibleAttribute.PropertyGroup);
                        }
                    }
                    else
                    {
                        readme[logIndex].Add(" ");
                    }
                }
            }
        }

        public void WriteOut(Assembly executingAssembly, ModelGroup test, string assetFolder)
        {
            string template;
            string templatePath = "AssetGenerator.ReadmeTemplates." + test.Name.ToString() + ".md";

            // Reads the template file
            using (Stream stream = executingAssembly.GetManifestResourceStream(templatePath))
            using (var streamReader = new StreamReader(stream))
            {
                template = streamReader.ReadToEnd();
            }

            // If there are required properties, build the header table and inserts it into the template
            if (test.CommonProperties != null)
            {
                foreach (var line in readmePrereqs)
                {
                    if (line.Count > 0)
                    {
                        md.AppendLine("| " + String.Join(" | ", line) + " |");
                    }
                }
                template = template.Replace("~~HeaderTable~~", md.ToString());
                md.Clear();
            }
            else
            {
                template = template.Replace("~~HeaderTable~~", "");
            }

            // Build the table for the test properties and inserts it into the template
            foreach (var line in readme)
            {
                if (line.Count > 0)
                {
                    md.AppendLine("| " + String.Join(" | ", line) + " |");
                }
            }
            template = template.Replace("~~Table~~", md.ToString());

            // Writes the logs out to file
            var readmeFilePath = Path.Combine(assetFolder, "README.md");
            File.WriteAllText(readmeFilePath, template);
        }
    }
}
