# docker file for building the Truestory Front-end project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet publish "./Truestory.Frontend/Truestory.Frontend.csproj" -c Release -o /publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Truestory.Frontend.dll", "--urls", "http://0.0.0.0:80"]