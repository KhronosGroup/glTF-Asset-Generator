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
            mesh.AddPrimitive(meshPrim);
            scene.AddMesh(mesh);
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
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f),
                new Vector3(0.0f,0.0f,-1.0f)
            };

            // 1:1 UV mapping
            List<List<Vector2>> planeTextureCoordSets = new List<List<Vector2>>
            {
                new List<Vector2>
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(0.0f, 0.0f)
                },
            };

            List<int> PlaneIndices = new List<int>
            {
                0, 1, 3, 1, 2, 3
            };
            Runtime.GLTF wrapper = new Runtime.GLTF();
            Runtime.Scene scene = new Runtime.Scene();
            Runtime.Mesh mesh = new Runtime.Mesh();
            Runtime.MeshPrimitive meshPrim = new Runtime.MeshPrimitive
            {
                Positions = planePositions,
                Normals = planeNormals,
                TextureCoordSets = planeTextureCoordSets,
                Indices = PlaneIndices
            };
            mesh.AddPrimitive(meshPrim);
            scene.AddMesh(mesh);
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
            mesh.AddPrimitive(meshPrim);
            scene.AddMesh(mesh);
            wrapper.Scenes.Add(scene);

            return wrapper;
        }
    }
}
