If a UV is set on a primitive, then a material and the following attributes are set on that primitive as well.  
Otherwise, no material or attributes are set on that primitive.

~~HeaderTable~~

<br>

Both primitives are using the same index values, but have different vertex positions.  
If indices are not used, then the model are assigned unique vertex positions per the primitive mode.  

Indices for Primitive 0 (Left) | Indices for Primitive 1 (Right)
:---: | :---:
<img src="./Icon_Indices_Primitive0.png" height="144" width="144" align="middle"> | <img src="./Icon_Indices_Primitive1.png" height="144" width="144" align="middle">

<br>

If Vertex UV 1 is set for a primitive, then it is the UV used by the texture on that primitive.  
Otherwise, Vertex UV 0 is used. Both primitives have different UV values.  

Primitive 0 Vertex UV 0 Mapping | Primitive 1 Vertex UV 0 Mapping | Primitive 0 Vertex UV 1 Mapping | Primitive 1 Vertex UV 1 Mapping
:---: | :---: | :---: | :---:
<img src="./Icon_UVSpace2.png" height="144" width="144" align="middle"> | <img src="./Icon_UVSpace3.png" height="144" width="144" align="middle"> | <img src="./Icon_UVSpace4.png" height="144" width="144" align="middle"> | <img src="./Icon_UVSpace5.png" height="144" width="144" align="middle">

<br>

The following table shows the properties that are set for a given model.  
Models 09, 10, and 11 are invalid due to having a UV1 but no UV0.  They can be loaded, but should display a warning. 

~~Table~~ 
