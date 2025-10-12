cd .\web\Hive.Gateway.Service\

dotnet user-secrets init
dotnet user-secrets set "UploadMediaClient:Cloudinary:Name" "dfcfk4gc8"
dotnet user-secrets set "UploadMediaClient:Cloudinary:Key" "551369622324283"
dotnet user-secrets set "UploadMediaClient:Cloudinary:Secret" "YvULPSsFyWMLFx2QO8ijqUaUUeo"

#[System.Environment]::SetEnvironmentVariable("UploadMediaClient__Cloudinary__Name", "...", "Machine")
#[System.Environment]::SetEnvironmentVariable("UploadMediaClient__Cloudinary__Key", "...", "Machine")
#[System.Environment]::SetEnvironmentVariable("UploadMediaClient__Cloudinary__Secret", "...", "Machine")