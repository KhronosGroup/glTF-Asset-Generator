using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_MetallicRoughness : ModelGroup
    {
        public Material_MetallicRoughness()
        {
            modelGroupName = ModelGroupName.Material_MetallicRoughness;
            onlyBinaryProperties = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image metallicRoughnessTexture = new Runtime.Image
            {
                Uri = texture_MetallicRoughness
            };
            usedTextures.Add(baseColorTexture);
            usedTextures.Add(metallicRoughnessTexture);
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:2),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.BaseColorFactor, new Vector4(0.2f, 0.2f, 0.2f, 0.8f)),
                new Property(Propertyname.MetallicFactor, 0.0f),
                new Property(Propertyname.RoughnessFactor, 0.0f),
                new Property(Propertyname.MetallicRoughnessTexture, metallicRoughnessTexture),
            };
            // Not called explicitly, but values are required here to run ApplySpecialProperties
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:2),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorFactor),
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.MetallicFactor)));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.MetallicRoughnessTexture),
                properties.Find(e => e.name == Propertyname.RoughnessFactor)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Test the VertexColor in combo with BaseColorTexture
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            string vertexColorName = ReadmeStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector3_Float.ToString());
            string baseColorTextureName = ReadmeStringHelper.GenerateNameWithSpaces(Propertyname.BaseColorTexture.ToString());
            foreach (var y in combos)
            {
                // Checks if combos contain the vertexcolor property
                if ((y.Find(e => ReadmeStringHelper.GenerateNameWithSpaces(e.name.ToString()) == vertexColorName)) != null)
                {
                    // Makes sure that BaseColorTexture isn't already in that combo
                    if ((y.Find(e => ReadmeStringHelper.GenerateNameWithSpaces(e.name.ToString()) == baseColorTextureName)) == null)
                    {
                        y.Add(baseColorTexture);
                    }
                }
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
            // Initialize MetallicRoughness for the empty set
            if (combo.Count == 0)
            {
                material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
            }

            foreach (Property property in combo)
            {
                if (material.MetallicRoughnessMaterial == null)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                }

                switch (property.name)
                {
                    case Propertyname.BaseColorFactor:
                        {
                            material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                            break;
                        }
                    case Propertyname.MetallicFactor:
                        {
                            material.MetallicRoughnessMaterial.MetallicFactor = property.value;
                            break;
                        }
                    case Propertyname.RoughnessFactor:
                        {
                            material.MetallicRoughnessMaterial.RoughnessFactor = property.value;
                            break;
                        }
                    case Propertyname.BaseColorTexture:
                        {
                            material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                            material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.MetallicRoughnessTexture:
                        {
                            material.MetallicRoughnessMaterial.MetallicRoughnessTexture = new Runtime.Texture();
                            material.MetallicRoughnessMaterial.MetallicRoughnessTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector3_Float:
                        {
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
