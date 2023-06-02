# Set the base image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["ThirdAssignment-Server.csproj", "."]
RUN dotnet restore "ThirdAssignment-Server.csproj"

# Copy the remaining source code
COPY . .

# Build the application
RUN dotnet build "ThirdAssignment-Server.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ThirdAssignment-Server.csproj" -c Release -o /app/publish

# Set the final image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app

# Copy the published files
COPY --from=publish /app/publish .

# Set the environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:9285
ENV CONTENT_ROOT_PATH /app

# Expose the port that the application listens on
EXPOSE 9285

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ThirdAssignment-Server.dll"]
