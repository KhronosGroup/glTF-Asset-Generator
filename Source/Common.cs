using glTFLoader.Schema;
using System.Collections.Generic;

namespace AssetGenerator
{
    public class Common
    {
        /// <summary>
        /// Creates a triangle model using the glTF wrapper
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="geometryData"></param>
        /// <returns>GLTFWrapper object</returns>
        public static Runtime.GLTF SingleTriangleMultipleUVSetsWrapper(Gltf gltf, Data geometryData)
        {
            List<Vector3> trianglePositions = new List<Vector3>()
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };
            List<Vector3> triangleNormals = new List<Vector3>()
            {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };
            List<List<Vector2>> triangleTextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(0.5f, 1.0f),
                    new Vector2(0.25f, 0.0f)
                },
                new List<Vector2>
                {
                    new Vector2(0.5f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.75f, 0.0f)
                }

            };
            Runtime.GLTF wrapper = new Runtime.GLTF();
            Runtime.Scene scene = new Runtime.Scene();
            Runtime.Mesh mesh = new Runtime.Mesh();
            Runtime.MeshPrimitive meshPrim = new Runtime.MeshPrimitive
            {
                Positions = trianglePositions,
                Normals = triangleNormals,
                TextureCoordSets = triangleTextureCoordSets
            };
            mesh.AddPrimitive(meshPrim);
            scene.AddMesh(mesh);
            wrapper.Scenes.Add(scene);

            return wrapper;

        }
        public static Runtime.GLTF SinglePlaneWrapper(Gltf gltf, Data geometryData)
        {
            List<Vector3> planePositions = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f, 0.0f),
                new Vector3( -1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3( -1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f)
            };

            List<List<Vector2>> planeTextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(-1.0f, 2.0f),
                    new Vector2(2.0f, 2.0f),
                    new Vector2(-1.0f, -1.0f),
                    new Vector2(2.0f, 2.0f),
                    new Vector2(2.0f, -1.0f),
                    new Vector2(-1.0f, -1.0f)
                },
            };
            Runtime.GLTF wrapper = new Runtime.GLTF();
            Runtime.Scene scene = new Runtime.Scene();
            Runtime.Mesh mesh = new Runtime.Mesh();
            Runtime.MeshPrimitive meshPrim = new Runtime.MeshPrimitive
            {
                Positions = planePositions,
                Normals = planeNormals,
                TextureCoordSets = planeTextureCoordSets
            };
            mesh.AddPrimitive(meshPrim);
            scene.AddMesh(mesh);
            wrapper.Scenes.Add(scene);

            return wrapper;
        }
        public static void SingleCube(Gltf gltf, Data geometryData)
        {
            var positions = new[]
            {
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3( 1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, -1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, 1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3( 1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3( 1.0f, -1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, -1.0f, 1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f)
            };

            var normals = new[]
            {
                new Vector3( 0.0f, 0.0f, -1.0f),
                new Vector3( 0.0f, 0.0f, -1.0f),
                new Vector3( 0.0f, 0.0f, -1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 1.0f, 0.0f, 0.0f),
                new Vector3( 1.0f, 0.0f, 0.0f),
                new Vector3( 1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, -1.0f, 0.0f),
                new Vector3( 0.0f, -1.0f, 0.0f),
                new Vector3( 0.0f, -1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 0.0f, -1.0f),
                new Vector3( 0.0f, 0.0f, -1.0f),
                new Vector3( 0.0f, 0.0f, -1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 1.0f, 0.0f, 0.0f),
                new Vector3( 1.0f, 0.0f, 0.0f),
                new Vector3( 1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, -1.0f, 0.0f),
                new Vector3( 0.0f, -1.0f, 0.0f),
                new Vector3( 0.0f, -1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f)


            };

            geometryData.Writer.Write(positions);
            geometryData.Writer.Write(normals);

            gltf.Buffers = new[]
            {
                new Buffer
                {
                    Uri = geometryData.Name,
                    ByteLength = sizeof(float) * 3 * (positions.Length + normals.Length),
                }
            };

            gltf.BufferViews = new[]
            {
                new BufferView
                {
                    Buffer = 0,
                    ByteLength = sizeof(float) * 3 * positions.Length,
                },
                new BufferView
                {
                    Buffer = 0,
                    ByteOffset = sizeof(float) * 3 * positions.Length,
                    ByteLength = sizeof(float) * 3 * normals.Length
                }
            };

            gltf.Accessors = new[]
            {
                new Accessor
                {
                    BufferView = 0,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = positions.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] { 1.0f, 1.0f, 1.0f },
                    Min = new[] { -1.0f, -1.0f, -1.0f },
                },
                new Accessor
                {
                    BufferView = 1,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = normals.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] {1.0f, 1.0f, 1.0f},
                    Min = new[] {-1.0f, -1.0f, -1.0f}
                }
            };

            gltf.Meshes = new[]
            {
                new Mesh
                {
                    Primitives = new[]
                    {
                        new MeshPrimitive
                        {
                            Attributes = new Dictionary<string, int>
                            {
                                { "POSITION", 0 },
                                { "NORMAL", 1}
                            }
                        }
                    },
                }
            };

            gltf.Nodes = new[]
            {
                new Node
                {
                    Mesh = 0
                }
            };

            gltf.Scenes = new[]
            {
                new Scene
                {
                    Nodes = new[] { 0 }
                }
            };

            gltf.Scene = 0;
        }

    }
}
