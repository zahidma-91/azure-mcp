// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Core.Commands;
using AzureMcp.Core.Services.Telemetry;
using AzureMcp.Postgres.Commands;
using AzureMcp.Postgres.Options.Server;
using AzureMcp.Postgres.Services;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Postgres.Commands.Server;

public sealed class ServerListCommand(ILogger<ServerListCommand> logger) : BasePostgresCommand<ServerListOptions>(logger)
{
    private const string CommandTitle = "List PostgreSQL Servers";

    public override string Name => "list";

    public override string Description =>
        "Lists all PostgreSQL servers in the specified subscription.";

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = true };

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        try
        {
            var options = BindOptions(parseResult);
            if (!Validate(parseResult.CommandResult, context.Response).IsValid)
            {
                return context.Response;
            }

            context.Activity?.WithSubscriptionTag(options);

            IPostgresService pgService = context.GetService<IPostgresService>() ?? throw new InvalidOperationException("PostgreSQL service is not available.");
            List<string> servers = await pgService.ListServersAsync(options.Subscription!, options.ResourceGroup!, options.User!);
            context.Response.Results = servers?.Count > 0 ?
                ResponseResult.Create(
                    new ServerListCommandResult(servers),
                    PostgresJsonContext.Default.ServerListCommandResult) :
                null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while listing servers");
            HandleException(context, ex);
        }

        return context.Response;
    }

    internal record ServerListCommandResult(List<string> Servers);
}
