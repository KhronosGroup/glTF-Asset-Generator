using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    public class Manifest
    {
        public string folder;
        public List<Model> models = new List<Model>();

        // Model group, to be listed in the manifest as the folder name
        public Manifest(ModelGroupName name)
        {
            folder = name.ToString();
        }

        // Model properties to be listed in the manifest
        public class Model
        {
            public string fileName;
            public string sampleImageName;
            //public string sampleThumbnailName;
            public Camera camera;

            public Model(string name, ModelGroupName modelGroupName)
            {
                fileName = name;
                sampleImageName = "SampleImages" + '/' + name.Replace(".gltf", ".png");
                //sampleThumbnailName = "Thumbnails" + '/' + name.Replace(".gltf", ".png");
                camera = CustomCameraList.GetCamera(modelGroupName);
            }
        }

        // Camera properties
        public class Camera
        {
            public float[] translation = new float[3];

            public Camera(Vector3 cameratranslation)
            {
                cameratranslation.CopyTo(translation);
            }
        }

        // Used to track camera properties for model groups that need a custom camera
        internal static class CustomCameraList
        {
            internal static List<ModelCameraPairing> customCameraList;

            internal class ModelCameraPairing
            {
                internal Camera camera;
                internal ModelGroupName modelGroup;

                internal ModelCameraPairing(Camera cameraSettings, ModelGroupName name)
                {
                    camera = cameraSettings;
                    modelGroup = name;
                }
            }

            internal static Camera GetCamera(ModelGroupName modelGroup)
            {
                // Checks if the list has been initialized, so it isn't recreated multiple times.
                if (customCameraList == null)
                {
                    BuildCameraParings();
                }

                // Searches the list for a matching custom camera
                var custom = customCameraList.Find(e => e.modelGroup == modelGroup);
                Camera camera;

                // Use the custom camera if it is found, otherwise use the default camera
                if (custom == null)
                {
                    camera = customCameraList[0].camera;
                }
                else
                {
                    camera = custom.camera;
                }

                return camera;
            }

            /// <summary>
            ///  Contains the values used for the camera when creating sample images of the models. They all use the first
            ///  by default, but adding in another section below will cause all models in that model group to use that camera.
            ///  This is the only section that will need to be changed when needing to use a custom camera for the sample images.
            /// </summary>
            internal static void BuildCameraParings()
            {
                customCameraList = new List<ModelCameraPairing>();

                // Default camera position. Keep this in the first position on the list.
                customCameraList.Add(
                    new ModelCameraPairing(
                        new Camera(new Vector3(0, 0, 1.3f)),
                        ModelGroupName.Undefined)
                        );

                // Node_Attribute
                customCameraList.Add(
                    new ModelCameraPairing(
                        new Camera(new Vector3(0, 20, -20)),
                        ModelGroupName.Node_Attribute)
                        );

                // Node_NegativeScale
                customCameraList.Add(
                    new ModelCameraPairing(
                        new Camera(new Vector3(0, 20, -20)),
                        ModelGroupName.Node_NegativeScale)
                        );
            }
        }
    }
}
