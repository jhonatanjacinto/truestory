# docker file for building the Truestory WebApi project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet publish "./Truestory.WebApi/Truestory.WebApi.csproj" -c Release -o /publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Truestory.WebApi.dll", "--urls", "http://0.0.0.0:80"]