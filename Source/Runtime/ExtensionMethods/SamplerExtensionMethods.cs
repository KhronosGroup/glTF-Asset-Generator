namespace AssetGenerator.Runtime.ExtensionMethods
{
    internal static class SamplerExtensionMethods
    {
        /// <summary>
        /// Function which determines if two Sampler objects have equal values
        /// </summary>
        public static bool SamplersEqual(this glTFLoader.Schema.Sampler s1, glTFLoader.Schema.Sampler s2)
        {
            return ((s1.MagFilter == s2.MagFilter) && (s1.MinFilter == s2.MinFilter) && (s1.Name == s2.Name) && (s1.WrapS == s2.WrapS) && (s1.WrapT == s2.WrapT));

        }
    }
}
