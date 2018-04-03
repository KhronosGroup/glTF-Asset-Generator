using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Linq;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_SpecularGlossiness : ModelGroup
    {
        public Material_SpecularGlossiness(List<string> imageList) : base(imageList)
        {
            modelGroupName = ModelGroupName.Material_SpecularGlossiness;
            onlyBinaryProperties = false;
            var diffuseTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Diffuse_Plane"))
            };
            var specularGlossinessTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("SpecularGlossiness_Plane"))
            };
            var baseColorTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("BaseColor_X"))
            };
            usedTextures.Add(diffuseTexture);
            usedTextures.Add(specularGlossinessTexture);
            usedTextures.Add(baseColorTexture);
            var colorCoord = new List<Vector4>()
            {
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f)
            };

            requiredProperty = new List<Property>
            {
                new Property(Propertyname.ExtensionUsed_SpecularGlossiness, "Specular Glossiness", group:3),
                new Property(Propertyname.BaseColorTexture, baseColorTexture)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:2),
                new Property(Propertyname.DiffuseTexture, diffuseTexture),
                new Property(Propertyname.DiffuseFactor, new Vector4(0.2f, 0.2f, 0.2f, 0.8f)),
                new Property(Propertyname.SpecularGlossinessTexture, specularGlossinessTexture),
                new Property(Propertyname.SpecularFactor, new Vector3(0.4f, 0.4f, 0.4f), group:1),
                new Property(Propertyname.SpecularFactor_Override, new Vector3(0.0f, 0.0f, 0.0f), group:1),
                new Property(Propertyname.GlossinessFactor, 0.3f),
            };

            var vertColor = properties.Find(e => e.name == Propertyname.VertexColor_Vector3_Float);
            var diffTex = properties.Find(e => e.name == Propertyname.DiffuseTexture);
            var diffFac = properties.Find(e => e.name == Propertyname.DiffuseFactor);
            var specGlossTex = properties.Find(e => e.name == Propertyname.SpecularGlossinessTexture);
            var specFac = properties.Find(e => e.name == Propertyname.SpecularFactor);
            var glossFac = properties.Find(e => e.name == Propertyname.GlossinessFactor);
            var specFacOverride = properties.Find(e => e.name == Propertyname.SpecularFactor_Override);
            specialCombos.Add(new List<Property>()
            {
                diffTex,
                diffFac,
                specFacOverride,
            });
            specialCombos.Add(new List<Property>()
            {
                specGlossTex,
                specFac,
            });
            specialCombos.Add(new List<Property>()
            {
                specGlossTex,
                glossFac
            });
            specialCombos.Add(new List<Property>()
            {
                diffFac,
                specFacOverride,
            });
            specialCombos.Add(new List<Property>()
            {
                vertColor,
                diffTex,
                specFacOverride,
            });
            specialCombos.Add(new List<Property>()
            {
                diffTex,
                specFacOverride,
            });
            specialCombos.Add(new List<Property>()
            {
                diffTex,
                glossFac,
            });
            specialCombos.Add(new List<Property>()
            {
                diffTex,
                specFac,
                glossFac,
            });
            specialCombos.Add(new List<Property>()
            {
                vertColor,
                specFacOverride,
            });


            removeCombos.Add(new List<Property>()
            {
                vertColor,
            });
            removeCombos.Add(new List<Property>()
            {
                diffTex,
            });
            removeCombos.Add(new List<Property>()
            {
                diffFac,
            });
            removeCombos.Add(new List<Property>()
            {
                specFacOverride,
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Sort the combos by complexity
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                // Doesn't count the Specular Factor override in the total number of properties for the purpose of sorting
                int xTrueCount;
                int yTrueCount;

                if ((x.Find(e => e.name == Propertyname.SpecularFactor_Override)) != null)
                {
                    xTrueCount = x.Count - 1;
                }
                else
                {
                    xTrueCount = x.Count;
                }

                if ((y.Find(e => e.name == Propertyname.SpecularFactor_Override)) != null)
                {
                    yTrueCount = y.Count - 1;
                }
                else
                {
                    yTrueCount = y.Count;
                }

                // Sorting checks
                if (xTrueCount == 0) return -1; // Empty Set
                else if (yTrueCount == 0) return 1; // Empty Set
                else if (xTrueCount > yTrueCount) return 1;
                else if (xTrueCount < yTrueCount) return -1;
                else if (xTrueCount == yTrueCount)
                {
                    // Tie goes to the combo with the left-most property on the table, not counting the Specular Factor override
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

            var extension = material.Extensions[0] as Runtime.Extensions.PbrSpecularGlossiness;
            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.DiffuseFactor:
                        extension.DiffuseFactor = property.value;
                        break;
                    case Propertyname.SpecularFactor:
                        extension.SpecularFactor = property.value;
                        break;
                    case Propertyname.SpecularFactor_Override:
                        extension.SpecularFactor = property.value;
                        break;
                    case Propertyname.GlossinessFactor:
                        extension.GlossinessFactor = property.value;
                        break;
                    case Propertyname.DiffuseTexture:
                        extension.DiffuseTexture = new Runtime.Texture();
                        extension.DiffuseTexture.Source = property.value;
                        break;
                    case Propertyname.SpecularGlossinessTexture:
                        extension.SpecularGlossinessTexture = new Runtime.Texture();
                        extension.SpecularGlossinessTexture.Source = property.value;
                        break;
                    case Propertyname.OcclusionTexture:
                        material.OcclusionTexture = new Runtime.Texture();
                        material.OcclusionTexture.Source = property.value;
                        break;
                    case Propertyname.VertexColor_Vector3_Float:
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                        break;
                    case Propertyname.ExtensionUsed_SpecularGlossiness:
                        if (wrapper.ExtensionsUsed == null)
                        {
                            wrapper.ExtensionsUsed = new List<string>();
                        }
                        wrapper.ExtensionsUsed = wrapper.ExtensionsUsed.Union(
                            new string[] { "KHR_materials_pbrSpecularGlossiness" }).ToList();
                        break;
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
            return wrapper;
        }
    }
}
