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
            usedImages.Add(baseColorTexture);
            usedImages.Add(metallicRoughnessTexture);
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
                new Property(Propertyname.BaseColorFactor, new Vector4(0.2f, 0.2f, 0.2f, 0.8f)),
                new Property(Propertyname.MetallicFactor, 0.0f),
                new Property(Propertyname.RoughnessFactor, 0.0f),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
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
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Test the VertexColor in combo with BaseColorTexture
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            string vertexColorName = LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector3_Float.ToString());
            string baseColorTextureName = LogStringHelper.GenerateNameWithSpaces(Propertyname.BaseColorTexture.ToString());
            foreach (var y in combos)
            {
                // Checks if combos contain the vertexcolor property
                if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == vertexColorName)) != null)
                {
                    // Makes sure that BaseColorTexture isn't already in that combo
                    if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == baseColorTextureName)) == null)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            // Inserts the solo BaseColorTexture model next to the other models that use the texture
            combos.Insert(3, ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.BaseColorTexture)));

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
