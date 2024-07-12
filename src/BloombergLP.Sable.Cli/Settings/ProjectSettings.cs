// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BloombergLP.Sable.Cli.Settings;

public class ProjectSettings : CommandSettings
{
    [Description("Path to the project file of the Marten project. Defaults to using the project file in the current directory if it's the only one in there.")]
    [CommandOption("-p|--project")]
    public string ProjectFilePath { get; private set; }

    [Description("Which database to use. Defaults to the 'Marten' database.")]
    [CommandOption("-d|--database")]
    public string DatabaseName { get; init; } = SableCliConstants.DefaultDatabaseName;

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(ProjectFilePath))
        {
            var projectDirectory = Directory.GetCurrentDirectory();
            var projectFiles = Directory.EnumerateFiles(projectDirectory, "*.csproj", SearchOption.TopDirectoryOnly)
                .ToList();
            if (projectFiles.Count != 1)
            {
                return ValidationResult.Error("The path to the project file must be specified.");
            }
            ProjectFilePath = projectFiles.First();
        }
        ProjectFilePath = Path.GetFullPath(ProjectFilePath);
        var fileExists = File.Exists(ProjectFilePath);
        return fileExists
            ? ValidationResult.Success()
            : ValidationResult.Error("The specified project file does not exist.");
    }
}
