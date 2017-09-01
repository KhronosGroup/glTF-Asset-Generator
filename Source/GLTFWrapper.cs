using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using glTFLoader.Schema;

namespace AssetGenerator
{
    /// <summary>
    /// GLTFWrapper class for abstracting the glTF Loader API
    /// </summary>
    public class GLTFWrapper
    {
        /// <summary>
        /// List of scenes in the gltf wrapper
        /// </summary>
        public List<GLTFScene> scenes;
        /// <summary>
        /// index of the main scene
        /// </summary>
        public int? mainScene { get; set; }
        /// <summary>
        /// Initializes the gltf wrapper
        /// </summary>
        public GLTFWrapper()
        {
            scenes = new List<GLTFScene>();
        }

        /// <summary>
        /// converts the wrapper data into a gltf loader object. 
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="geometryData"></param>
        /// <returns>Returns a gltf object</returns>
        public Gltf buildGLTF(Gltf gltf, Data geometryData)
        {
            List<glTFLoader.Schema.Buffer> buffers = new List<glTFLoader.Schema.Buffer>();
            List<BufferView> bufferViews = new List<BufferView>();
            List<Accessor> accessors = new List<Accessor>();
            List<Material> materials = new List<Material>();
            List<Node> nodes = new List<Node>();
            List<Scene> gscenes = new List<Scene>();
            List<int> scene_indices = new List<int>();
            List<Image> images = new List<Image>();
            List<Sampler> samplers = new List<Sampler>();
            List<Texture> textures = new List<Texture>();
            List<Mesh> meshes = new List<Mesh>();
            GLTFBuffer gBuffer = new GLTFBuffer
            {
                uri = geometryData.Name,
                byteLength = 0,
                bufferIndex = 0
            };
            

            
            foreach (GLTFScene scene in scenes)
            {
                for (int mesh_index = 0; mesh_index < scene.meshes.Count(); ++mesh_index)
                {
                    GLTFMesh gMesh = scene.meshes[mesh_index];

                    Mesh m = gMesh.convertToMesh(bufferViews, accessors, samplers, images, textures, materials, geometryData, ref gBuffer);                    
                    meshes.Add(m);

                    Node node = new Node
                    {
                        Mesh = meshes.Count() - 1
                    };

                    if (gMesh.transformationMatrix != null)
                    {
                        node.Matrix = gMesh.transformationMatrix.ToArray();
                    }
                    if (gMesh.translation.HasValue)
                    {
                        node.Translation = gMesh.translation.Value.ToArray();
                    }
                    if (gMesh.rotation != null)
                    {
                        node.Rotation = gMesh.rotation.ToArray();
                    }
                    if (gMesh.scale.HasValue)
                    {
                        node.Scale = gMesh.scale.Value.ToArray();
                    }
                    nodes.Add(node);

                    scene_indices.Add(nodes.Count() - 1);
                }
                gltf.Scenes = new[]
                {
                    new Scene
                    {
                        Nodes = scene_indices.ToArray()
                    }
                };

                gltf.Scene = 0;

                if (meshes != null)
                {
                    gltf.Meshes = meshes.ToArray();
                }
                if (materials != null)
                {
                    gltf.Materials = materials.ToArray();
                }
                if (accessors != null)
                {
                    gltf.Accessors = accessors.ToArray();
                }
                if (bufferViews != null)
                {
                    gltf.BufferViews = bufferViews.ToArray();
                }
                
                gltf.Buffers = new[] { gBuffer.convertToBuffer() };
                if (nodes != null)
                {
                    gltf.Nodes = nodes.ToArray();
                }
                
                if (images.Count > 0)
                {
                    gltf.Images = images.ToArray();

                }
                if (textures.Count > 0)
                {
                    gltf.Textures = textures.ToArray();
                }
                if (samplers.Count > 0)
                {
                    gltf.Samplers = samplers.ToArray();
                }
            }
            if (mainScene.HasValue)
            {
                gltf.Scene = mainScene.Value;
            }
            
            return gltf;
        }
        /// <summary>
        /// Wrapper for glTF loader's Scene
        /// </summary>
        public class GLTFScene
        {
            /// <summary>
            /// List of meshes in the scene
            /// </summary>
            public List<GLTFMesh> meshes;

