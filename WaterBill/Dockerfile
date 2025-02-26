# Use official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use the runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy built application from the build stage
COPY --from=build /app/out .

# Expose the application port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "WaterBill.dll"]
