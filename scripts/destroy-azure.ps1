param(
    [string]
    [Parameter(Mandatory)]
    $Configuration
)

# load core functions
. .\functions.ps1

# variable initialization
$config = Get-Json $Configuration
$rg = Merge-Name $config.prefix $config.resourceGroup

# delete resource group
& az group delete -n $rg -y