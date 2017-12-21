﻿using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Primitive_Attribute : ModelGroup
    {
        public Primitive_Attribute()
        {
            modelGroupName = ModelGroupName.Primitive_Attribute;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image uvIcon0 = new Runtime.Image
            {
                Uri = icon_UVspace0
            };
            Runtime.Image uvIcon1 = new Runtime.Image
            {
                Uri = icon_UVspace1
            };
            usedImages.Add(normalTexture);
            usedImages.Add(baseColorTexture);
            usedImages.Add(uvIcon0);
            usedImages.Add(uvIcon1);
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f)
            };
            List<Vector2> textureCoords1 = new List<Vector2>()
            {
                new Vector2(1.0f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.0f),
                new Vector2(1.0f, 0.0f)
            };
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
                
            };
            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexUV0_Float, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, group:1),
                new Property(Propertyname.VertexUV0_Byte, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, group:1),
                new Property(Propertyname.VertexUV0_Short, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, group:1),
                new Property(Propertyname.VertexUV1_Float, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, Propertyname.VertexUV0_Float, 2),
                new Property(Propertyname.VertexUV1_Byte, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_UBYTE, Propertyname.VertexUV0_Byte, 2),
                new Property(Propertyname.VertexUV1_Short, 
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.NORMALIZED_USHORT, Propertyname.VertexUV0_Short, 2),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector4_Byte, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector4_Short, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector3_Float, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector3_Byte, vertexColors, group:3),
                new Property(Propertyname.VertexColor_Vector3_Short, vertexColors, group:3),
                new Property(Propertyname.VertexNormal, planeNormals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.TexCoord, textureCoords1),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.VertexUV0_Float,
                    Runtime.MeshPrimitive.TextureCoordsComponentTypeEnum.FLOAT, group:1)
            };
            var uv0 = properties.Find(e => e.name == Propertyname.VertexUV0_Float);
            var uv1 = properties.Find(e => e.name == Propertyname.VertexUV1_Float);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normalTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            specialCombos.Add(new List<Property>()
            {
                uv0,
                uv1,
                normalTex,
            });
            specialCombos.Add(new List<Property>()
            {
                uv0,
                uv1,
                normal,
                normalTex,
            });
            specialCombos.Add(new List<Property>()
            {
                uv0,
                uv1,
                normal,
                tangent,
                normalTex,
            });
            specialCombos.Add(new List<Property>()
            {
                uv0,
                normalTex,
                colorTex
            });
            specialCombos.Add(new List<Property>()
            {
                normal,
                tangent,
                normalTex,
            });
            specialCombos.Add(new List<Property>()
            {
                normal,
                normalTex,
            });
            removeCombos.Add(new List<Property>()
            {
                tangent,
            });
            removeCombos.Add(new List<Property>()
            {
                normalTex,
            });
            removeCombos.Add(new List<Property>()
            {
                colorTex,
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // BaseColorTexture is used everywhere except in the empty set and with vertexcolor
            var baseColorTexture = specialProperties.Find(e => e.name == Propertyname.BaseColorTexture);
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo, or vertexcolor
                if ((y.Find(e => e.name == baseColorTexture.name)) == null &&
                    (y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector3_Float.ToString()))) == null)
                {
                    // Skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            // TextCoord0 is used everywhere a base color texture is used, so include it in everything except the empty set
            var vertexUV0 = specialProperties.Find(e => e.name == Propertyname.VertexUV0_Float);
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(vertexUV0.name.ToString()))) == null)
                {
                    // If there are already values in the combo, just add this new property
                    // Otherwise skip the empty set
                    if (y.Count > 0)
                    {
                        y.Insert(0, vertexUV0);
                    }
                }
            }

            //// Sort the combos so that models using a normal texture are grouped at the top of the readme
            //combos.Sort(delegate (List<Property> x, List<Property> y)
            //{
            //    if (x.Count == 0) return -1; // Empty Set
            //    else if (y.Count == 0) return 1; // Empty Set
            //    else if (x.Count == 7) return -1; // Contains all properties
            //    else if (y.Count == 7) return 1; // Contains all properties
            //    else if (x.Count > y.Count) return 1;
            //    else if (x.Count < y.Count) return -1;
            //    else if (x.Count == y.Count)
            //    {
            //        // Tie goes to the combo with the left-most property on the table
            //        for (int p = 0; p < x.Count; p++)
            //        {
            //            if (x[p].propertyGroup != y[p].propertyGroup ||
            //                x[p].propertyGroup == 0)
            //            {
            //                int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
            //                int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
            //                if (xPropertyIndex > yPropertyIndex) return 1;
            //                else if (xPropertyIndex < yPropertyIndex) return -1;
            //            }
            //        }
            //        for (int p = 0; p < x.Count; p++)
            //        {
            //            int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
            //            int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
            //            if (xPropertyIndex > yPropertyIndex) return 1;
            //            else if (xPropertyIndex < yPropertyIndex) return -1;
            //        }
            //        return 0;
            //    }
            //    else return 0;
            //});

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
                else if (property.name == Propertyname.VertexNormal)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = property.value;
                }
                else if (property.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = property.value;
                    material.NormalTexture.TexCoordIndex = 0;
                }
                else if (property.name == Propertyname.VertexTangent)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = property.value;
                }
                else if (property.name == Propertyname.VertexUV0_Float ||
                         property.name == Propertyname.VertexUV0_Byte ||
                         property.name == Propertyname.VertexUV0_Short)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordsComponentType = property.value;
                }
                else if (property.name == Propertyname.VertexUV1_Float ||
                         property.name == Propertyname.VertexUV1_Byte ||
                         property.name == Propertyname.VertexUV1_Short)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordsComponentType = property.value;

                    if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Count < 2)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.TexCoord).value);
                    }
                }
                else if (property.name == Propertyname.VertexColor_Vector3_Float)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Float)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector3_Byte)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Byte)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_UBYTE;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector3_Short)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Short)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.NORMALIZED_USHORT;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
            }
            if (combo.Count > 0) // Don't set the material on the empty set
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
            }
            // Use the second UV if it has been set
            if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Count > 1)
            {
                if (material.NormalTexture != null)
                {
                    material.NormalTexture.TexCoordIndex = 1;
                }
                if (material.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                }
            }

            return wrapper;
        }
    }
}
