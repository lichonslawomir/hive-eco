# Configurable variables
$serviceName = "Hive.Gateway.Service"
$publishDir = "C:\hive\gateway"
$projectPath = ".\web\Hive.Gateway.Service\Hive.Gateway.Service.csproj"
$publishProfile = "FolderProfile"
$url = "http://*:5000"

# Stop the service if it exists
if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
    Write-Host "Stopping service $serviceName..."
    Stop-Service -Name $serviceName -Force
    Start-Sleep -Seconds 3
    sc delete $serviceName
    Start-Sleep -Seconds 3
}

# Publish the application
Write-Host "Publishing .NET application..."
dotnet publish $projectPath -c Debug -p:PublishProfile=$publishProfile -o $publishDir

# Install the service if not already installed
if (-not (Get-Service -Name $serviceName -ErrorAction SilentlyContinue)) {
    Write-Host "Installing service $serviceName..."
    New-Service -Name $serviceName `
      -BinaryPathName "$publishDir\Hive.Gateway.Service.exe" `
      -DisplayName "Hive Gateway Service" `
      -StartupType Automatic
    Start-Sleep -Seconds 2
} else {
    Write-Host "Service $serviceName already installed."
}

# Start the service
Write-Host "Starting service $serviceName..."
Start-Service -Name $serviceName
Write-Host "Done."