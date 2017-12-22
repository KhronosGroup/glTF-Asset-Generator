using System.Collections.Generic;

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
        Material_Alpha,
        Material_Doublesided,
        Material_MetallicRoughness,
        Material_SpecularGlossiness,
        Mesh_Indices,
        Mesh_Primitives,
        Mesh_PrimitivesUV,
        Texture_Sampler,
        Primitive_Attribute,
        Primitive_VertexColor,
    }
}
