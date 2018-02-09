using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

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

        public static List<string> FindImageFiles(Assembly executingAssembly, string resourceFolder)
        {
            // Gets the name of each image, including its expected output folder
            string textureSource = string.Format("{0}.{1}", executingAssembly.GetName().Name, resourceFolder);
            var images = executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(textureSource))
                .Select(r => r.Substring(executingAssembly.GetName().Name.Length + 1))
                .ToList();

            // Replaces the '.' with a '/', so a useable path is returned 
            for (int x = 0; x < images.Count(); x++)
            {
                Regex regex = new Regex(@"(\.)");
                images[x] = regex.Replace(images[x], "/", 1);
            }

            return images;
        }

        public static void CopyImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages, 
            string destinationName = "", bool useThumbnails = false)
        {
            if (usedImages.Count > 0)
            {
                // Creates a folder in the model group's output folder for the images
                Directory.CreateDirectory(Path.Combine(outputFolder, Regex.Match(usedImages[0].Uri.ToString(), @"(.+)(\/)").ToString()));
                foreach (var image in usedImages)
                {
                    string name;
                    if (destinationName == "")
                    {
                        // Use the Uri to create a name if a custom one was not provided
                        name = image.Uri.ToString();
                    }
                    else
                    {
                        name = destinationName;
                    }
                    // Replaces the '/' with a '.', to create the path to the embedded resource
                    Regex formatRegex = new Regex(@"(\/)");
                    string imageSourcePath = "AssetGenerator." + formatRegex.Replace(image.Uri.ToString(), ".", 1);
                    string imageDestinationPath = Path.Combine(outputFolder, formatRegex.Replace(name, "\\", 1));

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

            if (useThumbnails == true)
            {
                CopyThumbnailImageFiles(executingAssembly, outputFolder, usedImages);
            }
        }

        static void CopyThumbnailImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages)
        {
            // Use the list of images to infer the list of thumbnails
            List<Runtime.Image> usedThumbnailImages = new List<Runtime.Image>();
            Regex changePath = new Regex(@"(.*)(?=\/)");
            foreach (var image in usedImages)
            {
                usedThumbnailImages.Add(image);
            }
            foreach (var image in usedThumbnailImages)
            {
                image.Uri = changePath.Replace(image.Uri.ToString(), "Thumbnails", 1);
            }

            // Copy those thumbnails to the destination directory
            CopyImageFiles(executingAssembly, outputFolder, usedThumbnailImages);
        }
    }
}
