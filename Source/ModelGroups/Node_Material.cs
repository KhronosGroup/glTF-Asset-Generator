using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_Material : ModelGroup
    {
        public Node_Material()
        {
            modelGroupName = ModelGroupName.Node_Material;
            onlyBinaryProperties = false;

            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image figureNodes = new Runtime.Image
            {
                Uri = figure_Nodes
            };
            usedTextures.Add(baseColorTexture);
            usedFigures.Add(figureNodes);

            List<Vector4> vertexColors = new List<Vector4>();
            for (int x = 0; x < 12; x++)
            {
                vertexColors.Add(new Vector4(0.0f, 1.0f, 0.0f, 0.2f));
            }
            for (int x = 0; x < 12; x++)
            {
                vertexColors.Add(new Vector4(0.0f, 0.0f, 1.0f, 0.2f));
            }

            requiredProperty = new List<Property>
            {

            };
            properties = new List<Property>
            {
                new Property(Propertyname.Mesh0_None, "None", group:1),
                new Property(Propertyname.Mesh0_Vec3Color, "Vec3 Float Vertex Color", group:1),
                new Property(Propertyname.Mesh0_Vec4Color, "Vec4 Float Vertex Color", group:1),
                new Property(Propertyname.Mesh0_Texture, "Base Color Texture", group:1),
                new Property(Propertyname.Mesh1_None, "None", group:2),
                new Property(Propertyname.Mesh1_Vec3Color, "Vec3 Float Vertex Color", group:2),
                new Property(Propertyname.Mesh1_Vec4Color, "Vec4 Float Vertex Color", group:2),
                new Property(Propertyname.Mesh1_Texture, "Base Color Texture", group:2),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.Mesh0_Vec3Color, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC3,
                    vertexColors)),
                new Property(Propertyname.Mesh0_Vec4Color, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC4,
                    vertexColors)),
                new Property(Propertyname.Mesh1_Vec3Color, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC3,
                    vertexColors)),
                new Property(Propertyname.Mesh1_Vec4Color, group:3, propertyValue: new VertexColor(
                    Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT,
                    Runtime.MeshPrimitive.ColorTypeEnum.VEC4,
                    vertexColors)),
            };

            //var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            //var translation = properties.Find(e => e.name == Propertyname.Translation);
            //var rotation = properties.Find(e => e.name == Propertyname.Rotation);
            //var scale = properties.Find(e => e.name == Propertyname.Scale);
            //removeCombos.Add(new List<Property>()
            //{
            //    matrix,
            //    translation,
            //    rotation,
            //    scale,
            //});
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Switch from the flat plane to the cube
            wrapper = Common.SingleCube();

            //Create the nodes by using the same properties as the original node and then changing positions slightly
            var nodeList = new List<Runtime.Node>();
            nodeList.Add(wrapper.Scenes[0].Nodes[0]);
            nodeList.Add((DeepCopy.CloneObject(wrapper.Scenes[0].Nodes[0])));
            wrapper.Scenes[0].Nodes.Add(nodeList[1]);

            // Changes the new node's positions slightly
            var originPosition = nodeList[0].Mesh.MeshPrimitives[0].Positions;
            List<Vector3> Pos1 = new List<Vector3>();
            foreach (var vec in originPosition)
            {
                Pos1.Add(new Vector3(vec.X - 1.2f, vec.Y - 1.2f, vec.Z));
            }
            nodeList[1].Mesh.MeshPrimitives[0].Positions = Pos1;

            // Name the nodes for debug reasons
            nodeList[0].Name = "Node_0";
            nodeList[1].Name = "Node_1";

            // Make seperate materials for the two meshes
            var material_0 = DeepCopy.CloneObject(material);
            var material_1 = DeepCopy.CloneObject(material);

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Mesh0_None ||
                    property.name == Propertyname.Mesh0_None)
                {
                    // Do nothing
                }
                else if (property.name == Propertyname.Mesh0_Vec3Color ||
                         property.name == Propertyname.Mesh0_Vec4Color)
                {
                    var color = specialProperties.Find(e => e.name == property.name);
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = color.value.componentType;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = color.value.type;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = color.value.colors;
                }
                else if (property.name == Propertyname.Mesh1_Vec3Color ||
                         property.name == Propertyname.Mesh1_Vec4Color)
                {
                    var color = specialProperties.Find(e => e.name == property.name);
                    wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].ColorComponentType = color.value.componentType;
                    wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].ColorType = color.value.type;
                    wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].Colors = color.value.colors;
                }
                else if (property.name == Propertyname.Mesh0_Texture)
                {
                    var texture = specialProperties.Find(e => e.name == Propertyname.BaseColorTexture);
                    material_0.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material_0.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material_0.MetallicRoughnessMaterial.BaseColorTexture.Source = texture.value;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material_0;
                }
                else if (property.name == Propertyname.Mesh1_Texture)
                {
                    var texture = specialProperties.Find(e => e.name == Propertyname.BaseColorTexture);
                    material_1.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material_1.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material_1.MetallicRoughnessMaterial.BaseColorTexture.Source = texture.value;
                    wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].Material = material_1;
                }
            }

            return wrapper;
        }
    }
}
