// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

namespace Sable.Cli.Utilities;

public static class FileSystemUtilities
{
    public static string ResolveProjectDirectory(string projectFilePath)
    {
        var projectDirectory = string.IsNullOrWhiteSpace(projectFilePath)
              ? Directory.GetCurrentDirectory()
              : Path.GetDirectoryName(projectFilePath);
        return projectDirectory;
    }

    public static async Task<string> ResolveDatabaseSchemaName(string projectDirectory, string database)
    {
        var databaseSchemaFilePath = Path.Combine(projectDirectory, "sable", database, "schema.txt");
        var databaseSchemaName = await File.ReadAllTextAsync(databaseSchemaFilePath);
        databaseSchemaName = databaseSchemaName.Trim();
        return databaseSchemaName;
    }
}
