using System.Collections.Generic;
using System.Numerics;
using System;

namespace AssetGenerator
{
    internal class Material : ModelGroup
    {
        public Material(List<string> imageList)
        {
            modelGroupName = ModelGroupName.Material;

            Runtime.Image emissiveImage = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Emissive_Plane"))
            };
            Runtime.Image normalImage = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Normal_Plane"))
            };
            Runtime.Image occlusionImage = new Runtime.Image
            {
                Uri = imageList.Find(e => e.Contains("Occlusion_Plane"))
            };
            usedTextures.Add(emissiveImage);
            usedTextures.Add(normalImage);
            usedTextures.Add(occlusionImage);

            List<Vector3> planeNormalsValue = new List<Vector3>()
            {
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f),
                new Vector3( 0.0f, 0.0f, 1.0f)
            };
            var baseColorFactorValue = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            var emissiveFactorValue = new Vector3(1.0f, 1.0f, 1.0f);

            // Declares the properties and their values. Order doesn't matter here.
            var normalTexture = new Property(
                "Normal Texture",
                normalImage);
            var scale = new Property(
                "Scale",
                "10.0");
            var occlusionTexture = new Property(
                "Occlusion Texture",
                occlusionImage);
            var strength = new Property(
                "Strength",
                "0.5");
            var emissiveTexture = new Property(
                "Emissive Texture",
                emissiveImage);
            var emissiveFactor = new Property(
                "Emissive Factor",
                emissiveFactorValue);

            // Used, but not listed in the readme
            var normals = new Property(
                "Normals",
                planeNormalsValue);

            // Required properties
            var metallicFactor = new Property(
                "Metallic Factor",
                "0.0");
            var baseColorFactor = new Property(
                "Base Color Factor",
                baseColorFactorValue);

            // Main object creation loop. This is invoked later by CreateModel() in the Model class.
            Func<List<Property>, Runtime.GLTF> getModel = (List<Property> setProperties) =>
            {
                // Creates a fresh gltf model 
                Runtime.GLTF gltf = Common.SinglePlane();
                // Creates the components of the gltf that will be modified
                var meshPrimitive = gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0];
                var material = new Runtime.Material();
                var metallicRoughness = new Runtime.PbrMetallicRoughness();

                // Creates the actions that will modify the gltf for each property
                metallicFactor.value = (() => MetallicFactor(metallicRoughness, 0.0f));
                baseColorFactor.value = (() => BaseColorFactor(metallicRoughness, baseColorFactorValue));
                normalTexture.value = (() => NormalTexture(material, normalImage));
                scale.value = (() => Scale(material, 10.0f));
                occlusionTexture.value = (() => OcclusionTexture(material, occlusionImage));
                strength.value = (() => Strength(material, 0.5f));
                emissiveTexture.value = (() => EmissiveTexture(material, emissiveImage));
                emissiveFactor.value = (() => EmissiveFactor(material, emissiveFactorValue));
                normals.value = (() => Normals(meshPrimitive, planeNormalsValue));

                // Actions the required properties on every model.
                // The type of the result is checked, and then added to the correct part of the gltf.
                // This is to avoid issues like having an empty material.
                foreach (var property in requiredProperty)
                {
                    MergeGltfWithChanges(gltf, property.value());
                }

                // Actions desired properties on the model
                foreach (var property in setProperties)
                {
                    MergeGltfWithChanges(gltf, property.value());
                }

                return gltf;
            };

            // Declares the list of required properties. The order here determins the column order. Items on this list are listed at the top of the readme.
            requiredProperty = new List<Property>
            {
                metallicFactor,
                baseColorFactor
            };

            // Declares the list of properties. The order here determins the column order. Leave items off of this list to have them not show up in the readme.
            properties = new List<Property>
            {
                normalTexture,
                scale,
                occlusionTexture,
                strength,
                emissiveTexture,
                emissiveFactor
            };

            // Declares the combos. The order here determines the number for the model (assending order) and the order that the properties are applied to a model.
            models = new List<Model>
            {
                new Model
                (
                    new List<Property>
                    {

                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        normalTexture,
                        normals
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        occlusionTexture
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        emissiveFactor
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        normalTexture,
                        normals,
                        scale
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        occlusionTexture,
                        strength
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        emissiveTexture,
                        emissiveFactor
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),

                new Model
                (
                    new List<Property>
                    {
                        normalTexture,
                        normals,
                        scale,
                        occlusionTexture,
                        strength,
                        emissiveTexture,
                        emissiveFactor
                    },
                    (usedProperties) =>
                    {
                        return getModel(usedProperties);
                    }
                ),
            };
        }

        /// <summary>
        /// Takes a gltf and an object containing changes we want to apply.
        /// The type of the object is determined, and it is then added to the gltf at the correct position.
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="result"></param>
        internal void MergeGltfWithChanges(Runtime.GLTF gltf, object result)
        {
            var gltfPosition = gltf.Scenes[0].Nodes[0].Mesh.MeshPrimitives[0];
            if (result.GetType().Equals(typeof(Runtime.MeshPrimitive)))
            {
                gltfPosition = (Runtime.MeshPrimitive)result;
            }
            else if (result.GetType().Equals(typeof(Runtime.Material)))
            {
                if (gltfPosition.Material == null)
                {
                    gltfPosition.Material = new Runtime.Material();
                }
                gltfPosition.Material = (Runtime.Material)result;
            }
            else if (result.GetType().Equals(typeof(Runtime.PbrMetallicRoughness)))
            {
                if (gltfPosition.Material == null)
                {
                    gltfPosition.Material = new Runtime.Material();
                }
                if (gltfPosition.Material.MetallicRoughnessMaterial == null)
                {
                    gltfPosition.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                }
                gltfPosition.Material.MetallicRoughnessMaterial = (Runtime.PbrMetallicRoughness)result;
            }
        }

        internal Runtime.PbrMetallicRoughness MetallicFactor(Runtime.PbrMetallicRoughness metallicRoughness, float value)
        {
            metallicRoughness.MetallicFactor = value;
            return metallicRoughness;
        }
        internal Runtime.PbrMetallicRoughness BaseColorFactor(Runtime.PbrMetallicRoughness metallicRoughness, Vector4 value)
        {
            metallicRoughness.BaseColorFactor = value;
            return metallicRoughness;
        }
        internal Runtime.Material NormalTexture(Runtime.Material material, Runtime.Image value)
        {
            material.NormalTexture = new Runtime.Texture
            {
                Source = value
            };
            return material;
        }
        internal Runtime.MeshPrimitive Normals(Runtime.MeshPrimitive meshPrimitive, List<Vector3> value)
        {
            meshPrimitive.Normals = value;
            return meshPrimitive;
        }
        internal Runtime.Material Scale(Runtime.Material material, float value)
        {
            material.NormalScale = value;
            return material;
        }
        internal Runtime.Material OcclusionTexture(Runtime.Material material, Runtime.Image value)
        {
            material.OcclusionTexture = new Runtime.Texture();
            material.OcclusionTexture.Source = value;
            return material;
        }
        internal Runtime.Material Strength(Runtime.Material material, float value)
        {
            material.OcclusionStrength = value;
            return material;
        }
        internal Runtime.Material EmissiveTexture(Runtime.Material material, Runtime.Image value)
        {
            material.EmissiveTexture = new Runtime.Texture();
            material.EmissiveTexture.Source = value;
            return material;
        }
        internal Runtime.Material EmissiveFactor(Runtime.Material material, Vector3 value)
        {
            material.EmissiveFactor = value;
            return material;
        }
    }
}
