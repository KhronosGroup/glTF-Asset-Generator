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
    public class SceneTests
    {
        [TestMethod()]
        public void SceneTest()
        {
            Scene scene = new Scene();
        }

        [TestMethod()]
        public void AddMeshTest()
        {
            Runtime.Scene scene = new Scene();
            scene.AddMesh(new Runtime.Mesh());
        }
    }
}