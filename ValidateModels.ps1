$outputFolder = "Output"

$manifestPath = Join-Path -Path ".." -ChildPath $outputFolder | Join-Path -ChildPath "Manifest.json"
$logFile = Join-Path -Path ".." -ChildPath $outputFolder |Join-Path -ChildPath "ValidationResults.txt"

Set-Location "Validate"
Write-Host "Validating models in manifest:"
Resolve-Path -Path $manifestPath | Write-Host
npm start -- "manifest=$manifestPath" | Out-File $logFile
Set-Location ..