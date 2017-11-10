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

    }
}
