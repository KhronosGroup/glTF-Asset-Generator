using glTFLoader.Schema;
using System.Collections.Generic;

namespace AssetGenerator
{
    public class Common
    {
        public static void SingleTriangle(Gltf gltf, Data geometryData, Tests testArea)
        {
            var positions = new[]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            };

            var normals = new[]
            {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };

            if (testArea == Tests.Materials)
            {
                gltf.Materials = new[]
                {
                    new Material
                    {

                    }
                };
            }

            if (testArea == Tests.PbrMetallicRoughness)
            {
                gltf.Materials = new[]
                {
                    new Material
                    {
                        PbrMetallicRoughness = new MaterialPbrMetallicRoughness
                        {
                        }
                    }
                };
            }

            geometryData.Writer.Write(positions);
            geometryData.Writer.Write(normals);

            gltf.Buffers = new[]
            {
                new Buffer
                {
                    Uri = geometryData.Name,
                    ByteLength = (sizeof(float) * 3 * (positions.Length + normals.Length )),
                }
            };

            gltf.BufferViews = new[]
            {
                new BufferView
                {
                    Name = "Positions",
                    Buffer = 0,
                    ByteLength = sizeof(float) * 3 * positions.Length,
                },
                new BufferView
                {
                    Name = "Normals",
                    Buffer = 0,
                    ByteOffset = sizeof(float) * 3 * positions.Length,
                    ByteLength = sizeof(float) * 3 * normals.Length
                }
            };

            gltf.Accessors = new[]
            {
                new Accessor
                {
                    Name="Position Accessor",
                    BufferView = 0,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = positions.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] { 1.0f, 1.0f, 0.0f },
                    Min = new[] { 0.0f, 0.0f, 0.0f },
                },
                new Accessor
                {
                    Name="Normal Accessor",
                    BufferView = 1,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = normals.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] { 0.0f, 0.0f, 1.0f},
                    Min = new[] { 0.0f, 0.0f, 0.0f}

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
                                { "NORMAL", 1 }
                            }
                        }
                    }
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
        public static void SingleTriangleMultipleUVSets(Gltf gltf, Data geometryData)
        {
            var positions = new[]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            };

            var normals = new[]
            {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f)
            };
            var uv1 = new[]
            {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.25f, 0.0f)

            };
            var uv2 = new[]
            {
                new Vector2(0.5f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.75f, 0.0f)

            };

            geometryData.Writer.Write(positions);
            geometryData.Writer.Write(normals);
            geometryData.Writer.Write(uv1);
            geometryData.Writer.Write(uv2);

            gltf.Buffers = new[]
            {
                new Buffer
                {
                    Uri = geometryData.Name,
                    ByteLength = (sizeof(float) * 3 * (positions.Length + normals.Length )) + (2 * sizeof(float) *  (uv1.Length + uv2.Length)),
                }
            };

            gltf.BufferViews = new[]
            {
                new BufferView
                {
                    Name = "Positions",
                    Buffer = 0,
                    ByteLength = sizeof(float) * 3 * positions.Length,
                },
                new BufferView
                {
                    Name = "Normals",
                    Buffer = 0,
                    ByteOffset = sizeof(float) * 3 * positions.Length,
                    ByteLength = sizeof(float) * 3 * normals.Length
                },
                new BufferView
                {
                    Name = "uv1",
                    Buffer = 0,
                    ByteOffset = (sizeof(float) * 3 * positions.Length) + (sizeof(float) * 3 * normals.Length),
                    ByteLength = sizeof(float) * 2 * (uv1.Length)
                },
                new BufferView
                {
                    Name = "uv2",
                    Buffer = 0,
                    ByteOffset = (sizeof(float) * 3 * positions.Length) + (sizeof(float) * 3 * normals.Length) + (sizeof(float) * 2 * (uv1.Length)),
                    ByteLength = sizeof(float) * 2 * (uv2.Length)
                },
            };

            gltf.Accessors = new[]
            {
                new Accessor
                {
                    Name="Position Accessor",
                    BufferView = 0,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = positions.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] { 1.0f, 1.0f, 0.0f },
                    Min = new[] { 0.0f, 0.0f, 0.0f },
                },
                new Accessor
                {
                    Name="Normal Accessor",
                    BufferView = 1,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = normals.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] { 0.0f, 0.0f, 1.0f},
                    Min = new[] { 0.0f, 0.0f, 0.0f}

                },
                new Accessor
                {
                    Name="UV1 Accessor",
                    BufferView = 2,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = uv1.Length,
                    Type = Accessor.TypeEnum.VEC2,
                    Max = new[] { 1.0f, 1.0f},
                    Min = new[] { 0.0f, 0.0f}

                },
                new Accessor
                {
                    Name="UV2 Accessor",
                    BufferView = 3,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = uv2.Length,
                    Type = Accessor.TypeEnum.VEC2,
                    Max = new[] { 1.0f, 1.0f},
                    Min = new[] { 0.0f, 0.0f}
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
                                { "NORMAL", 1 },
                                { "TEXCOORD_0", 2 },
                                { "TEXCOORD_1", 3 }
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
        public static void SinglePlane(Gltf gltf, Data geometryData)
        {
            var positions = new[]
            {
                new Vector3( 0.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f)

            };
            var normals = new[]
            {
                new Vector3(0.0f,0.0f,1.0f),
                new Vector3(0.0f,0.0f,1.0f),
                new Vector3(0.0f,0.0f,1.0f),
                new Vector3(0.0f,0.0f,1.0f),
                new Vector3(0.0f,0.0f,1.0f),
                new Vector3(0.0f,0.0f,1.0f)
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
                    ByteLength = sizeof(float) * 3 * positions.Length 
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
                    Max = new[] { 1.0f, 1.0f, 0.0f },
                    Min = new[] { -1.0f, -1.0f, 0.0f },
                },
                new Accessor
                {
                    BufferView = 1,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = normals.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] {0.0f, 0.0f, 1.0f},
                    Min = new[] {0.0f, 0.0f, 0.0f}
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
                                { "NORMAL", 1 }
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
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(0.0f, 0.0f)
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
        public static void SinglePlaneTextured(Gltf gltf, Data geometryData)
        {
            var positions = new[]
            {
                new Vector3( 0.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3( 0.0f, 1.0f, 0.0f)

            };
            var normals = new[]
            {
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f)
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
                    ByteLength = sizeof(float) * 3 * positions.Length
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
                    Max = new[] { 1.0f, 1.0f, 0.0f },
                    Min = new[] { -1.0f, -1.0f, 0.0f },
                },
                new Accessor
                {
                    BufferView = 1,
                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                    Count = normals.Length,
                    Type = Accessor.TypeEnum.VEC3,
                    Max = new[] {0.0f, 0.0f, 1.0f},
                    Min = new[] {0.0f, 0.0f, 0.0f}
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
                                { "NORMAL", 1 },
                                { "TEXCOORD_0", 0 }
                            }
                        }
                    },
                }
            };
            gltf.Textures = new[]
            {
                new Texture
                {
                    Sampler = 0,
                    Source = 0,
                    Name = "Brick Texture"
                }
            };
            gltf.Images = new[]
            {
                new Image
                {
                    Uri = "brick_2.png"
                }
            };
            gltf.Samplers = new[]
            {
                new Sampler
                {
                    MagFilter = Sampler.MagFilterEnum.LINEAR,
                    MinFilter = Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR,
                    WrapS = Sampler.WrapSEnum.REPEAT,
                    WrapT = Sampler.WrapTEnum.REPEAT
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
