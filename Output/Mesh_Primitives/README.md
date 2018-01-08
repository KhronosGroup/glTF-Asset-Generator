These models are intended to test the various properties that can be applied to a primitive.  

The mesh of the base model is split into two primitives, and then primitive attributes are set on both primitives.  

Primitive 0 Vertex UV Mapping | Primitive 1 Vertex UV Mapping
:---: | :---:
<img src="Textures/Icon_UVSpace2.png" height="144" width="144" align="middle"> | <img src="Textures/Icon_UVspace3.png" height="144" width="144" align="middle"> 

<br>

Both primitives are using the same index values, but have different vertex positions.  
They are positioned next to each other so that together they appear to be a square plane.

Indices for Primitive 0 (Left) | Indices for Primitive 1 (Right)
:---: | :---:
<img src="Textures/Icon_Indices_Primitive0.png" height="144" width="144" align="middle"> | <img src="Textures/Icon_Indices_Primitive1.png" height="144" width="144" align="middle">


<br>

The following table shows the properties that are set for a given model.  


Index | Vertex Normal | Vertex Tangent | Vertex Color | Normal Texture | Base Color Texture
:---: | :---: | :---: | :---: | :---: | :---:
[00](./Mesh_Primitives_00.gltf) |   |   |   |   |  
[01](./Mesh_Primitives_01.gltf) | :white_check_mark: | :white_check_mark: | :white_check_mark: | <img src="./Textures/Texture_normal.png" height="72" width="72" align="middle"> | <img src="./Textures/Texture_baseColor.png" height="72" width="72" align="middle">
[02](./Mesh_Primitives_02.gltf) | :white_check_mark: |   |   |   | <img src="./Textures/Texture_baseColor.png" height="72" width="72" align="middle">
[03](./Mesh_Primitives_03.gltf) | :white_check_mark: | :white_check_mark: |   | <img src="./Textures/Texture_normal.png" height="72" width="72" align="middle"> | <img src="./Textures/Texture_baseColor.png" height="72" width="72" align="middle">
[04](./Mesh_Primitives_04.gltf) |   |   |   |   | <img src="./Textures/Texture_baseColor.png" height="72" width="72" align="middle">
[05](./Mesh_Primitives_05.gltf) |   |   | :white_check_mark: |   |  
 
