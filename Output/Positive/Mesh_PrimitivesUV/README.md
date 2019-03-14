If a UV is set on a primitive, then a material and the following attributes are set on that primitive as well.  
Otherwise, no material or attributes are set on that primitive.

| Property | **Values** |
| :---: | :---: |
| Vertex Normal | :white_check_mark: |
| Vertex Tangent | :white_check_mark: |
| Vertex Color | :white_check_mark: |
| Normal Texture | [<img src="Figures/Thumbnails/Normal_Plane.png" align="middle">](Textures/Normal_Plane.png) |
| Base Color Texture | [<img src="Figures/Thumbnails/BaseColor_Plane.png" align="middle">](Textures/BaseColor_Plane.png) |


<br>

Both primitives are using the same index values, but have different vertex positions.  
If indices are not used, then the model are assigned unique vertex positions per the primitive mode.  

Indices for Primitive 0 (Left) | Indices for Primitive 1 (Right)
:---: | :---:
<img src="Figures/Indices_Primitive0.png" height="144" width="144" align="middle"> | <img src="Figures/Indices_Primitive1.png" height="144" width="144" align="middle">

<br>

The texture applied to a primitive uses Vertex UV 1 if possible. otherwise, Vertex UV 0 is used.  

Primitive 0 Vertex UV 0 Mapping | Primitive 1 Vertex UV 0 Mapping | Primitive 0 Vertex UV 1 Mapping | Primitive 1 Vertex UV 1 Mapping
:---: | :---: | :---: | :---:
<img src="Figures/UVSpace2.png" height="144" width="144" align="middle"> | <img src="Figures/UVSpace3.png" height="144" width="144" align="middle"> | <img src="Figures/UVSpace4.png" height="144" width="144" align="middle"> | <img src="Figures/UVSpace5.png" height="144" width="144" align="middle">

<br>

The following table shows the properties that are set for a given model.  

|   | Sample Image | Primitive 0 Vertex UV 0 | Primitive 1 Vertex UV 0 | Primitive 0 Vertex UV 1 | Primitive 1 Vertex UV 1 |
| :---: | :---: | :---: | :---: | :---: | :---: |
| [00](Mesh_PrimitivesUV_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=0) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_00.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_00.png) |   |   |   |   |
| [01](Mesh_PrimitivesUV_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=1) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_01.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_01.png) | :white_check_mark: |   |   |   |
| [02](Mesh_PrimitivesUV_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=2) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_02.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_02.png) |   | :white_check_mark: |   |   |
| [03](Mesh_PrimitivesUV_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=3) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_03.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_03.png) | :white_check_mark: | :white_check_mark: |   |   |
| [04](Mesh_PrimitivesUV_04.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=4) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_04.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_04.png) | :white_check_mark: |   | :white_check_mark: |   |
| [05](Mesh_PrimitivesUV_05.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=5) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_05.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_05.png) |   | :white_check_mark: |   | :white_check_mark: |
| [06](Mesh_PrimitivesUV_06.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=6) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_06.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_06.png) | :white_check_mark: | :white_check_mark: |   | :white_check_mark: |
| [07](Mesh_PrimitivesUV_07.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=7) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_07.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_07.png) | :white_check_mark: | :white_check_mark: | :white_check_mark: |   |
| [08](Mesh_PrimitivesUV_08.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=18&model=8) | [<img src="Figures/Thumbnails/Mesh_PrimitivesUV_08.png" align="middle">](Figures/SampleImages/Mesh_PrimitivesUV_08.png) | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
 
