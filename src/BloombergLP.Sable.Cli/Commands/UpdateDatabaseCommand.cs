// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.ComponentModel;
using BloombergLP.Sable.Cli.Settings;
using BloombergLP.Sable.Cli.Utilities;
using Npgsql;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BloombergLP.Sable.Cli.Commands;

public class UpdateDatabaseCommand : AsyncCommand<UpdateDatabaseCommand.Settings>
{
    private readonly IMartenMigrationManager _martenMigrationManager;
    private readonly IConsoleLogger _consoleLogger;

    public UpdateDatabaseCommand(IMartenMigrationManager martenMigrationManager, IConsoleLogger consoleLogger)
    {
        _martenMigrationManager =
            martenMigrationManager ?? throw new ArgumentNullException(nameof(martenMigrationManager));
        _consoleLogger = consoleLogger ?? throw new ArgumentNullException(nameof(consoleLogger));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var script = await _martenMigrationManager.CreateMigrationScript(settings.ProjectFilePath, settings.DatabaseName, to: settings.TargetMigration);
        await using var dataSource = NpgsqlDataSource.Create(settings.ConnectionString);
        await using var command = dataSource.CreateCommand(script);
        await command.ExecuteNonQueryAsync();
        _consoleLogger.LogInfo("Successfully updated the database.");
        return 0;
    }
    public class Settings : ProjectSettings
    {
        [Description("Connection string for the database that is to be updated.")]
        [CommandArgument(0, "<connection-string>")]
        public string ConnectionString { get; set; }

        [Description("Id or name of the latest migration that should be applied. Defaults to the last migration that was generated.")]
        [CommandOption("-m|--migration")]
        public string TargetMigration { get; init; }

        public override ValidationResult Validate()
        {
            var baseResult = base.Validate();
            if (baseResult == ValidationResult.Error())
            {
                return baseResult;
            }

            var validationResult =
                ValidationUtilities.MigrationsInfrastructureHasBeenInitialized(ProjectFilePath, DatabaseName);
            return validationResult;
        }
    }
}
