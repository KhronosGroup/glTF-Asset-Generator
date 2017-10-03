using System;
using System.IO;
using System.Diagnostics;

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

        public static void CopyImageFiles(string executingAssemblyFolder, string assetFolder, Type t)
        {
            var imageFolder = Path.Combine(executingAssemblyFolder, "ImageDependencies");
            ImageAttribute[] usedImages = (ImageAttribute[])Attribute.GetCustomAttributes(t, typeof(ImageAttribute));
            if (usedImages != null)
            {
                foreach (var image in usedImages)
                {
                    if (File.Exists(Path.Combine(imageFolder, image.Name)))
                    {
                        File.Copy(Path.Combine(imageFolder, image.Name), Path.Combine(assetFolder, image.Name), true);
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
