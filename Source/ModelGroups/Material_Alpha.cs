using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_Alpha : ModelGroup
    { 
        public Material_Alpha()
        {
            modelGroupName = ModelGroupName.Material_Alpha;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(normalTexture);
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.3f, 0.3f, 0.3f, 0.4f),
                new Vector4( 0.3f, 0.3f, 0.3f, 0.2f),
                new Vector4( 0.3f, 0.3f, 0.3f, 0.8f),
                new Vector4( 0.3f, 0.3f, 0.3f, 0.6f)
            };
            List<Vector3> planeNormals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f),
                new Vector3( 0.0f, 0.0f,1.0f)
            };
            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:2),
                new Property(Propertyname.AlphaMode_Mask, glTFLoader.Schema.Material.AlphaModeEnum.MASK, group:1),
                new Property(Propertyname.AlphaMode_Blend, glTFLoader.Schema.Material.AlphaModeEnum.BLEND, group:1),
                new Property(Propertyname.AlphaCutoff_Low, 0.6f,  group:3),
                new Property(Propertyname.AlphaCutoff_High, 0.8f,  group:3),
                new Property(Propertyname.AlphaCutoff_Equal, 0.7f,  group:3),
                new Property(Propertyname.DoubleSided, true),
                new Property(Propertyname.BaseColorFactor, new Vector4(1.0f, 1.0f, 1.0f, 0.7f)),
                new Property(Propertyname.VertexNormal, planeNormals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors, group:2),
            };
            var mask = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var blend = properties.Find(e => e.name == Propertyname.AlphaMode_Blend);
            var alphaCutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var alphaCutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var alphaCutoffEqual = properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
            var doubleSided = properties.Find(e => e.name == Propertyname.DoubleSided);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffLow,
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffEqual
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffHigh
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffLow,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffEqual,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                alphaCutoffHigh,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                normal,
                tangent,
                normTex
            });
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                alphaCutoffLow,
                mask,
                normal,
                tangent,
                normTex
            });
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                blend,
                normal,
                tangent,
                normTex
            });
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                alphaCutoffLow,
                mask
            });
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                blend
            });
            specialCombos.Add(new List<Property>()
            {
                mask,
                baseColorFactor
            });
            specialCombos.Add(new List<Property>()
            {
                baseColorFactor,
                colorTex,
                blend,
                color
            });
            removeCombos.Add(new List<Property>()
            {
                 alphaCutoffLow
            });
            removeCombos.Add(new List<Property>()
            {
                 alphaCutoffEqual
            });
            removeCombos.Add(new List<Property>()
            {
                 alphaCutoffHigh
            });
            removeCombos.Add(new List<Property>()
            {
                 baseColorFactor
            });
            removeCombos.Add(new List<Property>()
            {
                 color
            });
            removeCombos.Add(new List<Property>()
            {
                 colorTex
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
            var blend = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var mask = properties.Find(e => e.name == Propertyname.AlphaMode_Mask);
            var alphaCutoffLow = properties.Find(e => e.name == Propertyname.AlphaCutoff_Low);
            var alphaCutoffHigh = properties.Find(e => e.name == Propertyname.AlphaCutoff_High);
            var alphaCutoffEqual = properties.Find(e => e.name == Propertyname.AlphaCutoff_Equal);
            var baseColorFactor = properties.Find(e => e.name == Propertyname.BaseColorFactor);
            var baseColorTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);

            // BaseColorTexture is used everywhere except in the empty set
            foreach (var y in combos)
            {
                // Checks if the property is already in that combo, or vertexcolor
                if ((y.Find(e => e.name == baseColorTexture.name)) == null &&
                    (y.Find(e => LogStringHelper.GenerateNameWithSpaces(e.name.ToString()) ==
                    LogStringHelper.GenerateNameWithSpaces(Propertyname.VertexColor_Vector4_Float.ToString()))) == null)
                {
                    // Skip the empty set
                    if (y.Count > 0)
                    {
                        y.Add(baseColorTexture);
                    }
                }
            }

            // Add AlphaMode_Blend/Alphamode_Mask + VertexColor combos to the bottom, so BaseColorTexture isn't split up
            combos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Blend),
                properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float)));
            combos.Add(ComboHelper.CustomComboCreation(
                properties.Find(e => e.name == Propertyname.AlphaMode_Mask),
                properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float)));

            // Add more combos last, so they don't have a base color texture
            combos.Add(new List<Property>()
            {
                blend,
                baseColorFactor
            });
            combos.Add(new List<Property>()
            {
                mask,
                baseColorFactor
            });
            combos.Add(new List<Property>()
            {
                mask,
                alphaCutoffLow,
                baseColorFactor,
            });
            combos.Add(new List<Property>()
            {
                mask,
                alphaCutoffEqual,
                baseColorFactor
            });
            combos.Add(new List<Property>()
            {
                mask,
                alphaCutoffHigh,
                baseColorFactor,
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

            foreach (Property property in combo)
            {
                if (property.propertyGroup == 1) // Alpha Mode
                {
                    material.AlphaMode = property.value;
                }
                else if (property.propertyGroup == 3) // Alpha Cutoff
                {
                    material.AlphaCutoff = property.value;
                }
                else if (property.name == Propertyname.DoubleSided)
                {
                    material.DoubleSided = property.value;
                }
                else if (property.name == Propertyname.BaseColorFactor)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }
                    material.MetallicRoughnessMaterial.BaseColorFactor = property.value;
                }
                else if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Float)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorComponentType = Runtime.MeshPrimitive.ColorComponentTypeEnum.FLOAT;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].ColorType = Runtime.MeshPrimitive.ColorTypeEnum.VEC4;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                }
                else if (property.name == Propertyname.VertexNormal)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = property.value;
                }
                else if (property.name == Propertyname.VertexTangent)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = property.value;
                }
                else if (property.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = property.value;
                    material.NormalTexture.TexCoordIndex = 0;
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
