using System;

namespace AssetGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AssetGroupAttribute : Attribute
    {
        public string Folder { get; private set; }

        public AssetGroupAttribute(string folder)
        {
            this.Folder = folder;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AssetAttribute : Attribute
    {
        public string Name { get; private set; }

        public AssetAttribute(string name)
        {
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple =true)]
    public class ImageAttribute : Attribute
    {
        public string Name { get; private set; }

        public ImageAttribute(string name)
        {
            this.Name = name;
        }
    }
}
