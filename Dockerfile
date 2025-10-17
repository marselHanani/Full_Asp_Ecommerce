# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY . .
RUN dotnet restore "Ecommerce.API/Ecommerce.API.csproj"

# Build and publish the project
RUN dotnet publish "Ecommerce.API/Ecommerce.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose default ASP.NET port
EXPOSE 80
EXPOSE 443

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="Production"

# Start the application
ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]