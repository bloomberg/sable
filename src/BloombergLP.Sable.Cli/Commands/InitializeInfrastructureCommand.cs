// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.ComponentModel;
using BloombergLP.Sable.Cli.Options;
using BloombergLP.Sable.Cli.Settings;
using BloombergLP.Sable.Cli.Utilities;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BloombergLP.Sable.Cli.Commands;

public class InitializeInfrastructureCommand : AsyncCommand<InitializeInfrastructureCommand.Settings>
{
    private readonly IMartenMigrationManager _martenMigrationManager;

    public InitializeInfrastructureCommand(IMartenMigrationManager martenMigrationManager)
    {
        _martenMigrationManager = martenMigrationManager ?? throw new ArgumentNullException(nameof(martenMigrationManager));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var result = await _martenMigrationManager.SetupInfrastructure(settings.ProjectFilePath, settings.DatabaseName,
            settings.DatabaseSchemaName, settings.PostgresContainerOptions);
        return result;
    }

    public class Settings : ProjectSettings
    {
        [Description("Name of the database schema. Defaults to the 'public' schema.")]
        [CommandOption("-s|--schema")]
        public string DatabaseSchemaName { get; init; } = SableCliConstants.DefaultDatabaseSchemaName;

        [Description("Path to a JSON file that contains options for buiding a custom Postgres container that is used as the shadow database for migration management.")]
        [CommandOption("-c|--container-options")]
        public string ContainerOptionsFilePath { get; init; }

        public PostgresContainerOptions PostgresContainerOptions { get; private set; } = new();
        public override ValidationResult Validate()
        {
            if (!string.IsNullOrWhiteSpace(ContainerOptionsFilePath))
            {
                var fileContents = File.ReadAllText(ContainerOptionsFilePath);
                PostgresContainerOptions = JsonConvert.DeserializeObject<PostgresContainerOptions>(fileContents);
            }

            var baseResult = base.Validate();
            if (!baseResult.Successful)
            {
                return baseResult;
            }

            var validationResult = ValidationUtilities.MigrationsInfrastructureHasBeenInitialized(ProjectFilePath, DatabaseName);
            return !validationResult.Successful
                ? ValidationResult.Success()
                : ValidationResult.Error($"The migration infrastructure has already been initialized for the '{DatabaseName}' database.");
        }
    }
}
