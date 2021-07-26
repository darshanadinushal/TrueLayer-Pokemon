#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

EXPOSE 5000
ENV ASPNETCORE_URLS http://*:5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/TrueLayer.DataProduct.Pokemon.RestService/TrueLayer.DataProduct.Pokemon.RestService.csproj", "src/TrueLayer.DataProduct.Pokemon.RestService/"]
COPY ["src/TrueLayer.DataProduct.Pokemon.Shared/TrueLayer.DataProduct.Pokemon.Shared.csproj", "src/TrueLayer.DataProduct.Pokemon.Shared/"]
COPY ["src/TrueLayer.DataProduct.Pokemon.Manager/TrueLayer.DataProduct.Pokemon.Manager.csproj", "src/TrueLayer.DataProduct.Pokemon.Manager/"]
RUN dotnet restore "src/TrueLayer.DataProduct.Pokemon.RestService/TrueLayer.DataProduct.Pokemon.RestService.csproj"
COPY . .
WORKDIR "/src/src/TrueLayer.DataProduct.Pokemon.RestService"
RUN dotnet build "TrueLayer.DataProduct.Pokemon.RestService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueLayer.DataProduct.Pokemon.RestService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueLayer.DataProduct.Pokemon.RestService.dll"]