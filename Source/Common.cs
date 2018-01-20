using glTFLoader.Schema;
using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    internal class Common
    {
        /// <summary>
        /// Creates a triangle model using the glTF wrapper
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="geometryData"></param>
        /// <returns>GLTFWrapper object</returns>
        public static Runtime.GLTF SingleTriangle()
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
            mesh.MeshPrimitives.Add(meshPrim);
            scene.Nodes = new List<Runtime.Node> {
                new Runtime.Node
                {
                    Mesh = mesh
                }
            };
            wrapper.Scenes.Add(scene);

            return wrapper;
        }
        public static Runtime.GLTF SinglePlane()
        {
            List<Vector3> planePositions = new List<Vector3>()
            {
                new Vector3( 0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };

            // 1:1 UV mapping
            List<List<Vector2>> planeTextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 1.0f),
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f)
                },
            };

            List<int> PlaneIndices = new List<int>
            {
                1, 0, 3, 1, 3, 2
            };
            Runtime.GLTF wrapper = new Runtime.GLTF();
            Runtime.Scene scene = new Runtime.Scene();
            Runtime.Mesh mesh = new Runtime.Mesh();
            Runtime.MeshPrimitive meshPrim = new Runtime.MeshPrimitive
            {
                Indices = PlaneIndices,
                Positions = planePositions,
                TextureCoordSets = planeTextureCoordSets
            };
            mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
            {
                meshPrim
            };
            scene.Nodes = new List<Runtime.Node>
            {
                new Runtime.Node
                {
                    Mesh = mesh
                }
            };

            wrapper.Scenes.Add(scene);

            return wrapper;
        }

        public static Runtime.GLTF SingleCube()
        {
            List<Vector3> cubePositions = new List<Vector3>()
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
            };
            List<Vector3> cubeNormals = new List<Vector3>()
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
            };

            List<List<Vector2>> cubeTextureCoordSets = new List<List<Vector2>>
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
                },
            };
            List<int> cubeIndices = new List<int>
            {
                0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23
            };

            Runtime.GLTF wrapper = new Runtime.GLTF();
            Runtime.Scene scene = new Runtime.Scene();
            Runtime.Mesh mesh = new Runtime.Mesh();
            Runtime.MeshPrimitive meshPrim = new Runtime.MeshPrimitive
            {
                Positions = cubePositions,
                Normals = cubeNormals,
                TextureCoordSets = cubeTextureCoordSets,
                Indices = cubeIndices
            };
            mesh.MeshPrimitives = new List<Runtime.MeshPrimitive> { meshPrim };
            scene.Nodes = new List<Runtime.Node>
            {
                new Runtime.Node
                {
                    Mesh = mesh
                }
            };
            wrapper.Scenes.Add(scene);

            return wrapper;
        }

        public static Runtime.GLTF MultiNode()
        {
            List<Vector3> vertexPositions0 = new List<Vector3>()
            {
                new Vector3(-2.5f, 2.5f, 2.5f), // 01
                new Vector3(-2.5f,-2.5f, 2.5f), // 02
                new Vector3( 2.5f,-2.5f, 2.5f), // 03

                new Vector3( 2.5f,-2.5f, 2.5f), // 03
                new Vector3( 2.5f, 2.5f, 2.5f), // 04
                new Vector3(-2.5f, 2.5f, 2.5f), // 01

                new Vector3(-2.5f, 2.5f,   0f), // 05
                new Vector3(-2.5f, 2.5f, 2.5f), // 01
                new Vector3( 2.5f, 2.5f, 2.5f), // 04

                new Vector3( 2.5f, 2.5f, 2.5f), // 04
                new Vector3(   0f, 2.5f,   0f), // 06
                new Vector3(-2.5f, 2.5f,   0f), // 05

                new Vector3(-2.5f, 7.5f,-2.5f), // 07
                new Vector3(-2.5f, 7.5f,   0f), // 08
                new Vector3(   0f, 7.5f,   0f), // 09

                new Vector3(   0f, 7.5f,   0f), // 09
                new Vector3(   0f, 7.5f,-2.5f), // 10
                new Vector3(-2.5f, 7.5f,-2.5f), // 07

                new Vector3(   0f, 2.5f,-2.5f), // 11
                new Vector3(   0f, 2.5f,   0f), // 06
                new Vector3( 2.5f, 2.5f, 2.5f), // 04

                new Vector3( 2.5f, 2.5f, 2.5f), // 04
                new Vector3( 2.5f, 2.5f,-2.5f), // 12
                new Vector3(   0f, 2.5f,-2.5f), // 11

                new Vector3(-2.5f,   0f,-7.5f), // 13
                new Vector3(-2.5f, 2.5f,-7.5f), // 14
                new Vector3(   0f, 2.5f,-7.5f), // 15

                new Vector3(   0f, 2.5f,-7.5f), // 15
                new Vector3(   0f,   0f,-7.5f), // 16
                new Vector3(-2.5f,   0f,-7.5f), // 13

                new Vector3(   0f, 2.5f,-2.5f), // 11
                new Vector3( 2.5f, 2.5f,-2.5f), // 12
                new Vector3(   0f,   0f,-2.5f), // 17

                new Vector3(-2.5f,-2.5f,-2.5f), // 18
                new Vector3(-2.5f,   0f,-2.5f), // 19
                new Vector3(   0f,   0f,-2.5f), // 17

                new Vector3(   0f,   0f,-2.5f), // 17
                new Vector3( 2.5f,-2.5f,-2.5f), // 20
                new Vector3(-2.5f,-2.5f,-2.5f), // 18

                new Vector3(   0f,   0f,-2.5f), // 17
                new Vector3( 2.5f, 2.5f,-2.5f), // 12
                new Vector3( 2.5f,-2.5f,-2.5f), // 20

                new Vector3(-2.5f,-2.5f, 2.5f), // 02
                new Vector3(-2.5f,-2.5f,-2.5f), // 18
                new Vector3( 2.5f,-2.5f,-2.5f), // 20

                new Vector3( 2.5f,-2.5f,-2.5f), // 20
                new Vector3( 2.5f,-2.5f, 2.5f), // 03
                new Vector3(-2.5f,-2.5f, 2.5f), // 02

                new Vector3( 2.5f, 2.5f, 2.5f), // 04
                new Vector3( 2.5f,-2.5f, 2.5f), // 03
                new Vector3( 2.5f,-2.5f,-2.5f), // 20

                new Vector3( 2.5f,-2.5f,-2.5f), // 20
                new Vector3( 2.5f, 2.5f,-2.5f), // 12
                new Vector3( 2.5f, 2.5f, 2.5f), // 04

                new Vector3(-2.5f,   0f,-2.5f), // 19
                new Vector3(-2.5f,-2.5f,-2.5f), // 18
                new Vector3(-2.5f,-2.5f, 2.5f), // 02

                new Vector3(-2.5f,-2.5f, 2.5f), // 02
                new Vector3(-2.5f,   0f,   0f), // 21
                new Vector3(-2.5f,   0f,-2.5f), // 19

                new Vector3(-2.5f,-2.5f, 2.5f), // 02
                new Vector3(-2.5f, 2.5f, 2.5f), // 01
                new Vector3(-2.5f,   0f,   0f), // 21

                new Vector3(-7.5f, 2.5f,-2.5f), // 22
                new Vector3(-7.5f,   0f,-2.5f), // 23
                new Vector3(-7.5f,   0f,   0f), // 24

                new Vector3(-7.5f,   0f,   0f), // 24
                new Vector3(-7.5f, 2.5f,   0f), // 25
                new Vector3(-7.5f, 2.5f,-2.5f), // 22

                new Vector3(-2.5f,   0f,   0f), // 21
                new Vector3(-2.5f, 2.5f, 2.5f), // 01
                new Vector3(-2.5f, 2.5f,   0f), // 05

                new Vector3(-7.5f,   0f,-2.5f), // 23
                new Vector3(-2.5f,   0f,-2.5f), // 19
                new Vector3(-2.5f,   0f,   0f), // 21

                new Vector3(-2.5f,   0f,   0f), // 21
                new Vector3(-7.5f,   0f,   0f), // 24
                new Vector3(-7.5f,   0f,-2.5f), // 23

                new Vector3(-7.5f,   0f,   0f), // 24
                new Vector3(-2.5f,   0f,   0f), // 21
                new Vector3(-2.5f, 2.5f,   0f), // 05

                new Vector3(-2.5f, 2.5f,   0f), // 05
                new Vector3(-7.5f, 2.5f,   0f), // 25
                new Vector3(-7.5f,   0f,   0f), // 24

                new Vector3(-7.5f, 2.5f,   0f), // 25
                new Vector3(-2.5f, 2.5f,   0f), // 05
                new Vector3(-2.5f, 2.5f,-2.5f), // 26

                new Vector3(-2.5f, 2.5f,-2.5f), // 26
                new Vector3(-7.5f, 2.5f,-2.5f), // 22
                new Vector3(-7.5f, 2.5f,   0f), // 25

                new Vector3(-7.5f, 2.5f,-2.5f), // 22
                new Vector3(-2.5f, 2.5f,-2.5f), // 26
                new Vector3(-2.5f,   0f,-2.5f), // 19

                new Vector3(-2.5f,   0f,-2.5f), // 19
                new Vector3(-7.5f,   0f,-2.5f), // 23
                new Vector3(-7.5f, 2.5f,-2.5f), // 22

                new Vector3(-2.5f, 7.5f,   0f), // 08
                new Vector3(-2.5f, 2.5f,   0f), // 05
                new Vector3(   0f, 2.5f,   0f), // 06

                new Vector3(   0f, 2.5f,   0f), // 06
                new Vector3(   0f, 7.5f,   0f), // 09
                new Vector3(-2.5f, 7.5f,   0f), // 08

                new Vector3(   0f, 7.5f,   0f), // 09
                new Vector3(   0f, 2.5f,   0f), // 06
                new Vector3(   0f, 2.5f,-2.5f), // 11

                new Vector3(   0f, 2.5f,-2.5f), // 11
                new Vector3(   0f, 7.5f,-2.5f), // 10
                new Vector3(   0f, 7.5f,   0f), // 09

                new Vector3(   0f, 7.5f,-2.5f), // 10
                new Vector3(   0f, 2.5f,-2.5f), // 11
                new Vector3(-2.5f, 2.5f,-2.5f), // 26

                new Vector3(-2.5f, 2.5f,-2.5f), // 26
                new Vector3(-2.5f, 7.5f,-2.5f), // 07
                new Vector3(   0f, 7.5f,-2.5f), // 10

                new Vector3(-2.5f, 7.5f,-2.5f), // 07
                new Vector3(-2.5f, 2.5f,-2.5f), // 26
                new Vector3(-2.5f, 2.5f,   0f), // 05

                new Vector3(-2.5f, 2.5f,   0f), // 05
                new Vector3(-2.5f, 7.5f,   0f), // 08
                new Vector3(-2.5f, 7.5f,-2.5f), // 07

                new Vector3(-2.5f, 2.5f,-7.5f), // 14
                new Vector3(-2.5f, 2.5f,-2.5f), // 26
                new Vector3(   0f, 2.5f,-2.5f), // 11

                new Vector3(   0f, 2.5f,-2.5f), // 11
                new Vector3(   0f, 2.5f,-7.5f), // 15
                new Vector3(-2.5f, 2.5f,-7.5f), // 14

                new Vector3(   0f, 2.5f,-7.5f), // 15
                new Vector3(   0f, 2.5f,-2.5f), // 11
                new Vector3(   0f,   0f,-2.5f), // 17

                new Vector3(   0f,   0f,-2.5f), // 17
                new Vector3(   0f,   0f,-7.5f), // 16
                new Vector3(   0f, 2.5f,-7.5f), // 15

                new Vector3(   0f,   0f,-7.5f), // 16
                new Vector3(   0f,   0f,-2.5f), // 17
                new Vector3(-2.5f,   0f,-2.5f), // 19

                new Vector3(-2.5f,   0f,-2.5f), // 19
                new Vector3(-2.5f,   0f,-7.5f), // 13
                new Vector3(   0f,   0f,-7.5f), // 16

                new Vector3(-2.5f,   0f,-7.5f), // 13
                new Vector3(-2.5f,   0f,-2.5f), // 19
                new Vector3(-2.5f, 2.5f,-2.5f), // 26

                new Vector3(-2.5f, 2.5f,-2.5f), // 26
                new Vector3(-2.5f, 2.5f,-7.5f), // 14
                new Vector3(-2.5f,   0f,-7.5f), // 13

            };
            List<Vector3> vertexPositions1 = new List<Vector3>()
            {

                new Vector3(-7.5f,  -1f,   3f), // 27
                new Vector3(-7.5f,  -1f, 7.5f), // 28
                new Vector3(   3f,  -1f, 7.5f), // 29

                new Vector3(   3f,  -1f, 7.5f), // 29
                new Vector3(   3f,  -1f,   3f), // 30
                new Vector3(-7.5f,  -1f,   3f), // 27

                new Vector3(   3f,  -1f,   3f), // 30
                new Vector3(   3f,  -1f, 7.5f), // 29
                new Vector3( 7.5f,  -1f, 7.5f), // 31

                new Vector3( 7.5f,  -1f, 7.5f), // 31
                new Vector3( 7.5f,  -1f,   3f), // 32
                new Vector3(   3f,  -1f,   3f), // 30

                new Vector3(   3f,  -1f,-7.5f), // 33
                new Vector3(   3f,  -1f,   3f), // 30
                new Vector3( 7.5f,  -1f,   3f), // 32

                new Vector3( 7.5f,  -1f,   3f), // 32
                new Vector3( 7.5f,  -1f,-7.5f), // 34
                new Vector3(   3f,  -1f,-7.5f), // 33
            };

            List<List<Vector2>> textureCoordSets0 = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(0.58472f  , 0.7941f   ), // 01
                    new Vector2(0.584685f , 0.590266f ), // 02
                    new Vector2(0.788519f , 0.590231f ), // 03

                    new Vector2(0.788519f , 0.590231f ), // 03
                    new Vector2(0.788554f , 0.794065f ), // 04
                    new Vector2(0.58472f  , 0.7941f   ), // 01

                    new Vector2(0.370001f , 0.119521f ), // 05
                    new Vector2(0.369983f , 0.0176041f), // 06
                    new Vector2(0.573817f , 0.0175695f), // 07

                    new Vector2(0.573817f , 0.0175695f), // 07
                    new Vector2(0.471918f , 0.119504f ), // 08
                    new Vector2(0.370001f , 0.119521f ), // 05

                    new Vector2(0.333317f , 0.0407918f), // 09
                    new Vector2(0.333426f , 0.142045f ), // 10
                    new Vector2(0.232172f , 0.142154f ), // 11

                    new Vector2(0.232172f , 0.142154f ), // 11
                    new Vector2(0.232064f , 0.0409003f), // 12
                    new Vector2(0.333317f , 0.0407918f), // 09

                    new Vector2(0.471935f , 0.221421f ), // 13
                    new Vector2(0.471918f , 0.119504f ), // 08
                    new Vector2(0.573817f , 0.0175695f), // 07

                    new Vector2(0.573817f , 0.0175695f), // 07
                    new Vector2(0.573852f , 0.221404f ), // 14
                    new Vector2(0.471935f , 0.221421f ), // 13

                    new Vector2(0.24914f  , 0.573203f ), // 15
                    new Vector2(0.350249f , 0.573108f ), // 16
                    new Vector2(0.350345f , 0.674217f ), // 17

                    new Vector2(0.350345f , 0.674217f ), // 17
                    new Vector2(0.249236f , 0.674312f ), // 18
                    new Vector2(0.24914f  , 0.573203f ), // 15

                    new Vector2(0.57387f  , 0.323321f ), // 19
                    new Vector2(0.573852f , 0.221404f ), // 14
                    new Vector2(0.675786f , 0.323303f ), // 20

                    new Vector2(0.777721f , 0.425203f ), // 21
                    new Vector2(0.675804f , 0.42522f  ), // 22
                    new Vector2(0.675786f , 0.323303f ), // 20

                    new Vector2(0.675786f , 0.323303f ), // 20
                    new Vector2(0.777686f , 0.221369f ), // 23
                    new Vector2(0.777721f , 0.425203f ), // 21

                    new Vector2(0.675786f , 0.323303f ), // 20
                    new Vector2(0.573852f , 0.221404f ), // 14
                    new Vector2(0.777686f , 0.221369f ), // 23

                    new Vector2(0.981486f , 0.0175001f), // 24
                    new Vector2(0.98152f  , 0.221334f ), // 25
                    new Vector2(0.777686f , 0.221369f ), // 23

                    new Vector2(0.777686f , 0.221369f ), // 23
                    new Vector2(0.777652f , 0.0175346f), // 26
                    new Vector2(0.981486f , 0.0175001f), // 24

                    new Vector2(0.573817f , 0.0175695f), // 07
                    new Vector2(0.777652f , 0.0175346f), // 26
                    new Vector2(0.777686f , 0.221369f ), // 23

                    new Vector2(0.777686f , 0.221369f ), // 23
                    new Vector2(0.573852f , 0.221404f ), // 14
                    new Vector2(0.573817f , 0.0175695f), // 07

                    new Vector2(0.380868f , 0.692218f ), // 27
                    new Vector2(0.380851f , 0.590301f ), // 28
                    new Vector2(0.584685f , 0.590266f ), // 02

                    new Vector2(0.584685f , 0.590266f ), // 02
                    new Vector2(0.482785f , 0.692201f ), // 29
                    new Vector2(0.380868f , 0.692218f ), // 27

                    new Vector2(0.584685f , 0.590266f ), // 02
                    new Vector2(0.58472f  , 0.7941f   ), // 01
                    new Vector2(0.482785f , 0.692201f ), // 29

                    new Vector2(0.22507f  , 0.773789f ), // 30
                    new Vector2(0.124248f , 0.773804f ), // 31
                    new Vector2(0.124234f , 0.672982f ), // 32

                    new Vector2(0.124234f , 0.672982f ), // 32
                    new Vector2(0.225056f , 0.672968f ), // 33
                    new Vector2(0.22507f  , 0.773789f ), // 30

                    new Vector2(0.482785f , 0.692201f ), // 29
                    new Vector2(0.58472f  , 0.7941f   ), // 01
                    new Vector2(0.482803f , 0.794118f ), // 34

                    new Vector2(0.124248f , 0.773804f ), // 31
                    new Vector2(0.124277f , 0.975447f ), // 35
                    new Vector2(0.023455f , 0.975461f ), // 36

                    new Vector2(0.023455f , 0.975461f ), // 36
                    new Vector2(0.0234265f, 0.773818f ), // 37
                    new Vector2(0.124248f , 0.773804f ), // 31

                    new Vector2(0.426714f , 0.773761f ), // 38
                    new Vector2(0.426742f , 0.975405f ), // 39
                    new Vector2(0.32592f  , 0.975419f ), // 40
                    
                    new Vector2(0.32592f  , 0.975419f ), // 40
                    new Vector2(0.325892f , 0.773776f ), // 41
                    new Vector2(0.426714f , 0.773761f ), // 38

                    new Vector2(0.325892f , 0.773776f ), // 41
                    new Vector2(0.32592f  , 0.975419f ), // 40
                    new Vector2(0.225098f , 0.975433f ), // 42

                    new Vector2(0.225098f , 0.975433f ), // 42
                    new Vector2(0.22507f  , 0.773789f ), // 30
                    new Vector2(0.325892f , 0.773776f ), // 41

                    new Vector2(0.22507f  , 0.773789f ), // 30
                    new Vector2(0.225098f , 0.975433f ), // 42
                    new Vector2(0.124277f , 0.975447f ), // 35

                    new Vector2(0.124277f , 0.975447f ), // 35
                    new Vector2(0.124248f , 0.773804f ), // 31
                    new Vector2(0.22507f  , 0.773789f ), // 30

                    new Vector2(0.333426f , 0.142045f ), // 10
                    new Vector2(0.333643f , 0.344553f ), // 43
                    new Vector2(0.232389f , 0.344662f ), // 44

                    new Vector2(0.232389f , 0.344662f ), // 44
                    new Vector2(0.232172f , 0.142154f ), // 11
                    new Vector2(0.333426f , 0.142045f ), // 10

                    new Vector2(0.232172f , 0.142154f ), // 11
                    new Vector2(0.232389f , 0.344662f ), // 44
                    new Vector2(0.131136f , 0.34477f  ), // 45

                    new Vector2(0.131136f , 0.34477f  ), // 45
                    new Vector2(0.130919f , 0.142263f ), // 46
                    new Vector2(0.232172f , 0.142154f ), // 11

                    new Vector2(0.130919f , 0.142263f ), // 46
                    new Vector2(0.131136f , 0.34477f  ), // 45
                    new Vector2(0.0298816f, 0.344879f ), // 47

                    new Vector2(0.0298816f, 0.344879f ), // 47
                    new Vector2(0.0296644f, 0.142371f ), // 48
                    new Vector2(0.130919f , 0.142263f ), // 46

                    new Vector2(0.43468f  , 0.141937f ), // 49
                    new Vector2(0.434897f , 0.344445f ), // 50
                    new Vector2(0.333643f , 0.344553f ), // 43

                    new Vector2(0.333643f , 0.344553f ), // 43
                    new Vector2(0.333426f , 0.142045f ), // 10
                    new Vector2(0.43468f  , 0.141937f ), // 49

                    new Vector2(0.350249f , 0.573108f ), // 16
                    new Vector2(0.350058f , 0.37089f  ), // 51
                    new Vector2(0.451167f , 0.370795f ), // 52

                    new Vector2(0.451167f , 0.370795f ), // 52
                    new Vector2(0.451358f , 0.573012f ), // 53
                    new Vector2(0.350249f , 0.573108f ), // 16

                    new Vector2(0.451358f , 0.573012f ), // 53
                    new Vector2(0.451167f , 0.370795f ), // 52
                    new Vector2(0.552276f , 0.370699f ), // 54

                    new Vector2(0.552276f , 0.370699f ), // 54
                    new Vector2(0.552467f , 0.572917f ), // 55
                    new Vector2(0.451358f , 0.573012f ), // 53

                    new Vector2(0.148031f , 0.573299f ), // 56
                    new Vector2(0.147841f , 0.371081f ), // 57
                    new Vector2(0.24895f  , 0.370986f ), // 58

                    new Vector2(0.24895f  , 0.370986f ), // 58
                    new Vector2(0.24914f  , 0.573203f ), // 15
                    new Vector2(0.148031f , 0.573299f ), // 56

                    new Vector2(0.24914f  , 0.573203f ), // 15
                    new Vector2(0.24895f  , 0.370986f ), // 58
                    new Vector2(0.350058f , 0.37089f  ), // 51

                    new Vector2(0.350058f , 0.37089f  ), // 51
                    new Vector2(0.350249f , 0.573108f ), // 16
                    new Vector2(0.24914f  , 0.573203f ), // 15
                },
            };
            List<List<Vector2>> textureCoordSets1 = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(0.820247f , 0.440646f ), // 59
                    new Vector2(0.979596f , 0.440646f ), // 60
                    new Vector2(0.979596f , 0.812462f ), // 61

                    new Vector2(0.979596f , 0.812462f ), // 61
                    new Vector2(0.820246f , 0.812462f ), // 62
                    new Vector2(0.820247f , 0.440646f ), // 59

                    new Vector2(0.820246f , 0.812462f ), // 62
                    new Vector2(0.979596f , 0.812462f ), // 61
                    new Vector2(0.979596f , 0.971812f ), // 63

                    new Vector2(0.979596f , 0.971812f ), // 63
                    new Vector2(0.820247f , 0.971812f ), // 64
                    new Vector2(0.820246f , 0.812462f ), // 62

                    new Vector2(0.448431f , 0.812462f ), // 65
                    new Vector2(0.820246f , 0.812462f ), // 62
                    new Vector2(0.820247f , 0.971812f ), // 64

                    new Vector2(0.820247f , 0.971812f ), // 64
                    new Vector2(0.448431f , 0.971812f ), // 66
                    new Vector2(0.448431f , 0.812462f ), // 65
                },
            };

            List<Vector3> vertexNormals0 = new List<Vector3>()
            {
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1

                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4

                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4

                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5

                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4

                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4

                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1

                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1

                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1
                new Vector3( 0, 0, 1), // 1

                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5

                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3
                new Vector3( 0, 0,-1), // 3

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5

                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5
                new Vector3( 1, 0, 0), // 5

                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4

                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4
                new Vector3( 0,-1, 0), // 4

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6

                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
                new Vector3(-1, 0, 0), // 6
            };
            List<Vector3> vertexNormals1 = new List<Vector3>()
            {
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 7

                new Vector3( 0, 1, 0), // 7
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 7
                new Vector3( 0, 1, 0), // 7

                new Vector3( 0, 1, 0), // 7
                new Vector3( 0, 1, 0), // 7
                new Vector3( 0, 1, 0), // 2

                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 7

                new Vector3( 0, 1, 0), // 7
                new Vector3( 0, 1, 0), // 2
                new Vector3( 0, 1, 0), // 2
            };

            Runtime.GLTF wrapper = new Runtime.GLTF();
            Runtime.Scene scene = new Runtime.Scene();
            Runtime.Mesh mesh0 = new Runtime.Mesh();
            Runtime.Mesh mesh1 = new Runtime.Mesh();
            scene.Nodes = new List<Runtime.Node>();

            Runtime.MeshPrimitive meshPrim0 = new Runtime.MeshPrimitive
            {
                Positions = vertexPositions0,
                TextureCoordSets = textureCoordSets0,
                Normals = vertexNormals0,
            };
            mesh0.MeshPrimitives = new List<Runtime.MeshPrimitive> { meshPrim0 };
            scene.Nodes.Add(
                new Runtime.Node
                {
                    Mesh = mesh0,
                    Name = "Node0"
                });

            Runtime.MeshPrimitive meshPrim1 = new Runtime.MeshPrimitive
            {
                Positions = vertexPositions1,
                TextureCoordSets = textureCoordSets1,
                Normals = vertexNormals1,
            };
            mesh1.MeshPrimitives = new List<Runtime.MeshPrimitive> { meshPrim1 };
            scene.Nodes.Add(
                new Runtime.Node
                {
                    Mesh = mesh1,
                    Name = "Node1"
                });

            wrapper.Scenes.Add(scene);

            return wrapper;
        }
    }
}
