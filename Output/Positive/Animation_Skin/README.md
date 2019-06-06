These models are intended to test using skin joints and weights.  

The skins used are depicted below. The joints are highlighted in blue.  

skinA | skinB | skinC
:---: | :---: | :---:
<img src="Figures/skinA.png" width="150" align="middle"> | <img src="Figures/skinB.png" width="150" align="middle"> | <img src="Figures/skinC.png" width="150" align="middle"> 

skinD | skinE
:---: | :---:
<img src="Figures/skinD.png" width="225" align="middle"> | <img src="Figures/skinE.png" width="225" align="middle">

<!---skinD | skinE | skinF--->
<!---:---: | :---: | :---:--->
<!---<img src="Figures/skinD.png" width="144" align="middle"> | <img src="Figures/skinE.png" width="200" align="middle"> | <img src="Figures/skinF.png" width="135" align="middle">--->

The following table shows the properties that are set for a given model.  

|   | Sample Image | Description |
| :---: | :---: | :---: |
| [00](Animation_Skin_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=0) | [<img src="Figures/Thumbnails/Animation_Skin_00.png" align="middle">](Figures/SampleImages/Animation_Skin_00.png) | `skinA`. |
| [01](Animation_Skin_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=1) | [<img src="Figures/Thumbnails/Animation_Skin_01.gif" align="middle">](Figures/SampleImages/Animation_Skin_01.gif) | `skinA` where `joint1` is animating with a rotation. |
| [02](Animation_Skin_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=2) | [<img src="Figures/Thumbnails/Animation_Skin_02.png" align="middle">](Figures/SampleImages/Animation_Skin_02.png) | `skinA` where the skinned node has a transform and a parent node with a transform. Both transforms should be ignored. |
| [03](Animation_Skin_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=3) | [<img src="Figures/Thumbnails/Animation_Skin_03.png" align="middle">](Figures/SampleImages/Animation_Skin_03.png) | `skinA` without inverse bind matrices. |
| [04](Animation_Skin_04.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=4) | [<img src="Figures/Thumbnails/Animation_Skin_04.gif" align="middle">](Figures/SampleImages/Animation_Skin_04.gif) | `skinA` where `joint1` is animated with a rotation and `joint1` has a triangle mesh attached to it. |
| [05](Animation_Skin_05.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=5) | [<img src="Figures/Thumbnails/Animation_Skin_05.png" align="middle">](Figures/SampleImages/Animation_Skin_05.png) | `skinA` where there are two meshes sharing a single skin. |
| [06](Animation_Skin_06.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=6) | [<img src="Figures/Thumbnails/Animation_Skin_06.png" align="middle">](Figures/SampleImages/Animation_Skin_06.png) | `skinA` where `joint1` is a root node and not a child of `joint0`. |
| [07](Animation_Skin_07.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=7) | [<img src="Figures/Thumbnails/Animation_Skin_07.gif" align="middle">](Figures/SampleImages/Animation_Skin_07.gif) | `skinB` which is made up of two skins. `joint1` is referenced by both skins and is animating with a rotation. |
| [08](Animation_Skin_08.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=8) | [<img src="Figures/Thumbnails/Animation_Skin_08.png" align="middle">](Figures/SampleImages/Animation_Skin_08.png) | `skinC` where all of the joints have a local rotation of -10 degrees, except the root which is rotated -90 degrees. |
| [09](Animation_Skin_09.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=9) | [<img src="Figures/Thumbnails/Animation_Skin_09.gif" align="middle">](Figures/SampleImages/Animation_Skin_09.gif) | `skinD` where each joint is animating with a rotation. There is a transform node in the joint hierarchy that is not a joint. That node has a mesh attached to it in order to show its location. |
| [10](Animation_Skin_10.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=10) | [<img src="Figures/Thumbnails/Animation_Skin_10.png" align="middle">](Figures/SampleImages/Animation_Skin_10.png) | `skinE`. |
| [11](Animation_Skin_11.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=2&model=11) | [<img src="Figures/Thumbnails/Animation_Skin_11.png" align="middle">](Figures/SampleImages/Animation_Skin_11.png) | Two instances of `skinA` sharing a mesh but with separate skins. |
 
