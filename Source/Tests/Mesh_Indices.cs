using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Mesh_Indices : Test
    {
        public Mesh_Indices()
        {
            testType = TestName.Mesh_Indices;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image iconIndices = new Runtime.Image
            {
                Uri = icon_Indices
            };
            usedImages.Add(iconIndices);
            List<Vector3> primitiveNoIndicesPositions = new List<Vector3>()
            {
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f)
            };
            Runtime.GLTF defaultModel = Common.SinglePlane(); // Only used to get the default indices
            List<int> defaultModelIndices = new List<int>(defaultModel.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Indices);
            List<int> primitiveTriangleIndices = new List<int>
            {
                0, 1, 2,
            };
            List<int> linesIndices = new List<int>
            {
                0, 3, 3, 2, 2, 1, 1, 0,
            };
            List<int> lineLoopAndPointsIndices = new List<int>
            {
                0, 3, 2, 1,
            };
            List<int> lineStripIndices = new List<int>
            {
                0, 3, 2, 1, 0,
            };
            List<int> triangleStripIndices = new List<int>
            {
                0, 3, 1, 2,
            };
            List<int> triangleFanIndices = new List<int>
            {
                0, 3, 2, 1,
            };
            Runtime.MeshPrimitive primitiveNoIndicesMesh = new Runtime.MeshPrimitive
            {
                Positions = primitiveNoIndicesPositions,
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
                new Property(Propertyname.IndicesValues_Points, lineLoopAndPointsIndices, Propertyname.Mode_Points, group: 2),
                new Property(Propertyname.IndicesValues_Lines, linesIndices, Propertyname.Mode_Lines, group: 2),
                new Property(Propertyname.IndicesValues_LineLoop, lineLoopAndPointsIndices, Propertyname.Mode_Line_Loop, group: 2),
                new Property(Propertyname.IndicesValues_LineStrip, lineStripIndices, Propertyname.Mode_Line_Strip, group: 2),
                new Property(Propertyname.IndicesValues_TriangleStrip, triangleStripIndices, Propertyname.Mode_Triangle_Strip, group: 2),
                new Property(Propertyname.IndicesValues_TriangleFan, triangleFanIndices, Propertyname.Mode_Triangle_Fan, group: 2),
                new Property(Propertyname.IndicesValues_Triangles, defaultModelIndices, Propertyname.Mode_Triangles, group: 2),
                new Property(Propertyname.IndicesValues_Triangle, primitiveTriangleIndices, group: 2),
                new Property(Propertyname.IndicesComponentType_Byte, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE, group: 4),
                new Property(Propertyname.IndicesComponentType_Short, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT, group: 4),
                new Property(Propertyname.IndicesComponentType_Int, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT, group: 4),
                new Property(Propertyname.IndicesComponentType_None, "No Indices", group: 4),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.IndicesComponentType_None, primitiveNoIndicesMesh, group: 2),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
            };
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Mode_Points)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Mode_Lines)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Mode_Line_Loop)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Mode_Line_Strip)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Mode_Triangle_Strip)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Mode_Triangle_Fan)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesValues_Triangle)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesComponentType_Int)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Removes the empty and full set models. Don't need them for this set.
            //combos.RemoveAt(6);
            //combos.RemoveAt(6);

            // Fills out the Mode and Indices component type fields where empty
            var modeTriangles = properties.Find(e => e.name == Propertyname.Mode_Triangles);
            var indicesTrianglesMode = properties.Find(e => e.name == Propertyname.IndicesValues_Triangles);
            var noIndices = properties.Find(e => e.name == Propertyname.IndicesComponentType_None);
            foreach (var y in combos)
            {
                // Checks if the property is already set in that combo
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(modeTriangles.name.ToString()))) == null)
                {
                    // Show in the log that the mode is Triangles for every model that doesn't have a mode set
                    y.Add(modeTriangles);
                    // Add the Indices Values for every model using a non-standard component type
                    if ((y.Find(e => e.name == indicesTrianglesMode.name)) == null &&
                        (y.Find(e => e.name == noIndices.name)) == null)
                    {
                        y.Add(indicesTrianglesMode);
                    }
                }
            }

            // Show in the log that indices are Int by default
            //var componentTypeInt = properties.Find(e => e.name == Propertyname.IndicesComponentType_Int);
            //foreach (var y in combos)
            //{
            //    // Checks if the property is already set in that combo
            //    if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
            //        LogStringHelper.GenerateNameWithSpaces(componentTypeInt.name.ToString()))) == null)
            //    {
            //        y.Add(componentTypeInt);
            //    }
            //}

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
                    property.name == Propertyname.Mode_Triangle_Fan)
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
                    }
                    if (indices != null)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Indices = indices.value;
                    }
                }
                else if (property.name == Propertyname.IndicesComponentType_Byte ||
                         property.name == Propertyname.IndicesComponentType_Short ||
                         property.name == Propertyname.IndicesComponentType_Int)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].IndexComponentType = property.value;
                }
                else if (property.name == Propertyname.IndicesComponentType_None)
                {
                    var mesh = specialProperties.Find(e => e.name == Propertyname.IndicesComponentType_None);
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0] = mesh.value;
                }
            }

            return wrapper;
        }
    }
}
