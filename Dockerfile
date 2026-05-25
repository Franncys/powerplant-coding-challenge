# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the full repository and restore dependencies.
COPY . .
RUN dotnet restore PowerPlantCodingChallenge.slnx

# Publish the API project in Release mode.
RUN dotnet publish src/PowerPlantCodingChallenge.Api/PowerPlantCodingChallenge.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# The challenge expects the API to be exposed on port 8888.
ENV ASPNETCORE_URLS=http://+:8888
EXPOSE 8888

ENTRYPOINT ["dotnet", "PowerPlantCodingChallenge.Api.dll"]