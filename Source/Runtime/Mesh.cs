using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Mesh
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// The user-defined name of this mesh.
        /// </summary>
        public string name;

        /// <summary>
        /// List of mesh primitives in the mesh
        /// </summary>
        public List<Runtime.MeshPrimitive> meshPrimitives;

        /// <summary>
        /// Transformation Matrix which performs translation, rotation and scale operations on the mesh
        /// </summary>
        public Matrix4x4 TransformationMatrix { get; set; }
        /// <summary>
        /// Rotation Quaternion for the mesh
        /// </summary>
        public Quaternion Rotation { get; set; }
        /// <summary>
        /// Translation Vector for the mesh.
        /// </summary>
        public Vector3? Translation { get; set; }
        /// <summary>
        /// Scale Vector for the mesh.
        /// </summary>
        public Vector3? Scale { get; set; }
        /// <summary>
        /// Initializes the Mesh
        /// </summary>
        public Mesh()
        {
            meshPrimitives = new List<Runtime.MeshPrimitive>();
        }
        /// <summary>
        /// Adds mesh primitive to mesh
        /// </summary>
        /// <param name="meshPrimitive"></param>
        public void AddPrimitive(Runtime.MeshPrimitive meshPrimitive)
        {
            meshPrimitives.Add(meshPrimitive);
        }
        /// <summary>
        /// Converts the wrapped mesh into a GLTF Mesh object.
        /// </summary>
        /// <param name="bufferViews"></param>
        /// <param name="accessors"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="materials"></param>
        /// <param name="geometryData"></param>
        /// <param name="gBuffer"></param>
        /// <returns></returns>
        public glTFLoader.Schema.Mesh ConvertToMesh(List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, List<glTFLoader.Schema.Material> materials, Data geometryData, ref glTFLoader.Schema.Buffer buffer, int buffer_index, int buffer_offset)
        {
            glTFLoader.Schema.Mesh mesh = new glTFLoader.Schema.Mesh();
            List<glTFLoader.Schema.MeshPrimitive> primitives = new List<glTFLoader.Schema.MeshPrimitive>(meshPrimitives.Count);
            // Loops through each wrapped mesh primitive within the mesh and converts them to mesh primitives, as well as updating the
            // indices in the lists
            foreach (Runtime.MeshPrimitive gPrimitive in meshPrimitives)
            {
                glTFLoader.Schema.MeshPrimitive mPrimitive = gPrimitive.ConvertToMeshPrimitive(bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, buffer_index, buffer_offset);
                primitives.Add(mPrimitive);
            }
            if (name != null)
            {
                mesh.Name = name;
            }
            if (meshPrimitives != null)
            {
                mesh.Primitives = primitives.ToArray();
            }

            return mesh;
        }
    }
}
