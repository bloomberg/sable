// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.ComponentModel;
using BloombergLP.Sable.Cli.Settings;
using BloombergLP.Sable.Cli.Utilities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BloombergLP.Sable.Cli.Commands;

public class BackfillMigrationsCommand : AsyncCommand<BackfillMigrationsCommand.Settings>
{
    private readonly IMartenMigrationManager _martenMigrationManager;
    private readonly IConsoleLogger _consoleLogger;

    public BackfillMigrationsCommand(IMartenMigrationManager martenMigrationManager, IConsoleLogger consoleLogger)
    {
        _martenMigrationManager = martenMigrationManager ?? throw new ArgumentNullException(nameof(martenMigrationManager));
        _consoleLogger = consoleLogger ?? throw new ArgumentNullException(nameof(consoleLogger));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var script = await _martenMigrationManager.CreateBackfillMigrationScript(settings.ProjectFilePath, settings.DatabaseName);
        var scriptFilePath = settings.Output;
        if (string.IsNullOrWhiteSpace(settings.Output))
        {
            var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(settings.ProjectFilePath);
            var currentTime = DateTime.UtcNow;
            var timestamp = currentTime.ToString(SableCliConstants.TimeSerializationFormat);
            var scriptName = $"{timestamp}_backfill.sql";
            scriptFilePath = Path.Combine(projectDirectory, "sable", settings.DatabaseName,
                scriptName);
        }
        await File.WriteAllTextAsync(scriptFilePath, script);
        _consoleLogger.LogInfo($"Successfully saved backfill script to '{scriptFilePath}' file.");
        return 0;
    }


    public class Settings : ProjectSettings
    {
        [Description("Path of the file to save the script to. Defaults to a path within the 'sable' directory tree.")]
        [CommandOption("-o|--output")]
        public string Output { get; init; }
        public override ValidationResult Validate()
        {
            var baseResult = base.Validate();
            if (!baseResult.Successful)
            {
                return baseResult;
            }
            var validationResult =
                ValidationUtilities.MigrationsInfrastructureHasBeenInitialized(ProjectFilePath, DatabaseName);
            return validationResult;
        }
    }
}
