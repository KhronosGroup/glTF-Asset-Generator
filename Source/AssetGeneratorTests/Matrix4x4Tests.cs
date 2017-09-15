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
    public class Matrix4x4Tests
    {
        [TestMethod()]
        public void Matrix4x4Test()
        {
            Matrix4x4 mat = new Matrix4x4(
                new Vector4<float>(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector4<float>(0.0f, 2.0f, 0.0f, 0.0f),
                new Vector4<float>(0.0f, 0.0f, 3.0f, 0.0f),
                new Vector4<float>(0.0f, 0.0f, 0.0f, 4.0f)
                );

            Assert.AreEqual(mat.Rows[0], new Vector4<float>(1.0f, 0.0f, 0.0f, 0.0f));
            Assert.AreEqual(mat.Rows[1], new Vector4<float>(0.0f, 2.0f, 0.0f, 0.0f));
            Assert.AreEqual(mat.Rows[2], new Vector4<float>(0.0f, 0.0f, 3.0f, 0.0f));
            Assert.AreEqual(mat.Rows[3], new Vector4<float>(0.0f, 0.0f, 0.0f, 4.0f));
        }

        [TestMethod()]
        public void IdentityTest()
        {
            Matrix4x4 identityMatrix = Matrix4x4.Identity();
            Assert.AreEqual(identityMatrix.Rows[0], new Vector4<float>(1.0f, 0.0f, 0.0f, 0.0f));
            Assert.AreEqual(identityMatrix.Rows[1], new Vector4<float>(0.0f, 1.0f, 0.0f, 0.0f));
            Assert.AreEqual(identityMatrix.Rows[2], new Vector4<float>(0.0f, 0.0f, 1.0f, 0.0f));
            Assert.AreEqual(identityMatrix.Rows[3], new Vector4<float>(0.0f, 0.0f, 0.0f, 1.0f));

        }
        [TestMethod()]
        public void ToArrayTest()
        {
            Matrix4x4 mat = Matrix4x4.Identity();
            float[] entries = mat.ToArray();
            float[] entriesComparison = 
            {
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            };
            Assert.IsTrue(entries.SequenceEqual(entriesComparison));
            
        }

    }
}