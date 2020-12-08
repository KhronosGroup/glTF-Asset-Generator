using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;

namespace AssetGenerator
{
    internal class Manifest
    {
        public string Folder;
        public int Id;
        public List<Model> Models = new List<Model>();

        // Model group, to be listed in the manifest as the folder name.
        public Manifest(ModelGroupId modelGroupId)
        {
            Folder = modelGroupId.ToString();
            Id = (int)modelGroupId;
        }

        // Model properties to be listed in the manifest.
        public class Model
        {
            public string FileName;
            public bool? Loadable;
            public string SampleImageName;
            public Camera Camera;

            public Model(string name, ModelGroupId modelGroupId, bool noSampleImages, Camera cameraPositioning, bool animated, bool? loadable)
            {
                FileName = name;
                Loadable = loadable;
                if (noSampleImages == false)
                {
                    SampleImageName = $"Figures/SampleImages/{name.Replace(".gltf", (animated ? ".gif" : ".png"))}";
                }

                if (cameraPositioning == null)
                {
                    // Used when a model group has a shared camera position.
                    Camera = CustomCameraList.GetCamera(modelGroupId);
                }
                else
                {
                    // Used when an individual model has a custom camera position.
                    Camera = cameraPositioning;
                }
            }
        }

        // Camera position properties.
        public class Camera
        {
            [JsonConverter(typeof(Vector3ToFloatArrayJsonConverter))]
            public readonly Vector3 Translation = new Vector3();

            public Camera(Vector3 cameraOffset)
            {
                Translation = cameraOffset;
            }
        }

        // Used to track camera properties for model groups that need a custom camera.
        private static class CustomCameraList
        {
            private static List<ModelCameraPairing> customCameras;

            public class ModelCameraPairing
            {
                public Camera camera;
                public ModelGroupId? modelGroup;

                public ModelCameraPairing(Camera cameraSettings, ModelGroupId? modelGroupId)
                {
                    camera = cameraSettings;
                    modelGroup = modelGroupId;
                }
            }

            public static Camera GetCamera(ModelGroupId modelGroup)
            {
                // Checks if the list has been initialized, so it isn't recreated multiple times.
                if (customCameras == null)
                {
                    BuildCameraParings();
                }

                // Searches the list for a matching custom camera
                var custom = customCameras.Find(e => e.modelGroup == modelGroup);
                Camera camera;

                // Use the custom camera if it is found, otherwise use the default camera.
                if (custom == null)
                {
                    camera = customCameras[0].camera;
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
            ///  This is the only section that needs to be changed when needing to use a custom camera for an entire model group.
            /// </summary>
            internal static void BuildCameraParings()
            {
                customCameras = new List<ModelCameraPairing>();

                // Default camera position 
                // Keep this in the first position on the list.
                customCameras.Add(
                    new ModelCameraPairing(
                        new Camera(new Vector3(0, 0, 1.3f)),
                        null)
                        );

                // Node_Attribute
                customCameras.Add(
                    new ModelCameraPairing(
                        new Camera(new Vector3(0, 20, -20)),
                        ModelGroupId.Node_Attribute)
                        );

                // Node_NegativeScale
                customCameras.Add(
                    new ModelCameraPairing(
                        new Camera(new Vector3(0, 20, -20)),
                        ModelGroupId.Node_NegativeScale)
                        );
            }
        }
    }
}
