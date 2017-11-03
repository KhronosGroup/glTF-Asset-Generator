- Model 00 is a control model with minimal properties set, as usual.  

- Model 01 has a 'light' object at the root level that isn't in glTF 2.0, so that
object should be ignored by the client when the model is loaded.  

- Model 02 has a 'light' property that isn't in glTF 2.0 added to the 2.0 Node object, so
only the 'light' property should be ignored when the model is loaded.  

- Model 03 has a AlphaMode enum with a value unknown to glTF 2.0, and a new property 'AlphaMode2'.
If the client is a lower version that does not support 'AlphaMode2', then it should ignore the
new property and instead use the normal 'AlphaMode' value as a fallback.  

- Model 04 should fail to load on a client with a highest supported version of glTF 2.0, due to requiring version 2.1  

- Model 05 requires an extension to load. This model should fail to load, since no client will have that extension.  

~~Table~~
