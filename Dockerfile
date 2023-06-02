# Set the base image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the project files to the container
COPY . ./

# Restore dependencies and build the project
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Set the runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# Set the working directory
WORKDIR /app

# Copy the published output from the build stage to the runtime image
COPY --from=build-env /app/out .

# Expose the port that the web API will listen on
EXPOSE 9285

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ThirdAssignment-Server.dll"]
