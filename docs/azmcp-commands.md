# Azure MCP CLI Command Reference

> [!IMPORTANT]
> The Azure MCP Server has two modes: MCP Server mode and CLI mode.  When you start the MCP Server with `azmcp server start` that will expose an endpoint for MCP Client communication. The `azmcp` CLI also exposes all of the Tools via a command line interface, i.e. `azmcp subscription list`.  Since `azmcp` is built on a CLI infrastructure, you'll see the word "Command" be used interchangeably with "Tool".

## Global Options

The following options are available for all commands:

| Option | Required | Default | Description |
|-----------|----------|---------|-------------|
| `--subscription` | No | Environment variable `AZURE_SUBSCRIPTION_ID` | Azure subscription ID for target resources |
| `--tenant-id` | No | - | Azure tenant ID for authentication |
| `--auth-method` | No | 'credential' | Authentication method ('credential', 'key', 'connectionString') |
| `--retry-max-retries` | No | 3 | Maximum retry attempts for failed operations |
| `--retry-delay` | No | 2 | Delay between retry attempts (seconds) |
| `--retry-max-delay` | No | 10 | Maximum delay between retries (seconds) |
| `--retry-mode` | No | 'exponential' | Retry strategy ('fixed' or 'exponential') |
| `--retry-network-timeout` | No | 100 | Network operation timeout (seconds) |

## Available Commands

### Server Operations

The Azure MCP Server can be started in several different modes depending on how you want to expose the Azure tools:

#### Default Mode (Namespace)

Exposes Azure tools grouped by service namespace. Each Azure service appears as a single namespace-level tool that routes to individual operations internally. This is the default mode to reduce tool count and prevent VS Code from hitting the 128 tool limit.

```bash
# Start MCP Server with namespace-level tools (default behavior)
azmcp server start \
    [--transport <transport>] \
    [--read-only]

# Explicitly specify namespace mode
azmcp server start \
    --mode namespace \
    [--transport <transport>] \
    [--read-only]
```

#### All Tools Mode

Exposes all Azure tools individually. Each Azure service operation appears as a separate MCP tool.

```bash
# Start MCP Server with all tools exposed individually
azmcp server start \
    --mode all \
    [--transport <transport>] \
    [--read-only]
```

#### Single Tool Mode

Exposes a single "azure" tool that handles internal routing across all Azure MCP tools.

```bash
# Start MCP Server with single azure tool
azmcp server start \
    --mode single \
    [--transport <transport>] \
    [--read-only]
```

#### Namespace Filtering

Exposes only tools for specific Azure service namespaces. Use multiple `--namespace` parameters to include multiple namespaces.

```bash
# Start MCP Server with only Storage tools
azmcp server start \
    --namespace storage \
    --mode all \
    [--transport <transport>] \
    [--read-only]

# Start MCP Server with Storage and Key Vault tools
azmcp server start \
    --namespace storage \
    --namespace keyvault \
    --mode all \
    [--transport <transport>] \
    [--read-only]
```

#### Namespace Mode (Default)

Collapses all tools within each namespace into a single tool (e.g., all storage operations become one "storage" tool with internal routing). This mode is particularly useful when working with MCP clients that have tool limits - for example, VS Code only supports a maximum of 128 tools across all registered MCP servers.

```bash
# Start MCP Server with service proxy tools
azmcp server start \
    --mode namespace \
    [--transport <transport>] \
    [--read-only]
```

#### Single Tool Proxy Mode

Exposes a single "azure" tool that handles internal routing across all Azure MCP tools.

```bash
# Start MCP Server with single Azure tool proxy
azmcp server start \
    --mode single \
    [--transport <transport>] \
    [--read-only]
```

> **Note:**
>
> - For namespace mode, replace `<namespace-name>` with available top level command groups. Run `azmcp -h` to review available namespaces. Examples include `storage`, `keyvault`, `cosmos`, `monitor`, etc.
> - The `--read-only` flag applies to all modes and filters the tool list to only contain tools that provide read-only operations.
> - Multiple `--namespace` parameters can be used together to expose tools for multiple specific namespaces.
> - The `--namespace` and `--mode` parameters can also be combined to provide a unique running mode based on the desired scenario.

