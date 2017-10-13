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
    internal class Mesh
    {
        /// <summary>
        /// The user-defined name of this mesh.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of mesh primitives in the mesh
        /// </summary>
        public List<MeshPrimitive> MeshPrimitives { get; set; }

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
            MeshPrimitives = new List<Runtime.MeshPrimitive>();
        }
        /// <summary>
        /// Adds mesh primitive to mesh
        /// </summary>
        /// <param name="meshPrimitive"></param>
        public void AddPrimitive(Runtime.MeshPrimitive meshPrimitive)
        {
            MeshPrimitives.Add(meshPrimitive);
        }
        /// <summary>
        /// Converts the mesh to schema
        /// </summary>
        /// <param name="bufferViews"></param>
        /// <param name="accessors"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="materials"></param>
        /// <param name="geometryData"></param>
        /// <param name="gBuffer"></param>
        /// <returns>glTFLoader.Schema.Mesh</returns>
        public glTFLoader.Schema.Mesh ConvertToSchema(Runtime.GLTF gltf, List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, List<glTFLoader.Schema.Material> materials, Data geometryData, ref glTFLoader.Schema.Buffer buffer, int bufferIndex)
        {
            glTFLoader.Schema.Mesh mesh = new glTFLoader.Schema.Mesh();
            List<glTFLoader.Schema.MeshPrimitive> primitives = new List<glTFLoader.Schema.MeshPrimitive>(MeshPrimitives.Count);
            List<float> weights = new List<float>();
            // Loops through each wrapped mesh primitive within the mesh and converts them to mesh primitives, as well as updating the
            // indices in the lists
            foreach (Runtime.MeshPrimitive gPrimitive in MeshPrimitives)
            {
                glTFLoader.Schema.MeshPrimitive mPrimitive = gPrimitive.ConvertToSchema(gltf, bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, bufferIndex);
                if (gPrimitive.MorphTargets != null && gPrimitive.MorphTargets.Count() > 0)
                {
                    List<Dictionary<string, int> > morphTargetAttributes = gPrimitive.GetMorphTargets(bufferViews, accessors, ref buffer, geometryData, ref weights, bufferIndex);
                    mPrimitive.Targets = morphTargetAttributes.ToArray();
                }
                primitives.Add(mPrimitive);
            }
            if (Name != null)
            {
                mesh.Name = Name;
            }
            if (MeshPrimitives != null && primitives.Count > 0)
            {
                mesh.Primitives = primitives.ToArray();
            }
            if (weights.Count > 0)
            {
                mesh.Weights = weights.ToArray();
            }

            return mesh;
        }
    }
}
