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
            specialProperties = new List<Property>
            {
                new Property(Propertyname.VertexNormal, planeNormals),
            };

            // Declares the properties and their values. Order doesn't matter here
            Property normalTextureProperty = new Property(Propertyname.NormalTexture, normalTexture);
            Property scale = new Property(Propertyname.Scale, 10.0f, Propertyname.NormalTexture);
            Property occlusionTextureProperty = new Property(Propertyname.OcclusionTexture, occlusionTexture);
            Property strength = new Property(Propertyname.Strength, 0.5f, Propertyname.OcclusionTexture);
            Property emissiveTextureProperty = new Property(Propertyname.EmissiveTexture, emissiveTexture);
            Property emissiveFactor = new Property(Propertyname.EmissiveFactor, new Vector3(1.0f, 1.0f, 1.0f));

            // Declares the list of properties. The order here determins the column order.
            properties = new List<Property>
            {
                normalTextureProperty,
                scale,
                occlusionTextureProperty,
                strength,
                emissiveTextureProperty,
                emissiveFactor
            };

            // Declares the combos. The order here determines the number for the model (assending order) and the order that the properties are applied to a model.
            combos.Add(new List<Property>()
            {

            });
            combos.Add(new List<Property>()
            {
                normalTextureProperty
            });
            combos.Add(new List<Property>()
            {
                occlusionTextureProperty
            });
            combos.Add(new List<Property>()
            {
                emissiveFactor
            });
            combos.Add(new List<Property>()
            {
                normalTextureProperty,
                scale
            });
            combos.Add(new List<Property>()
            {
                occlusionTextureProperty,
                strength
            });
            combos.Add(new List<Property>()
            {
                emissiveTextureProperty,
                emissiveFactor
            });
            combos.Add(new List<Property>()
            {
                normalTextureProperty,
                scale,
                occlusionTextureProperty,
                strength,
                emissiveTextureProperty,
                emissiveFactor
            });
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
