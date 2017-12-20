using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_Doublesided : ModelGroup
    { 
        public Material_Doublesided()
        {
            modelGroupName = ModelGroupName.Material_Doublesided;
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
                new Property(Propertyname.DoubleSided, true),
                new Property(Propertyname.VertexNormal, planeNormals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            var doubleSided = properties.Find(e => e.name == Propertyname.DoubleSided);
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var normTex = properties.Find(e => e.name == Propertyname.NormalTexture);
            var colorTex = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                normal,
                normTex,
                colorTex
            });
            
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                colorTex
            });
            specialCombos.Add(new List<Property>()
            {
                doubleSided,
                normal,
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
            removeCombos.Add(new List<Property>()
            {
                 colorTex
            });
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            combos.Sort(delegate (List<Property> x, List<Property> y)
            {
                var xNormTex = x.Find(e => e.name == Propertyname.NormalTexture);
                var yNormTex = y.Find(e => e.name == Propertyname.NormalTexture);
                var xColorTex = x.Find(e => e.name == Propertyname.BaseColorTexture);
                var yColorTex = y.Find(e => e.name == Propertyname.BaseColorTexture);

                if (x.Count == 0) return -1; // Empty Set
                else if (y.Count == 0) return 1; // Empty Set
                else if (x.Count == 7) return -1; // Contains all properties
                else if (y.Count == 7) return 1; // Contains all properties
                else if (xNormTex == null && yNormTex != null) return 1;
                else if (xNormTex != null && yNormTex == null) return -1;
                else if (xNormTex == null && yNormTex == null)
                {
                    if (xColorTex == null && yColorTex != null) return 1;
                    else if (xColorTex != null && yColorTex == null) return -1;
                    else return 0;
                }
                else return 0;
            });

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (Property property in combo)
            {
                if (property.name == Propertyname.DoubleSided)
                {
                    material.DoubleSided = property.value;
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
                else if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
