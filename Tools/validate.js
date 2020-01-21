const os = require('os');
const fs = require('fs');
const path = require('path');
const validator = require('gltf-validator');

const outputDirectory = path.join(__dirname, '..', 'Output');
const positiveTestManifest = path.join(outputDirectory, 'Positive', 'Manifest.json');
const negativeTestManifest = path.join(outputDirectory, 'Negative', 'Manifest.json');

// Each manifest is validated seperatly. 
// A summary of the results are printed to the console once they have all completed.
Promise.all([
    validateModelsInManifest(positiveTestManifest, 'Positive Tests'),
    validateModelsInManifest(negativeTestManifest, 'Negative Tests'),
])
.then((results) => {
    for (const result of results)
    {
        console.log('');
        console.log(result.name);
        console.log('Models validated: ' + result.modelsVerified);
        console.log('Models with errors: ' + result.modelsWithErrors);
    }
})
.catch((error) => { throw new Error(error)});

/**
 * Returns a promise for testing all models in a manifest.
 * Writes a summary report to file when finished.
 * The result of the promise is an object containing a count of the number of models validated and the count of models that have reported validator errors.
 * @param manifestPath Path to a Manifest.json 
 * @param testName Name used to refer to all of the model groups in the manifest.
 */
function validateModelsInManifest(manifestPath, testName)
{
    return new Promise((resolve) => {
        // Load the manifest and convert the metadata into objects for easier consumption.
        const glTFAssets = loadManifestFile(manifestPath);

        // For each model, build a promise to validate it.
        const promises = [];
        for (const glTFAsset of glTFAssets) {
            promises.push(validateModel(glTFAsset));
        }

        // Validate each model async. When all models are finished being validated, output the results.
        Promise.all(promises).then((results) => {

            // The summary report is a single file, but there is a seperate table for each modelgroup.
            const summary = [];
            let lastHeaderEntered = null;

            // Build a row for each model in the summary report.
            // The last modelgroup header entered into the summary is tracked so the headers are only created once each.
            for (const glTFAsset of glTFAssets) {
                if (lastHeaderEntered != glTFAsset.modelGroup)
                {
                    // Build the table header when the first model in a modelgroup is being added.
                    // Model group name is derived from the folder name. Underscores are replaced by 
                    // spaces and spaces are added before capital letters, except in the case of 'UV'.
                    summary.push(`# ${glTFAsset.modelGroup.replace('_', ' ').replace(/([A-Z])/g, ' $1').replace('U V', 'UV').trim()}`);
                    summary.push('| Model | Status | Errors | Warnings | Infos | Hints |');
                    summary.push('| :---: | :---: | :---: | :---: | :---: | :---: |');
                }
                lastHeaderEntered = glTFAsset.modelGroup;

                const issues = glTFAsset.report.issues;
                const status = (parseInt(issues.numErrors) > 0) ? ':x:' : ':white_check_mark:';
                summary.push(`| ${glTFAsset.logHyperlink} | ${status} | ${issues.numErrors} | ${issues.numWarnings} | ${issues.numInfos} | ${issues.numHints} |`);
            }

            // Write the summary report to file.
            fs.writeFile(path.join(path.dirname(manifestPath), 'README.md'), summary.join(os.EOL), (error) => {
                if (error) throw error;
            });

            // Returns data so summary results can be displayed in the console.
            resolve({
                name: testName,
                modelsVerified: results.length,
                modelsWithErrors: results.reduce((accumulator, currentValue) => accumulator + currentValue),
            });
        });
    });
}

/**
 * Wraps the code for calling the Validator in a function so that it can be used as a promise.
 * @param glTFAsset Object containing location and name metadata of a model. Created by createGLTFAsset().
 */
async function validateModel(glTFAsset) {
    const asset = fs.readFileSync(glTFAsset.modelFilepath);

    return validator.validateBytes(new Uint8Array(asset), {
        uri: glTFAsset.modelName,
        writeTimestamp: false,
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

        // Write the results to file.
        fs.mkdirSync(glTFAsset.logDirectory, { recursive: true } );
        fs.writeFile(glTFAsset.logFilepath, (JSON.stringify(report, null, 4).replace(/(?:\n)/g, os.EOL)), (error) => {
            if (error) throw error;
        });

        // Write a simple result to console as models are completed.
        if (parseInt(report.issues.numErrors) > 0) {
            // Console text color is changed to red for just this line.
            console.log('\x1b[31m%s\x1b[0m', 'Error(s): ' + report.uri);
            return 1;
        }
        else {
            console.log('Passed: ' + report.uri);
            return 0;
        }
     })
    .catch((error) => console.error('Validation failed: ', error));
}

/**
 * Get all the model metadata of the manifest file.
 * @param manifestJSON Filepath to the manifest.
 */
function loadManifestFile(manifestJSON) {
    const result = [];
    const outputFolder = path.dirname(manifestJSON);

    // Open the manifest file.
    const content = fs.readFileSync(manifestJSON);

    // Create an object for each model, containing that model's metadata.
    const modelGroups = JSON.parse(content);
    for (const modelGroup of modelGroups) {
        const modelFolder = modelGroup.folder;
        for (const model of modelGroup.models) {
            const baseFileName = path.basename(model.fileName, '.gltf');
            const logPath = path.join(outputFolder, modelFolder, 'ValidatorResults');
            result.push({
                modelName: model.fileName,
                modelGroup: modelFolder,
                modelFilepath: path.join(outputFolder, modelFolder, model.fileName),
                logDirectory: logPath,
                logFilepath: `${path.join(logPath, baseFileName)}.json`,
                logHyperlink: `[${baseFileName.slice(-2)}](${modelFolder}/ValidatorResults/${baseFileName}.json)`
            });
        }
    }
    return result;
}
