// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.Text;
using Sable.Cli.Extensions;
using Sable.Cli.Options;
using Sable.Cli.Utilities;
using CliWrap;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Scriban;

namespace Sable.Cli;

public class MartenMigrationManager : IMartenMigrationManager
{
    private readonly IConsoleLogger _consoleLogger;

    public MartenMigrationManager(IConsoleLogger consoleLogger)
    {
        _consoleLogger = consoleLogger ?? throw new ArgumentNullException(nameof(consoleLogger));
    }

    public async Task<int> SetupInfrastructure(string projectFilePath, string databaseName, string databaseSchemaName, PostgresContainerOptions postgresContainerOptions)
    {
        var currentTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        var timestamp = currentTime.ToString(SableCliConstants.TimeSerializationFormat);
        var migrationFileName = $"{timestamp}_{SableCliConstants.InfrastructureSetupMigrationName}.sql";
        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(projectFilePath);
        var migrationFilePath = Path.Combine(projectDirectory, "sable", databaseName, "migrations",
            migrationFileName);
        var fileInfo = new FileInfo(migrationFilePath);
        var migrationDirectory = fileInfo.DirectoryName;
        Directory.CreateDirectory(migrationDirectory!);
        var migrationId = migrationFileName.Replace(".sql", "");
        var template = Template.Parse(Templates.InfrastructureSetupTemplate);
        var migration = await template.RenderAsync(new
        {
            DatabaseSchemaName = databaseSchemaName,
            MigrationId = migrationId,
            Date = currentTime.ToString(),
        }, member => member.Name);
        await File.WriteAllTextAsync(migrationFilePath, migration);

        var databaseSchemaNameFilePath = Path.Combine(projectDirectory, "sable", databaseName, "schema.txt");
        await File.WriteAllTextAsync(databaseSchemaNameFilePath, databaseSchemaName);

        var result = await AddMigration(projectFilePath, databaseName, SableCliConstants.InitialMigrationName, postgresContainerOptions);
        if (result != 0)
        {
            _consoleLogger.LogError("Failed to create initial migration.");
            return result;
        }
        _consoleLogger.LogInfo($"Successfully initialized migration infrastructure in '{Path.Combine(projectDirectory, "sable", databaseName)}' directory.");
        return 0;
    }

    private IContainer CreatePostgresContainer(PostgresContainerOptions containerOptions)
    {
        var readinessProbeStrategy = new ReadinessProbeWaitStrategy(containerOptions.ConnectionString);
        var waitStrategy = Wait
            .ForUnixContainer()
            .AddCustomWaitStrategy(readinessProbeStrategy);
        var containerBuilder = new ContainerBuilder()
            .WithImage(containerOptions.Image)
            .WithEnvironment(containerOptions.EnvironmentVariables)
            .WithWaitStrategy(waitStrategy);
        foreach (var portBinding in containerOptions.PortBindings)
        {
            containerBuilder = containerBuilder.WithPortBinding(portBinding.HostPort, portBinding.ContainerPort);
        }
        var container = containerBuilder.Build();
        return container;
    }

