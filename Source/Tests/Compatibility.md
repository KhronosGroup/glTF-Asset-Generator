The following table shows the properties that are set for every model.  

~~HeaderTable~~

Model 01 has a property at the root level that isn't in glTF 2.0, so that
property should be ignored when the model is loaded.  

Model 02 has a property that isn't in glTF 2.0 added to a 2.0 property, so
only the non-2.0 property should be ignored when the model is loaded.  

Model 03 has a enum with a value unknown to glTF 2.0, and has a new field
which informs clients with the target version to use that new enum value. If
the client is a lower version then it should ignore both the new enum and
extra property, and instead use a fallback value.  

Model 04 should fail to load, due to requiring version 2.1  

Model 05 requires an experimental extension to load. This model should fail to load.  

~~Table~~ 
