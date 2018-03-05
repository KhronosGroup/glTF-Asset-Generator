import * as BABYLON from 'babylonjs';
import 'babylonjs-loaders';

const { remote } = require('electron');
const { dialog } = remote;
const w = remote.getCurrentWindow();

const fs = require('fs');
const readline = require('readline');
const stream = require('stream');

const con = remote.getGlobal('console');
const { app } = remote;

const { ipcRenderer } = require('electron');

interface ICamera {
    translation: BABYLON.Vector3,
    rotation: BABYLON.Quaternion
};

interface IGLTFAsset {
    filepath: string,
    camera: ICamera;
    sampleImageName: string;
    sampleThumbnailName: string;
}

export default class Renderer {
    private _canvas: HTMLCanvasElement;
    private _engine: BABYLON.Engine;
    private _scene: BABYLON.Scene;
    private _camera: BABYLON.Camera;
    private disabled: boolean;

    createSceneSync(canvas: HTMLCanvasElement, engine: BABYLON.Engine, filename?: string, camPos?: BABYLON.Vector3, camRotation?: BABYLON.Quaternion) {
        this._canvas = canvas;

        this._engine = engine;

        // This creates a basic Babylon Scene object (non-mesh)
        const scene = new BABYLON.Scene(engine);

        var hdrTexture = BABYLON.CubeTexture.CreateFromPrefilteredData("assets/environment.dds", scene);
        let rotationMatrix = new BABYLON.Matrix();
        const rotationQuaternion = BABYLON.Quaternion.RotationAxis(BABYLON.Vector3.Up(), Math.PI/2);// + Math.PI/4 + Math.PI/16 );
        BABYLON.Matrix.FromQuaternionToRef(rotationQuaternion, rotationMatrix);
        hdrTexture.setReflectionTextureMatrix(rotationMatrix);
        hdrTexture.gammaSpace = false;
        const skybox = scene.createDefaultSkybox(hdrTexture, true, 100, 0.0);
        skybox.rotationQuaternion = rotationQuaternion;
        this._scene = scene;

        camPos = new BABYLON.Vector3(0, 0, 1.3);
        camRotation = (BABYLON.Quaternion.RotationAxis(BABYLON.Vector3.Up(), Math.PI));

        if (camPos && camRotation) {
            const freeCamera = new BABYLON.UniversalCamera("cam", camPos, scene);

            freeCamera.rotationQuaternion = camRotation;
            this._camera = freeCamera;
        }
        else {

            // This creates and positions a free camera (non-mesh)
            this._camera = new BABYLON.ArcRotateCamera("Camera", Math.PI / 2, Math.PI / 2, 1.3, BABYLON.Vector3.Zero(), scene);
        }
        // This attaches the camera to the canvas
        this._camera.attachControl(canvas, true);

        // This creates a light, aiming 0,1,0 - to the sky (non-mesh)
        const light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);

        // Default intensity is 1. Let's dim the light a small amount
        light.intensity = 0.7;

