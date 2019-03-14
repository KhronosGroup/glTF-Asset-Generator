These models are intended to test flagging a model as backwards compatible within glTF 2, as well as requiring a specific version or extension.  

Model 00 is a control model with minimal properties set, as usual.  

Model 01 has a `light` object at the root level that isn't in the glTF 2.0 specification,
so the `light` object should be ignored by a 2.0 client.  

Model 02 has a `light` property that isn't in the glTF 2.0 specification added to the `node` object,
so the `light` property should be ignored by a 2.0 client.  

Model 03 has a `alphaMode2` property set to a enum value that isn't in the glTF 2.0 specification.
A 2.0 client should ignore the `alphaMode2` property and instead use the `alphaMode` property.  

Model 04 should fail to load on a 2.0 client, due to the minimum version 2.1 requirement.  

Model 05 should fail to load on all clients, since no clients should support the required extension defined in this model.  

Model 06 uses the Specular-Glossiness extension, and should fallback on Metallic-Roughness if Specular-Glossiness is not available.  

|   | Version | Min Version | Description | Model Should Load |
| :---: | :---: | :---: | :---: | :---: |
| [00](Compatibility_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=0) | 2.0 |   |   | :white_check_mark: |
| [01](Compatibility_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=1) | 2.1 |   | Light object added at root | :white_check_mark: |
| [02](Compatibility_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=2) | 2.1 |   | Light property added to node object | :white_check_mark: |
| [03](Compatibility_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=3) | 2.1 |   | Alpha mode updated with a new enum value, and a fallback value | :white_check_mark: |
| [04](Compatibility_04.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=4) | 2.1 | 2.1 | Requires a specific version or higher | Only in version 2.1 or higher |
| [05](Compatibility_05.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=5) | 2.0 |   | Extension required | :x: |
| [06](Compatibility_06.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=5&model=6) | 2.0 |   | Specular Glossiness extension used but not required | :white_check_mark: |

