using System.Numerics;
using static glTFLoader.Schema.MeshPrimitive;

namespace AssetGenerator.Runtime
{
    internal enum MeshPrimitiveMode
    {
        Points = ModeEnum.POINTS,
        Lines = ModeEnum.LINES,
        LineLoop = ModeEnum.LINE_LOOP,
        LineStrip = ModeEnum.LINE_STRIP,
        Triangles = ModeEnum.TRIANGLES,
        TriangleStrip = ModeEnum.TRIANGLE_STRIP,
        TriangleFan = ModeEnum.TRIANGLE_FAN,
    }

    internal class JointVector
    {
        public int Index0 { get; }
        public int Index1 { get; }
        public int Index2 { get; }
        public int Index3 { get; }

        public JointVector(int index0, int index1 = 0, int index2 = 0, int index3 = 0)
        {
            Index0 = index0;
            Index1 = index1;
            Index2 = index2;
            Index3 = index3;
        }
    }

    internal class WeightVector
    {
        public float Value0 { get; }
        public float Value1 { get; }
        public float Value2 { get; }
        public float Value3 { get; }

        public WeightVector(float value0, float value1 = 0.0f, float value2 = 0.0f, float value3 = 0.0f)
        {
            Value0 = value0;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }
    }

    internal class MeshPrimitive
    {
        public Material Material { get; set; }

        public MeshPrimitiveMode Mode { get; set; } = MeshPrimitiveMode.Triangles;
        public Data<int> Indices { get; set; }

        public bool? Interleave { get; set; }
        public Data<Vector3> Positions { get; set; }
        public Data<Vector3> Normals { get; set; }
        public Data<Vector4> Tangents { get; set; }
        public Data Colors { get; set; }
        public Data<Vector2> TexCoords0 { get; set; }
        public Data<Vector2> TexCoords1 { get; set; }
        public Data<JointVector> Joints { get; set; }
        public Data<WeightVector> Weights { get; set; }
    }
}
