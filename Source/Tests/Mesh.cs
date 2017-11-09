using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Mesh : Test
    {
        public Mesh()
        {
            testType = TestName.Mesh;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            string indicesIconSingle = "<img src=\"./Icon_Indices.png\" height=\"72\" width=\"72\" align=\"middle\">";
            string indicesIconSplit = "<img src=\"./Icon_Indices_Primitive2.png\" height=\"72\" width=\"72\" align=\"middle\">" +
                "<img src=\"./Icon_Indices_Primitive1.png\" height=\"72\" width=\"72\" align=\"middle\">";
            Runtime.Image iconIndices = new Runtime.Image
            {
                Uri = icon_Indices
            };
            Runtime.Image iconIndicesPrimitive1 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive1
            };
            Runtime.Image iconIndicesPrimitive2 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive2
            };
            usedImages.Add(iconIndices);
            usedImages.Add(iconIndicesPrimitive1);
            usedImages.Add(iconIndicesPrimitive2);
            List<Vector3> primitive1Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
            };
            List<Vector3> primitive2Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };
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
                0, 1, 1, 2, 2, 3, 3, 0,
            };
            List<int> lineLoopAndPointsIndices = new List<int>
            {
                0, 1, 2, 3,
            };
            List<int> lineStripIndices = new List<int>
            {
                0, 1, 2, 3, 0,
            };
            List<int> triangleStripIndices = new List<int>
            {
                0, 3, 1, 2,
            };
            List<int> triangleFanIndices = new List<int>
            {
                0, 3, 2, 1,
            };
            Runtime.MeshPrimitive primitive1Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive1Positions,
                Indices = primitiveTriangleIndices,
            };
            Runtime.MeshPrimitive primitive2Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive2Positions,
                Indices = primitiveTriangleIndices,
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
                new Property(Propertyname.IndicesLocation_SinglePrimitive, indicesIconSingle, group: 3),
                new Property(Propertyname.IndicesLocation_TwoPrimitives, indicesIconSplit, group: 3),
                new Property(Propertyname.IndicesComponentType_Byte, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_BYTE, group: 4),
                new Property(Propertyname.IndicesComponentType_Short, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_SHORT, group: 4),
                new Property(Propertyname.IndicesComponentType_Int, Runtime.MeshPrimitive.IndexComponentTypeEnum.UNSIGNED_INT, group: 4),
                new Property(Propertyname.IndicesComponentType_None, "No Indices", group: 4),
                new Property(Propertyname.Primitive_Single, "Single primitive", group: 5),
                new Property(Propertyname.Primitive_Split1, "Two primitives<br>First (on left) has attributes set", group: 5),
                new Property(Propertyname.Primitive_Split2, "Two primitives<br>Second (on right) has attributes set", group: 5),
                new Property(Propertyname.Primitive_Split3, "Two primitives<br>Both have attributes set", group: 5),
                new Property(Propertyname.Primitive_Split4, "Two primitives<br>Neither has attributes set", group: 5),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.IndicesComponentType_None, primitiveNoIndicesMesh, group: 2),
                new Property(Propertyname.Primitive_Split1, primitive1Mesh, group: 3),
                new Property(Propertyname.Primitive_Split2, primitive2Mesh, group: 3),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
            };
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Primitive_Single)));
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
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesLocation_SinglePrimitive)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesLocation_TwoPrimitives)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Removes the empty and full set models. Don't need them for this set.
            combos.RemoveAt(6);
            combos.RemoveAt(6);

            // Show in the log that there is only a single primitive in every model that the plane isn't split
            var singlePrimitive = properties.Find(e => e.name == Propertyname.Primitive_Single);
            var indicesSingle = properties.Find(e => e.name == Propertyname.IndicesLocation_SinglePrimitive);
            var indicesSplit = properties.Find(e => e.name == Propertyname.IndicesLocation_TwoPrimitives);
            foreach (var y in combos)
            {
                // Checks if the property is already set in that combo
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(singlePrimitive.name.ToString()))) == null)
                {
                    y.Add(singlePrimitive);
                    y.Add(indicesSingle);
                }
                else
                {
                    y.Add(indicesSplit);
                }
            }

            // Fills out the Mode and Indices component type fields where empty
            var modeTriangles = properties.Find(e => e.name == Propertyname.Mode_Triangles);
            var indicesTwoTriangles = properties.Find(e => e.name == Propertyname.IndicesValues_Triangle);
            var indicesTrianglesMode = properties.Find(e => e.name == Propertyname.IndicesValues_Triangles);
            var noIndices = properties.Find(e => e.name == Propertyname.IndicesComponentType_None);
            var primitive = properties.Find(e => e.name == Propertyname.Primitive_Single);
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
                    // Add the Indices Values for every model with two primitives
                    if ((y.Find(e => e.name == primitive.name)) == null)
                    {
                        y.Add(indicesTwoTriangles);
                    }
                }
            }

            // Show in the log that indices are Int by default
            var componentTypeInt = properties.Find(e => e.name == Propertyname.IndicesComponentType_Int);
            foreach (var y in combos)
            {
                // Checks if the property is already set in that combo
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(componentTypeInt.name.ToString()))) == null)
                {
                    y.Add(componentTypeInt);
                }
            }

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
                else if (property.name == Propertyname.Primitive_Split1 ||
                         property.name == Propertyname.Primitive_Split2 ||
                         property.name == Propertyname.Primitive_Split3 ||
                         property.name == Propertyname.Primitive_Split4)
                {
                    // Same plane, but split into two triangle primitives
                    var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitive_Split1);
                    var primitive2 = specialProperties.Find(e => e.name == Propertyname.Primitive_Split2);

                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        primitive1.value,
                        primitive2.value
                    };

                    // Applies primitive attribute properties to just one of the two primitives
                    var color = specialProperties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
                    if (property.name == Propertyname.Primitive_Split1)
                    {
                        // Attributes set for only the first primitive
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitive_Split2)
                    {
                        // Attributes set for only the second primitive
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitive_Split3)
                    {
                        // Attributes set for both of the primitives
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = color.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitive_Split4)
                    {
                        // Attributes set for neither of the primitives
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = null;
                    }
                }
            }

            return wrapper;
        }
    }
}
