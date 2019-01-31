using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AssetGenerator
{
    internal static class NpmHelper
    {
        public static void GenerateScreenshots(List<Manifest> manifest, string manifestPath, string outputFolder)
        {
            var projectDirectory = Directory.GetParent(outputFolder).ToString();
            var resourcesDirectory = Path.Combine(projectDirectory, "Source", "Resources");
            var preGenImagesDirectory = Path.Combine(resourcesDirectory, "Figures", "SampleImages");
            var rootTempDirectory = Path.Combine(projectDirectory, "tempImages");
            var tempOutputDirectory = Path.Combine(rootTempDirectory, "Figures");
            var tempSampleImagesDirectory = Path.Combine(tempOutputDirectory, "SampleImages");
            var tempThumbnailsDirectory = Path.Combine(tempOutputDirectory, "Thumbnails");
            var defaultImage = Path.Combine(resourcesDirectory, "Figures", "NYI.png");
            var screenshotGeneratorDirectory = Path.Combine(projectDirectory, "ScreenshotGenerator");
            var resizeScriptDirectory = Path.Combine(screenshotGeneratorDirectory, "pythonScripts", "dist", "resizeImages.exe");

            Directory.CreateDirectory(tempOutputDirectory);
            Directory.CreateDirectory(tempSampleImagesDirectory);
            Directory.CreateDirectory(tempThumbnailsDirectory);

            // Runs the ScreenshotGenerator.
            Console.WriteLine();
            Console.WriteLine("Running the ScreenshotGenerator");
            var psiNpmStartScreenshotGenerator = new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                WorkingDirectory = screenshotGeneratorDirectory
            };
            var pScreenshotGenerator = Process.Start(psiNpmStartScreenshotGenerator);
            pScreenshotGenerator.StandardInput.WriteLine($"npm start -- \"headless=true\" \"manifest={manifestPath}\" \"outputDirectory={tempSampleImagesDirectory}\" & exit");
            pScreenshotGenerator.WaitForExit();

            // Resizes the images to create thumbnails.
            Console.WriteLine();
            Console.WriteLine("Creating thumbnails...");
            var pResizeImages = Process.Start(resizeScriptDirectory, $"--dir={tempSampleImagesDirectory} --outputDir={tempThumbnailsDirectory} --width=72 --height=72");
            pResizeImages.WaitForExit();

            // Check for images that are pre-generated.
            var preGenImages = FileHelper.FindImageFiles(preGenImagesDirectory);
            var genScreenshots = FileHelper.FindImageFiles(tempSampleImagesDirectory);
            var existingImageList = new List<string>();
            foreach (var genImage in genScreenshots)
            {
                foreach (var preGenImage in preGenImages)
                {
                    if (genImage == preGenImage)
                    {
                        existingImageList.Add(genImage);
                        break;
                    }
                }
            }
            if (existingImageList.Count > 0)
            {
                Console.WriteLine("The following generated image(s) will not be used, due to there already being a pre-generated image.");
                foreach (var preGenImage in existingImageList)
                {
                    Console.WriteLine(preGenImage);
                }
            }

            // Move the sample images and thumbnails into their respective folders.
            Console.WriteLine("Copying images to the Output directory...");
            foreach (var modelgroup in manifest)
            {
                var modelGroupPath = Path.Combine(outputFolder, modelgroup.Folder);

                foreach (var model in modelgroup.Models)
                {
                    if (model.SampleImageName != null)
                    {
                        var thumbnailName = model.SampleImageName.Replace("SampleImages", "Thumbnails");

                        // Builds paths to the expected generated images and their destinations.
                        var imageDestination = Path.Combine(modelGroupPath, model.SampleImageName);
                        var imageThumbnailDestination = Path.Combine(modelGroupPath, thumbnailName);
                        var imageSource = Path.Combine(rootTempDirectory, model.SampleImageName);
                        var imageThumbnailSource = Path.Combine(rootTempDirectory, thumbnailName);

                        // Create the directory if it doesn't exist.
                        Directory.CreateDirectory(Path.GetDirectoryName(imageDestination));
                        Directory.CreateDirectory(Path.GetDirectoryName(imageThumbnailDestination));

                        // Check if there is an pre-gen image, and use that filepath instead if it does.
                        if (existingImageList.Count > 0)
                        {
                            foreach (var preGenImage in existingImageList)
                            {
                                if (preGenImage == Path.GetFileName(model.SampleImageName))
                                {
                                    imageSource = Path.Combine(resourcesDirectory, model.SampleImageName);
                                    imageThumbnailSource = Path.Combine(resourcesDirectory, thumbnailName);
                                    break;
                                }
                            }
                        }

                        if (File.Exists(imageSource) && File.Exists(imageThumbnailSource))
                        {
                            // Copy the image and thumbnail into the relevant folder in the Output directory.
                            File.Copy(imageSource, imageDestination, true);
                            File.Copy(imageThumbnailSource, imageThumbnailDestination, true);
                        }
                        else
                        {
                            // There is no image, so use a copy of the default image instead.
                            File.Copy(defaultImage, imageDestination, true);
                            File.Copy(defaultImage, imageThumbnailDestination, true);
                        }
                    }
                }
            }

            // Delete the temp directory.
            FileHelper.ClearOldFiles(projectDirectory, rootTempDirectory);
        }
    }
}
