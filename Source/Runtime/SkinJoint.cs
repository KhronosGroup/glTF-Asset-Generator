using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal class SkinJoint : IEquatable<SkinJoint>
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
        public bool Equals(SkinJoint other)
        {
            if (other == null)
            {
                return false;
            }
            else if (other.InverseBindMatrix == InverseBindMatrix && other.Node == Node && other.Skin == Skin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(Object other)
        {
            if (other == null)
            {
                return false;
            }
            else if (!(other is SkinJoint skinJointObj))
            {
                return false;
            }
            else
            {
                return Equals(skinJointObj);
            }
        }
    }
}
