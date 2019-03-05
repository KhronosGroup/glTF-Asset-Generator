const os = require('os');
const fs = require('fs');
const path = require('path');
const exec = require('child_process').exec;

// Declare all of the common paths that will be used in generating screenshots.
const projectDirectory = path.join(__dirname, '..');
const outputFolder = path.join(projectDirectory, 'Output');
const manifestPath = path.join(outputFolder, 'Manifest.json');
const resourcesDirectory = path.join(projectDirectory, "Source", "Resources");
const preGenImagesDirectory = path.join(resourcesDirectory, "Figures", "SampleImages");
const rootTempDirectory = path.join(projectDirectory, "tempImages");
const tempOutputDirectory = path.join(rootTempDirectory, "Figures");
const tempSampleImagesDirectory = path.join(tempOutputDirectory, "SampleImages");
const tempThumbnailsDirectory = path.join(tempOutputDirectory, "Thumbnails");
const defaultImage = path.join(resourcesDirectory, "Figures", "NYI.png");
const screenshotGeneratorRoot = path.join(projectDirectory, "ScreenshotGenerator");
const screenshotGeneratorDirectory = path.join(screenshotGeneratorRoot, 'app');
const resizeScript = path.join(screenshotGeneratorRoot, "pythonScripts", "dist", "resizeImages.exe");

// Load the manifest into memory.
var manifest;
fs.readFile(manifestPath, function(error, data) {
    if (error) throw error;
    manifest = JSON.parse(data);
 });
 
// Create some temp folders. The screenshots will be stored here after generation until being sorted into the Output folder.
try {
    fs.mkdirSync(tempOutputDirectory, { recursive: true } );
    fs.mkdirSync(tempSampleImagesDirectory, { recursive: true } );
    fs.mkdirSync(tempThumbnailsDirectory, { recursive: true } );
} catch (e) {
    console.log('Cannot create folder ', e);
}

// Generate the screenshots via the ScreenshotGenerator.
const callScreenshotGenerator = `npm start -- "headless=true" "manifest=${manifestPath}" "outputDirectory=${tempSampleImagesDirectory}"`;
const callThumbnailGenerator = `${resizeScript} --dir=${tempSampleImagesDirectory} --outputDir=${tempThumbnailsDirectory} --width=72 --height=72`;
console.log();
console.log('Running the ScreenshotGenerator...');
runProgram(callScreenshotGenerator, screenshotGeneratorDirectory, function() {
    console.log('Finished generating screenshots.');
    console.log('');
    console.log('Creating thumbnails...');
    runProgram(callThumbnailGenerator, screenshotGeneratorDirectory, function() {
        console.log('Finished generating thumbnails.');
        console.log('');
        sortImages()
        .then(
            // Delete the temp images directory.
            console.log('Finished copying images to the Output directory.'),
            console.log(''),
            console.log('Cleaning up temporary files...'),
            deleteFolderRecursive(rootTempDirectory),
            console.log('Clean up complete.'),
        );
    });
});

function sortImages() {
    return new Promise(resolve => {
        // Check for images that are pre-generated.
        const preGenImages = fs.readdirSync(preGenImagesDirectory);
        const genImages = fs.readdirSync(tempSampleImagesDirectory);
        const existingImageList = [];

        for (const genImage in genImages) {
            for (const preGenImage in preGenImages) {
                if (genImages[genImage] == preGenImages[preGenImage]) {
                    existingImageList.push(preGenImages[preGenImage]);
                    break;
                }
            }
        }
        if (existingImageList.length > 0) {
            console.log('The following generated image(s) will not be used, due to there already being a pre-generated image:');
            for (const image in existingImageList) {
                console.log(image);
            }
        }

        // Move the sample images and thumbnails into their respective folders.
        console.log('Copying images to the Output directory...');
        for (const modelgroup in manifest) {
            const modelGroupPath = path.join(outputFolder, manifest[modelgroup].folder);
            for (const model in manifest[modelgroup].models) {
                if (manifest[modelgroup].models[model].sampleImageName != null) {
                    const thumbnailName = manifest[modelgroup].models[model].sampleImageName.replace('SampleImages', 'Thumbnails');

                    // Builds paths to the expected generated images and their destinations.
                    const imageDestination = path.join(modelGroupPath, manifest[modelgroup].models[model].sampleImageName);
                    const imageThumbnailDestination = path.join(modelGroupPath, thumbnailName);
                    const imageSource = path.join(rootTempDirectory, manifest[modelgroup].models[model].sampleImageName);
                    const imageThumbnailSource = path.join(rootTempDirectory, thumbnailName);

                    // Create the directory if it doesn't exist.
                    try {
                        fs.mkdirSync(path.dirname(imageDestination), { recursive: true } );
                        fs.mkdirSync(path.dirname(imageThumbnailDestination), { recursive: true } );
                    } catch (e) {
                        console.log('Cannot create folder ', e);
                    }

                    // Check if there is an pre-gen image, and use that filepath instead if it does.
                    if (existingImageList.Count > 0) {
                        for (const preGenImage in existingImageList) {
                            if (existingImageList[preGenImage] == path.basename(manifest[modelgroup].models[model].sampleImageName)) {
                                imageSource = path.join(resourcesDirectory, manifest[modelgroup].models[model].sampleImageName);
                                imageThumbnailSource = path.join(resourcesDirectory, thumbnailName);
                                break;
                            }
                        }
                    }

                    if (fs.existsSync(imageSource) && fs.existsSync(imageThumbnailSource)) {
                        // Copy the image and thumbnail into the relevant folder in the Output directory.
                        fs.copyFile(imageSource, imageDestination, (error) => { if (error) throw error; });
                        fs.copyFile(imageThumbnailSource, imageThumbnailDestination, (error) => { if (error) throw error; });
                    }
                    else {
                        // There is no image, so use a copy of the default image instead.
                        fs.copyFile(defaultImage, imageDestination, (error) => { if (error) throw error; });
                        fs.copyFile(defaultImage, imageThumbnailDestination, (error) => { if (error) throw error; });
                    }
                    
                }
            }
        }
        resolve();
    });
}

/**
 * Launches an specified external program.
 * @param cmd Program to run, including command line parameters.
 * @param directory Filepath to execute the program from.
 * @param exitFunc Code to be run on completion. (Exit message, next function, etc...)
 */
function runProgram(cmd, directory, exitFunc) {
    const child = exec(cmd, {cwd: directory, stdio: 'inherit'});
    child.stdout.on('data', function(data) {
        console.log(data.toString());
    });
    child.stderr.on('data', function(data) {
        console.log(data.toString());
    });
    child.on('close', function() {
        exitFunc();
    });
}

function deleteFolderRecursive(folderPath) {
    var files = [];
    if (fs.existsSync(folderPath)) {
        files = fs.readdirSync(folderPath);
        files.forEach(function(file,index) {
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
