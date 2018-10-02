using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Mesh
        {
            public static Runtime.Mesh CreatePrism(Vector3? Scale = null)
            {
                var positions = new List<Vector3>()
                {
                    new Vector3( 0.0f, 0.2f, 0.2f),
                    new Vector3( 0.2f,-0.2f, 0.2f),
                    new Vector3(-0.2f,-0.2f, 0.2f),

                    new Vector3( 0.0f, 0.2f,-0.2f),
                    new Vector3(-0.2f,-0.2f,-0.2f),
                    new Vector3( 0.2f,-0.2f,-0.2f),
                };

                if (Scale == null)
                {
                    Scale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                for (int vertexIndex = 0; vertexIndex < positions.Count; vertexIndex++)
                {
                    positions[vertexIndex] = positions[vertexIndex] * Scale.Value;
                }

                var prismMesh = new Runtime.Mesh
                {
                    Name = "prism",
                    MeshPrimitives = new[]
                    {
                        new Runtime.MeshPrimitive
                        {
                            Positions = positions,
                            Indices = new List<int>
                            {
                                0, 1, 3,
                                3, 1, 5,
                                2, 0, 4,
                                4, 0, 3,
                                1, 2, 5,
                                5, 2, 4,
                            },
                            Colors = new List<Vector4>()
                            {
                                new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                                new Vector4(0.8f, 0.8f, 0.8f, 0.8f),
                            },
                            Material = new Runtime.Material
                            {
                                DoubleSided = true
                            }
                        }
                    }
                };

                return prismMesh;
            }
        }
    }
}
