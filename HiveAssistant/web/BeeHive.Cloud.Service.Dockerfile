FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BeeHive.Cloud.Service.Client/BeeHive.Cloud.Service.Client.csproj", "BeeHive.Cloud.Service.Client/"]
COPY ["BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj", "BeeHive.Cloud.Service/"]
RUN dotnet restore "BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj"
COPY . .
RUN dotnet build "BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeeHive.Cloud.Service.dll"]