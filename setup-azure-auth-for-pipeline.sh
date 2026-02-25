#!/bin/bash

# ConferenceApp CI/CD Pipeline Setup Script
# This script sets up Azure authentication and GitHub environments for the CI/CD pipeline

set -e

# Function to display usage
usage() {
    echo "Usage: $0 -s SUBSCRIPTION_ID -r GITHUB_REPO [-l LOCATION] [-p RESOURCE_GROUP_PREFIX]"
    echo "  -s SUBSCRIPTION_ID       Azure subscription ID"
    echo "  -r GITHUB_REPO          GitHub repository in format username/repository-name"
    echo "  -l LOCATION             Azure location (default: swedencentral)"
    echo "  -p RESOURCE_GROUP_PREFIX Resource group prefix (default: rg-conferenceapp)"
    exit 1
}

# Default values
LOCATION="swedencentral"
RESOURCE_GROUP_PREFIX="rg-conferenceapp"

# Parse command line arguments
while getopts "s:r:l:p:h" opt; do
    case $opt in
        s) SUBSCRIPTION_ID="$OPTARG";;
        r) GITHUB_REPO="$OPTARG";;
        l) LOCATION="$OPTARG";;
        p) RESOURCE_GROUP_PREFIX="$OPTARG";;
        h) usage;;
        \?) echo "Invalid option -$OPTARG" >&2; usage;;
    esac
done

# Check required parameters
if [ -z "$SUBSCRIPTION_ID" ] || [ -z "$GITHUB_REPO" ]; then
    echo "❌ Error: Subscription ID and GitHub repository are required"
    usage
fi

echo "🚀 Starting ConferenceApp CI/CD Pipeline Setup"

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI not found. Please install Azure CLI first."
    echo "   Visit: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI not found. Please install GitHub CLI first."
    echo "   Visit: https://cli.github.com/"
    exit 1
fi

if ! command -v jq &> /dev/null; then
    echo "❌ jq not found. Please install jq first."
    echo "   On macOS: brew install jq"
    echo "   On Ubuntu: sudo apt-get install jq"
    exit 1
fi

# Login to Azure
echo "🔐 Logging into Azure..."
az login
az account set --subscription "$SUBSCRIPTION_ID"

# Get tenant ID
TENANT_ID=$(az account show --query tenantId -o tsv)
echo "✅ Using Tenant ID: $TENANT_ID"

# Login to GitHub
echo "🔐 Logging into GitHub..."
gh auth login

# Create resource groups for different environments
declare -A RESOURCE_GROUPS
ENVIRONMENTS=("dev" "staging" "prod")

for env in "${ENVIRONMENTS[@]}"; do
    RG_NAME="${RESOURCE_GROUP_PREFIX}-${env}"
    echo "📦 Creating resource group: $RG_NAME"
    
    az group create --name "$RG_NAME" --location "$LOCATION" > /dev/null
    RESOURCE_GROUPS[$env]=$RG_NAME
    
    echo "✅ Resource group created: $RG_NAME"
done

# Create User-assigned Managed Identity for CI/CD
MANAGED_IDENTITY_RG="rg-conferenceapp-pipeline"
MANAGED_IDENTITY_NAME="id-conferenceapp-pipeline"

echo "🆔 Creating managed identity resource group..."
az group create --name "$MANAGED_IDENTITY_RG" --location "$LOCATION" > /dev/null

echo "🆔 Creating user-assigned managed identity..."
IDENTITY_JSON=$(az identity create --name "$MANAGED_IDENTITY_NAME" --resource-group "$MANAGED_IDENTITY_RG" --location "$LOCATION")

CLIENT_ID=$(echo "$IDENTITY_JSON" | jq -r '.clientId')
PRINCIPAL_ID=$(echo "$IDENTITY_JSON" | jq -r '.principalId')

echo "✅ Managed Identity created with Client ID: $CLIENT_ID"

# Wait a moment for the identity to propagate
echo "⏳ Waiting for identity propagation..."
sleep 30

# Assign RBAC permissions
echo "🔑 Assigning RBAC permissions..."

