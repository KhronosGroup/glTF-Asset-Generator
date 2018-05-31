using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class MeshPrimitive
        {
            public static Runtime.MeshPrimitive CreateCube()
            {
                return new Runtime.MeshPrimitive
                {
                    Positions = new List<Vector3>()
                    {
                        // Left
                        new Vector3(-0.1f, -0.1f, -0.1f),
                        new Vector3(-0.1f, -0.1f, 0.1f),
                        new Vector3(-0.1f, 0.1f, 0.1f),
                        new Vector3(-0.1f, 0.1f, -0.1f),

                        // Front
                        new Vector3(-0.1f, -0.1f, 0.1f),
                        new Vector3(0.1f, -0.1f, 0.1f),
                        new Vector3(0.1f, 0.1f, 0.1f),
                        new Vector3(-0.1f, 0.1f, 0.1f),

                        // Right
                        new Vector3(0.1f, -0.1f, 0.1f),
                        new Vector3(0.1f, -0.1f, -0.1f),
                        new Vector3(0.1f, 0.1f, -0.1f),
                        new Vector3(0.1f, 0.1f, 0.1f),

                        // Top
                        new Vector3(-0.1f, 0.1f, 0.1f),
                        new Vector3(0.1f, 0.1f, 0.1f),
                        new Vector3(0.1f, 0.1f, -0.1f),
                        new Vector3(-0.1f, 0.1f, -0.1f),

                        // Back
                        new Vector3(-0.1f, 0.1f, -0.1f),
                        new Vector3(0.1f, 0.1f, -0.1f),
                        new Vector3(0.1f, -0.1f, -0.1f),
                        new Vector3(-0.1f, -0.1f, -0.1f),

                        // Bottom
                        new Vector3(-0.1f, -0.1f, -0.1f),
                        new Vector3(0.1f, -0.1f, -0.1f),
                        new Vector3(0.1f, -0.1f, 0.1f),
                        new Vector3(-0.1f, -0.1f, 0.1f)
                    },
                    Normals = new List<Vector3>()
                    {
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(1.0f, 0.0f, 0.0f),
                        new Vector3(1.0f, 0.0f, 0.0f),
                        new Vector3(1.0f, 0.0f, 0.0f),
                        new Vector3(1.0f, 0.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 0.0f,-1.0f),
                        new Vector3(0.0f, 0.0f,-1.0f),
                        new Vector3(0.0f, 0.0f,-1.0f),
                        new Vector3(0.0f, 0.0f,-1.0f),
                        new Vector3(0.0f,-1.0f, 0.0f),
                        new Vector3(0.0f,-1.0f, 0.0f),
                        new Vector3(0.0f,-1.0f, 0.0f),
                        new Vector3(0.0f,-1.0f, 0.0f)
                    },
                    TextureCoordSets = new List<List<Vector2>>
                    {
                        new List<Vector2>
                        {
                            // Right
                            new Vector2(0.000f, 0.667f),
                            new Vector2(0.500f, 0.667f),
                            new Vector2(0.500f, 0.333f),
                            new Vector2(0.000f, 0.333f),

                            // Front
                            new Vector2(0.500f, 0.333f),
                            new Vector2(1.000f, 0.333f),
                            new Vector2(1.000f, 0.000f),
                            new Vector2(0.500f, 0.000f),

                            // Left
                            new Vector2(0.000f, 0.333f),
                            new Vector2(0.500f, 0.333f),
                            new Vector2(0.500f, 0.000f),
                            new Vector2(0.000f, 0.000f),

                            // Top
                            new Vector2(0.500f, 0.667f),
                            new Vector2(1.000f, 0.667f),
                            new Vector2(1.000f, 0.333f),
                            new Vector2(0.500f, 0.333f),

                            // Back
                            new Vector2(0.500f, 0.667f),
                            new Vector2(0.000f, 0.667f),
                            new Vector2(0.000f, 1.000f),
                            new Vector2(0.500f, 1.000f),

                            // Bottom
                            new Vector2(0.500f, 1.000f),
                            new Vector2(1.000f, 1.000f),
                            new Vector2(1.000f, 0.667f),
                            new Vector2(0.500f, 0.667f),
                        }
                    },
                    Indices = new List<int>
                    {
                        0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23
                    },
                };
            }
        }
    }
}

