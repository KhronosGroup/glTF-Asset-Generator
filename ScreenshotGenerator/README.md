How to build:
Run these commands:

```
npm install --save babylonjs babylonjs-loaders
npm install --save-dev typescript@2.6.2
npm install --save-dev electron@1.7.10
```

To build the generator:

```
npm run build
```

To execute the generator in interactive mode:

```
npm start
```
Drag or use the arrow keys to reposition the camera

L key - loads a new glTF/glb model
S - takes a screenshot and saves to disk


To run in headless mode:
```
npm start -- headless=true manifest=../path/to/manifest/file
```

The output will be saved to the download folder.