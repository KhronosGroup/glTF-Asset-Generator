### THESE MODELS ASSUME THAT THE HIGHEST GLTF VERSION THE CLIENT SUPPORTS IS 2.0  
If the client supports a higher version than 2.0, then the results **WILL NOT** match this table.  

- Model 01 has a 'light' object at the root level that isn't in glTF 2.0, so that
object should be ignored by the client when the model is loaded.  

- Model 02 has a 'light' property that isn't in glTF 2.0 added to the 2.0 Node object, so
only the 'light' property should be ignored when the model is loaded.  

- Model 03 has a AlphaMode enum with a value unknown to glTF 2.0, and the property 'AlphaMode2'.
If the client is a lower version that does not support 'AlphaMode2', then it should ignore the
new enum and, instead use the normal 'AlphaMode' value as a fallback.  

- Model 04 should fail to load on a client with a max version of glTF 2.0, due to requiring version 2.1  

- Model 05 requires an extension to load. This model should fail to load, since no client will have that extension.  

~~Table~~ 
