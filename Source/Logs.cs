using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace AssetGenerator
{
    class LogBuilder
    {
        StringBuilder csv = new StringBuilder();
        StringBuilder md = new StringBuilder();
        List<List<string>> mdLogPrereqs = new List<List<string>>();
        List<List<string>> mdLog = new List<List<string>>();
        string lastName = null;

        public LogBuilder()
        {

        }

        public void SetupHeader(ModelGroup test)
        {
            // Setup the log file header
            if (test.requiredProperty != null)
            {
                // List attributes that are set in every generated model (prerequisites)
                mdLogPrereqs.Add(new List<string>()); // First line of table must be blank
                mdLogPrereqs.Add(new List<string>
                    {
                    "Property", // First cells are a static label
                    "**Values**"
                    });
                mdLogPrereqs.Add(new List<string>
                    {
                    ":---:", // Hyphens for row after header
                    ":---:",
                    });
                for (int i = 0; i < test.requiredProperty.Count; i++)
                {
                    string attributeName;
                    attributeName = test.requiredProperty[i].name.ToString();
                    attributeName = LogStringHelper.GenerateNameWithSpaces(attributeName);
                    mdLogPrereqs.Add(new List<string>
                    {
                    attributeName,
                    LogStringHelper.ConvertTestValueToString(test.requiredProperty[i])
                    });
                }
            }

            // Now start the table for generated models
            mdLog.Add(new List<string>()); // First line of table must be blank
            mdLog.Add(new List<string>
                {
                    "Index" // First cell is a static header name
                });
            mdLog.Add(new List<string>
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
                attributeName = LogStringHelper.GenerateNameWithSpaces(attributeName);
                if (attributeName != lastName) // Skip duplicate names caused by non-binary attributes
                {
                    lastName = attributeName;
                    mdLog[1].Add(attributeName);
                    mdLog[2].Add(":---:");
                }
            }
        }

        public void SetupTable(ModelGroup test, int comboIndex, List<List<Property>> combos)
        {
            mdLog.Add(new List<string> // New row for a new model
                    {
                        // Displays the number of the model and is a link to the model
                        '[' + comboIndex.ToString("D2") + "](./" + test.modelGroupName.ToString() + '_' + comboIndex.ToString("D2") + ".gltf)"
                    });
            int logIndex = mdLog.Count - 1;
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
                            mdLog[logIndex][mdLog[logIndex].Count - 1] = LogStringHelper.ConvertTestValueToString(possibleAttribute);
                        }
                        else
                        {
                            // Creates a new cell, since this nonbinary type had not been encountered before
                            mdLog[logIndex].Add(LogStringHelper.ConvertTestValueToString(possibleAttribute));
                            nonBinaryUsed.Add(possibleAttribute.propertyGroup);
                        }
                    }
                    else
                    {
                        mdLog[logIndex].Add(LogStringHelper.ConvertTestValueToString(possibleAttribute));
                    }
                }
                else
                {
                    if (possibleAttribute.propertyGroup > 0)
                    {
                        var alreadyUsed = nonBinaryUsed.Exists(x => x == possibleAttribute.propertyGroup);
                        if (!alreadyUsed)
                        {
                            mdLog[logIndex].Add(" ");
                            nonBinaryUsed.Add(possibleAttribute.propertyGroup);
                        }
                    }
                    else
                    {
                        mdLog[logIndex].Add(" ");
                    }
                }
            }

            string[] name = LogStringHelper.GenerateName(combos[comboIndex]);
            var writeToLog = string.Format("{0},{1}", comboIndex, String.Join(" - ", name));
            csv.AppendLine(writeToLog);
        }

        public void WriteOut(Assembly executingAssembly, ModelGroup test, string assetFolder)
        {
            string template;
            string templatePath = "AssetGenerator.LogTemplates." + test.modelGroupName.ToString() + ".md";

            // Reads the template file
            using (Stream stream = executingAssembly.GetManifestResourceStream(templatePath))
            using (var streamReader = new StreamReader(stream))
            {
                template = streamReader.ReadToEnd();
            }

            // If there are required properties, build the header table and inserts it into the template
            if (test.requiredProperty != null)
            {
                foreach (var line in mdLogPrereqs)
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
            foreach (var line in mdLog)
            {
                md.AppendLine(String.Join(" | ", line));
            }
            template = template.Replace("~~Table~~", md.ToString());

            // Writes the logs out to file
            var logFile = Path.Combine(assetFolder, test.modelGroupName.ToString() + "_log.csv");
            File.WriteAllText(logFile, csv.ToString());
            var mdLogFile = Path.Combine(assetFolder, "README.md");
            File.WriteAllText(mdLogFile, template);
        }
    }
}
