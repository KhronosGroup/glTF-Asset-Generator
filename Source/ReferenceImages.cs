using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

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

            // List of all of the reference images we have
            List<string> refImageList = FileHelper.FindImageFiles(executingAssembly, "ReferenceImages");

            // Loop through each model group
            // - Create a folder for the reference images
            // - Copy the default reference image into the folder for each model that needs it (requires file rename)
            // - Copy the available reference images into the folder via filehelper
            Regex findFileName = new Regex(@"(?<=\\)(.+)");
            foreach (var modelGroup in manifestMaster)
            {
                string assetFolder = Path.Combine(outputFolder, modelGroup.folder);
                foreach (var model in modelGroup.models)
                {
                    List<Runtime.Image> imageList = new List<Runtime.Image>();
                    string imageFileName = Path.Combine("ReferenceImages", model.fileName.Replace(".gltf", ".png"));
                    string refImageURI = refImageList.Find(e => e.Contains(findFileName.Match(imageFileName).ToString()));
                    if (refImageURI != null)
                    {
                        Runtime.Image image = new Runtime.Image
                        {
                            Uri = refImageURI
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
