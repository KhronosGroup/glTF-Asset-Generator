using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Runtime abstraction for Mesh Primitive
    /// </summary>
    public class MeshPrimitive
    {
        
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

        public List<Vector3> Tangents { get; set; }

        /// <summary>
        /// List of texture coordinate sets (as lists of Vector2) 
        /// </summary>
        public List<List<Vector2>> TextureCoordSets { get; set; }

        public List<MeshPrimitive> MorphTargets { get; set; }
        public float morphTargetWeight { get; set; }

        /// <summary>
        /// Sets the type of primitive to render.
        /// </summary>
        public glTFLoader.Schema.MeshPrimitive.ModeEnum Mode { get; set; }

       
        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as a list of Vector2 lists </returns>
        public Vector3[] GetMinMaxNormals()
        {
            Vector3[] minMaxNormals = GetMinMaxVector3(Normals);

            return minMaxNormals;
        }
        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as an array of two vectors, minimum and maximum respectively</returns>
        public Vector3[] GetMinMaxPositions()
        {
            Vector3[] minMaxPositions = GetMinMaxVector3(Positions);

            return minMaxPositions;
        }
        /// <summary>
        /// Computes and returns the minimum and maximum positions for each texture coordinate
        /// </summary>
        /// <returns>Returns the result as a list of two vectors, minimun and maximum respectively</returns>
        public List<Vector2[]> GetMinMaxTextureCoords()
        {
            List<Vector2[]> textureCoordSetsMinMax = new List<Vector2[]>();
            foreach (List<Vector2> textureCoordSet in TextureCoordSets)
            {
                textureCoordSetsMinMax.Add(GetMinMaxVector2(textureCoordSet));
            }
            return textureCoordSetsMinMax;
        }
        /// <summary>
        /// Computes the minimum and maximum values of a list of Vector2
        /// </summary>
        /// <param name="vecs"></param>
        /// <returns>Returns an array of two Vector2, minimum and maximum respectively.</returns>
        private Vector2[] GetMinMaxVector2(List<Vector2> vecs)
        {
            //get the max and min values
            Vector2 minVal = new Vector2
            {
                x = float.MaxValue,
                y = float.MaxValue
            };
            Vector2 maxVal = new Vector2
            {
                x = float.MinValue,
                y = float.MinValue
            };
            foreach (Vector2 vec in vecs)
            {
                maxVal.x = Math.Max(vec.x, maxVal.x);
                maxVal.y = Math.Max(vec.y, maxVal.y);

                minVal.x = Math.Min(vec.x, minVal.x);
                minVal.y = Math.Min(vec.y, minVal.y);
            }
            Vector2[] results = { minVal, maxVal };
            return results;

        }
        /// <summary>
        /// Computes the minimum and maximum values of a list of Vector3
        /// </summary>
        /// <param name="vecs"></param>
        /// <returns>Returns an array of two Vector3, minimum and maximum respectively.</returns>
        private Vector3[] GetMinMaxVector3(List<Vector3> vecs)
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
            foreach (Vector3 vec in vecs)
            {
                maxVal.x = Math.Max(vec.x, maxVal.x);
                maxVal.y = Math.Max(vec.y, maxVal.y);
                maxVal.z = Math.Max(vec.z, maxVal.z);

                minVal.x = Math.Min(vec.x, minVal.x);
                minVal.y = Math.Min(vec.y, minVal.y);
                minVal.z = Math.Min(vec.z, minVal.z);
            }
            Vector3[] results = { minVal, maxVal };
            return results;

        }
        /// <summary>
        /// Computes the minimum and maximum values of a list of Vector4
        /// </summary>
        /// <param name="vecs"></param>
        /// <returns>Returns an array of two Vector4, minimum and maximum respectively.</returns>
        private Vector4[] GetMinMaxVector4(List<Vector4> vecs)
        {
            //get the max and min values
            Vector4 minVal = new Vector4
            {
                x = float.MaxValue,
                y = float.MaxValue,
                z = float.MaxValue,
                w = float.MaxValue
            };
            Vector4 maxVal = new Vector4
            {
                x = float.MinValue,
                y = float.MinValue,
                z = float.MinValue,
                w = float.MinValue
            };
            foreach (Vector4 vec in vecs)
            {
                maxVal.x = Math.Max(vec.x, maxVal.x);
                maxVal.y = Math.Max(vec.y, maxVal.y);
                maxVal.z = Math.Max(vec.z, maxVal.z);
                maxVal.w = Math.Max(vec.w, maxVal.w);

                minVal.x = Math.Min(vec.x, minVal.x);
                minVal.y = Math.Min(vec.y, minVal.y);
                minVal.z = Math.Min(vec.z, minVal.z);
                minVal.w = Math.Min(vec.w, minVal.w);
            }
            Vector4[] results = { minVal, maxVal };
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
        private glTFLoader.Schema.BufferView CreateBufferView(int buffer_index, string name, int byteLength, int byteOffset)
        {
            glTFLoader.Schema.BufferView bufferView = new glTFLoader.Schema.BufferView
            {
                Name = name,
                ByteLength = byteLength,
                ByteOffset = byteOffset,
                Buffer = buffer_index
            };

            return bufferView;
        }
        /// <summary>
        /// Creates an Accessor object
        /// </summary>
        /// <param name="bufferview_index"></param>
        /// <param name="byteOffset"></param>
        /// <param name="componentType"></param>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="type"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private glTFLoader.Schema.Accessor CreateAccessor(int bufferview_index, int? byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, glTFLoader.Schema.Accessor.TypeEnum? type, bool? normalized)
        {
            glTFLoader.Schema.Accessor accessor = new glTFLoader.Schema.Accessor
            {
                BufferView = bufferview_index,
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
        public glTFLoader.Schema.MeshPrimitive ConvertToMeshPrimitive(List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, List<glTFLoader.Schema.Material> materials, Data geometryData, ref glTFLoader.Schema.Buffer buffer, int buffer_index, bool minMaxRangePositions, bool minMaxRangeNormals, bool minMaxRangeTextureCoords)
        {
            Dictionary<string, int> attributes = new Dictionary<string, int>();

            if (Positions != null)
            {
                //Create BufferView for the position
                int byteLength = sizeof(float) * 3 * Positions.Count();
                float[] min = new float[] { };
                float[] max = new float[] { };
                if (minMaxRangePositions)
                {
                    //get the max and min values
                    Vector3[] minMaxPositions = GetMinMaxPositions();
                    max = new[] { minMaxPositions[0].x, minMaxPositions[0].y, minMaxPositions[0].z };
                    min = new[] { minMaxPositions[1].x, minMaxPositions[1].y, minMaxPositions[1].z };
                }
                

                glTFLoader.Schema.BufferView bufferView = CreateBufferView(buffer_index, "Positions", byteLength, buffer.ByteLength);
                
                
                bufferViews.Add(bufferView);
                int bufferview_index = bufferViews.Count() - 1;

                // Create an accessor for the bufferView
                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferview_index, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, Positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                buffer.ByteLength += byteLength;
             

              //  bufferView.ByteLength += byteLength;
                accessors.Add(accessor);
                geometryData.Writer.Write(Positions.ToArray());
                attributes.Add("POSITION", accessors.Count() - 1);


            }
            if (Normals != null)
            {
                // Create BufferView
                int byteLength = sizeof(float) * 3 * Normals.Count();
                // Create a bufferView
                glTFLoader.Schema.BufferView bufferView = CreateBufferView(buffer_index, "Normals", byteLength, buffer.ByteLength);
                
                //get the max and min values
                float[] min = new float[] { };
                float[] max = new float[] { };
                if (minMaxRangeNormals)
                {
                    Vector3[] minMaxNormals = GetMinMaxNormals();

                    
                    max = new[] { minMaxNormals[0].x, minMaxNormals[0].y, minMaxNormals[0].z };
                    min = new[] { minMaxNormals[1].x, minMaxNormals[1].y, minMaxNormals[1].z };
                }
                

                bufferViews.Add(bufferView);
                int bufferview_index = bufferViews.Count() - 1;

                // Create an accessor for the bufferView
                glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferview_index, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, Normals.Count(), "Normals Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                
                buffer.ByteLength += byteLength;
                accessors.Add(accessor);
                geometryData.Writer.Write(Normals.ToArray());
                attributes.Add("NORMAL", accessors.Count() - 1);
            }

            if (TextureCoordSets != null)
            {
                //get the max and min values
                
                List<Vector2[]> minMaxTextureCoords = new List<Vector2[]>();
                if (minMaxRangeTextureCoords)
                {
                    minMaxTextureCoords = GetMinMaxTextureCoords();
                }

                for (int i = 0; i < TextureCoordSets.Count; ++i)
                {
                    List<Vector2> textureCoordSet = TextureCoordSets[i];

                    int byteLength = sizeof(float) * 2 * textureCoordSet.Count();

                    // Create a bufferView
                    glTFLoader.Schema.BufferView bufferView = CreateBufferView(buffer_index, "Texture Coords " + (i + 1), byteLength, buffer.ByteLength);
                    
                    float[] min = new float[] { };
                    float[] max = new float[] { };
                    if (minMaxRangeTextureCoords)
                    {
                        max = new[] { minMaxTextureCoords[i][1].x, minMaxTextureCoords[i][1].y };
                        min = new[] { minMaxTextureCoords[i][0].x, minMaxTextureCoords[i][0].y };
                    }
                    

                    
                    bufferViews.Add(bufferView);
                    int bufferview_index = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferview_index, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, textureCoordSet.Count(), "UV Accessor " + (i + 1), max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC2, null);
                    
                    buffer.ByteLength += byteLength;
                    accessors.Add(accessor);
                    geometryData.Writer.Write(textureCoordSet.ToArray());
                    attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);


                }
            }
            glTFLoader.Schema.MeshPrimitive mPrimitive = new glTFLoader.Schema.MeshPrimitive
            {
                Attributes = attributes,
            };
            if (Material != null)
            {
                glTFLoader.Schema.Material nMaterial = Material.CreateMaterial(samplers, images, textures);
                materials.Add(nMaterial);
                mPrimitive.Material = materials.Count() - 1;
            }
            

            return mPrimitive;

        }
        /// <summary>
        /// Converts the morph target list of dictionaries into Morph Target
        /// </summary>
        public List<Dictionary<string, int>> GetMorphTargets(List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, ref glTFLoader.Schema.Buffer buffer, Data geometryData, ref List<float> weights,  int buffer_index)
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
                            
                            glTFLoader.Schema.BufferView bufferView = CreateBufferView(buffer_index, "Positions", byteLength, buffer.ByteLength);

                            bufferViews.Add(bufferView);
                            int bufferview_index = bufferViews.Count() - 1;

                            // Create an accessor for the bufferView
                            glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferview_index, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Positions.Count(), "Positions Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                            buffer.ByteLength += byteLength;


                            //  bufferView.ByteLength += byteLength;
                            accessors.Add(accessor);
                            geometryData.Writer.Write(morphTarget.Positions.ToArray());
                            morphTargetAttributes.Add("POSITION", accessors.Count() - 1);
                        }
                    }
                    if (morphTarget.Normals != null && morphTarget.Normals.Count > 0)
                    {
                        if (morphTarget.Normals != null)
                        {
                            // Create BufferView
                            int byteLength = sizeof(float) * 3 * morphTarget.Normals.Count();
                            // Create a bufferView
                            glTFLoader.Schema.BufferView bufferView = CreateBufferView(buffer_index, "Normals", byteLength, buffer.ByteLength);
                            //get the max and min values
                            

                            bufferViews.Add(bufferView);
                            int bufferview_index = bufferViews.Count() - 1;

                            // Create an accessor for the bufferView
                            glTFLoader.Schema.Accessor accessor = CreateAccessor(bufferview_index, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                            buffer.ByteLength += byteLength;
                            accessors.Add(accessor);
                            geometryData.Writer.Write(morphTarget.Normals.ToArray());
                            morphTargetAttributes.Add("NORMAL", accessors.Count() - 1);
                        }
                    }
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Count > 0)
                    {
                        //not implemented...

                    }

                    morphTargetDicts.Add(new Dictionary<string, int> (morphTargetAttributes));
                    weights.Add(morphTargetWeight);
                }
            }

            return morphTargetDicts;

        }
    }
    
    
}
