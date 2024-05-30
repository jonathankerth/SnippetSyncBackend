# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Install necessary utilities
RUN apt-get update && apt-get install -y curl net-tools

# Use the official ASP.NET Core SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SnippetSyncBackend.csproj", "."]
RUN dotnet restore "SnippetSyncBackend.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "SnippetSyncBackend.csproj" -c Release -o /app/build

# Use the build image to publish the output
FROM build AS publish
RUN dotnet publish "SnippetSyncBackend.csproj" -c Release -o /app/publish

# Use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY snippetsync.db /app/snippetsync.db
ENTRYPOINT ["dotnet", "SnippetSyncBackend.dll"]
