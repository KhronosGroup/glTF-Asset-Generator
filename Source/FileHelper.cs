using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

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

        public static void CopyImageFiles(Assembly executingAssembly, string executingAssemblyFolder, string assetFolder, List<Runtime.Image> usedImages)
        {
            var imageFolder = Path.Combine(executingAssemblyFolder, "Tests");
            if (usedImages.Count > 0)
            {
                foreach (var image in usedImages)
                {
                    // Reads the template file
                    string imageSourcePath = "AssetGenerator.Tests." + image.Uri;
                    string imageDestinationPath = Path.Combine(assetFolder, image.Uri);
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
