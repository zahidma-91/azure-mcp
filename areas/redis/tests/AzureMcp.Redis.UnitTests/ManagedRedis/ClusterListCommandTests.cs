// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.Json;
using AzureMcp.Core.Models;
using AzureMcp.Core.Models.Command;
using AzureMcp.Core.Options;
using AzureMcp.Redis.Commands.ManagedRedis;
using AzureMcp.Redis.Services;
using AzureMcp.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using ClusterModel = AzureMcp.Redis.Models.ManagedRedis.Cluster;

namespace AzureMcp.Redis.UnitTests.ManagedRedis;

public class ClusterListCommandTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRedisService _redisService;
    private readonly ILogger<ClusterListCommand> _logger;

    public ClusterListCommandTests()
    {
        _redisService = Substitute.For<IRedisService>();
        _logger = Substitute.For<ILogger<ClusterListCommand>>();

        var collection = new ServiceCollection();
        collection.AddSingleton(_redisService);

        _serviceProvider = collection.BuildServiceProvider();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsClusters_WhenClustersExist()
    {
        var expectedClusters = new ClusterModel[] { new() { Name = "cluster1" }, new() { Name = "cluster2" } };
        _redisService.ListClustersAsync("sub123", Arg.Any<string>(), Arg.Any<AuthMethod>(), Arg.Any<RetryPolicyOptions>())
            .Returns(expectedClusters);

        var command = new ClusterListCommand(_logger);
        var args = command.GetCommand().Parse(["--subscription", "sub123"]);
        var context = new CommandContext(_serviceProvider);
        var response = await command.ExecuteAsync(context, args);

        Assert.NotNull(response);
        Assert.Equal(200, response.Status);
        Assert.Equal("Success", response.Message);
        Assert.NotNull(response.Results);

        var json = JsonSerializer.Serialize(response.Results);
        var result = JsonSerializer.Deserialize<ClusterListResult>(json, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Collection(result.Clusters,
            item => Assert.Equal("cluster1", item.Name),
            item => Assert.Equal("cluster2", item.Name));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenNoClusters()
    {
        _redisService.ListClustersAsync("sub123").Returns([]);

        var command = new ClusterListCommand(_logger);
        var parser = new Parser(command.GetCommand());
        var args = parser.Parse(["--subscription", "sub123"]);
        var context = new CommandContext(_serviceProvider);
        var response = await command.ExecuteAsync(context, args);

        Assert.NotNull(response);
        Assert.Null(response.Results);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesException()
    {
        var expectedError = "Test error. To mitigate this issue, please refer to the troubleshooting guidelines here at https://aka.ms/azmcp/troubleshooting.";
        _redisService.ListClustersAsync("sub123", Arg.Any<string>(), Arg.Any<AuthMethod>(), Arg.Any<RetryPolicyOptions>())
            .ThrowsAsync(new Exception("Test error"));

        var command = new ClusterListCommand(_logger);
        var parser = new Parser(command.GetCommand());
        var args = parser.Parse(["--subscription", "sub123"]);
        var context = new CommandContext(_serviceProvider);

        var response = await command.ExecuteAsync(context, args);

        Assert.NotNull(response);
        Assert.Equal(500, response.Status);
        Assert.Equal(expectedError, response.Message);
    }

    [Theory]
    [InlineData("--subscription")]
    public async Task ExecuteAsync_ReturnsError_WhenParameterIsMissing(string missingParameter)
    {
        var command = new ClusterListCommand(_logger);
        var args = command.GetCommand().Parse(
        [
            missingParameter == "--subscription" ? "" : "--subscription", "sub123",
        ]);

        var context = new CommandContext(_serviceProvider);
        var response = await command.ExecuteAsync(context, args);

        Assert.NotNull(response);
        Assert.Equal(400, response.Status);
        Assert.Equal($"Missing Required options: {missingParameter}", response.Message);
    }

    private record ClusterListResult(IEnumerable<ClusterModel> Clusters);
}
