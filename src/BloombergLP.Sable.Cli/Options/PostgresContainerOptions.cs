// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.


namespace BloombergLP.Sable.Cli.Options;

public class PostgresContainerOptions
{
    public string Image { get; set; } = "postgres:15.1";

    public List<PortBinding> PortBindings { get; set; } = new()
    {
        new PortBinding { HostPort = 5470, ContainerPort = 5432 }
    };

    public Dictionary<string, string> EnvironmentVariables { get; set; } = new()
    {
        { "PGPORT", "5432" },
        { "POSTGRES_DB", "postgres" },
        { "POSTGRES_USER", "postgres" },
        { "POSTGRES_PASSWORD", "postgres" },
    };

    public string ConnectionString { get; set; } =
        "Host=localhost;Port=5470;Username=postgres;Password=postgres;Database=postgres";
}

public class PortBinding
{
    public int HostPort { get; set; }
    public int ContainerPort { get; set; }
}
