// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

namespace BloombergLP.Sable.Cli;

public static class SableCliConstants
{
    public const string InfrastructureSetupMigrationName = "InfrastructureSetup";
    public const string InitialMigrationName = "Initial";
    public const string TimeSerializationFormat = "yyyyMMddHHmmss";
    public const string DirectivePrefix = "-- Sable";
    public const string NoTransactionWrapperDirective = $"{DirectivePrefix} NoTransactionWrapper";
    public const string NoIdempotenceWrapperDirective = $"{DirectivePrefix} NoIdempotenceWrapper";
    public const string DefaultDatabaseName = "Marten";
    public const string DefaultDatabaseSchemaName = "public";
}
