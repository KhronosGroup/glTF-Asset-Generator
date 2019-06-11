namespace AssetGenerator.Runtime
{
    internal class AnimationChannel
    {
        public AnimationSampler Sampler;
        public AnimationChannelTarget Target;
        /// <summary>
        /// Toggles if the animation channel sampler will be checked for instantiation.
        /// This must be set to false for animation channel sampler accessors to be instanced if the samplers are identical.
        /// </summary>
        public bool SamplerInstanced = true;
    }
}
