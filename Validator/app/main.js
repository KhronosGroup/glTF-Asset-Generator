const fs = require('fs');
const path = require('path');
const validator = require('gltf-validator');

const outputFolder = path.join(__dirname, '..', '..', 'Output');
const logOutputFolder = path.join(outputFolder, '_ValidatorResults');
const manifest = path.join(outputFolder, 'Manifest.json');

var assetsWithErrors = [];
var promises = [];

// Load the manifest and convert the metadata into objects for easier consumption.
glTFAssets = loadManifestFile(outputFolder, manifest);

// For each model, build a promise to validate it.
for (let i = 0; i < glTFAssets.length; ++i) {
    promises.push(validateModel(glTFAssets[i]));
}

// Validate each model async. When all models are finished being validated, output the results.
Promise.all(promises).then(() => {
    console.log();
    console.log('Models verified: ' + glTFAssets.length);
    console.log('Models with errors: ' + assetsWithErrors.length);

    // Build the summary report header.
    let summary = [];
    summary.push('\n');
    summary.push('| Model | Status | Errors | Warnings | Messages |\n');
    summary.push('| :---: | :---: | :---: | :---: | :---: |\n');

    // Build a row for each model in the summary report.
    for (let i = 0; i < glTFAssets.length; ++i) {
        const issues = glTFAssets[i].report.issues;
        let status;
        if (parseInt(issues.numErrors) > 0) {
            status = ':x:';
        }
        else{
            status = ':white_check_mark:'
        }

        const modelLink = `[${glTFAssets[i].fileName.slice(0, -5)}](${convertToURL(path.join('..', glTFAssets[i].modelGroup, glTFAssets[i].fileName))})`;
        const messages = JSON.stringify(issues.messages).split(',').join('<br>').split('}').join('<br>').split('{').join('').replace('[', '').replace(']', '');
        summary.push(`| ${modelLink} | ${status} | ${issues.numErrors} | ${issues.numWarnings} | ${messages} |\n`);
    }

    // Write the summary report to file.
    fs.writeFile(path.join(logOutputFolder, 'README.md'), summary.join('').split('\n').join('\r\n'), (err) => {
        if (err) throw err;
    });

});

/**
 * Wraps the code for calling the Validator in a function so that it can be used as a promise.
 * @param glTFAsset Object containing location and name metadata of a model. Created by createGLTFAsset().
 */
function validateModel(glTFAsset) {
    const asset = fs.readFileSync(glTFAsset.filePath);

    return validator.validateBytes(new Uint8Array(asset), {
        uri: glTFAsset.fileName,
    }).then((report) => {
        glTFAsset.report = report;

        //  Write the results to file.
        const modelDirectory = path.join(logOutputFolder, path.basename(glTFAsset.fileDirectory));
        delete report.validatedAt;
        try {
            fs.mkdirSync(modelDirectory, { recursive: true } );
        } catch (e) {
            console.log('Cannot create folder ', e);
        }
        fs.writeFile(path.join(modelDirectory, glTFAsset.fileName) + '.log', (JSON.stringify(report, null, 4).split('\n').join('\r\n') + '\r\n'), (err) => {
            if (err) throw err;
        });

        // Write a simple result to console as models are completed.
        if(parseInt(report.issues.numErrors) > 0) {
            console.log('Error found: ' + report.uri);
            assetsWithErrors.push(asset);
        }
        else {
            console.log('Passed: ' + report.uri);
        }
     })
    .catch((error) => console.error('Validation failed: ', error));
}

/**
 * Get all the model metadata of the manifest file.
 * @param outputFolder Path to the Output folder, where the manifest is located.
 * @param manifestJSON Filepath to the manifest.
 */
function loadManifestFile(outputFolder, manifestJSON) {
    const result = [];

    // Open the manifest file.
    const content = fs.readFileSync(manifestJSON);

    // Create an object for each model's metadata.
    const jsonData = JSON.parse(content);
    for (let i = 0; i < jsonData.length; ++i) {
        const jsonObj = jsonData[i];
        const modelFolder = jsonObj.folder;
        for (const model of jsonObj.models) {
            result.push(createGLTFAsset(model, path.join(outputFolder, modelFolder), modelFolder));
        }
    }
    return result;
}

/**
 * Assembles location and name metadata of the glTF model.
 * @param model JSON object of the model's metadata, extracted from the manifest.
 * @param rootDirectory Directory that the model is located in.
 * @param modelFolder Name of the folder containing the model. Is also the name of the modelgroup the model belongs to.
 */
function createGLTFAsset(model, rootDirectory, modelFolder) {
    const asset = {
        modelGroup: modelFolder,
        fileDirectory: rootDirectory,
        filePath: path.join(rootDirectory, model.fileName),
        fileName: model.fileName
    };
    return asset;
}

function convertToURL(filePath) {
    return filePath.replace(/\\/g, '/');
}
