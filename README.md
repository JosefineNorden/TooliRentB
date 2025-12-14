# TooliRentB – Tools Rental Web API

TooliRentB is a tools rental and booking system built as an ASP.NET Core Web API.
The API is designed according to a 4-layer N-tier architecture and is documented and tested using Swagger (OpenAPI).

Authentication is handled using JWT (JSON Web Tokens) together with ASP.NET Identity.
Authorization is role-based with the following roles:

- **Admin** – Full access to administration, tools, customers, rentals, and statistics
- **Member** – Can book tools, manage own rentals, and view own customer profile

---

## Architecture (4-Layer N-tier)

The solution follows a clean 4-layer N-tier architecture:

### Core
Domain layer containing:
- Entity models (Tool, Customer, Rental, RentalDetail, Category)
- Enums (ToolStatus, CustomerStatus)
- BaseEntity with common fields

### Infrastructure
Data access layer responsible for:
- Entity Framework Core
- DbContexts
- Repositories
- Database migrations
- ASP.NET Identity persistence

### Services
Application layer containing:
- Business logic
- DTOs
- Validation (FluentValidation)
- AutoMapper profiles
- Service interfaces and implementations

### API
Presentation layer exposing REST endpoints:
- Controllers
- JWT authentication
- Role-based authorization
- Swagger/OpenAPI documentation

---

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Identity
- JWT Authentication
- Swagger / OpenAPI
- AutoMapper
- FluentValidation
- SQL Server

---

## Authentication & Authorization

- JWT Bearer authentication is configured in the API.
- Role-based authorization is enforced using `[Authorize]` and `[Authorize(Roles = "...")]`.

### Roles
- **Admin**
  - Manage tools, customers, rentals
  - View admin statistics
  - Manage users and roles
- **Member**
  - Book tools
  - View and manage own rentals
  - View own customer profile

In Swagger:
1. Call `/api/Auth/login`
2. Copy the `accessToken`
3. Click **Authorize** and enter: `Bearer <token>`

---

## API Controllers Overview


---

### AdminSummaryController (Admin only)
**Route:** `/api/AdminSummary`

| Method | Endpoint | Description |
|------|---------|-------------|
| GET | `/summary` | Get overall system statistics |
| GET | `/top-tools` | Get top rented tools (optional date range) |

All endpoints require the **Admin** role.

---

### AuthController
**Route:** `/api/Auth`

| Method | Endpoint | Description | Access |
|------|---------|-------------|--------|
| POST | `/login` | Login and receive JWT + refresh token | Public |
| POST | `/refresh` | Refresh access token | Public |
| POST | `/register-member` | Register new member | Public |
| POST | `/register-admin` | Register new admin | Admin |
| GET | `/users/admins` | List admin users | Admin |
| GET | `/users/members` | List member users | Admin |
| PUT | `/deactivate/{email}` | Deactivate user | Admin |
| PUT | `/activate/{email}` | Activate user | Admin |
| DELETE | `/delete/{email}` | Delete user | Admin |
| GET | `/me` | Get current user info | Authenticated |

**Note:**  
When registering a Member, the system automatically creates or updates a corresponding Customer record.

---

### CustomerController
**Route:** `/api/Customer`

| Method | Endpoint | Description | Access |
|------|---------|-------------|--------|
| GET | `/` | Get all customers | Admin |
| GET | `/{id}` | Get customer by ID | Admin |
| POST | `/` | Create customer | Admin |
| PUT | `/{id}` | Update customer | Admin |
| DELETE | `/{id}` | Delete customer | Admin |
| GET | `/me` | Get own customer profile | Admin, Member |

**Note:**  
Deleting a customer may fail if related rentals exist due to database foreign key constraints.  
Recommended approach is to deactivate the user/customer instead.


---

### RentalController
**Route:** `/api/Rental`

> All endpoints require authentication. Ownership and role checks are enforced in the service layer.

| Method | Endpoint | Description | Access |
|------|---------|-------------|--------|
| GET | `/Get All Rentals (admin)` | Get all rentals | Admin |
| GET | `/{id}/ Get rental by Id` | Get rental by ID | Owner / Admin |
| POST | `/Book tools for customer` | Admin books for customer | Admin |
| POST | `/book` | Member books tools | Member |
| PUT | `/{id}/ Update rental` | Update rental | Owner / Admin |
| PATCH | `/{id}/pickup` | Mark rental as picked up | Admin, Member |
| PATCH | `/{id}/return` | Mark rental as returned | Admin, Member |
| GET | `/my` | Get own rentals | Authenticated |
| GET | `/overdue` | Get overdue rentals | Admin |
| DELETE | `/{id}/cancel` | Cancel rental | Owner / Admin |
| DELETE | `/{id}` | Delete rental | Admin |

**Important:**  
The `{id}` used in pickup, return, cancel, update, and delete endpoints refers to the **Rental ID** returned when a booking is created — not CustomerId or ToolId.


---

### ToolController
**Route:** `/api/Tool`

> Write operations are restricted to **Admin**.  
> Only the tool list endpoint is public.

| Method | Endpoint | Description | Access |
|------|---------|-------------|--------|
| GET | `/` | Get all tools (with filters) | Public |
| GET | `/{id}` | Get tool by ID | Admin |
| GET | `/{id}/available` | Check availability | Admin |
| GET | `/status/{status}` | Get tools by status | Admin |
| POST | `/` | Create tool | Admin |
| PUT | `/{id}` | Update tool | Admin |
| DELETE | `/{id}` | Delete tool | Admin |

---

## Run Locally

### Requirements
- .NET SDK
- SQL Server
- EF Core CLI (recommended)

### Configuration

`appsettings.json` example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TooliRentB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS",
    "Issuer": "TooliRent",
    "Audience": "TooliRent"
  }
}

Apply migrations:

dotnet ef database update || dotnet run
