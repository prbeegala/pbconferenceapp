# ConferenceApp CI/CD Pipeline Setup Script
# This script sets up Azure authentication and GitHub environments for the CI/CD pipeline

param(
    [Parameter(Mandatory=$true)]
    [string]$SubscriptionId,
    
    [Parameter(Mandatory=$true)]
    [string]$GitHubRepo,  # Format: username/repository-name
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "swedencentral",
    
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupPrefix = "rg-conferenceapp"
)

# Function to check if command exists
function Test-Command {
    param($Command)
    return Get-Command $Command -ErrorAction SilentlyContinue
}

Write-Host "🚀 Starting ConferenceApp CI/CD Pipeline Setup" -ForegroundColor Green

# Check prerequisites
Write-Host "📋 Checking prerequisites..." -ForegroundColor Yellow

if (!(Test-Command "az")) {
    Write-Error "❌ Azure CLI not found. Please install Azure CLI first."
    exit 1
}

if (!(Test-Command "gh")) {
    Write-Error "❌ GitHub CLI not found. Please install GitHub CLI first."
    exit 1
}

# Login to Azure
Write-Host "🔐 Logging into Azure..." -ForegroundColor Yellow
az login
az account set --subscription $SubscriptionId

# Get tenant ID
$TenantId = (az account show --query tenantId -o tsv)
Write-Host "✅ Using Tenant ID: $TenantId" -ForegroundColor Green

# Login to GitHub
Write-Host "🔐 Logging into GitHub..." -ForegroundColor Yellow
gh auth login

# Create resource groups for different environments
$Environments = @("dev", "staging", "prod")
$ResourceGroups = @{}

foreach ($env in $Environments) {
    $rgName = "$ResourceGroupPrefix-$env"
    Write-Host "📦 Creating resource group: $rgName" -ForegroundColor Yellow
    
    az group create --name $rgName --location $Location
    $ResourceGroups[$env] = $rgName
    
    Write-Host "✅ Resource group created: $rgName" -ForegroundColor Green
}

# Create User-assigned Managed Identity for CI/CD
$ManagedIdentityRG = "rg-conferenceapp-pipeline"
$ManagedIdentityName = "id-conferenceapp-pipeline"

Write-Host "🆔 Creating managed identity resource group..." -ForegroundColor Yellow
az group create --name $ManagedIdentityRG --location $Location

Write-Host "🆔 Creating user-assigned managed identity..." -ForegroundColor Yellow
$Identity = az identity create --name $ManagedIdentityName --resource-group $ManagedIdentityRG --location $Location | ConvertFrom-Json

$ClientId = $Identity.clientId
$PrincipalId = $Identity.principalId

Write-Host "✅ Managed Identity created with Client ID: $ClientId" -ForegroundColor Green

# Assign RBAC permissions
Write-Host "🔑 Assigning RBAC permissions..." -ForegroundColor Yellow

# Contributor role for all resource groups
foreach ($env in $Environments) {
    $rgName = $ResourceGroups[$env]
    Write-Host "   Assigning Contributor role to $rgName..." -ForegroundColor Cyan
    az role assignment create --assignee $PrincipalId --role "Contributor" --scope "/subscriptions/$SubscriptionId/resourceGroups/$rgName"
}

# AcrPull role for the subscription (will be needed for container registry)
Write-Host "   Assigning AcrPull role..." -ForegroundColor Cyan
az role assignment create --assignee $PrincipalId --role "7f951dda-4ed3-4680-a7ca-43fe172d538d" --scope "/subscriptions/$SubscriptionId"

Write-Host "✅ RBAC permissions assigned" -ForegroundColor Green

# Setup Federated Credentials for each environment
Write-Host "🔗 Setting up federated credentials..." -ForegroundColor Yellow

$FederatedCreds = @(
    @{ name = "development"; subject = "repo:$GitHubRepo`:environment:development" },
    @{ name = "staging"; subject = "repo:$GitHubRepo`:environment:staging" },
    @{ name = "production"; subject = "repo:$GitHubRepo`:environment:production" }
)

foreach ($cred in $FederatedCreds) {
    Write-Host "   Creating federated credential for $($cred.name)..." -ForegroundColor Cyan
    
    $credParam = @{
        name = $cred.name
        issuer = "https://token.actions.githubusercontent.com"
        subject = $cred.subject
        audiences = @("api://AzureADTokenExchange")
    } | ConvertTo-Json -Compress
    
    az identity federated-credential create --name $cred.name --identity-name $ManagedIdentityName --resource-group $ManagedIdentityRG --issuer "https://token.actions.githubusercontent.com" --subject $cred.subject --audiences "api://AzureADTokenExchange"
}

Write-Host "✅ Federated credentials created" -ForegroundColor Green

# Create GitHub environments and configure secrets/variables
Write-Host "🔧 Setting up GitHub environments..." -ForegroundColor Yellow

# Set repository-level secrets
Write-Host "   Setting repository secrets..." -ForegroundColor Cyan
gh secret set AZURE_CLIENT_ID --body $ClientId --repo $GitHubRepo
gh secret set AZURE_TENANT_ID --body $TenantId --repo $GitHubRepo
gh secret set AZURE_SUBSCRIPTION_ID --body $SubscriptionId --repo $GitHubRepo

$EnvironmentMappings = @{
    "development" = "dev"
    "staging" = "staging"  
    "production" = "prod"
}

foreach ($envName in $EnvironmentMappings.Keys) {
    $envSuffix = $EnvironmentMappings[$envName]
    $rgName = $ResourceGroups[$envSuffix]
    
    Write-Host "   Creating environment: $envName..." -ForegroundColor Cyan
    
    # Create environment (this might require manual approval in GitHub UI)
    gh api --method PUT "repos/$GitHubRepo/environments/$envName" --field "wait_timer=0" --field "prevent_self_review=false" --field-file "deployment_branch_policy=@-" --input - <<< '{"protected_branches":true,"custom_branch_policies":false}'
    
    # Set environment variables
    gh variable set AZD_ENVIRONMENT_NAME --body "conferenceapp-$envSuffix" --env $envName --repo $GitHubRepo
    gh variable set AZURE_LOCATION --body $Location --env $envName --repo $GitHubRepo
    gh variable set AZURE_RESOURCE_GROUP --body $rgName --env $envName --repo $GitHubRepo
    
    Write-Host "   ✅ Environment $envName configured" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎉 Setup completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Next steps:" -ForegroundColor Yellow
Write-Host "1. Go to your GitHub repository settings" 
Write-Host "2. Navigate to Environments and configure approval workflows for 'staging' and 'production'"
Write-Host "3. Run the Infrastructure Deployment workflow to provision Azure resources"
Write-Host "4. Push code changes to test the CI/CD pipeline"
Write-Host ""
Write-Host "🔑 Key Information:" -ForegroundColor Cyan
Write-Host "   Managed Identity Client ID: $ClientId"
Write-Host "   Subscription ID: $SubscriptionId"
Write-Host "   Tenant ID: $TenantId"
Write-Host "   Location: $Location"
Write-Host ""

foreach ($env in $Environments) {
    Write-Host "   $env Resource Group: $($ResourceGroups[$env])" -ForegroundColor White
}

Write-Host ""
Write-Host "📚 For troubleshooting, see .azure/pipeline-setup.md" -ForegroundColor Yellow