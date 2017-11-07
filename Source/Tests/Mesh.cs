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
            noPrerequisite = true;
            List<Vector3> primitive1Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };
            List<Vector3> primitive2Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
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
            List<int> primitiveTriangleIndices = new List<int>
            {
                0, 1, 2,
            };
            List<int> linesIndices = new List<int>
            {
                0, 1, 1, 2, 2, 3, 3, 0,
            };
            List<int> lineLoopIndices = new List<int>
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
                0, 1, 2, 3,
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
                new Property(Propertyname.Mode_Triangles, Runtime.MeshPrimitive.ModeEnum.TRIANGLES, group: 1),
                new Property(Propertyname.Mode_Triangle_Strip, Runtime.MeshPrimitive.ModeEnum.TRIANGLE_STRIP, group: 1),
                new Property(Propertyname.Mode_Triangle_Fan, Runtime.MeshPrimitive.ModeEnum.TRIANGLE_FAN, group: 1),
                //new Property(Propertyname.IndicesComponentType_Byte, Runtime. group: 2),
                new Property(Propertyname.IndicesComponentType_None, "No Indices", group: 2),
                new Property(Propertyname.Primitive_Single, "Single primitive", group: 3),
                new Property(Propertyname.Primitive_Split1, "Two primitives<br>First (on right) has attributes set", group: 3),
                new Property(Propertyname.Primitive_Split2, "Two primitives<br>Second (on left) has attributes set", group: 3),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Mode_Lines, linesIndices, group: 1),
                new Property(Propertyname.Mode_Line_Loop, lineLoopIndices, group: 1),
                new Property(Propertyname.Mode_Line_Strip, lineStripIndices, group: 1),
                new Property(Propertyname.Mode_Triangle_Strip, triangleStripIndices, group: 1),
                new Property(Propertyname.Mode_Triangle_Fan, triangleFanIndices, group: 1),
                new Property(Propertyname.IndicesComponentType_None, primitiveNoIndicesMesh, group: 2),
                new Property(Propertyname.Primitive_Split1, primitive1Mesh, group: 3),
                new Property(Propertyname.Primitive_Split2, primitive2Mesh, group: 3),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
            };
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.Primitive_Single)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Show in the log that there is only a single primitive in every model that the plane isn't split
            var singlePrimitive = properties.Find(e => e.name == Propertyname.Primitive_Single);
            foreach (var y in combos)
            {
                // Checks if the property is already set in that combo
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(singlePrimitive.name.ToString()))) == null)
                {
                      y.Add(singlePrimitive);
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
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Mode = property.value;

                    // These modes need a different set of indices than provided by the default model
                    Property indices = null;
                    switch (property.name)
                    {
                        case Propertyname.Mode_Lines:
                            {
                                indices = specialProperties.Find(e => e.name == Propertyname.Mode_Lines);
                                break;
                            }
                        case Propertyname.Mode_Line_Loop:
                            {
                                indices = specialProperties.Find(e => e.name == Propertyname.Mode_Line_Loop);
                                break;
                            }
                        case Propertyname.Mode_Line_Strip:
                            {
                                indices = specialProperties.Find(e => e.name == Propertyname.Mode_Line_Strip);
                                break;
                            }
                        case Propertyname.Mode_Triangle_Strip:
                            {
                                indices = specialProperties.Find(e => e.name == Propertyname.Mode_Triangle_Strip);
                                break;
                            }
                        case Propertyname.Mode_Triangle_Fan:
                            {
                                indices = specialProperties.Find(e => e.name == Propertyname.Mode_Triangle_Fan);
                                break;
                            }
                    }
                    if (indices != null)
                    {
                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Indices = indices.value;
                    }
                }
                else if (property.name == Propertyname.IndicesComponentType_None)
                {
                    var mesh = specialProperties.Find(e => e.name == Propertyname.IndicesComponentType_None);
                    wrapper.Scenes[0].Meshes[0].MeshPrimitives[0] = mesh.value;
                }
                else if (property.name == Propertyname.Primitive_Split1 ||
                    property.name == Propertyname.Primitive_Split2)
                {
                    // Same plane, but split into two triangle primitives
                    var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitive_Split1);
                    var primitive2 = specialProperties.Find(e => e.name == Propertyname.Primitive_Split2);

                    wrapper.Scenes[0].Meshes[0].MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        primitive1.value,
                        primitive2.value
                    };

                    // Applies primitive attribute properties to just one of the two primitives
                    var color = specialProperties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
                    if (property.name == Propertyname.Primitive_Split1)
                    {
                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitive_Split2)
                    {
                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Meshes[0].MeshPrimitives[1].Colors = color.value;
                    }
                }
            }

            return wrapper;
        }
    }
}
