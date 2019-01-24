const fs = require('fs');
const validator = require('gltf-validator');
const BABYLON = require('babylonjs');

let manifest = "";
const commandLineArgs = parseCommandLineArguments();
if (validateCommandLineArguments(commandLineArgs)) {
    for (let i = 0; i < commandLineArgs.length; ++i) {
        const commandLineArg = commandLineArgs[i];
        switch (commandLineArg.key) {
            case 'manifest': {
                manifest = commandLineArg.value;
                break;
            }
            default: {
                console.warn('Unrecognized command line argument: ' + commandLineArg.key);
            }
        }
    }
}
else {
    System.exit(1);
}

glTFAssets = loadManifestFile(manifest);

for (let i = 0; i < glTFAssets.length; ++i) {

    const asset = fs.readFileSync(glTFAssets[i].filepath);

    

    validator.validateBytes(new Uint8Array(asset), {
        uri: glTFAssets[i].filename,
    }).then((report) => parseValidatorResults(report))
    .catch((error) => console.error('Validation failed: ', error));   
}

function parseValidatorResults(report) {
    if(report.issues.messages.length > 0) {
        console.log("Filename: " + report.uri);
        console.log("Errors: " + report.issues.numErrors);
        console.log("Warnings: " + report.issues.numWarnings);
        for (const message of report.issues.messages) {
            console.log(message);
        }   
    }
}

function parseCommandLineArguments() {
    const args = new Array();
    const commandLineArgs = process.argv;
    const numberOfArgs = commandLineArgs.length;
    args.push({ key: 'executable', value: commandLineArgs[0] });
    args.push({ key: 'script', value: commandLineArgs[1] });
    for (let a = 2; a < numberOfArgs; ++a) {
        let arg = commandLineArgs[a];
        const line = arg.split('=');
        const entry = {
            key: line[0]
        };
        if (line.length === 2) {
            entry.value = line[1];
        }
        args.push(entry);
    }
    return args;
}

function validateCommandLineArguments(commandLineArgs) {
    for (let commandLineArg of commandLineArgs) {
        if (commandLineArg.key === 'manifest') {
            if (fs.existsSync(commandLineArg.value)) {
                return true;
            }
            else {
                console.error(`manifest file ${commandLineArg.value} does not exist!`);
                return false;
            }
        }
    }
}

/**
 * Get all the gltf assets of a manifest file.
 * @param manifesFile
 */
function loadManifestFile(manifestJSON) {
    const rootDirectory = BABYLON.Tools.GetFolderPath(convertToURL(manifestJSON));
    const result = [];
    const content = fs.readFileSync(manifestJSON);
    // open the manifest file
    const jsonData = JSON.parse(content);
    if ('models' in jsonData) {
        for (const model of jsonData['models']) {
            result.push(createGLTFAsset(model, rootDirectory));
        }
    }
    else {
        for (let i = 0; i < jsonData.length; ++i) {
            const jsonObj = jsonData[i];
            const folder = jsonObj.folder;
            for (const model of jsonObj.models) {
                result.push(createGLTFAsset(model, rootDirectory + folder + "/"));
            }
        }
    }
    return result;
}

function convertToURL(filePath) {
    return filePath.replace(/\\/g, '/');
}

function createGLTFAsset(model, rootDirectory) {
    const asset = {
        filedirectory: rootDirectory,
        filepath: rootDirectory + model.fileName,
        filename: model.fileName
    };
    return asset;
}