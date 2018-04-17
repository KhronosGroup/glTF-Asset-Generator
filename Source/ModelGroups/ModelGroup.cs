using System.Collections.Generic;
using System;

namespace AssetGenerator
{
    internal class ModelGroup
    {
        public ModelGroupName modelGroupName;
        public List<Property> properties;
        public List<Property> requiredProperty;
        public List<Model> models;
        public List<Runtime.Image> usedTextures = new List<Runtime.Image>();
        public List<Runtime.Image> usedFigures = new List<Runtime.Image>();
        public int id = -1;
        public bool noSampleImages = false;

        public virtual glTFLoader.Schema.Gltf PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            return gltf;
        }
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

    internal class Model
    {
        public List<Property> usedProperties { get; set; }
        private Func<List<Property>, Runtime.GLTF> applyProperties;

        public Model (List<Property> propertyList, Func<List<Property>, Runtime.GLTF> applyFunc)
        {
            usedProperties = propertyList;
            applyProperties = applyFunc;
        }

        public Runtime.GLTF CreateModel()
        {
            return applyProperties(usedProperties);
        }
    }
}
