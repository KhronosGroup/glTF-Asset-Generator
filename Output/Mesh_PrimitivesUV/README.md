If a UV is set on a primitive, then a material and the following attributes are set on that primitive as well.  
Otherwise, no material or attributes are set on that primitive.


Property | **Values**
:---: | :---:
<span style="line-height:72px">Vertex Normal</span> | :white_check_mark:
<span style="line-height:72px">Vertex Tangent</span> | :white_check_mark:
<span style="line-height:72px">Vertex Color</span> | :white_check_mark:
<span style="line-height:72px">Normal Texture</span> | <img src="./Texture_normal.png" height="72" width="72" align="middle">
<span style="line-height:72px">Base Color Texture</span> | <img src="./Texture_baseColor.png" height="72" width="72" align="middle">


<br>

Both primitives are using the same index values, but have different vertex positions.  
If indices are not used, then the model are assigned unique vertex positions per the primitive mode.  

Indices for Primitive 0 (Left) | Indices for Primitive 1 (Right)
:---: | :---:
<img src="./Icon_Indices_Primitive0.png" height="144" width="144" align="middle"> | <img src="./Icon_Indices_Primitive1.png" height="144" width="144" align="middle">

<br>

The texture applied to a primitive uses Vertex UV 1 if possible. otherwise, Vertex UV 0 is used.  

Primitive 0 Vertex UV 0 Mapping | Primitive 0 Vertex UV 1 Mapping | Primitive 1 Vertex UV 0 Mapping | Primitive 1 Vertex UV 1 Mapping
:---: | :---: | :---: | :---:
<img src="./Icon_UVspace2.png" height="144" width="144" align="middle"> | <img src="./Icon_UVspace4.png" height="144" width="144" align="middle"> | <img src="./Icon_UVspace3.png" height="144" width="144" align="middle"> | <img src="./Icon_UVspace5.png" height="144" width="144" align="middle">

<br>

The following table shows the properties that are set for a given model.  


Index | Primitive 0 Vertex UV 0 | Primitive 0 Vertex UV 1 | Primitive 1 Vertex UV 0 | Primitive 1 Vertex UV 1
:---: | :---: | :---: | :---: | :---:
<span style="line-height:72px">[00](./Mesh_PrimitivesUV_00.gltf)</span> |   |   |   |  
<span style="line-height:72px">[01](./Mesh_PrimitivesUV_01.gltf)</span> | :white_check_mark: | :white_check_mark: |   |  
<span style="line-height:72px">[02](./Mesh_PrimitivesUV_02.gltf)</span> | :white_check_mark: |   |   |  
<span style="line-height:72px">[03](./Mesh_PrimitivesUV_03.gltf)</span> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark:
<span style="line-height:72px">[04](./Mesh_PrimitivesUV_04.gltf)</span> | :white_check_mark: |   | :white_check_mark: |  
<span style="line-height:72px">[05](./Mesh_PrimitivesUV_05.gltf)</span> | :white_check_mark: |   | :white_check_mark: | :white_check_mark:
<span style="line-height:72px">[06](./Mesh_PrimitivesUV_06.gltf)</span> | :white_check_mark: | :white_check_mark: | :white_check_mark: |  
<span style="line-height:72px">[07](./Mesh_PrimitivesUV_07.gltf)</span> |   |   | :white_check_mark: | :white_check_mark:
<span style="line-height:72px">[08](./Mesh_PrimitivesUV_08.gltf)</span> |   |   | :white_check_mark: |  
 