        if (filename) {
            this.addMeshToScene(filename);
        }
        else {
            // Our built-in 'sphere' shape. Params: name, subdivs, size, scene
            const sphere = BABYLON.Mesh.CreateSphere("sphere1", 16, 2, scene);
        const pbrMat = new BABYLON.PBRMaterial("pbrmat", scene);
        pbrMat.metallic = 1.0;
        pbrMat.roughness = 1.0;
        sphere.material = pbrMat;

            // Move the sphere upward 1/2 its height
            sphere.position.y = 1;
            sphere.position.z = -3;

            // Our built-in 'ground' shape. Params: name, width, depth, subdivs, scene
            const ground = BABYLON.Mesh.CreateGround("ground1", 6, 6, 2, scene);
        }
    }
    convertArcRotateCameraToUniversalCamera(arcRotateCam: BABYLON.ArcRotateCamera) {
        const radius = arcRotateCam.radius;
        const alpha = arcRotateCam.alpha;
        const beta = arcRotateCam.beta;

        

    }

    createSceneAsync(canvas: HTMLCanvasElement, engine: BABYLON.Engine, filepath?: string, camPos?: BABYLON.Vector3, camRotation?: BABYLON.Quaternion): Promise<any> {
        const self = this;
        
        return new Promise((resolve, reject) => {
            if (self._scene) {
                self._scene.dispose();
            }
            self._canvas = canvas;
            self._engine = engine;

            // This creates a basic Babylon Scene object (non-mesh)
            const scene = new BABYLON.Scene(engine);
            var hdrTexture = BABYLON.CubeTexture.CreateFromPrefilteredData("assets/environment.dds", scene);
            hdrTexture.gammaSpace = false;
            scene.createDefaultSkybox(hdrTexture, true, 100, 0.0);
            self._scene = scene;

            if (camPos && camRotation) {
                const freeCamera = new BABYLON.UniversalCamera("freeCam", camPos, scene);

                freeCamera.rotationQuaternion = camRotation;
                self._camera = freeCamera;
            }
            else {
                // This creates and positions a free camera (non-mesh)
                self._camera = new BABYLON.ArcRotateCamera("Camera", Math.PI / 2, Math.PI / 2, 1.3, BABYLON.Vector3.Zero(), scene);
            }
            // This attaches the camera to the canvas
            self._camera.attachControl(canvas, true);

            // This creates a light, aiming 0,1,0 - to the sky (non-mesh)
            const light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);

            // Default intensity is 1. Let's dim the light a small amount
            light.intensity = 0.7;

            if (filepath) {
                const fileURL = filepath.replace(/\\/g, '/');
                const rootDirectory = BABYLON.Tools.GetFolderPath(fileURL);
                const sceneFileName = BABYLON.Tools.GetFilename(fileURL);
                const self = this;

                if (fs.existsSync(filepath)) {
                    BABYLON.SceneLoader.ImportMesh("", rootDirectory, sceneFileName, self._scene, function (meshes) {
                        let root: BABYLON.AbstractMesh = new BABYLON.Mesh("root", self._scene);

                        for (const mesh of meshes) {
                            if (!mesh.parent) {
                                mesh.parent = root;
                            }
                        }
                        root.position = new BABYLON.Vector3(0, 0, 0);
                        root.rotation = new BABYLON.Vector3(0, 0, 0);

                        resolve("createSceneAsync: Loaded model: " + fileURL);
                    }, null, function (scene, message, exception) {
                        reject("createSceneAsync: Failed to load model: " + message);
                    });
                }
                else {
                    reject("createSceneAsync: File not found: " + filepath);
                }
            }
            else {
                // Our built-in 'sphere' shape. Params: name, subdivs, size, scene
                const sphere = BABYLON.Mesh.CreateSphere("sphere1", 16, 2, self._scene);

                // Move the sphere upward 1/2 its height
                sphere.position.y = 1;

                // Our built-in 'ground' shape. Params: name, width, depth, subdivs, scene
                const ground = BABYLON.Mesh.CreateGround("ground1", 6, 6, 2, self._scene);

                resolve("createSceneAsync: created default scene");

            }
        });
    }

    createSnapshotAsync(canvas: HTMLCanvasElement, engine: BABYLON.Engine, filename: string): Promise<any> {
        const self = this;

        return new Promise((resolve, reject) => {
            const name = BABYLON.Tools.GetFilename(filename).replace('.glb', '.png').replace('.gltf', '.png') || "test.png";
            con.log("making snapshot for: " + name);

            self._scene.executeWhenReady(() => {
                self._scene.render();

                self.createScreenshot({ width: self._canvas.width, height: self._canvas.height }, function (base64Image: string) {
                    if (base64Image) {
                        base64Image = base64Image.replace(/^data:image\/png;base64,/, "");
                        const filename = app.getPath('downloads') + '/screenshots/' + name;
                        fs.writeFile(filename, base64Image, 'base64', function (err: string) {
                            if (err) {
                                reject("error happened: " + err);
                            }
                            else {
                                resolve("snapshot generated");
                            }
                        });
                    }
                    else {
                        reject("No image data available");
                    }
                });
            });
        });
    }

    async createSnapshotsAsync(glTFAssets: IGLTFAsset[], canvas: HTMLCanvasElement, engine: BABYLON.Engine) {
        let snapshotPromiseChain: Promise<any> = null;
        const folder = fs.mkdir(app.getPath('downloads') + "/screenshots");
        for (let i = 0; i < glTFAssets.length; ++i) {
            try {
                const r = await this.createSceneAsync(canvas, engine, glTFAssets[i].filepath, glTFAssets[i].camera.translation, glTFAssets[i].camera.rotation);
                const result = await this.createSnapshotAsync(canvas, engine, glTFAssets[i].filepath);
            }
            catch (err) {
                con.log("Failed to create snapshot: " + err);
            }
        }
        con.log("Complete!");
        w.close();
    }

    initialize(canvas: HTMLCanvasElement) {
        let runHeadless = false;
        let gltf: string;
        runHeadless = ipcRenderer.sendSync('synchronous-message', 'headless');
        gltf = ipcRenderer.sendSync('synchronous-message', 'gltf');
        let manifest = ipcRenderer.sendSync('synchronous-message', 'manifest');
        const engine = new BABYLON.Engine(canvas, true, { preserveDrawingBuffer: true });
        let glTFAssets: IGLTFAsset[] = null;

        if (manifest) {
            glTFAssets = this.loadManifestFile2(manifest);
        }


        if (runHeadless) {
            this.createSnapshotsAsync(glTFAssets, canvas, engine);
        }
        else {
            this.createSceneSync(canvas, engine);
            canvas.addEventListener("keyup", this.onKeyUp.bind(this));
            engine.runRenderLoop(() => {
                this._scene.render();
            });

            window.addEventListener('resize', function () {
                engine.resize();
            });
        }
    }


    convertToURL(filePath: string): string {
        return filePath.replace(/\\/g, '/');
    }

    /**
     * Get all the gltf assets of a manifest file.
     * @param manifesFile 
     */
    loadManifestFile(manifestJSON: string): string[] {
        const rootDirectory = BABYLON.Tools.GetFolderPath(this.convertToURL(manifestJSON));
        const result: string[] = [];

        const content = fs.readFileSync(manifestJSON);
        // open the manifest file
        const jsonData = JSON.parse(content);

        if ('files' in jsonData) {
            BABYLON.Tools.Log("Files present");
            for (let file of jsonData['files']) {
                result.push(rootDirectory + file);
            }
        }
        else {

            for (let i = 0; i < jsonData.length; ++i) {
                const jsonObj = jsonData[i];
                const directory = jsonObj.folder;
                const files = jsonObj.files;
                const folder = jsonObj.folder;
                const camera = jsonObj.camera;

                for (const file of files) {
                    result.push(rootDirectory + folder + "/" + file);
                }
            }
        }

        // for each gltf asset in the file, add to the list
        return result;
    }

    createGLTFAsset(model: any, rootDirectory: string): IGLTFAsset {
        BABYLON.Tools.Log("Models present");

        const camera: ICamera = {
            translation: BABYLON.Vector3.FromArray([model.camera.translation[0], model.camera.translation[1], -model.camera.translation[2]]),
            rotation: BABYLON.Quaternion.FromArray([-model.camera.rotation[0], -model.camera.rotation[1], model.camera.rotation[2], model.camera.rotation[3]])
        }

        const asset: IGLTFAsset = {
            camera: camera,
            filepath: rootDirectory + model.fileName,
            sampleImageName: model.sampleImageName,
            sampleThumbnailName: model.sampleThumbnailName
        };

        return asset;
    }

    /**
     * Get all the gltf assets of a manifest file.
     * @param manifesFile 
     */
    loadManifestFile2(manifestJSON: string): IGLTFAsset[] {
        const rootDirectory = BABYLON.Tools.GetFolderPath(this.convertToURL(manifestJSON));
        const result: IGLTFAsset[] = [];

        const content = fs.readFileSync(manifestJSON);
        // open the manifest file
        const jsonData = JSON.parse(content);

        if ('models' in jsonData) {
            BABYLON.Tools.Log("Models present");
            con.log("models present");
            for (const model of jsonData['models']) {
                result.push(this.createGLTFAsset(model, rootDirectory));
            }
        }
        else {
            for (let i = 0; i < jsonData.length; ++i) {
                const jsonObj = jsonData[i];
                const folder = jsonObj.folder;

                for (const model of jsonObj.models) {
                    result.push(this.createGLTFAsset(model, rootDirectory + folder + "/"));
                }
            }
        }

        return result;
    }

    addMeshToScene(filepath: string) {
        if (fs.existsSync(filepath)) {
            const fileURL = filepath.replace(/\\/g, '/');
            const rootDirectory = BABYLON.Tools.GetFolderPath(fileURL);
            const sceneFileName = BABYLON.Tools.GetFilename(fileURL);
            const self = this;


            const addMeshPromise = new Promise(function (resolve, reject) {
                BABYLON.SceneLoader.ImportMesh("", rootDirectory, sceneFileName, self._scene, function (meshes) {
                    let root: BABYLON.AbstractMesh = new BABYLON.Mesh("root", self._scene);

                    for (const mesh of meshes) {
                        if (!mesh.parent) {
                            mesh.parent = root;
                        }
                    }
                    root.position = new BABYLON.Vector3(0, 0, 0);
                    root.rotation = new BABYLON.Vector3(0, 0, 0);

                    BABYLON.Tools.Log("Loaded Model");
                    resolve("loaded model: " + fileURL);
                }, null, function (scene, message, exception) {
                    reject("Failed to load model: " + message);
                });
            });
            addMeshPromise.then((result) => {
                con.log(result);
            });
            addMeshPromise.catch((error) => {
                con.error("Failed: " + error);
                throw Error("Failed = " + error);
            });


        }
        else {
            con.log("file not found");
            BABYLON.Tools.Error("File not found: " + filepath);
        }
    }

    onKeyUp(event: KeyboardEvent) {
        const self = this;
        if (event.key == 's' || event.key == 'S') {
            BABYLON.Tools.Log(this._canvas.width.toString());
            this.createScreenshot({ width: this._canvas.width, height: this._canvas.height });
        }
        if (event.key == 'l' || event.key == 'L') {
            BABYLON.Tools.Log('Loading file');
            dialog.showOpenDialog({ properties: ['openFile'] }, function (filePaths) {
                if (filePaths && filePaths.length) {
                    self._scene.dispose();
                    self.createSceneSync(self._canvas, self._engine, filePaths[0]);
                }
            });
        }
        if (event.key == 'i' || event.key == 'I') {
            BABYLON.Tools.Log("logging cam position");
            BABYLON.Tools.Log("position = " + this._camera.position);
            BABYLON.Tools.Log("rotation = " + (this._camera as BABYLON.FreeCamera).rotationQuaternion);
        }
    }

    /**
     * Create an image from the WebGL canvas.
     * @param size - dimensions to use for the image.
     */
    createScreenshot(size: number | { width: number, height: number } | { precision: number }, callback?: (data: string) => void): void {
        BABYLON.Tools.Log('Exporting texture...');
        BABYLON.Tools.CreateScreenshot(this._engine, this._camera, size, callback);
    }
}

const renderer = new Renderer();
let canvas: HTMLCanvasElement = document.getElementById('render-canvas') as HTMLCanvasElement;
renderer.initialize(canvas);