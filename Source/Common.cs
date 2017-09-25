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
                new Vector3( 0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,- 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
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
        
    }
}
