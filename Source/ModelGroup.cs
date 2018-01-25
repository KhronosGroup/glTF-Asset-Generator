using System.Collections.Generic;
using System.IO;

namespace AssetGenerator
{
    internal class ModelGroup
    {
        public ModelGroupName modelGroupName;
        public List<Property> properties;
        public List<Property> requiredProperty = null;
        public List<Runtime.Image> usedTextures = new List<Runtime.Image>();
        public List<Runtime.Image> usedFigures = new List<Runtime.Image>();
        public List<List<Property>> specialCombos = new List<List<Property>>();
        public List<List<Property>> removeCombos = new List<List<Property>>();
        public List<Property> specialProperties = new List<Property>();
        public bool onlyBinaryProperties = true;
        public bool noPrerequisite = true;
        const string texturePath = "Textures/";
        const string figurePath = "Figures/";
        public string texture_Normal = texturePath + "Texture_normal.png";
        public string texture_Emissive = texturePath + "Texture_emissive.png";
        public string texture_BaseColor = texturePath + "Texture_baseColor.png";
        public string texture_MetallicRoughness = texturePath + "Texture_metallicRoughness.png";
        public string texture_Occlusion = texturePath + "Texture_occlusion.png";
        public string texture_Diffuse = texturePath + "Texture_diffuse.png";
        public string texture_SpecularGlossiness = texturePath + "Texture_specularGlossiness.png";
        public string texture_Error = texturePath + "Texture_X.png";
        public string texture_BaseColor_Nodes = texturePath + "Texture_baseColor_Nodes.png";
        public string texture_Normal_Nodes = texturePath + "Texture_normal_Nodes.png";
        public string texture_MetallicRoughness_Nodes = texturePath + "Texture_metallicRoughness_Nodes.png";
        public string figure_Indices = figurePath + "Figure_Indices.png";
        public string figure_Indices_Primitive0 = figurePath + "Figure_Indices_Primitive0.png";
        public string figure_Indices_Primitive1 = figurePath + "Figure_Indices_Primitive1.png";
        public string figure_UVspace0 = figurePath + "Figure_UVSpace0.png";
        public string figure_UVspace1 = figurePath + "Figure_UVSpace1.png";
        public string figure_UVSpace2 = figurePath + "Figure_UVSpace2.png";
        public string figure_UVSpace3 = figurePath + "Figure_UVSpace3.png";
        public string figure_UVSpace4 = figurePath + "Figure_UVSpace4.png";
        public string figure_UVSpace5 = figurePath + "Figure_UVSpace5.png";

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
        Node_NegativeScale,
        Node_TransformChild,
        Node_TransformParent,
        Texture_Sampler,
        Primitive_Attribute,
        Primitive_VertexColor,
    }
}
