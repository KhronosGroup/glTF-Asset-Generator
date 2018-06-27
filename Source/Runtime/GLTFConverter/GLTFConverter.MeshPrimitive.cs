using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal partial class GLTFConverter
    {

        /// <summary>
        /// Converts runtime mesh primitive to schema.
        /// </summary>
        private glTFLoader.Schema.MeshPrimitive ConvertMeshPrimitiveToSchema(MeshPrimitive runtimeMeshPrimitive)
        {
            var mPrimitive = CreateInstance<glTFLoader.Schema.MeshPrimitive>();
            var attributes = new Dictionary<string, int>();
            if (runtimeMeshPrimitive.Interleave != null && runtimeMeshPrimitive.Interleave == true)
            {
                attributes = InterleaveMeshPrimitiveAttributes(runtimeMeshPrimitive, geometryData, bufferIndex);
            }
            else
            {
                var positions = runtimeMeshPrimitive.Vertices.Where(vertex => vertex.Position != null).Select(vertex => vertex.Position.Value);

                if (positions.Count() > 0)
                {
                    //Create BufferView for the position
                    int byteLength = sizeof(float) * 3 * positions.Count();
                    float[] min = new float[] { };
                    float[] max = new float[] { };

                    var minMaxPositions = GetMinMaxPositions(runtimeMeshPrimitive);

                    //get the max and min values
                    min = new[] { minMaxPositions[0].X, minMaxPositions[0].Y, minMaxPositions[0].Z };
                    max = new[] { minMaxPositions[1].X, minMaxPositions[1].Y, minMaxPositions[1].Z };
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                    var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                    accessors.Add(accessor);
                    geometryData.Writer.Write(positions.ToArray());
                    attributes.Add("POSITION", accessors.Count - 1);
                }
                var normals = runtimeMeshPrimitive.Vertices.Where(vertex => vertex.Normal != null).Select(vertex => vertex.Normal.Value);
                if (normals.Count() > 0)
                {
                    // Create BufferView
                    int byteLength = sizeof(float) * 3 * normals.Count();
                    // Create a bufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);

                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                    accessors.Add(accessor);
                    geometryData.Writer.Write(normals.ToArray());
                    attributes.Add("NORMAL", accessors.Count() - 1);
                }
                var tangents = runtimeMeshPrimitive.Vertices.Where(vertex => vertex.Tangent != null).Select(vertex => vertex.Tangent.Value);
                if (tangents.Count() > 0)
                {
                    // Create BufferView
                    int byteLength = sizeof(float) * 4 * tangents.Count();
                    // Create a bufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);


                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, null);
                    accessors.Add(accessor);
                    geometryData.Writer.Write(tangents.ToArray());
                    attributes.Add("TANGENT", accessors.Count() - 1);
                }
                var colors = runtimeMeshPrimitive.Vertices.Where(vertex => vertex.Color != null).Select(vertex => vertex.Color.Value);
                if (colors.Count() > 0)
                {
                    var colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                    var colorAccessorType = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? glTFLoader.Schema.Accessor.TypeEnum.VEC3 : glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                    int vectorSize = runtimeMeshPrimitive.ColorType == MeshPrimitive.ColorTypeEnum.VEC3 ? 3 : 4;

                    // Create BufferView
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;

                    int byteLength = WriteColors(runtimeMeshPrimitive, colors, 0, colors.Count() - 1, geometryData);
                    int? byteStride = null;
                    switch (runtimeMeshPrimitive.ColorComponentType)
                    {
                        case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE:
                            colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                            if (vectorSize == 3)
                            {
                                byteStride = 4;
                            }
                            break;
                        case MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT:
                            colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            if (vectorSize == 3)
                            {
                                byteStride = 8;
                            }
                            break;
                        default: //Default to ColorComponentTypeEnum.FLOAT:
                            colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            break;
                    }

                    var bufferView = CreateBufferView(bufferIndex, "Colors", byteLength, byteOffset, byteStride);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    // we normalize if the color accessor mode is not set to FLOAT
                    bool normalized = runtimeMeshPrimitive.ColorComponentType != MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    var accessor = CreateAccessor(bufferviewIndex, 0, colorAccessorComponentType, colors.Count(), "Colors Accessor", null, null, colorAccessorType, normalized);
                    accessors.Add(accessor);
                    attributes.Add("COLOR_0", accessors.Count() - 1);
                    if (normalized)
                    {
                        // Pad any additional bytes if byteLength is not a multiple of 4
                        Align(geometryData, byteLength, 4);
                    }
                }
                var runtimeVertex = runtimeMeshPrimitive.Vertices.Count() > 0 ? runtimeMeshPrimitive.Vertices.ElementAt(0) : null;

                var count = (runtimeVertex != null && runtimeVertex.TextureCoordSet != null) ? runtimeVertex.TextureCoordSet.Count() : 0;
                
                for (int i = 0; i < count; ++i)
                {
                    var texCoordSets = runtimeMeshPrimitive.Vertices.Where(vertex => vertex.TextureCoordSet != null && vertex.TextureCoordSet.Count() > i).Select(vertex => vertex.TextureCoordSet.ElementAt(i));
                    if (texCoordSets.Count() > 0)
                    {
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        int byteLength = WriteTextureCoords(runtimeMeshPrimitive, texCoordSets, 0, texCoordSets.Count() - 1, geometryData);

                        glTFLoader.Schema.Accessor accessor;
                        glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                        // we normalize only if the texture cood accessor type is not float
                        bool normalized = runtimeMeshPrimitive.TextureCoordsComponentType != MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT;
                        int? byteStride = null;
                        switch (runtimeMeshPrimitive.TextureCoordsComponentType)
                        {
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT:
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                                break;
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                                byteStride = 4;
                                break;
                            case MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                                break;
                            default: // Default to Float
                                accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                                break;
                        }

                        var bufferView = CreateBufferView(bufferIndex, "Texture Coords " + i, byteLength, byteOffset, byteStride);
                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;
                        // Create Accessor
                        accessor = CreateAccessor(bufferviewIndex, 0, accessorComponentType, texCoordSets.Count(), "UV Accessor " + i, null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, normalized);

                        accessors.Add(accessor);

                        // Add any additional bytes if the data is normalized
                        if (normalized)
                        {
                            // Pad any additional bytes if byteLength is not a multiple of 4
                            Align(geometryData, byteLength, 4);
                        }
                        attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                var joints = runtimeMeshPrimitive.Vertices.Where(vertex => vertex.Joints != null).Select(vertex => vertex.Joints);
                if (joints.Count() > 0)
                {
                    List<int> jointIndices = new List<int>();
                    foreach (var vertexJoint in joints)
                    {
                        if (vertexJoint.Count() == 4)
                        {
                            foreach (var joint in vertexJoint)
                            {
                                jointIndices.Add(joint.SkinJointIndex);
                            }
                        }
                        else
                        {
                            throw new Exception("The vertex does not have four joint values!");
                        }
                    }
                    int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                    glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                    switch (runtimeMeshPrimitive.JointsComponentType)
                    {
                        case MeshPrimitive.JointsComponentTypeEnum.UNSIGNED_BYTE:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                            foreach (var jointIndex in jointIndices)
                            {
                                geometryData.Writer.Write(Convert.ToByte(jointIndex));
                            }
                            break;
                        case MeshPrimitive.JointsComponentTypeEnum.UNSIGNED_SHORT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            foreach (var jointIndex in jointIndices)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(jointIndex));
                            }
                            break;
                        default:
                            throw new NotSupportedException("Joints accessor component type not supported!");
                    }
                    int byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;
                    // create buffer view for joints
                    var bufferView = CreateBufferView(bufferIndex, "Joints buffer view", byteLength, byteOffset, null);
                    bufferViews.Add(bufferView);
                    var bufferViewIndex = bufferViews.Count() - 1;

                    // create accessor for the joints
                    var accessor = CreateAccessor(bufferViewIndex, byteOffset, accessorComponentType, jointIndices.Count(), "Joints Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, false);
                    accessors.Add(accessor);
                }
                var weights = GetWeights(joints);
                if (weights.Count() > 0)
                {
                    // Write weights data to buffer

                    glTFLoader.Schema.Accessor.ComponentTypeEnum weightsComponentType;
                    var byteOffset = (int)geometryData.Writer.BaseStream.Position;

                    switch (runtimeMeshPrimitive.WeightsComponentType)
                    {
                        case MeshPrimitive.WeightsComponentTypeEnum.FLOAT:
                            weightsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            foreach (var weight in weights)
                            {
                                geometryData.Writer.Write(weight);
                            }
                            break;
                        case MeshPrimitive.WeightsComponentTypeEnum.UNSIGNED_BYTE:
                            weightsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.BYTE;
                            foreach (var weight in weights)
                            {
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(weight.X * byte.MaxValue)));
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(weight.Y * byte.MaxValue)));
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(weight.Z * byte.MaxValue)));
                                geometryData.Writer.Write(Convert.ToByte(Math.Round(weight.W * byte.MaxValue)));
                            }
                            break;
                        case MeshPrimitive.WeightsComponentTypeEnum.UNSIGNED_SHORT:
                            weightsComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            foreach (var weight in weights)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(weight.X * ushort.MaxValue)));
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(weight.Y * ushort.MaxValue)));
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(weight.Z * ushort.MaxValue)));
                                geometryData.Writer.Write(Convert.ToUInt16(Math.Round(weight.W * ushort.MaxValue)));
                            }
                            break;
                        default:
                            throw new NotSupportedException("weight type not supported!");
                    }
                    var byteLength = (int)geometryData.Writer.BaseStream.Position - byteOffset;
                    // create buffer view for the weights
                    var weightsBufferView = CreateBufferView(bufferIndex, "weights buffer view", byteLength, byteOffset, null);
                    bufferViews.Add(weightsBufferView);
                    var bufferViewIndex = bufferViews.Count() - 1;
                    // create an accessor for the weights
                    var weightsAccessor = CreateAccessor(bufferViewIndex, byteOffset, weightsComponentType, weights.Count(), "Weights accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC4, false);

                }
            }
            if (runtimeMeshPrimitive.Indices != null && runtimeMeshPrimitive.Indices.Count() > 0)
            {
                int byteLength;
                int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                glTFLoader.Schema.Accessor.ComponentTypeEnum indexComponentType;

                switch (runtimeMeshPrimitive.IndexComponentType)
                {
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE:
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                        byteLength = sizeof(byte) * runtimeMeshPrimitive.Indices.Count();
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT:
                        byteLength = sizeof(ushort) * runtimeMeshPrimitive.Indices.Count();
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                        break;
                    case MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT:
                        byteLength = sizeof(uint) * runtimeMeshPrimitive.Indices.Count();
                        indexComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT;
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unrecognized Index Component Type Enum " + runtimeMeshPrimitive.IndexComponentType);
                }
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Indices", byteLength, byteOffset, null);
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                var accessor = CreateAccessor(bufferviewIndex, 0, indexComponentType, runtimeMeshPrimitive.Indices.Count(), "Indices Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                accessors.Add(accessor);
                switch (indexComponentType)
                {
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToUInt32(index));
                        }
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToByte(index));
                        }
                        break;
                    case glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        foreach (var index in runtimeMeshPrimitive.Indices)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(index));
                        }
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported Index Component Type");
                }

                mPrimitive.Indices = accessors.Count() - 1;
            }

            mPrimitive.Attributes = attributes;
            if (runtimeMeshPrimitive.Material != null)
            {
                var nMaterial = ConvertMaterialToSchema(runtimeMeshPrimitive.Material, runtimeGLTF);
                materials.Add(nMaterial);
                mPrimitive.Material = materials.Count() - 1;
            }

            if (runtimeMeshPrimitive.Mode.HasValue)
            {
                switch (runtimeMeshPrimitive.Mode)
                {
                    case MeshPrimitive.ModeEnum.POINTS:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.POINTS;
                        break;
                    case MeshPrimitive.ModeEnum.LINES:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINES;
                        break;
                    case MeshPrimitive.ModeEnum.LINE_LOOP:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_LOOP;
                        break;
                    case MeshPrimitive.ModeEnum.LINE_STRIP:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_STRIP;
                        break;
                    case MeshPrimitive.ModeEnum.TRIANGLES:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLES;
                        break;
                    case MeshPrimitive.ModeEnum.TRIANGLE_FAN:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_FAN;
                        break;
                    case MeshPrimitive.ModeEnum.TRIANGLE_STRIP:
                        mPrimitive.Mode = glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_STRIP;
                        break;
                }
            }

            return mPrimitive;
        }

        private IEnumerable<Vector4> GetWeights(IEnumerable<IEnumerable<VertexJoint>> vertexJoints)
        {
            var weights = new List<Vector4>();
            vertexJoints.ForEach(vertexJoint =>
            {
                var weight = new Vector4();
                if (vertexJoint.Count() == 4)
                {
                    weight.X = vertexJoint.ElementAt(0).Weight;
                    weight.Y = vertexJoint.ElementAt(1).Weight;
                    weight.Z = vertexJoint.ElementAt(2).Weight;
                    weight.W = vertexJoint.ElementAt(3).Weight;
                    weights.Add(weight);
                }
                else
                {
                    throw new IndexOutOfRangeException("Vertex joint does not have four elements!");
                }
            });

            return weights;
        }
    }

}
