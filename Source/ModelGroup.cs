using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        public abstract ModelGroupName Name { get; }
        public List<Model> Models { get; protected set; }
        public List<Property> CommonProperties { get; private set; }
        public List<Property> Properties { get; private set; }
        public List<Runtime.Image> UsedTextures { get; private set; }
        public List<Runtime.Image> UsedFigures { get; private set; }
        public int Id { get; set; }
        public bool NoSampleImages { get; protected set; }

        protected ModelGroup()
        {
            CommonProperties = new List<Property>();
            Properties = new List<Property>();
            UsedTextures = new List<Runtime.Image>();
            UsedFigures = new List<Runtime.Image>();
            NoSampleImages = false;
        }

        protected Runtime.Image UseTexture(List<string> imageList, string name)
        {
            Runtime.Image image = GetImage(imageList, name);
            UsedTextures.Add(image);

            return image;
        }

        protected void UseFigure(List<string> imageList, string name)
        {
            UsedFigures.Add(GetImage(imageList, name));
        }

        private Runtime.Image GetImage(List<string> imageList, string name)
        {
            var image = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains(name))
            };

            return image;
        }

        protected static Runtime.GLTF CreateGLTF(Func<Runtime.Scene> createScene, List<string> extensionsUsed = null)
        {
            return new Runtime.GLTF
            {
                Asset = new Runtime.Asset
                {
                    Generator = "glTF Asset Generator",
                    Version = "2.0",
                },
                Scenes = new List<Runtime.Scene>
                {
                    createScene(),
                },
                ExtensionsUsed = extensionsUsed,
            };
        }

        protected static class MeshPrimitive
        {
            public static Runtime.MeshPrimitive CreateSinglePlane(bool includeTextureCoords = true, bool includeIndices = true)
            {
                List<List<Vector2>> textureCoords = null;
                List<int> indices = null;

                if (includeTextureCoords)
                {
                    textureCoords = GetSinglePlaneTextureCoordSets();
                }

                if (includeIndices)
                {
                    indices = GetSinglePlaneIndices();
                }

                return new Runtime.MeshPrimitive
                {
                    Positions = GetSinglePlanePositions(),
                    TextureCoordSets = textureCoords,
                    Indices = indices,
                };
            }

            public static List<Vector3> GetSinglePlanePositions()
            {
                return new List<Vector3>()
                {
                    new Vector3( 0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f,  0.5f, 0.0f),
                    new Vector3( 0.5f,  0.5f, 0.0f)
                };
            }

            public static List<List<Vector2>> GetSinglePlaneTextureCoordSets()
            {
                return new List<List<Vector2>>
                {
                    new List<Vector2>
                    {
                        new Vector2( 1.0f, 1.0f),
                        new Vector2( 0.0f, 1.0f),
                        new Vector2( 0.0f, 0.0f),
                        new Vector2( 1.0f, 0.0f)
                    },
                };
            }

            public static List<int> GetSinglePlaneIndices()
            {
                return new List<int>
                {
                    1, 0, 3, 1, 3, 2
                };
            }

            /// <summary>
            /// Creates two mesh primitives that, when taken together, are intended to look like a square plane.
            /// </summary>
            public static List<Runtime.MeshPrimitive> CreateMultiPrimitivePlane(bool includeTextureCoords = true)
            {
                return new List<Runtime.MeshPrimitive>
                {
                    CreateLeftPrimitiveTriangle(includeTextureCoords),
                    CreateRightPrimitiveTriangle(includeTextureCoords),
                };
            }

            /// <summary>
            /// Triangle making up the top right corner of the multi primitive plane.
            /// </summary>
            public static Runtime.MeshPrimitive CreateLeftPrimitiveTriangle(bool includeTextureCoords = true)
            {
                return new Runtime.MeshPrimitive
                {
                    Positions = new List<Vector3>()
                    {
                        new Vector3(-0.5f,-0.5f, 0.0f),
                        new Vector3( 0.5f, 0.5f, 0.0f),
                        new Vector3(-0.5f, 0.5f, 0.0f)
                    },
                    TextureCoordSets = includeTextureCoords ? new List<List<Vector2>>
                    {
                        GetLeftPrimitiveTriangleTextureCoordSets(),
                    } : null,
                    Indices = new List<int>
                    {
                        0, 1, 2,
                    },
                };
            }

            /// <summary>
            /// Texture coordinates for the left triangle in the multi primitive plane
            /// </summary>
            public static List<Vector2> GetLeftPrimitiveTriangleTextureCoordSets()
            {
                return new List<Vector2>
                {
                    new Vector2( 0.0f, 1.0f),
                    new Vector2( 1.0f, 0.0f),
                    new Vector2( 0.0f, 0.0f)
                };
            }

            /// <summary>
            /// Triangle making up the bottom left corner of the multi primitive plane.
            /// </summary>
            public static Runtime.MeshPrimitive CreateRightPrimitiveTriangle(bool includeTextureCoords = true)
            {
                return new Runtime.MeshPrimitive
                {
                    Positions = new List<Vector3>()
                    {
                        new Vector3(-0.5f,-0.5f, 0.0f),
                        new Vector3( 0.5f,-0.5f, 0.0f),
                        new Vector3( 0.5f, 0.5f, 0.0f)
                    },
                    TextureCoordSets = includeTextureCoords ? new List<List<Vector2>>
                    {
                        GetRightPrimitiveTriangleTextureCoordSets(),
                    } : null,
                    Indices = new List<int>
                    {
                        0, 1, 2,
                    },
                };
            }

            /// <summary>
            /// Texture coordinates for the right triangle in the multi primitive plane
            /// </summary>
            public static List<Vector2> GetRightPrimitiveTriangleTextureCoordSets()
            {
                return new List<Vector2>
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f)
            };
            }
        }

        protected void GenerateUsedPropertiesList()
        {
            // Creates a list with each unique property used by the model group.
            foreach (var model in Models)
            {
                Properties = Properties.Union(model.Properties).ToList();
            }

            // Sort both properties lists
            SortPropertiesList(CommonProperties);
            SortPropertiesList(Properties);
        }

        protected void SortPropertiesList(List<Property> properties)
        {
            // Sorts the list so every readme has the same column order, determined by enum value.
            if (properties.Count > 0)
            {
                properties.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
        }
    }

    internal class Model
    {
        public List<Property> Properties;
        public Runtime.GLTF GLTF;
        public Action<glTFLoader.Schema.Gltf> PostRuntimeChanges = gltf => {};
        public Func<Type, object> CreateSchemaInstance = Activator.CreateInstance;
    }

    internal enum ModelGroupName
    {
        Undefined,
        Buffer_Interleaved,
        Compatibility,
        Instancing,
        Material,
        Material_AlphaMask,
        Material_AlphaBlend,
        Material_DoubleSided,
        Material_MetallicRoughness,
        Material_SpecularGlossiness,
        Material_Mixed,
        Mesh_PrimitiveAttribute,
        Mesh_PrimitiveVertexColor,
        Mesh_PrimitiveMode,
        Mesh_Primitives,
        Mesh_PrimitivesUV,
        Node_NegativeScale,
        Node_Attribute,
        Texture_Sampler,
        Primitive_VertexColor,
    }
}