### Azure AI Foundry Operations

```bash
# List AI Foundry models
azmcp foundry models list [--search-for-free-playground <search-for-free-playground>] \
                          [--publisher-name <publisher-name>] \
                          [--license-name <license-name>] \
                          [--model-name <model-name>]

# Deploy an AI Foundry model
azmcp foundry models deploy --subscription <subscription> \
                            --resource-group <resource-group> \
                            --deployment-name <deployment-name> \
                            --model-name <model-name> \
                            --model-format <model-format> \
                            --azure-ai-services-name <azure-ai-services-name> \
                            [--model-version <model-version>] \
                            [--model-source <model-source>] \
                            [--sku-name <sku-name>] \
                            [--sku-capacity <sku-capacity>] \
                            [--scale-type <scale-type>] \
                            [--scale-capacity <scale-capacity>]

# List AI Foundry model deployments
azmcp foundry models deployments list --endpoint <endpoint>
```

### Azure AI Search Operations

```bash
# List AI Search accounts in a subscription
azmcp search list --subscription <subscription>

# List AI Search indexes in account
azmcp search index list --subscription <subscription> \
                        --service-name <service-name>

# Get AI Search index
azmcp search index describe --subscription <subscription> \
                            --service-name <service-name> \
                            --index-name <index-name>

# Query AI Search index
azmcp search index query --subscription <subscription> \
                         --service-name <service-name> \
                         --index-name <index-name> \
                         --query <query>
```

### Azure App Configuration Operations

```bash
# List App Configuration stores in a subscription
azmcp appconfig account list --subscription <subscription>

# List all key-value settings in an App Configuration store
azmcp appconfig kv list --subscription <subscription> \
                        --account-name <account-name> \
                        [--key <key>] \
                        [--label <label>]

# Show a specific key-value setting
azmcp appconfig kv show --subscription <subscription> \
                        --account-name <account-name> \
                        --key <key> \
                        [--label <label>]

# Set a key-value setting
azmcp appconfig kv set --subscription <subscription> \
                       --account-name <account-name> \
                       --key <key> \
                       --value <value> \
                       [--label <label>]

# Lock a key-value setting (make it read-only)
azmcp appconfig kv lock --subscription <subscription> \
                        --account-name <account-name> \
                        --key <key> \
                        [--label <label>]

# Unlock a key-value setting (make it editable)
azmcp appconfig kv unlock --subscription <subscription> \
                          --account-name <account-name> \
                          --key <key> \
                          [--label <label>]

# Delete a key-value setting
azmcp appconfig kv delete --subscription <subscription> \
                          --account-name <account-name> \
                          --key <key> \
                          [--label <label>]
```

### Azure CLI Operations

```bash
# Execute any Azure CLI command
azmcp extension az --command "<command>"

# Examples:
# List resource groups
azmcp extension az --command "group list"

# Get storage account details
azmcp extension az --command "storage account show --name <account-name> --resource-group <resource-group>"

# List virtual machines
azmcp extension az --command "vm list --resource-group <resource-group>"
```

### Azure Cosmos DB Operations

```bash
# List Cosmos DB accounts in a subscription
azmcp cosmos account list --subscription <subscription>

# List databases in a Cosmos DB account
azmcp cosmos database list --subscription <subscription> \
                           --account-name <account-name>

# List containers in a Cosmos DB database
azmcp cosmos database container list --subscription <subscription> \
                                     --account-name <account-name> \
                                     --database-name <database-name>

# Query items in a Cosmos DB container
azmcp cosmos database container item query --subscription <subscription> \
                                           --account-name <account-name> \
                                           --database-name <database-name> \
                                           --container-name <container-name> \
                                           [--query "SELECT * FROM c"]
```

### Azure Data Explorer Operations

