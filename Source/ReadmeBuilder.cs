using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AssetGenerator
{
    internal class ReadmeBuilder
    {
        StringBuilder md = new StringBuilder();
        List<List<string>> readmePrereqs = new List<List<string>>();
        List<List<string>> readme = new List<List<string>>();
        List<PropertyName> columnNames = new List <PropertyName>();

        public ReadmeBuilder()
        {

        }

        /// <summary>
        /// Updates the main readme to display which model groups are being generated.
        /// </summary>
        public static void UpdateMainReadme(Assembly executingAssembly, string outputFolder, List<Manifest> manifests)
        {
            // Use the main manifest to build an updated table of contents
            StringBuilder newTableOfContents = new StringBuilder();
            foreach (var modelgroup in manifests)
            {
                string ReadableFolderName = ReadmeStringHelper.GenerateNameWithSpaces(modelgroup.Folder, true);
                newTableOfContents.AppendLine($"- [{ReadableFolderName}](Output/{modelgroup.Folder}/README.md)");
            }

            // Reads the readme file template
            string template;
            string templatePath = "AssetGenerator.ReadmeTemplates.MainPage.md";
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

        /// <summary>
        /// Creates the table of required properties, as well as the column names for the main table.
        /// </summary>
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
                foreach (var property in test.CommonProperties)
                {
                    readmePrereqs.Add(new List<string>
                    {
                        property.ReadmeColumnName,
                        property.ReadmeValue
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

            // Generates the list of column names for use when setting up the table, and generates that part of the table as well.
            foreach (var property in test.Properties)
            {
                var attributeName = property.ReadmeColumnName;

                if (!columnNames.Contains(property.Name))
                {
                    readme[1].Add(attributeName);
                    readme[2].Add(":---:");
                    columnNames.Add(property.Name);
                }
            }
        }

        /// <summary>
        /// Builds the strings used to make the main table for each model group's readme.
        /// </summary>
        public void SetupTable(ModelGroup test, int modelIndex, List<Property> model)
        {
            string modelGroupName = test.Id.ToString();
            string modelNumber = modelIndex.ToString("D2");
            string liveURL = $"https://bghgary.github.io/glTF-Assets-Viewer/?folder={test.Id:D}&model={modelIndex}";

            // Creates a new row for a new model
            List<string> modelInfo = new List<string>
            {
                // Displays the number of the model and is a link to the model
                $"[{modelNumber}]({modelGroupName}_{modelNumber}.gltf)<br>[View]({liveURL})"
            };

            if (test.NoSampleImages == false)
            {
                // Also a sample image in the second cell
                modelInfo.Add($"[<img src=\"Figures/Thumbnails/{modelGroupName}_{modelNumber}.png\" align=\"middle\">](Figures/SampleImages/{modelGroupName}_{modelNumber}.png)");
            }
            readme.Add(modelInfo);

            // Checks the list of properties used in the model against the list of column names. 
            // If there is no property that matches a column name, that cell is left blank. Otherwise the property value is added to that cell.
            // There is no handling for multiple properties with the same name on the same model.
            int logIndex = readme.Count - 1;
            foreach (var possibleAttribute in columnNames)
            {
                var attributeIndex = model.FindIndex(e => e.Name == possibleAttribute);
                if (attributeIndex == -1)
                {
                    readme[logIndex].Add(" ");
                }
                else
                {
                    readme[logIndex].Add(model[attributeIndex].ReadmeValue);
                }
            }
        }

        /// <summary>
        /// Writes the readme to file.
        /// </summary>
        public void WriteOut(Assembly executingAssembly, ModelGroup test, string assetFolder)
        {
            string template;
            string templatePath = $"AssetGenerator.ReadmeTemplates.{test.Id}.md";

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
                        md.AppendLine($"| {String.Join(" | ", line)} |");
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
                    md.AppendLine($"| {String.Join(" | ", line)} |");
                }
            }
            template = template.Replace("~~Table~~", md.ToString());

            // Writes the logs out to file
            var readmeFilePath = Path.Combine(assetFolder, "README.md");
            File.WriteAllText(readmeFilePath, template);
        }
    }
}
