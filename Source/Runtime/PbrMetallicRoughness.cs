using System.Numerics;

namespace AssetGenerator.Runtime
{
    internal class PbrMetallicRoughness
    {
        public TextureInfo BaseColorTexture { get; set; }
        public Vector4? BaseColorFactor { get; set; }
        public TextureInfo MetallicRoughnessTexture { get; set; }
        public float? MetallicFactor { get; set; }
        public float? RoughnessFactor { get; set; }
    }
}