    public async Task<int> AddMigration(string projectFilePath, string databaseName, string migrationName,
        PostgresContainerOptions postgresContainerOptions, bool noIdempotenceWrapper = false,
        bool noTransactionWrapper = false)
    {
        await using var container = CreatePostgresContainer(postgresContainerOptions);
        await container.StartAsync();
        await using var dataSource = NpgsqlDataSource.Create(postgresContainerOptions.ConnectionString);
        var script = await CreateMigrationScript(projectFilePath, databaseName);
        await using var command = dataSource.CreateCommand(script);
        await command.ExecuteNonQueryAsync();

        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(projectFilePath);
        var currentTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        var timestamp = currentTime.ToString(SableCliConstants.TimeSerializationFormat);
        var migrationFileName = $"{timestamp}_{migrationName}.sql";
        var migrationFilePath =
            Path.Combine(projectDirectory, "sable", databaseName, "migrations", migrationFileName);
        //projectFilePath = projectFilePath.Replace(@"\\", @"\").Replace(@"\", @"\\");
        //migrationFilePath = migrationFilePath.Replace(@"\\", @"\").Replace(@"\", @"\\");
        var patchCommandExecutionResult = await CliWrap.Cli.Wrap("dotnet")
            .WithArguments(new[] { "run", "--project", projectFilePath, "--", "marten-patch", "--database", $"{databaseName}", migrationFilePath })
            .WithWorkingDirectory(projectDirectory)
            .WithEnvironmentVariables(new Dictionary<string, string>
            {
                [SableConstants.ConnectionStringOverride] = postgresContainerOptions.ConnectionString
            })
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
        if (patchCommandExecutionResult.ExitCode != 0)
        {
            _consoleLogger.LogError("Failed to add migration.");
            return patchCommandExecutionResult.ExitCode;
        }

        var migrationBuilder = new StringBuilder();
        migrationBuilder.AppendLine($"-- Generated by Sable on {currentTime}");
        if (noIdempotenceWrapper)
        {
            migrationBuilder.AppendLine($"{SableCliConstants.NoIdempotenceWrapperDirective}");
        }
        if (noTransactionWrapper)
        {
            migrationBuilder.AppendLine($"{SableCliConstants.NoTransactionWrapperDirective}");
        }

        var changeDetected = File.Exists(migrationFilePath);
        if (changeDetected)
        {
            var dropFilePath = migrationFilePath.Replace(".sql", ".drop.sql");
            File.Delete(dropFilePath);
            migrationBuilder.AppendLine();
            var migrationContents = await File.ReadAllTextAsync(migrationFilePath);
            migrationBuilder.Append(migrationContents);
            var enrichedMigration = migrationBuilder.ToString();
            await File.WriteAllTextAsync(migrationFilePath, enrichedMigration);
        }
        else
        {
            var emptyMigration = migrationBuilder.ToString();
            await File.WriteAllTextAsync(migrationFilePath, emptyMigration);
        }
        _consoleLogger.LogInfo($"Successfully saved migration to '{migrationFilePath}' file.");
        return 0;
    }

    public async Task<string> CreateMigrationScript(string projectFilePath, string databaseName, string from = null, string to = null)
    {
        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(projectFilePath);
        var databaseSchemaName = await FileSystemUtilities.ResolveDatabaseSchemaName(projectDirectory, databaseName);
        var scriptDirectoryPath = Path.Combine(projectDirectory, "sable", databaseName, "migrations");
        var migrations = Directory.EnumerateFiles(scriptDirectoryPath, "*.sql", SearchOption.TopDirectoryOnly)
            .Select(p => new Migration(p, databaseSchemaName))
            .OrderBy(m => m.Timestamp)
            .ToList();
        if (!string.IsNullOrWhiteSpace(from))
        {
            migrations = migrations
                .SkipWhile(m => m.Id != from && m.Name != from)
                .ToList();
        }
        if (!string.IsNullOrWhiteSpace(to))
        {
            migrations = migrations.TakeWhile(m => m.Id != from && m.Name != from, true).ToList();
        }
        var transactions = migrations
            .OrderBy(m => m.Timestamp)
            .Select(m => m.GetTransactionalIdempotentScript())
            .ToList();
        var scriptBuilder = new StringBuilder();
        scriptBuilder.Append($"-- Generated by Sable on {DateTime.UtcNow}{Environment.NewLine}");
        foreach (var transactionSegment in transactions.Select(transaction => $"{Environment.NewLine}{transaction}"))
        {
            scriptBuilder.AppendLine(transactionSegment);
        }
        var script = scriptBuilder.ToString();
        return script;
    }

    public async Task<string> CreateBackfillMigrationScript(string projectFilePath, string databaseName)
    {
        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(projectFilePath);
        var databaseSchemaName = await FileSystemUtilities.ResolveDatabaseSchemaName(projectDirectory, databaseName);
        var scriptDirectoryPath = Path.Combine(projectDirectory, "sable", databaseName, "migrations");
        var migrationInsertionScripts = Directory.EnumerateFiles(scriptDirectoryPath, "*.sql", SearchOption.TopDirectoryOnly)
            .Select(p => new Migration(p, databaseSchemaName))
            .OrderBy(m => m.Timestamp)
            .Select(m => m.GetIdempotentMigrationRecordInsertionScript(true))
            .ToList();
        var template = Template.Parse(Templates.BackfillScriptTemplate);
        var script = await template.RenderAsync(new
        {
            Date = DateTime.UtcNow.ToString(),
            DatabaseSchemaName = databaseSchemaName,
            MigrationInsertionScripts = migrationInsertionScripts
        }, member => member.Name);
        return script;
    }
}
