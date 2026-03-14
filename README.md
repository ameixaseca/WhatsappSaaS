# WhatsApp SaaS Automation & Management System

This is a multi-tenant SaaS MVP for WhatsApp automation and customer service management, built with .NET 8, Blazor, SQL Server, and SignalR.

## Features
- **Multi-tenancy**: Isolated data per company using EF Core Global Query Filters.
- **Inbox**: Real-time messaging interface with SignalR.
- **CRM**: Basic contact management with tags and notes.
- **Auth**: JWT-based authentication.
- **Docker**: Ready for deployment with Docker Compose.

## Project Structure
- `WhatsappSaaS.Domain`: Entities and business rules.
- `WhatsappSaaS.Application`: DTOs, interfaces, and business logic.
- `WhatsappSaaS.Infrastructure`: Data persistence (EF Core), SignalR Hubs, and external services.
- `WhatsappSaaS.API`: REST API controllers and configuration.
- `WhatsappSaaS.Web`: Blazor Web App frontend.

## How to Run

### Prerequisites
- Docker and Docker Compose

### Execution
1. Clone the repository.
2. Navigate to the root folder.
3. Run the following command:
   ```bash
   docker-compose up --build
   ```

The system will start the SQL Server database, the API (port 5000), and the Web application (port 5001).

## Tech Stack
- **Backend**: ASP.NET Core 8.0
- **Frontend**: Blazor Server
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Real-time**: SignalR
- **Containerization**: Docker
