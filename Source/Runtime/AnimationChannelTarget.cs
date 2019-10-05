using static glTFLoader.Schema.AnimationChannelTarget;

namespace AssetGenerator.Runtime
{
    internal enum AnimationChannelTargetPath
    {
        Translation = PathEnum.translation,
        Rotation = PathEnum.rotation,
        Scale = PathEnum.scale,
        Weights = PathEnum.weights
    }

    internal class AnimationChannelTarget
    {
        public Node Node { get; set; }
        public AnimationChannelTargetPath Path { get; set; }
    }
}
