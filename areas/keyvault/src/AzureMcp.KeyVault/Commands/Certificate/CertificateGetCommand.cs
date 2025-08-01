// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Core.Commands;
using AzureMcp.Core.Commands.Subscription;
using AzureMcp.Core.Services.Telemetry;
using AzureMcp.KeyVault.Commands;
using AzureMcp.KeyVault.Options;
using AzureMcp.KeyVault.Options.Certificate;
using AzureMcp.KeyVault.Services;
using Microsoft.Extensions.Logging;

namespace AzureMcp.KeyVault.Commands.Certificate;

public sealed class CertificateGetCommand(ILogger<CertificateGetCommand> logger) : SubscriptionCommand<CertificateGetOptions>
{
    private const string CommandTitle = "Get Key Vault Certificate";
    private readonly ILogger<CertificateGetCommand> _logger = logger;
    private readonly Option<string> _vaultOption = KeyVaultOptionDefinitions.VaultName;
    private readonly Option<string> _certificateOption = KeyVaultOptionDefinitions.CertificateName;

    public override string Name => "get";

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = true };

    public override string Description =>
        """
        Gets a certificate from an Azure Key Vault. This command retrieves and displays details
        about a specific certificate in the specified vault.
        """;

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_vaultOption);
        command.AddOption(_certificateOption);
    }

    protected override CertificateGetOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.VaultName = parseResult.GetValueForOption(_vaultOption);
        options.CertificateName = parseResult.GetValueForOption(_certificateOption);
        return options;
    }

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        var options = BindOptions(parseResult);

        try
        {
            if (!Validate(parseResult.CommandResult, context.Response).IsValid)
            {
                return context.Response;
            }

            context.Activity?.WithSubscriptionTag(options);

            var keyVaultService = context.GetService<IKeyVaultService>();
            var certificate = await keyVaultService.GetCertificate(
                options.VaultName!,
                options.CertificateName!,
                options.Subscription!,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = ResponseResult.Create(
                new CertificateGetCommandResult(
                    certificate.Name,
                    certificate.Id,
                    certificate.KeyId,
                    certificate.SecretId,
                    Convert.ToBase64String(certificate.Cer),
                    certificate.Properties.X509ThumbprintString,
                    certificate.Properties.Enabled,
                    certificate.Properties.NotBefore,
                    certificate.Properties.ExpiresOn,
                    certificate.Properties.CreatedOn,
                    certificate.Properties.UpdatedOn,
                    certificate.Policy.Subject,
                    certificate.Policy.IssuerName),
                KeyVaultJsonContext.Default.CertificateGetCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting certificate {CertificateName} from vault {VaultName}", options.CertificateName, options.VaultName);
            HandleException(context, ex);
        }

        return context.Response;
    }

    internal record CertificateGetCommandResult(string Name, Uri Id, Uri KeyId, Uri SecretId, string Cer, string Thumbprint, bool? Enabled, DateTimeOffset? NotBefore, DateTimeOffset? ExpiresOn, DateTimeOffset? CreatedOn, DateTimeOffset? UpdatedOn, string Subject, string IssuerName);
}
