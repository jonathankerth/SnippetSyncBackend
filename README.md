# SnippetSyncBackend

SnippetSyncBackend is an ASP.NET Core web API for managing code snippets. It provides RESTful endpoints for CRUD operations on code snippets and their associated tags. The application uses Entity Framework Core with a SQLite database and is containerized using Docker. It is deployed on Google Cloud Platform using Google App Engine.

## Features

- **CRUD Operations**: Create, read, update, and delete code snippets.
- **Tag Management**: Associate tags with code snippets.
- **Search**: Search for code snippets by title, code, or tags.
- **Health Check**: Simple health check endpoint to verify the application's status.
- **Swagger UI**: API documentation and testing interface.

## Live Demo

The project is deployed and can be accessed [here](https://snippetsync.wl.r.appspot.com/index.html).

## Getting Started

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Google Cloud SDK](https://cloud.google.com/sdk/docs/install)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/jonathankerth/SnippetSyncBackend.git
   cd SnippetSyncBackend/SnippetSyncBackend
   ```

2. **Build and run the application locally**:
   ```bash
   dotnet build
   dotnet run
   ```

3. **Run the application in Docker**:
   ```bash
   docker build -t snippetsync-backend .
   docker run -p 80:80 snippetsync-backend
   ```

4. **Deploy to Google App Engine**:
   - **Build and push Docker image to Google Container Registry**:
     ```bash
     docker tag snippetsync-backend gcr.io/<your-gcp-project-id>/snippetsync-backend
     docker push gcr.io/<your-gcp-project-id>/snippetsync-backend
     ```

   - **Deploy using gcloud**:
     ```bash
     gcloud app deploy
     ```

## Project Structure

```
SnippetSyncBackend
├── Controllers
│   └── SnippetsController.cs  # Handles HTTP requests for code snippets
├── Data
│   └── SnippetContext.cs      # Entity Framework Core database context
├── Models
│   ├── CodeSnippet.cs         # Code snippet model
│   └── Tag.cs                 # Tag model
├── Properties
│   └── launchSettings.json    # Development launch settings
├── app.yaml                   # Google App Engine configuration
├── Dockerfile                 # Docker configuration
├── Program.cs                 # Main entry point of the application
├── SnippetSyncBackend.csproj  # Project file
└── appsettings.json           # Configuration settings
```

## API Endpoints

- **GET /api/snippets**: Retrieve all code snippets.
- **POST /api/snippets**: Create a new code snippet.
- **PUT /api/snippets/{id}**: Update an existing code snippet.
- **DELETE /api/snippets/{id}**: Delete a code snippet.
- **GET /api/snippets/search?query={query}**: Search for code snippets by title, code, or tags.
- **GET /health**: Health check endpoint.

## Configuration

- **Database**:
  - The SQLite database file is located at `snippetsync.db` in the application root directory.
  - Entity Framework Core is used to manage database access.

- **Environment Variables**:
  - `ASPNETCORE_ENVIRONMENT`: Set to `Production` for production environment.

## Docker

The application is containerized using Docker. The `Dockerfile` is set up to build and run the application in a Docker container.

### Dockerfile

```Dockerfile
# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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
```

## Deployment

The application is deployed on Google App Engine. The `app.yaml` file contains the configuration for the deployment.

### app.yaml

```yaml
runtime: custom
env: flex

env_variables:
  ASPNETCORE_ENVIRONMENT: Production

manual_scaling:
  instances: 1

readiness_check:
  path: "/health"
  check_interval_sec: 5
  timeout_sec: 4
  failure_threshold: 2
  success_threshold: 2
  app_start_timeout_sec: 1800

liveness_check:
  path: "/health"
  check_interval_sec: 5
  timeout_sec: 4
  failure_threshold: 2
  success_threshold: 2
```

## License

This project is licensed under the MIT License.

## Acknowledgements

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Google Cloud Platform](https://cloud.google.com/)
- [Docker](https://www.docker.com/)