            /// <summary>
            /// The user-defined name of the scene
            /// </summary>
            public string name;
            public GLTFScene()
            {
                meshes = new List<GLTFMesh>();
            }
            /// <summary>
            /// Adds a GLTFMesh to the scene
            /// </summary>
            /// <param name="mesh"></param>
            public void addMesh(GLTFMesh mesh) { meshes.Add(mesh); }
        }

        /// <summary>
        /// Wrapper for glTF loader's Mesh
        /// </summary>
        public class GLTFMesh
        {
            /// <summary>
            /// The user-defined name of this mesh.
            /// </summary>
            public string name;
            
            /// <summary>
            /// List of mesh primitives in the mesh
            /// </summary>
            public List<GLTFMeshPrimitive> meshPrimitives;

            /// <summary>
            /// Transformation Matrix which performs translation, rotation and scale operations on the mesh
            /// </summary>
            public Matrix4x4 transformationMatrix { get; set; }
            /// <summary>
            /// Rotation Quaternion for the mesh
            /// </summary>
            public Quaternion rotation { get; set; }
            /// <summary>
            /// Translation Vector for the mesh.
            /// </summary>
            public Vector3? translation { get; set; }
            /// <summary>
            /// Scale Vector for the mesh.
            /// </summary>
            public Vector3? scale { get; set; }
            /// <summary>
            /// Initializes the Mesh
            /// </summary>
            public GLTFMesh()
            {
                meshPrimitives = new List<GLTFMeshPrimitive>();
            }
            /// <summary>
            /// Adds mesh primitive to mesh
            /// </summary>
            /// <param name="meshPrimitive"></param>
            public void addPrimitive(GLTFMeshPrimitive meshPrimitive)
            {
                meshPrimitives.Add(meshPrimitive);
            }
            public Mesh convertToMesh(List<BufferView> bufferViews, List<Accessor> accessors, List<Sampler> samplers, List<Image> images, List<Texture> textures, List<Material> materials, Data geometryData, ref GLTFBuffer gBuffer)
            {
                Mesh mesh = new Mesh();
                List<MeshPrimitive> primitives = new List<MeshPrimitive>(meshPrimitives.Count);

                foreach(GLTFMeshPrimitive gPrimitive in meshPrimitives)
                {
                    MeshPrimitive mPrimitive = gPrimitive.convertToMeshPrimitive(bufferViews, accessors, samplers, images, textures, materials, geometryData, ref gBuffer);
                    primitives.Add(mPrimitive);
                }
                if (name != null)
                {
                    mesh.Name = name;
                }
                if (meshPrimitives != null)
                {
                    mesh.Primitives = primitives.ToArray();
                }

                return mesh;
            }
        }
        /// <summary>
        /// Wrapper for glTF loader's Mesh Primitive
        /// </summary>
        public class GLTFMeshPrimitive
        {
            /// <summary>
            /// Material for the mesh primitive
            /// </summary>
            public GLTFMaterial material { get; set; }

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
            public MeshPrimitive.ModeEnum mode { get; set; }

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
            public GLTFAccessor createAccessor(GLTFBufferView gBufferView, int? byteOffset, Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, Accessor.TypeEnum? type, bool? normalized)
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

