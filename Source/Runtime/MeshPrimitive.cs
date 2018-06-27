using System;
using System.Collections.Generic;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Runtime abstraction for glTF Mesh Primitive
    /// </summary>
    internal class MeshPrimitive
    {
        /// <summary>
        /// List of vertices in the mesh primitive
        /// </summary>
        public IEnumerable<MeshPrimitiveVertex> Vertices { get; set; }
        /// <summary>
        /// Specifies which component type to use when defining the color accessor 
        /// </summary>
        public enum ColorComponentTypeEnum { FLOAT, NORMALIZED_USHORT, NORMALIZED_UBYTE };

        /// <summary>
        /// Specifies which data type to use when defining the color accessor
        /// </summary>
        public enum ColorTypeEnum { VEC3, VEC4 };

        public bool? Interleave { get; set; }

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
        public IEnumerable<int> Indices { get; set; }

        public enum JointsComponentTypeEnum { UNSIGNED_BYTE, UNSIGNED_SHORT};

        public JointsComponentTypeEnum JointsComponentType { get; set; }

        /// <summary>
        /// Available component types to use when defining the weights accessor
        /// </summary>
        public enum WeightsComponentTypeEnum { FLOAT, UNSIGNED_BYTE, UNSIGNED_SHORT};

        /// <summary>
        /// Specifies which component type to use when defining the weights accessor
        /// </summary>
        public WeightsComponentTypeEnum WeightsComponentType { get; set; }

        /// <summary>
        /// morph target weight (when the mesh primitive is used as a morph target)
        /// </summary>
        public float morphTargetWeight { get; set; }

        public enum ModeEnum { POINTS, LINES, LINE_LOOP, LINE_STRIP, TRIANGLES, TRIANGLE_STRIP, TRIANGLE_FAN };

        /// <summary>
        /// Sets the mode of the primitive to render.
        /// </summary>
        public ModeEnum? Mode { get; set; }

        public static void SetVertexProperties<T>(IEnumerable<Runtime.MeshPrimitiveVertex> vertices, IEnumerable<T> properties, Action<Runtime.MeshPrimitiveVertex, T> action)
        {
            var verticesEnumerator = vertices.GetEnumerator();
            var propertiesEnumerator = properties.GetEnumerator();

            verticesEnumerator.Reset();
            propertiesEnumerator.Reset();
            while (verticesEnumerator.MoveNext() && propertiesEnumerator.MoveNext())
            {
                action(verticesEnumerator.Current, propertiesEnumerator.Current);
            }
        }

        public void PrintVertices()
        {
            foreach(var vertex in Vertices)
            {
                Console.Write("new Runtime.MeshPrimitiveVertex(");
                if (vertex.Position != null)
                {
                    Console.Write($" position : new Vector3({vertex.Position.Value.X}f, {vertex.Position.Value.Y}f, {vertex.Position.Value.Z}f)");
                }
                if (vertex.Normal != null)
                {
                    Console.Write($", normal : new Vector3({vertex.Normal.Value.X}f, {vertex.Normal.Value.Y}f, {vertex.Normal.Value.Z}f)");
                }
                if (vertex.Tangent != null)
                {
                    Console.Write($", tangent : new Vector4({vertex.Tangent.Value.X}f, {vertex.Tangent.Value.Y}f, {vertex.Tangent.Value.Z}f, {vertex.Tangent.Value.W}f)");
                }
                if (vertex.Color != null)
                {
                    Console.Write($", color : new Vector4({vertex.Color.Value.X}f, {vertex.Color.Value.Y}f, {vertex.Color.Value.Z}f, {vertex.Color.Value.W}f)");
                }
                if (vertex.TextureCoordSet != null)
                {
                    Console.Write($", textureCoordSet : new Vector2[] ");
                    foreach (var tex in vertex.TextureCoordSet)
                    {
                        Console.Write($"{{ new Vector2({tex.X}f, {tex.Y}f) }}");    
                    }
                }
                Console.WriteLine("),");
            }
        }
        
    }
}
