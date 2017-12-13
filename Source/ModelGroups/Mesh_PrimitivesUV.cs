using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Mesh_PrimitivesUV : ModelGroup
    {
        public Mesh_PrimitivesUV()
        {
            modelGroupName = ModelGroupName.Mesh_PrimitivesUV;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image iconIndicesPrimitive0 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive0
            };
            Runtime.Image iconIndicesPrimitive1 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive1
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image iconUVSpace2 = new Runtime.Image
            {
                Uri = icon_UVSpace2
            };
            Runtime.Image iconUVSpace3 = new Runtime.Image
            {
                Uri = icon_UVSpace3
            };
            Runtime.Image iconUVSpace4 = new Runtime.Image
            {
                Uri = icon_UVSpace4
            };
            Runtime.Image iconUVSpace5 = new Runtime.Image
            {
                Uri = icon_UVSpace5
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(normalTexture);
            usedImages.Add(iconIndicesPrimitive0);
            usedImages.Add(iconIndicesPrimitive1);
            usedImages.Add(iconUVSpace2);
            usedImages.Add(iconUVSpace3);
            usedImages.Add(iconUVSpace4);
            usedImages.Add(iconUVSpace5);
            List<Vector3> primitive0Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f)
            };
            List<Vector3> primitive1Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };
            List<int> primitiveTriangleIndices = new List<int>
            {
                0, 1, 2,
            };
            Runtime.MeshPrimitive primitive0Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive0Positions,
                Indices = primitiveTriangleIndices,
            };
            Runtime.MeshPrimitive primitive1Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive1Positions,
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
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
            };
            List<Vector2> textureCoords0Prim0 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 0.0f, 0.0f)
            };
            List<Vector2> textureCoords1Prim0 = new List<Vector2>()
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
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Primitive0VertexUV0, ":white_check_mark:"),
                new Property(Propertyname.Primitive0VertexUV1, ":white_check_mark:"),
                new Property(Propertyname.Primitive1VertexUV0, ":white_check_mark:"),
                new Property(Propertyname.Primitive1VertexUV1, ":white_check_mark:"),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split4, "No attributes set", group: 1),
                new Property(Propertyname.Primitives_Split3, "Attributes set on both", group: 1),
                new Property(Propertyname.Primitives_Split1, "Primitive 0 attributes set", group: 1),
                new Property(Propertyname.Primitives_Split2, "Primitive 1 attributes set", group: 1),
                new Property(Propertyname.Primitive_0, primitive0Mesh),
                new Property(Propertyname.Primitive_1, primitive1Mesh),
                new Property(Propertyname.Primitive0VertexUV0, textureCoords0Prim0),
                new Property(Propertyname.Primitive0VertexUV1, textureCoords1Prim0),
                new Property(Propertyname.Primitive1VertexUV0, textureCoords0Prim2),
                new Property(Propertyname.Primitive1VertexUV1, textureCoords1Prim2),
            };
            var normal = requiredProperty.Find(e => e.name == Propertyname.VertexNormal);
            var normTex = requiredProperty.Find(e => e.name == Propertyname.NormalTexture);
            var tangent = requiredProperty.Find(e => e.name == Propertyname.VertexTangent);
            var color = requiredProperty.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
            var uv0Prim0 = properties.Find(e => e.name == Propertyname.Primitive0VertexUV0);
            var uv1Prim0 = properties.Find(e => e.name == Propertyname.Primitive0VertexUV1);
            var uv0Prim1 = properties.Find(e => e.name == Propertyname.Primitive1VertexUV0);
            var uv1Prim1 = properties.Find(e => e.name == Propertyname.Primitive1VertexUV1);
            var pbrTexture = requiredProperty.Find(e => e.name == Propertyname.BaseColorTexture);
            specialCombos.Add(new List<Property>()
            {
                uv0Prim0,
                uv1Prim0,
                uv0Prim1,
                specialProperties[1], // Both attributes set
                normal,
                tangent,
                color,
                normTex,
                pbrTexture
            });
            specialCombos.Add(new List<Property>()
            {
                uv0Prim0,
                uv0Prim1,
                uv1Prim1,
                specialProperties[1], // Both attributes set
                normal,
                tangent,
                color,
                normTex,
                pbrTexture
            });
            foreach (var property in properties)
            {
                removeCombos.Add(new List<Property>()
                {
                    property,
                });
            }
            foreach (var property in specialProperties)
            {
                if (property.propertyGroup == 1 &&
                    property.name != Propertyname.Primitives_Split4)
                {
                    specialCombos.Add(new List<Property>()
                    {
                        uv0Prim0,
                        uv0Prim1,
                        property,
                        normal,
                        tangent,
                        color,
                        normTex,
                        pbrTexture
                    });
                    specialCombos.Add(new List<Property>()
                    {
                        uv0Prim0,
                        uv0Prim1,
                        uv1Prim0,
                        uv1Prim1,
                        property,
                        normal,
                        tangent,
                        color,
                        normTex,
                        pbrTexture
                    });
                    // Look at the last two special combos and remove UV 0/1 as appropriate
                    for (int x = specialCombos.Count - 2; x < specialCombos.Count; x++)
                    {
                        if (property.name == Propertyname.Primitives_Split1)
                        {
                            specialCombos[x].Remove(uv0Prim1);
                            specialCombos[x].Remove(uv1Prim1);
                        }
                        else if (property.name == Propertyname.Primitives_Split2)
                        {
                            specialCombos[x].Remove(uv0Prim0);
                            specialCombos[x].Remove(uv1Prim0);
                        }
                    }
                }
            }
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Removes the empty and full set models. Don't need the automatic ones for this set.
            combos.RemoveAt(0);
            combos.RemoveAt(0);

            // Moves the split property back to the front of the list, so we create the two primitives first
            // Can't create these combos with this property first because it would shunt the empty and full sets into the middle
            foreach (var combo in combos)
            {
                var splitType = combo.Find(e => e.propertyGroup == 1);
                if (splitType != null)
                {
                    combo.Remove(splitType);
                    combo.Insert(0, splitType);
                }
            }

            // Add an empty set to the front of the list
            var emptySet = specialProperties.Find(e => e.name == Propertyname.Primitives_Split4);
            combos.Insert(0, new List<Property>()
            {
                emptySet
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Determines which of the primitives will have the material and attributes applied to it
            var splitType = combo.Find(e => e.propertyGroup == 1);
            foreach (Property property in combo)
            {
                if (property.name == Propertyname.Primitives_Split1 ||
                         property.name == Propertyname.Primitives_Split2 ||
                         property.name == Propertyname.Primitives_Split3 ||
                         property.name == Propertyname.Primitives_Split4)
                {
                    // Same plane, but split into two triangle primitives
                    var primitive0 = specialProperties.Find(e => e.name == Propertyname.Primitive_0);
                    var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitive_1);
                    Runtime.MeshPrimitive prim0 = new Runtime.MeshPrimitive
                    {
                        Positions = primitive0.value.Positions,
                        Indices = primitive0.value.Indices,
                    };
                    Runtime.MeshPrimitive prim1 = new Runtime.MeshPrimitive
                    {
                        Positions = primitive1.value.Positions,
                        Indices = primitive1.value.Indices,
                    };
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        prim0,
                        prim1
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

                if (property.name == Propertyname.NormalTexture)
                {
                    if (material.NormalTexture == null)
                    {
                        material.NormalTexture = new Runtime.Texture();
                    }
                    material.NormalTexture.Source = property.value;
                    material.NormalTexture.TexCoordIndex = 0;
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
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.Primitive0VertexUV0.ToString())) // All UV0
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.Primitive0VertexUV0).value);
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.Primitive0VertexUV1.ToString())) // All UV1
                    {
                        if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets == null)
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                        }
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.Primitive0VertexUV1).value);
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
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.Primitive1VertexUV0.ToString())) // All UV0
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.Primitive1VertexUV0).value);
                    }
                    else if (LogStringHelper.GenerateNameWithSpaces(property.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.Primitive1VertexUV1.ToString())) // All UV1
                    {
                        if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets == null)
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                        }
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.Primitive1VertexUV1).value);
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
                    else if (property.name == Propertyname.Primitive0VertexUV0)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.Primitive0VertexUV0).value);
                    }
                    else if (property.name == Propertyname.Primitive1VertexUV0)
                    {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.Primitive1VertexUV0).value);
                    }
                    else if (property.name == Propertyname.Primitive0VertexUV1)
                    {
                        if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets == null)
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                        }
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.Primitive0VertexUV1).value);
                    }
                    else if (property.name == Propertyname.Primitive1VertexUV1)
                    {
                        if (wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets == null)
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                        }
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                            specialProperties.Find(e => e.name == Propertyname.Primitive1VertexUV1).value);
                    }
                }
                // Attributes set for neither of the primitives
                else if (splitType.name == Propertyname.Primitives_Split4)
                {
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Normals = null;

                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Tangents = null;

                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = null;

                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = null;

                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = null;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = null;
                }
            }

            // Material needs to be a deep copy here, or both primitives will get the same Mat.
            if (material.MetallicRoughnessMaterial != null)
            {
                if (splitType.name == Propertyname.Primitives_Split1 ||
                    splitType.name == Propertyname.Primitives_Split3)
                {
                    var mat = DeepCopy.CloneObject(material);
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = mat;
                }
                if (splitType.name == Propertyname.Primitives_Split2 ||
                    splitType.name == Propertyname.Primitives_Split3)
                {
                    var mat = DeepCopy.CloneObject(material);
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = mat;
                }
            }

            // Use the second UV if it has been set
            if (splitType.name != Propertyname.Primitives_Split4)
            {
                var prim0UV1 = combo.Find(e => e.name == Propertyname.Primitive0VertexUV1);
                var prim1UV1 = combo.Find(e => e.name == Propertyname.Primitive1VertexUV1);
                if (prim0UV1 != null)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].
                        Material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].
                        Material.NormalTexture.TexCoordIndex = 1;
                }
                if (prim1UV1 != null)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].
                        Material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 1;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].
                        Material.NormalTexture.TexCoordIndex = 1;
                }
            }

            return wrapper;
        }
    }
}
