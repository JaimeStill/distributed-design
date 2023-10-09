param(
    [string]
    [Parameter(Mandatory)]
    $Configuration,
    [string]
    [Parameter(Mandatory)]
    $SqlAdmin,
    [SecureString]
    [Parameter(Mandatory)]
    $SqlPassword,
    [string]
    [Parameter()]
    $Root = '..\'
)

# load core functions
. .\functions.ps1

# variable initialization
$origin = $pwd
$config = Get-Json $Configuration
$ip = Get-PublicIp
$rg = Merge-Name $config.prefix $config.resourceGroup
$plan = Merge-Name $config.prefix $config.appPlan.name
$sql = Merge-Name $config.prefix $config.sql.server

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

$acrPassword = (az acr credential show `
        --name $config.acr.name `
        --query "passwords[0].value" `
        --output tsv)

# appservice plan
az appservice plan create `
    --name $plan `
    --resource-group $rg `
    --sku $config.appPlan.sku `
    --is-linux

# sql server
az sql server create `
    --name $sql `
    --resource-group $rg `
    --location $config.location `
    --admin-user $SqlAdmin `
    --admin-password (Get-SecureString $SqlPassword)

# sql firewall rule: Home IP Address
az sql server firewall-rule create `
    --server $sql `
    --resource-group $rg `
    --name $config.sql.firewallRule `
    --start-ip-address $ip `
    --end-ip-address $ip

# sql firewall rule: Azure Services
az sql server firewall-rule create `
    --server $sql `
    --resource-group $rg `
    --name "Azure Services" `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

foreach ($node in $config.nodes.projects) {
    # initialize variables
    $name = Merge-Name $config.prefix "-$($node.tag)"
    $appsettings = Get-Json $node.appsettings

    # build and deploy image
    az acr build `
        --registry $config.acr.name `
        --image $node.tag `
        --file $node.dockerfile `
        $config.nodes.path

    # deploy image to app service
    az webapp create `
        --name $name `
        --plan $plan `
        --resource-group $rg `
        --docker-registry-server-user $acrAdmin `
        --docker-registry-server-password $acrPassword `
        --deployment-container-image-name "$($config.acr.name).$($config.acr.domain)/$($node.tag):latest"

    # configure webapp logging
    az webapp log config `
        --name $name `
        --resource-group $rg `
        --web-server-logging filesystem

    # configure continuous deployment
    az webapp deployment container config `
        --resource-group $rg `
        --name $name `
        --enable-cd $true

    # get webhook URL
    $hook = (az webapp deployment container show-cd-url `
            --resource-group $rg `
            --name $name `
            --query "CI_CD_URL" `
            --output tsv)

    # register webhook with acr
    az acr webhook create `
        --resource-group $rg `
        --registry $config.acr.name `
        --name $node.hook `
        --uri $hook `
        --actions push

    # add cors with origins
    az webapp cors add `
        --resource-group $rg `
        --name $name `
        --allowed-origins $appsettings.CorsOrigins

    # enable credentials for CORS
    az resource update `
        --name web `
        --resource-group $rg `
        --namespace Microsoft.Web `
        --resource-type config `
        --parent sites/$name `
        --set properties.cors.supportCredentials=true

    # create sql database
    az sql db create `
        --server $sql `
        --resource-group $rg `
        --name $node.db `
        --service-objective S0

    # capture connection string
    $cs = (az sql db show-connection-string `
            --name $node.db `
            --server $sql `
            --client ado.net `
            --output tsv)

    $cs = $cs.Replace('<username>', $SqlAdmin).Replace('<password>', (Get-SecureString $SqlPassword))

    # apply database migrations
    & dotnet ef database update `
        --project $node.data `
        --connection $cs

    # register connection string with webapp
    az webapp config connection-string set `
        --name $name `
        --resource-group $rg `
        --connection-string-type SQLServer `
        --settings "Node=$cs"
}

foreach ($app in $config.apps.projects) {
    # initialize variables
    $name = Merge-Name $config.prefix "-$($app.tag)"

    # build and deploy image
    az acr build `
        --registry $config.acr.name `
        --image $app.tag `
        --file $app.dockerfile `
        $config.apps.path

    # deploy image to app service
    az webapp create `
        --name $name `
        --plan $plan `
        --resource-group $rg `
        --docker-registry-server-user $acrAdmin `
        --docker-registry-server-password $acrPassword `
        --deployment-container-image-name "$($config.acr.name).$($config.acr.domain)/$($app.tag):latest"

    # configure webapp logging
    az webapp log config `
        --name $name `
        --resource-group $rg `
        --web-server-logging filesystem

    # configure continuous deployment
    az webapp deployment container config `
        --resource-group $rg `
        --name $name `
        --enable-cd $true

    # get webhook URL
    $hook = (az webapp deployment container show-cd-url `
            --resource-group $rg `
            --name $name `
            --query "CI_CD_URL" `
            --output tsv)

    # register webhook with acr
    az acr webhook create `
        --resource-group $rg `
        --registry $config.acr.name `
        --name $app.hook `
        --uri $hook `
        --actions push
}

Set-Location $origin