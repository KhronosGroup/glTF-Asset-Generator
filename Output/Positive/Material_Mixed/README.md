These models are intended to test the effects of flagging specular glossiness as `ExtensionUsed`, but not applying it to a mesh.  
The model is made up of two triangle meshes positioned next to each other.

Primitive 0 Vertex UV Mapping | Primitive 1 Vertex UV Mapping
:---: | :---:
<img src="Figures/UVSpace2.png" height="144" width="144" align="middle"> | <img src="Figures/UVSpace3.png" height="144" width="144" align="middle"> 

The following table shows the properties that are set for every model. The metallic roughness base color texture acts as a fallback when the specular glossiness extension is not supported in the renderer.  

| Property | **Values** |
| :---: | :---: |
| Extension Used | Specular Glossiness |
| Base Color Texture | [<img src="Figures/Thumbnails/BaseColor_X.png" align="middle">](Textures/BaseColor_X.png) |

 
The following table shows the properties that are set for a given model.  

|   | Sample Image | Specular Glossiness On Material 0 | Specular Glossiness On Material 1 |
| :---: | :---: | :---: | :---: |
| [00](Material_Mixed_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=11&model=0) | [<img src="Figures/Thumbnails/Material_Mixed_00.png" align="middle">](Figures/SampleImages/Material_Mixed_00.png) | :white_check_mark: | :white_check_mark: |
| [01](Material_Mixed_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=11&model=1) | [<img src="Figures/Thumbnails/Material_Mixed_01.png" align="middle">](Figures/SampleImages/Material_Mixed_01.png) | :x: | :x: |
| [02](Material_Mixed_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=11&model=2) | [<img src="Figures/Thumbnails/Material_Mixed_02.png" align="middle">](Figures/SampleImages/Material_Mixed_02.png) | :white_check_mark: | :x: |
 
