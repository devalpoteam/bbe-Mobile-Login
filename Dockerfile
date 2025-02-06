# Usar la imagen oficial de .NET 9 como base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Imagen para compilar y restaurar dependencias
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de la solución
COPY ["AutService.JwAuthLogin.Api/AutService.JwAuthLogin.Api.csproj", "AutService.JwAuthLogin.Api/"]
COPY ["AutService.JwAuthLogin.Application/AutService.JwAuthLogin.Application.csproj", "AutService.JwAuthLogin.Application/"]
COPY ["AutService.JwAuthLogin.Domain/AutService.JwAuthLogin.Domain.csproj", "AutService.JwAuthLogin.Domain/"]
COPY ["AutService.JwAuthLogin.Infrastructure/AutService.JwAuthLogin.Infrastructure.csproj", "AutService.JwAuthLogin.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "AutService.JwAuthLogin.Api/AutService.JwAuthLogin.Api.csproj"

# Copiar todo el código y compilar
COPY . .
WORKDIR "/src/AutService.JwAuthLogin.Api"
RUN dotnet build -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Imagen final para ejecutar la aplicación
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutService.JwAuthLogin.Api.dll"]
