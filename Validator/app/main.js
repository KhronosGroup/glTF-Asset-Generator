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
var assetsWithIssues = [];
var promises = [];

for (let i = 0; i < glTFAssets.length; ++i) {
    promises.push(validateModel(glTFAssets[i]));  
}

Promise.all(promises)
.then(() => {
    console.log();
    console.log('Models Verified: ' + glTFAssets.length);
    console.log('Models with issues: ' + assetsWithIssues.length);    
});

/**
 * Wraps the code for calling the Validator in a function so that it can be used as a promise.
 * @param glTFAsset
 */
function validateModel(glTFAsset) {
    const asset = fs.readFileSync(glTFAsset.filepath);

    validator.validateBytes(new Uint8Array(asset), {
        uri: glTFAsset.filename,
    }).then((report) => parseValidatorResults(report, asset))
    .catch((error) => console.error('Validation failed: ', error));  
}

/**
 * Parses the Validator report results into a desired format that focuses on errors and warnings.
 * @param report
 * @param asset
 */
function parseValidatorResults(report, asset) {
    if(parseInt(report.issues.numWarnings) > 0 || parseInt(report.issues.numErrors) > 0 || report.issues.messages.length > 0) {

        assetsWithIssues.push(asset);
        let lineDivider = '-------------------------------------------------------------------------';
        console.log();
        console.log(lineDivider);
        console.log('Filename: ' + report.uri);
        console.log('Errors: ' + report.issues.numErrors);
        console.log('Warnings: ' + report.issues.numWarnings);
        console.log('Messages:')
        for (const message of report.issues.messages) {
            console.log();
            console.log(message);
        }
        console.log(lineDivider);   
    }
}

/**
 * Parses the arguments passed when the script called from the command line.
 */
function parseCommandLineArguments() {
    const args = new Array();
    const commandLineArgs = process.argv;
    const numberOfArgs = commandLineArgs.length;
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

/**
 * Validates the arguments passed when the script called from the command line.
 * @param commandLineArgs
 */
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
            if (model.valid != false) {
                result.push(createGLTFAsset(model, rootDirectory));
            }
        }
    }
    else {
        for (let i = 0; i < jsonData.length; ++i) {
            const jsonObj = jsonData[i];
            const folder = jsonObj.folder;
            for (const model of jsonObj.models) {
                if (model.valid != false) {
                    result.push(createGLTFAsset(model, rootDirectory + folder + '/'));
                }
            }
        }
    }
    return result;
}

function convertToURL(filePath) {
    return filePath.replace(/\\/g, '/');
}

/**
 * Assembles location and name information of the glTF model.
 * @param modelInfroFromManifest
 * @param roodDirectory
 */
function createGLTFAsset(model, rootDirectory) {
    const asset = {
        filedirectory: rootDirectory,
        filepath: rootDirectory + model.fileName,
        filename: model.fileName
    };
    return asset;
}