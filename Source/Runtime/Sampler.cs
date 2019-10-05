using static glTFLoader.Schema.Sampler;

namespace AssetGenerator.Runtime
{
    internal enum SamplerMagFilter
    {
        Nearest = MagFilterEnum.NEAREST,
        Linear = MagFilterEnum.LINEAR,
    }

    internal enum SamplerMinFilter
    {
        Nearest = MinFilterEnum.NEAREST,
        Linear = MinFilterEnum.LINEAR,
        NearestMipmapNearest = MinFilterEnum.NEAREST_MIPMAP_NEAREST,
        LinearMipmapNearest = MinFilterEnum.LINEAR_MIPMAP_NEAREST,
        NearestMipmapLinear = MinFilterEnum.NEAREST_MIPMAP_LINEAR,
        LinearMipmapLinear = MinFilterEnum.LINEAR_MIPMAP_LINEAR,
    }

    internal enum SamplerWrap
    {
        Repeat = WrapSEnum.REPEAT,
        ClampToEdge = WrapSEnum.CLAMP_TO_EDGE,
        MirroredRepeat = WrapSEnum.MIRRORED_REPEAT,
    }

    internal class Sampler
    {
        public string Name { get; set; }
        public SamplerMagFilter? MagFilter { get; set; }
        public SamplerMinFilter? MinFilter { get; set; }
        public SamplerWrap? WrapS { get; set; }
        public SamplerWrap? WrapT { get; set; }
    }
}
