using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Mesh_PrimitiveMode : ModelGroup
    {
        public Mesh_PrimitiveMode(List<string> textures, List<string> figures) : base(textures, figures)
        {
            modelGroupName = ModelGroupName.Mesh_PrimitiveMode;
            onlyBinaryProperties = false;
            noPrerequisite = false;

            Runtime.Image figureIndices = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("Indices"))
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("Normal_Plane"))
            };
            usedTextures.Add(normalTexture);
            usedFigures.Add(figureIndices);

            List<Vector3> noIndicesPositionsTriangles = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
            };
            List<Vector3> noIndicesPositionsLines = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
            };
            List<Vector3> noIndicesPositionsLineloopFan = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
            };
            List<Vector3> noIndicesPositionsPoints = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.25f,-0.5f, 0.0f),
                new Vector3( 0.0f,-0.5f, 0.0f),
                new Vector3(-0.25f,-0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f, -0.25f, 0.0f),
                new Vector3(-0.5f, 0.0f, 0.0f),
                new Vector3(-0.5f, 0.25f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.25f, 0.5f, 0.0f),
                new Vector3( 0.0f, 0.5f, 0.0f),
                new Vector3( 0.25f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.25f, 0.0f),
                new Vector3( 0.5f, 0.0f, 0.0f),
                new Vector3( 0.5f,-0.25f, 0.0f),
            };
            List<Vector3> noIndicesPositionsLineStrip = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
            };
            List<Vector3> noIndicesPositionsTrianglestrip = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
            };
            Runtime.GLTF defaultModel = Common.SinglePlane(); // Only used to get the default indices
            List<int> defaultModelIndices = new List<int>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Indices);
            List<int> linesIndices = new List<int>
            {
                0, 1, 1, 2, 2, 3, 3, 0,
            };
            List<int> lineloopFanIndices = new List<int>
            {
                0, 3, 2, 1,
            };
            List<int> linestripIndices = new List<int>
            {
                0, 3, 2, 1, 0,
            };
            List<int> trianglestripIndices = new List<int>
            {
                0, 3, 1, 2,
            };
            List<int> pointsIndices = new List<int>();
            for (int x = 0; x < noIndicesPositionsPoints.Count; x++)
            {
                pointsIndices.Add(x);
            }
            
            List<Vector2> pointsTextureCoords = new List<Vector2>()
            {
                new Vector2(1.0f, 1.0f),
                new Vector2(0.75f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.25f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.75f),
                new Vector2(0.0f, 0.5f),
                new Vector2(0.0f, 0.25f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.25f, 0.0f),
                new Vector2(0.5f, 0.0f),
                new Vector2(0.75f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 0.25f),
                new Vector2(1.0f, 0.5f),
                new Vector2(1.0f, 0.75f),
            };
            List<Vector2> linesTextureCoords = new List<Vector2>()
            {
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
            };
            List<List<Vector2>> textureCoords = new List<List<Vector2>>()
            {
                pointsTextureCoords,
                linesTextureCoords
            };
            List<Vector3> normalsPoints = new List<Vector3>();
            List<Vector4> tangentsPoints = new List<Vector4>();
            for (int x = 0; x < 16; x++)
            {
                normalsPoints.Add(new Vector3(0.0f, 0.0f, 1.0f));
                tangentsPoints.Add(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
            }
            List<Vector3> normalsLines = new List<Vector3>();
            List<Vector4> tangentsLines = new List<Vector4>();
            for (int x = 0; x < 8; x++)
            {
                normalsLines.Add(new Vector3(0.0f, 0.0f, 1.0f));
                tangentsLines.Add(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
            }
            List<List<Vector3>> normals = new List<List<Vector3>>()
            {
                normalsPoints,
                normalsLines
            };
            List<List<Vector4>> tangents = new List<List<Vector4>>()
            {
                tangentsPoints,
                tangentsLines
            };

            properties = new List<Property>
            {
                new Property(Propertyname.Mode_Points, Runtime.MeshPrimitive.ModeEnum.POINTS, group: 1),
                new Property(Propertyname.Mode_Lines, Runtime.MeshPrimitive.ModeEnum.LINES, group: 1),
                new Property(Propertyname.Mode_Line_Loop, Runtime.MeshPrimitive.ModeEnum.LINE_LOOP, group: 1),
                new Property(Propertyname.Mode_Line_Strip, Runtime.MeshPrimitive.ModeEnum.LINE_STRIP, group: 1),
                new Property(Propertyname.Mode_Triangle_Strip, Runtime.MeshPrimitive.ModeEnum.TRIANGLE_STRIP, group: 1),
                new Property(Propertyname.Mode_Triangle_Fan, Runtime.MeshPrimitive.ModeEnum.TRIANGLE_FAN, group: 1),
                new Property(Propertyname.Mode_Triangles, Runtime.MeshPrimitive.ModeEnum.TRIANGLES, group: 1),
                new Property(Propertyname.IndicesValues_Points, pointsIndices, Propertyname.Mode_Points, group: 2),
                new Property(Propertyname.IndicesValues_Lines, linesIndices, Propertyname.Mode_Lines, group: 2),
                new Property(Propertyname.IndicesValues_LineLoop, lineloopFanIndices, Propertyname.Mode_Line_Loop, group: 2),
                new Property(Propertyname.IndicesValues_LineStrip, linestripIndices, Propertyname.Mode_Line_Strip, group: 2),
                new Property(Propertyname.IndicesValues_TriangleStrip, trianglestripIndices, Propertyname.Mode_Triangle_Strip, group: 2),
                new Property(Propertyname.IndicesValues_TriangleFan, lineloopFanIndices, Propertyname.Mode_Triangle_Fan, group: 2),
                new Property(Propertyname.IndicesValues_Triangles, defaultModelIndices, Propertyname.Mode_Triangles, group: 2),
                new Property(Propertyname.IndicesValues_None, " ", group: 2),
                new Property(Propertyname.IndicesComponentType_Byte, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE, group: 4),
                new Property(Propertyname.IndicesComponentType_Short, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT, group: 4),
                new Property(Propertyname.IndicesComponentType_Int, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT, group: 4),
                new Property(Propertyname.IndicesComponentType_None, " ", group: 4),
                new Property(Propertyname.VertexUV0_Float, ":white_check_mark:", group:5),
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexUV0_Float, textureCoords, group:5),
                new Property(Propertyname.Mode_Points, noIndicesPositionsPoints, group: 1),
                new Property(Propertyname.Mode_Lines, noIndicesPositionsLines, group: 1),
                new Property(Propertyname.Mode_Line_Loop, noIndicesPositionsLineloopFan, group: 1),
                new Property(Propertyname.Mode_Line_Strip, noIndicesPositionsLineStrip, group: 1),
                new Property(Propertyname.Mode_Triangle_Strip, noIndicesPositionsTrianglestrip, group: 1),
                new Property(Propertyname.Mode_Triangle_Fan, noIndicesPositionsLineloopFan, group: 1),
                new Property(Propertyname.Mode_Triangles, noIndicesPositionsTriangles, group: 1),
            };
            // Each mode with and without indices, and drop singles
            var defaultIndices = properties.Find(e => e.name == Propertyname.IndicesComponentType_Int);
            var noIndicesType = properties.Find(e => e.name == Propertyname.IndicesComponentType_None);
            var noIndicesValue = properties.Find(e => e.name == Propertyname.IndicesValues_None);
            foreach (var property in properties)
            {
                if (property.propertyGroup == 1)
                {
                    var IndicesValues = properties.Find(e => e.prerequisite == property.name);
                    specialCombos.Add(ComboHelper.CustomComboCreation(
                        property,
                        defaultIndices,
                        IndicesValues));
                    specialCombos.Add(ComboHelper.CustomComboCreation(
                        property,
                        noIndicesValue,
                        noIndicesType));
                }
                removeCombos.Add(ComboHelper.CustomComboCreation(
                    property));
            }

            var valueTriangles = properties.Find(e => e.name == Propertyname.IndicesValues_Triangles);
            var modeTriangles = properties.Find(e => e.name == Propertyname.Mode_Triangles);
            var modePoints = properties.Find(e => e.name == Propertyname.Mode_Points);
            var modeLines = properties.Find(e => e.name == Propertyname.Mode_Lines);
            var typeByte = properties.Find(e => e.name == Propertyname.IndicesComponentType_Byte);
            var typeShort = properties.Find(e => e.name == Propertyname.IndicesComponentType_Short);
            var vertexUV = properties.Find(e => e.name == Propertyname.VertexUV0_Float);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normalTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            specialCombos.Add(new List<Property>()
            {
                valueTriangles,
                typeByte,
                modeTriangles
            });
            specialCombos.Add(new List<Property>()
            {
                valueTriangles,
                typeShort,
                modeTriangles
            });
            specialCombos.Add(new List<Property>()
            {
                modePoints,
                noIndicesValue,
                noIndicesType,
                vertexUV,
                normal
            });
            specialCombos.Add(new List<Property>()
            {
                modeLines,
                noIndicesValue,
                noIndicesType,
                vertexUV,
                normal
            });
            specialCombos.Add(new List<Property>()
            {
                modeLines,
                noIndicesValue,
                noIndicesType,
                vertexUV,
                normal,
                tangent,
                normalTex
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Removes the empty and full set combos, as well as automaticly created prerequisite combos.
            for (int x = 0; x < 8; x++)
            {
                combos.RemoveAt(0);
            }

            // Sort the combos by complexity
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                if (x.Count == 0) return -1; // Empty Set
                else if (y.Count == 0) return 1; // Empty Set
                else if (x.Count > y.Count) return 1;
                else if (x.Count < y.Count) return -1;
                else if (x.Count == y.Count)
                {
                    // Tie goes to the combo with the left-most property on the table
                    for (int p = 0; p < x.Count; p++)
                    {
                        if (x[p].propertyGroup != y[p].propertyGroup ||
                            x[p].propertyGroup == 0)
                        {
                            int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
                            int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
                            if (xPropertyIndex > yPropertyIndex) return 1;
                            else if (xPropertyIndex < yPropertyIndex) return -1;
                        }
                    }
                    for (int p = 0; p < x.Count; p++)
                    {
                        int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
                        int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
                        if (xPropertyIndex > yPropertyIndex) return 1;
                        else if (xPropertyIndex < yPropertyIndex) return -1;
                    }
                    return 0;
                }
                else return 0;
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Mode_Points ||
                    property.name == Propertyname.Mode_Lines ||
                    property.name == Propertyname.Mode_Line_Loop ||
                    property.name == Propertyname.Mode_Line_Strip ||
                    property.name == Propertyname.Mode_Triangles ||
                    property.name == Propertyname.Mode_Triangle_Strip ||
                    property.name == Propertyname.Mode_Triangle_Fan ||
                    property.name == Propertyname.Mode_Triangles)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Mode = property.value;

                    // Points and Lines uses a different set of vertexes for their base model
                    if (property.name == Propertyname.Mode_Points ||
                        property.name == Propertyname.Mode_Lines)
                    {
                        var modeVertexes = specialProperties.Find(e => e.name == property.name);
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Positions = modeVertexes.value;
                    }
                }
                else if (property.name.ToString().Contains("IndicesValues_") &&
                         property.name != Propertyname.IndicesValues_None)
                {
                    // These modes need a different set of indices than provided by the default model
                    Property indices = null;
                    var mode = combo.Find(e => e.name.ToString().Contains("Mode_"));
                    switch (mode.name)
                    {
                        case Propertyname.Mode_Points:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_Points);
                                break;
                            }
                        case Propertyname.Mode_Lines:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_Lines);
                                break;
                            }
                        case Propertyname.Mode_Line_Loop:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_LineLoop);
                                break;
                            }
                        case Propertyname.Mode_Line_Strip:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_LineStrip);
                                break;
                            }
                        case Propertyname.Mode_Triangle_Strip:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_TriangleStrip);
                                break;
                            }
                        case Propertyname.Mode_Triangle_Fan:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_TriangleFan);
                                break;
                            }
                        case Propertyname.Mode_Triangles:
                            {
                                indices = properties.Find(e => e.name == Propertyname.IndicesValues_Triangles);
                                break;
                            }
                        case Propertyname.IndicesComponentType_None:
                            {
                                indices = null;
                                break;
                            }
                    }
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Indices = indices.value;
                }
                else if (property.name == Propertyname.IndicesComponentType_Byte ||
                         property.name == Propertyname.IndicesComponentType_Short ||
                         property.name == Propertyname.IndicesComponentType_Int)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].IndexComponentType = property.value;
                }
                else if (property.name == Propertyname.IndicesComponentType_None)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Indices = null;

                    // If there are no indicies, some modes need custom positions
                    var mode = combo.Find(e => e.name.ToString().Contains("Mode_"));
                    var modeVertexes = specialProperties.Find(e => e.name == mode.name);
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Positions = modeVertexes.value;
                }
                else
                {
                    int index = -1;
                    if (combo.Find(e => e.name == Propertyname.Mode_Points) != null)
                    {
                        index = 0;
                    }
                    else if (combo.Find(e => e.name == Propertyname.Mode_Lines) != null)
                    {
                        index = 1;
                    }

                    if (property.name == Propertyname.VertexUV0_Float)
                    {
                        List<List<Vector2>> texCoords = new List<List<Vector2>>();
                        if (combo.Find(e => e.name == Propertyname.Mode_Points) != null)
                        {
                            texCoords.Add(specialProperties.Find(e => e.name == Propertyname.VertexUV0_Float).value[index]);
                        }
                        else if (combo.Find(e => e.name == Propertyname.Mode_Lines) != null)
                        {
                            texCoords.Add(specialProperties.Find(e => e.name == Propertyname.VertexUV0_Float).value[index]);
                        }
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = texCoords;
                    }
                    else if (property.name == Propertyname.VertexNormal)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = property.value[index];
                    }
                    else if (property.name == Propertyname.VertexTangent)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = property.value[index];
                    }
                    else if (property.name == Propertyname.NormalTexture)
                    {
                        material.NormalTexture = new Runtime.Texture();
                        material.NormalTexture.Source = property.value;
                        material.NormalTexture.TexCoordIndex = 0;
                    }
                }
            }

            if (material.NormalTexture == null)
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
            }

            return wrapper;
        }
    }
}
