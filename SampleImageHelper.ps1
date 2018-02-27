$outputFolder = "Output"
$sourceFolder = "Source"
$defaultImage = Join-Path -Path $SourceFolder -ChildPath "Figures" | Join-Path -ChildPath "NYI.png"

# Run the image generator
# NYI!
$tempFolder = "tempRefImages"
New-Item -ItemType Directory -Path $tempFolder -Force | Out-Null

# Verify the image generator output against the sample images folder
# If a duplicate exists, then throw an error and ask before continuing.
$existingImageList = [System.Collections.ArrayList]@()
Get-ChildItem $tempFolder | Foreach-Object {
    $imageExists = Join-Path -Path $sourceFolder -ChildPath "SampleImages" | Join-Path -ChildPath $_.Name

    if (Test-Path $imageExists)
    {
        $existingImageList.Add($_.Name)
    }
}

if ($existingImageList.Count -gt 0)
{
    Write-Warning "The following sample image(s) already exist! Are you sure you want to overwrite them?"
    Foreach-Object -InputObject $existingImageList {
        $_
    }
    Read-Host "Press Enter to continue."
}

# Load the JSON manifest file.
$manifestPath = Join-Path -Path $outputFolder -ChildPath "Manifest.json"
$manifest = Get-Content $manifestPath | ConvertFrom-Json

# Move the sample images and thumbnails into their respective folders.
# If a sample image is missing, replace it with a copy of the default image instead.
For ($x=0; $x -lt $manifest.Length; $x++)
{
    $modelGroup = $manifest[$x].folder
    $modelGroupPath = Join-Path -Path $outputFolder -ChildPath $modelGroup

    For ($y=0; $y -lt $manifest[$x].models.Length; $y++)
    {
        $model = $manifest[$x].models[$y]

        # Checks if a sample image is available
        $sourceSampleImage = Join-Path -Path $sourceFolder -ChildPath $model.sampleImageName
        $sourceSampleThumbnail = Join-Path -Path $sourceFolder -ChildPath $model.sampleThumbnailName
        $destinationSampleImage = Join-Path -Path $modelGroupPath -ChildPath $model.sampleImageName
        $destinationSampleThumbnail = Join-Path -Path $modelGroupPath -ChildPath $model.sampleThumbnailName

        # 'Touch' the destination files first to create the directory if it doesn't exist
        New-Item -ItemType File -Path $destinationSampleImage -Force
        New-Item -ItemType File -Path $destinationSampleThumbnail -Force

        if (Test-Path $sourceSampleImage)
        {
            # There is a sample image, so copy it and the thumbnail into the correct folder in the Output directory
            Copy-Item -Path $sourceSampleImage -Destination $destinationSampleImage
            Copy-Item -Path $sourceSampleThumbnail -Destination $destinationSampleThumbnail
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
