namespace AssetGenerator.Runtime
{
    internal class TextureInfo
    {
        public Texture Texture { get; set; }
        public int? TexCoord { get; set; }
    }

    internal class NormalTextureInfo : TextureInfo
    {
        public float? Scale { get; set; }
    }

    internal class OcclusionTextureInfo : TextureInfo
    {
        public float? Strength { get; set; }
    }
}

