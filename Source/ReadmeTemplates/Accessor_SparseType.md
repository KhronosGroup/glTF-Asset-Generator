These models are intended to test the various component types that sparse accessors can use.  

The "B" mesh is using a sparse accessor that is initialized by the accessor used by the "A" mesh.
The model that does not reference a buffer view (displays only the "B" mesh) is expected to be initialized from an array of zeros, as per the specification.  

In the following table, green signifies the sparse accessor once it has been initialized.  

|   | Accessor before and after being modified by sparse |
| :---: | :---: |
| Figure 1<br>Input | <img src="Figures/SparseAccessor_Input.png" height="144" align="middle"> |
| Figure 2<br>Output | <img src="Figures/SparseAccessor_Output-Rotation.png" height="144" align="middle"> |
| Figure 3<br>No Buffer View | <img src="Figures/SparseAccessor_NoBufferView.png" height="144" align="middle"> |  

The following table shows the properties that are set for a given model.  

~~Table~~ 
