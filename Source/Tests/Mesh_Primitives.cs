using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Mesh_Primitives : Test
    {
        public Mesh_Primitives()
        {
            testType = TestName.Mesh_Primitives;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image iconIndicesPrimitive1 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive1
            };
            Runtime.Image iconIndicesPrimitive2 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive2
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(iconIndicesPrimitive1);
            usedImages.Add(iconIndicesPrimitive2);
            List<Vector3> primitive1Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f)
            };
            List<Vector3> primitive2Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };
            List<int> primitiveTriangleIndices = new List<int>
            {
                0, 1, 2,
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
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
            };
            List<Vector3> normals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f)
            };
            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f)
            };
            List<Vector2> textureCoords0Prim1 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 0.0f, 0.0f)
            };
            List<Vector2> textureCoords1Prim1 = new List<Vector2>()
            {
                new Vector2( 0.5f, 0.5f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 0.5f, 0.0f)
            };
            List<Vector2> textureCoords0Prim2 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f)
            };
            List<Vector2> textureCoords1Prim2 = new List<Vector2>()
            {
                new Vector2( 0.5f, 0.5f),
                new Vector2( 1.0f, 0.5f),
                new Vector2( 1.0f, 0.0f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split4, "Two primitives<br>Neither has attributes set", group: 1),
                new Property(Propertyname.Primitives_Split3, "Two primitives<br>Both have attributes set", group: 1),
                new Property(Propertyname.Primitives_Split1, "Two primitives<br>First (on left) has attributes set", group: 1),
                new Property(Propertyname.Primitives_Split2, "Two primitives<br>Second (on right) has attributes set", group: 1),
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
                new Property(Propertyname.VertexUV0_Prim1, "Default UV"),
                new Property(Propertyname.VertexUV1_Prim1, "Zoomed In UV"),
                new Property(Propertyname.VertexUV0_Prim2, "Default UV"),
                new Property(Propertyname.VertexUV1_Prim2, "Zoomed In UV"),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split1, primitive1Mesh, group: 1),
                new Property(Propertyname.Primitives_Split2, primitive2Mesh, group: 1),
                new Property(Propertyname.VertexUV0_Prim1, textureCoords0Prim1),
                new Property(Propertyname.VertexUV1_Prim1, textureCoords1Prim1),
                new Property(Propertyname.VertexUV0_Prim2, textureCoords0Prim2),
                new Property(Propertyname.VertexUV1_Prim2, textureCoords1Prim2),
            };
            foreach (var property in properties)
            {
                if (property.propertyGroup == 1)
                {
                    var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
                    var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
                    var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
                    var uv0Prim1 = properties.Find(e => e.name == Propertyname.VertexUV0_Prim1);
                    var uv1Prim1 = properties.Find(e => e.name == Propertyname.VertexUV1_Prim1);
                    var uv0Prim2 = properties.Find(e => e.name == Propertyname.VertexUV0_Prim2);
                    var uv1Prim2 = properties.Find(e => e.name == Propertyname.VertexUV1_Prim2);
                    var pbrTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
                    if (property.name != Propertyname.Primitives_Split4)
                    {
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            normal,
                            uv0Prim1,
                            uv0Prim2,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            normal,
                            tangent,
                            uv0Prim1,
                            uv0Prim2,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0Prim1,
                            uv0Prim2,
                            uv1Prim1,
                            uv1Prim2,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0Prim1,
                            uv0Prim2,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0Prim1,
                            uv0Prim2,
                            color));
                    }
                    else
                    {
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0Prim1,
                            uv0Prim2));
                    }
                }
                removeCombos.Add(ComboHelper.CustomComboCreation(
                    property));
            }
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
        {
            // Removes the empty and full set models. Don't need them for this set.
            combos.RemoveAt(0);
            combos.RemoveAt(0);

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            var splitType = combo[0];
            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Primitives_Split1 ||
                         property.name == Propertyname.Primitives_Split2 ||
                         property.name == Propertyname.Primitives_Split3 ||
                         property.name == Propertyname.Primitives_Split4)
                {
                    // Same plane, but split into two triangle primitives
                    var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split1);
                    var primitive2 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split2);
                    Runtime.MeshPrimitive prim1 = new Runtime.MeshPrimitive
                    {
                        Positions = primitive1.value.Positions,
                        Indices = primitive1.value.Indices,
                    };
                    Runtime.MeshPrimitive prim2 = new Runtime.MeshPrimitive
                    {
                        Positions = primitive2.value.Positions,
                        Indices = primitive2.value.Indices,
                    };
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        prim1,
                        prim2
                    };
                }

                if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                        material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 0;
                }

                // Attributes set for only the first primitive
                if (splitType.name == Propertyname.Primitives_Split1)
                {
                    if (property.name == Propertyname.VertexNormal)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = property.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Normals = null;
                    }
                    else if (property.name == Propertyname.VertexTangent)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = property.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Tangents = null;
                    }
                    else if (property.name == Propertyname.VertexColor_Vector4_Float)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = null;
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexUV0_Prim1.ToString())) // All UV0
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.VertexUV0_Prim1).value);
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexUV1_Prim1.ToString())) // All UV1
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.VertexUV1_Prim1).value);
                    }
                }
                // Attributes set for only the second primitive
                else if (splitType.name == Propertyname.Primitives_Split2)
                {
                    if (property.name == Propertyname.VertexNormal)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Normals = property.value;
                    }
                    else if (property.name == Propertyname.VertexTangent)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Tangents = property.value;
                    }
                    else if (property.name == Propertyname.VertexColor_Vector4_Float)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = property.value;
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexUV0_Prim2.ToString())) // All UV0
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.VertexUV0_Prim2).value);
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexUV1_Prim2.ToString())) // All UV1
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.VertexUV1_Prim2).value);
                    }
                }
                // Attributes set for both of the primitives
                else if (splitType.name == Propertyname.Primitives_Split3)
                {
                    if (property.name == Propertyname.VertexNormal)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = property.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Normals = property.value;
                    }
                    else if (property.name == Propertyname.VertexTangent)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = property.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Tangents = property.value;
                    }
                    else if (property.name == Propertyname.VertexColor_Vector4_Float)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = property.value;
                    }
                    else if (property.name == Propertyname.VertexUV0_Prim1)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.VertexUV0_Prim1).value);
                    }
                    else if (property.name == Propertyname.VertexUV0_Prim2)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.VertexUV0_Prim2).value);
                    }
                    else if (property.name == Propertyname.VertexUV1_Prim1)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.VertexUV1_Prim1).value);
                    }
                    else if (property.name == Propertyname.VertexUV1_Prim2)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.VertexUV1_Prim2).value);
                    }
                }
                // Attributes set for neither of the primitives
                else if (splitType.name == Propertyname.Primitives_Split4)
                {
                    
                    if (property.name == Propertyname.VertexNormal)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Normals = null;
                    }
                    else if (property.name == Propertyname.VertexTangent)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Tangents = null;
                    }
                    else if (property.name == Propertyname.VertexColor_Vector4_Float)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = null;
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexUV0_Prim1.ToString())) // All UV0
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = null;
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexUV1_Prim1.ToString())) // All UV1
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = null;
                    }
                }
            }
            // Use the second UV if it has been set
            if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets != null &&
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Count > 1)
            {
                if (material.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                }
            }
            if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets != null &&
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Count > 1)
            {
                if (material.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                }
            }

            if (material.MetallicRoughnessMaterial != null)
            {
                if (splitType.name == Propertyname.Primitives_Split1 ||
                    splitType.name == Propertyname.Primitives_Split3)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
                }
                if (splitType.name == Propertyname.Primitives_Split2 ||
                    splitType.name == Propertyname.Primitives_Split3)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = material;
                }
            }

            return wrapper;
        }
    }
}
