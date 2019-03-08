const os = require('os');
const fs = require('fs');
const path = require('path');
const validator = require('gltf-validator');

// Alternate path is provided for when the script is run directly, instead of as a VS Code launch configuration.
const outputFolder = fs.existsSync('Output') ? 'Output' : path.join('..', 'Output');
const logOutputFolder = fs.existsSync('Output') ? 'ValidatorResults' : path.join('..', 'ValidatorResults');
const manifest = path.join(outputFolder, 'Manifest.json');

const assetsWithErrors = [];
const promises = [];

// Load the manifest and convert the metadata into objects for easier consumption.
glTFAssets = loadManifestFile(outputFolder, manifest);

// For each model, build a promise to validate it.
for (const glTFAsset of glTFAssets) {
    promises.push(validateModel(glTFAsset));
}

// Validate each model async. When all models are finished being validated, output the results.
Promise.all(promises).then(() => {
    console.log('');
    console.log('Models verified: ' + glTFAssets.length);
    console.log('Models with errors: ' + assetsWithErrors.length);

    // The summary report is a single file, but there is a seperate table for each modelgroup.
    const summary = [];
    let lastModelgroupEntered = null;

    // Build a row for each model in the summary report.
    for (const glTFAsset of glTFAssets) {
        if (lastModelgroupEntered != glTFAsset.modelGroup)
        {
            // Build the table header when the first model in a modelgroup is being added.
            // Model group name is derived from the folder name. Underscores are replaced by 
            // spaces and spaces are added before capital letters, except in the case of 'UV'.
            summary.push(`# ${glTFAsset.modelGroup.replace('_', ' ').replace(/([A-Z])/g, ' $1').replace('U V', 'UV').trim()}`);
            summary.push('| Model | Status | Errors | Warnings | Infos | Hints |');
            summary.push('| :---: | :---: | :---: | :---: | :---: | :---: |');
        }
        lastModelgroupEntered = glTFAsset.modelGroup;

        const issues = glTFAsset.report.issues;
        const status = (parseInt(issues.numErrors) > 0) ? ':x:' : ':white_check_mark:';

        summary.push(`| ${glTFAsset.logHyperlink} | ${status} | ${issues.numErrors} | ${issues.numWarnings} | ${issues.numInfos} | ${issues.numHints} |`);
    }

    // Write the summary report to file.
    fs.writeFile(path.join(logOutputFolder, 'README.md'), summary.join(os.EOL), (error) => {
        if (error) throw error;
    });
});

/**
 * Wraps the code for calling the Validator in a function so that it can be used as a promise.
 * @param glTFAsset Object containing location and name metadata of a model. Created by createGLTFAsset().
 */
function validateModel(glTFAsset) {
    const asset = fs.readFileSync(glTFAsset.modelFilepath);

    return validator.validateBytes(new Uint8Array(asset), {
        uri: glTFAsset.modelName,
        externalResourceFunction: (uri) =>
            new Promise((resolve, reject) => {
                uri = path.resolve(path.dirname(glTFAsset.modelFilepath), decodeURIComponent(uri));
                fs.readFile(uri, (error, data) => {
                    if (error) {
                        console.error(error.toString());
                        reject(error.toString());
                        return;
                    }
                    resolve(data);
                });
            })
    }).then((report) => {
        glTFAsset.report = report;

        // The property 'validatedAt' shows up as a change every time the validator is run. Deleting in order to focus diff results on actual changes.
        delete report.validatedAt;

        // Write the results to file.
        fs.mkdirSync(glTFAsset.logDirectory, { recursive: true } );
        fs.writeFile(glTFAsset.logFilepath, (JSON.stringify(report, null, 4).replace(/(?:\n)/g, os.EOL)), (error) => {
            if (error) throw error;
        });

        // Write a simple result to console as models are completed.
        if (parseInt(report.issues.numErrors) > 0) {
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

    // Create an object for each model, containing that model's metadata.
    const modelGroups = JSON.parse(content);
    for (const modelGroup of modelGroups) {
        const modelFolder = modelGroup.folder;
        for (const model of modelGroup.models) {
            const baseFileName = path.basename(model.fileName, '.gltf');
            result.push({
                modelName: model.fileName,
                modelGroup: modelFolder,
                modelFilepath: path.join(outputFolder, modelFolder, model.fileName),
                logDirectory: path.join(logOutputFolder, modelFolder),
                logFilepath: `${path.join(logOutputFolder, modelFolder, baseFileName)}.log`,
                logHyperlink: `[${baseFileName.slice(-2)}](${modelFolder}/${baseFileName}.log)`
            });
        }
    }
    return result;
}
