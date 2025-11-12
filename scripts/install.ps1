$appName = "SimpleSync"
$githubUser = "a-sLamT-M"
$repo = "SimpleSync"
$release = "v.1.1.0"
$installDir = "$env:ProgramFiles\$appName"
$zipUrl = "https://github.com/$githubUser/$repo/releases/download/$release/$appName-v1.1.0-win-x64.zip"
$zipFile = "$env:TEMP\$appName.zip"

New-Item -ItemType Directory -Force -Path $installDir | Out-Null

Write-Host "Downloading $appName..."
Invoke-WebRequest -Uri $zipUrl -OutFile $zipFile

Write-Host "Extracting $appName..."
Expand-Archive -Path $zipFile -DestinationPath $installDir -Force

Remove-Item $zipFile

$currentPath = [Environment]::GetEnvironmentVariable("Path", "Machine")
if ($currentPath -notlike "*$installDir*") {
    Write-Host "Adding $appName to PATH..."
    $newPath = "$currentPath;$installDir"
    [Environment]::SetEnvironmentVariable("Path", $newPath, "Machine")
    $env:Path = $newPath
}

Write-Host "$appName has been installed to $installDir and added to PATH."
Write-Host "Please restart your terminal or log off and on again for the PATH changes to take effect."