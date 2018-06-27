using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class MeshPrimitive
        {
            public static Runtime.MeshPrimitive CreateCube()
            {
                var meshPrimitive = new Runtime.MeshPrimitive
                {
                    Vertices = new Runtime.MeshPrimitiveVertex[]
                    {
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, -0.3f, -0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, -0.3f, 0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, 0.3f, 0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, 0.3f, -0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, -0.3f, 0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, -0.3f, 0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, 0.3f, 0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, 0.3f, 0.3f), normal : new Vector3(0f, 0f, 1f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, -0.3f, 0.3f), normal : new Vector3(1f, 0f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, -0.3f, -0.3f), normal : new Vector3(1f, 0f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.75f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, 0.3f, -0.3f), normal : new Vector3(1f, 0f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.75f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, 0.3f, 0.3f), normal : new Vector3(1f, 0f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, 0.3f, 0.3f), normal : new Vector3(0f, 1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, 0.3f, 0.3f), normal : new Vector3(0f, 1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.5f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, 0.3f, -0.3f), normal : new Vector3(0f, 1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.25f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, 0.3f, -0.3f), normal : new Vector3(0f, 1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.25f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, 0.3f, -0.3f), normal : new Vector3(0f, 0f, -1f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.25f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, 0.3f, -0.3f), normal : new Vector3(0f, 0f, -1f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.25f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, -0.3f, -0.3f), normal : new Vector3(0f, 0f, -1f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, -0.3f, -0.3f), normal : new Vector3(0f, 0f, -1f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, -0.3f, -0.3f), normal : new Vector3(0f, -1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 1f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, -0.3f, -0.3f), normal : new Vector3(0f, -1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 1f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(0.3f, -0.3f, 0.3f), normal : new Vector3(0f, -1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.5f, 0.75f) }),
                        new Runtime.MeshPrimitiveVertex( position : new Vector3(-0.3f, -0.3f, 0.3f), normal : new Vector3(0f, -1f, 0f), textureCoordSet : new Vector2[] { new Vector2(0.25f, 0.75f) }),
                    },
                    Indices = new int[]
                    {
                        0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23
                    }
                };

                return meshPrimitive;
            }
        }
    }
}

