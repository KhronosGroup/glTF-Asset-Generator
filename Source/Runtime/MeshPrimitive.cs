using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Runtime abstraction for glTF Mesh Primitive
    /// </summary>
    public class MeshPrimitive
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

        /// <summary>
        /// Sets the mode of the primitive to render.
        /// </summary>
        public glTFLoader.Schema.MeshPrimitive.ModeEnum Mode { get; set; }

        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as an array of two vectors, minimum and maximum respectively</returns>
        public Vector3[] GetMinMaxPositions()
        {
            //get the max and min values
            Vector3 minVal = new Vector3
            {
                x = float.MaxValue,
                y = float.MaxValue,
                z = float.MaxValue
            };
            Vector3 maxVal = new Vector3
            {
                x = float.MinValue,
                y = float.MinValue,
                z = float.MinValue
            };
            foreach (Vector3 position in Positions)
            {
                maxVal.x = Math.Max(position.x, maxVal.x);
                maxVal.y = Math.Max(position.y, maxVal.y);
                maxVal.z = Math.Max(position.z, maxVal.z);

                minVal.x = Math.Min(position.x, minVal.x);
                minVal.y = Math.Min(position.y, minVal.y);
                minVal.z = Math.Min(position.z, minVal.z);
            }
            Vector3[] results = { minVal, maxVal };
            return results;
        }

        /// <summary>
        /// Creates a BufferView object
        /// </summary>
        /// <param name="gBuffer"></param>
        /// <param name="name"></param>
        /// <param name="byteLength"></param>
        /// <param name="byteOffset"></param>
        /// <returns>BufferView</returns>
        private glTFLoader.Schema.BufferView CreateBufferView(int bufferIndex, string name, int byteLength, int byteOffset)
        {
            glTFLoader.Schema.BufferView bufferView = new glTFLoader.Schema.BufferView
            {
                Name = name,
                ByteLength = byteLength,
                ByteOffset = byteOffset,
                Buffer = bufferIndex
            };
            return bufferView;
        }

        /// <summary>
        /// Creates an Accessor object
        /// </summary>
        /// <param name="bufferviewIndex"></param>
        /// <param name="byteOffset"></param>
        /// <param name="componentType"></param>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="type"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private glTFLoader.Schema.Accessor CreateAccessor(int bufferviewIndex, int? byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, glTFLoader.Schema.Accessor.TypeEnum? type, bool? normalized)
        {
            glTFLoader.Schema.Accessor accessor = new glTFLoader.Schema.Accessor
            {
                BufferView = bufferviewIndex,
                Name = name,
            };
            if (min != null && min.Count() > 0)
            {
                accessor.Min = min;
            };
            if (max != null && max.Count() > 0)
            {
                accessor.Max = max;
            }
            if (componentType.HasValue)
            {
                accessor.ComponentType = componentType.Value;
            }
            if (byteOffset.HasValue)
            {
                accessor.ByteOffset = byteOffset.Value;
            }
            if (count.HasValue)
            {
                accessor.Count = count.Value;
            }
            if (type.HasValue)
            {
                accessor.Type = type.Value;
            }
            if (normalized.HasValue)
            {
                accessor.Normalized = normalized.Value;
            }   
            return accessor;
        }
        /// <summary>
        /// Converts the wrapped mesh primitive into gltf mesh primitives, as well as updates the indices in the lists
        /// </summary>
        /// <param name="bufferViews"></param>
        /// <param name="accessors"></param>
        /// <param name="samplers"></param>
        /// <param name="images"></param>
        /// <param name="textures"></param>
        /// <param name="materials"></param>
        /// <param name="geometryData"></param>
        /// <param name="gBuffer"></param>
        /// <returns>MeshPrimitive instance</returns>
        public glTFLoader.Schema.MeshPrimitive ConvertToMeshPrimitive(List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, List<glTFLoader.Schema.Material> materials, Data geometryData, ref glTFLoader.Schema.Buffer buffer, int bufferIndex)
        {
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            glTFLoader.Schema.MeshPrimitive mPrimitive = new glTFLoader.Schema.MeshPrimitive();

            if (Positions != null)
            {
                //Create BufferView for the position
                int byteLength = sizeof(float) * 3 * Positions.Count();
                float[] min = new float[] { };
                float[] max = new float[] { };
            
                //get the max and min values
                Vector3[] minMaxPositions = GetMinMaxPositions();
                max = new[] { minMaxPositions[0].x, minMaxPositions[0].y, minMaxPositions[0].z };
                min = new[] { minMaxPositions[1].x, minMaxPositions[1].y, minMaxPositions[1].z };
                
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, buffer.ByteLength);
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                // Create an accessor for the bufferView
                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, Positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                buffer.ByteLength += byteLength;
             
                accessors.Add(accessor);
                geometryData.Writer.Write(Positions.ToArray());
                attributes.Add("POSITION", accessors.Count() - 1);
            }
            if (Normals != null)
            {
                // Create BufferView
                int byteLength = sizeof(float) * 3 * Normals.Count();
                // Create a bufferView
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, buffer.ByteLength);
                
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                // Create an accessor for the bufferView
                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                
                buffer.ByteLength += byteLength;
                accessors.Add(accessor);
                geometryData.Writer.Write(Normals.ToArray());
                attributes.Add("NORMAL", accessors.Count() - 1);
            }
            if (Tangents != null && Tangents.Count > 0)
            {
                // Create BufferView
                int byteLength = sizeof(float) * 3 * Tangents.Count();
                // Create a bufferView
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, buffer.ByteLength);

               
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;
                
                // Create an accessor for the bufferView
                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                buffer.ByteLength += byteLength;
                accessors.Add(accessor);
                geometryData.Writer.Write(Tangents.ToArray());
                attributes.Add("TANGENT", accessors.Count() - 1);
            }
            if (Indices != null && Indices.Count() > 0)
            {
                int byteLength = sizeof(int) * Indices.Count();
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Indices", byteLength, buffer.ByteLength);

                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;

                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT, Indices.Count(), "Indices Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                buffer.ByteLength += byteLength;
                accessors.Add(accessor);
                foreach(var indice in Indices)
                {
                    geometryData.Writer.Write((uint)indice);
                }
                mPrimitive.Indices = accessors.Count() - 1;
            }
            if (Colors != null)
            {                
                glTFLoader.Schema.Accessor.ComponentTypeEnum colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                glTFLoader.Schema.Accessor.TypeEnum colorAccessorType = ColorType == ColorTypeEnum.VEC3 ? glTFLoader.Schema.Accessor.TypeEnum.VEC3 : glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                int vectorSize = ColorType == ColorTypeEnum.VEC3 ? 3 : 4;
                int byteLength = sizeof(float) * vectorSize * Colors.Count();

                switch (ColorComponentType)
                {
                    case ColorComponentTypeEnum.FLOAT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;

                        foreach (Vector4 color in Colors)
                        {
                            geometryData.Writer.Write(color.x);
                            geometryData.Writer.Write(color.y);
                            geometryData.Writer.Write(color.z);
                            if (colorAccessorType == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                            {
                                geometryData.Writer.Write(color.w);
                            }
                        }
                        break;
                    case ColorComponentTypeEnum.NORMALIZED_UBYTE:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                    
                        foreach (Vector4 color in Colors)
                        {
                            geometryData.Writer.Write(Convert.ToByte(color.x));
                            geometryData.Writer.Write(Convert.ToByte(color.y));
                            geometryData.Writer.Write(Convert.ToByte(color.z));
                            if (colorAccessorType == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                            {
                                geometryData.Writer.Write(Convert.ToByte(color.w));
                            }
                        }
                        break;
                    case ColorComponentTypeEnum.NORMALIZED_USHORT:
                        colorAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                       
                        foreach (Vector4 color in Colors)
                        {
                            geometryData.Writer.Write(Convert.ToUInt16(color.x));
                            geometryData.Writer.Write(Convert.ToUInt16(color.y));
                            geometryData.Writer.Write(Convert.ToUInt16(color.z));
                            if (colorAccessorType == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(color.w));
                            }
                        }
                        break;
                }
                // Create BufferView
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Colors", byteLength, buffer.ByteLength);
                bufferViews.Add(bufferView);
                int bufferviewIndex = bufferViews.Count() - 1;
                buffer.ByteLength += byteLength;

                // Create an accessor for the bufferView
                // we normalize if the color accessor mode is not set to FLOAT
                bool normalized = ColorComponentType != ColorComponentTypeEnum.FLOAT;
                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, colorAccessorComponentType, Colors.Count(), "Colors Accessor", null, null, colorAccessorType, normalized);
                accessors.Add(accessor);
                attributes.Add("COLOR_0", accessors.Count() - 1);
                if (normalized)
                {
                    // Pad any additional bytes if byteLength is not a multiple of 4
                    int additionalPaddedBytes = Align(byteLength, 4) - byteLength;
                    Enumerable.Range(0, additionalPaddedBytes).ForEach(arg => geometryData.Writer.Write((byte)0));
                    buffer.ByteLength += additionalPaddedBytes;
                }
            }
            if (TextureCoordSets != null)
            {
                for (int i = 0; i < TextureCoordSets.Count; ++i)
                {
                    List<Vector2> textureCoordSet = TextureCoordSets[i];
                    int byteLength;

                    
                    glTFLoader.Schema.Accessor accessor;
                    glTFLoader.Schema.Accessor.ComponentTypeEnum accessorComponentType;
                    // we normalize only if the texture cood accessor type is not float
                    bool normalized = TextureCoordsComponentType != TextureCoordsComponentTypeEnum.FLOAT;
                    switch(TextureCoordsComponentType)
                    {
                        case TextureCoordsComponentTypeEnum.FLOAT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            byteLength = sizeof(float) * 2 * textureCoordSet.Count();
                            break;
                        case TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE;
                            byteLength = sizeof(byte) * 2 * textureCoordSet.Count();
                            break;
                        case TextureCoordsComponentTypeEnum.NORMALIZED_USHORT:
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                            byteLength = sizeof(ushort) * 2 * textureCoordSet.Count();
                            break;
                        default: // Default to Float
                            accessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
                            byteLength = sizeof(float) * 2 * textureCoordSet.Count();
                            break;
                    }
                    glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Texture Coords " + (i + 1), byteLength, buffer.ByteLength);
                    bufferViews.Add(bufferView);
                    int bufferviewIndex = bufferViews.Count() - 1; 
                    // Create Accessor
                    accessor = CreateAccessor(bufferviewIndex, 0, accessorComponentType, textureCoordSet.Count(), "UV Accessor " + (i + 1), null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC2, normalized);

                    buffer.ByteLength += byteLength;
                    
                    accessors.Add(accessor);
                    Vector2[] textureCoordSetArr = textureCoordSet.ToArray();
                    if (accessor.Normalized && !accessor.ComponentType.Equals(glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT))
                    {
                        if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE)
                        {
                            foreach (Vector2 tcs in textureCoordSetArr)
                            {
                                geometryData.Writer.Write(Convert.ToByte(tcs.x));
                                geometryData.Writer.Write(Convert.ToByte(tcs.y));
                            }
                        }
                        else if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
                        {
                            foreach(Vector2 tcs in textureCoordSetArr)
                            {
                                geometryData.Writer.Write(Convert.ToUInt16(tcs.x));
                                geometryData.Writer.Write(Convert.ToUInt16(tcs.y));
                            }
                        }
                    }
                    else // write float
                    {
                        geometryData.Writer.Write(textureCoordSetArr);
                    }
                    // Add any additional bytes if the data is normalized
                    if (normalized)
                    {
                        // Pad any additional bytes if byteLength is not a multiple of 4
                        int additionalPaddedBytes = Align(byteLength, 4) - byteLength;
                        Enumerable.Range(0, additionalPaddedBytes).ForEach(arg => geometryData.Writer.Write((byte)0));
                        buffer.ByteLength += additionalPaddedBytes;
                    }
                    attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);
                }
            }
            
            mPrimitive.Attributes = attributes;
            if (Material != null)
            {
                glTFLoader.Schema.Material nMaterial = Material.CreateMaterial(samplers, images, textures);
                materials.Add(nMaterial);
                mPrimitive.Material = materials.Count() - 1;
            }
            return mPrimitive;
        }
        /// <summary>
        /// Pads the value of the values to ensure it is a multiple of size
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private int Align(int value, int size)
        {
            var remainder = value % size;
            return (remainder == 0 ? value : checked(value + size - remainder));
        }
        /// <summary>
        /// Converts the morph target list of dictionaries into Morph Target
        /// </summary>
        public List<Dictionary<string, int>> GetMorphTargets(List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, ref glTFLoader.Schema.Buffer buffer, Data geometryData, ref List<float> weights,  int bufferIndex)
        {
            List<Dictionary<string, int>> morphTargetDicts = new List<Dictionary<string, int>>();
            if (MorphTargets != null)
            {
                foreach(MeshPrimitive morphTarget in MorphTargets)
                {
                    Dictionary<string, int> morphTargetAttributes = new Dictionary<string, int>();
                    

                    if (morphTarget.Positions != null && morphTarget.Positions.Count > 0)
                    {
                        if (morphTarget.Positions != null)
                        {
                            //Create BufferView for the position
                            int byteLength = sizeof(float) * 3 * morphTarget.Positions.Count();
                            
                            glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, buffer.ByteLength);

                            bufferViews.Add(bufferView);
                            int bufferviewIndex = bufferViews.Count() - 1;

                            // Create an accessor for the bufferView
                            glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Positions.Count(), "Positions Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                            buffer.ByteLength += byteLength;
                            accessors.Add(accessor);
                            geometryData.Writer.Write(morphTarget.Positions.ToArray());
                            morphTargetAttributes.Add("POSITION", accessors.Count() - 1);
                        }
                    }
                    if (morphTarget.Normals != null && morphTarget.Normals.Count > 0)
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Normals.Count();
                        // Create a bufferView
                        glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, buffer.ByteLength);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        buffer.ByteLength += byteLength;
                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Normals.ToArray());
                        morphTargetAttributes.Add("NORMAL", accessors.Count() - 1);
                    }
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Count > 0)
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Tangents.Count();
                        // Create a bufferView
                        glTFLoader.Schema.BufferView bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, buffer.ByteLength);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        buffer.ByteLength += byteLength;
                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Tangents.ToArray());
                        morphTargetAttributes.Add("TANGENT", accessors.Count() - 1);
                    }
                    morphTargetDicts.Add(new Dictionary<string, int> (morphTargetAttributes));
                    weights.Add(morphTargetWeight);
                }
            }
            return morphTargetDicts;
        }
    }
}
