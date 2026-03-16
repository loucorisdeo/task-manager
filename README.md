# Task Manager

## Overview

Task Manager is a multi-tier task management system that demonstrates a full-stack architecture using ASP.NET Core, React, and WinForms.

The application provides a REST API backend with two client interfaces:

* A React single-page web application
* A WinForms desktop client

Both clients authenticate using JWT and interact with the same API to retrieve and update tasks.

This project demonstrates clean architecture, separation of concerns, and support for multiple client types.

---

## Features

### Authentication

* JWT-based authentication
* Secure API endpoints

### Task Management

* View tasks
* Mark tasks complete
* Multi-tenant task isolation

### Web Client (React)

* Login screen
* Task dashboard
* Responsive SPA interface

### Desktop Client (WinForms)

* Login using JWT authentication
* Task dashboard using DataGridView
* Ability to refresh and complete tasks

---

## Technology Stack

### Backend

* ASP.NET Core
* Entity Framework Core
* SQL database (SQLite or SQL Server depending on configuration)
* JWT Authentication

### Frontend

* React
* JavaScript (JSX)
* Axios HTTP client

### Desktop Client

* .NET WinForms
* HttpClient for API communication

### Other

* REST API
* JSON serialization
* Git version control

---

## Solution Structure

```
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
└─ TaskManager.sln
```

---

## Solution Architecture

```
                    +----------------------+
                    |     React SPA        |
                    |   TaskManager.Client |
                    +----------+-----------+
                               |
                               | HTTP / JSON
                               |
                    +----------v-----------+
                    |      ASP.NET API     |
                    |    TaskManager.Api   |
                    +----------+-----------+
                               |
                    Application Services
                               |
                    +----------v-----------+
                    | TaskManager.Application |
                    |   Business Logic       |
                    +----------+-----------+
                               |
                         Data Access
                               |
                    +----------v-----------+
                    | TaskManager.Infrastructure |
                    | EF Core / Database        |
                    +----------+-----------+
                               |
                           Database

                    +----------------------+
                    |  WinForms Desktop    |
                    | TaskManager.Desktop  |
                    +----------+-----------+
                               |
                         HTTP / JSON
                               |
                        Same REST API
```

Both clients communicate with the same REST API ensuring consistent business logic and validation.

---

## Design Decisions

### Layered Architecture

The project follows a layered architecture to separate responsibilities.

```
Domain → Application → Infrastructure → API → Clients
```

Benefits:

* clear separation of concerns
* maintainable codebase
* easier testing and extension

---

### API-First Design

The React SPA and WinForms client both interact with the same REST API.

Benefits:

* centralized business logic
* consistent validation
* support for multiple client applications

---

### JWT Authentication

Authentication is implemented using JSON Web Tokens.

Workflow:

1. Client sends credentials to `/api/auth/login`
2. API validates the user
3. API returns a JWT token
4. Client includes the token in subsequent requests

Example header:

```
Authorization: Bearer {token}
```

---

### Shared Domain Models

Domain entities are defined in **TaskManager.Domain** and reused across the application layers.

Benefits:

* avoids duplicate models
* keeps business entities centralized
* improves maintainability

---

## Running the Project

### 1. Clone the repository

```
git clone <repository-url>
cd TaskManager
```

---

### 2. Run the API

```
cd TaskManager.Api
dotnet run
```

The API will start on ports defined in:

```
TaskManager.Api/Properties/launchSettings.json
```

Default development URL:

```
https://localhost:7015
```

---

### 3. Run the React client

Create a `.env` file inside `TaskManager.Client`:

```
VITE_API_URL=https://localhost:7015
```

Then start the client:

```
cd TaskManager.Client
npm install
npm run dev
```

The SPA will start on a local development server.

---

### 4. Run the WinForms desktop client

Open the solution in Visual Studio or run:

```
cd TaskManager.Desktop
dotnet run
```

After logging in, the dashboard will display tasks retrieved from the API.

---

## API Endpoints

### Authentication

```
POST /api/auth/login
```

### Tasks (JWT required)

```
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
PUT    /api/tasks/{id}/complete
DELETE /api/tasks/{id}
```

Notes:

* `GET /api/tasks` returns tasks for the authenticated user's tenant
* `GET /api/tasks/{id}` returns a single task for the authenticated user's tenant
* `POST /api/tasks` creates a task for the authenticated user's tenant
* `PUT /api/tasks/{id}` updates a task for the authenticated user's tenant
* `PUT /api/tasks/{id}/complete` marks a task as completed
* `DELETE /api/tasks/{id}` requires the **Admin** role
* Task access is tenant-scoped using the `tenantId` claim from the JWT

---

## Example Workflow

1. User logs in through the web client or desktop application
2. The API returns a JWT token
3. The token is stored by the client
4. All subsequent requests include the JWT token
5. The user can view and complete tasks

---

## Assumptions

* Users are authenticated via JWT tokens issued by the API
* Tasks are tenant-scoped using the `tenantId` claim
* Task deletion is restricted to users with the `Admin` role

---

## Possible Future Improvements

With additional time the following improvements could be added:

* task creation and editing in the desktop client
* pagination and filtering for large task lists
* improved UI styling
* integration tests for API endpoints
* role-based authorization
* containerized deployment using Docker

---

## Author

Louis Corisdeo
Senior Software Engineer
