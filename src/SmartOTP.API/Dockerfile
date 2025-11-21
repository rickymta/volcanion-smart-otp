FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/SmartOTP.API/SmartOTP.API.csproj", "src/SmartOTP.API/"]
COPY ["src/SmartOTP.Application/SmartOTP.Application.csproj", "src/SmartOTP.Application/"]
COPY ["src/SmartOTP.Domain/SmartOTP.Domain.csproj", "src/SmartOTP.Domain/"]
COPY ["src/SmartOTP.Infrastructure/SmartOTP.Infrastructure.csproj", "src/SmartOTP.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/SmartOTP.API/SmartOTP.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/SmartOTP.API"
RUN dotnet build "SmartOTP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartOTP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartOTP.API.dll"]
