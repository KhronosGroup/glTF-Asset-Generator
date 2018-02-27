using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.IO;

namespace AssetGenerator
{
    public class Manifest
    {
        public string folder;
        public List<Model> models = new List<Model>();

        public Manifest(ModelGroupName name)
        {
            folder = name.ToString();
        }

        public class Model
        {
            public string fileName;
            public string sampleImageName;
            public string sampleThumbnailName;
            public Camera camera;

            public Model(string name, Vector3 pos, Vector4 rot)
            {
                fileName = name;
                sampleImageName = "SampleImages" + '/' + name.Replace(".gltf", ".png");
                sampleThumbnailName = "Thumbnails" + '/' + name.Replace(".gltf", ".png");
                camera = new Camera(pos, rot);
            }
        }

        public class Camera
        {
            public float[] cameraPosition = new float[3];
            public float[] cameraRotation = new float[4];

            public Camera(Vector3 pos, Vector4 rot)
            {
                pos.CopyTo(cameraPosition);
                rot.CopyTo(cameraRotation);
            }
        }
    }
}
