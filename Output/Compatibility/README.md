Model 00 is a control model with minimal properties set, as usual.  

Model 01 has a `light` object at the root level that isn't in the glTF 2.0 specification,
so the `light` object should be ignored by a 2.0 client.  

Model 02 has a `light` property that isn't in the glTF 2.0 specification added to the `node` object,
so the `light` property should be ignored by a 2.0 client.  

Model 03 has a `alphaMode2` property set to a enum value that isn't in the glTF 2.0 specification.
A 2.0 client should ignore the `alphaMode2` property and instead use the `alphaMode` property.  

Model 04 should fail to load on a 2.0 client, due to the minimum version 2.1 requirement.  

Model 05 should fail to load on all clients, since no clients will support the required extension defined in this model.  


Index | Min Version | Version | Description | Model Should Load
:---: | :---: | :---: | :---: | :---:
[00](./Compatibility_0.gltf) |   | 2.0 |   | :white_check_mark:
[01](./Compatibility_1.gltf) |   | 2.1 | Light object added at root | :white_check_mark:
[02](./Compatibility_2.gltf) |   | 2.1 | Light property added to node object | :white_check_mark:
[03](./Compatibility_3.gltf) |   | 2.1 | Alpha mode updated with a new enum value, and a fallback value | :white_check_mark:
[04](./Compatibility_4.gltf) | 2.1 | 2.1 | Requires a specific version or higher | Only in version 2.1 or higher
[05](./Compatibility_5.gltf) |   | 2.0 | Extension required | :x:

