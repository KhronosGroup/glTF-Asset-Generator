using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Mesh_PrimitiveVertexColor : ModelGroup
    {
        public Mesh_PrimitiveVertexColor(List<string> figures) : base(figures)
        {
            modelGroupName = ModelGroupName.Mesh_PrimitiveVertexColor;
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
                new Property(Propertyname.VertexColor_Vector3_Float, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC3,
                    vertexColors)),
                new Property(Propertyname.VertexColor_Vector3_Byte, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC3,
                    vertexColors)),
                new Property(Propertyname.VertexColor_Vector3_Short, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC3,
                    vertexColors)),
                new Property(Propertyname.VertexColor_Vector4_Float, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC4,
                    vertexColors)),
                new Property(Propertyname.VertexColor_Vector4_Byte, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC4,
                    vertexColors)),
                new Property(Propertyname.VertexColor_Vector4_Short, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC4,
                    vertexColors)),
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
            // Remove the base model's UV0
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.RemoveAt(0);
            material.MetallicRoughnessMaterial = null;

            foreach (Property property in combo)
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = property.value.componentType;
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = property.value.type;
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value.colors;
            }

            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
