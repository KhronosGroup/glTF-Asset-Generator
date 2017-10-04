using System;

namespace AssetGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestAttribute : System.Attribute
    {
        //public TestName Name { get; private set; }

        //public TestAttribute(TestName name)
        //{
        //    this.Name = name;
        //}
    }

    //[AttributeUsage(AttributeTargets.Class, AllowMultiple =true)]
    //public class ImageAttribute : System.Attribute
    //{
    //    public string Name { get; private set; }

    //    public ImageAttribute(string name)
    //    {
    //        this.Name = name;
    //    }
    //}
}
