using System;
using System.Collections.Generic;
using System.Text;

namespace AssetGenerator.Runtime
{
    internal interface IAnimationSampler
    {
        List<float> InputKeys { get; set; }

        glTFLoader.Schema.AnimationSampler.InterpolationEnum GetInterpolation();

        AnimationSamplerComponentTypeEnum OutputAccessorComponentType { get; set; }

        void WriteOutputData(Data geometryData);

        int GetOutputKeyCount();
        glTFLoader.Schema.Accessor.TypeEnum GetOutputAccessorType();
    }

    internal enum AnimationSamplerComponentTypeEnum { UNSIGNED_INT, FLOAT }
}