```bash
# List Azure Data Explorer clusters in a subscription
azmcp kusto cluster list --subscription <subscription>

# Get details for a Azure Data Explorer cluster
azmcp kusto cluster get --subscription <subscription> \
                        --cluster-name <cluster-name>

# List databases in a Azure Data Explorer cluster
azmcp kusto database list [--cluster-uri <cluster-uri> | --subscription <subscription> --cluster-name <cluster-name>]

# List tables in a Azure Data Explorer database
azmcp kusto table list [--cluster-uri <cluster-uri> | --subscription <subscription> --cluster-name <cluster-name>] \
                       --database-name <database-name>

# Retrieves the schema of a specified Azure Data Explorer table.
azmcp kusto table schema [--cluster-uri <cluster-uri> | --subscription <subscription> --cluster-name <cluster-name>] \
                         --database-name <database-name> \
                         --table <table-name>

# Query Azure Data Explorer database
azmcp kusto query [--cluster-uri <cluster-uri> | --subscription <subscription> --cluster-name <cluster-name>] \
                  --database-name <database-name> \
                  --query "<kql-query>"

# Retrieves a sample of data from a specified Azure Data Explorer table.
azmcp kusto sample [--cluster-uri <cluster-uri> | --subscription <subscription> --cluster-name <cluster-name>]
                   --database-name <database-name> \
                   --table <table-name> \
                   [--limit <limit>]

```

### Azure Database for PostgreSQL Operations

#### Database commands

```bash
# List all databases in a PostgreSQL server
azmcp postgres database list --subscription <subscription> \
                             --resource-group <resource-group> \
                             --user-name <user> \
                             --server <server>

# Execute a query on a PostgreSQL database
azmcp postgres database query --subscription <subscription> \
                              --resource-group <resource-group> \
                              --user-name <user> \
                              --server <server> \
                              --database <database> \
                              --query <query>
```

#### Table Commands

```bash
# List all tables in a PostgreSQL database
azmcp postgres table list --subscription <subscription> \
                          --resource-group <resource-group> \
                          --user-name <user> \
                          --server <server> \
                          --database <database>

# Get the schema of a specific table in a PostgreSQL database
azmcp postgres table schema --subscription <subscription> \
                            --resource-group <resource-group> \
                            --user-name <user> \
                            --server <server> \
                            --database <database> \
                            --table <table>
```

#### Server Commands

```bash
# List all PostgreSQL servers in a subscription & resource group
azmcp postgres server list --subscription <subscription> \
                           --resource-group <resource-group> \
                           --user-name <user>

# Retrieve the configuration of a PostgreSQL server
azmcp postgres server config --subscription <subscription> \
                             --resource-group <resource-group> \
                             --user-name <user> \
                             --server <server>

# Retrieve a specific parameter of a PostgreSQL server
azmcp postgres server param --subscription <subscription> \
                            --resource-group <resource-group> \
                            --user-name <user> \
                            --server <server> \
                            --param <parameter>

# Set a specific parameter of a PostgreSQL server to a specific value
azmcp postgres server setparam --subscription <subscription> \
                               --resource-group <resource-group> \
                               --user-name <user> \
                               --server <server> \
                               --param <parameter> \
                               --value <value>
```

### Azure Developer CLI Operations

```bash
# Execute any Azure CLI command
azmcp extension azd --command "<command>"

# Examples:
# Create a sample todo list app with NodeJS and MongoDB
azmcp extension azd --command "init --template todo-nodejs-mongo"
```

### Azure Key Vault Operations

```bash
# Creates a certificate in a key vault with the default policy
azmcp keyvault certificate create --subscription <subscription> \
                                  --vault <vault-name> \
                                  --name <certificate-name>

# Gets a certificate in a key vault
azmcp keyvault certificate get --subscription <subscription> \
                               --vault <vault-name> \
                               --name <certificate-name>

# Lists certificates in a key vault
azmcp keyvault certificate list --subscription <subscription> \
                                --vault <vault-name>

# Creates a key in a key vault
azmcp keyvault key create --subscription <subscription> \
                          --vault <vault-name> \
                          --key <key-name> \
                          --key-type <key-type>

# Lists keys in a key vault
azmcp keyvault key list --subscription <subscription> \
                        --vault <vault-name> \
                        --include-managed <true/false>

# Creates a secret in a key vault
azmcp keyvault secret create --subscription <subscription> \
                             --vault <vault-name> \
                             --name <secret-name> \
                             --value <secret-value>

# Lists secrets in a key vault
azmcp keyvault secret list --subscription <subscription> \
                           --vault <vault-name>
```

### Azure Kubernetes Service (AKS) Operations

