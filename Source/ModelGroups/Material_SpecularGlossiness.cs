using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Linq;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_SpecularGlossiness : ModelGroup
    {
        public Material_SpecularGlossiness()
        {
            modelGroupName = ModelGroupName.Material_SpecularGlossiness;
            onlyBinaryProperties = false;
            var diffuseTexture = new Runtime.Image
            {
                Uri = texture_Diffuse
            };
            var specularGlossinessTexture = new Runtime.Image
            {
                Uri = texture_SpecularGlossiness
            };
            var baseColorTexture = new Runtime.Image
            {
                Uri = texture_Error
            };
            usedImages.Add(diffuseTexture);
            usedImages.Add(specularGlossinessTexture);
            usedImages.Add(baseColorTexture);
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
                new Property(Propertyname.SpecularFactor, new Vector3(0.4f, 0.4f, 0.4f), group:1),
                new Property(Propertyname.SpecularFactor_Override, new Vector3(0.0f, 0.0f, 0.0f), group:1),
                new Property(Propertyname.GlossinessFactor, 0.3f),
                new Property(Propertyname.SpecularGlossinessTexture, specularGlossinessTexture),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.DiffuseFactor),
                properties.Find(e => e.name == Propertyname.DiffuseTexture)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SpecularGlossinessTexture),
                properties.Find(e => e.name == Propertyname.SpecularFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SpecularGlossinessTexture),
                properties.Find(e => e.name == Propertyname.GlossinessFactor)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SpecularFactor_Override)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Test the VertexColor in combo with DiffuseTexture
            var diffuseTexture = properties.Find(e => e.name == Propertyname.DiffuseTexture);
            string vertexColorName = LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector3_Float.ToString());
            string diffuseTextureName = LogStringHelper.GenerateNameWithSpaces(Propertyname.DiffuseTexture.ToString());
            foreach (var y in combos)
            {
                // Checks if combos contain the vertexcolor property
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == vertexColorName)) != null)
                {
                    // Makes sure that diffuseTexture isn't already in that combo
                    if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == diffuseTextureName)) == null)
                    {
                        y.Add(diffuseTexture);
                    }
                }
            }

            // When not testing SpecularFactor, set it to all 0s to avoid a default of 1s overriding the diffuse texture.
            var specularFactorOverride = properties.Find(e => e.name == Propertyname.SpecularFactor_Override);
            foreach (var y in combos)
            {
                // Not one of the empty sets, doesn't already have SpecFactor set. is using a DiffuseTexture
                if (y.Count > 0 &&
                   (y.Find(e => e.name == Propertyname.SpecularFactor)) == null &&
                   (y.Find(e => e.name == Propertyname.DiffuseTexture)) != null)
                {
                    y.Add(specularFactorOverride);
                }
            }

            var diffuseFactor = properties.Find(e => e.name == Propertyname.DiffuseFactor);
            var glossinessFactor = properties.Find(e => e.name == Propertyname.GlossinessFactor);
            combos.Add(new List<Property>()
            {
                diffuseTexture,
                diffuseFactor
            });
            combos.Add(new List<Property>()
            {
                diffuseTexture,
                glossinessFactor
            });

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
