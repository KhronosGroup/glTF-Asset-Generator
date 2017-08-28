using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Tests
{
    [TestClass()]
    public class QuaternionTests
    {
        [TestMethod()]
        public void QuaternionTest()
        {
            Quaternion quat = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
            Assert.AreEqual(quat.components.x, 0.0f);
            Assert.AreEqual(quat.components.y, 0.0f);
            Assert.AreEqual(quat.components.z, 0.0f);
            Assert.AreEqual(quat.components.w, 1.0f);
        }

        [TestMethod()]
        public void IdentityTest()
        {
            Quaternion quat_identity = Quaternion.Identity();
            Assert.AreEqual(quat_identity.components.x, 0.0f);
            Assert.AreEqual(quat_identity.components.y, 0.0f);
            Assert.AreEqual(quat_identity.components.z, 0.0f);
            Assert.AreEqual(quat_identity.components.w, 1.0f);
        }
    }
}