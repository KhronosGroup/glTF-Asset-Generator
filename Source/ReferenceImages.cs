using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace AssetGenerator
{
    static public class ReferenceImages
    {
        // Default image used when we don't have a reference image for a model
        const string noRefImage = "ReferenceImages/NYI.png";

        public static void Create(Assembly executingAssembly, string outputFolder, List<Manifest> manifestMaster)
        {
            // Make a list of models that need a reference image

            // Pass that list to the generator (Kacey's code)

            // Loop through each model group
            Runtime.Image defaultNYI = new Runtime.Image
            {
                Uri = noRefImage
            };
            List<Runtime.Image> placeholder = new List<Runtime.Image>()
            {
                defaultNYI
            };

            foreach (var modelGroup in manifestMaster)
            {
                string assetFolder = Path.Combine(outputFolder, modelGroup.folder);
                foreach (var model in modelGroup.files)
                {
                    FileHelper.CopyImageFiles(executingAssembly, assetFolder, placeholder, destinationName: model);
                }

                // - Create a folder for the reference images
                // - Copy the default reference image into the folder for each model that needs it (requires file rename)
                // - Copy the available reference images into the folder via filehelper
            }
        }
    }
}
