using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// A node in a node hierarchy.  
    /// </summary>
    internal class Node
    {
        /// <summary>
        /// A floating-point 4x4 transformation matrix stored in column-major order.
        /// </summary>
        public Matrix4x4 Matrix { get; set; }

        /// <summary>
        /// The mesh in this node.
        /// </summary>
        public Mesh Mesh { get; set; }

        /// <summary>
        /// The node's unit quaternion rotation in the order (x, y, z, w), where w is the scalar.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The node's non-uniform scale
        /// </summary>
        public Vector3? Scale { get; set; }

        /// <summary>
        /// The node's translation
        /// </summary>
        public Vector3? Translation { get; set; }

        /// <summary>
        /// children of this node.
        /// </summary>
        public List<Node> Children { get; set; }

        /// <summary>
        /// Name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Convert the node to schema
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="meshes"></param>
        /// <param name="accessors"></param>
        /// <param name="materials"></param>
        /// <param name="bufferViews"></param>
        /// <param name="buffer"></param>
        /// <param name="geometryData"></param>
        /// <param name="bufferIndex"></param>
        /// <returns></returns>
        public glTFLoader.Schema.Node ConvertToSchema(Runtime.GLTF gltf, List<glTFLoader.Schema.Node> nodes, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, List<glTFLoader.Schema.Mesh> meshes, List<glTFLoader.Schema.Accessor> accessors, List<glTFLoader.Schema.Material> materials,  List<glTFLoader.Schema.BufferView> bufferViews, ref glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var node = new glTFLoader.Schema.Node();
            if (Name != null)
            {
                node.Name = Name;
            }
            if (Matrix != null)
            {
                node.Matrix = Matrix.ToArray();
            }
            if (Mesh != null)
            {
                var schemaMesh = Mesh.ConvertToSchema(gltf, bufferViews, accessors, samplers, images, textures, materials, geometryData, ref buffer, bufferIndex);
                meshes.Add(schemaMesh);
                node.Mesh = meshes.Count() - 1;
            }
            if (Rotation != null)
            {
                node.Rotation = Rotation.ToArray();
            }
            if (Scale.HasValue)
            {
                node.Scale = Scale.Value.ToArray();
            }
            if (Translation.HasValue)
            {
                node.Translation = Translation.Value.ToArray();
            }
            if (Children != null)
            {
                var childrenIndices = new List<int>();
                foreach (var childNode in Children)
                {
                    var child = childNode.ConvertToSchema(gltf, nodes, samplers, images, textures, meshes, accessors, materials, bufferViews, ref buffer, geometryData, bufferIndex);
                    nodes.Add(child);
                    childrenIndices.Add(nodes.Count() - 1);
                }
                node.Children = childrenIndices.ToArray();
            }

            return node;
        }
    }
}
