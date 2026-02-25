# Conference Application - Copilot Instructions

This is a comprehensive conference application built with ASP.NET Core MVC and deployed to Azure Container Apps.

## Project Overview
- **Technology**: ASP.NET Core MVC, Entity Framework Core, C#, Bootstrap
- **Database**: SQL Server (Azure SQL Database)
- **Authentication**: ASP.NET Core Identity
- **Deployment**: Azure Container Apps in Sweden Central region
- **CI/CD**: GitHub Actions

## Key Features
- Session management with 10 pre-populated technology sessions
- User registration for sessions
- Session submission with approval process
- Admin dashboard for session approvals
- Responsive web interface

## Architecture Components
- **Models**: Session, User, Registration, Submission
- **Controllers**: Home, Sessions, Admin, Account
- **Views**: Razor pages with Bootstrap styling
- **Data**: Entity Framework Core with SQL Server
- **Authorization**: Role-based access (Admin, User)

## Development Guidelines
- Use Entity Framework Core for data access
- Implement proper validation on all forms
- Follow ASP.NET Core MVC conventions
- Use Bootstrap for responsive design
- Implement proper error handling and logging
- Follow security best practices for authentication

## Deployment Configuration
- Container deployed to Azure Container Apps
- Database: Azure SQL Database
- Region: Sweden Central
- CI/CD through GitHub Actions