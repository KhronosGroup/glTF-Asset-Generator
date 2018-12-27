# Building the glTF Asset Generator

## Setting Up The glTF Asset Generator In Visual Studio Code (VS Code)
1. Use `git clone https://github.com/KhronosGroup/glTF-Asset-Generator.git` to download the repro.
2. [Install and launch VS Code.](https://code.visualstudio.com/Download)
3. Open the local copy of the repro in VS Code.
  1. Select `Explorer` from the sidebar.
  2. Select `Open Folder`.
  3. Select the location of the `glTF-Asset-Generator` folder.
4. Install the extension `C# for VS Code (powered by OmniSharp)`. There will likely be a popup prompt to take this action.
  + After installation, either relaunch VS Code or reload the extension. 
5. Press F5 or select `Start Debugging` from the debug menu.

## Setting Up Model Screenshots And Other Images
Screenshots are generated in a step seperatly from running the glTF Asset Generator, which also includes the moving of textures and figures into the output folders.
This is done to speed up debugging. The creation of screenshots is a lengthy process and often the screenshots are not needed until the majority of debugging has been completed.
1. Download the [ScreenshotGenerator](https://github.com/kcoley/screenshotGenerator)
  + Please follow the directions in that repro's readme on how to build the generator.
  + Place the folder containing the Screenshot Generator inside of the glTF-Asset-Generator directory `.\glTF-Asset-Generator\ScreenshotGenerator\`
2. Run the PowerShell script [SampleImageHelper.ps1](SampleImageHelper.ps1)

## Common Errors
+ The error message `Configured debug type 'coreclr' is encountered when debugging in VS Code 
  + This is caused by debugging without `C# for VS Code (powered by OmniSharp)` being correctly installed and loaded.
  + Be sure to reload the extension after installing it, or relaunching VS Code.
+ Error message encountered while debugging `Unhandled Exception: System.UnauthorizedAccessException: Access to the path is denied.`
  + A file could not be overwritten. 
  + Check that the file isn't open in another program and that it isn't set to readonly.
+ Error message encountered while debugging `An unhandled exception of type 'System.IO.IOException' occurred in System.IO.FileSystem.dll: 'The requested operation cannot be performed on a file with a user-mapped section open'`
    + Encountered when running the debugger again too quickly after having just run it.
    + Wait a few seconds longer between debugging sessions to allow the OS to release its locks on the files. 
