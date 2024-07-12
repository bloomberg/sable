// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Npgsql;

namespace BloombergLP.Sable.Cli;

public class ReadinessProbeWaitStrategy : IWaitUntil
{
    private readonly string _connectionString;

    public ReadinessProbeWaitStrategy(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }
    public async Task<bool> UntilAsync(IContainer container)
    {
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        await using var command = dataSource.CreateCommand();
        command.CommandText = "SELECT 1;";
        try
        {
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
