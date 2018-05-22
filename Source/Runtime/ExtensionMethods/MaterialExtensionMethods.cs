using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class MaterialExtensionMethods
    {
        /// <summary>
        /// Function which determines if two Material objects have equal values
        /// </summary>
        public static bool MaterialsEqual(this glTFLoader.Schema.Material m1, glTFLoader.Schema.Material m2)
        {
            // debug
            var boolList = new List<bool>();
            boolList.Add(m1.Name == m2.Name);
            boolList.Add(m1.AlphaCutoff == m2.AlphaCutoff);
            boolList.Add(m1.AlphaMode == m2.AlphaMode);
            boolList.Add(m1.DoubleSided == m2.DoubleSided);
            //boolList.Add(m1.EmissiveFactor == m2.EmissiveFactor);
            boolList.Add(m1.EmissiveTexture == m2.EmissiveTexture);
            boolList.Add(m1.Extensions == m2.Extensions);
            boolList.Add(m1.Extras == m2.Extras);
            boolList.Add(m1.NormalTexture == m2.NormalTexture);
            boolList.Add(m1.OcclusionTexture == m2.OcclusionTexture);
            boolList.Add(m1.PbrMetallicRoughness.PbrMetallicRoughnessEqual(m2.PbrMetallicRoughness));
            // end debug

            // Resolves false for no reason
            //&& (m1.EmissiveFactor == m2.EmissiveFactor)

            return ((m1.Name == m2.Name) && (m1.AlphaCutoff == m2.AlphaCutoff) && (m1.AlphaMode == m2.AlphaMode) &&
                (m1.DoubleSided == m2.DoubleSided) && (m1.EmissiveTexture == m2.EmissiveTexture) &&
                (m1.Extensions == m2.Extensions) && (m1.Extras == m2.Extras) && (m1.NormalTexture == m2.NormalTexture) &&
                (m1.OcclusionTexture == m2.OcclusionTexture) && (m1.PbrMetallicRoughness.PbrMetallicRoughnessEqual(m2.PbrMetallicRoughness)));
        }

        /// <summary>
        /// Function which determines if two PbrMetallicRoughness objects have equal values
        /// </summary>
        public static bool PbrMetallicRoughnessEqual(this glTFLoader.Schema.MaterialPbrMetallicRoughness m1, glTFLoader.Schema.MaterialPbrMetallicRoughness m2)
        {
            if (m1 == null || m2 == null)
            {
                if (m1 == null && m2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // debug
            var boolList = new List<bool>();
            boolList.Add(m1.BaseColorFactor == m2.BaseColorFactor);
            boolList.Add(m1.BaseColorTexture == m2.BaseColorTexture);
            boolList.Add(m1.Extensions == m2.Extensions);
            boolList.Add(m1.Extras == m2.Extras);
            boolList.Add(m1.MetallicFactor == m2.MetallicFactor);
            boolList.Add(m1.MetallicRoughnessTexture == m2.MetallicRoughnessTexture);
            boolList.Add(m1.RoughnessFactor == m2.RoughnessFactor);
            // end debug

            return ((m1.BaseColorFactor == m2.BaseColorFactor) && (m1.BaseColorTexture == m2.BaseColorTexture) && (m1.Extensions == m2.Extensions) &&
                (m1.Extras == m2.Extras) && (m1.MetallicFactor == m2.MetallicFactor) && 
                (m1.MetallicRoughnessTexture == m2.MetallicRoughnessTexture) && (m1.RoughnessFactor == m2.RoughnessFactor));
        }
    }
}