```bash
# List AKS clusters in a subscription
azmcp aks cluster list --subscription <subscription>

# Get details of a specific AKS cluster
azmcp aks cluster get --subscription <subscription> \
                      --name <cluster-name>
```

### Azure Load Testing Operations

```bash
# Create load test
azmcp loadtesting test create --subscription <subscription> \
                              --resource-group <resource-group> \
                              --test-resource-name <test-resource-name> \
                              --test-id <test-id> \
                              --display-name <display-name> \
                              --description <description> \
                              --endpoint <endpoint> \
                              --virtual-users <virtual-users> \
                              --duration <duration> \
                              --ramp-up-time <ramp-up-time>

# Get load test
azmcp loadtesting test get --subscription <subscription> \
                           --resource-group <resource-group> \
                           --test-resource-name <test-resource-name> \
                           --test-id <test-id>

# List load test resources
azmcp loadtesting testresource list --subscription <subscription> \
                                    --resource-group <resource-group> \
                                    --test-resource-name <test-resource-name>

# Create load test resources
azmcp loadtesting testresource create --subscription <subscription> \
                                      --resource-group <resource-group> \
                                      --test-resource-name <test-resource-name>

# Create load test run
azmcp loadtesting testrun create --subscription <subscription> \
                                 --resource-group <resource-group> \
                                 --test-resource-name <test-resource-name> \
                                 --test-id <test-id> \
                                 --testrun-id <testrun-id> \
                                 --display-name <display-name> \
                                 --description <description> \
                                 --old-testrun-id <old-testrun-id>

# Get load test run
azmcp loadtesting testrun get --subscription <subscription> \
                              --resource-group <resource-group> \
                              --test-resource-name <test-resource-name> \
                              --testrun-id <testrun-id>

# List load test run
azmcp loadtesting testrun list --subscription <subscription> \
                               --resource-group <resource-group> \
                               --test-resource-name <test-resource-name> \
                               --test-id <test-id>

# Update load test run
azmcp loadtesting testrun update --subscription <subscription> \
                                 --resource-group <resource-group> \
                                 --test-resource-name <test-resource-name> \
                                 --test-id <test-id> \
                                 --testrun-id <testrun-id> \
                                 --display-name <display-name> \
                                 --description <description>
```

### Azure Managed Grafana Operations

```bash
# List Azure Managed Grafana
azmcp grafana list --subscription <subscription>
```

### Azure Marketplace Operations

```bash
# Get details about an Azure Marketplace product
azmcp marketplace product get --subscription <subscription> \
                              --product-id <product-id> \
                              [--include-stop-sold-plans <true/false>] \
                              [--language <language-code>] \
                              [--market <market-code>] \
                              [--lookup-offer-in-tenant-level <true/false>] \
                              [--plan-id <plan-id>] \
                              [--sku-id <sku-id>] \
                              [--include-service-instruction-templates <true/false>] \
                              [--partner-tenant-id <partner-tenant-id>] \
                              [--pricing-audience <pricing-audience>]
```

### Azure MCP Best Practices

```bash
# Get best practices for secure, production-grade Azure usage
azmcp bestpractices get --resource <resource> --action <action>

# Resource options:
#   general        - General Azure best practices
#   azurefunctions - Azure Functions specific best practices
#   static-web-app - Azure Static Web Apps specific best practices
#
# Action options:
#   all             - Best practices for both code generation and deployment (only for static-web-app)
#   code-generation - Best practices for code generation (for general and azurefunctions)
#   deployment      - Best practices for deployment (for general and azurefunctions)

```

### Azure MCP Tools

```bash
# List all available tools in the Azure MCP server
azmcp tool list
```

### Azure Monitor Operations

#### Log Analytics

