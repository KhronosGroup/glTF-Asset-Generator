using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AssetGenerator
{
    static public class ReferenceImages
    {
        // Default image used when we don't have a reference image for a model
        const string noRefImage = "Figures/NYI.png";

        public static void Create(Assembly executingAssembly, string outputFolder, List<Manifest> manifestMaster)
        {
            Runtime.Image defaultImage = new Runtime.Image
            {
                Uri = noRefImage
            };

            // Create a list of models that need a reference image
            List<string> noRefImageList = new List<string>();
            //{
            //};
            // We're not creating reference images yet, so there are none to load. Use the default for now
            foreach (var modelGroup in manifestMaster)
            {
                foreach (var filename in modelGroup.files)
                {
                    noRefImageList.Add(filename);
                }
            }

            // List of all of the reference images we have
            List<string> refImageList = FileHelper.FindImageFiles(executingAssembly, "ReferenceImages");

            // Loop through each model group
            // - Create a folder for the reference images
            // - Copy the default reference image into the folder for each model that needs it (requires file rename)
            // - Copy the available reference images into the folder via filehelper
            foreach (var modelGroup in manifestMaster)
            {
                string assetFolder = Path.Combine(outputFolder, modelGroup.folder);
                foreach (var filename in modelGroup.files)
                {
                    List<Runtime.Image> imageList = new List<Runtime.Image>();
                    string imageFileName = Path.Combine("ReferenceImages", filename.Replace(".gltf", ".png"));
                    if (refImageList.Find(e => e == imageFileName) != null)
                    {
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = noRefImage
                        };
                        imageList.Add(image);
                    }
                    else
                    {
                        imageList.Add(defaultImage);
                    }
                    FileHelper.CopyImageFiles(executingAssembly, assetFolder, imageList, imageFileName, useThumbnails: true);
                }
            }
        }
    }
}
