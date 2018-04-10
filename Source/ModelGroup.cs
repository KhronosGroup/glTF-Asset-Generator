using System.Collections.Generic;

namespace AssetGenerator
{
    partial class ModelGroup
    {
        public ModelGroupName modelGroupName;
        public List<Property> properties;
        public List<Property> requiredProperty;
        public List<Property> specialProperties;
        public List<List<Property>> combos = new List<List<Property>>();
        public List<Runtime.Image> usedTextures = new List<Runtime.Image>();
        public List<Runtime.Image> usedFigures = new List<Runtime.Image>();
        public int id = -1;
        public bool noSampleImages = false;

        public ModelGroup()
        {

        }

        public Runtime.GLTF SetModelAttributes(Runtime.GLTF wrapper, Runtime.Material material, List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {
            foreach (var property in combo)
            {
                property.value(wrapper);
            }

            return wrapper;
        }

        public void PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

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

}