# Contributor role for all resource groups
for env in "${ENVIRONMENTS[@]}"; do
    RG_NAME="${RESOURCE_GROUPS[$env]}"
    echo "   Assigning Contributor role to $RG_NAME..."
    az role assignment create --assignee "$PRINCIPAL_ID" --role "Contributor" --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RG_NAME" > /dev/null
done

# AcrPull role for the subscription
echo "   Assigning AcrPull role..."
az role assignment create --assignee "$PRINCIPAL_ID" --role "7f951dda-4ed3-4680-a7ca-43fe172d538d" --scope "/subscriptions/$SUBSCRIPTION_ID" > /dev/null

echo "✅ RBAC permissions assigned"

# Setup Federated Credentials for each environment
echo "🔗 Setting up federated credentials..."

FEDERATED_CREDS=(
    "development:repo:$GITHUB_REPO:environment:development"
    "staging:repo:$GITHUB_REPO:environment:staging"
    "production:repo:$GITHUB_REPO:environment:production"
)

for cred in "${FEDERATED_CREDS[@]}"; do
    IFS=':' read -r name subject <<< "$cred"
    echo "   Creating federated credential for $name..."
    
    az identity federated-credential create \
        --name "$name" \
        --identity-name "$MANAGED_IDENTITY_NAME" \
        --resource-group "$MANAGED_IDENTITY_RG" \
        --issuer "https://token.actions.githubusercontent.com" \
        --subject "$subject" \
        --audiences "api://AzureADTokenExchange" > /dev/null
done

echo "✅ Federated credentials created"

# Create GitHub environments and configure secrets/variables
echo "🔧 Setting up GitHub environments..."

# Set repository-level secrets
echo "   Setting repository secrets..."
gh secret set AZURE_CLIENT_ID --body "$CLIENT_ID" --repo "$GITHUB_REPO"
gh secret set AZURE_TENANT_ID --body "$TENANT_ID" --repo "$GITHUB_REPO"
gh secret set AZURE_SUBSCRIPTION_ID --body "$SUBSCRIPTION_ID" --repo "$GITHUB_REPO"

# Environment mappings
declare -A ENV_MAPPINGS
ENV_MAPPINGS["development"]="dev"
ENV_MAPPINGS["staging"]="staging"
ENV_MAPPINGS["production"]="prod"

for env_name in "${!ENV_MAPPINGS[@]}"; do
    env_suffix="${ENV_MAPPINGS[$env_name]}"
    rg_name="${RESOURCE_GROUPS[$env_suffix]}"
    
    echo "   Creating environment: $env_name..."
    
    # Create environment
    gh api --method PUT "repos/$GITHUB_REPO/environments/$env_name" \
        --field "wait_timer=0" \
        --field "prevent_self_review=false" > /dev/null || true
    
    # Set environment variables
    gh variable set AZD_ENVIRONMENT_NAME --body "conferenceapp-$env_suffix" --env "$env_name" --repo "$GITHUB_REPO"
    gh variable set AZURE_LOCATION --body "$LOCATION" --env "$env_name" --repo "$GITHUB_REPO"
    gh variable set AZURE_RESOURCE_GROUP --body "$rg_name" --env "$env_name" --repo "$GITHUB_REPO"
    
    echo "   ✅ Environment $env_name configured"
done

echo ""
echo "🎉 Setup completed successfully!"
echo ""
echo "📝 Next steps:"
echo "1. Go to your GitHub repository settings"
echo "2. Navigate to Environments and configure approval workflows for 'staging' and 'production'"
echo "3. Run the Infrastructure Deployment workflow to provision Azure resources"
echo "4. Push code changes to test the CI/CD pipeline"
echo ""
echo "🔑 Key Information:"
echo "   Managed Identity Client ID: $CLIENT_ID"
echo "   Subscription ID: $SUBSCRIPTION_ID"
echo "   Tenant ID: $TENANT_ID"
echo "   Location: $LOCATION"
echo ""

for env in "${ENVIRONMENTS[@]}"; do
    echo "   $env Resource Group: ${RESOURCE_GROUPS[$env]}"
done

echo ""
echo "📚 For troubleshooting, see .azure/pipeline-setup.md"