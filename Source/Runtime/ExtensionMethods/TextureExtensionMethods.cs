namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class TextureExtensionMethods
    {
        /// <summary>
        /// Function which determines if two Textures objects have equal values
        /// </summary>
        public static bool TexturesEqual(this glTFLoader.Schema.Texture t1, glTFLoader.Schema.Texture t2)
        {
            return ((t1.Name == t2.Name) && (t1.Source == t2.Source) && (t1.Sampler == t2.Sampler));
        }
    }
}
