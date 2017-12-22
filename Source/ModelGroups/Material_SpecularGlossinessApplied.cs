using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Linq;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Material_SpecularGlossinessApplied : ModelGroup
    {
        public Material_SpecularGlossinessApplied()
        {
            modelGroupName = ModelGroupName.Material_SpecularGlossinessApplied;
            onlyBinaryProperties = false;
            var baseColorTexture = new Runtime.Image
            {
                Uri = texture_Error
            };
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
                new Property(Propertyname.ExtensionUsed_SpecularGlossiness, "Specular Glossiness", group:1),
                new Property(Propertyname.BaseColorTexture, baseColorTexture)
            };
            properties = new List<Property>
            {
                new Property(Propertyname.SpecularGlossinessAppliedToMesh_Yes, "Yes", group:2),
                new Property(Propertyname.SpecularGlossinessAppliedToMesh_No, "No", group:2),
                new Property(Propertyname.SpecularGlossinessAppliedToMesh_Some, "One of Two Meshes", group:2),
            };
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Removes the empty and full set models. Don't need the automatic ones for this set.
            combos.RemoveAt(0);
            combos.RemoveAt(0);

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

            foreach (Property property in combo)
            {
                switch (property.name)
                {
                    case Propertyname.SpecularGlossinessAppliedToMesh_Yes:
                        // Default. No changes needed
                        break;
                    case Propertyname.SpecularGlossinessAppliedToMesh_No:
                        material.Extensions = null;
                        break;
                    case Propertyname.SpecularGlossinessAppliedToMesh_Some:
                        wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
                        var mesh = DeepCopy.CloneObject(wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0]);
                        wrapper.Scenes[0].Nodes.Add(new Runtime.Node());
                        wrapper.Scenes[0].Nodes[1].Mesh = new Runtime.Mesh(); 
                        wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>();
                        wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives.Add(mesh);
                        wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].Material.Extensions = null;
                        var newPositions = wrapper.Scenes[0].Nodes[1].Mesh.MeshPrimitives[0].Positions;
                        for (int index = 0; index < newPositions.Count; index++)
                        {
                            var newVertex = new Vector3();
                            newVertex.X = newPositions[index].X + 1;
                            newVertex.Y = newPositions[index].Y;
                            newVertex.Z = newPositions[index].Z;
                            newPositions[index] = newVertex;
                        }
                        break;
                    default:
                        throw new InvalidEnumArgumentException(property.name + " not implemented for Specular Glossiness!");
                }
            }
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;

            return wrapper;
        }
    }
}
