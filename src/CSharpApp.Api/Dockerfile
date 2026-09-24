# -------------------------
# Build
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy csproj files first for Docker layer caching
COPY ["src/CSharpApp.Api/CSharpApp.Api.csproj", "CSharpApp.Api/"]
COPY ["src/CSharpApp.Application/CSharpApp.Application.csproj", "CSharpApp.Application/"]
COPY ["src/CSharpApp.Core/CSharpApp.Core.csproj", "CSharpApp.Core/"]
COPY ["src/CSharpApp.Infrastructure/CSharpApp.Infrastructure.csproj", "CSharpApp.Infrastructure/"]

RUN dotnet restore "CSharpApp.Api/CSharpApp.Api.csproj"

# Copy source code
COPY src/ .

WORKDIR "/src/CSharpApp.Api"

RUN dotnet publish "CSharpApp.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# -------------------------
# Runtime
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "CSharpApp.Api.dll"]