using System;

namespace AssetGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    class AssetGroupAttribute : Attribute
    {
        public string Folder { get; private set; }

        public AssetGroupAttribute(string folder)
        {
            this.Folder = folder;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    class AssetAttribute : Attribute
    {
        public string Name { get; private set; }

        public AssetAttribute(string name)
        {
            this.Name = name;
        }
    }
}
