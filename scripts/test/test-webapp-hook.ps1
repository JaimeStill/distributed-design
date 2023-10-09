param(
    [string]
    [Parameter(Mandatory)]
    $Configuration,
    [string]
    [Parameter()]
    $Root = '..\..\'
)

# load core functions
. ..\functions.ps1

$origin = $pwd
$config = Get-Json $Configuration
$rg = Merge-Name $config.prefix $config.resourceGroup
$plan = Merge-Name $config.prefix $config.appPlan.name

Set-Location $Root

# resource group
az group create `
    --location $config.location `
    --name $rg

# azure container registry
az acr create `
    --resource-group $rg `
    --name $config.acr.name `
    --sku $config.acr.sku `
    --admin-enabled $true

$acrAdmin = (az acr credential show `
    --name $config.acr.name `
    --query "username" `
    --output tsv)

Write-Host "ACR Admin: $acrAdmin"

$acrPassword = (az acr credential show `
    --name $config.acr.name `
    --query "passwords[0].value" `
    --output tsv)

Write-Host "ACR Password: $acrPassword"

az appservice plan create `
    --name $plan `
    --resource-group $rg `
    --sku $config.appPlan.sku `
    --is-linux

$node = $config.nodes.projects[0]
$app = Merge-Name $config.prefix "-$($node.tag)"

az acr build `
    --registry $config.acr.name `
    --image $node.tag `
    --file $node.dockerfile `
    $config.nodes.path

az webapp create `
    --name $app `
    --plan $plan `
    --resource-group $rg `
    --docker-registry-server-user $acrAdmin `
    --docker-registry-server-password $acrPassword `
    --deployment-container-image-name "$($config.acr.name).$($config.acr.domain)/$($node.tag):latest"

az webapp deployment container config `
    --resource-group $rg `
    --name $app `
    --enable-cd $true

$hook = (az webapp deployment container show-cd-url `
    --resource-group $rg `
    --name $app `
    --query "CI_CD_URL" `
    --output tsv)

Write-Host "ACR Hook: $hook"

Set-Location $origin