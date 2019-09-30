using AssetGenerator.Runtime.Extensions;
using System.Collections.Generic;
using System.Numerics;
using static glTFLoader.Schema.Material;

namespace AssetGenerator.Runtime
{
    internal enum MaterialAlphaMode
    {
        Opaque = AlphaModeEnum.OPAQUE,
        Mask = AlphaModeEnum.MASK,
        Blend = AlphaModeEnum.BLEND,
    }

    internal class Material
    {
        public string Name { get; set; }
        public PbrMetallicRoughness PbrMetallicRoughness { get; set; }
        public NormalTextureInfo NormalTexture { get; set; }
        public OcclusionTextureInfo OcclusionTexture { get; set; }
        public TextureInfo EmissiveTexture { get; set; }
        public Vector3? EmissiveFactor { get; set; }
        public bool? DoubleSided { get; set; }
        public MaterialAlphaMode? AlphaMode { get; set; }
        public float? AlphaCutoff { get; set; }
        public IEnumerable<Extension> Extensions { get; set; }
    }
}
