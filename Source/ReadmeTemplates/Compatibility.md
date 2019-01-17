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

~~Table~~
