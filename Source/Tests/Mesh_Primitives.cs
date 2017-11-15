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
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(normalTexture);
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
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f)
            };
            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( -1.0f, 0.0f, 0.0f, 1.0f)
            };
            List<Vector2> textureCoords2 = new List<Vector2>()
            {
                new Vector2(1.0f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.0f),
                new Vector2(1.0f, 0.0f)
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
                new Property(Propertyname.VertexUV0_Float, "Default UV"),
                new Property(Propertyname.VertexUV1_Float, "Zoomed In UV"),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split1, primitive1Mesh, group: 1),
                new Property(Propertyname.Primitives_Split2, primitive2Mesh, group: 1),
                new Property(Propertyname.VertexUV1_Float, textureCoords2),
            };
            foreach (var property in properties)
            {
                if (property.propertyGroup == 1)
                {
                    var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
                    var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
                    var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
                    var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
                    var uv0 = properties.Find(e => e.name == Propertyname.VertexUV0_Float);
                    var uv1 = properties.Find(e => e.name == Propertyname.VertexUV1_Float);
                    var pbrTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
                    if (property.name != Propertyname.Primitives_Split4)
                    {
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            normal,
                            uv0,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            normal,
                            uv0,
                            normTex,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            normal,
                            tangent,
                            uv0,
                            normTex,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0,
                            uv1,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0,
                            normTex,
                            pbrTexture));
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0,
                            color));
                    }
                    else
                    {
                        specialCombos.Add(ComboHelper.CustomComboCreation(
                            property,
                            uv0));
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

                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        primitive1.value,
                        primitive2.value
                    };

                    // Applies primitive attribute properties to just one of the two primitives
                    var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
                    if (property.name == Propertyname.Primitives_Split1)
                    {
                        // Attributes set for only the first primitive
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitives_Split2)
                    {
                        // Attributes set for only the second primitive
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitives_Split3)
                    {
                        // Attributes set for both of the primitives
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = color.value;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = color.value;
                    }
                    else if (property.name == Propertyname.Primitives_Split4)
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
