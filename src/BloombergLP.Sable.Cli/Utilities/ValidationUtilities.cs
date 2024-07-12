// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Spectre.Console;

namespace BloombergLP.Sable.Cli.Utilities;

public static class ValidationUtilities
{
    public static ValidationResult MigrationsInfrastructureHasBeenInitialized(string projectFilePath, string databaseName)
    {
        var projectDirectory = FileSystemUtilities.ResolveProjectDirectory(projectFilePath);
        var infrastructureRootPath = Path.Combine(projectDirectory, "sable", databaseName);
        var initialized = Directory.Exists(infrastructureRootPath);
        return initialized
            ? ValidationResult.Success()
            : ValidationResult.Error($"The migration infrastructure must be initialized with the 'init' command for the '{databaseName}' database before this command can be run.");
    }
}
