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
for (const glTFAsset of glTFAssets) {
    promises.push(validateModel(glTFAsset));
}

// Validate each model async. When all models are finished being validated, output the results.
Promise.all(promises).then(() => {
    console.log();
    console.log('Models verified: ' + glTFAssets.length);
    console.log('Models with errors: ' + assetsWithErrors.length);

    // Build the summary report header.
    const summary = [];
    summary.push('');
    summary.push('| Model | Status | Errors | Warnings | Infos | Hints | Truncated |');
    summary.push('| :---: | :---: | :---: | :---: | :---: | :---: | :---: |');

    // Build a row for each model in the summary report.
    for (const glTFAsset of glTFAssets) {
        const issues = glTFAsset.report.issues;
        const status = (parseInt(issues.numErrors) > 0) ? ':x:' : ':white_check_mark:';

        const modelLink = `[${glTFAsset.baseFileName}](${glTFAsset.modelGroup}/${glTFAsset.baseFileName}.log)`;
        summary.push(`| ${modelLink} | ${status} | ${issues.numErrors} | ${issues.numWarnings} | ${issues.numInfos} | ${issues.numHints} | ${issues.truncated} |`);
    }

    // Write the summary report to file.
    fs.writeFile(path.join(logOutputFolder, 'README.md'), summary.join('\r\n'), (err) => {
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
        maxIssues: 10, // limit max number of output issues to 10
        ignoredIssues: ['UNSUPPORTED_EXTENSION'], // mute UNSUPPORTED_EXTENSION issue
        severityOverrides: { 'ACCESSOR_INDEX_TRIANGLE_DEGENERATE': 0 }, // treat degenerate triangles as errors
        externalResourceFunction: (uri) =>
            new Promise((resolve, reject) => {
                uri = path.resolve(path.dirname(glTFAsset.filePath), decodeURIComponent(uri));
                fs.readFile(uri, (err, data) => {
                    if (err) {
                        console.error(err.toString());
                        reject(err.toString());
                        return;
                    }
                    resolve(data);
                });
            })
    }).then((report) => {
        glTFAsset.report = report;

        //  Write the results to file.
        const modelDirectory = path.join(logOutputFolder, path.basename(glTFAsset.fileDirectory));

        // The property 'validatedAt' shows up as a change every time the validator is run. Deleting in order to focus diff results on actual changes.
        delete report.validatedAt;

        try {
            fs.mkdirSync(modelDirectory, { recursive: true } );
        } catch (e) {
            console.log('Cannot create folder ', e);
        }
        fs.writeFile(path.join(modelDirectory, glTFAsset.baseFileName) + '.log', (JSON.stringify(report, null, 4) + '\r\n'), (err) => {
            if (err) throw err;
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
            const rootDirectory = path.join(outputFolder, modelFolder);
            result.push({
                modelGroup: modelFolder,
                fileDirectory: rootDirectory,
                filePath: path.join(rootDirectory, model.fileName),
                fileName: model.fileName,
                baseFileName: path.basename(model.fileName, '.gltf')
            });
        }
    }
    return result;
}
