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
    public class SamplerTests
    {
        [TestMethod()]
        public void ConvertToSamplerTest()
        {
            Runtime.Sampler rsampler = new Runtime.Sampler();
            glTFLoader.Schema.Sampler sampler = rsampler.ConvertToSampler();
        }

    }
}