namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class MaterialExtensionMethods
    {
        /// <summary>
        /// Function which determines if two Materials objects have equal values.
        /// </summary>
        public static bool MaterialsEqual(this glTFLoader.Schema.Material t1, glTFLoader.Schema.Material t2)
        {
            return (
                (t1.Name == t2.Name) &&
                (t1.AlphaCutoff == t2.AlphaCutoff) &&
                (t1.AlphaMode == t2.AlphaMode) &&
                (t1.DoubleSided == t2.DoubleSided) &&
                (t1.EmissiveFactor == t2.EmissiveFactor) &&
                (t1.EmissiveTexture == t2.EmissiveTexture) &&
                (t1.NormalTexture == t2.NormalTexture) &&
                (t1.OcclusionTexture == t2.OcclusionTexture) &&
                (t1.PbrMetallicRoughness == t2.PbrMetallicRoughness)
            );
        }
    }
}
