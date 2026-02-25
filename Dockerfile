# Use the official .NET 8.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8.0 SDK as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["ConferenceApp.csproj", "."]
RUN dotnet restore "ConferenceApp.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "ConferenceApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConferenceApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "ConferenceApp.dll"]