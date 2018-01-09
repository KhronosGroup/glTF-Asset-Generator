using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Node_Attribute : ModelGroup
    {
        public Node_Attribute()
        {
            modelGroupName = ModelGroupName.Node_Attribute;
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
            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),
            };
            for (int x = 0; x < 23; x++)
            {
                tangents.Add(tangents[0]);
            }
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.ChildNodes, figureNodes),
                new Property(Propertyname.VertexNormal, null),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.NormalTexture, normalTexture),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.Matrix, new Matrix4x4(
                    0.5f, 0.5f, 0.5f, 0.5f,
                    0.5f, 0.5f, 0.5f, 0.5f,
                    0.5f, 0.5f, 0.5f, 0.5f,
                    0.5f, 0.5f, 0.5f, 0.5f)),
                new Property(Propertyname.Translation, new Vector3(3, 3, 3)),
                new Property(Propertyname.Rotation, new Quaternion(0.6f, 0.6f, 0.6f, 0.6f)),
                new Property(Propertyname.Scale, new Vector3(2, 2, 2)),
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
            // Switch from the flat plane to the cube
            wrapper = Common.SingleCube();

            foreach (Property req in requiredProperty)
            {
                if (req.name == Propertyname.ChildNodes)
                {
                    // Makes copies of the existing node
                    for (int x = 0; x < 3; x++)
                    {
                        wrapper.Scenes[0].Nodes.Add(DeepCopy.CloneObject(wrapper.Scenes[0].Nodes[0]));
                    }

                    // Sets the new nodes as children
                    wrapper.Scenes[0].Nodes[0].Children = new List<AssetGenerator.Runtime.Node>();
                    wrapper.Scenes[0].Nodes[0].Children.Add(wrapper.Scenes[0].Nodes[1]);
                    wrapper.Scenes[0].Nodes[0].Children.Add(wrapper.Scenes[0].Nodes[2]);

                    wrapper.Scenes[0].Nodes[2].Children = new List<AssetGenerator.Runtime.Node>();
                    wrapper.Scenes[0].Nodes[2].Children.Add(wrapper.Scenes[0].Nodes[3]);

                    // Changes the new node's positions slightly
                    var originPosition = wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Positions;
                    List<Vector3> Pos1 = new List<Vector3>();
                    List<Vector3> Pos2 = new List<Vector3>();
                    List<Vector3> Pos3 = new List<Vector3>();
                    foreach (var vec in originPosition)
                    {
                        Pos1.Add(new Vector3(vec.X -1.2f, vec.Y - 1.2f, vec.Z));
                        Pos2.Add(new Vector3(vec.X + 1.2f, vec.Y - 1.2f, vec.Z));
                        Pos3.Add(new Vector3(vec.X + 2.4f, vec.Y - 2.4f, vec.Z));
                    }
                    wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].Positions = Pos1;
                    wrapper.Scenes[0].Nodes[2].Mesh.MeshPrimitives[0].Positions = Pos2;
                    wrapper.Scenes[0].Nodes[3].Mesh.MeshPrimitives[0].Positions = Pos3;
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
                            // Already set in Common.cs
                            //node.Mesh.MeshPrimitives[0].Normals = req.value;
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
                    node.Mesh.MeshPrimitives[0].Material = material;
                }
            }

            return wrapper;
        }
    }
}