```bash
# List Log Analytics workspaces in a subscription
azmcp monitor workspace list --subscription <subscription>

# List tables in a Log Analytics workspace
azmcp monitor table list --subscription <subscription> \
                         --workspace <workspace> \
                         --resource-group <resource-group>

# Query logs from Azure Monitor using KQL
azmcp monitor workspace log query --subscription <subscription> \
                                  --workspace <workspace> \
                                  --table-name <table-name> \
                                  --query "<kql-query>" \
                                  [--hours <hours>] \
                                  [--limit <limit>]

azmcp monitor resource log query --subscription <subscription> \
                                 --resource-id <resource-id> \
                                 --table-name <table-name> \
                                 --query "<kql-query>" \
                                 [--hours <hours>] \
                                 [--limit <limit>]

# Examples:
# Query logs from a specific table
azmcp monitor workspace log query --subscription <subscription> \
                                  --workspace <workspace> \
                                  --table-name "AppEvents_CL" \
                                  --query "| order by TimeGenerated desc"
```

#### Health Models

```bash
# Get the health of an entity
azmcp monitor healthmodels entity gethealth --subscription <subscription> \
                                            --resource-group <resource-group> \
                                            --model-name <health-model-name> \
                                            --entity <entity-id>
```

#### Metrics

```bash
# Query Azure Monitor metrics for a resource
azmcp monitor metrics query --subscription <subscription> \
                            --resource-name <resource-name> \
                            --metric-namespace <metric-namespace> \
                            --metric-names <metric-names> \
                            [--resource-group <resource-group>] \
                            [--resource-type <resource-type>] \
                            [--start-time <start-time>] \
                            [--end-time <end-time>] \
                            [--interval <interval>] \
                            [--aggregation <aggregation>] \
                            [--filter <filter>] \
                            [--max-buckets <max-buckets>]

# List available metric definitions for a resource
azmcp monitor metrics definitions --subscription <subscription> \
                                  --resource-name <resource-name> \
                                  [--resource-group <resource-group>] \
                                  [--resource-type <resource-type>] \
                                  [--metric-namespace <metric-namespace>] \
                                  [--search-string <search-string>] \
                                  [--limit <limit>]

# Examples:
# Query CPU and memory metrics for a virtual machine
azmcp monitor metrics query --subscription <subscription> \
                            --resource-name <resource-name> \
                            --resource-group <resource-group> \
                            --metric-namespace "microsoft.compute/virtualmachines" \
                            --resource-type "Microsoft.Compute/virtualMachines" \
                            --metric-names "Percentage CPU,Available Memory Bytes" \
                            --start-time "2024-01-01T00:00:00Z" \
                            --end-time "2024-01-01T23:59:59Z" \
                            --interval "PT1H" \
                            --aggregation "Average"

# List all available metrics for a storage account
azmcp monitor metrics definitions --subscription <subscription> \
                                  --resource-name <resource-name> \
                                  --resource-type "Microsoft.Storage/storageAccounts"

# Find metrics related to transactions
azmcp monitor metrics definitions --subscription <subscription> \
                                  --resource-name <resource-name> \
                                  --search-string "transaction"
```

### Azure Native ISV Operations

```bash
# List monitored resources in Datadog
azmcp datadog monitoredresources list --subscription <subscription> \
                                      --resource-group <resource-group> \
                                      --datadog-resource <datadog-resource>
```

### Azure Quick Review CLI Extension Operations

```bash
# Scan a subscription for recommendations
azmcp extension azqr --subscription <subscription>

# Scan a subscription and scope to a specific resource group
azmcp extension azqr --subscription <subscription> \
                     --resource-group <resource-group-name>
```

### Azure RBAC Operations

```bash
# List Azure RBAC role assignments
azmcp role assignment list --subscription <subscription> \
                           --scope <scope>
```

### Azure Redis Operations

```bash
# Lists Redis Clusters in the Azure Managed Redis or Azure Redis Enterprise services
azmcp redis cluster list --subscription <subscription>

# Lists Databases in an Azure Redis Cluster
azmcp redis cluster database list --subscription <subscription> \
                                  --resource-group <resource-group> \
                                  --cluster <cluster-name>

# Lists Redis Caches in the Azure Cache for Redis service
azmcp redis cache list --subscription <subscription>

# Lists Access Policy Assignments in an Azure Redis Cache
azmcp redis cache list accesspolicy --subscription <subscription> \
                                    --resource-group <resource-group> \
                                    --cache <cache-name>
```

### Azure Resource Group Operations

```bash
# List resource groups in a subscription
azmcp group list --subscription <subscription>
```

### Azure Service Bus Operations

