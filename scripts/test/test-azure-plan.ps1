param(
    [string]
    [Parameter(Mandatory)]
    $Admin,
    [SecureString]
    [Parameter(Mandatory)]
    $Password
)

$prefix="jps-distributed"
# region
$location="eastus"
# resource group
$rg="$prefix-rg"
# sql server
$sql="$prefix-sql"
# database
$db="$prefix-db"
# firewall rule
$fw="HomeIP"
# app plan
$plan="$prefix-plan"
# web app
$app="$prefix-app"

Write-Host "Setting public IP address"
$ip = Invoke-RestMethod "ifcfg.me"

Write-Host "Creating resource gruop $rg"
az group create `
    --location $location `
    --name $rg `

Write-Host "Creating SQL Server $sql"
az sql server create `
    --name $sql `
    --resource-group $rg `
    --location $location `
    --admin-user $Admin `
    --admin-password $Password

Write-Host "Creating SQL firewall rule $fw"
az sql server firewall-rule create `
    --server $sql `
    --resource-group $rg `
    --name $fw `
    --start-ip-address $ip `
    --end-ip-address $ip

Write-Host "Creating SQL database $db"
az sql db create `
    --server $sql `
    --resource-group $rg `
    --name $db `
    --service-objective S0

Write-Host "Setting SQL database connection string"
$cs=(az sql db show-connection-string `
    --name $db `
    --server $sql `
    --client ado.net `
    --output tsv)

$cs=$cs.Replace('<username>', $Admin).Replace('<password>', $Password)

Write-Host "Creating app service plan"
az appservice plan create `
    --name $plan `
    --resource-group $rg `
    --location $location

Write-Host "Creating a web app"
az webapp create `
    --name $app `
    --plan $plan `
    --resource-group $rg

Write-Host "Setting web app connection string"
az webapp config connection-string set `
    --name $app `
    --resource-group $rg `
    --connection-string-type SQLServer `
    --settings "App=$cs"