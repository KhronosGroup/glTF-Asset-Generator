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
    }

    public class Model
    {
        public string fileName;
        public string sampleImageName;
        public string sampleThumbnailName;
        public Vector3 cameraPosition;
        public Vector4 cameraRotation;

        public Model(string name, Vector3 pos, Vector4 rot)
        {
            fileName = name;

            sampleImageName = "SampleImages" + '/' + name.Replace(".gltf", ".png");

            sampleThumbnailName = "Thumbnails" + '/' + name.Replace(".gltf", ".png");

            cameraPosition = pos;

            cameraRotation = rot;
        }
    }
}
