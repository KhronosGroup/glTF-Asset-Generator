using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime.Tests
{
    [TestClass()]
    public class ImageTests
    {
        [TestMethod()]
        public void ConvertToImageTest()
        {
            
            Runtime.Image rImage = new Runtime.Image();
            rImage.Uri = "test.png";
            glTFLoader.Schema.Image image = rImage.ConvertToImage();
            Assert.IsTrue(image.Uri.Equals(rImage.Uri));
        }
    }
}