using System;

namespace AssetGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestAttribute : System.Attribute
    {
        public TestNames TestName { get; private set; }

        public TestAttribute(TestNames name)
        {
            this.TestName = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AssetGroupAttribute : System.Attribute
    {
        public string Folder { get; private set; }

        public AssetGroupAttribute(string folder)
        {
            this.Folder = folder;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AssetAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public AssetAttribute(string name)
        {
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple =true)]
    public class ImageAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public ImageAttribute(string name)
        {
            this.Name = name;
        }
    }
}
