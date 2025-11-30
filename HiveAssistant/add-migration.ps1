param (
    [Parameter(Mandatory = $true)]
    [string]$ModuleName,

    [Parameter(Mandatory = $true)]
    [string]$MigrationName
)

$BasePath = Join-Path $PSScriptRoot ".\web"
$StartupProjectPath = Join-Path $BasePath "EfMigrationTools"
$OutputDirPath = "Migrations"
$Configuration = "Debug"

switch ($ModuleName) {
    "BeeHive" {
        $env:ASPNETCORE_ENVIRONMENT="Sqlite"
        $ProjectPath = Join-Path $BasePath "BeeHive.Infra.Sqlite"
        dotnet ef migrations add $MigrationName --context BeeHiveDbContext --project $ProjectPath --startup-project $StartupProjectPath --output-dir $OutputDirPath --configuration $Configuration

        $env:ASPNETCORE_ENVIRONMENT="Postgres"
        $ProjectPath = Join-Path $BasePath "BeeHive.Infra.Postgres"
        dotnet ef migrations add $MigrationName --context BeeHiveDbContext --project $ProjectPath --startup-project $StartupProjectPath --output-dir $OutputDirPath --configuration $Configuration
    }
    default {
        Write-Host "Error: Invalid ModuleName. Please specify 'BeeHive'..."
        exit 1
    }
}