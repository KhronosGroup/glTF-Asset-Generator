using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Mesh
        {
            public static Runtime.Mesh CreateTriangle()
            {
                return new Runtime.Mesh
                {
                    Name = "triangle",
                    MeshPrimitives = new[]
                    {
                        new Runtime.MeshPrimitive
                        {
                            Positions = new List<Vector3>()
                            {
                                new Vector3( 0.0f, -0.2f, -0.05f),
                                new Vector3( 0.0f, -0.2f, 0.05f),
                                new Vector3( 0.0f, 0.0f, 0.0f),
                            },
                            Indices = new List<int>
                            {
                                0, 1, 2,
                            },
                            Material = new Runtime.Material
                            {
                                DoubleSided = true,
                                MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                                {
                                    BaseColorFactor = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
