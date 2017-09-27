using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.TestNames
{
    [TestClass()]
    public class QuaternionTests
    {
        [TestMethod()]
        public void QuaternionTest()
        {
            Quaternion quat = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
            Assert.AreEqual(quat.Components.x, 0.0f);
            Assert.AreEqual(quat.Components.y, 0.0f);
            Assert.AreEqual(quat.Components.z, 0.0f);
            Assert.AreEqual(quat.Components.w, 1.0f);
        }

        [TestMethod()]
        public void IdentityTest()
        {
            Quaternion quat_identity = Quaternion.Identity();
            Assert.AreEqual(quat_identity.Components.x, 0.0f);
            Assert.AreEqual(quat_identity.Components.y, 0.0f);
            Assert.AreEqual(quat_identity.Components.z, 0.0f);
            Assert.AreEqual(quat_identity.Components.w, 1.0f);
        }
    }
}