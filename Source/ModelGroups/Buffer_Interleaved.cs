using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Buffer_Interleaved : ModelGroup
    {
        public Buffer_Interleaved(List<string> imageList) : base(imageList)
        {
            modelGroupName = ModelGroupName.Buffer_Interleaved;
            onlyBinaryProperties = false;
            noPrerequisite = false;

            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("BaseColor_Grey"))
            };
            usedTextures.Add(baseColorTexture);
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
            };

            requiredProperty = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexUV0_Float,
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, group:1),
                new Property(Propertyname.VertexUV0_Byte,
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, group:1),
                new Property(Propertyname.VertexUV0_Short,
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, group:1),
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
            };
            var UVF = properties.Find(e => e.name == Propertyname.VertexUV0_Float);
            var UVB = properties.Find(e => e.name == Propertyname.VertexUV0_Byte);
            var UVS = properties.Find(e => e.name == Propertyname.VertexUV0_Short);
            var colorF = properties.Find(e => e.name == Propertyname.VertexColor_Vector3_Float);
            var colorB = properties.Find(e => e.name == Propertyname.VertexColor_Vector3_Byte);
            var colorS = properties.Find(e => e.name == Propertyname.VertexColor_Vector3_Short);
            foreach (var property in properties)
            {
                removeCombos.Add(new List<Property>()
                {
                    property
                });
            }
            specialCombos.Add(new List<Property>()
            {
                UVS,
                colorF
            });
            specialCombos.Add(new List<Property>()
            {
                UVB,
                colorF
            });
            specialCombos.Add(new List<Property>()
            {
                UVF,
                colorS
            });
            specialCombos.Add(new List<Property>()
            {
                UVF,
                colorB
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            combos.RemoveAt(0); // Remove the empty set

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Enabled interleaved buffers
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Interleave = true;

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Sampler = new Runtime.Sampler();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                }
            }

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.VertexColor_Vector3_Float |
                    property.name == Propertyname.VertexColor_Vector3_Byte |
                    property.name == Propertyname.VertexColor_Vector3_Short)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = property.value.componentType;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = property.value.type;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value.colors;
                }
                else if (property.name == Propertyname.VertexUV0_Float ||
                         property.name == Propertyname.VertexUV0_Byte ||
                         property.name == Propertyname.VertexUV0_Short)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordsComponentType = property.value;
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
