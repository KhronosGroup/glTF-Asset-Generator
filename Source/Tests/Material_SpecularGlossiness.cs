using System.Collections.Generic;

namespace AssetGenerator.Tests
{
    [TestAttribute]
    class Material_SpecularGlossiness : Test
    {
        public Material_SpecularGlossiness()
        {
            testType = TestName.Material_SpecularGlossiness;
            onlyBinaryProperties = false;
            Runtime.Image diffuseTexture = new Runtime.Image
            {
                Uri = texture_Diffuse
            };
            Runtime.Image specularGlossinessTexture = new Runtime.Image
            {
                Uri = texture_SpecularGlossiness
            };
            usedImages.Add(diffuseTexture);
            usedImages.Add(specularGlossinessTexture);
            List<Vector4> colorCoord = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.8f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.8f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.DiffuseFactor, new Vector4(0.2f, 0.2f, 0.2f, 0.8f)),
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:2),
                new Property(Propertyname.SpecularFactor, new Vector3(0.4f, 0.4f, 0.4f), group:1),
                new Property(Propertyname.SpecularFactor_Override, new Vector3(0.0f, 0.0f, 0.0f), group:1),
                new Property(Propertyname.GlossinessFactor, 0.3f),
                new Property(Propertyname.DiffuseTexture, diffuseTexture),
                new Property(Propertyname.SpecularGlossinessTexture, specularGlossinessTexture),
            };
            // Not called explicitly, but values are required here to run ApplySpecialProperties
            specialProperties = new List<Property>
            {
                new Property(Propertyname.SpecularFactor_Override, new Vector3(0.0f, 0.0f, 0.0f), group:1),
                new Property(Propertyname.VertexColor_Vector3_Float, colorCoord, group:2),
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
                properties.Find(e => e.name == Propertyname.DiffuseTexture)));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.SpecularFactor_Override)));
        }

        override public List<List<Property>> ApplySpecialProperties(Test test, List<List<Property>> combos)
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
                    // Makes sure that BaseColorTexture isn't already in that combo
                    if ((y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) == diffuseTextureName)) == null)
                    {
                        y.Add(diffuseTexture);
                    }
                }
            }

            // Inserts the solo DiffuseTexture model next to the other models that use the texture
            combos.Insert(3, ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.DiffuseTexture)));

            // When not testing SpecularFactor, set it to all 0s to avoid a default of 1s overriding the diffuse texture
            var specularFactorOverride = specialProperties.Find(e => e.name == Propertyname.SpecularFactor_Override);
            foreach (var y in combos)
            {
                // Not the empty set, doesn't already have SpecFactor set. is using a DiffuseTexture
                if (y.Count > 0 &&
                   (y.Find(e => e.name == Propertyname.SpecularFactor)) == null &&
                   (y.Find(e => e.name == Propertyname.DiffuseTexture)) != null)
                {
                    y.Add(specularFactorOverride);
                }
            }

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo)
        {
            // Initialize SpecGloss for the empty set
            if (combo.Count == 0)
            {
                material.Extensions = new List<Runtime.Extensions.Extension>();
                material.Extensions.Add(new Runtime.Extensions.PbrSpecularGlossiness());
            }

            foreach (Property property in combo)
            {
                if (material.Extensions == null)
                {
                    material.Extensions = new List<Runtime.Extensions.Extension>();
                    material.Extensions.Add(new Runtime.Extensions.PbrSpecularGlossiness());
                }

                var extension = material.Extensions[0] as Runtime.Extensions.PbrSpecularGlossiness;

                switch (property.name)
                {
                    case Propertyname.DiffuseFactor:
                        {
                            extension.DiffuseFactor = property.value;
                            break;
                        }
                    case Propertyname.SpecularFactor:
                        {
                            extension.SpecularFactor = property.value;
                            break;
                        }
                    case Propertyname.SpecularFactor_Override:
                        {
                            extension.SpecularFactor = property.value;
                            break;
                        }
                    case Propertyname.GlossinessFactor:
                        {
                            extension.GlossinessFactor = property.value;
                            break;
                        }
                    case Propertyname.DiffuseTexture:
                        {
                            extension.DiffuseTexture = new Runtime.Texture();
                            extension.DiffuseTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.SpecularGlossinessTexture:
                        {
                            extension.SpecularGlossinessTexture = new Runtime.Texture();
                            extension.SpecularGlossinessTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.OcclusionTexture:
                        {
                            material.OcclusionTexture = new Runtime.Texture();
                            material.OcclusionTexture.Source = property.value;
                            break;
                        }
                    case Propertyname.VertexColor_Vector3_Float:
                        {
                            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC3;
                            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Colors = property.value;
                            break;
                        }
                }
            }
            wrapper.Scenes[0].Meshes[0].MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
