param (
    [Parameter(Mandatory = $true)]
    [string]$ModuleName,

    [Parameter(Mandatory = $true)]
    [string]$MigrationName
)

$BasePath = Join-Path $PSScriptRoot ".\web"
$StartupProjectPath = Join-Path $BasePath "EfMigrationTools"
$OutputDirPath = "DataAccess\Migrations"
$Configuration = "Debug"

switch ($ModuleName) {
    "BeeHive" {
        $ProjectPath = Join-Path $BasePath "BeeHive.Infra"
        dotnet ef migrations add $MigrationName --context BeeHiveDbContext --project $ProjectPath --startup-project $StartupProjectPath --output-dir $OutputDirPath --configuration $Configuration
    }
    default {
        Write-Host "Error: Invalid ModuleName. Please specify 'BeeHive'..."
        exit 1
    }
}