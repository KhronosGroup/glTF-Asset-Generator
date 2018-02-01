using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Linq;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_Mixed : ModelGroup
    {
        public Material_Mixed(List<string> textures, List<string> figures) : base(textures, figures)
        {
            modelGroupName = ModelGroupName.Material_Mixed;
            onlyBinaryProperties = false;
            var baseColorTexture = new Runtime.Image
            {
                Uri = textures.Find(e => e.Contains("BaseColor_X"))
            };
            Runtime.Image figureUVSpace2 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("UVSpace2"))
            };
            Runtime.Image figureUVSpace3 = new Runtime.Image
            {
                Uri = figures.Find(e => e.Contains("UVSpace3"))
            };
            usedTextures.Add(baseColorTexture);
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
                new Property(Propertyname.ExtensionUsed_SpecularGlossiness, "Specular Glossiness", group:1),
                new Property(Propertyname.BaseColorTexture, baseColorTexture)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.SpecularGlossinessOnMaterial0_Yes, null, group:2),
                new Property(Propertyname.SpecularGlossinessOnMaterial0_No, ":x:", group:2),
                new Property(Propertyname.SpecularGlossinessOnMaterial1_Yes, null, group:3),
                new Property(Propertyname.SpecularGlossinessOnMaterial1_No, ":x:", group:3),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split1, primitive0Mesh, group: 4),
                new Property(Propertyname.Primitives_Split2, primitive1Mesh, group: 4),
                new Property(Propertyname.Primitive0VertexUV0, textureCoords0Prim0),
                new Property(Propertyname.Primitive1VertexUV0, textureCoords0Prim2),
            };
            var mat0Yes = properties.Find(e => e.name == Propertyname.SpecularGlossinessOnMaterial0_Yes);
            var mat0No = properties.Find(e => e.name == Propertyname.SpecularGlossinessOnMaterial0_No);
            var mat1Yes = properties.Find(e => e.name == Propertyname.SpecularGlossinessOnMaterial1_Yes);
            var mat1No = properties.Find(e => e.name == Propertyname.SpecularGlossinessOnMaterial1_No);
            specialCombos.Add(new List<Property>()
            {
                mat0No,
                mat1No
            });
            specialCombos.Add(new List<Property>()
            {
                mat0Yes,
                mat1No
            });
            removeCombos.Add(new List<Property>()
            {
                mat0Yes
            });
            removeCombos.Add(new List<Property>()
            {
                mat0No
            });
            removeCombos.Add(new List<Property>()
            {
                mat1Yes
            });
            removeCombos.Add(new List<Property>()
            {
                mat1No
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Removes the empty set models. Don't need the automatic one for this set.
            combos.RemoveAt(0);

            // Sort the combos by complexity
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                if (x[0].value == null &&
                    x[1].value == null) return -1; // Spec gloss on all
                else if (x[0].value == null &&
                         x[1].value != null) return 1; // Split application
                else return 0;
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (var req in requiredProperty)
            {
                if (req.name == Propertyname.ExtensionUsed_SpecularGlossiness)
                {
                    // Initialize SpecGloss for every set
                    material.Extensions = new List<Runtime.Extensions.Extension>();
                    material.Extensions.Add(new Runtime.Extensions.PbrSpecularGlossiness());
                    if (wrapper.ExtensionsUsed == null)
                    {
                        wrapper.ExtensionsUsed = new List<string>();
                    }
                    wrapper.ExtensionsUsed = wrapper.ExtensionsUsed.Union(
                        new string[] { "KHR_materials_pbrSpecularGlossiness" }).ToList();
                }
                else if (req.name == Propertyname.BaseColorTexture)
                {
                    // Apply the fallback MetallicRoughness for every set
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness
                    {
                        BaseColorTexture = new Runtime.Texture
                        {
                            Source = req.value
                        }
                    };
                }
            }
            var material2 = DeepCopy.CloneObject(material);

            Runtime.MeshPrimitive prim0 = null;
            Runtime.MeshPrimitive prim1 = null;
            foreach (var property in specialProperties)
            {
                if (property.name == Propertyname.Primitives_Split1)
                {
                    prim0 = new Runtime.MeshPrimitive
                    {
                        Positions = property.value.Positions,
                        Indices = property.value.Indices,
                    };
                }
                else if (property.name == Propertyname.Primitives_Split2)
                {
                    prim1 = new Runtime.MeshPrimitive
                    {
                        Positions = property.value.Positions,
                        Indices = property.value.Indices,
                    };
                }
                else if (property.propertyGroup == 0
                    )
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
                    {
                        prim0,
                        prim1
                    };
                }

                if (property.name == Propertyname.Primitive0VertexUV0)
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


            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.SpecularGlossinessOnMaterial0_Yes:
                        // Default. No changes needed
                        break;
                    case Propertyname.SpecularGlossinessOnMaterial0_No:
                        material.Extensions = null;
                        break;
                    case Propertyname.SpecularGlossinessOnMaterial1_Yes:
                        // Default. No changes needed
                        break;
                    case Propertyname.SpecularGlossinessOnMaterial1_No:
                        material2.Extensions = null;
                        break;
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = material2;

            return wrapper;
        }
    }
}