```bash
# Returns runtime and details about the Service Bus queue
azmcp servicebus queue details --subscription <subscription> \
                               --namespace <service-bus-namespace> \
                               --queue-name <queue-name>

# Gets runtime details a Service Bus topic
azmcp servicebus topic details --subscription <subscription> \
                               --namespace <service-bus-namespace> \
                               --topic-name <topic-name>

# Gets runtime details and message counts for a Service Bus subscription
azmcp servicebus topic subscription details --subscription <subscription> \
                                            --namespace <service-bus-namespace> \
                                            --topic-name <topic-name> \
                                            --subscription-name <subscription-name>
```

### Azure SQL Database Operations

```bash
# Gets a list of all databases in a SQL server
azmcp sql db list --subscription <subscription> \
                  --resource-group <resource-group> \
                  --server <server-name>

# Show details of a specific SQL database
azmcp sql db show --subscription <subscription> \
                  --resource-group <resource-group> \
                  --server <server-name> \
                  --database <database-name>

# Gets a list of firewall rules for a SQL server
azmcp sql firewall-rule list --subscription <subscription> \
                                  --resource-group <resource-group> \
                                  --server <server-name>
```

### Azure SQL Elastic Pool Operations

```bash
# List all elastic pools in a SQL server
azmcp sql elastic-pool list --subscription <subscription> \
                            --resource-group <resource-group> \
                            --server <server-name>
```

### Azure SQL Server Operations

```bash
# List Microsoft Entra ID administrators for a SQL server
azmcp sql server entra-admin list --subscription <subscription> \
                                  --resource-group <resource-group> \
                                  --server <server-name>
```

### Azure Storage Operations

```bash
# List Storage accounts in a subscription
azmcp storage account list --subscription <subscription>

# List tables in a Storage account
azmcp storage table list --subscription <subscription> \
                         --account-name <account-name>

# List blobs in a Storage container
azmcp storage blob list --subscription <subscription> \
                        --account-name <account-name> \
                        --container-name <container-name>

# List containers in a Storage blob service
azmcp storage blob container list --subscription <subscription> \
                                  --account-name <account-name>

# Get detailed properties of a storage container
azmcp storage blob container details --subscription <subscription> \
                                     --account-name <account-name> \
                                     --container-name <container-name>

# List paths in a Data Lake file system
azmcp storage datalake file-system list-paths --subscription <subscription> \
                                              --account-name <account-name> \
                                              --file-system-name <file-system-name>

# Create a directory in DataLake using a specific path
azmcp storage datalake directory create --subscription <subscription> \
                                        --account-name <account-name> \
                                        --directory-path <directory-path>
```

### Azure Subscription Management

```bash
# List available Azure subscriptions
azmcp subscription list [--tenant-id <tenant-id>]
```

## Azure Terraform Best Practices

```bash
# Get secure, production-grade Azure Terraform best practices for effective code generation and command execution.
azmcp azureterraformbestpractices get
```

### Azure Workbooks Operations

```bash
# Create a new workbook
azmcp workbooks create --subscription <subscription> \
                       --resource-group <resource-group> \
                       --display-name <display-name> \
                       --serialized-content <json-content> \
                       [--source-id <source-id>]

# Delete a workbook
azmcp workbooks delete --workbook-id <workbook-resource-id>

# List Azure Monitor workbooks in a resource group
azmcp workbooks list --subscription <subscription> \
                     --resource-group <resource-group> \
                     [--category <category>] \
                     [--kind <kind>] \
                     [--source-id <source-id>]

# Show details of a specific workbook by resource ID
azmcp workbooks show --workbook-id <workbook-resource-id>

# Update an existing workbook
azmcp workbooks update --workbook-id <workbook-resource-id> \
                       [--display-name <display-name>] \
                       [--serialized-content <json-content>]
```

### Bicep

```bash
# Get Bicep schema for a specific Azure resource type
azmcp bicepschema get --resource-type <resource-type> \
```

## Response Format

All responses follow a consistent JSON format:

```json
{
  "status": "200|403|500, etc",
  "message": "",
  "options": [],
  "results": [],
  "duration": 123
}
```

## Error Handling

The CLI returns structured JSON responses for errors, including:

- Service availability issues
- Authentication errors
