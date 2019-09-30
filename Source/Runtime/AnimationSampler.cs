using static glTFLoader.Schema.AnimationSampler;

namespace AssetGenerator.Runtime
{
    internal enum AnimationSamplerInterpolation
    {
        Linear = InterpolationEnum.LINEAR,
        Step = InterpolationEnum.STEP,
        CubicSpline = InterpolationEnum.CUBICSPLINE,
    }

    internal class AnimationSampler
    {
        public AnimationSamplerInterpolation Interpolation { get; set; }
        public Data<float> Input { get; set; }
        public Data Output { get; set; }
    }
}