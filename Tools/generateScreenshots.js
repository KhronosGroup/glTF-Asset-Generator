const fs = require('fs');
const path = require('path');
const exec = require('child_process').exec;

// Declare all of the common paths that will be used in generating screenshots.
const projectDirectory = path.join(__dirname, '..');
const screenshotGeneratorDirectory = path.join(projectDirectory, 'screenshotGenerator', 'app');
const resizeScript = path.join(projectDirectory, 'screenshotGenerator', 'pythonScripts', 'dist', 'resizeImages.exe');
const positiveTestManifest = path.join(projectDirectory, 'Output', 'Positive', 'Manifest.json');
// Uncomment to generate screenshots for negative test models. See comment for generateScreenshots() below.
//const negativeTestManifest = path.join(projectDirectory, 'Output', 'Negative', 'Manifest.json');
const preGenImagesDirectory = path.join(projectDirectory, 'Source', 'Resources', 'Figures', 'SampleImages');
const defaultImage = path.join(projectDirectory, 'Source', 'Resources', 'Figures', 'NYI.png');
const rootTempDirectory = path.join(projectDirectory, 'tempImages');
const tempOutputDirectory = path.join(rootTempDirectory, 'Figures');
const tempSampleImagesDirectory = path.join(tempOutputDirectory, 'SampleImages');
const tempThumbnailsDirectory = path.join(tempOutputDirectory, 'Thumbnails');

// Uncomment the following code to generate screenshots for negative test models.
// No negative tests currently use screenshots, so generating them is a waste of time.
generateScreenshots(positiveTestManifest, 'Positive Tests')//.then(() => {
//    generateScreenshots(negativeTestManifest, 'Negative Tests');
//})
.catch((error) => { throw new Error(error)});

/**
 * Creates screenshots of all models in a manifest.
 * @param manifestPath Path to a Manifest.json 
 * @param Name used to refer to all of the model groups in the manifest.
 */
function generateScreenshots(manifestPath, testName)
{
    return new Promise((resolve) => {
        // Create some temp folders. The screenshots will be stored here after creation until being sorted into the Output folder.
        fs.mkdirSync(tempOutputDirectory, { recursive: true } );
        fs.mkdirSync(tempSampleImagesDirectory, { recursive: true } );
        fs.mkdirSync(tempThumbnailsDirectory, { recursive: true } );

        // Generate the screenshots via the ScreenshotGenerator, resize those screenshots to create thumbnails, 
        // sorts images into the Output directory, and deletes the temporary images folder created by the screenshot generator.
        console.log('');
        console.log(`Running the ScreenshotGenerator for: ${testName}...`);
        const screenshotGeneratorCmd = `npm start -- -- headless=true manifest="${manifestPath}" outputDirectory="${tempSampleImagesDirectory}"`;
        runProgram(screenshotGeneratorCmd, screenshotGeneratorDirectory)
        .then(() => {
            console.log('Finished generating screenshots.');
            console.log('');
            console.log('Creating thumbnails...');
            const thumbnailGeneratorCmd = `"${resizeScript}" --dir="${tempSampleImagesDirectory}" --outputDir="${tempThumbnailsDirectory}" --width=72 --height=72`;
            return runProgram(thumbnailGeneratorCmd, screenshotGeneratorDirectory);
        })
        .then(() => {
            console.log('Finished generating thumbnails.');
            console.log('');
            console.log('Copying images to the Output directory...');
            sortImages(manifestPath);
            console.log('Finished copying images to the Output directory.');
            console.log('');
            console.log('Cleaning up temporary files...');
            deleteFolderRecursive(rootTempDirectory);
            console.log(`Finished screenshot creation for: ${testName}`);
            resolve();
        })
        .catch((error) => { throw new Error(error)});
    });
}

/**
 * Moves screenshots and thumbnails from the temp folders into their final location in the Output directory.
 */
