// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.ComponentModel;
using System.Text.RegularExpressions;
using Sable.Cli.Options;
using Sable.Cli.Settings;
using Sable.Cli.Utilities;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Sable.Cli.Commands;

public class AddMigrationCommand : AsyncCommand<AddMigrationCommand.Settings>
{
    private readonly IMartenMigrationManager _martenMigrationManager;

    public AddMigrationCommand(IMartenMigrationManager martenMigrationManager)
    {
        _martenMigrationManager =
            martenMigrationManager ?? throw new ArgumentNullException(nameof(martenMigrationManager));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var result = await _martenMigrationManager.AddMigration(settings.ProjectFilePath, settings.DatabaseName, settings.Name, settings.PostgresContainerOptions, settings.NoIdempotenceWrapper, settings.NoTransactionWrapper);
        return result;
    }

    public class Settings : ProjectSettings
    {
        [Description("The name of the migration.")]
        [CommandArgument(0, "<name>")]
        public string Name { get; set; }

        [Description("Path to a JSON file that contains options for buiding a custom Postgres container that is used as the shadow database for migration management.")]
        [CommandOption("-c|--container-options")]
        public string ContainerOptionsFilePath { get; init; }

        [Description("By default, when embedding a migration as part of a larger, aggregate migration script, Sable will wrap it in an anynomous function block to ensure it is executed idemtotently." +
                     "Additionally, that code block will then be wrapped in a trasaction block to ensure the entire migration is executed in a single atomic operation." +
                     "However, some Postgres statements must not be executed within a trasaction. For a migration that contains those type of statements, this flag must be set to" +
                     "avoid running into issues when generating migration scripts. This option isreserved for advanced use cases. Do not use it unless you know what you are doing.")]
        [CommandOption("--no-transaction-wrapper")]
        public bool NoTransactionWrapper { get; init; } = false;

        [Description("By default, when embedding a migration as part of a larger, aggregate migration script, Sable will wrap it in an anynomous function block to ensure it is executed idemtotently." +
                     "However, some Postgres statements must not be executed within such a block. For a migration that contains those type of statements, this flag must be set to" +
                     "avoid running into issues when generating migration scripts. Additionally, given that those statements will execute outside of indempotent context, " +
                     "they must be made to be indempotent (e.g., `CREATE INDEX CONCURRENTLY IF NOT EXISTS my_index ON my_table (column_name);` instead of `CREATE INDEX CONCURRENTLY my_index ON my_table (column_name);`)." +
                     ". This option isreserved for advanced use cases. Do not use it unless you know what you are doing.")]
        [CommandOption("--no-idempotence-wrapper")]
        public bool NoIdempotenceWrapper { get; init; } = false;

        public PostgresContainerOptions PostgresContainerOptions { get; private set; } = new();

        public override ValidationResult Validate()
        {
            if (!string.IsNullOrWhiteSpace(ContainerOptionsFilePath))
            {
                var fileContents = File.ReadAllText(ContainerOptionsFilePath);
                PostgresContainerOptions = JsonConvert.DeserializeObject<PostgresContainerOptions>(fileContents);
            }

            var baseResult = base.Validate();
            if (baseResult == ValidationResult.Error())
            {
                return baseResult;
            }

            var nameIsValid = Regex.IsMatch(Name, "^[a-zA-Z0-9]+$");
            if (!nameIsValid)
            {
                return ValidationResult.Error("The migration name must contain only alphanumeric characters.");
            }

            var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(ProjectFilePath);
            var migrationsDirectory = Path.Combine(projectDirectory, "sable", DatabaseName, "migrations");
            var existingMigrationNames =
                Directory.EnumerateFiles(migrationsDirectory, "*.sql", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileNameWithoutExtension)
                    .Select(n => n.Split("_").Last())
                    .ToHashSet();
            if (existingMigrationNames.Contains(Name))
            {
                return ValidationResult.Error("A migration with the specified name already exists.");
            }

            var validationResult =
                ValidationUtilities.MigrationsInfrastructureHasBeenInitialized(ProjectFilePath, DatabaseName);
            return validationResult;
        }
    }
}
