using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Runtime abstraction for glTF Mesh Primitive
    /// </summary>
    internal class MeshPrimitive
    {
        /// <summary>
        /// Specifies which component type to use when defining the color accessor 
        /// </summary>
        public enum ColorComponentTypeEnum { FLOAT, NORMALIZED_USHORT, NORMALIZED_UBYTE };

        /// <summary>
        /// Specifies which data type to use when defining the color accessor
        /// </summary>
        public enum ColorTypeEnum { VEC3, VEC4 };

        /// <summary>
        /// Specifies which color component type to use for the mesh primitive instance
        /// </summary>
        public ColorComponentTypeEnum ColorComponentType { get; set; }

        /// <summary>
        /// Specifies which color data type to use for the mesh primitive instance
        /// </summary>
        public ColorTypeEnum ColorType { get; set; }

        /// <summary>
        /// Specifies which component type to use when defining the texture coordinates accessor 
        /// </summary>
        public enum TextureCoordsComponentTypeEnum { FLOAT, NORMALIZED_USHORT, NORMALIZED_UBYTE };

        /// <summary>
        /// Specifies which texture coords component type to use for the mesh primitive instance
        /// </summary>
        public TextureCoordsComponentTypeEnum TextureCoordsComponentType { get; set; }

        /// <summary>
        /// Material for the mesh primitive
        /// </summary>
        public Runtime.Material Material { get; set; }

        /// <summary>
        /// List of Position/Vertices for the mesh primitive
        /// </summary>
        public List<Vector3> Positions { get; set; }

        /// <summary>
        /// List of normals for the mesh primitive
        /// </summary>
        public List<Vector3> Normals { get; set; }

        /// <summary>
        /// List of tangents for the mesh primitive
        /// </summary>
        public List<Vector4> Tangents { get; set; }

        /// <summary>
        /// Available component types to use when defining the indices accessor
        /// </summary>
        public enum IndexComponentTypeEnum { UNSIGNED_INT, UNSIGNED_BYTE, UNSIGNED_SHORT };

        /// <summary>
        /// Specifices which component type to use when defining the indices accessor
        /// </summary>
        public IndexComponentTypeEnum IndexComponentType { get; set; }

        /// <summary>
        /// List of indices for the mesh primitive
        /// </summary>
        public List<int> Indices { get; set; }

        /// <summary>
        /// List of colors for the mesh primitive
        /// </summary>
        public List<Vector4> Colors { get; set; }

        /// <summary>
        /// List of texture coordinate sets (as lists of Vector2) 
        /// </summary>
        public List<List<Vector2>> TextureCoordSets { get; set; }

        /// <summary>
        /// List of morph targets
        /// </summary>
        public List<MeshPrimitive> MorphTargets { get; set; }

        /// <summary>
        /// morph target weight (when the mesh primitive is used as a morph target)
        /// </summary>
        public float morphTargetWeight { get; set; }

        public enum ModeEnum { POINTS, LINES, LINE_LOOP, LINE_STRIP, TRIANGLES, TRIANGLE_STRIP, TRIANGLE_FAN };

        /// <summary>
        /// Sets the mode of the primitive to render.
        /// </summary>
        public ModeEnum? Mode { get; set; }

    }
}
