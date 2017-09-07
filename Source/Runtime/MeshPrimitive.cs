using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using static AssetGenerator.GLTFWrapper;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Mesh Primitive
    /// </summary>
    public class MeshPrimitive
    {
        /// <summary>
        /// Material for the mesh primitive
        /// </summary>
        public Runtime.Material material { get; set; }

        /// <summary>
        /// List of Position/Vertices for the mesh primitive
        /// </summary>
        public List<Vector3> positions { get; set; }

        /// <summary>
        /// List of normals for the mesh primitive
        /// </summary>
        public List<Vector3> normals { get; set; }

        /// <summary>
        /// List of texture coordinate sets (as lists of Vector2) 
        /// </summary>
        public List<List<Vector2>> textureCoordSets { get; set; }

        /// <summary>
        /// Sets the type of primitive to render.
        /// </summary>
        public glTFLoader.Schema.MeshPrimitive.ModeEnum mode { get; set; }

        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as a list of Vector2 lists </returns>
        public Vector3[] getMinMaxNormals()
        {
            Vector3[] minMaxNormals = getMinMaxVector3(normals);

            return minMaxNormals;
        }
        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as an array of two vectors, minimum and maximum respectively</returns>
        public Vector3[] getMinMaxPositions()
        {
            Vector3[] minMaxPositions = getMinMaxVector3(positions);

            return minMaxPositions;
        }
        /// <summary>
        /// Computes and returns the minimum and maximum positions for each texture coordinate
        /// </summary>
        /// <returns>Returns the result as a list of two vectors, minimun and maximum respectively</returns>
        public List<Vector2[]> getMinMaxTextureCoords()
        {
            List<Vector2[]> textureCoordSetsMinMax = new List<Vector2[]>();
            foreach (List<Vector2> textureCoordSet in textureCoordSets)
            {
                textureCoordSetsMinMax.Add(getMinMaxVector2(textureCoordSet));
            }
            return textureCoordSetsMinMax;
        }
        /// <summary>
        /// Computes the minimum and maximum values of a list of Vector2
        /// </summary>
        /// <param name="vecs"></param>
        /// <returns>Returns an array of two Vector2, minimum and maximum respectively.</returns>
        private Vector2[] getMinMaxVector2(List<Vector2> vecs)
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
        private Vector3[] getMinMaxVector3(List<Vector3> vecs)
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
        private Vector4[] getMinMaxVector4(List<Vector4> vecs)
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
        /// Creates a wrapped GLTFBufferView object
        /// </summary>
        /// <param name="gBuffer"></param>
        /// <param name="name"></param>
        /// <param name="byteLength"></param>
        /// <param name="byteOffset"></param>
        /// <returns>GLTFBufferView</returns>
        public GLTFBufferView createBufferView(ref GLTFBuffer gBuffer, string name, int byteLength, int byteOffset)
        {
            GLTFBufferView gBufferView = new GLTFBufferView
            {
                name = name,
                buffer = gBuffer,
                byteLength = byteLength,
                byteOffset = byteOffset
            };

            return gBufferView;
        }
        /// <summary>
        /// Creates a wrapped GLTFAccessor object
        /// </summary>
        /// <param name="gBufferView"></param>
        /// <param name="byteOffset"></param>
        /// <param name="componentType"></param>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="type"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private GLTFAccessor createAccessor(GLTFBufferView gBufferView, int? byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, glTFLoader.Schema.Accessor.TypeEnum? type, bool? normalized)
        {
            GLTFAccessor gAccessor = new GLTFAccessor();
            if (gBufferView != null)
            {
                gAccessor.bufferView = gBufferView;
            }
            if (byteOffset.HasValue)
            {
                gAccessor.byteOffset = byteOffset.Value;
            }
            if (componentType.HasValue)
            {
                gAccessor.componentType = componentType.Value;
            }
            if (count.HasValue)
            {
                gAccessor.count = count.Value;
            }
            if (name != null)
            {
                gAccessor.name = name;
            }
            if (max.Length > 0)
            {
                gAccessor.max = max;

            }
            if (min.Length > 0)
            {
                gAccessor.min = min;
            }
            if (normalized.HasValue)
            {
                gAccessor.normalized = normalized.Value;
            }
            if (type.HasValue)
            {
                gAccessor.type = type.Value;
            }

            return gAccessor;
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
        public glTFLoader.Schema.MeshPrimitive convertToMeshPrimitive(List<glTFLoader.Schema.BufferView> bufferViews, List<glTFLoader.Schema.Accessor> accessors, List<glTFLoader.Schema.Sampler> samplers, List<glTFLoader.Schema.Image> images, List<glTFLoader.Schema.Texture> textures, List<glTFLoader.Schema.Material> materials, Data geometryData, ref GLTFBuffer gBuffer)
        {
            Dictionary<string, int> attributes = new Dictionary<string, int>();

            if (positions != null)
            {
                //Create BufferView
                int byteLength = sizeof(float) * 3 * positions.Count();
                //get the max and min values
                Vector3[] minMaxPositions = getMinMaxPositions();
                // Create a bufferView
                
                float[] max = new[] { minMaxPositions[0].x, minMaxPositions[0].y, minMaxPositions[0].z };
                float[] min = new[] { minMaxPositions[1].x, minMaxPositions[1].y, minMaxPositions[1].z };


                

                GLTFBufferView gBufferView = createBufferView(ref gBuffer, "Positions", byteLength, gBuffer.byteLength);
                glTFLoader.Schema.BufferView bufferView = gBufferView.convertToBufferView();
                bufferViews.Add(bufferView);
                gBufferView.index = bufferViews.Count() - 1;

                // Create an accessor for the bufferView
                GLTFAccessor gAccessor = createAccessor(gBufferView, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, positions.Count(), "Positions Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                glTFLoader.Schema.Accessor accessor = gAccessor.convertToAccessor();

                gBuffer.byteLength += byteLength;
                accessors.Add(accessor);
                geometryData.Writer.Write(positions.ToArray());
                attributes.Add("POSITION", accessors.Count() - 1);


            }
            if (normals != null)
            {
                // Create BufferView
                int byteLength = sizeof(float) * 3 * normals.Count();
                //get the max and min values
                Vector3[] minMaxNormals = getMinMaxNormals();

                // Create a bufferView
                GLTFBufferView gBufferView = createBufferView(ref gBuffer, "Normals", byteLength, gBuffer.byteLength);
                float[] max = new[] { minMaxNormals[0].x, minMaxNormals[0].y, minMaxNormals[0].z };
                float[] min = new[] { minMaxNormals[1].x, minMaxNormals[1].y, minMaxNormals[1].z };

                glTFLoader.Schema.BufferView bufferView = gBufferView.convertToBufferView();
                bufferViews.Add(bufferView);
                gBufferView.index = bufferViews.Count() - 1;

                // Create an accessor for the bufferView
                GLTFAccessor gAccessor = createAccessor(gBufferView, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, normals.Count(), "Normals Accessor", max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                glTFLoader.Schema.Accessor accessor = gAccessor.convertToAccessor();
                gBuffer.byteLength += byteLength;
                accessors.Add(accessor);
                geometryData.Writer.Write(normals.ToArray());
                attributes.Add("NORMAL", accessors.Count() - 1);

            }

            if (textureCoordSets != null)
            {
                //get the max and min values
                List<Vector2[]> minMaxTextureCoords = getMinMaxTextureCoords();

                for (int i = 0; i < textureCoordSets.Count; ++i)
                {
                    List<Vector2> textureCoordSet = textureCoordSets[i];

                    int byteLength = sizeof(float) * 2 * textureCoordSet.Count();

                    // Create a bufferView
                    GLTFBufferView gBufferView = createBufferView(ref gBuffer, "Texture Coords " + (i + 1), byteLength, gBuffer.byteLength);
                    float[] max = new[] { minMaxTextureCoords[i][1].x, minMaxTextureCoords[i][1].y };
                    float[] min = new[] { minMaxTextureCoords[i][0].x, minMaxTextureCoords[i][0].y };

                    glTFLoader.Schema.BufferView bufferView = gBufferView.convertToBufferView();
                    bufferViews.Add(bufferView);
                    gBufferView.index = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    GLTFAccessor gAccessor = createAccessor(gBufferView, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, textureCoordSet.Count(), "UV Accessor " + (i + 1), max, min, glTFLoader.Schema.Accessor.TypeEnum.VEC2, null);
                    glTFLoader.Schema.Accessor accessor = gAccessor.convertToAccessor();
                    gBuffer.byteLength += byteLength;
                    accessors.Add(accessor);
                    geometryData.Writer.Write(textureCoordSet.ToArray());
                    attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);

                }
            }
            glTFLoader.Schema.MeshPrimitive mPrimitive = new glTFLoader.Schema.MeshPrimitive
            {
                Attributes = attributes,
            };
            if (material != null)
            {
                glTFLoader.Schema.Material nMaterial = material.createMaterial(samplers, images, textures);
                materials.Add(nMaterial);
                mPrimitive.Material = materials.Count() - 1;
            }

            return mPrimitive;

        }
    }
    
    
}
