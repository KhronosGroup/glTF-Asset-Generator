using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Runtime abstraction for glTF Mesh Primitive
    /// </summary>
    internal class MeshPrimitive
    {
        public bool? Interleave { get; set; }

        /// <summary>
        /// Material for the mesh primitive
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// List of Position/Vertices for the mesh primitive
        /// </summary>
        public Accessor Positions { get; set; }

        /// <summary>
        /// List of normals for the mesh primitive
        /// </summary>
        public Accessor Normals { get; set; }

        /// <summary>
        /// List of tangents for the mesh primitive
        /// </summary>
        public Accessor Tangents { get; set; }

        /// <summary>
        /// List of indices for the mesh primitive
        /// </summary>
        public Accessor Indices { get; set; }

        /// <summary>
        /// List of colors for the mesh primitive
        /// </summary>
        public Accessor Colors { get; set; }

        /// <summary>
        /// List of texture coordinate sets (as lists of Vector2) 
        /// </summary>
        public Accessor TextureCoordSets { get; set; }

        /// <summary>
        /// List of morph targets
        /// </summary>
        public IEnumerable<MeshPrimitive> MorphTargets { get; set; }

        /// <summary>
        /// Morph target weight (when the mesh primitive is used as a morph target)
        /// </summary>
        public float MorphTargetWeight { get; set; }

        public enum ModeEnum { TRIANGLES, POINTS, LINES, LINE_LOOP, LINE_STRIP, TRIANGLE_STRIP, TRIANGLE_FAN };

        /// <summary>
        /// Sets the mode of the primitive to render.
        /// </summary>
        public ModeEnum Mode { get; set; }

        public enum WeightComponentTypeEnum { FLOAT, NORMALIZED_UNSIGNED_BYTE, NORMALIZED_UNSIGNED_SHORT };

        public WeightComponentTypeEnum WeightComponentType { get; set; }

        public enum JointComponentTypeEnum { UNSIGNED_SHORT, UNSIGNED_BYTE };

        public JointComponentTypeEnum JointComponentType { get; set; }

        public IEnumerable<IEnumerable<JointWeight>> VertexJointWeights { get; set; }
    }
}
