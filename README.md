# Conference Application

A comprehensive conference management application built with ASP.NET Core 8.0 MVC and deployed to Azure Container Apps.

## 🚀 Features

- **Session Management**: Browse and search conference sessions by technology, level, and keywords
- **User Registration**: Register for sessions with capacity management and waiting lists
- **Session Submissions**: Users can submit session proposals for approval
- **Admin Approval Workflow**: Administrators can approve, reject, or request revisions for submitted sessions
- **Authentication & Authorization**: Role-based access control with ASP.NET Core Identity
- **Responsive Design**: Modern, mobile-friendly interface built with Bootstrap

## 🏗️ Architecture

### Technology Stack
- **Backend**: ASP.NET Core 8.0 MVC
- **Frontend**: Razor Pages with Bootstrap 5
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity
- **Containerization**: Docker
- **Cloud Platform**: Azure Container Apps
- **CI/CD**: GitHub Actions

### Key Components
- **Models**: Session, Registration, SessionSubmission, ApplicationUser
- **Controllers**: SessionsController, AdminController, HomeController
- **Database**: ApplicationDbContext with seeded data for 10 technology sessions
- **Authentication**: Role-based authorization (Admin, User)

## 🛠️ Local Development

### Prerequisites
- .NET 8.0 SDK
- SQL Server LocalDB or SQL Server
- Docker (optional, for containerization)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ClaudeFunctions
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (if needed)
   Edit `appsettings.json` and update the connection string if not using LocalDB.

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Navigate to `https://localhost:7000` or `http://localhost:5000`
   - Default admin credentials: `admin@conference.com` / `Admin123!`

### Database Setup
The application uses Entity Framework Core migrations and will automatically:
- Create the database on first run
- Seed 10 technology sessions
- Create Admin and User roles
- Create a default admin user

### Running Tests
```bash
dotnet test
```

## 🐳 Docker Support

### Build Docker Image
```bash
docker build -t conference-app .
```

### Run Docker Container
```bash
docker run -d -p 8080:8080 --name conference-app-container conference-app
```

## ☁️ Azure Deployment

This application is configured for deployment to Azure Container Apps using Azure Developer CLI (AZD).

### Quick Deployment

1. **Install Azure Developer CLI**
   ```bash
   # Windows
   winget install microsoft.azd
   
   # macOS
   brew tap azure/azd && brew install azd
   
   # Linux
   curl -fsSL https://aka.ms/install-azd.sh | bash
   ```

2. **Login to Azure**
   ```bash
   azd auth login
   ```

3. **Initialize and deploy**
   ```bash
   azd init
   azd up
   ```

### CI/CD Pipeline

The repository includes a complete CI/CD pipeline using GitHub Actions:

#### Setup CI/CD Pipeline
1. **Run the setup script**:
   ```powershell
   # Windows
   .\setup-azure-auth-for-pipeline.ps1 -SubscriptionId "your-subscription-id" -GitHubRepo "username/repository"
   
   # macOS/Linux  
   ./setup-azure-auth-for-pipeline.sh -s "your-subscription-id" -r "username/repository"
   ```

2. **Configure GitHub environments** in your repository settings for approval workflows

3. **Run infrastructure deployment** workflow to provision Azure resources

#### Pipeline Features
- **Continuous Integration**: Build and test on every pull request
- **Multi-environment Deployment**: Development → Staging → Production
- **Infrastructure as Code**: Bicep templates for Azure resources
- **Security**: OIDC authentication with managed identity
- **Environment Protection**: Manual approvals for staging and production

For detailed setup instructions, see [Pipeline Setup Guide](.azure/pipeline-setup.md).

## 📊 Application Features

### For Users
- **Browse Sessions**: View all conference sessions with filtering and search
- **Session Details**: Detailed information about speakers, timing, and capacity
- **Registration**: Register for sessions with capacity tracking
- **My Registrations**: View and manage your session registrations
- **Submit Sessions**: Propose sessions for the conference
- **Track Submissions**: Monitor the status of your submitted sessions

### For Administrators
- **Dashboard**: Overview of sessions, registrations, and submissions
- **Review Submissions**: Approve, reject, or request revisions for submitted sessions
- **Session Management**: View all sessions and registration details
- **User Management**: View registrations across all sessions

## 🔧 Configuration

### Environment Variables
- `ConnectionStrings__DefaultConnection`: Database connection string
- `ASPNETCORE_ENVIRONMENT`: Environment (Development, Staging, Production)

### Azure Configuration
- **Location**: Sweden Central
- **Hosting**: Azure Container Apps  
- **Database**: Azure SQL Database
- **Authentication**: Azure AD integration available

## 📝 API Documentation

The application provides web-based interfaces for all functionality. Key endpoints include:

- `/` - Home page with upcoming sessions
- `/Sessions` - Browse all sessions
- `/Sessions/Details/{id}` - Session details and registration
- `/Sessions/Submit` - Submit a new session proposal
- `/Admin` - Administrative dashboard
- `/Admin/ReviewSubmission/{id}` - Review submitted sessions

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Check the [Pipeline Setup Guide](.azure/pipeline-setup.md) for deployment issues
- Review GitHub Actions logs for CI/CD troubleshooting
- Check Azure Activity Logs for resource provisioning issues

## 🎯 Roadmap

- [ ] Email notifications for registration confirmations
- [ ] Calendar integration for session scheduling
- [ ] Speaker profile management
- [ ] Advanced reporting and analytics
- [ ] Mobile application companion
- [ ] Integration with external conference management systems