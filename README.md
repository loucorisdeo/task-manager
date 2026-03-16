# Task Manager

## Overview

Task Manager is a multi-tier task management system that demonstrates a
full-stack architecture using ASP.NET Core, React, and WinForms.

The application provides a REST API backend with two client interfaces:

-   A React single-page web application
-   A WinForms desktop client

Both clients authenticate using JWT and interact with the same API to
retrieve and update tasks.

This project demonstrates clean architecture, separation of concerns,
and support for multiple client types.

------------------------------------------------------------------------

## Features

### Authentication

-   JWT-based authentication
-   Secure API endpoints

### Task Management

-   View tasks
-   Mark tasks complete
-   Multi-tenant task isolation

### Web Client (React)

-   Login screen
-   Task dashboard
-   Responsive SPA interface

### Desktop Client (WinForms)

-   Login using JWT authentication
-   Task dashboard using DataGridView
-   Ability to refresh and complete tasks

------------------------------------------------------------------------

## Technology Stack

### Backend

-   ASP.NET Core
-   Entity Framework Core
-   SQLite database
-   JWT Authentication

### Frontend

-   React
-   JavaScript (JSX)
-   Axios HTTP client

### Desktop Client

-   .NET WinForms
-   HttpClient for API communication

### Other

-   REST API
-   JSON serialization
-   Git version control

------------------------------------------------------------------------

## Solution Structure

    TaskManager
    │
    ├─ TaskManager.Domain
    │    Domain entities and core models
    │
    ├─ TaskManager.Application
    │    Business logic and application services
    │
    ├─ TaskManager.Infrastructure
    │    Data access and persistence
    │
    ├─ TaskManager.Api
    │    ASP.NET Core REST API
    │
    ├─ TaskManager.Client
    │    React SPA frontend
    │
    ├─ TaskManager.Desktop
    │    WinForms desktop client
    │
    ├─ TaskManager.Tests
    │    Unit tests
    │
    └─ TaskManager.sln

------------------------------------------------------------------------

## Running the Project

### 1. Clone the repository

    git clone <repository-url>
    cd TaskManager

------------------------------------------------------------------------

### 2. Database Setup

The API uses Entity Framework Core with SQLite for local development.

When the API starts, Entity Framework automatically applies migrations
and creates the database if it does not already exist.

The SQLite database file will be created at:

    TaskManager.Api/taskmanager.db

If you need to create or update the database manually:

    cd TaskManager.Api
    dotnet ef database update

If you want to reset the database:

1.  Delete the database file

    TaskManager.Api/taskmanager.db

2.  Run the API again

    dotnet run

The database will be recreated automatically.

------------------------------------------------------------------------

### 3. Run the API

    cd TaskManager.Api
    dotnet run

The API will start on ports defined in:

    TaskManager.Api/Properties/launchSettings.json

Default development URL:

    https://localhost:7015

------------------------------------------------------------------------

### 4. Run the React client

The React client is preconfigured to connect to the API running at:

    https://localhost:7015

Start the client:

    cd TaskManager.Client
    npm install
    npm run dev

The SPA will start on a local development server.

------------------------------------------------------------------------

### 5. Run the WinForms desktop client

Open the solution in Visual Studio or run:

    cd TaskManager.Desktop
    dotnet run

After logging in, the dashboard will display tasks retrieved from the
API.

------------------------------------------------------------------------

## API Endpoints

### Authentication

    POST /api/auth/login

### Tasks (JWT required)

    GET    /api/tasks
    GET    /api/tasks/{id}
    POST   /api/tasks
    PUT    /api/tasks/{id}
    PUT    /api/tasks/{id}/complete
    DELETE /api/tasks/{id}

Notes:

-   GET /api/tasks returns tasks for the authenticated user's tenant
-   GET /api/tasks/{id} returns a single task for the authenticated
    user's tenant
-   POST /api/tasks creates a task for the authenticated user's tenant
-   PUT /api/tasks/{id} updates a task for the authenticated user's
    tenant
-   PUT /api/tasks/{id}/complete marks a task as completed
-   DELETE /api/tasks/{id} requires the Admin role
-   Task access is tenant-scoped using the tenantId claim from the JWT

------------------------------------------------------------------------

## Example Workflow

1.  User logs in through the web client or desktop application
2.  The API returns a JWT token
3.  The token is stored by the client
4.  All subsequent requests include the JWT token
5.  The user can view and complete tasks

------------------------------------------------------------------------

## Assumptions / Simplifications

-   Users are authenticated via JWT tokens issued by the API
-   Tasks are tenant-scoped using the tenantId claim
-   Task deletion is restricted to users with the Admin role
-   SQLite used for simplicity
-   passwords are stored and validated in a simplified way for demo
    purposes
-   desktop app focuses on core flows rather than full feature parity

------------------------------------------------------------------------

## Possible Future Improvements

With additional time the following improvements could be added:

-   task creation and editing in the desktop client
-   pagination and filtering for large task lists
-   improved UI styling
-   integration tests for API endpoints
-   role-based authorization improvements
-   containerized deployment using Docker

------------------------------------------------------------------------

## Testing

The solution includes unit tests using xUnit, Moq, FluentAssertions, and
EF Core InMemory.

Covered areas include:

-   authentication controller behavior
-   task controller behavior
-   authentication service logic
-   task service logic

Run tests with:

    dotnet test

------------------------------------------------------------------------

## Author

Louis Corisdeo Senior Software Engineer
