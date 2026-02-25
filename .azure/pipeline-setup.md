# CI/CD Pipeline Setup for ConferenceApp

This document outlines the steps required to set up the CI/CD pipeline for the ConferenceApp project.

## Prerequisites

1. **Azure Subscription**: Ensure you have an active Azure subscription
2. **GitHub Repository**: The code should be in a GitHub repository
3. **Azure CLI**: Install Azure CLI locally for setup scripts
4. **GitHub CLI**: Install GitHub CLI for repository configuration

## Pipeline Architecture

The CI/CD pipeline consists of:

### Workflows:
1. **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
   - **CI Stage**: Build and test the application
   - **CD Stage**: Deploy to Development → Staging → Production environments
   - Triggered on pushes to main branch

2. **Infrastructure Deployment** (`.github/workflows/infra-deploy.yml`)
   - Manual workflow for provisioning/destroying infrastructure
   - Supports all three environments (development, staging, production)

### Environments:
- **Development**: Auto-deployment for testing latest changes
- **Staging**: Pre-production testing with manual approval
- **Production**: Live environment with manual approval

## Setup Instructions

### Step 1: Run the Setup Script

Execute the authentication setup script:

**For Windows (PowerShell):**
```powershell
.\setup-azure-auth-for-pipeline.ps1
```

**For macOS/Linux (Bash):**
```bash
chmod +x setup-azure-auth-for-pipeline.sh
./setup-azure-auth-for-pipeline.sh
```

### Step 2: Configure GitHub Environments

The setup script will guide you through:

1. Creating Azure User-assigned Managed Identity
2. Setting up Federated Credentials for OIDC authentication
3. Assigning necessary RBAC permissions
4. Creating GitHub environments with approval workflows
5. Configuring GitHub secrets and variables

### Step 3: Initial Infrastructure Deployment

1. Go to your GitHub repository
2. Navigate to **Actions** → **Infrastructure Deployment**
3. Click **Run workflow**
4. Select **development** environment and **provision** action
5. Approve the deployment in GitHub environments
6. Repeat for **staging** and **production** environments

### Step 4: Test the CI/CD Pipeline

1. Make a small change to the application
2. Push to the main branch
3. Monitor the pipeline execution in GitHub Actions
4. Verify deployments in each environment

## Required GitHub Secrets

The following secrets will be configured by the setup script:

- `AZURE_CLIENT_ID`: User-assigned Managed Identity Client ID
- `AZURE_TENANT_ID`: Azure AD Tenant ID  
- `AZURE_SUBSCRIPTION_ID`: Azure Subscription ID

## Required GitHub Variables (per environment)

- `AZD_ENVIRONMENT_NAME`: Name of the AZD environment
- `AZURE_LOCATION`: Azure region (swedencentral)
- `AZURE_RESOURCE_GROUP`: Resource group name
- `AZURE_CONTAINER_REGISTRY_ENDPOINT`: ACR endpoint URL

## Security Considerations

1. **Managed Identity**: Uses OIDC authentication (no long-term secrets)
2. **RBAC**: Minimal permissions (Contributor + AcrPull roles)
3. **Environment Protection**: Manual approval required for staging/production
4. **Branch Protection**: Only main branch can trigger deployments

## Troubleshooting

### Common Issues:

1. **OIDC Authentication Failed**
   - Verify federated credentials configuration
   - Check GitHub repository settings

2. **Permission Denied**
   - Ensure Managed Identity has proper RBAC assignments
   - Verify resource group permissions

3. **AZD Environment Issues**
   - Check environment variables configuration
   - Verify AZD environment settings

### Getting Help:

- Check GitHub Actions logs for detailed error messages
- Review Azure Activity Log for resource provisioning issues
- Ensure all prerequisites are installed and configured

## Additional Resources

- [Azure Developer CLI Documentation](https://docs.microsoft.com/en-us/azure/developer/azure-developer-cli/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Container Apps Documentation](https://docs.microsoft.com/en-us/azure/container-apps/)