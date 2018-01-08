using System.Collections.Generic;
using System.IO;

namespace AssetGenerator
{
    internal class ModelGroup
    {
        public ModelGroupName modelGroupName;
        public List<Property> properties;
        public List<Property> requiredProperty = null;
        public List<Runtime.Image> usedImages = new List<Runtime.Image>();
        public List<List<Property>> specialCombos = new List<List<Property>>();
        public List<List<Property>> removeCombos = new List<List<Property>>();
        public List<Property> specialProperties = new List<Property>();
        public bool onlyBinaryProperties = true;
        public bool noPrerequisite = true;
        const string texturePath = "Textures/";
        public string texture_Normal = texturePath + "Texture_normal.png";
        public string texture_Emissive = texturePath + "Texture_emissive.png";
        public string texture_BaseColor = texturePath + "Texture_baseColor.png";
        public string texture_MetallicRoughness = texturePath + "Texture_metallicRoughness.png";
        public string texture_Occlusion = texturePath + "Texture_occlusion.png";
        public string texture_Diffuse = texturePath + "Texture_diffuse.png";
        public string texture_SpecularGlossiness = texturePath + "Texture_specularGlossiness.png";
        public string texture_Error = texturePath + "X.png";
        public string icon_Indices = texturePath + "Icon_Indices.png";
        public string icon_Indices_Primitive0 = texturePath + "Icon_Indices_Primitive0.png";
        public string icon_Indices_Primitive1 = texturePath + "Icon_Indices_Primitive1.png";
        public string icon_UVspace0 = texturePath + "Icon_UVSpace0.png";
        public string icon_UVspace1 = texturePath + "Icon_UVSpace1.png";
        public string icon_UVSpace2 = texturePath + "Icon_UVSpace2.png";
        public string icon_UVSpace3 = texturePath + "Icon_UVSpace3.png";
        public string icon_UVSpace4 = texturePath + "Icon_UVSpace4.png";
        public string icon_UVSpace5 = texturePath + "Icon_UVSpace5.png";

        public ModelGroup()
        {

        }

        public virtual List<List<Property>> ApplySpecialProperties(ModelGroup modelGroup, List<List<Property>> combos)
        {
            return combos;
        }

        public virtual void PostRuntimeChanges(List<Property> combo, ref glTFLoader.Schema.Gltf gltf)
        {

        }
    }
    public enum ModelGroupName
    {
        Undefined,
        Compatibility,
        Material,
        Material_AlphaMask,
        Material_AlphaBlend,
        Material_Doublesided,
        Material_MetallicRoughness,
        Material_SpecularGlossiness,
        Material_Mixed,
        Mesh_Indices,
        Mesh_Primitives,
        Mesh_PrimitivesUV,
        Texture_Sampler,
        Primitive_Attribute,
        Primitive_VertexColor,
    }
}
