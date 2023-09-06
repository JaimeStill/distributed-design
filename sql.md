# GitHub Codespaces SQL Server Configuration

1. F1 -> Codespaces: Add Dev Container Configuration Files...

2. Create a new configuration...

3. C# and MS SQL

4. Dotnet CLI

## devcontainer.json Modifications

```json
{
    "customizations": {
        "vscode": {
            "extensions": [
	        "ms-dotnettools.csdevkit",
	        "ms-mssql.mssql",
	        "PKief.material-icon-theme"
            ],
        }
    },
    "forwardPorts": [1433]
}
```

## Connection Strings

```json
"ConnectionStrings": {
    "App": "Server=localhost,1433;Encrypt=Mandatory;TrustServerCertificate=True;User=sa;Password=P@ssw0rd;Database={db}"
}
```

```json
"ConnectionStrings": {
    "App": "Server=localhost,1433;Encrypt=Mandatory;TrustServerCertificate=True;User=sa;Password=P@ssw0rd;Database=distributed-proposals"
}
```

```json
"ConnectionStrings": {
    "App": "Server=localhost,1433;Encrypt=Mandatory;TrustServerCertificate=True;User=sa;Password=P@ssw0rd;Database=distributed-workflows"
}
```

