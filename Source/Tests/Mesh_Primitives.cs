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
            properties = new List<Property>
            {
                new Property(Propertyname.IndicesValues_Triangle, primitiveTriangleIndices, group: 2),
                new Property(Propertyname.Primitive_Split1, "Two primitives<br>First (on left) has attributes set", group: 5),
                new Property(Propertyname.Primitive_Split2, "Two primitives<br>Second (on right) has attributes set", group: 5),
                new Property(Propertyname.Primitive_Split3, "Two primitives<br>Both have attributes set", group: 5),
                new Property(Propertyname.Primitive_Split4, "Two primitives<br>Neither has attributes set", group: 5),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitive_Split1, primitive1Mesh, group: 3),
                new Property(Propertyname.Primitive_Split2, primitive2Mesh, group: 3),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
            };
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.IndicesValues_Triangle)));
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
                if (property.name == Propertyname.Primitive_Split1 ||
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
