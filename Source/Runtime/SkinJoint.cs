using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class SkinJoint
    {
        public Matrix4x4 InverseBindMatrix;
        public Node Node;

        public SkinJoint(Matrix4x4 inverseBindMatrix, Node node)
        {
            InverseBindMatrix = inverseBindMatrix;
            Node = node;
        }
    }
}
