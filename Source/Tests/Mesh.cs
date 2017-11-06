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
            };
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
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
                }
            }

            return wrapper;
        }
    }
}
