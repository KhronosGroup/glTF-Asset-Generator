using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal class MeshPrimitiveVertex
    {
        public Vector3? Position;
        public Vector3? Normal;
        public Vector4? Tangent;
        public Vector4? Color;
        public IEnumerable<Vector2> TextureCoordSet;
        public IEnumerable<VertexJoint> Joints;
        public MeshPrimitiveVertex(Vector3? position = null, Vector3? normal = null, Vector4? tangent = null, Vector4? color = null, IEnumerable<Vector2> textureCoordSet = null, IEnumerable<VertexJoint> joints = null)
        {
            this.Position = position;
            this.Normal = normal;
            this.Tangent = tangent;
            this.Color = color;
            this.TextureCoordSet = textureCoordSet;
            this.Joints = joints;
        }
    }
}
