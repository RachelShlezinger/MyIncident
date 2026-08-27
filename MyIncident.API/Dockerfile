FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG CACHE_BUST=1
WORKDIR /src
COPY ["MyIncident.API.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:EnableCompressionInSingleFile=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_GCHeapHardLimit=0x10000000
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "MyIncident.API.dll"]
