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
                    Positions = Runtime.Data.Create
                    (
                        new[]
                        {
                            // Right
                            new Vector3(-0.3f, -0.3f, -0.3f),
                            new Vector3(-0.3f, -0.3f,  0.3f),
                            new Vector3(-0.3f,  0.3f,  0.3f),
                            new Vector3(-0.3f,  0.3f, -0.3f),

                            // Front
                            new Vector3(-0.3f, -0.3f,  0.3f),
                            new Vector3( 0.3f, -0.3f,  0.3f),
                            new Vector3( 0.3f,  0.3f,  0.3f),
                            new Vector3(-0.3f,  0.3f,  0.3f),

                            // Left
                            new Vector3( 0.3f, -0.3f,  0.3f),
                            new Vector3( 0.3f, -0.3f, -0.3f),
                            new Vector3( 0.3f,  0.3f, -0.3f),
                            new Vector3( 0.3f,  0.3f,  0.3f),

                            // Top
                            new Vector3(-0.3f,  0.3f,  0.3f),
                            new Vector3( 0.3f,  0.3f,  0.3f),
                            new Vector3( 0.3f,  0.3f, -0.3f),
                            new Vector3(-0.3f,  0.3f, -0.3f),

                            // Back
                            new Vector3(-0.3f,  0.3f, -0.3f),
                            new Vector3( 0.3f,  0.3f, -0.3f),
                            new Vector3( 0.3f, -0.3f, -0.3f),
                            new Vector3(-0.3f, -0.3f, -0.3f),

                            // Bottom
                            new Vector3(-0.3f, -0.3f, -0.3f),
                            new Vector3( 0.3f, -0.3f, -0.3f),
                            new Vector3( 0.3f, -0.3f,  0.3f),
                            new Vector3(-0.3f, -0.3f,  0.3f)
                        }
                    ),
                    Normals = Runtime.Data.Create
                    (
                        new[]
                        {
                            new Vector3(-1.0f,  0.0f,  0.0f),
                            new Vector3(-1.0f,  0.0f,  0.0f),
                            new Vector3(-1.0f,  0.0f,  0.0f),
                            new Vector3(-1.0f,  0.0f,  0.0f),
                            new Vector3( 0.0f,  0.0f,  1.0f),
                            new Vector3( 0.0f,  0.0f,  1.0f),
                            new Vector3( 0.0f,  0.0f,  1.0f),
                            new Vector3( 0.0f,  0.0f,  1.0f),
                            new Vector3( 1.0f,  0.0f,  0.0f),
                            new Vector3( 1.0f,  0.0f,  0.0f),
                            new Vector3( 1.0f,  0.0f,  0.0f),
                            new Vector3( 1.0f,  0.0f,  0.0f),
                            new Vector3( 0.0f,  1.0f,  0.0f),
                            new Vector3( 0.0f,  1.0f,  0.0f),
                            new Vector3( 0.0f,  1.0f,  0.0f),
                            new Vector3( 0.0f,  1.0f,  0.0f),
                            new Vector3( 0.0f,  0.0f, -1.0f),
                            new Vector3( 0.0f,  0.0f, -1.0f),
                            new Vector3( 0.0f,  0.0f, -1.0f),
                            new Vector3( 0.0f,  0.0f, -1.0f),
                            new Vector3( 0.0f, -1.0f,  0.0f),
                            new Vector3( 0.0f, -1.0f,  0.0f),
                            new Vector3( 0.0f, -1.0f,  0.0f),
                            new Vector3( 0.0f, -1.0f,  0.0f)
                        }
                    ),
                    TexCoords0 = Runtime.Data.Create
                    (
                        new[]
                        {
                            // Right
                            new Vector2(0.00f, 0.75f),
                            new Vector2(0.25f, 0.75f),
                            new Vector2(0.25f, 0.50f),
                            new Vector2(0.00f, 0.50f),

                            // Front
                            new Vector2(0.25f, 0.75f),
                            new Vector2(0.50f, 0.75f),
                            new Vector2(0.50f, 0.50f),
                            new Vector2(0.25f, 0.50f),

                            // Left
                            new Vector2(0.50f, 0.75f),
                            new Vector2(0.75f, 0.75f),
                            new Vector2(0.75f, 0.50f),
                            new Vector2(0.50f, 0.50f),

                            // Top
                            new Vector2(0.25f, 0.50f),
                            new Vector2(0.50f, 0.50f),
                            new Vector2(0.50f, 0.25f),
                            new Vector2(0.25f, 0.25f),

                            // Back
                            new Vector2(0.25f, 0.25f),
                            new Vector2(0.50f, 0.25f),
                            new Vector2(0.50f, 0.00f),
                            new Vector2(0.25f, 0.00f),

                            // Bottom
                            new Vector2(0.25f, 1.00f),
                            new Vector2(0.50f, 1.00f),
                            new Vector2(0.50f, 0.75f),
                            new Vector2(0.25f, 0.75f),
                        }
                    ),
                    Indices = Runtime.Data.Create
                    (
                        new[]
                        {
                            0, 1, 2,
                            0, 2, 3,
                            4, 5, 6,
                            4, 6, 7,
                            8, 9, 10,
                            8, 10, 11,
                            12, 13, 14,
                            12, 14, 15,
                            16, 17, 18,
                            16, 18, 19,
                            20, 21, 22,
                            20, 22, 23,
                        }
                    ),
                };
            }
        }
    }
}

