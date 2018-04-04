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

        /// <summary>
        ///  Builds a list of the names of each file in the targeted folder, to be later useded in creating image URIs.
        ///  Only looks at files in folders in the directory, and only one level deep.
        /// </summary>
        public static List<string> FindImageFiles(string imageFolder)
        {
            List<string> images = new List<string>();
            foreach (string folder in Directory.GetDirectories(imageFolder))
            {
                foreach (string image in Directory.GetFiles(folder))
                {
                    images.Add(FormatForUri(Path.Combine(Path.GetFileName(folder), Path.GetFileName(image))));
                }
            }

            return images;
        }

        /// <summary>
        /// Copies images from the resources folder into the Output directory
        /// </summary>
        public static void CopyImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages, bool useThumbnails = false)
        {
            if (usedImages.Count > 0)
            {
                foreach (var image in usedImages)
                {
                    string name = FormatForFileSystem(image.Uri.ToString());

                    var source = Path.Combine(Directory.GetCurrentDirectory(), "Resources", name);
                    var destination = Path.Combine(outputFolder, name);
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                    File.Copy(source, destination, true);
                }
            }

            if (useThumbnails == true)
            {
                CopyThumbnailImageFiles(executingAssembly, outputFolder, usedImages);
            }
        }

        /// <summary>
        /// Starts the copy for the thumbnail for a given list of images
        /// </summary>
        static void CopyThumbnailImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages)
        {
            // Use the list of images to infer the list of thumbnails
            List<Runtime.Image> usedThumbnailImages = DeepCopy.CloneObject(usedImages);

            // Change the file path to one used by the thumbnails
            foreach (var image in usedThumbnailImages)
            {
                image.Uri = FormatForUri(Path.Combine("Figures", "Thumbnails", Path.GetFileName(image.Uri.ToString())));
            }

            // Copy those thumbnails to the destination directory
            CopyImageFiles(executingAssembly, outputFolder, usedThumbnailImages);
        }

        /// <summary>
        /// Replaces '\\' with '/', for use in building a URI for an image
        /// </summary>
        static string FormatForUri(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        /// <summary>
        /// Replaces '/' with '\\', for use in converting a UR back into a useable local path for an image
        /// </summary>
        static string FormatForFileSystem(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
