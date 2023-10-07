function GetJson([string] $path) {
    Get-Content $path -Raw | ConvertFrom-Json
}

function GetPublicIp([string] $url = "ifcfg.me") {
    Invoke-RestMethod $url
}

function GetSecureString([SecureString] $value) {
    ConvertFrom-SecureString $value -AsPlainText
}

function MergeName([string] $prefix, [string] $value) {
    Write-Output "$prefix$value"
}