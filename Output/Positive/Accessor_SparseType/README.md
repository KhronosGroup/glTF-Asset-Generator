These models are intended to test the use of sparse accessors.  

Two animation channels are used, one for each visible mesh. The animation sampler referenced by the channel targeting the mesh positioned to the camera's right uses a sparse accessor. For the models that have a checkmark under the "Buffer View" column below, this sparse accessor is initialized from the accessor targeted by the animation channel applied to the mesh on the camera's left.

|   | Base Accessor | Sparse Accessor | Sparse initialized from base |
| :---: | :---: | :---: | :---: |
| Input | <img src="Figures/SparseAccessor_Input-Base.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_Input-Sparse.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_Input-Modified.png" height="144" width="144" align="middle"> |
| Output - Transform | <img src="Figures/SparseAccessor_OutputTransform-Base.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_OutputTransform-Sparse.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_OutputTransform-Modified.png" height="144" width="144" align="middle"> |
| Output - Rotation | <img src="Figures/SparseAccessor_OutputRotation-Base.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_OutputRotation-Sparse.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_OutputRotation-Modified.png" height="144" width="144" align="middle"> |
| No Buffer View | <img src="Figures/SparseAccessor_NoBufferView-Base.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_NoBufferView-Sparse.png" height="144" width="144" align="middle"> | <img src="Figures/SparseAccessor_NoBufferView-Modified.png" height="144" width="144" align="middle"> |  

The following table shows the properties that are set for a given model.  

|   | Sample Image | Indices Type | Value Type | Sparse Accessor | Buffer View |
| :---: | :---: | :---: | :---: | :---: | :---: |
| [00](Accessor_SparseType_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=0) | [<img src="Figures/Thumbnails/Accessor_SparseType_00.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_00.gif) | Unsigned Int | Float | Input | :white_check_mark: |
| [01](Accessor_SparseType_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=1) | [<img src="Figures/Thumbnails/Accessor_SparseType_01.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_01.gif) | Unsigned Byte | Float | Input | :white_check_mark: |
| [02](Accessor_SparseType_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=2) | [<img src="Figures/Thumbnails/Accessor_SparseType_02.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_02.gif) | Unsigned Short | Float | Input | :white_check_mark: |
| [03](Accessor_SparseType_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=3) | [<img src="Figures/Thumbnails/Accessor_SparseType_03.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_03.gif) | Unsigned Int | Normalized Byte | Output | :white_check_mark: |
| [04](Accessor_SparseType_04.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=4) | [<img src="Figures/Thumbnails/Accessor_SparseType_04.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_04.gif) | Unsigned Int | Normalized Short | Output | :white_check_mark: |
| [05](Accessor_SparseType_05.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=26&model=5) | [<img src="Figures/Thumbnails/Accessor_SparseType_05.gif" align="middle">](Figures/SampleImages/Accessor_SparseType_05.gif) | Unsigned Int | Float | Output |  |
 
