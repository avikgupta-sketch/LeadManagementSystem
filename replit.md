# Lead Management System (LMS)

ASP.NET Core MVC application for managing leads with role-based access (Admin, Manager, Agent).

## Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC + Razor Views
- **Auth**: ASP.NET Core Identity (cookie-based, integer keys)
- **ORM**: Entity Framework Core 8 (Sqlite provider)
- **Patterns**: MediatR (CQRS), AutoMapper, Serilog
- **Database**: SQLite file (`LMS.Web/lms.db`), schema created via `EnsureCreated()` on startup

## Project Layout

- `LMS.Web/` — MVC web project (controllers, views, Program.cs entry point)
- `LMS.Models/` — Entities, enums, and DTOs
- `LMS.Data/` — `AppDbContext` + database seeder (creates roles + admin user)
- `LMS.Handlers/` — MediatR command/query handlers (Auth, Dashboard, Leads, Users)

## Running Locally on Replit

The "Start application" workflow runs:

```
dotnet run --project LMS.Web/LMS.Web.csproj --no-launch-profile
```

The app binds to `http://0.0.0.0:5000` (configured via `WebHost.UseUrls` in `Program.cs`).
HTTPS redirection is disabled because Replit terminates TLS at its proxy.

## Default Admin Credentials

Seeded on first startup from `appsettings.json` `AdminSeed` section:

- Email: `admin@lms.com`
- Password: `Admin@123`

## Replit-Specific Adaptations

The original project targeted SQL Server with EF Core migrations. For Replit:

1. Replaced `Microsoft.EntityFrameworkCore.SqlServer` with `Microsoft.EntityFrameworkCore.Sqlite`
   in both `LMS.Web.csproj` and `LMS.Data.csproj`.
2. Connection string in `appsettings.json` switched to `Data Source=lms.db`.
3. Removed SQL Server–specific migration files under `LMS.Data/Migrations/`; the schema is now
   created at startup using `db.Database.EnsureCreatedAsync()` in `Program.cs`.
4. Forced Kestrel to listen on `http://0.0.0.0:5000` and removed `UseHttpsRedirection()` so the
   Replit preview proxy can reach the app over HTTP.
5. `.NET 8.0` SDK module installed (`dotnet-8.0`).

## Deployment

Configured as a VM deployment so the SQLite database file persists between requests:

- Build: `dotnet publish LMS.Web/LMS.Web.csproj -c Release -o publish`
- Run:   `dotnet publish/LMS.Web.dll`
