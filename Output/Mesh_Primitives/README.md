These models are intended to test the various properties that can be applied to a primitive.  

The mesh of the base model is split into two primitives, and then primitive attributes are set on both primitives.  

Primitive 0 Vertex UV Mapping | Primitive 1 Vertex UV Mapping
:---: | :---:
<img src="Figures/UVSpace2.png" height="144" width="144" align="middle"> | <img src="Figures/UVSpace3.png" height="144" width="144" align="middle"> 

<br>

Both primitives are using the same index values, but have different vertex positions.  
They are positioned next to each other so that together they appear to be a square plane.

Indices for Primitive 0 (Left) | Indices for Primitive 1 (Right)
:---: | :---:
<img src="Figures/Indices_Primitive0.png" height="144" width="144" align="middle"> | <img src="Figures/Indices_Primitive1.png" height="144" width="144" align="middle">


<br>

The following table shows the properties that are set for a given model.  

|   | Reference Image | Vertex Normal | Vertex Tangent | Vertex Color | Normal Texture | Base Color Texture |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| [00](Mesh_Primitives_00.gltf)<br>[View](https://sandbox.babylonjs.com/) | <img src="ReferenceImages/Mesh_Primitives_00.png" align="middle"> |   |   |   |   |   |
| [01](Mesh_Primitives_01.gltf)<br>[View](https://sandbox.babylonjs.com/) | <img src="ReferenceImages/Mesh_Primitives_01.png" align="middle"> | :white_check_mark: | :white_check_mark: | :white_check_mark: | <img src="Textures/Normal_Plane.png" height="72" width="72" align="middle"> | <img src="Textures/BaseColor_Plane.png" height="72" width="72" align="middle"> |
| [02](Mesh_Primitives_02.gltf)<br>[View](https://sandbox.babylonjs.com/) | <img src="ReferenceImages/Mesh_Primitives_02.png" align="middle"> | :white_check_mark: |   |   |   | <img src="Textures/BaseColor_Plane.png" height="72" width="72" align="middle"> |
| [03](Mesh_Primitives_03.gltf)<br>[View](https://sandbox.babylonjs.com/) | <img src="ReferenceImages/Mesh_Primitives_03.png" align="middle"> | :white_check_mark: | :white_check_mark: |   | <img src="Textures/Normal_Plane.png" height="72" width="72" align="middle"> | <img src="Textures/BaseColor_Plane.png" height="72" width="72" align="middle"> |
| [04](Mesh_Primitives_04.gltf)<br>[View](https://sandbox.babylonjs.com/) | <img src="ReferenceImages/Mesh_Primitives_04.png" align="middle"> |   |   |   |   | <img src="Textures/BaseColor_Plane.png" height="72" width="72" align="middle"> |
| [05](Mesh_Primitives_05.gltf)<br>[View](https://sandbox.babylonjs.com/) | <img src="ReferenceImages/Mesh_Primitives_05.png" align="middle"> |   |   | :white_check_mark: |   |   |
 
