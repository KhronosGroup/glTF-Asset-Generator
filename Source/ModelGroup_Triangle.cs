using AssetGenerator.Runtime;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Mesh
        {
            public static Runtime.Mesh CreateTriangle(bool includeMaterial = true, bool includeIndices = true, bool includePositions = true, bool includeNormals = false)
            {
                return new Runtime.Mesh
                {
                    Name = "triangle",
                    MeshPrimitives = new[]
                    {
                        new Runtime.MeshPrimitive
                        {
                            Positions = includePositions ? Data.Create(GetTrianglePositions()) : null,
                            Indices = includeIndices ? Data.Create(GetTriangleIndices()) : null,
                            Normals = includeNormals ? Data.Create(GetTriangleNormals()) : null,
                            Material = includeMaterial ? GetTriangleMaterial() : null,
                        }
                    }
                };
            }

            public static Vector3[] GetTrianglePositions()
            {
                return new[]
                {
                    new Vector3(0.0f, -0.2f, -0.05f),
                    new Vector3(0.0f, -0.2f,  0.05f),
                    new Vector3(0.0f,  0.0f,  0.00f),
                };
            }

            public static Runtime.Material GetTriangleMaterial()
            {
                return new Runtime.Material
                {
                    DoubleSided = true,
                    PbrMetallicRoughness = new PbrMetallicRoughness
                    {
                        BaseColorFactor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                    }
                };
            }

            public static int[] GetTriangleIndices()
            {
                return new[]
                {
                    0, 1, 2,
                };
            }

            public static Vector3[] GetTriangleNormals()
            {
                return new[]
                {
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                };
            }
        }
    }
}
