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
                    images.Add(FormatForUIR(Path.Combine(Path.GetFileName(folder), Path.GetFileName(image))));
                }
            }

            return images;
        }

        public static void CopyImageFiles(Assembly executingAssembly, string outputFolder, List<Runtime.Image> usedImages, 
            string destinationPath = "", string destinationName = "", bool useThumbnails = false)
        {
            if (usedImages.Count > 0)
            {
                // Creates a folder in the model group's output folder for the images
                //if (destinationPath == "")
                //{
                //    //if (usedImages[0].Uri.ToString().Contains("Thumbnails"))
                //    //{
                //    //    Directory.CreateDirectory(Path.Combine(outputFolder, "Figures",
                //    //        Regex.Match(usedImages[0].Uri.ToString().Replace("Resources/", ""), @"(.+)(\/)").ToString()));
                //    //}
                //    //else
                //    //{
                //    //    Directory.CreateDirectory(Path.Combine(outputFolder,
                //    //        Regex.Match(usedImages[0].Uri.ToString().Replace("Resources/", ""), @"(.+)(\/)").ToString()));
                //    //}
                //    Directory.CreateDirectory(Path.Combine(outputFolder, usedImages[0].Uri.ToString()));
                //}
                //else
                //{
                //    //Directory.CreateDirectory(Path.Combine(outputFolder,
                //    //    Regex.Match(destinationPath, @"(.+)\\").ToString()));
                //    Directory.CreateDirectory(Path.Combine(destinationPath, usedImages[0].Uri.ToString()));
                //}

                foreach (var image in usedImages)
                {
                    string name;
                    //if (destinationPath == "")
                    //{
                        // Use the Uri to create a name if a custom one was not provided
                        name = FormatForFilesystem(image.Uri.ToString());
                    //}
                    //else
                    //{
                    //    name = destinationPath;
                    //}



                    // Replaces the '/' with a '.', to create the path to the embedded resource
                    //Regex formatRegex = new Regex(@"(\/)");
                    //string imageSourcePath = "AssetGenerator." + formatRegex.Replace(image.Uri.ToString(), ".");
                    //if (!imageSourcePath.Contains("Resources"))
                    //{
                    //    imageSourcePath = imageSourcePath.Replace("AssetGenerator.", "AssetGenerator.Resources.");
                    //}
                    //if (image.Uri.Contains("Thumbnails") && !image.Uri.Contains("Resources.Figures"))
                    //{
                    //    imageSourcePath = imageSourcePath.Replace("Thumbnails", "Figures.Thumbnails");
                    //}

                    //string imageDestinationPath = "";
                    //if (image.Uri.Contains("Thumbnails"))
                    //{
                    //    imageDestinationPath = Path.Combine(outputFolder, "Figures", formatRegex.Replace(name.Replace("Resources/", ""), Path.DirectorySeparatorChar.ToString(), 1));
                    //}
                    //else
                    //{
                    //    imageDestinationPath = Path.Combine(outputFolder, formatRegex.Replace(name.Replace("Resources/", ""), Path.DirectorySeparatorChar.ToString(), 1));
                    //}


                    //using (Stream stream = executingAssembly.GetManifestResourceStream(imageSourcePath))
                    //var temp = Path.Combine(Directory.GetCurrentDirectory(), "Resources", name);
                    //using (Stream stream = executingAssembly.GetManifestResourceStream(Path.Combine(Directory.GetCurrentDirectory(), name)))
                    //{
                    //    if (stream == null)
                    //    {
                    //        throw new ArgumentException("No such image", image.Uri);
                    //    }
                    //    //using (Stream output = File.OpenWrite(imageDestinationPath))
                    //    using (Stream output = File.OpenWrite(Path.Combine(outputFolder, name)))
                    //    {
                    //        stream.CopyTo(output);
                    //    }
                    //}
                    var source = Path.Combine(Directory.GetCurrentDirectory(), "Resources", name);
                    var destination = Path.Combine(outputFolder, name);
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                    File.Copy(source, destination, true);
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
            //Regex changePath = new Regex(@".*\/(.*)");
            //Regex changeDestination = new Regex(@"(.+)(?=\\)");

            usedThumbnailImages = DeepCopy.CloneObject(usedImages);

            // Change the file path to that used by the thumbnails
            foreach (var image in usedThumbnailImages)
            {
                image.Uri = FormatForUIR(Path.Combine("Figures", "Thumbnails", Path.GetFileName(image.Uri.ToString())));
                //var match = Regex.Match(image.Uri.ToString(), @"(.+)\/[^\/]*"); // Selects just the containing folder
                //image.Uri = image.Uri.ToString().Replace(match.Groups[1].ToString(), "Thumbnails");
            }

            //if (destinationPath != "")
            //{
            //destinationPath = //changeDestination.Replace(destinationPath, "Thumbnails", 1);
            //}

            // Copy those thumbnails to the destination directory
            CopyImageFiles(executingAssembly, outputFolder, usedThumbnailImages, destinationPath);
        }

        /// <summary>
        /// Replaces '\\' with '/', for use in building a URI for an image
        /// </summary>
        static string FormatForUIR(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Replaces '/' with '\\', for use in converting a UR back into a useable local path for an image
        /// </summary>
        static string FormatForFilesystem(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
