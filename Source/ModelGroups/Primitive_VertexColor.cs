using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Primitive_VertexColor : ModelGroup
    {
        public Primitive_VertexColor()
        {
            modelGroupName = ModelGroupName.Primitive_VertexColor;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
                
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector3_Float, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector3_Byte, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector3_Short, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector4_Byte, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector4_Short, vertexColors, group:3),
            };
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            combos.RemoveAt(0); // Remove the empty set
            combos.RemoveAt(0); // Remove the full set

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Remove the base model's UV0 on the empty set
            if (combo.Count < 0)
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.RemoveAt(0);
                material.MetallicRoughnessMaterial = null;
            }

            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.VertexColor_Vector3_Float:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector4_Float:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector3_Byte:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector4_Byte:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector3_Short:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector4_Short:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                }
            }
            if (combo.Count > 0) // Don't set the material on the empty set
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
            }

            return wrapper;
        }
    }
}