            public MeshPrimitive convertToMeshPrimitive(List<BufferView> bufferViews, List<Accessor> accessors, List<Sampler> samplers, List<Image> images, List<Texture> textures, List<Material> materials, Data geometryData, ref GLTFBuffer gBuffer)
            {
                Dictionary<string, int> attributes = new Dictionary<string, int>();

                if (positions != null)
                {
                    //Create BufferView
                    int byteLength = sizeof(float) * 3 * positions.Count();
                    //get the max and min values
                    Vector3[] minMaxPositions = getMinMaxPositions();
                    // Create a bufferView
                    GLTFBufferView gBufferView = createBufferView(ref gBuffer, "Positions", byteLength, gBuffer.byteLength);
                    float[] max = new[] { minMaxPositions[0].x, minMaxPositions[0].y, minMaxPositions[0].z };
                    float[] min = new[] { minMaxPositions[1].x, minMaxPositions[1].y, minMaxPositions[1].z };

                    glTFLoader.Schema.BufferView bufferView = gBufferView.convertToBufferView();
                    bufferViews.Add(bufferView);
                    gBufferView.index = bufferViews.Count() - 1;

                    // Create an accessor for the bufferView
                    GLTFAccessor gAccessor = createAccessor(gBufferView, 0, Accessor.ComponentTypeEnum.FLOAT, positions.Count(), "Positions Accessor", max, min, Accessor.TypeEnum.VEC3, null);
                    Accessor accessor = gAccessor.convertToAccessor();
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
                    GLTFAccessor gAccessor = createAccessor(gBufferView, 0, Accessor.ComponentTypeEnum.FLOAT, normals.Count(), "Normals Accessor", max, min, Accessor.TypeEnum.VEC3, null);
                    Accessor accessor = gAccessor.convertToAccessor();
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
                        float[] max = new[]  { minMaxTextureCoords[i][1].x, minMaxTextureCoords[i][1].y };
                        float[] min = new[] { minMaxTextureCoords[i][0].x, minMaxTextureCoords[i][0].y };

                        glTFLoader.Schema.BufferView bufferView = gBufferView.convertToBufferView();
                        bufferViews.Add(bufferView);
                        gBufferView.index = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        GLTFAccessor gAccessor = createAccessor(gBufferView, 0, Accessor.ComponentTypeEnum.FLOAT, textureCoordSet.Count(), "UV Accessor " + (i + 1), max, min, Accessor.TypeEnum.VEC2, null);
                        Accessor accessor = gAccessor.convertToAccessor();
                        gBuffer.byteLength += byteLength;
                        accessors.Add(accessor);
                        geometryData.Writer.Write(textureCoordSet.ToArray());
                        attributes.Add("TEXCOORD_" + i, accessors.Count() - 1);

                    }
                }
                MeshPrimitive mPrimitive = new MeshPrimitive
                {
                    Attributes = attributes,
                };
                if (material != null)
                {
                    Material nMaterial = material.createMaterial(samplers, images, textures);
                    materials.Add(nMaterial);
                    mPrimitive.Material = materials.Count() - 1;
                }

                return mPrimitive;

            }
        }

        /// <summary>
        /// Wrapper for glTF loader's Sampler.  The sampler descibe the wrapping and scaling of textures.
        /// </summary>
        public class GLTFSampler
        {
            /// <summary>
            /// Magnification filter
            /// </summary>
            public Sampler.MagFilterEnum? magFilter;
            /// <summary>
            /// Minification filter
            /// </summary>
            public Sampler.MinFilterEnum? minFilter;
            /// <summary>
            /// S wrapping mode
            /// </summary>
            public Sampler.WrapSEnum? wrapS;
            /// <summary>
            /// T wrapping mode
            /// </summary>
            public Sampler.WrapTEnum? wrapT;
            /// <summary>
            /// User-defined name of the sampler
            /// </summary>
            public string name;
            /// <summary>
            /// Converts the GLTFSampler into a glTF loader Sampler object.
            /// </summary>
            /// <returns>Returns a Sampler object</returns>
            public Sampler convertToSampler()
            {
                Sampler sampler = new Sampler();
                if (magFilter.HasValue)
                {
                    sampler.MagFilter = magFilter.Value;
                }
                if (minFilter.HasValue)
                {
                    sampler.MinFilter = minFilter.Value;
                }
                if (wrapS.HasValue)
                {
                    sampler.WrapS = wrapS.Value;
                }
                if (wrapT.HasValue)
                {
                    sampler.WrapT = wrapT.Value;
                }
                if (name != null)
                {
                    sampler.Name = name;
                }
                return sampler;
            }
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                GLTFSampler other = obj as GLTFSampler;
                if ((System.Object)other == null)
                    return false;

