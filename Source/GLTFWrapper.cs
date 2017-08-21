using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using glTFLoader.Schema;

namespace AssetGenerator
{
    class GLTFWrapper
    {
        public List<GLTFScene> scenes;
        public int mainScene;
        public GLTFWrapper()
        {
            scenes = new List<GLTFScene>();
            mainScene = 0;
        }


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
                        nodes.Add(new Node
                        {
                            Mesh = mesh_index
                        });
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
        public class GLTFScene
        {
            public List<GLTFMesh> meshes;
            public GLTFScene()
            {
                meshes = new List<GLTFMesh>();
            }
            public void addMesh(GLTFMesh mesh) { meshes.Add(mesh); }
        }
        public class GLTFMesh
        {
            public List<GLTFMeshPrimitive> meshPrimitives;
            public GLTFMesh()
            {
                meshPrimitives = new List<GLTFMeshPrimitive>();
            }
            public void addPrimitive(GLTFMeshPrimitive meshPrimitive)
            {
                meshPrimitives.Add(meshPrimitive);
            }
        }
        public class GLTFMeshPrimitive
        {
            public GLTFMaterial material { get; set; }

            public List<Vector3> positions { get; set; }


            public List<Vector3> normals { get; set; }
            public List<List<Vector2>> textureCoordSets { get; set; }

            public Vector3 minNormals { get; private set; }
            public Vector3 maxNormals { get; private set; }

            public Vector2 minTextureCoords { get; private set; }
            public Vector2 maxTextureCoords { get; private set; }
            public Vector3[] getMinMaxNormals()
            {
                return getMinMaxVector3(normals);
            }
            public Vector3[] getMinMaxPositions()
            {
                return getMinMaxVector3(positions);
            }
            public List<Vector2[]> getMinMaxTextureCoords()
            {
                List<Vector2[]> textureCoordSetsMinMax = new List<Vector2[]>();
                foreach (List<Vector2> textureCoordSet in textureCoordSets)
                {
                    textureCoordSetsMinMax.Add(getMinMaxVector2(textureCoordSet));
                }
                return textureCoordSetsMinMax;
            }
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
            public Vector3[] getMinMaxVector3(List<Vector3> vecs)
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
            public Vector4[] getMinMaxVector4(List<Vector4> vecs)
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


        public class GLTFSampler
        {
            public int? magFilter;
            public int? minFilter;
            public int? wrapS;
            public int? wrapT;
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

        public class GLTFTexture
        {
            public GLTFImage source;
            public int texCoordIndex;
            public GLTFSampler sampler;

        }
        public class GLTFImage
        {
            public string uri;
            public Image convertToImage()
            {
                Image image = new Image
                {
                    Uri = uri
                };
                return image;
            }
        }
        public class GLTFMaterial
        {
            public GLTFMetallicRoughnessMaterial metallicRoughnessMaterial;
            public GLTFTexture normalTexture;
            public float? normalScale;
            public GLTFTexture occlusionTexture;
            public float? occlusionStrength;
            public GLTFTexture emissiveTexture;
            public Vector4? emissiveFactor;

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
        				emissiveFactor.Value.z,
        				emissiveFactor.Value.w
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
                return material;
            }

        }
        /// <summary>
        /// GLTF Wrapper class for defining MetallicRoughness
        /// </summary>
        public class GLTFMetallicRoughnessMaterial
        {
            public GLTFTexture baseColorTexture;
            public Vector4? baseColorFactor;
            public GLTFTexture metallicRoughnessTexture;
            public float? metallicFactor;
            public float? roughnessFactor;
        }
    }
}


