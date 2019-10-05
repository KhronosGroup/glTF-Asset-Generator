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

|   | Mode | Indices Component Type | Left Primitive Indices | Right Primitive Indices |
| :---: | :---: | :---: | :---: | :---: |
| [00](Mesh_PrimitiveRestart_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=0) | Points | Unsigned Byte | [0, 1, 255] | [0, 1, 254] |
| [01](Mesh_PrimitiveRestart_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=1) | Points | Unsigned Short | [0, 1, 65535] | [0, 1, 65534] |
| [02](Mesh_PrimitiveRestart_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=2) | Lines | Unsigned Byte | [0, 1, 1, 255, 255, 0] | [0, 1, 1, 254, 254, 0] |
| [03](Mesh_PrimitiveRestart_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=3) | Lines | Unsigned Short | [0, 1, 1, 65535, 65535, 0] | [0, 1, 1, 65534, 65534, 0] |
| [04](Mesh_PrimitiveRestart_04.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=4) | Line Loop | Unsigned Byte | [0, 1, 255] | [0, 1, 254] |
| [05](Mesh_PrimitiveRestart_05.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=5) | Line Loop | Unsigned Short | [0, 1, 65535] | [0, 1, 65534] |
| [06](Mesh_PrimitiveRestart_06.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=6) | Line Strip | Unsigned Byte | [0, 1, 255, 0] | [0, 1, 254, 0] |
| [07](Mesh_PrimitiveRestart_07.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=7) | Line Strip | Unsigned Short | [0, 1, 65535, 0] | [0, 1, 65534, 0] |
| [08](Mesh_PrimitiveRestart_08.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=8) | Triangles | Unsigned Byte | [0, 1, 255] | [0, 1, 254] |
| [09](Mesh_PrimitiveRestart_09.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=9) | Triangles | Unsigned Short | [0, 1, 65535] | [0, 1, 65534] |
| [10](Mesh_PrimitiveRestart_10.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=10) | Triangle Strip | Unsigned Byte | [0, 1, 255] | [0, 1, 254] |
| [11](Mesh_PrimitiveRestart_11.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=11) | Triangle Strip | Unsigned Short | [0, 1, 65535] | [0, 1, 65534] |
| [12](Mesh_PrimitiveRestart_12.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=12) | Triangle Fan | Unsigned Byte | [0, 1, 255] | [0, 1, 254] |
| [13](Mesh_PrimitiveRestart_13.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Negative&folder=15&model=13) | Triangle Fan | Unsigned Short | [0, 1, 65535] | [0, 1, 65534] |
 
