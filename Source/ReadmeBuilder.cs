using System;
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
                newTableOfContents.AppendLine(string.Format("- [{0}](Output/{0}/README.md)", modelgroup.folder));
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
            readme.Add(new List<string>
                {
                    "Index" // First cell is a static header name
                });
            readme.Add(new List<string>
                {
                    ":---:" // Hyphens for row after header 
                });
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
            readme.Add(new List<string> // New row for a new model
                    {
                        // Displays the number of the model and is a link to the model
                        '[' + comboIndex.ToString("D2") + "](" + test.modelGroupName.ToString() + '_' + comboIndex.ToString("D2") + ".gltf)"
                    });
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
                    md.AppendLine(String.Join(" | ", line));
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
                md.AppendLine(String.Join(" | ", line));
            }
            template = template.Replace("~~Table~~", md.ToString());

            // Writes the logs out to file
            var readmeFilePath = Path.Combine(assetFolder, "README.md");
            File.WriteAllText(readmeFilePath, template);
        }
    }
}
