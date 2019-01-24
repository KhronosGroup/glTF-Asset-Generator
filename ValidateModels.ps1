$outputFolder = "Output"

$manifestPath = Join-Path -Path ".." -ChildPath $outputFolder | Join-Path -ChildPath "Manifest.json"
$logFile = Join-Path -Path ".." -ChildPath $outputFolder |Join-Path -ChildPath "ValidationResults.txt"

cd "gltf-validator"
npm start -- "manifest=$manifestPath" | Out-File $logFile
cd ..