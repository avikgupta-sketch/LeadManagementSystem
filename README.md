Lead Management System (LMS)
A full-stack web application built with ASP.NET Core MVC (.NET 8) for managing sales leads across a role-based team of Managers and Agents.

📌 About

Centralized platform to create, assign, track and manage sales leads
Every action is logged with a full audit trail using Lead Remarks
Role-based access — every user sees only what they are allowed to see
No data is ever permanently deleted — soft delete is used throughout


✅ Features

Role-based login using ASP.NET Core Identity (Admin, Manager, Agent)
Create, assign, edit and soft-delete leads
Manager can reassign leads between agents
Agent can update lead status and add remarks
Dashboard showing total leads and count by each status
Full audit trail — every status change and reassignment is recorded
Server-side pagination, search and sort using DataTables
Serilog logging to console and daily rolling log files


🛠 Tech Stack
LayerTechnologyFrameworkASP.NET Core MVC (.NET 8)DatabaseSQL Server (SSMS 2022)ORMEntity Framework Core — Code FirstAuthASP.NET Core IdentityMediatorMediatR — CQRS patternLoggingSerilog (Console + File)MappingAutoMapperPaginationDataTables — server-side

🗂 Project Structure
LeadManagementSystem/
├── LMS.Web          →  Controllers, Views, Program.cs
├── LMS.Handlers     →  MediatR Commands, Queries and Handlers
├── LMS.Data         →  AppDbContext, Migrations, DatabaseSeeder
└── LMS.Models       →  Entities, DTOs, Enums

🚀 Getting Started
Prerequisites

.NET 8 SDK
SQL Server
Visual Studio 2022

Setup Steps
1. Clone the repository
bashgit clone https://github.com/your-username/LeadManagementSystem.git
cd LeadManagementSystem
2. Update the connection string

Open LMS.Web/appsettings.json
Replace YOUR_PC_NAME with your actual machine name

json"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_PC_NAME\\SQLEXPRESS;Database=LMSDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
3. Run the application

Open the solution in Visual Studio
Press F5
On first run the app will automatically:

Create the database and apply all migrations
Seed the default Admin user and all roles




🔑 Default Login Credentials
RoleEmailPasswordAdminadmin@lms.comAdmin@123

Log in as Admin → create a Manager → log in as Manager → create Agents and Leads


👥 Roles & Permissions
ActionAdminManagerAgentCreate Manager✅❌❌Create Agent❌✅❌Create Lead❌✅✅Reassign Lead❌✅❌Update Lead Status❌❌✅Add Remarks❌❌✅Delete Lead❌✅❌Delete Agent❌✅❌View Dashboard✅✅✅

🔄 Lead Status Flow
New → InProgress → FollowUp → Interested  → Converted ✅
                                           → NotInterested → Closed
                                                           → Rejected ❌

A lead always starts at New
Once a lead reaches Converted, Closed or Rejected it cannot be edited or reassigned
Every status change requires a remark explaining the change


🔒 Security

Passwords are hashed by ASP.NET Core Identity — never stored as plain text
Every handler validates that the logged-in user owns the resource before allowing any action
Soft-deleted users cannot log in
CSRF protection enabled on all POST requests via AntiForgery tokens


📁 Logs

Log files are created automatically in the Logs/ folder inside LMS.Web
A new file is created every day

Logs/lms-20260501.log
Logs/lms-20260502.log

⚠️ Important Notes

Do not delete the Migrations folder — it contains the database schema history
The lms.db file (if present) is a Replit leftover and can be safely deleted
The .local/ and attached_assets/ folders are also Replit artifacts and are not needed
