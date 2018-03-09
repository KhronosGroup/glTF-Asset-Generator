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
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
            };
            List<Vector3> noIndicesPositionsLineloopPointsFan = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
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
                0, 3, 3, 2, 2, 1, 1, 0,
            };
            List<int> lineloopPointsFanIndices = new List<int>
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
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
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
                new Property(Propertyname.IndicesValues_Points, lineloopPointsFanIndices, Propertyname.Mode_Points, group: 2),
                new Property(Propertyname.IndicesValues_Lines, linesIndices, Propertyname.Mode_Lines, group: 2),
                new Property(Propertyname.IndicesValues_LineLoop, lineloopPointsFanIndices, Propertyname.Mode_Line_Loop, group: 2),
                new Property(Propertyname.IndicesValues_LineStrip, linestripIndices, Propertyname.Mode_Line_Strip, group: 2),
                new Property(Propertyname.IndicesValues_TriangleStrip, trianglestripIndices, Propertyname.Mode_Triangle_Strip, group: 2),
                new Property(Propertyname.IndicesValues_TriangleFan, lineloopPointsFanIndices, Propertyname.Mode_Triangle_Fan, group: 2),
                new Property(Propertyname.IndicesValues_Triangles, defaultModelIndices, Propertyname.Mode_Triangles, group: 2),
                new Property(Propertyname.IndicesValues_None, " ", group: 2),
                new Property(Propertyname.IndicesComponentType_Byte, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE, group: 4),
                new Property(Propertyname.IndicesComponentType_Short, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT, group: 4),
                new Property(Propertyname.IndicesComponentType_Int, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT, group: 4),
                new Property(Propertyname.IndicesComponentType_None, " ", group: 4),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
                new Property(Propertyname.Mode_Points, noIndicesPositionsLineloopPointsFan, group: 1),
                new Property(Propertyname.Mode_Lines, noIndicesPositionsLines, group: 1),
                new Property(Propertyname.Mode_Line_Loop, noIndicesPositionsLineloopPointsFan, group: 1),
                new Property(Propertyname.Mode_Line_Strip, noIndicesPositionsLineStrip, group: 1),
                new Property(Propertyname.Mode_Triangle_Strip, noIndicesPositionsTrianglestrip, group: 1),
                new Property(Propertyname.Mode_Triangle_Fan, noIndicesPositionsLineloopPointsFan, group: 1),
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
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesValues_Triangles),
                properties.Find(e => e.name == Propertyname.IndicesComponentType_Byte),
                properties.Find(e => e.name == Propertyname.Mode_Triangles)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesValues_Triangles),
                properties.Find(e => e.name == Propertyname.IndicesComponentType_Short),
                properties.Find(e => e.name == Propertyname.Mode_Triangles)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Removes the empty and full set combos, as well as automaticly created prerequisite combos.
            for (int x = 0; x < 8; x++)
            {
                combos.RemoveAt(0);
            }

            // Sorts the models so all of the ones without indices are together in the MD log
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                var xIndices = x.Find(e => e.name == Propertyname.IndicesValues_None);
                var yIndices = y.Find(e => e.name == Propertyname.IndicesValues_None);

                if (xIndices == null && yIndices == null) return 0;
                else if (xIndices == null) return 1;
                else if (yIndices == null) return -1;
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

                    // These modes need a different set of indices than provided by the default model
                    Property indices = null;
                    switch (property.name)
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
                    }
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Mode = property.value;
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
                    var mode = combo.Find(e => e.propertyGroup == 1);
                    var modeVertexes = specialProperties.Find(e => e.name == mode.name);
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Positions = modeVertexes.value;
                }
            }

            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;

            return wrapper;
        }
    }
}
