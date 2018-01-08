using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AssetGenerator
{
    internal static class FileHelper
    {
        public static void ClearOldFiles(string executingAssemblyFolder, string assetFolder)
        {
            var trashFolder = Path.Combine(executingAssemblyFolder, "Delete");
            bool tryAgain = true;
            while (tryAgain)
            {
                try
                {
                    Directory.Move(assetFolder, trashFolder);
                    Directory.Delete(trashFolder, true);
                    tryAgain = false;
                }
                catch (DirectoryNotFoundException)
                {
                    // Do nothing
                    tryAgain = false;
                }
                catch (IOException)
                {
                    Console.WriteLine("Unable to delete the directory.");
                    Console.WriteLine("Verify that there are no open files and that the current user has write permission to that directory.");
                    Console.WriteLine("Press any key to try again.");
                    Console.ReadKey();
                    tryAgain = true;
                }
            }
        }

        public static void CopyImageFiles(Assembly executingAssembly, string executingAssemblyFolder,
            string outputFolder, List<Runtime.Image> usedImages)
        {
            if (usedImages.Count > 0)
            {
                Directory.CreateDirectory(outputFolder);
                foreach (var image in usedImages)
                {
                    // Removes part of the string starting at the beginning and ending with the first /
                    string imageFileName = Regex.Replace(image.Uri.ToString(), @"(.+?)(?<=/)", "");
                    string imageSourcePath = "AssetGenerator.Images." + imageFileName;
                    string imageDestinationPath = Path.Combine(outputFolder, imageFileName);

                    using (Stream stream = executingAssembly.GetManifestResourceStream(imageSourcePath))
                    {
                        if (stream == null)
                        {
                            throw new ArgumentException("No such image", image.Uri);
                        }
                        using (Stream output = File.OpenWrite(imageDestinationPath))
                        {
                            stream.CopyTo(output);
                        }
                    }
                }
            }
        }
    }
}
