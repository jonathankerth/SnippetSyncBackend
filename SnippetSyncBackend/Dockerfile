# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official ASP.NET Core SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SnippetSyncBackend.csproj", "./"]
RUN dotnet restore "SnippetSyncBackend.csproj"
COPY . .
RUN dotnet build "SnippetSyncBackend.csproj" -c Release -o /app/build

# Publish the output
RUN dotnet publish "SnippetSyncBackend.csproj" -c Release -o /app/publish

# Use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY snippetsync.db .
ENTRYPOINT ["dotnet", "SnippetSyncBackend.dll"]
