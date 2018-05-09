using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator
{
    internal abstract class ModelGroup
    {
        public abstract ModelGroupName Name { get; }

        public List<Property> CommonProperties = new List<Property>();
        public List<Property> Properties = new List<Property>();
        public List<Model> Models;
        public List<Runtime.Image> UsedTextures = new List<Runtime.Image>();
        public List<Runtime.Image> UsedFigures = new List<Runtime.Image>();
        public int Id = -1;
        public bool NoSampleImages = false;

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

            public static List<Runtime.MeshPrimitive> CreateMultiPrimitivePlane(bool includeTextureCoords = true)
            {
                List<List<List<Vector2>>> textureCoordSets = new List<List<List<Vector2>>>()
                {
                    null,
                    null,
                };

                if (includeTextureCoords)
                {
                    textureCoordSets = GetMultiPrimitivePlaneTextureCoordSets();
                }

                return new List<Runtime.MeshPrimitive>
                {
                    new Runtime.MeshPrimitive
                    {
                        Positions = new List<Vector3>()
                        {
                            new Vector3(-0.5f,-0.5f, 0.0f),
                            new Vector3( 0.5f, 0.5f, 0.0f),
                            new Vector3(-0.5f, 0.5f, 0.0f)
                        },
                        TextureCoordSets = textureCoordSets[0],
                        Indices = new List<int>
                        {
                            0, 1, 2,
                        },
                    },

                    new Runtime.MeshPrimitive
                    {
                        Positions = new List<Vector3>()
                        {
                            new Vector3(-0.5f,-0.5f, 0.0f),
                            new Vector3( 0.5f,-0.5f, 0.0f),
                            new Vector3( 0.5f, 0.5f, 0.0f)
                        },
                        TextureCoordSets = textureCoordSets[1],
                        Indices = new List<int>
                        {
                            0, 1, 2,
                        },
                    }
                };
            }
        }

        public static List<List<List<Vector2>>> GetMultiPrimitivePlaneTextureCoordSets()
        {
            return new List<List<List<Vector2>>>
            {
                new List<List<Vector2>>
                {
                    new List<Vector2>
                    {
                        new Vector2( 0.0f, 1.0f),
                        new Vector2( 1.0f, 0.0f),
                        new Vector2( 0.0f, 0.0f)
                    },
                },
                new List<List<Vector2>>
                {
                    new List<Vector2>
                    {
                        new Vector2( 0.0f, 1.0f),
                        new Vector2( 1.0f, 1.0f),
                        new Vector2( 1.0f, 0.0f)
                    }
                }
            };
        }

        protected static class Gltf
        {
            public static Runtime.GLTF CreateMultiNode()
            {
                List<Vector3> vertexPositions = new List<Vector3>()
                {
                    new Vector3(2.500000f,2.500000f,2.500000f),
                    new Vector3(-2.500000f,2.500000f,2.500000f),
                    new Vector3(-2.500000f,-2.500000f,2.500000f),
                    new Vector3(2.500000f,-2.500000f,2.500000f),
                    new Vector3(0.000000f,2.500000f,0.000000f),
                    new Vector3(-2.500000f,2.500000f,2.500000f),
                    new Vector3(2.500000f,2.500000f,2.500000f),
                    new Vector3(-2.500000f,2.500000f,0.000000f),
                    new Vector3(0.000000f,7.500000f,0.000000f),
                    new Vector3(0.000000f,7.500000f,-2.500000f),
                    new Vector3(-2.500000f,7.500000f,0.000000f),
                    new Vector3(-2.500000f,7.500000f,-2.500000f),
                    new Vector3(2.500000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,0.000000f,-7.500000f),
                    new Vector3(-2.500000f,0.000000f,-7.500000f),
                    new Vector3(-2.500000f,2.500000f,-7.500000f),
                    new Vector3(0.000000f,2.500000f,-7.500000f),
                    new Vector3(0.000000f,2.500000f,-2.500000f),
                    new Vector3(2.500000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,0.000000f,-2.500000f),
                    new Vector3(-2.500000f,-2.500000f,-2.500000f),
                    new Vector3(2.500000f,-2.500000f,-2.500000f),
                    new Vector3(-2.500000f,0.000000f,-2.500000f),
                    new Vector3(2.500000f,-2.500000f,2.500000f),
                    new Vector3(-2.500000f,-2.500000f,2.500000f),
                    new Vector3(-2.500000f,-2.500000f,-2.500000f),
                    new Vector3(2.500000f,-2.500000f,-2.500000f),
                    new Vector3(2.500000f,2.500000f,2.500000f),
                    new Vector3(2.500000f,-2.500000f,2.500000f),
                    new Vector3(2.500000f,2.500000f,-2.500000f),
                    new Vector3(2.500000f,-2.500000f,-2.500000f),
                    new Vector3(-2.500000f,-2.500000f,-2.500000f),
                    new Vector3(-2.500000f,-2.500000f,2.500000f),
                    new Vector3(-2.500000f,0.000000f,0.000000f),
                    new Vector3(-2.500000f,0.000000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,2.500000f),
                    new Vector3(-7.500000f,2.500000f,0.000000f),
                    new Vector3(-7.500000f,0.000000f,-2.500000f),
                    new Vector3(-7.500000f,0.000000f,0.000000f),
                    new Vector3(-7.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,0.000000f),
                    new Vector3(-7.500000f,0.000000f,0.000000f),
                    new Vector3(-2.500000f,0.000000f,-2.500000f),
                    new Vector3(-2.500000f,0.000000f,0.000000f),
                    new Vector3(-7.500000f,0.000000f,-2.500000f),
                    new Vector3(-7.500000f,2.500000f,0.000000f),
                    new Vector3(-2.500000f,0.000000f,0.000000f),
                    new Vector3(-2.500000f,2.500000f,0.000000f),
                    new Vector3(-7.500000f,0.000000f,0.000000f),
                    new Vector3(-2.500000f,2.500000f,-2.500000f),
                    new Vector3(-7.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,0.000000f),
                    new Vector3(-7.500000f,2.500000f,0.000000f),
                    new Vector3(-7.500000f,0.000000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,0.000000f,-2.500000f),
                    new Vector3(-7.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,7.500000f,0.000000f),
                    new Vector3(-2.500000f,2.500000f,0.000000f),
                    new Vector3(0.000000f,7.500000f,0.000000f),
                    new Vector3(0.000000f,2.500000f,0.000000f),
                    new Vector3(0.000000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,7.500000f,-2.500000f),
                    new Vector3(0.000000f,2.500000f,0.000000f),
                    new Vector3(0.000000f,7.500000f,0.000000f),
                    new Vector3(-2.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,7.500000f,-2.500000f),
                    new Vector3(0.000000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,7.500000f,-2.500000f),
                    new Vector3(-2.500000f,7.500000f,0.000000f),
                    new Vector3(-2.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,0.000000f),
                    new Vector3(-2.500000f,7.500000f,-2.500000f),
                    new Vector3(0.000000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,2.500000f,-7.500000f),
                    new Vector3(-2.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,-7.500000f),
                    new Vector3(0.000000f,0.000000f,-2.500000f),
                    new Vector3(0.000000f,0.000000f,-7.500000f),
                    new Vector3(0.000000f,2.500000f,-2.500000f),
                    new Vector3(0.000000f,2.500000f,-7.500000f),
                    new Vector3(-2.500000f,0.000000f,-2.500000f),
                    new Vector3(-2.500000f,0.000000f,-7.500000f),
                    new Vector3(0.000000f,0.000000f,-2.500000f),
                    new Vector3(0.000000f,0.000000f,-7.500000f),
                    new Vector3(-2.500000f,2.500000f,-2.500000f),
                    new Vector3(-2.500000f,2.500000f,-7.500000f),
                    new Vector3(-2.500000f,0.000000f,-2.500000f),
                    new Vector3(-2.500000f,0.000000f,-7.500000f),
                    new Vector3(3.000000f,-1.000000f,3.000000f),
                    new Vector3(-7.500000f,-1.000000f,7.500000f),
                    new Vector3(3.000000f,-1.000000f,7.500000f),
                    new Vector3(-7.500000f,-1.000000f,3.000000f),
                    new Vector3(7.500000f,-1.000000f,7.500000f),
                    new Vector3(7.500000f,-1.000000f,3.000000f),
                    new Vector3(7.500000f,-1.000000f,-7.500000f),
                    new Vector3(3.000000f,-1.000000f,-7.500000f),
                };


                List<List<Vector2>> textureCoordSets = new List<List<Vector2>>
                {
                    new List<Vector2>
                    {
                        new Vector2(0.788554f,0.205935f),
                        new Vector2(0.584720f,0.205900f),
                        new Vector2(0.584685f,0.409734f),
                        new Vector2(0.788519f,0.409769f),
                        new Vector2(0.471918f,0.880496f),
                        new Vector2(0.369983f,0.982396f),
                        new Vector2(0.573817f,0.982430f),
                        new Vector2(0.370001f,0.880479f),
                        new Vector2(0.232172f,0.857846f),
                        new Vector2(0.232064f,0.959100f),
                        new Vector2(0.333426f,0.857955f),
                        new Vector2(0.333317f,0.959208f),
                        new Vector2(0.573852f,0.778596f),
                        new Vector2(0.471935f,0.778579f),
                        new Vector2(0.249236f,0.325688f),
                        new Vector2(0.249140f,0.426797f),
                        new Vector2(0.350249f,0.426892f),
                        new Vector2(0.350345f,0.325783f),
                        new Vector2(0.573870f,0.676679f),
                        new Vector2(0.573852f,0.778596f),
                        new Vector2(0.675786f,0.676697f),
                        new Vector2(0.777721f,0.574797f),
                        new Vector2(0.777686f,0.778631f),
                        new Vector2(0.675804f,0.574780f),
                        new Vector2(0.777652f,0.982465f),
                        new Vector2(0.981486f,0.982500f),
                        new Vector2(0.981520f,0.778666f),
                        new Vector2(0.777686f,0.778631f),
                        new Vector2(0.573817f,0.982430f),
                        new Vector2(0.777652f,0.982465f),
                        new Vector2(0.573852f,0.778596f),
                        new Vector2(0.777686f,0.778631f),
                        new Vector2(0.380851f,0.409699f),
                        new Vector2(0.584685f,0.409734f),
                        new Vector2(0.482785f,0.307799f),
                        new Vector2(0.380868f,0.307782f),
                        new Vector2(0.584720f,0.205900f),
                        new Vector2(0.225056f,0.327032f),
                        new Vector2(0.124248f,0.226196f),
                        new Vector2(0.124234f,0.327018f),
                        new Vector2(0.225070f,0.226211f),
                        new Vector2(0.482803f,0.205882f),
                        new Vector2(0.023427f,0.226182f),
                        new Vector2(0.124277f,0.024553f),
                        new Vector2(0.023455f,0.024539f),
                        new Vector2(0.124248f,0.226196f),
                        new Vector2(0.325892f,0.226224f),
                        new Vector2(0.426742f,0.024595f),
                        new Vector2(0.325920f,0.024581f),
                        new Vector2(0.426714f,0.226239f),
                        new Vector2(0.225098f,0.024567f),
                        new Vector2(0.225070f,0.226211f),
                        new Vector2(0.325920f,0.024581f),
                        new Vector2(0.325892f,0.226224f),
                        new Vector2(0.124248f,0.226196f),
                        new Vector2(0.225098f,0.024567f),
                        new Vector2(0.124277f,0.024553f),
                        new Vector2(0.225070f,0.226211f),
                        new Vector2(0.333426f,0.857955f),
                        new Vector2(0.333643f,0.655447f),
                        new Vector2(0.232172f,0.857846f),
                        new Vector2(0.232389f,0.655338f),
                        new Vector2(0.131136f,0.655230f),
                        new Vector2(0.130919f,0.857737f),
                        new Vector2(0.232389f,0.655338f),
                        new Vector2(0.232172f,0.857846f),
                        new Vector2(0.029882f,0.655121f),
                        new Vector2(0.029664f,0.857629f),
                        new Vector2(0.131136f,0.655230f),
                        new Vector2(0.130919f,0.857737f),
                        new Vector2(0.333426f,0.857955f),
                        new Vector2(0.434897f,0.655555f),
                        new Vector2(0.333643f,0.655447f),
                        new Vector2(0.434680f,0.858063f),
                        new Vector2(0.451167f,0.629205f),
                        new Vector2(0.451358f,0.426988f),
                        new Vector2(0.350058f,0.629110f),
                        new Vector2(0.350249f,0.426892f),
                        new Vector2(0.552276f,0.629301f),
                        new Vector2(0.552467f,0.427083f),
                        new Vector2(0.451167f,0.629205f),
                        new Vector2(0.451358f,0.426988f),
                        new Vector2(0.248950f,0.629014f),
                        new Vector2(0.249140f,0.426797f),
                        new Vector2(0.147841f,0.628919f),
                        new Vector2(0.148031f,0.426701f),
                        new Vector2(0.350058f,0.629110f),
                        new Vector2(0.350249f,0.426892f),
                        new Vector2(0.248950f,0.629014f),
                        new Vector2(0.249140f,0.426797f),
                        new Vector2(0.820246f,0.187538f),
                        new Vector2(0.979596f,0.559354f),
                        new Vector2(0.979596f,0.187538f),
                        new Vector2(0.820247f,0.559354f),
                        new Vector2(0.979596f,0.028188f),
                        new Vector2(0.820247f,0.028188f),
                        new Vector2(0.448431f,0.028188f),
                        new Vector2(0.448431f,0.187538f),
                    },
                };

                List<Vector3> vertexNormals = new List<Vector3>()
                {
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(0.000000f,0.000000f,1.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(0.000000f,0.000000f,-1.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(1.000000f,0.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(0.000000f,-1.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(-1.000000f,0.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                    new Vector3(0.000000f,1.000000f,0.000000f),
                };

                List<Vector4> vertexTangents = new List<Vector4>()
                {
                    new Vector4(1.000000f,0.000170f,0.000000f,1.000000f),
                    new Vector4(1.000000f,0.000171f,0.000000f,1.000000f),
                    new Vector4(1.000000f,0.000172f,0.000000f,1.000000f),
                    new Vector4(1.000000f,0.000171f,0.000000f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000170f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000170f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000170f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000171f,1.000000f),
                    new Vector4(-0.999999f,0.000000f,0.001072f,1.000000f),
                    new Vector4(-0.999999f,0.000000f,0.001072f,1.000000f),
                    new Vector4(-0.999999f,0.000000f,0.001071f,1.000000f),
                    new Vector4(-0.999999f,0.000000f,0.001072f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000171f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000171f,1.000000f),
                    new Vector4(0.000947f,1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000947f,1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000947f,1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000947f,1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.000172f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.000171f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.000171f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.000170f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.000169f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.000172f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000169f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000170f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000171f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000170f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000170f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000171f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000169f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000170f,1.000000f),
                    new Vector4(0.000000f,0.000169f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000172f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000170f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000168f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000172f,1.000000f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000140f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000141f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000140f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000140f,1.000000f),
                    new Vector4(0.000000f,0.000170f,1.000000f,1.000000f),
                    new Vector4(0.000140f,0.000000f,-1.000000f,1.000000f),
                    new Vector4(0.000142f,0.000000f,-1.000000f,1.000000f),
                    new Vector4(0.000141f,0.000000f,-1.000000f,1.000000f),
                    new Vector4(0.000141f,0.000000f,-1.000000f,1.000000f),
                    new Vector4(0.000140f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000140f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000140f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000140f,-1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000139f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000138f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000140f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000139f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000141f,1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000138f,1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000140f,1.000000f,0.000000f,1.000000f),
                    new Vector4(0.000140f,1.000000f,0.000000f,1.000000f),
                    new Vector4(-0.999999f,-0.001071f,0.000000f,1.000000f),
                    new Vector4(-0.999999f,-0.001071f,0.000000f,1.000000f),
                    new Vector4(-0.999999f,-0.001071f,0.000000f,1.000000f),
                    new Vector4(-0.999999f,-0.001071f,0.000000f,1.000000f),
                    new Vector4(0.000000f,-0.001073f,0.999999f,1.000000f),
                    new Vector4(0.000000f,-0.001075f,0.999999f,1.000000f),
                    new Vector4(0.000000f,-0.001071f,0.999999f,1.000000f),
                    new Vector4(0.000000f,-0.001073f,0.999999f,1.000000f),
                    new Vector4(0.999999f,-0.001073f,0.000000f,1.000000f),
                    new Vector4(0.999999f,-0.001072f,0.000000f,1.000000f),
                    new Vector4(0.999999f,-0.001074f,0.000000f,1.000000f),
                    new Vector4(0.999999f,-0.001073f,0.000000f,1.000000f),
                    new Vector4(0.000000f,-0.001071f,-0.999999f,1.000000f),
                    new Vector4(0.000000f,-0.001074f,-0.999999f,1.000000f),
                    new Vector4(0.000000f,-0.001073f,-0.999999f,1.000000f),
                    new Vector4(0.000000f,-0.001073f,-0.999999f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000942f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000943f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000941f,1.000000f),
                    new Vector4(1.000000f,0.000000f,-0.000942f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000944f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000943f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000946f,1.000000f),
                    new Vector4(0.000000f,-1.000000f,-0.000944f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000941f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000941f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000942f,1.000000f),
                    new Vector4(-1.000000f,0.000000f,-0.000941f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000946f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000947f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000945f,1.000000f),
                    new Vector4(0.000000f,1.000000f,-0.000946f,1.000000f),
                    new Vector4(0.000000f,0.000000f,1.000000f,1.000000f),
                    new Vector4(-0.000001f,0.000000f,1.000000f,1.000000f),
                    new Vector4(-0.000000f,0.000000f,1.000000f,1.000000f),
                    new Vector4(-0.000000f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000000f,1.000000f,1.000000f),
                    new Vector4(0.000000f,0.000000f,1.000000f,1.000000f),
                };

                List<int> indices0 = new List<int>
                {
                    90, 91, 92, 91, 90, 93, 94, 95, 92, 92, 95, 90, 96, 90, 95, 90, 96, 97,
                };
                List<int> indices1 = new List<int>
                {
                    0, 1, 2, 0, 2, 3, 4, 5, 6, 5, 4, 7, 8, 9, 10, 10, 9, 11, 6, 12, 4, 4, 12, 13, 14, 15, 16, 14, 16, 17, 18, 19,
                    20, 21, 20, 22, 20, 21, 23, 20, 19, 22, 24, 25, 26, 24, 26, 27, 28, 29, 30, 29, 31, 30, 32, 33, 34, 32, 34, 35,
                    33, 36, 34, 37, 38, 39, 38, 37, 40, 34, 36, 41, 42, 43, 44, 43, 42, 45, 46, 47, 48, 47, 46, 49, 50, 51, 52, 53,
                    52, 51, 54, 55, 56, 55, 54, 57, 58, 59, 60, 60, 59, 61, 62, 63, 64, 65, 64, 63, 66, 67, 68, 68, 67, 69, 70, 71,
                    72, 71, 70, 73, 74, 75, 76, 76, 75, 77, 78, 79, 80, 80, 79, 81, 82, 83, 84, 84, 83, 85, 86, 87, 88, 88, 87, 89,
                };


                Runtime.GLTF gltf = new Runtime.GLTF();
                Runtime.Scene scene = new Runtime.Scene();
                Runtime.Mesh mesh0 = new Runtime.Mesh();
                Runtime.Mesh mesh1 = new Runtime.Mesh();
                scene.Nodes = new List<Runtime.Node>();

                Runtime.MeshPrimitive meshPrim0 = new Runtime.MeshPrimitive
                {
                    Positions = vertexPositions,
                    TextureCoordSets = textureCoordSets,
                    Normals = vertexNormals,
                    Tangents = vertexTangents,
                    Indices = indices0,
                };
                mesh0.MeshPrimitives = new List<Runtime.MeshPrimitive> { meshPrim0 };
                scene.Nodes.Add(
                    new Runtime.Node
                    {
                        Mesh = mesh0,
                        Name = "Node0"
                    });

                Runtime.MeshPrimitive meshPrim1 = new Runtime.MeshPrimitive
                {
                    Positions = vertexPositions,
                    TextureCoordSets = textureCoordSets,
                    Normals = vertexNormals,
                    Tangents = vertexTangents,
                    Indices = indices1,
                };
                mesh1.MeshPrimitives = new List<Runtime.MeshPrimitive> { meshPrim1 };
                scene.Nodes[0].Children = new List<Runtime.Node>();
                scene.Nodes[0].Children.Add(
                    new Runtime.Node
                    {
                        Mesh = mesh1,
                        Name = "Node1"
                    });

                gltf.Scenes.Add(scene);

                return gltf;
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

        public virtual glTFLoader.Schema.Gltf PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            return gltf;
        }
    }

    internal struct Model
    {
        public List<Property> Properties { get; set; }
        public Runtime.GLTF GLTF;
    }

    public enum ModelGroupName
    {
        Undefined,
        Buffer_Interleaved,
        Compatibility,
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
