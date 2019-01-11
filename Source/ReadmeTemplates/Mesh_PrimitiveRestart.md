:warning:<b>These are not valid glTF models as primitive restart values are disallowed.</b>  
[See the glTF specification](https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#primitiveindices) for more details.  

These models are intended to test behavior of primitive restart feature of graphics APIs.  

Each asset features a mesh consisting of two primitives with geometry data organized like this (example for TRIANGLES with UNSIGNED_SHORT).  

#### Vertex Buffer
```
v0 (lower-right)
v1 (upper-left)
v2...v65533 (0, 0, 0)
v65534 (lower-left)
v65535 (lower-left, same as previous)
```

#### Index Buffer: 
- Left primitive: `0, 1, 65535`
- Right primitive: `0, 1, 65534`

If the left primitive is rendered, the graphics API doesn't perform primitive restart for a giver primitive type and/or index buffer type.

Note, that some engines may rebuild index buffers into different data types on load which may lead to different results.

The following table shows the properties that are set for a given model.  

~~Table~~ 
