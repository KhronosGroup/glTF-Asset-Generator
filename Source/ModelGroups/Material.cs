using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material : ModelGroup
    {
        public Material(List<string> imageList) : base(imageList)
        {
            modelGroupName = ModelGroupName.Material;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image emissiveTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Emissive_Plane"))
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Normal_Plane"))
    };
            Runtime.Image occlusionTexture = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Occlusion_Plane"))
};
            usedTextures.Add(emissiveTexture);
            usedTextures.Add(normalTexture);
            usedTextures.Add(occlusionTexture);
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f)
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.MetallicFactor, 0.0f),
                new Property(Propertyname.BaseColorFactor, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.Scale, 10.0f, Propertyname.NormalTexture),
                new Property(Propertyname.OcclusionTexture, occlusionTexture),
                new Property(Propertyname.Strength, 0.5f, Propertyname.OcclusionTexture),
                new Property(Propertyname.EmissiveTexture, emissiveTexture),
                new Property(Propertyname.EmissiveFactor, new Vector3(1.0f, 1.0f, 1.0f)),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexNormal, planeNormals),
            };
            specialCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.EmissiveFactor),
                properties.Find(e => e.name == Propertyname.EmissiveTexture)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.EmissiveTexture)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
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
            material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.MetallicFactor)
                {
                    material.MetallicRoughnessMaterial.MetallicFactor = req.value;
                }
                else if (req.name == Propertyname.BaseColorFactor)
                {
                    material.MetallicRoughnessMaterial.BaseColorFactor = req.value;
                }
            }

            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.EmissiveFactor:
                        {
                            material.EmissiveFactor = property.value;
                            break;
                        }
                    case Propertyname.NormalTexture:
                        {
                            material.NormalTexture = new Runtime.Texture();
                            material.NormalTexture.Source = property.value;

                            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals =
                                specialProperties.Find(e => e.name == Propertyname.VertexNormal).value;
                            break;
                        }
                    case Propertyname.Scale:
                        {
                            material.NormalScale = property.value;
                            break;
                        }
                    case Propertyname.OcclusionTexture:
                        {
                            material.OcclusionTexture = new Runtime.Texture();
                            material.OcclusionTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.Strength:
                        {
                            material.OcclusionStrength = property.value;
                            break;
                        }
                    case Propertyname.EmissiveTexture:
                        {
                            material.EmissiveTexture = new Runtime.Texture();
                            material.EmissiveTexture.Source = property.value;
                            break;
                        }
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
