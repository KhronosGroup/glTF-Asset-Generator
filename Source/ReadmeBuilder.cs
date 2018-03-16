﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace AssetGenerator
{
    class ReadmeBuilder
    {
        StringBuilder md = new StringBuilder();
        List<List<string>> readmePrereqs = new List<List<string>>();
        List<List<string>> readme = new List<List<string>>();
        string lastName = null;

        public ReadmeBuilder()
        {

        }

        static public void UpdateMainReadme(Assembly executingAssembly, string outputFolder, List<Manifest> manifests)
        {
            // Use the main manifest to build an updated table of contents
            StringBuilder newTableOfContents = new StringBuilder();
            foreach (var modelgroup in manifests)
            {
                newTableOfContents.AppendLine(string.Format("- [{0}](Output/{1}/README.md)", 
                    ReadmeStringHelper.GenerateNameWithSpaces(modelgroup.folder, true), modelgroup.folder));
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
            if (test.requiredProperty != null)
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
                for (int i = 0; i < test.requiredProperty.Count; i++)
                {
                    string attributeName;
                    attributeName = test.requiredProperty[i].name.ToString();
                    attributeName = ReadmeStringHelper.GenerateNameWithSpaces(attributeName);
                    readmePrereqs.Add(new List<string>
                    {
                    attributeName,
                    ReadmeStringHelper.ConvertTestValueToString(test.requiredProperty[i])
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
            if (test.noSampleImages == false)
            {
                firstLine.Add("Sample Image"); // The second cell is a static header name
                secondLine.Add(":---:"); // Add another row for the header name
            }
            readme.Add(firstLine);
            readme.Add(secondLine);

            for (int i = 0; i < test.properties.Count; i++)
            {
                string attributeName;
                if (test.properties[i].prerequisite != Propertyname.Undefined && test.properties[i].propertyGroup == 0)
                {
                    attributeName = test.properties[i].prerequisite.ToString() + test.properties[i].name.ToString();
                }
                else
                {
                    attributeName = test.properties[i].name.ToString();
                }
                attributeName = ReadmeStringHelper.GenerateNameWithSpaces(attributeName);
                if (attributeName != lastName) // Skip duplicate names caused by non-binary attributes
                {
                    lastName = attributeName;
                    readme[1].Add(attributeName);
                    readme[2].Add(":---:");
                }
            }
        }

        public void SetupTable(ModelGroup test, int comboIndex, List<List<Property>> combos)
        {
            string modelGroupName = test.modelGroupName.ToString();
            string modelNumber = comboIndex.ToString("D2");
            string liveURL = string.Format("https://bghgary.github.io/glTF-Assets-Viewer/?folder={0}&model={1}",
                test.id, comboIndex);

            // New row for a new model
            List<string> modelInfo = new List<string>
            {
                // Displays the number of the model and is a link to the model
                string.Format("[{1}]({0}_{1}.gltf)<br>[View]({2})", modelGroupName, modelNumber, liveURL)
            };
            if (test.noSampleImages == false)
            {
                // Also a sample image in the second cell
                modelInfo.Add(string.Format("[<img src=\"Figures/Thumbnails/{0}_{1}.png\" align=\"middle\">](Figures/SampleImages/{0}_{1}.png)", modelGroupName, modelNumber));
            }
            readme.Add(modelInfo);

            int logIndex = readme.Count - 1;
            List<int> nonBinaryUsed = new List<int>();
            foreach (var possibleAttribute in test.properties)
            {
                var attributeIndex = combos[comboIndex].FindIndex(e =>
                    e.name == possibleAttribute.name &&
                    e.prerequisite == possibleAttribute.prerequisite);
                if (attributeIndex != -1)
                {
                    if (possibleAttribute.propertyGroup > 0)
                    {
                        var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleAttribute.propertyGroup);
                        if (alreadyUsed)
                        {
                            // Overwrites the empty cell if a nonbinary of the same time had already been encountered and not used
                            readme[logIndex][readme[logIndex].Count - 1] = ReadmeStringHelper.ConvertTestValueToString(possibleAttribute);
                        }
                        else
                        {
                            // Creates a new cell, since this nonbinary type had not been encountered before
                            readme[logIndex].Add(ReadmeStringHelper.ConvertTestValueToString(possibleAttribute));
                            nonBinaryUsed.Add(possibleAttribute.propertyGroup);
                        }
                    }
                    else
                    {
                        readme[logIndex].Add(ReadmeStringHelper.ConvertTestValueToString(possibleAttribute));
                    }
                }
                else
                {
                    if (possibleAttribute.propertyGroup > 0)
                    {
                        var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleAttribute.propertyGroup);
                        if (!alreadyUsed)
                        {
                            readme[logIndex].Add(" ");
                            nonBinaryUsed.Add(possibleAttribute.propertyGroup);
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
            string templatePath = "AssetGenerator.ReadmeTemplates." + test.modelGroupName.ToString() + ".md";

            // Reads the template file
            using (Stream stream = executingAssembly.GetManifestResourceStream(templatePath))
            using (var streamReader = new StreamReader(stream))
            {
                template = streamReader.ReadToEnd();
            }

            // If there are required properties, build the header table and inserts it into the template
            if (test.requiredProperty != null)
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
