// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.Json;
using System.Text.Json.Serialization;
using AzureMcp.Core.Models;
using AzureMcp.Core.Models.Command;
using AzureMcp.Core.Options;
using AzureMcp.Storage.Commands.Table;
using AzureMcp.Storage.Services;
using AzureMcp.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace AzureMcp.Storage.UnitTests.Table;

public class TableListCommandTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IStorageService _storageService;
    private readonly ILogger<TableListCommand> _logger;
    private readonly TableListCommand _command;
    private readonly CommandContext _context;
    private readonly Parser _parser;
    private readonly string _knownAccountName = "account123";
    private readonly string _knownSubscriptionId = "sub123";

    public TableListCommandTests()
    {
        _storageService = Substitute.For<IStorageService>();
        _logger = Substitute.For<ILogger<TableListCommand>>();

        var collection = new ServiceCollection().AddSingleton(_storageService);

        _serviceProvider = collection.BuildServiceProvider();
        _command = new(_logger);
        _context = new(_serviceProvider);
        _parser = new(_command.GetCommand());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsTables()
    {
        // Arrange
        var expectedTables = new List<string> { "table1", "table2" };

        _storageService.ListTables(Arg.Is(_knownAccountName), Arg.Is(_knownSubscriptionId),
            Arg.Is(AuthMethod.Credential), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<RetryPolicyOptions>())
                .Returns(expectedTables);

        var args = _parser.Parse([
            "--account-name", _knownAccountName,
            "--subscription", _knownSubscriptionId,
            "--auth-method", AuthMethod.Credential.ToString().ToLowerInvariant()
        ]);

        // Act
        var response = await _command.ExecuteAsync(_context, args);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        var json = JsonSerializer.Serialize(response.Results);
        var result = JsonSerializer.Deserialize<TableListResult>(json);

        Assert.NotNull(result);
        Assert.Equal(expectedTables, result.Tables);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenNoTables()
    {
        // Arrange
        _storageService.ListTables(Arg.Is(_knownAccountName), Arg.Is(_knownSubscriptionId),
            Arg.Is(AuthMethod.Credential), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<RetryPolicyOptions>())
            .Returns([]);

        var args = _parser.Parse([
            "--account-name", _knownAccountName,
            "--subscription", _knownSubscriptionId,
            "--auth-method", AuthMethod.Credential.ToString().ToLowerInvariant()
        ]);

        // Act
        var response = await _command.ExecuteAsync(_context, args);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Results);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesException()
    {
        // Arrange
        var expectedError = "Test error";

        _storageService.ListTables(Arg.Is(_knownAccountName), Arg.Is(_knownSubscriptionId),
            Arg.Is(AuthMethod.Credential), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<RetryPolicyOptions>())
            .ThrowsAsync(new Exception(expectedError));

        var args = _command.GetCommand().Parse([
            "--account-name", _knownAccountName,
            "--subscription", _knownSubscriptionId,
            "--auth-method", AuthMethod.Credential.ToString().ToLowerInvariant()
        ]);

        // Act
        var response = await _command.ExecuteAsync(_context, args);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(500, response.Status);
        Assert.StartsWith(expectedError, response.Message);
    }

    private class TableListResult
    {
        [JsonPropertyName("tables")]
        public List<string> Tables { get; set; } = [];
    }
}
