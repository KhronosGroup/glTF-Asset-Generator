These models are intended to test the various component types that sparse accessors can use.  

The "B" mesh is using a sparse accessor that is initialized by the accessor used by the "A" mesh.
The model that does not reference a buffer view (displays only the "B" mesh) is expected to be initialized from an array of zeros, as per the specification.  

In the following table, green signifies the sparse accessor once it has been initialized.  

|   | Accessor before and after being modified by sparse |
| :---: | :---: |
| Input | <img src="Figures/SparseAccessor_Input.png" height="144" align="middle"> |
| Output | <img src="Figures/SparseAccessor_Output-Rotation.png" height="144" align="middle"> |
| No Buffer View | <img src="Figures/SparseAccessor_NoBufferView.png" height="144" align="middle"> |  

The following table shows the properties that are set for a given model.  

|   | Sample Image | Indices Type | Value Type | Sparse Accessor | Buffer View |
| :---: | :---: | :---: | :---: | :---: | :---: |
| [00](Accessor_SparseType_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=0) | [<img src="Figures/Thumbnails/Accessor_SparseType_00.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_00.gif) | Unsigned Int | Float | Input | :white_check_mark: |
| [01](Accessor_SparseType_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=1) | [<img src="Figures/Thumbnails/Accessor_SparseType_01.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_01.gif) | Unsigned Byte | Float | Input | :white_check_mark: |
| [02](Accessor_SparseType_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=2) | [<img src="Figures/Thumbnails/Accessor_SparseType_02.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_02.gif) | Unsigned Short | Float | Input | :white_check_mark: |
| [03](Accessor_SparseType_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=3) | [<img src="Figures/Thumbnails/Accessor_SparseType_03.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_03.gif) | Unsigned Int | Byte | Output | :white_check_mark: |
| [04](Accessor_SparseType_04.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=4) | [<img src="Figures/Thumbnails/Accessor_SparseType_04.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_04.gif) | Unsigned Int | Short | Output | :white_check_mark: |
| [05](Accessor_SparseType_05.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=5) | [<img src="Figures/Thumbnails/Accessor_SparseType_05.png" align="middle">](Figures/SampleImages/Accessor_SparseType_05.png) | Unsigned Int | Unsigned Int | Mesh Primitive Indices | :white_check_mark: |
| [06](Accessor_SparseType_06.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=6) | [<img src="Figures/Thumbnails/Accessor_SparseType_06.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_06.gif) | Unsigned Int | Float | Output |  |
 
