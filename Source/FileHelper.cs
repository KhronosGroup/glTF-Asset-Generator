﻿using System;
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

            // Removes files from subfolders
            for (int x = 0; x < images.Count(); x++)
            {

            }

            // Replaces the '.' with a '/', so a useable path is returned 
            for (int x = 0; x < images.Count(); x++)
            {
                Regex regex = new Regex(@"\.(?=.*?\.)");
                images[x] = regex.Replace(images[x], "/");
            }

            return images;
        }

        public static void CopyImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages, 
            string destinationPath = "", string destinationName = "", bool useThumbnails = false)
        {
            if (usedImages.Count > 0)
            {
                // Creates a folder in the model group's output folder for the images
                if (destinationPath == "")
                {
                    if (usedImages[0].Uri.ToString().Contains("Thumbnails"))
                    {
                        Directory.CreateDirectory(Path.Combine(outputFolder, "Figures",
                            Regex.Match(usedImages[0].Uri.ToString().Replace("Resources/", ""), @"(.+)(\/)").ToString()));
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.Combine(outputFolder,
                            Regex.Match(usedImages[0].Uri.ToString().Replace("Resources/", ""), @"(.+)(\/)").ToString()));
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(outputFolder,
                        Regex.Match(destinationPath, @"(.+)\\").ToString()));
                }
                foreach (var image in usedImages)
                {
                    string name;
                    if (destinationPath == "")
                    {
                        // Use the Uri to create a name if a custom one was not provided
                        name = image.Uri.ToString();
                    }
                    else
                    {
                        name = destinationPath;
                    }
                    // Replaces the '/' with a '.', to create the path to the embedded resource
                    Regex formatRegex = new Regex(@"(\/)");
                    string imageSourcePath = "AssetGenerator." + formatRegex.Replace(image.Uri.ToString(), ".");
                    if (!imageSourcePath.Contains("Resources"))
                    {
                        imageSourcePath = imageSourcePath.Replace("AssetGenerator.", "AssetGenerator.Resources.");
                    }
                    if (image.Uri.Contains("Thumbnails") && !image.Uri.Contains("Resources.Figures"))
                    {
                        imageSourcePath = imageSourcePath.Replace("Thumbnails", "Figures.Thumbnails");
                    }
                    string imageDestinationPath = "";
                    if (image.Uri.Contains("Thumbnails"))
                    {
                        imageDestinationPath = Path.Combine(outputFolder, "Figures", formatRegex.Replace(name.Replace("Resources/", ""), Path.DirectorySeparatorChar.ToString(), 1));
                    }
                    else
                    {
                        imageDestinationPath = Path.Combine(outputFolder, formatRegex.Replace(name.Replace("Resources/", ""), Path.DirectorySeparatorChar.ToString(), 1));
                    }
                    

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
                CopyThumbnailImageFiles(executingAssembly, outputFolder, usedImages, destinationPath);
            }
        }

        static void CopyThumbnailImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages,
            string destinationPath = "")
        {
            // Use the list of images to infer the list of thumbnails
            List<Runtime.Image> usedThumbnailImages = new List<Runtime.Image>();
            Regex changePath = new Regex(@".*\/(.*)");
            Regex changeDestination = new Regex(@"(.+)(?=\\)");

            usedThumbnailImages = DeepCopy.CloneObject(usedImages);

            foreach (var image in usedThumbnailImages)
            {
                var match = Regex.Match(image.Uri.ToString(), @"(.+)\/[^\/]*"); // Selects just the containing folder
                image.Uri = image.Uri.ToString().Replace(match.Groups[1].ToString(), "Thumbnails");
            }

            if (destinationPath != "")
            {
                destinationPath = changeDestination.Replace(destinationPath, "Thumbnails", 1);
            }

            // Copy those thumbnails to the destination directory
            CopyImageFiles(executingAssembly, outputFolder, usedThumbnailImages, destinationPath);
        }
    }
}
