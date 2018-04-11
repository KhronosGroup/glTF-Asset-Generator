using System.Collections.Generic;
using System.Numerics;

namespace AssetGenerator
{
    class ModelGroup
    {
        public ModelGroupName modelGroupName;
        public List<Property> properties;
        public List<Property> requiredProperty;
        public List<Property> specialProperties;
        public List<List<Property>> combos;
        public List<Runtime.Image> usedTextures;
        public List<Runtime.Image> usedFigures;
        public int id;
        public bool noSampleImages;

        public ModelGroup(InitialiseModelGroup initialValues)
        {
            modelGroupName = initialValues.modelGroupName;
            properties = initialValues.properties;
            requiredProperty = initialValues.requiredProperty;
            specialProperties = initialValues.specialProperties;
            combos = initialValues.combos;
            usedTextures = initialValues.usedTextures;
            usedFigures = initialValues.usedFigures;
            id = initialValues.id;
            noSampleImages = initialValues.noSampleImages;
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

        internal Runtime.GLTF ApplyModelProperty(Runtime.GLTF wrapper, List<Property> combo)
        {

            return wrapper;
        }

        static internal Runtime.GLTF InitializeMaterial(Runtime.GLTF wrapper, ValueIndexPositions index)
        {
            if (wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material == null)
            {
                wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material = new Runtime.Material();
            }

            return wrapper;
        }

        static internal Runtime.GLTF InitializeMetallicRoughness(Runtime.GLTF wrapper, ValueIndexPositions index)
        {
            if (wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.MetallicRoughnessMaterial == null)
            {
                wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
            }

            return wrapper;
        }

        static internal Runtime.GLTF MetallicFactor(Runtime.GLTF wrapper, ValueIndexPositions index, float value)
        {
            InitializeMaterial(wrapper, index);
            InitializeMetallicRoughness(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.MetallicRoughnessMaterial.MetallicFactor = value;
            return wrapper;
        }
        static internal Runtime.GLTF BaseColorFactor(Runtime.GLTF wrapper, ValueIndexPositions index, Vector4 value)
        {
            InitializeMaterial(wrapper, index);
            InitializeMetallicRoughness(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.MetallicRoughnessMaterial.BaseColorFactor = value;
            return wrapper;
        }
        static internal Runtime.GLTF NormalTexture(Runtime.GLTF wrapper, ValueIndexPositions index, Runtime.Image value)
        {
            InitializeMaterial(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.NormalTexture = new Runtime.Texture();
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.NormalTexture.Source = value;
            return wrapper;
        }
        static internal Runtime.GLTF Normals(Runtime.GLTF wrapper, ValueIndexPositions index, List<Vector3> value)
        {
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Normals = value;
            return wrapper;
        }
        static internal Runtime.GLTF Scale(Runtime.GLTF wrapper, ValueIndexPositions index, float value)
        {
            InitializeMaterial(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.NormalScale = value;
            return wrapper;
        }
        static internal Runtime.GLTF OcclusionTexture(Runtime.GLTF wrapper, ValueIndexPositions index, Runtime.Image value)
        {
            InitializeMaterial(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.OcclusionTexture = new Runtime.Texture();
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.OcclusionTexture.Source = value;
            return wrapper;
        }
        static internal Runtime.GLTF Strength(Runtime.GLTF wrapper, ValueIndexPositions index, float value)
        {
            InitializeMaterial(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.OcclusionStrength = value;
            return wrapper;
        }
        static internal Runtime.GLTF EmissiveTexture(Runtime.GLTF wrapper, ValueIndexPositions index, Runtime.Image value)
        {
            InitializeMaterial(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.EmissiveTexture = new Runtime.Texture();
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.EmissiveTexture.Source = value;
            return wrapper;
        }
        static internal Runtime.GLTF EmissiveFactor(Runtime.GLTF wrapper, ValueIndexPositions index, Vector3 value)
        {
            InitializeMaterial(wrapper, index);
            wrapper.Scenes[index.scene].Nodes[index.node].Mesh.MeshPrimitives[index.meshPrimitive].Material.EmissiveFactor = value;
            return wrapper;
        }
    }

    public class ValueIndexPositions
    {
        public int scene { get; set; }
        public int node { get; set; }
        public int meshPrimitive { get; set; }

        public ValueIndexPositions(int setScene = 0, int setNode = 0, int setMeshPrimitive = 0)
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