function sortImages(manifestPath) {
    // Check for images that are pre-generated.
    const preGenImages = fs.readdirSync(preGenImagesDirectory);
    const genImages = fs.readdirSync(tempSampleImagesDirectory);
    const existingImageList = [];

    for (const genImage of genImages) {
        for (const preGenImage of preGenImages) {
            if (genImage == preGenImage) {
                existingImageList.push(preGenImage);
                break;
            }
        };
    };
    if (existingImageList.length > 0) {
        console.log('The following generated image(s) will not be used, due to there already being a pre-generated image:');
        for (const image in existingImageList) {
            console.log(image);
        }
    }

    // Move the sample images and thumbnails into their respective folders.
    JSON.parse(fs.readFileSync(manifestPath)).forEach((modelgroup) => {
        const modelGroupPath = path.join(path.dirname(manifestPath), modelgroup.folder);
        modelgroup.models.forEach((model) => {
            if (model.sampleImageName != null) {
                const thumbnailName = model.sampleImageName.replace('SampleImages', 'Thumbnails');

                // Builds paths to the expected generated images and their destinations.
                const imageDestination = path.join(modelGroupPath, model.sampleImageName);
                const imageThumbnailDestination = path.join(modelGroupPath, thumbnailName);
                const imageSource = path.join(rootTempDirectory, model.sampleImageName);
                const imageThumbnailSource = path.join(rootTempDirectory, thumbnailName);

                // Create the directory if it doesn't exist.
                fs.mkdirSync(path.dirname(imageDestination), { recursive: true } );
                fs.mkdirSync(path.dirname(imageThumbnailDestination), { recursive: true } );

                // Check if there is an pre-gen image, and use that filepath instead if it does.
                if (existingImageList.Count > 0) {
                    for (const preGenImage of existingImageList) {
                        if (preGenImage == path.basename(model.sampleImageName)) {
                            imageSource = path.join(resourcesDirectory, model.sampleImageName);
                            imageThumbnailSource = path.join(resourcesDirectory, thumbnailName);
                            break;
                        }
                    }
                }

                if (fs.existsSync(imageSource) && fs.existsSync(imageThumbnailSource)) {
                    // Copy the image and thumbnail into the relevant folder in the Output directory.
                    fs.copyFileSync(imageSource, imageDestination, (error) => { if (error) throw error; });
                    fs.copyFileSync(imageThumbnailSource, imageThumbnailDestination, (error) => { if (error) throw error; });
                }
                else {
                    // There is no image, so use a copy of the default image instead.
                    fs.copyFileSync(defaultImage, imageDestination, (error) => { if (error) throw error; });
                    fs.copyFileSync(defaultImage, imageThumbnailDestination, (error) => { if (error) throw error; });
                }
            }
        })
    });
}

/**
 * Launches an specified external program.
 * @param cmd Program to run, including command line parameters.
 * @param directory Filepath to execute the program from.
 * @param exitFunc Code to be run on completion. (Exit message, next function, etc...)
 */
function runProgram(cmd, directory) {
    return new Promise((resolve, reject) => {
        const child = exec(cmd, {cwd: directory});
        child.stdout.on('data', (data) => {
            console.log(data.toString());
        });
        child.stderr.on('data', (data) => {
            console.log(data.toString());
            reject(data);
        });
        child.on('close', () => {
            resolve();
        });
    });
}

/**
 * Deletes a folder and all files/folders it contains.
 * @param folderPath Folder to delete.
 */
function deleteFolderRecursive(folderPath) {
    var files = [];
    if (fs.existsSync(folderPath)) {
        files = fs.readdirSync(folderPath);
        files.forEach((file) => {
            var curPath = path.join(folderPath, file);
            // Recursive
            if (fs.lstatSync(curPath).isDirectory()) {
                deleteFolderRecursive(curPath);
            } else { 
                // Delete file
                fs.unlinkSync(curPath);
            }
        });
        fs.rmdirSync(folderPath);
    }
};
