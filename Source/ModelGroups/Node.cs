using System.Collections.Generic;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node : ModelGroup
    {
        public Node()
        {
            modelGroupName = ModelGroupName.Node;
            onlyBinaryProperties = false;
            Runtime.Image normalTexture = new Runtime.Image
            {
                Uri = texture_Normal
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image figureNodes = new Runtime.Image
            {
                Uri = figure_Nodes
            };
            usedTextures.Add(normalTexture);
            usedTextures.Add(baseColorTexture);
            usedFigures.Add(figureNodes);
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.ChildNodes, figureNodes),
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, null),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix, null),
                new Property(Propertyname.Translation, null),
                new Property(Propertyname.Rotation, null),
                new Property(Propertyname.Scale, null),
            };
            var matrix = properties.Find(e => e.name == Propertyname.Matrix);
            var translation = properties.Find(e => e.name == Propertyname.Translation);
            var rotation = properties.Find(e => e.name == Propertyname.Rotation);
            var scale = properties.Find(e => e.name == Propertyname.Scale);
            removeCombos.Add(new List<Property>()
            {
                matrix,
                translation,
                rotation,
                scale,
            });
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.ChildNodes)
                {
                    // ADD CHILD NODES
                }
                else if (req.name == Propertyname.NormalTexture)
                {
                    material.NormalTexture = new Runtime.Texture();
                    material.NormalTexture.Source = req.value;
                }
                else if (req.name == Propertyname.BaseColorTexture)
                {
                    material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = req.value;
                }
                else
                {
                    foreach (var node in wrapper.Scenes[0].Nodes)
                    {
                        if (req.name == Propertyname.VertexNormal)
                        {
                            node.Mesh.MeshPrimitives[0].Normals = req.value;
                        }
                        else if (req.name == Propertyname.VertexTangent)
                        {
                            node.Mesh.MeshPrimitives[0].Tangents = req.value;
                        }
                    }
                }
                
            }

            foreach (Property property in combo)
            {
                foreach (var node in wrapper.Scenes[0].Nodes)
                {
                    if (property.name == Propertyname.Matrix)
                    {
                        node.Matrix = property.value;
                    }
                    else if (property.name == Propertyname.Translation)
                    {
                        node.Translation = property.value;
                    }
                    else if (property.name == Propertyname.Rotation)
                    {
                        node.Rotation = property.value;
                    }
                    else if (property.name == Propertyname.Scale)
                    {
                        node.Scale = property.value;
                    }
                }
            }

            return wrapper;
        }
    }
}
