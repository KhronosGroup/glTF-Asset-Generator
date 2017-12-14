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
<<<<<<< HEAD
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
        public string figure_Indices = figurePath + "Figure_Indices.png";
        public string figure_Indices_Primitive0 = figurePath + "Figure_Indices_Primitive0.png";
        public string figure_Indices_Primitive1 = figurePath + "Figure_Indices_Primitive1.png";
        public string figure_UVspace0 = figurePath + "Figure_UVSpace0.png";
        public string figure_UVspace1 = figurePath + "Figure_UVSpace1.png";
        public string figure_UVSpace2 = figurePath + "Figure_UVSpace2.png";
        public string figure_UVSpace3 = figurePath + "Figure_UVSpace3.png";
        public string figure_UVSpace4 = figurePath + "Figure_UVSpace4.png";
        public string figure_UVSpace5 = figurePath + "Figure_UVSpace5.png";
=======
        public const string texture_Normal = "Texture_normal.png";
        public const string texture_Emissive = "Texture_emissive.png";
        public const string texture_BaseColor = "Texture_baseColor.png";
        public const string texture_MetallicRoughness = "Texture_metallicRoughness.png";
        public const string texture_Occlusion = "Texture_occlusion.png";
        public const string texture_Diffuse = "Texture_diffuse.png";
        public const string texture_SpecularGlossiness = "Texture_specularGlossiness.png";
        public const string texture_Error = "X.png";
        public const string icon_UVspace0 = "Icon_UVspace0.png";
        public const string icon_UVspace1 = "Icon_UVspace1.png";
        public const string icon_Indices = "Icon_Indices.png";
        public const string icon_Indices_Primitive0 = "Icon_Indices_Primitive0.png";
        public const string icon_Indices_Primitive1 = "Icon_Indices_Primitive1.png";
        public const string icon_UVSpace2 = "Icon_UVspace2.png";
        public const string icon_UVSpace3 = "Icon_UVspace3.png";
        public const string icon_UVSpace4 = "Icon_UVspace4.png";
        public const string icon_UVSpace5 = "Icon_UVspace5.png";
        public const string icon_Nodes = "Icon_Nodes.png";
>>>>>>> Partial conversion to testing negative scale in its own group and update to remove combo code

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
        Node,
        Node_NegativeScale,
        Texture_Sampler,
        Primitive_Attribute,
        Primitive_VertexColor,
    }
}
