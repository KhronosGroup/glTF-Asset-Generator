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

            // Create a list of models that need a reference image
            List<string> noRefImageList = new List<string>();
            //{
                //Material_AlphaMask_05.gltf, // Won't load in Babylon.js
                //Mesh_Indices_00.gltf, // NYI
                //Mesh_Indices_01.gltf, // NYI
                //Mesh_Indices_02.gltf, // NYI
                //Mesh_Indices_03.gltf, // NYI
                //Mesh_Indices_04.gltf, // NYI
                //Mesh_Indices_05.gltf, // NYI
                //Mesh_Indices_07.gltf, // NYI
                //Mesh_Indices_08.gltf, // NYI
                //Mesh_Indices_09.gltf, // NYI
                //Mesh_Indices_10.gltf, // NYI
                //Mesh_Indices_11.gltf, // NYI
                //Mesh_Indices_12.gltf, // NYI
                //Compatibility_04.gltf, // Model isn't supposed to load
                //Compatibility_05.gltf, // Model isn't supposed to load
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
            Regex findFileName = new Regex(@"(?<=\\)(.+)");
            foreach (var modelGroup in manifestMaster)
            {
                string assetFolder = Path.Combine(outputFolder, modelGroup.folder);
                foreach (var filename in modelGroup.files)
                {
                    List<Runtime.Image> imageList = new List<Runtime.Image>();
                    string imageFileName = Path.Combine("ReferenceImages", filename.Replace(".gltf", ".png"));
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
