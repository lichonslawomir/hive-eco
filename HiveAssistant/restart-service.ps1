# Configurable variables
$serviceName = "Hive.Gateway.Service"
$publishDir = "C:\hive\gateway"
$projectPath = ".\web\Hive.Gateway.Service\Hive.Gateway.Service.csproj"
$publishProfile = "FolderProfile"
$url = "http://*:5000"

New-Service -Name $serviceName `
      -BinaryPathName "$publishDir\Hive.Gateway.Service.exe --urls $url" `
      -DisplayName "Hive Gateway Service" `
      -StartupType Automatic