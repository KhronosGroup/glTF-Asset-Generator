using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal class SkinJoint
    {
        public Matrix4x4 InverseBindMatrix;
        public Node Node;
        public Skin Skin;

        public SkinJoint(Matrix4x4 inverseBindMatrix, Node node, Skin skin)
        {
            InverseBindMatrix = inverseBindMatrix;
            Node = node;
            Skin = skin;
        }
    }
}
