# glTF-Asset-Generator

## What is this project?
This is a C# Visual Studio project for generating various glTF 2.0 assets for validating glTF 2.0 importers.

## Why was it created?
In order to help ensure that glTF 2.0 importers meet the [glTF 2.0 Specification](https://github.com/KhronosGroup/glTF/tree/master/specification/2.0) requirements, various types of asset files would need to be created that cover different aspects of the specification .  This project is intended to create these various models.

## What does it currently cover?
So far, this project covers these requirements, with more to come:
- Materials
- Textures
- Images
- Material Alphas
- Primitive Attributes
- Texture Samplers

## What is the feature roadmap?
Please refer to the [Feature Roadmap](https://github.com/bghgary/glTF-Asset-Generator/issues/63)

## How to build the project?
* The following are the project dependencies:
    - git (if you wish to clone this repository)
    - Visual Studio 2017
    - gltf2Loader NuGet package (available within Visual Studio 2017's NuGet Package Manager or [here](https://www.nuget.org/packages/glTF2Loader/))


* Within Visual Studio, open the AssetGenerator.sln solution.  Be sure to import the gltf2Loader NuGet package.  Afterwards, you can Build and Run the solution.  

## Where can I find the generated assets?
* The generated assets wil be located in `glTF-Asset-Generator/Source/bin/Debug`, within their own subdirectories
  - i.e. Materials, PbrMetallicRoughness, Sampler, etc.

* Alternatively, you can locate the pre-generated assets in the [generated-assets](https://github.com/bghgary/glTF-Asset-Generator/tree/generated-assets) branch






