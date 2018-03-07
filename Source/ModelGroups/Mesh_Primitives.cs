using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Mesh_Primitives : ModelGroup
    {
        public Mesh_Primitives(List<string> textures, List<string> figures) : base(textures, figures)
        {
            modelGroupName = ModelGroupName.Mesh_Primitives;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image figureIndicesPrimitive0 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("Indices_Primitive0"))
            };
            Runtime.Image figureIndicesPrimitive1 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("Indices_Primitive1"))
            };
            Runtime.Image figureUVSpace2 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("UVSpace2"))
            };
            Runtime.Image figureUVSpace3 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("UVSpace3"))
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("BaseColor_Plane"))
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("Normal_Plane"))
            };
            usedTextures.Add(baseColorTexture);
            usedTextures.Add(normalTexture);
            usedFigures.Add(figureIndicesPrimitive0);
            usedFigures.Add(figureIndicesPrimitive1);
            usedFigures.Add(figureUVSpace2);
            usedFigures.Add(figureUVSpace3);
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
            List<Vector2> textureCoords0Prim2 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f)
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.Primitive0VertexUV0, "UV 0 mapping"),
                new Property(Propertyname.Primitive1VertexUV0, "UV 0 mapping"),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.VertexTangent, tangents),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split1, primitive0Mesh, group: 1),
                new Property(Propertyname.Primitives_Split2, primitive1Mesh, group: 1),
                new Property(Propertyname.Primitive0VertexUV0, textureCoords0Prim0),
                new Property(Propertyname.Primitive1VertexUV0, textureCoords0Prim2),
            };
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var pbrTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
            specialCombos.Add(new List<Property>()
            {
                normal,
                tangent,
                normTex,
                pbrTexture
            });
            specialCombos.Add(new List<Property>()
            {
                normal,
                pbrTexture
            });
            removeCombos.Add(new List<Property>()
            {
                normal
            });
            removeCombos.Add(new List<Property>()
            {
                tangent
            });
            removeCombos.Add(new List<Property>()
            {
                normTex
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Adds UV 0 for both primitives into each model
            var uv0Prim0 = requiredProperty.Find(e => e.name == Propertyname.Primitive0VertexUV0);
            var uv1Prim0 = requiredProperty.Find(e => e.name == Propertyname.Primitive1VertexUV0);
            foreach (var combo in combos)
            {
                combo.Add(uv0Prim0);
                combo.Add(uv1Prim0);
            }

            // Sort the combos by complexity
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                if (x.Count == 0) return -1; // Empty Set
                else if (y.Count == 0) return 1; // Empty Set
                else if (x.Count > y.Count) return 1;
                else if (x.Count < y.Count) return -1;
                else if (x.Count == y.Count)
                {
                    // Tie goes to the combo with the left-most property on the table
                    for (int p = 0; p < x.Count; p++)
                    {
                        if (x[p].propertyGroup != y[p].propertyGroup ||
                            x[p].propertyGroup == 0)
                        {
                            int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
                            int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
                            if (xPropertyIndex > yPropertyIndex) return 1;
                            else if (xPropertyIndex < yPropertyIndex) return -1;
                        }
                    }
                    for (int p = 0; p < x.Count; p++)
                    {
                        int xPropertyIndex = properties.FindIndex(e => e.name == x[p].name);
                        int yPropertyIndex = properties.FindIndex(e => e.name == y[p].name);
                        if (xPropertyIndex > yPropertyIndex) return 1;
                        else if (xPropertyIndex < yPropertyIndex) return -1;
                    }
                    return 0;
                }
                else return 0;
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Same plane, but split into two triangle primitives
            var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split1);
            var primitive2 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split2);
            Runtime.MeshPrimitive prim0 = new Runtime.MeshPrimitive
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
                prim0,
                prim2
            };

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

                if (property.name == Propertyname.NormalTexture)
                {
                    if (material.NormalTexture == null)
                    {
                        material.NormalTexture = new Runtime.Texture();
                    }
                    material.NormalTexture.Source = property.value;
                    material.NormalTexture.TexCoordIndex = 0;
                }

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
            }

            if (material.MetallicRoughnessMaterial != null)
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = material;
            }

            return wrapper;
        }
    }
}
