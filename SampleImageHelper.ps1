$outputFolder = "Output"
$sourceFolder = "Source"
$figuresFolder = "Figures"
$tempFolder = "tempImages"
$imagesFolder = Join-Path -Path $sourceFolder -ChildPath "SampleImages"
$thumbnailsFolder = Join-Path -Path $imagesFolder -ChildPath "Thumbnails"
$tempImagesFolder = Join-Path -Path $tempFolder -ChildPath "screenshots"
$tempthumbnailsFolder = Join-Path -Path $tempFolder -ChildPath "Thumbnails"
$defaultImage = Join-Path -Path $SourceFolder -ChildPath $figuresFolder | Join-Path -ChildPath "NYI.png"
$manifestPath = Join-Path -Path $outputFolder -ChildPath "Manifest.json"
$screenshotGeneratorPath = Join-Path -Path "pythonScripts" -ChildPath "dist" | Join-Path -ChildPath "resizeImages.exe"

# Run the image generator
$tempFolderFromChild = Join-Path -Path ".." -ChildPath $tempFolder
$manifestPathFromChild = Join-Path -Path ".." -ChildPath $manifestPath
$tempsourceSampleImageFolder = Join-Path -Path $tempFolderFromChild -ChildPath "screenshots"
$sourceSampleThumbnailFolder = Join-Path -Path $tempFolderFromChild -ChildPath "Thumbnails"
New-Item -ItemType Directory -Path $tempFolder -Force | Out-Null
cd "ScreenshotGenerator"
New-Item -ItemType Directory -Path $sourceSampleThumbnailFolder -Force | Out-Null
npm start -- "headless=true" "manifest=$manifestPathFromChild" "outputDirectory=$tempFolderFromChild" # Creates the sample images
Start-Process -Wait -Filepath $screenshotGeneratorPath -ArgumentList "--dir=$tempsourceSampleImageFolder --outputDir=$sourceSampleThumbnailFolder --width=72 --height=72"
cd ".."
Rename-Item -Path $tempImagesFolder -NewName "SampleImages"
$tempImagesFolder = Join-Path -Path $tempFolder -ChildPath "SampleImages"

# Verify the image generator output against the sample images folder
$existingImageList = [System.Collections.ArrayList]@()
Get-ChildItem $tempImagesFolder -File | Foreach-Object {
    $imageExists = Join-Path -Path $sourceFolder -ChildPath "SampleImages" | Join-Path -ChildPath $_.Name

    if (Test-Path $imageExists)
    {
        $existingImageList.Add($_.Name)
    }
}

if ($existingImageList.Count -gt 0)
{
    Write-Warning "The following generated sample image(s) will not be used, due to there already being a custom sample image"
    Foreach-Object -InputObject $existingImageList {
        $_
    }
}

# Load the JSON manifest file.
$manifest = Get-Content $manifestPath | ConvertFrom-Json

# Move the sample images and thumbnails into their respective folders.
# If a sample image is missing, replace it with a copy of the default image instead.
For ($x=0; $x -lt $manifest.Length; $x++)
{
    $modelGroup = $manifest[$x].folder
    $modelGroupPath = Join-Path -Path $outputFolder -ChildPath $modelGroup | Join-Path -ChildPath $figuresFolder

    For ($y=0; $y -lt $manifest[$x].models.Length; $y++)
    {
        $model = $manifest[$x].models[$y]

        # Builds paths to the expected generated images and their destinations
        $overrideSampleImage = Join-Path -Path $sourceFolder -ChildPath $model.sampleImageName
        $overrideSampleThumbnail = Join-Path -Path $sourceFolder -ChildPath $model.sampleThumbnailName
        $sourceSampleImage = Join-Path -Path $tempFolder -ChildPath $model.sampleImageName
        $sourceSampleThumbnail = Join-Path -Path $tempFolder -ChildPath $model.sampleThumbnailName
        $destinationSampleImage = Join-Path -Path $modelGroupPath -ChildPath $model.sampleImageName
        $destinationSampleThumbnail = Join-Path -Path $modelGroupPath -ChildPath $model.sampleThumbnailName

        # 'Touch' the destination files first to create the directory if it doesn't exist
        New-Item -ItemType File -Path $destinationSampleImage -Force
        New-Item -ItemType File -Path $destinationSampleThumbnail -Force

        # Check that there isn't an override sample image
        $override = $False
        if ($existingImageList.Count -gt 0)
        {
            foreach($existingImage in $existingImageList){
                if ($existingImage -eq $model.fileName)
                {
                    $override = $True
                }
            }
        }

        if ((Test-Path $sourceSampleImage) -And ($override -eq $False))
        {
            # There is a sample image, so copy it and the thumbnail into the correct folder in the Output directory
            Copy-Item -Path $sourceSampleImage -Destination $destinationSampleImage
            Copy-Item -Path $sourceSampleThumbnail -Destination $destinationSampleThumbnail
        }
        elseif ($override -eq $True)
        {
            # There is an override for the sample image, so use that instead of the generated one
            Copy-Item -Path $overrideSampleImage -Destination $destinationSampleImage
            Copy-Item -Path $overrideSampleThumbnail -Destination $destinationSampleThumbnail
        }
        else
        {
            # There is no sample image, so replace it and the thumbnail with a copy of the default image instead.
            Copy-Item -Path $defaultImage -Destination $destinationSampleImage
            Copy-Item -Path $defaultImage -Destination $destinationSampleThumbnail
        }
    }
}

# Cleanup the image generator output directory
Remove-Item -Recurse -Force $tempFolder
