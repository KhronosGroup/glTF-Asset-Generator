﻿using System.Collections.Generic;
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
                            Positions = includePositions ? GetTrianglePositions() : null,
                            Indices = includeIndices ? GetTriangleIndices() : null,
                            Material = includeMaterial ? GetTriangleMaterial() : null,
                            Normals = includeNormals ? GetTriangleNormals() : null,
                        }
                    }
                };
            }

            public static List<Vector3> GetTrianglePositions()
            {
                return new List<Vector3>()
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
                    MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorFactor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                    }
                };
            }

            public static List<int> GetTriangleIndices()
            {
                return new List<int>
                {
                    0, 1, 2,
                };
            }

            public static List<Vector3> GetTriangleNormals()
            {
                return new List<Vector3>()
                {
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                };
            }
        }
    }
}
