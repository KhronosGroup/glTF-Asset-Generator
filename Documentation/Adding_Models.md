# Creating a New Model Group
1. [Create a Model Group Readme Template in Markdown.](#create-a-model-group-readme-template-in-markdown)
2. [Create a New Model Group Class.](#create-a-new-model-group-class)
3. Delete undesired models and screenshots from the local [Output](../Output) folder. (Files will be overwritten, but not deleted)
4. Compile and run the build.
5. [Validate Models](#validate-models)
6. [Generate Screenshots.](#generate-screenshots)

## Create a Model Group Readme Template in Markdown
Every model group will generate a readme. The code starts with a template specific to that model group, then inserts a generated table based on the models created.

1. Create a .md file named after the model group (same name as the class) to the [ReadmeTemplates](../Source/ReadmeTemplates) folder.
2. At the top of the template, describe what area is being tested by this group of models. A statement of purpose to quickly explain why these models are useful.
3. Optionally use `~~HeaderTable~~`. This text will be replaced with a table of properties that are present but not being actively tested. Remove this from the template if no `CommonProperties` are set.
4. Add `~~Table~~` to the template. This text will be replaced with the code generated table of test properties and models in the model group. This is required!

### Readme Tips
+ Models will be listed in the readme in the order they are created in code.
+ Property names and values as listed in the readme are formatted by the [ReadmeStringHelper](../Source/ReadmeStringHelper.cs) class, which adds spaces, fixes capitalization, and converts values into strings.
+ Use `":white_check_mark:"` :white_check_mark: to show that something is enabled or has a positive result. Use `":x:"` :x: to show something is disabled or has a negative result.
+ When including images in a table, use a thumbnail that is the size the image is expected to be viewed at, and link it to a full-size image. This avoids issues with images being stretched.

### Readme Template
```
These models are intended to test...  

The following table shows the properties that are set for every model.  

~~HeaderTable~~

The following table shows the properties that are set for a given model.

~~Table~~
```

### Figures
Use images to better explain how a model is setup. This is especially useful for showing model properties that aren't necessarily visible, like UV coordinates or joint skeletons.

1. Create the image
    + Use [draw.io](https://www.draw.io/) to create figures. Export as a PNG, zoom 100%, selection only, Crop.
    + Save the project as an xml for use as a template in the Resources folder.
2. Add the files to the project.
    + Place the PNG in [.\Source\Resources\Figures](../Source/Resources/Figures) and set the file properties to "Copy if newer" (Visual Code?)
    + Place the template in [.\Source\Resources\Templates](../Source/Resources/Templates)
3. Declare the images as being used in a model group.
    + At the top of the model group class that will be using this image, add `UseFigure(imageList, "IMAGEFILENAME");`
4. Insert the image into the readme template.
    + This can be done with either markdown or HTML formatting.  
`![alt-text](Figures/IMAGEFILENAME.png)`  
`<img src="Figures/IMAGEFILENAME.png">`

Be careful of adding images that are too large to a model group's readme, as this can cause weird spacing issues with markdown tables.
+ Clamp the size of the image to keep the image from changing cell widths and heights too much.  
`<img src="Figures/BigImage.png" width="144" height="144" align="middle">`

## Create a New Model Group Class
1. Create a copy of the [ModelGroup_Template](../Source/Resources/Templates/ModelGroup_Template.cs) and place it under the [ModelGroups](../Source/ModelGroups) folder.
2. Name the Class and .cs file as appropriate for what is being tested. For related model groups, use a CATEGORY_NAME format.
3. Add the name of the model group to the `ModelGroupId` enum in [ModelGroup](../Source/ModelGroup.cs) (in alphabetical order). Then in the new class set the `Id` as that enum.
4. Call the class from [Program](../Source/Program.cs). This is done by creating an instance of the class to add to the `allModelGroups` list (in alphabetical order).
5. Add code to the new model group. Add new values to the `PropertyName` enum in `Property.cs` as needed.

### Structure of a Model Group
Declare the following values at the beginning of the constructor
+ Add the figures being used to the `UseFigure` list (See [Figures](#figures) above)
+ Declare the textures being used and add them to the `UseTexture` list.  
`var TextureImage = UseTexture(imageList, "FILENAME")`
+ Declare camera position(s) if a custom one will be used.
+ Declare property values that will be used in more than one model. Add properties used by every model to the `CommonProperties` list.
+ To not generate sample images for the model group, set `NoSampleImages` to true here. This needs to be done if the models are not expected to load successfully.

Assemble the model in the `CreateModel` function.
+ The properties that will be passed to `setProperties` are declared in this function's properties and should be changed to fit the needs of the specific model group.
+ Create the base model (Model with no properties set that are the intended test target). Typically this involves generating the mesh primitive by calling `MeshPrimitive.CreateSinglePlane()` . Avoid creating new base models where possible.
+ Create new objects that make up the parts of the model that will have values set.
+ Call `setProperties`, which will set the desired test values on the component model objects.
+ Assemble and return the model, creating a new `Model` object using the component model objects.

Add helper functions that reduce duplicate code below `CreateModel`.
+ Generally, this is where the properties list is populated, which is used in creating the model group readme (See [Properties](#properties) below)
+ These functions should be specific to the one model group.

Create the anonymous methods
Inside of the code block `this.Models = new List<Model>` is where the values for each specific model will be set.  
If no values are set on a model, leave a comment to show that was intentional.
```C#
CreateModel((properties, meshPrimitive) => {
	// There are no properties set on this model.
}),
```
Otherwise, modify the component model objects that were passed in with the desired test values.  
Be careful to only modify the objects and not replace them! If the object is set equal to a new object, then the ref will point at a new object instead of updating the desired object. Instead only modify values of the object, or work with lists of objects.

At the bottom of the model group `GenerateUsedPropertiesList()` is called. This is used in creating the model group readme and won't need to be modified.

### Typical Model Group
+ The first model has no properties set and is intended to be used as a control during testing.
+ The last model has all properties set (as possible) and is intended as a quick test of having combinations of properties set.
+ Other models are either individual properties being set, or combinations of interest (ones that are likely to interact and cause issues).

## Generate Screenshots
1. Download the [ScreenshotGenerator](https://github.com/stevk/screenshotGenerator)
    + Follow the directions in that repro's readme on how to build the generator.
    + Place the folder containing the Screenshot Generator inside of the local glTF-Asset-Generator directory `.\glTF-Asset-Generator\screenshotGenerator\`
2. Run the `Generate Screenshots` VS Code launch configuration, or run the script directly from the [Tools](../Tools) folder with the command `npm run generateScreenshots`

Screenshots are generated in a step separately from running the glTF Asset Generator, which also includes the moving of textures and figures into the output folders.  
This is done to speed up debugging. The creation of screenshots is a time intensive process and often the screenshots are not needed until the majority of debugging has been completed.

## Validate Models
Run the `Validate Models` VS Code launch configuration in order to use the [glTF-Validator](https://github.com/KhronosGroup/glTF-Validator) to validate the generated models. The results are saved under in the  [ValidatorResults folder](../ValidatorResults).  
This script can also be run directly from the [Tools](../Tools) folder with the command `npm run validate`.  
New and modified models are expected to have been validated before being checked in.

## Properties
Properties are attributes that can be set on a model. For example, Doublesided is a property and it can have a value of true or false.

For each tested property that is set on a model, a [Property](../Source/Property.cs) object needs to be created.
```C#
properties.Add(new Property(PropertyName.PROPERTYNAME, PROPERTYVALUE));
```
The enum will be the name of a column on the readme. The value will be displayed in that column.  
Readme columns are ordered based on the int value for `PropertyName` enums in the [Property](../Source/Property.cs) class.  
If having a property name doesn't make sense, use `Description` as the enum and use an explanatory string as the value.

## Runtime Layer
The [Runtime layer](../Source/Runtime) is a group of classes that are used to represent the glTF model as an object. It also has the functions for converting that object into a glTF file.
+ It is often easier to modify the individual classes separately, and then combine them together into a Runtime.GLTF object only after all of the other changes have been made.
+ When manipulating lists, create a separate list variable and assign to the enumerable property at the end. This is to avoid calls like `MeshPrimitives.ElementAt(2)` in favor of `MeshPrimitives[2]`.
