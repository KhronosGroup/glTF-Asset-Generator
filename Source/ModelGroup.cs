using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

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
        //const string texturePath = "Textures/";
        //const string figurePath = "Figures/";

        // Textures
        //public List<string> textures;
        //public string texture_Normal = texturePath + "Normal_Plane.png";
        //public string texture_Emissive = texturePath + "Emissive_Plane.png";
        //public string texture_BaseColor = texturePath + "BaseColor_Plane.png";
        //public string texture_MetallicRoughness = texturePath + "MetallicRoughness_Plane.png";
        //public string texture_Occlusion = texturePath + "Occlusion_Plane.png";
        //public string texture_Diffuse = texturePath + "Diffuse_Plane.png";
        //public string texture_SpecularGlossiness = texturePath + "SpecularGlossiness_Plane.png";
        //public string texture_Error = texturePath + "X_Plane.png";
        //public string texture_BaseColor_Nodes = texturePath + "BaseColor_Nodes.png";
        //public string texture_Normal_Nodes = texturePath + "Normal_Nodes.png";
        //public string texture_MetallicRoughness_Nodes = texturePath + "MetallicRoughness_Nodes.png";
        //public string texture_BaseColor_Grey = texturePath + "BaseColor_Grey.png";

        // Figures
        //public List<string> figures;
        //public string figure_Indices = figurePath + "Indices.png";
        //public string figure_Indices_Primitive0 = figurePath + "Indices_Primitive0.png";
        //public string figure_Indices_Primitive1 = figurePath + "Indices_Primitive1.png";
        //public string figure_UVspace0 = figurePath + "UVSpace0.png";
        //public string figure_UVspace1 = figurePath + "UVSpace1.png";
        //public string figure_UVSpace2 = figurePath + "UVSpace2.png";
        //public string figure_UVSpace3 = figurePath + "UVSpace3.png";
        //public string figure_UVSpace4 = figurePath + "UVSpace4.png";
        //public string figure_UVSpace5 = figurePath + "UVSpace5.png";

        public ModelGroup()
        {

        }

        public ModelGroup(List<string> textures, List<string> figures)
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
        Buffer_Interleaved,
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
        Node_Attribute,
        Texture_Sampler,
        Primitive_Attribute,
        Primitive_VertexColor,
    }
}
