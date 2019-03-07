const fs = require('fs');
const path = require('path');
const exec = require('child_process').exec;

// Declare all of the common paths that will be used in generating screenshots.
const projectDirectory = path.join(__dirname, '..');
const outputFolder = path.join(projectDirectory, 'Output');
const manifestPath = path.join(outputFolder, 'Manifest.json');
const resourcesDirectory = path.join(projectDirectory, 'Source', 'Resources');
const preGenImagesDirectory = path.join(resourcesDirectory, 'Figures', 'SampleImages');
const rootTempDirectory = path.join(projectDirectory, 'tempImages');
const tempOutputDirectory = path.join(rootTempDirectory, 'Figures');
const tempSampleImagesDirectory = path.join(tempOutputDirectory, 'SampleImages');
const tempThumbnailsDirectory = path.join(tempOutputDirectory, 'Thumbnails');
const defaultImage = path.join(resourcesDirectory, 'Figures', 'NYI.png');
const screenshotGeneratorRoot = path.join(projectDirectory, 'ScreenshotGenerator');
const screenshotGeneratorDirectory = path.join(screenshotGeneratorRoot, 'app');
const resizeScript = path.join(screenshotGeneratorRoot, 'pythonScripts', 'dist', 'resizeImages.exe');
 
// Create some temp folders. The screenshots will be stored here after generation until being sorted into the Output folder.
try {
    fs.mkdirSync(tempOutputDirectory, { recursive: true } );
    fs.mkdirSync(tempSampleImagesDirectory, { recursive: true } );
    fs.mkdirSync(tempThumbnailsDirectory, { recursive: true } );
} catch (error) {
    console.log('Cannot create folder');
    throw error;
}

// Generate the screenshots via the ScreenshotGenerator, resize those screenshots to create thumbnails, 
// sorts images into the Output directory, and deletes the temporary images folder created by the screenshot generator.
console.log('');
console.log('Running the ScreenshotGenerator...');
const screenshotGeneratorCmd = `npm start -- headless=true manifest="${manifestPath}" outputDirectory="${tempSampleImagesDirectory}"`;
runProgram(screenshotGeneratorCmd, screenshotGeneratorDirectory, function() {
    console.log('Finished generating screenshots.');
    console.log('');
    console.log('Creating thumbnails...');
    const thumbnailGeneratorCmd = `"${resizeScript}" --dir="${tempSampleImagesDirectory}" --outputDir="${tempThumbnailsDirectory}" --width=72 --height=72`;
    runProgram(thumbnailGeneratorCmd, screenshotGeneratorDirectory, function() {
        console.log('Finished generating thumbnails.');
        console.log('');
        console.log('Copying images to the Output directory...');
        sortImages();
        console.log('Finished copying images to the Output directory.');
        console.log('');
        console.log('Cleaning up temporary files...');
        deleteFolderRecursive(rootTempDirectory);
        console.log('Clean up complete.');
    });
});

/**
 * Moves screenshots and thumbnails from the temp folders into their final location in the Output directory.
 */
function sortImages() {
    // Check for images that are pre-generated.
    const preGenImages = fs.readdirSync(preGenImagesDirectory);
    const genImages = fs.readdirSync(tempSampleImagesDirectory);
    const existingImageList = [];

    genImages.forEach(function (genImage) {
        preGenImages.some(function (preGenImage) {
            if (genImage == preGenImage) {
                existingImageList.push(preGenImage);
                return true;
            } else return false;
        })
    });
    if (existingImageList.length > 0) {
        console.log('The following generated image(s) will not be used, due to there already being a pre-generated image:');
        for (const image in existingImageList) {
            console.log(image);
        }
    }

    // Move the sample images and thumbnails into their respective folders.
    JSON.parse(fs.readFileSync(manifestPath)).forEach(function (modelgroup) {
        const modelGroupPath = path.join(outputFolder, modelgroup.folder);
        modelgroup.models.forEach(function (model) {
            if (model.sampleImageName != null) {
                const thumbnailName = model.sampleImageName.replace('SampleImages', 'Thumbnails');

                // Builds paths to the expected generated images and their destinations.
                const imageDestination = path.join(modelGroupPath, model.sampleImageName);
                const imageThumbnailDestination = path.join(modelGroupPath, thumbnailName);
                const imageSource = path.join(rootTempDirectory, model.sampleImageName);
                const imageThumbnailSource = path.join(rootTempDirectory, thumbnailName);

                // Create the directory if it doesn't exist.
                try {
                    fs.mkdirSync(path.dirname(imageDestination), { recursive: true } );
                    fs.mkdirSync(path.dirname(imageThumbnailDestination), { recursive: true } );
                } catch (error) {
                    console.log('Cannot create folder');
                    throw error;
                }

                // Check if there is an pre-gen image, and use that filepath instead if it does.
                if (existingImageList.Count > 0) {
                    for (const preGenImage in existingImageList) {
                        if (existingImageList[preGenImage] == path.basename(model.sampleImageName)) {
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
function runProgram(cmd, directory, exitFunc) {
    const child = exec(cmd, {cwd: directory});
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

/**
 * Deletes a folder and all files/folders it contains.
 * @param folderPath Folder to delete.
 */
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
