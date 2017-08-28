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
        public int mainScene;
        /// <summary>
        /// Initializes the gltf wrapper
        /// </summary>
        public GLTFWrapper()
        {
            scenes = new List<GLTFScene>();
            mainScene = 0;
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
            List<MeshPrimitive> meshPrimitives = new List<MeshPrimitive>();
            List<Material> materials = new List<Material>();
            List<Node> nodes = new List<Node>();
            List<Scene> gscenes = new List<Scene>();
            List<int> scene_indices = new List<int>();
            List<Image> images = new List<Image>();
            List<Sampler> samplers = new List<Sampler>();
            List<Texture> textures = new List<Texture>();

            foreach (GLTFScene scene in scenes)
            {
                for (int mesh_index = 0; mesh_index < scene.meshes.Count(); ++mesh_index)
                {
                    GLTFMesh mesh = scene.meshes[mesh_index];
                    int byteOffset = 0;
                    foreach (GLTFMeshPrimitive meshPrimitive in mesh.meshPrimitives)
                    {
                        Dictionary<string, int> attributes = new Dictionary<string, int>();

                        if (meshPrimitive.positions != null)
                        {
                            int bytelength = sizeof(float) * 3 * meshPrimitive.positions.Count();
                            BufferView bufferView = new BufferView
                            {
                                Name = "Positions",
                                Buffer = mesh_index,
                                ByteLength = bytelength
                            };
                            if (byteOffset > 0)
                            {
                                bufferView.ByteOffset = byteOffset;

                            }
                            bufferViews.Add(bufferView);
                            byteOffset += bytelength;

                            //get the max and min values
                            Vector3[] minMaxPositions = meshPrimitive.getMinMaxPositions();

                            // Create Accessor
                            Accessor accessor = new Accessor
                            {
                                Name = "Positions Accessor",
                                BufferView = bufferViews.Count() - 1,
                                ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                                Count = meshPrimitive.positions.Count(),
                                Type = Accessor.TypeEnum.VEC3,
                                Max = new[] { minMaxPositions[1].x, minMaxPositions[1].y, minMaxPositions[1].z },
                                Min = new[] { minMaxPositions[0].x, minMaxPositions[0].y, minMaxPositions[0].z }
                            };
                            accessors.Add(accessor);
                            geometryData.Writer.Write(meshPrimitive.positions.ToArray());
                            attributes.Add("POSITION", accessors.Count() - 1);
                        }
                        if (meshPrimitive.normals != null)
                        {
                            // Create BufferView
                            int bytelength = sizeof(float) * 3 * meshPrimitive.normals.Count();
                            BufferView bufferView = new BufferView
                            {
                                Name = "Normals",
                                Buffer = mesh_index,
                                ByteLength = bytelength
                            };
                            if (byteOffset > 0)
                            {
                                bufferView.ByteOffset = byteOffset;

                            }
                            bufferViews.Add(bufferView);
                            byteOffset += bytelength;

                            //get the max and min values
                            Vector3[] minMaxNormals = meshPrimitive.getMinMaxNormals();

                            // Create Accessor
                            Accessor accessor = new Accessor
                            {
                                Name = "Normals Accessor",
                                BufferView = bufferViews.Count() - 1,
                                ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                                Count = meshPrimitive.normals.Count(),
                                Type = Accessor.TypeEnum.VEC3,
                                Max = new[] { minMaxNormals[1].x, minMaxNormals[1].y, minMaxNormals[1].z },
                                Min = new[] { minMaxNormals[0].x, minMaxNormals[0].y, minMaxNormals[0].z }
                            };
                            accessors.Add(accessor);
                            attributes.Add("NORMAL", accessors.Count() - 1);
                            geometryData.Writer.Write(meshPrimitive.normals.ToArray());
                        }

                        if (meshPrimitive.textureCoordSets != null)
                        {
                            //get the max and min values
                            List<Vector2[]> minMaxTextureCoords = meshPrimitive.getMinMaxTextureCoords();

                            for (int i = 0; i < meshPrimitive.textureCoordSets.Count; ++i)
                            {
                                List<Vector2> textureCoordSet = meshPrimitive.textureCoordSets[i];

                                int bytelength = sizeof(float) * 2 * textureCoordSet.Count();
                                BufferView bufferView = new BufferView
                                {
                                    Name = "texture coords " + (i + 1),
                                    Buffer = mesh_index,
                                    ByteLength = bytelength
                                };
                                if (byteOffset > 0)
                                {
                                    bufferView.ByteOffset = byteOffset;

                                }
                                bufferViews.Add(bufferView);
                                byteOffset += bytelength;

                                // Create Accessor
                                Accessor accessor = new Accessor
                                {
                                    Name = "UV Accessor" + (i + 1),
                                    BufferView = bufferViews.Count() - 1,
                                    ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                                    Count = textureCoordSet.Count(),
                                    Type = Accessor.TypeEnum.VEC2,
                                    Max = new[] { minMaxTextureCoords[i][1].x, minMaxTextureCoords[i][1].y },
                                    Min = new[] { minMaxTextureCoords[i][0].x, minMaxTextureCoords[i][0].y }
                                };
                                accessors.Add(accessor);
                                attributes.Add("TEXCOORD_" + (i), accessors.Count() - 1);
                                geometryData.Writer.Write(textureCoordSet.ToArray());
                            }
                        }
                        MeshPrimitive mPrimitive = new MeshPrimitive
                        {
                            Attributes = attributes,       
                        };
                        if (meshPrimitive.material != null)
                        {
                            Material material = meshPrimitive.material.createMaterial(samplers, images, textures);
                            materials.Add(material);
                            mPrimitive.Material = materials.Count() - 1;
                        }
                        meshPrimitives.Add(mPrimitive);
                        glTFLoader.Schema.Buffer buffer = new glTFLoader.Schema.Buffer
                        {
                            Uri = geometryData.Name,
                            ByteLength = byteOffset
                        };
                        buffers.Add(buffer);
                        gltf.Materials = materials.ToArray();
                        Node node = new Node
                        {
                            Mesh = mesh_index
                        };
                        
                        if (mesh.transformationMatrix != null)
                        {
                            node.Matrix = mesh.transformationMatrix.ToArray();
                        }
                        if (mesh.translation.HasValue)
                        {
                            node.Translation = mesh.translation.Value.ToArray();
                        }
                        if (mesh.rotation != null)
                        {
                            node.Rotation = mesh.rotation.ToArray();
                        }
                        if (mesh.scale.HasValue)
                        {
                            node.Scale = mesh.scale.Value.ToArray();
                        }
                        nodes.Add(node);

                        scene_indices.Add(nodes.Count() - 1);
                    }
                }
                gltf.Scenes = new[]
                {
                    new Scene
                    {
                        Nodes = scene_indices.ToArray()
                    }
                };
                gltf.Scene = 0;
                gltf.Meshes = new[]
                {
                    new Mesh
                    {
                        Primitives = meshPrimitives.ToArray()
                    }
                };
                gltf.Accessors = accessors.ToArray();
                gltf.BufferViews = bufferViews.ToArray();
                gltf.Buffers = buffers.ToArray();
                gltf.Nodes = nodes.ToArray();
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
            public List<Vector3> normals { get; set;}

            /// <summary>
            /// List of texture coordinate sets (as lists of Vector2) 
            /// </summary>
            public List<List<Vector2>> textureCoordSets { get; set;}

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
        }

        /// <summary>
        /// Wrapper for glTF loader's Sampler.  The sampler descibe the wrapping and scaling of textures.
        /// </summary>
        public class GLTFSampler
        {
            public int? magFilter;
            public int? minFilter;
            public int? wrapS;
            public int? wrapT;
            /// <summary>
            /// Converts the GLTFSampler into a glTF loader Sampler object.
            /// </summary>
            /// <returns>Returns a Sampler object</returns>
            public Sampler convertToSampler()
            {
                Sampler sampler = new Sampler();
                if (magFilter.HasValue)
                {
                    sampler.MagFilter = sampler.MagFilter;
                }
                if (minFilter.HasValue)
                {
                    sampler.MinFilter = sampler.MinFilter;
                }
                if (wrapS.HasValue)
                {
                    sampler.WrapS = sampler.WrapS;
                }
                if (wrapT.HasValue)
                {
                    sampler.WrapT = sampler.WrapT;
                }
                return sampler;
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
            public int texCoordIndex;
            /// <summary>
            /// Sampler for this texture.
            /// </summary>
            public GLTFSampler sampler;

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
            /// converts the GLTFImage to a glTF Image
            /// </summary>
            /// <returns>Returns an Image object</returns>
            public Image convertToImage()
            {
                Image image = new Image
                {
                    Uri = uri
                };
                return image;
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
            /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers (</returns>
            public int[] addTexture(GLTFTexture gTexture, List<Sampler> samplers, List<Image> images, List<Texture> textures, Material material)
            {
                List<int> indices = new List<int>();

                if (gTexture != null)
                {
                    Sampler sampler = gTexture.sampler.convertToSampler();
                    samplers.Add(sampler);
                    int sampler_index = samplers.Count() - 1;

                    Image image = gTexture.source.convertToImage();
                    images.Add(image);
                    int image_index = images.Count() - 1;

                    Texture texture = new Texture
                    {
                        Sampler = sampler_index,
                        Source = image_index
                    };
                    textures.Add(texture);
                    indices.Add(textures.Count() - 1);
                    indices.Add(gTexture.texCoordIndex);
                }
                return indices.ToArray();
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
						int[] baseColorIndices = addTexture(metallicRoughnessMaterial.baseColorTexture, samplers, images, textures, material);
						material.PbrMetallicRoughness.BaseColorTexture = new TextureInfo
						{
							Index = baseColorIndices[0],
							TexCoord = baseColorIndices[1]
						};
                        
                    }
                    if (metallicRoughnessMaterial.metallicRoughnessTexture != null)
                    {
						int[] metallicRoughnessIndices = addTexture(metallicRoughnessMaterial.metallicRoughnessTexture, samplers, images, textures, material);
						material.PbrMetallicRoughness.MetallicRoughnessTexture = new TextureInfo
						{
							Index = metallicRoughnessIndices[0],
							TexCoord = metallicRoughnessIndices[1]
						};
                        
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
					int[] normalIndicies = addTexture(normalTexture, samplers, images, textures, material);
					material.NormalTexture = new MaterialNormalTextureInfo
					{
						Index = normalIndicies[0],
						TexCoord = normalIndicies[1]
					};
                    
                }
                if (occlusionTexture != null)
                {
					int[] occlusionIndicies = addTexture(occlusionTexture, samplers, images, textures, material);
					material.OcclusionTexture = new MaterialOcclusionTextureInfo
					{
						Index = occlusionIndicies[0],
						TexCoord = occlusionIndicies[1]
					};
                    
                }
                if (emissiveTexture != null)
                {
					int[] emissiveIndicies = addTexture(emissiveTexture, samplers, images, textures, material);
					material.EmissiveTexture = new TextureInfo
					{
						Index = emissiveIndicies[0],
						TexCoord = emissiveIndicies[1]
					};   
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
    }
}


