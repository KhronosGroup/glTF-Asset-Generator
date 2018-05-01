﻿using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace AssetGenerator
{
    internal abstract class ModelGroup
    {
        public abstract ModelGroupName Name { get; }

        public List<Property> CommonProperties = new List<Property>();
        public List<Property> Properties = new List<Property>();
        public List<Model> Models;
        public List<Runtime.Image> UsedTextures = new List<Runtime.Image>();
        public List<Runtime.Image> UsedFigures = new List<Runtime.Image>();
        public int Id = -1;
        public bool NoSampleImages = false;

        protected Runtime.Image GetImage(List<string> imageList, string name)
        {
            var image = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains(name))
            };

            UsedTextures.Add(image);
            return image;
        }

        protected static Runtime.GLTF CreateGLTF(Func<Runtime.Scene> createScene)
        {
            return new Runtime.GLTF
            {
                Asset = new Runtime.Asset
                {
                    Generator = "glTF Asset Generator",
                    Version = "2.0",
                },
                Scenes = new List<Runtime.Scene>
                {
                    createScene(),
                },
            };
        }

        protected static class MeshPrimitive
        {
            public static Runtime.MeshPrimitive CreateSinglePlane()
            {
                return new Runtime.MeshPrimitive
                {
                    Positions = new List<Vector3>()
                    {
                        new Vector3( 0.5f, -0.5f, 0.0f),
                        new Vector3(-0.5f, -0.5f, 0.0f),
                        new Vector3(-0.5f,  0.5f, 0.0f),
                        new Vector3( 0.5f,  0.5f, 0.0f)
                    },
                    TextureCoordSets = new List<List<Vector2>>
                    {
                        new List<Vector2>
                        {
                            new Vector2( 1.0f, 1.0f),
                            new Vector2( 0.0f, 1.0f),
                            new Vector2( 0.0f, 0.0f),
                            new Vector2( 1.0f, 0.0f)
                        },
                    },
                    Indices = new List<int>
                    {
                        1, 0, 3, 1, 3, 2
                    },
            };
            }
        }

        protected void GenerateUsedPropertiesList()
        {
            // Creates a list with each unique property used by the model group.
            foreach (var model in Models)
            {
                Properties = Properties.Union(model.Properties, new PropertyComparer()).ToList();
                //Properties.AddRange(model.Properties);
            }

            // Sort both properties lists
            SortPropertiesList(CommonProperties);
            SortPropertiesList(Properties);
        }

        protected void SortPropertiesList(List<Property> properties)
        {
            // Sorts the list so every readme has the same column order, determined by enum value.
            if (properties.Count > 0)
            {
                properties.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
        }

        public virtual glTFLoader.Schema.Gltf PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            return gltf;
        }
    }

    internal struct Model
    {
        public List<Property> Properties { get; set; }
        public Runtime.GLTF GLTF;
    }

    public enum ModelGroupName
    {
        Undefined,
        Buffer_Interleaved,
        Compatibility,
        Material,
        Material_AlphaMask,
        Material_AlphaBlend,
        Material_DoubleSided,
        Material_MetallicRoughness,
        Material_SpecularGlossiness,
        Material_Mixed,
        Mesh_PrimitiveAttribute,
        Mesh_PrimitiveVertexColor,
        Mesh_PrimitiveMode,
        Mesh_Primitives,
        Mesh_PrimitivesUV,
        Node_NegativeScale,
        Node_Attribute,
        Texture_Sampler,
        Primitive_VertexColor,
    }
}
