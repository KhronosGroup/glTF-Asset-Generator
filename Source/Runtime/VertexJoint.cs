using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal struct VertexJoint
    {
        public Node Node;
        public int SkinJointIndex;
        public Matrix4x4 InverseBindMatrix;
        public float Weight;
        public VertexJoint(Node node, Matrix4x4 inverseBindMatrix, float weight, int skinJointIndex = 0)
        {
            Node = node;
            InverseBindMatrix = inverseBindMatrix;
            Weight = weight;
            SkinJointIndex = skinJointIndex;
        }

    }
}
