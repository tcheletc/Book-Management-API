# -------- Stage 1: Build the application --------
# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Book-Management-API.sln ./
COPY BookApi/*.csproj ./BookApi/
COPY Tests/*.csproj ./Tests/

# Restore NuGet packages for all projects
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the main API project in Release mode
WORKDIR /src/BookApi
RUN dotnet publish -c Release -o /app/publish

# -------- Stage 2: Create the runtime image --------
# Use a smaller ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose port 80 to access the API
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "BookApi.dll"]
