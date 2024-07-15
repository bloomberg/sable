// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Sable.Cli.Options;

namespace Sable.Cli;

public interface IMartenMigrationManager
{
    public Task<int> SetupInfrastructure(string projectFilePath, string databaseName, string databaseSchemaName, PostgresContainerOptions postgresContainerOptions);

    public Task<int> AddMigration(string projectFilePath, string databaseName,
        string migrationName, PostgresContainerOptions postgresContainerOptions, bool noIdempotenceWrapper = false, bool noTransactionWrapper = false);

    public Task<string> CreateMigrationScript(string projectFilePath, string databaseName,
        string from = null, string to = null);

    public Task<string> CreateBackfillMigrationScript(string projectFilePath, string databaseName);
}
