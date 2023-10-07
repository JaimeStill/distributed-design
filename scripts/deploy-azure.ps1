param(
    [string]
    [Parameter(Mandatory)]
    $Configuration,
    [string]
    [Parameter(Mandatory)]
    $SqlAdmin,
    [SecureString]
    [Parameter(Mandatory)]
    $SqlPassword
)

# load core functions
. .\functions.ps1

# variable initialization
$origin = $pwd
$config = GetJson $Configuration
$ip = GetPublicIp
$rg = MergeName $config.prefix $config.resourceGroup
$plan = MergeName $config.prefix $config.appPlan.name
$sql = MergeName $config.prefix $config.sql.server

Set-Location $config.root

# resource group
& az group create `
    --location $config.location `
    --name $rg

# azure container registry
& az acr create `
    --resource-group $rg `
    --name $config.acr.name `
    --sku $config.acr.sku `
    --admin-enabled $true

$acrAdmin = (az acr credential show `
    --name $config.acr.name `
    --query "username" `
    --output tsv)

$acrPassword = (az acr credential show `
    --name $config.acr.name `
    --query "passwords[0].value" `
    --output tsv)

# appservice plan
& az appservice plan create `
    --name $plan `
    --resource-group $rg `
    --sku $config.appPlan.sku `
    --is-linux

# # sql server
# & az sql server create `
#     --name $sql `
#     --resource-group $rg `
#     --location $config.location `
#     --admin-user $SqlAdmin `
#     --admin-password (GetSecureString $SqlPassword)

# # sql firewall rule
# & az sql server firewall-rule create `
#     --server $sql `
#     --resource-group $rg `
#     --name $config.sql.firewallRule `
#     --start-ip-address $ip `
#     --end-ip-address $ip

foreach ($node in $config.nodes.projects) {
    # initialize variables
    $app = MergeName $config.prefix "-$($node.tag)"

    # build and deploy image
    az acr build `
        --registry $config.acr.name `
        --image $node.tag `
        --file $node.dockerfile `
        $config.nodes.path

    # deploy image to app service
    az webapp create `
        --name $app `
        --plan $plan `
        --resource-group $rg `
        --docker-registry-server-user $acrAdmin `
        --docker-registry-server-password $acrPassword `
        --deployment-container-image-name "$($config.acr.name).$($config.acr.domain)/$($node.tag):latest"

    # configure webapp logging
    az webapp log config `
        --name $app `
        --resource-group $rg `
        --web-server-logging filesystem

    # configure continuous deployment
    az webapp deployment container config `
        --resource-group $rg `
        --name $app `
        --enable-cd $true

    # configure webhooks
    $hook = (az webapp deplyoment container show-cd-url `
        --resource-group $rg `
        --name $app `
        --query "CI_CD_URL" `
        --output tsv)

    az acr webhook create `
        --resource-group $rg `
        --registry $config.acr.name `
        --name $node.hook `
        --uri $hook `
        --actions push
}

Set-Location $origin