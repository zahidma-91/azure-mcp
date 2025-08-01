// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using AzureMcp.Storage.Commands.Account;
using AzureMcp.Storage.Commands.Blob;
using AzureMcp.Storage.Commands.Blob.Container;
using AzureMcp.Storage.Commands.DataLake.Directory;
using AzureMcp.Storage.Commands.DataLake.FileSystem;
using AzureMcp.Storage.Commands.Table;

namespace AzureMcp.Storage.Commands;

[JsonSerializable(typeof(BlobListCommand.BlobListCommandResult))]
[JsonSerializable(typeof(AccountListCommand.Result), TypeInfoPropertyName = "AccountListCommandResult")]
[JsonSerializable(typeof(TableListCommand.TableListCommandResult))]
[JsonSerializable(typeof(ContainerListCommand.ContainerListCommandResult))]
[JsonSerializable(typeof(ContainerDetailsCommand.ContainerDetailsCommandResult))]
[JsonSerializable(typeof(FileSystemListPathsCommand.FileSystemListPathsCommandResult))]
[JsonSerializable(typeof(DirectoryCreateCommand.DirectoryCreateCommandResult))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class StorageJsonContext : JsonSerializerContext
{
}
