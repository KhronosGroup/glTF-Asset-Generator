$outputFolder = "Output"

$manifestPath = Join-Path -Path ".." -ChildPath $outputFolder | Join-Path -ChildPath "Manifest.json"
$logFile = Join-Path -Path ".." -ChildPath $outputFolder |Join-Path -ChildPath "ValidationResults.log"

Set-Location "Validator"
Write-Host "Validating models in manifest:"
Resolve-Path -Path $manifestPath | Write-Host
npm start -- "manifest=$manifestPath" | Out-File $logFile -Encoding utf8
Set-Location ..
