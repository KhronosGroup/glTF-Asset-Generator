namespace AssetGenerator.Runtime
{
    internal struct AnimationChannel
    {
        public AnimationSampler Sampler { get; set; }

        public AnimationChannelTarget AnimationTarget { get; set; }
    }
}
