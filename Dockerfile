# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TimeROD.sln", "./"]
COPY ["src/TimeROD.API/TimeROD.API.csproj", "src/TimeROD.API/"]
COPY ["src/TimeROD.Core/TimeROD.Core.csproj", "src/TimeROD.Core/"]
COPY ["src/TimeROD.Infrastructure/TimeROD.Infrastructure.csproj", "src/TimeROD.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TimeROD.sln"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/TimeROD.API"
RUN dotnet build "TimeROD.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TimeROD.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy published files
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TimeROD.API.dll"]
