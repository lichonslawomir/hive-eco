FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Core.Contract/Core.Contract.csproj", "Core.Contract/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["Core.App/Core.App.csproj", "Core.App/"]
COPY ["Core.Infra/Core.Infra.csproj", "Core.Infra/"]
COPY ["BeeHive.Contract/BeeHive.Contract.csproj", "BeeHive.Contract/"]
COPY ["BeeHive.Domain/BeeHive.Domain.csproj", "BeeHive.Domain/"]
COPY ["BeeHive.App/BeeHive.App.csproj", "BeeHive.App/"]
COPY ["BeeHive.Infra/BeeHive.Infra.csproj", "BeeHive.Infra/"]
COPY ["BeeHive.Cloud.Service/BeeHive.Client.Shared/BeeHive.Client.Shared.csproj", "BeeHive.Cloud.Service/BeeHive.Client.Shared/"]
COPY ["BeeHive.Cloud.Service/BeeHive.Cloud.Service.Client/BeeHive.Cloud.Service.Client.csproj", "BeeHive.Cloud.Service/BeeHive.Cloud.Service.Client/"]
COPY ["BeeHive.Cloud.Service/BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj", "BeeHive.Cloud.Service/BeeHive.Cloud.Service/"]
RUN dotnet restore "BeeHive.Cloud.Service/BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj"
COPY . .
RUN dotnet build "BeeHive.Cloud.Service/BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeeHive.Cloud.Service/BeeHive.Cloud.Service/BeeHive.Cloud.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeeHive.Cloud.Service.dll"]