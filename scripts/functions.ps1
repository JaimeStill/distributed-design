function Get-Json([string] $path) {
    Get-Content $path -Raw | ConvertFrom-Json
}

function Get-PublicIp([string] $url = "ifcfg.me") {
    Invoke-RestMethod $url
}

function Get-SecureString([SecureString] $value) {
    ConvertFrom-SecureString $value -AsPlainText
}

function Merge-Name([string] $prefix, [string] $value) {
    Write-Output "$prefix$value"
}