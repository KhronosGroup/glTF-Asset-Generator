﻿using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator.ModelGroups
{
    [ModelGroupAttribute]
    class Mesh_Primitives : ModelGroup
    {
        public Mesh_Primitives()
        {
            modelGroupName = ModelGroupName.Mesh_Primitives;
            onlyBinaryProperties = false;
            noPrerequisite = false;
            Runtime.Image iconIndicesPrimitive0 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive0
            };
            Runtime.Image iconIndicesPrimitive1 = new Runtime.Image
            {
                Uri = icon_Indices_Primitive1
            };
            Runtime.Image baseColorTexture = new Runtime.Image
            {
                Uri = texture_BaseColor
            };
            Runtime.Image iconUVSpace2 = new Runtime.Image
            {
                Uri = icon_UVSpace2
            };
            Runtime.Image iconUVSpace3 = new Runtime.Image
            {
                Uri = icon_UVSpace3
            };
            Runtime.Image iconUVSpace4 = new Runtime.Image
            {
                Uri = icon_UVSpace4
            };
            Runtime.Image iconUVSpace5 = new Runtime.Image
            {
                Uri = icon_UVSpace5
            };
            usedImages.Add(baseColorTexture);
            usedImages.Add(iconIndicesPrimitive0);
            usedImages.Add(iconIndicesPrimitive1);
            usedImages.Add(iconUVSpace2);
            usedImages.Add(iconUVSpace3);
            usedImages.Add(iconUVSpace4);
            usedImages.Add(iconUVSpace5);
            List<Vector3> primitive0Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f)
            };
            List<Vector3> primitive1Positions = new List<Vector3>()
            {
                new Vector3(-0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f,-0.5f, 0.0f),
                new Vector3( 0.5f, 0.5f, 0.0f)
            };
            List<int> primitiveTriangleIndices = new List<int>
            {
                0, 1, 2,
            };
            Runtime.MeshPrimitive primitive0Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive0Positions,
                Indices = primitiveTriangleIndices,
            };
            Runtime.MeshPrimitive primitive1Mesh = new Runtime.MeshPrimitive
            {
                Positions = primitive1Positions,
                Indices = primitiveTriangleIndices,
            };
            List<Vector4> vertexColors = new List<Vector4>()
            {
                new Vector4( 0.0f, 1.0f, 0.0f, 0.2f),
                new Vector4( 1.0f, 0.0f, 0.0f, 0.2f),
                new Vector4( 0.0f, 0.0f, 1.0f, 0.2f)
            };
            List<Vector3> normals = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f)
            };
            List<Vector4> tangents = new List<Vector4>()
            {
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f)
            };
            List<Vector2> textureCoords0Prim0 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( 0.0f, 0.0f)
            };
            List<Vector2> textureCoords0Prim2 = new List<Vector2>()
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 1.0f),
                new Vector2( 1.0f, 0.0f)
            };
            requiredProperty = new List<Property>
            {
                new Property(Propertyname.Primitive0VertexUV0, "UV 0 mapping"),
                new Property(Propertyname.Primitive1VertexUV0, "UV 0 mapping"),
            };
            properties = new List<Property>
            {
                new Property(Propertyname.VertexNormal, normals),
                new Property(Propertyname.VertexTangent, tangents),
                new Property(Propertyname.VertexColor_Vector4_Float, vertexColors),
                new Property(Propertyname.BaseColorTexture, baseColorTexture),
            };
            specialProperties = new List<Property>
            {
                new Property(Propertyname.Primitives_Split1, primitive0Mesh, group: 1),
                new Property(Propertyname.Primitives_Split2, primitive1Mesh, group: 1),
                new Property(Propertyname.Primitive0VertexUV0, textureCoords0Prim0),
                new Property(Propertyname.Primitive1VertexUV0, textureCoords0Prim2),
            };
            var normal = properties.Find(e => e.name == Propertyname.VertexNormal);
            var tangent = properties.Find(e => e.name == Propertyname.VertexTangent);
            var pbrTexture = properties.Find(e => e.name == Propertyname.BaseColorTexture);
            var color = properties.Find(e => e.name == Propertyname.VertexColor_Vector4_Float);
            specialCombos.Add(ComboHelper.CustomComboCreation(
                normal,
                tangent,
                pbrTexture));
            specialCombos.Add(ComboHelper.CustomComboCreation(
                normal,
                pbrTexture));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                normal));
            removeCombos.Add(ComboHelper.CustomComboCreation(
                tangent));
        }

        override public List<List<Property>> ApplySpecialProperties(ModelGroup test, List<List<Property>> combos)
        {
            // Moves the texture combos next to each other
            var baseColorTexture = combos[5];
            combos.RemoveAt(5);
            combos.Insert(4, baseColorTexture);

            return combos;
        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            // Same plane, but split into two triangle primitives
            var primitive1 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split1);
            var primitive2 = specialProperties.Find(e => e.name == Propertyname.Primitives_Split2);
            Runtime.MeshPrimitive prim0 = new Runtime.MeshPrimitive
            {
                Positions = primitive1.value.Positions,
                Indices = primitive1.value.Indices,
            };
            Runtime.MeshPrimitive prim2 = new Runtime.MeshPrimitive
            {
                Positions = primitive2.value.Positions,
                Indices = primitive2.value.Indices,
            };
            wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives = new List<Runtime.MeshPrimitive>
            {
                prim0,
                prim2
            };

            foreach (Property property in combo)
            {
                if (property.name == Propertyname.BaseColorTexture)
                {
                    if (material.MetallicRoughnessMaterial == null)
                    {
                        material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                        material.MetallicRoughnessMaterial.BaseColorTexture = new Runtime.Texture();
                    }
                    material.MetallicRoughnessMaterial.BaseColorTexture.Source = property.value;
                    material.MetallicRoughnessMaterial.BaseColorTexture.TexCoordIndex = 0;
                }

                if (property.name == Propertyname.VertexNormal)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Normals = property.value;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Normals = property.value;
                }
                else if (property.name == Propertyname.VertexTangent)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Tangents = property.value;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Tangents = property.value;
                }
                else if (property.name == Propertyname.VertexColor_Vector4_Float)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Colors = property.value;
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Colors = property.value;
                }
                else if (property.name == Propertyname.Primitive0VertexUV0)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets = new List<List<Vector2>>();
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.Primitive0VertexUV0).value);
                }
                else if (property.name == Propertyname.Primitive1VertexUV0)
                {
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets = new List<List<Vector2>>();
                    wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].TextureCoordSets.Add(
                        specialProperties.Find(e => e.name == Propertyname.Primitive1VertexUV0).value);
                }
            }

            if (material.MetallicRoughnessMaterial != null)
            {
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0].Material = material;
                wrapper.Scenes[0].Nodes[0].Mesh.MeshPrimitives[1].Material = material;
            }

            return wrapper;
        }
    }
}
