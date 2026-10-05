# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["TicketApi/ticket-api.csproj", "TicketApi/"]
RUN dotnet restore "TicketApi/ticket-api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TicketApi"
RUN dotnet build "ticket-api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ticket-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ticket-api.dll"]