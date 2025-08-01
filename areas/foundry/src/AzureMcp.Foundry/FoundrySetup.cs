// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Core.Areas;
using AzureMcp.Core.Commands;
using AzureMcp.Foundry.Commands;
using AzureMcp.Foundry.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Foundry;

public class FoundrySetup : IAreaSetup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFoundryService, FoundryService>();
    }

    public void RegisterCommands(CommandGroup rootGroup, ILoggerFactory loggerFactory)
    {
        var foundry = new CommandGroup("foundry", "Foundry service operations - Commands for listing and managing services and resources in AI Foundry.");
        rootGroup.AddSubGroup(foundry);

        var models = new CommandGroup("models", "Foundry models operations - Commands for listing and managing models in AI Foundry.");
        foundry.AddSubGroup(models);

        var deployments = new CommandGroup("deployments", "Foundry models deployments operations - Commands for listing and managing models deployments in AI Foundry.");
        models.AddSubGroup(deployments);

        deployments.AddCommand("list", new DeploymentsListCommand());

        models.AddCommand("list", new ModelsListCommand());
        models.AddCommand("deploy", new ModelDeploymentCommand());
    }
}
