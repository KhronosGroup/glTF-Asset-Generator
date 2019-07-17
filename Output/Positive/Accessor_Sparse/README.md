These models are intended to test the use of sparse accessors.  

The "B" mesh is using a sparse accessor that is initialized by the accessor used by the "A" mesh.  

In the following table, green signifies the sparse accessor once it has been initialized.  

|   | Accessor before and after being modified by sparse |
| :---: | :---: |
| Figure 1<br>Input | <img src="Figures/SparseAccessor_Input.png" height="144" align="middle"> |
| Figure 2<br>Output | <img src="Figures/SparseAccessor_Output-Translation.png" height="144" align="middle"> |  

The following table shows the properties that are set for a given model.  

|   | Sample Image | Sparse Accessor | Description |
| :---: | :---: | :---: | :---: |
| [00](Accessor_Sparse_00.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=25&model=0) | [<img src="Figures/Thumbnails/Accessor_Sparse_00.gif" align="middle">](Figures/SampleImages/Accessor_Sparse_00.gif) | Animation Sampler Input | See Figure 1 |
| [01](Accessor_Sparse_01.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=25&model=1) | [<img src="Figures/Thumbnails/Accessor_Sparse_01.gif" align="middle">](Figures/SampleImages/Accessor_Sparse_01.gif) | Animation Sampler Output | See Figure 2 |
| [02](Accessor_Sparse_02.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=25&model=2) | [<img src="Figures/Thumbnails/Accessor_Sparse_02.png" align="middle">](Figures/SampleImages/Accessor_Sparse_02.png) | Positions | Model B has a sparse position accessor which overwrites the values of the top left and bottom right vertexes. |
| [03](Accessor_Sparse_03.gltf)<br>[View](https://bghgary.github.io/glTF-Assets-Viewer/?type=Positive&folder=25&model=3) | [<img src="Figures/Thumbnails/Accessor_Sparse_03.png" align="middle">](Figures/SampleImages/Accessor_Sparse_03.png) | Mesh Primitive Indices | Both models have six vertexes, but only four are used to make the visible mesh. Model B has a sparse indices accessor which replaces indices pointing to two of the vertexes with indices pointing at the previously unused vertexes. |
 