                return (magFilter == other.magFilter) && (minFilter == other.minFilter) && (wrapS == other.wrapS) && (wrapT == other.wrapT);
            }
        }

        /// <summary>
        /// Wrapper for glTF loader's Texture
        /// </summary>
        public class GLTFTexture
        {
            /// <summary>
            /// Image source for the texture
            /// </summary>
            public GLTFImage source;
            /// <summary>
            /// Texture coordinate index used for this texture
            /// </summary>
            public int? texCoordIndex;
            /// <summary>
            /// Sampler for this texture.
            /// </summary>
            public GLTFSampler sampler;

            /// <summary>
            /// User defined name
            /// </summary>
            public string name;

        }

        /// <summary>
        /// Wrapper for glTF loader's Image
        /// </summary>
        public class GLTFImage
        {
            /// <summary>
            /// The location of the image file, or a data uri containing texture data as an encoded string
            /// </summary>
            public string uri;

            /// <summary>
            /// The user-defined name of the image
            /// </summary>
            public string name;

            /// <summary>
            /// The image's mimetype
            /// </summary>
            public Image.MimeTypeEnum? mimeType;
            /// <summary>
            /// converts the GLTFImage to a glTF Image
            /// </summary>
            /// <returns>Returns an Image object</returns>
            public Image convertToImage()
            {
                Image image = new Image
                {
                    Uri = uri
                };
                if (mimeType.HasValue)
                {
                    image.MimeType = mimeType.Value;
                }
                if (name != null)
                {
                    image.Name = name;
                }
                return image;
            }
        }
        public class ObjectSearch<T>
        {
            public ObjectSearch(T obj)
            {
                this.obj = obj;
            }
            public T obj { get; set; }
            public bool Equals(T obj)
            {
                if (this.obj.Equals(obj))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Wrapper for glTF loader's Material
        /// </summary>
        public class GLTFMaterial
        {
            /// <summary>
            /// The user-defined name of this object
            /// </summary>
            public string name;
            /// <summary>
            /// A set of parameter values that are used to define the metallic-roughness material model from Physically-Based Rendering methodology
            /// </summary>
            public GLTFMetallicRoughnessMaterial metallicRoughnessMaterial;
            /// <summary>
            /// Texture that contains tangent-space normal information
            /// </summary>
            public GLTFTexture normalTexture;
            /// <summary>
            /// Scaling factor for the normal texture
            /// </summary>
            public float? normalScale;
            /// <summary>
            /// Texture that defines areas of the surface that are occluded from light, and thus rendered darker.  This information is contained in the "red" channel.
            /// </summary>
            public GLTFTexture occlusionTexture;
            /// <summary>
            /// Scaling factor for the occlusion texture
            /// </summary>
            public float? occlusionStrength;
            /// <summary>
            /// Texture that may be used to illuminate parts of the object surface. It defines the color of the light that is emitted from the surface
            /// </summary>
            public GLTFTexture emissiveTexture;
            /// <summary>
            /// Contains scaling factors for the "red", "green" and "blue" components of the emissive texture
            /// </summary>
            public Vector3? emissiveFactor;

            /// <summary>
            /// Specifies whether the material is double sided
            /// </summary>
            public bool? doubleSided;

            /// <summary>
            /// The alpha rendering mode of the material
            /// </summary>
            public Material.AlphaModeEnum? alphaMode;
            /// <summary>
            /// The alpha cutoff value of the material
            /// </summary>
            public float? alphaCutoff;
            /// <summary>
            /// Adds a texture to the property components of the GLTFWrapper.
            /// </summary>
            /// <param name="gTexture"></param>
            /// <param name="samplers"></param>
            /// <param name="images"></param>
            /// <param name="textures"></param>
            /// <param name="material"></param>
            /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
            public int?[] addTexture(GLTFTexture gTexture, List<Sampler> samplers, List<Image> images, List<Texture> textures, Material material)
            {
                List<int> indices = new List<int>();
                int? sampler_index = null;
                int? image_index = null;

                if (gTexture != null)
                {
                    if (gTexture.sampler != null)
                    {
                        // If a similar sampler is already being used in the list, return that index
                        if (samplers.Count > 0)
                        {
                            int find_index;
                            ObjectSearch<Sampler> samplerSearch = new ObjectSearch<Sampler>(gTexture.sampler.convertToSampler());
                            find_index = samplers.FindIndex(0, samplers.Count - 1, samplerSearch.Equals);
                            if (find_index != -1)
                                sampler_index = find_index;
                        }


                        // Add the sampler to the list of samples
                        if (!sampler_index.HasValue)
                        {
                            Sampler sampler = gTexture.sampler.convertToSampler();
                            samplers.Add(sampler);
                            sampler_index = samplers.Count() - 1;
                        }
                       
                    }
                    if (gTexture.source != null)
                    {
                        Image image = gTexture.source.convertToImage();
                        images.Add(image);
                        image_index = images.Count() - 1;
                    }

                    Texture texture = new Texture();
                    if (sampler_index.HasValue)
                    {
                        texture.Sampler = sampler_index.Value;
                    }
                    if (image_index.HasValue)
                    {
                        texture.Source = image_index.Value;
                    }
                    if (gTexture.name != null)
                    {
                        texture.Name = gTexture.name;
                    }
                    // If an identical texture has already been created, re-use that texture's index in the textures list
                    int find_texture_index = -1;
                    if (textures.Count > 0)
                    {
                        ObjectSearch<Texture> textureSearch = new ObjectSearch<Texture>(texture);
                        find_texture_index = textures.FindIndex(textureSearch.Equals);
                    }
                    if (find_texture_index > -1)
                    {
                        indices.Add(find_texture_index);
                    }
                    else
                    {
                        textures.Add(texture);
                        indices.Add(textures.Count() - 1);
                    }
                
                    if (gTexture.texCoordIndex.HasValue)
                    {
                        indices.Add(gTexture.texCoordIndex.Value);
                    }
                }
                int?[] result = { sampler_index, image_index };
                return result;
            }
            /// <summary>
            /// Creates a Material object and updates the property components of the GLTFWrapper.
            /// </summary>
            /// <param name="samplers"></param>
            /// <param name="images"></param>
            /// <param name="textures"></param>
            /// <returns>Returns a Material object, and updates the properties of the GLTFWrapper</returns>
            public Material createMaterial(List<Sampler> samplers, List<Image> images, List<Texture> textures)
            {
                Material material = new Material();
                material.PbrMetallicRoughness = new MaterialPbrMetallicRoughness();

                if (metallicRoughnessMaterial != null)
                {
                    if (metallicRoughnessMaterial.baseColorFactor != null)
                    {
                        material.PbrMetallicRoughness.BaseColorFactor = new[]
                        {
                            metallicRoughnessMaterial.baseColorFactor.Value.x,
                            metallicRoughnessMaterial.baseColorFactor.Value.y,
                            metallicRoughnessMaterial.baseColorFactor.Value.z,
                            metallicRoughnessMaterial.baseColorFactor.Value.w
                        };
                    }

                    if (metallicRoughnessMaterial.baseColorTexture != null)
                    {
                        int?[] baseColorIndices = addTexture(metallicRoughnessMaterial.baseColorTexture, samplers, images, textures, material);

                        material.PbrMetallicRoughness.BaseColorTexture = new TextureInfo();
                        if (baseColorIndices[0].HasValue)
                        {
                            material.PbrMetallicRoughness.BaseColorTexture.Index = baseColorIndices[0].Value;
                        }
                        if (baseColorIndices[1].HasValue)
                        {
                            material.PbrMetallicRoughness.BaseColorTexture.TexCoord = baseColorIndices[1].Value;
                        };

                    }
                    if (metallicRoughnessMaterial.metallicRoughnessTexture != null)
                    {
                        int?[] metallicRoughnessIndices = addTexture(metallicRoughnessMaterial.metallicRoughnessTexture, samplers, images, textures, material);

                        material.PbrMetallicRoughness.MetallicRoughnessTexture = new TextureInfo();
                        if (metallicRoughnessIndices[0].HasValue)
                        {
                            material.PbrMetallicRoughness.MetallicRoughnessTexture.Index = metallicRoughnessIndices[0].Value;
                        }
                        if (metallicRoughnessIndices[1].HasValue)
                        {
                            material.PbrMetallicRoughness.MetallicRoughnessTexture.TexCoord = metallicRoughnessIndices[1].Value;
                        }
                    }
                    if (metallicRoughnessMaterial.metallicFactor.HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicFactor = metallicRoughnessMaterial.metallicFactor.Value;
                    }
                    if (metallicRoughnessMaterial.roughnessFactor.HasValue)
                    {
                        material.PbrMetallicRoughness.RoughnessFactor = metallicRoughnessMaterial.roughnessFactor.Value;
                    }
                }
                if (emissiveFactor != null)
                {
                    material.EmissiveFactor = new[]
                    {
                        emissiveFactor.Value.x,
                        emissiveFactor.Value.y,
                        emissiveFactor.Value.z
                    };

                }
                if (normalTexture != null)
                {
                    int?[] normalIndicies = addTexture(normalTexture, samplers, images, textures, material);
                    material.NormalTexture = new MaterialNormalTextureInfo();

                    if (normalIndicies[0].HasValue)
                    {
                        material.NormalTexture.Index = normalIndicies[0].Value;

                    }
                    if (normalIndicies[1].HasValue)
                    {
                        material.NormalTexture.TexCoord = normalIndicies[1].Value;
                    }
                }
                if (occlusionTexture != null)
                {
                    int?[] occlusionIndicies = addTexture(occlusionTexture, samplers, images, textures, material);
                    material.OcclusionTexture = new MaterialOcclusionTextureInfo();
                    if (occlusionIndicies[0].HasValue)
                    {
                        material.OcclusionTexture.Index = occlusionIndicies[0].Value;

                    };
                    if (occlusionIndicies[1].HasValue)
                    {
                        material.OcclusionTexture.TexCoord = occlusionIndicies[1].Value;
                    }
                }
                if (emissiveTexture != null)
                {
                    int?[] emissiveIndicies = addTexture(emissiveTexture, samplers, images, textures, material);
                    material.EmissiveTexture = new TextureInfo();
                    if (emissiveIndicies[0].HasValue)
                    {
                        material.EmissiveTexture.Index = emissiveIndicies[0].Value;
                    }
                    if (emissiveIndicies[1].HasValue)
                    {
                        material.EmissiveTexture.TexCoord = emissiveIndicies[1].Value;
                    }
                }
                if (alphaMode.HasValue)
                {
                    material.AlphaMode = alphaMode.Value;
                }
                if (alphaCutoff.HasValue)
                {
                    material.AlphaCutoff = alphaCutoff.Value;
                }
                if (name != null)
                {
                    material.Name = name;
                }
                if (doubleSided.HasValue)
                {
                    material.DoubleSided = doubleSided.Value;
                }
                return material;
            }

        }
        /// <summary>
        /// GLTF Wrapper for glTF loader's MetallicRoughness
        /// </summary>
        public class GLTFMetallicRoughnessMaterial
        {
            /// <summary>
            /// The main texture that will be applied to the object.
            /// </summary>
            public GLTFTexture baseColorTexture;
            /// <summary>
            /// The scaling factors for the red, green, blue and alpha components of the color.
            /// </summary>
            public Vector4? baseColorFactor;
            /// <summary>
            /// Texture containing the metalness value in the "blue" color channel, and the roughness value in the "green" color channel.
            /// </summary>
            public GLTFTexture metallicRoughnessTexture;
            /// <summary>
            /// Scaling factor for the metalness component
            /// </summary>
            public float? metallicFactor;
            /// <summary>
            /// Scaling factor for the roughness component
            /// </summary>
            public float? roughnessFactor;
        }
        /// <summary>
        /// GLTF Wrapper for glTF loader's Buffer
        /// </summary>
        public class GLTFBuffer
        {
            public int? bufferIndex { get; set; }

            /// <summary>
            /// The length of the buffer in bytes
            /// </summary>
            public int byteLength { get; set; }
            /// <summary>
            /// The uri of the buffer.  Relative paths are relative to the .gltf file.  Instead of referencing an external file, the uri can also be a data-uri.
            /// </summary>
            public string uri { get; set; }


            /// <summary>
            /// Converts the GLTFBuffer to a Buffer type
            /// </summary>
            /// <returns></returns>
            public glTFLoader.Schema.Buffer convertToBuffer()
            {
                glTFLoader.Schema.Buffer buffer = new glTFLoader.Schema.Buffer();
                buffer.ByteLength = byteLength;
                
                if (uri != null)
                {
                    buffer.Uri = uri;
                }

                return buffer;
            }
        }

        public class GLTFBufferView
        {
            public int? index;
            public string name { get; set; }
            public GLTFBuffer buffer { get; set; }
            public int? byteOffset { get; set; }
            public int byteLength { get; set; }
            public int? byteStride { get; set; }
            public BufferView.TargetEnum? target { get; set; }

            public BufferView convertToBufferView()
            {
                BufferView bufferView = new BufferView
                {
                    ByteLength = byteLength
                };
                if (buffer.bufferIndex.HasValue)
                {
                    bufferView.Buffer = buffer.bufferIndex.Value;
                }
                if (byteOffset.HasValue)
                {
                    bufferView.ByteOffset = byteOffset.Value;
                }
                if (byteStride.HasValue)
                {
                    bufferView.ByteStride = byteStride.Value;
                }
                if (target.HasValue)
                {
                    bufferView.Target = target.Value;
                }
                if (name != null)
                {
                    bufferView.Name = name;
                }
                


                return bufferView;
            }

        }

        public class GLTFAccessor
        {
            /// <summary>
            /// The GLTFBufferView object
            /// </summary>
            public GLTFBufferView bufferView { get; set; }
            /// <summary>
            /// The offset relative to the start of the bufferView in bytes.
            /// </summary>
            public int? byteOffset { get; set; }
            public Accessor.ComponentTypeEnum? componentType { get; set; }
            /// <summary>
            /// Specifies whether integer data values should be normalized (true) or converted directly (false) when they are accessed.
            /// This property should be defined only for accessors that contain vertex attributes or animation output data.
            /// </summary>
            public bool? normalized { get; set; }
            /// <summary>
            /// The number of attributes referenced by this accessor
            /// </summary>
            public int? count { get; set; }
            /// <summary>
            /// Specifies if the attribute is a scalar, vector, or matrix
            /// </summary>
            public Accessor.TypeEnum? type { get; set; }
            /// <summary>
            /// Maximum value of each component in this attribute
            /// </summary>
            public float[] max { get; set; }
            /// <summary>
            /// Minimum value of each component in this attribute.
            /// </summary>
            public float[] min { get; set; }
            /// <summary>
            /// Sparse storage of attributes that deviate from their initialization value.
            /// </summary>
            public GLTFAccessorSparse sparse { get; set; }
            /// <summary>
            /// User-defined name for this accessor
            /// </summary>
            public string name { get; set; }

            public Accessor convertToAccessor()
            {
                Accessor accessor = new Accessor();
                if (count.HasValue)
                {
                    accessor.Count = count.Value;
                }
                if (type.HasValue)
                {
                    accessor.Type = type.Value;
                }
                if (max != null)
                {
                    accessor.Max = max;
                }
                if (min != null)
                {
                    accessor.Min = min;
                }
                if (name != null)
                {
                    accessor.Name = name;
                }
                if (normalized.HasValue)
                {
                    accessor.Normalized = normalized.Value;
                }
                if (componentType.HasValue)
                {
                    accessor.ComponentType = componentType.Value;
                }
                if (byteOffset.HasValue)
                {
                    accessor.ByteOffset = byteOffset.Value;
                }
                if (bufferView != null && bufferView.index.HasValue)
                {
                    accessor.BufferView = bufferView.index.Value;
                }


                return accessor;
            }

        }

        public class GLTFAccessorSparseIndices
        {
            /// <summary>
            /// The index of the bufferView with sparse indices.
            /// Referenced bufferView should not have ARRAY_BUFFER or ELEMENT_ARRAY_BUFER target.
            /// </summary>
            public GLTFBufferView bufferView { get; set; }
            /// <summary>
            /// The offset relative to the stary of the bufferView in bytes.  Must be aligned.
            /// </summary>
            public int byteOffset { get; set; }
            /// <summary>
            /// The indices data type.
            /// </summary>
            public Accessor.ComponentTypeEnum componentType { get; set; }
        }

        /// <summary>
        /// Sparse storage of attributes that deviate from their initializaation value.
        /// </summary>
        public class GLTFAccessorSparse
        {
            /// <summary>
            /// Number of entries stored in the sparse array.
            /// </summary>
            public int count { get; set; }
            /// <summary>
            /// Index array of size count that points to those accessor attributes that deviate from their initialization value.  Indices must strictly increase
            /// </summary>
            public GLTFAccessorSparseIndices indices { get; set; }
            /// <summary>
            /// Array of size count times number of components, storing the displaced accessor attributes pointed by indices.  
            /// Substituted values must have the same `componentType` and number of components as the base accessor
            /// </summary>
            public GLTFAccessorSparseValues values { get; set; }
        }
        /// <summary>
        /// Array of size `accessor.sparse.count` times number of components storing the displaced accessor attributes pointed by `accessor.sparse.indices`
        /// </summary>
        public class GLTFAccessorSparseValues
        {
            /// <summary>
            /// Reference to the bufferView with sparse values.
            /// Referenced bufferView should not have ARRAY_BUFFER or ELEMENT_ARRAY_BUFFER
            /// </summary>
            public GLTFBufferView bufferView { get; set; }
            /// <summary>
            /// The offset relative to the start of the bufferView in bytes.  Must be aligned.
            /// </summary>
            public int byteOffset { get; set; }
        }




    }
}


