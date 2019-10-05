using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Mesh
        {
            public static Runtime.Mesh CreatePrism(Vector4 color, Vector3? scale = null)
            {
                var positions = new List<Vector3>
                {
                    new Vector3( 0.0f,  0.3f,  0.3f),
                    new Vector3( 0.3f, -0.3f,  0.3f),
                    new Vector3(-0.3f, -0.3f,  0.3f),

                    new Vector3( 0.0f,  0.3f, -0.3f),
                    new Vector3(-0.3f, -0.3f, -0.3f),
                    new Vector3( 0.3f, -0.3f, -0.3f),
                };

                if (scale != null)
                {
                    for (var vertexIndex = 0; vertexIndex < positions.Count; vertexIndex++)
                    {
                        positions[vertexIndex] = positions[vertexIndex] * scale.Value;
                    }
                }

                var prismMesh = new Runtime.Mesh
                {
                    Name = "prism",
                    MeshPrimitives = new[]
                    {
                        new Runtime.MeshPrimitive
                        {
                            Positions = Runtime.Data.Create(positions),
                            Indices = Runtime.Data.Create(new[]
                            {
                                0, 1, 3,
                                3, 1, 5,
                                2, 0, 4,
                                4, 0, 3,
                                1, 2, 5,
                                5, 2, 4,
                            }),
                            Material = new Runtime.Material
                            {
                                DoubleSided = true,
                                PbrMetallicRoughness = new Runtime.PbrMetallicRoughness
                                {
                                    BaseColorFactor = color
                                }
                            }
                        }
                    }
                };

                return prismMesh;
            }
        }
    }
}
