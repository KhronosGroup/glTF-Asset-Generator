using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace AssetGenerator
{
    public static class FileHelper
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

        public static void CopyImageFiles(string executingAssemblyFolder, string assetFolder, List<Runtime.Image> usedImages)
        {
            var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");
            if (usedImages.Count > 0)
            {
                foreach (var image in usedImages)
                {
                    if (File.Exists(Path.Combine(imageFolder, image.Uri)))
                    {
                        File.Copy(Path.Combine(imageFolder, image.Uri), Path.Combine(assetFolder, image.Uri), true);
                    }
                    else
                    {
                        Debug.WriteLine(imageFolder + " does not exist");
                    }
                }
            }
        }
    }
}
