function Remove-Artifact([string] $path) {
    if (Test-Path $path) {
        Remove-Item $path -Force -Recurse
    }
}

function Remove-Artifacts([string] $path) {
    Write-Host "Removing artifacts for $path" -ForegroundColor Blue

    $dist = Join-Path $path 'dist'
    $modules = Join-Path $path 'node_modules'
    $angular = Join-Path $path '.angular'

    Remove-Artifact $dist
    Remove-Artifact $modules
    Remove-Artifact $angular

    Write-Host "Artifacts successfully removed" -ForegroundColor Green
}

$Directories = @(
    "../apps/libs/core/",
    "../apps/libs/contracts/core/",
    "../apps/libs/contracts/workflows/",
    "../apps/libs/distributed",
    "../apps/proposals/",
    "../apps/workflows/"
)

foreach ($directory in $Directories) {
    Remove-Artifacts $directory
}