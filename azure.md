# Azure Deployment Planning

## Variables

```powershell
$prefix="jps-distributed"
# region
$location="eastus"
# public IP Address
$ip=Invoke-RestMethod "ifcfg.me"
# resource group
$rg="$prefix-rg"
# sql server
$sql="$prefix-sql"
# database
$db="$prefix-db"
# admin
$dbAdmin="sa-sql"
# password
$dbPw="[password]"
# firewall rule
$fw="HomeIP"
# app plan
$plan="$prefix-plan"
# web app
$app="$prefix-app"
```

## Notes

Capturing an object into a variable:

```powershell
$user=(az ad signed-in-user show | ConvertFrom-Json)
```

Azure SQL Database service objectives are based on the DTU purchase model. See [Azure SQL Database Pricing Details](https://azure.microsoft.com/en-us/pricing/details/azure-sql-database/single/) and ensure *Purchase Model* is set to **DTU**.