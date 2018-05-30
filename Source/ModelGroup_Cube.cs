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
                        new Vector3( -0.5f, -0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, 0.5f),
                        new Vector3( -0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, 0.5f),
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(-0.5f, 0.5f, -0.5f),
                        new Vector3(-0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(-0.5f, -0.5f, 0.5f)
                    },
                    Normals = new List<Vector3>()
                    {
                        new Vector3(0.0f,0.0f,1.0f),
                        new Vector3(0.0f,0.0f,1.0f),
                        new Vector3(0.0f,0.0f,1.0f),
                        new Vector3(0.0f,0.0f,1.0f),
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
                        new Vector3(0.0f,0.0f,-1.0f),
                        new Vector3(0.0f,0.0f,-1.0f),
                        new Vector3(0.0f,0.0f,-1.0f),
                        new Vector3(0.0f,0.0f,-1.0f),
                        new Vector3(0.0f,-1.0f,0.0f),
                        new Vector3(0.0f,-1.0f,0.0f),
                        new Vector3(0.0f,-1.0f,0.0f),
                        new Vector3(0.0f,-1.0f,0.0f)
                    },
                    TextureCoordSets = new List<List<Vector2>>
                    {
                        new List<Vector2>
                        {
                            new Vector2(0.125f, 1.0f),
                            new Vector2(0.375f, 1.0f),
                            new Vector2(0.375f, 0.75f),
                            new Vector2(0.125f, 0.75f),
                            new Vector2(0.375f, 1.00f),
                            new Vector2(0.625f, 1.00f),
                            new Vector2(0.625f, 0.75f),
                            new Vector2(0.375f, 0.75f),
                            new Vector2(0.625f, 1.00f),
                            new Vector2(0.875f, 1.00f),
                            new Vector2(0.875f, 0.75f),
                            new Vector2(0.625f, 0.75f),
                            new Vector2(0.375f, 0.75f),
                            new Vector2(0.625f, 0.75f),
                            new Vector2(0.625f, 0.5f),
                            new Vector2(0.375f, 0.5f),
                            new Vector2(0.375f, 0.5f),
                            new Vector2(0.625f, 0.5f),
                            new Vector2(0.625f, 0.25f),
                            new Vector2(0.375f, 0.25f),
                            new Vector2(0.375f, 0.25f),
                            new Vector2(0.625f, 0.25f),
                            new Vector2(0.625f, 0.0f),
                            new Vector2(0.375f, 0.0f)
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

