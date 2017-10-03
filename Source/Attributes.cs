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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true)]
    public class ImageAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public ImageAttribute(string name)
        {
            this.Name = name;
        }
    }
}
