# Setting Custom Camera Translation Values
The default camera position assumes that the base square plane model is being used. This is insufficient for some other models where the model is either too big or moves outside of the frame. In these cases, set a custom camera position.

The translation of the camera is from the world coordinate 0,0,0 using Z to control the distance from the model and X to control the angle.

Setting a custom camera translation for a model group involves just setting the Camera object when creating a new model, in the `CreateModel` function.
```C#
return new Model
{
    Properties = properties,
    GLTF = CreateGLTF(() => new Runtime.Scene()
    {
        Nodes = nodes
    }),
    Camera = new Manifest.Camera(new Vector3(0.5f, 0.0f, 0.6f))
};
```

Things get more complicated when setting camera translation on a model-by-model basis.
Note that the null set and check are not needed if every model in the model group needs a custom camera translation, but not all the same one.
1. Declare the desired camera translation(s) at the top of the model group's constructor.
```C#
var closeCamera = new Manifest.Camera(new Vector3(0.5f, 0.0f, 0.6f));
```
2. Add `Action<Model> setCamera = null` to `CreateModel()` properties.
3. Add this block of code to `CreateModel()` after the model is created.
```C#
if (setCamera != null)
{
    setCamera(model);
}
```
4. Set the camera translation as a part of the anonymous method that sets the model's properties.
```C#
CreateModel((properties, material) => {
	// Set normal model properties here
	properties.Add(new Property(PropertyName.Description, "Model with a custom camera translation"));
}, (model) => { model.Camera = closeCamera; }),
```

# Flag Model as Not Valid
The 'Model' object has the bool `Valid` to track if the model is a valid glTF model or not. This value is true by default, but can be changed by adding an action. 

1. Add `Action<Model> modifyModel = null` to `CreateModel()` properties.
2. Add this block of code to `CreateModel()` after the model is created.
```C#
modifyModel?.Invoke(model);
```
3. Add the change that will be made post runtime and call it as a part of the anonymous method that sets the model's properties.
```C#
CreateModel((properties, material) => {
	// Set normal model properties here.
	properties.Add(new Property(PropertyName.Description, "Model is not valid."));
}, (model) => { model.Valid = false; }),
```

# Post Runtime Changes
There are some specific types of models that the [Runtime layer](../Source/Runtime) isn't setup to create. For these cases there is an option to make post Runtime tweaks, specifically for cases when it doesn't make sense to make the changes to the Runtime code to do the same thing.

1. Add `Action<glTFLoader.Schema.Gltf> postRuntimeChanges = null` to `CreateModel()` properties.
2. Add this block of code to `CreateModel()` after the model is created.
```C#
model.PostRuntimeChanges = postRuntimeChanges;
```
3. Add the change that will be made post runtime and call it as a part of the anonymous method that sets the model's properties.
```C#
CreateModel((properties, material) => {
	// Set normal model properties here.
	properties.Add(new Property(PropertyName.Description, "Model with post runtime changes."));
}, (gltf) => { gltf.Scenes.First().Nodes = new[] { 0 }; }),
```

# Creating a new base model
New base models are created in order to reduce duplicate code and to help focus model groups on the properties that are specifically being tested.

1. Create an abstract partial class of [ModelGroup](../Source/ModelGroup.cs). Preface the name of the file with `ModelGroup_` to show this relation.
2. Create a static partial class of the lowest level component model object. Typically, this is `MeshPrimitive`. This should not create the entire glTF object if possible.
3. Create a function to create the desired base object. Be sure to name the function something descriptive of the resulting model, along the lines of `CreateCube()`
4. Set the desired values for the base model and return the object.

## Tips When Adding a New Base Model
+ Try to center the model's position on 0,0,0 for consistency and to avoid needing a custom camera translation.
+ When adding a base model keep in mind other users will likely need to look through this code in order to understand the model when debugging issues. Doing things like labeling groups of positions can be a big help!
