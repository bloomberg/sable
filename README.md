<p align="center">
  <a href="https://bloomberg.github.io/sable/" target="_blank" rel="noopener noreferrer">
    <img width="180" src="./_docs/public/logo.svg" alt="Sable logo">
  </a>
</p>

# Sable ⚡

> Database Migration Management Tool for Marten

[Sable (*Martes zibellina*)](https://en.wikipedia.org/wiki/Sable) is a species of marten.

## Menu

- [Quick Start](#quick-start)
- [Documentation](#documentation)
- [Contributions](#contributions)
- [License](#license)
- [Code of Conduct](#code-of-conduct)
- [Security Policy](#security-policy)

## Quick Start

### Prerequisites

Before starting, ensure the following prerequisites are met:
- [Docker](https://docs.docker.com/engine/install/) is installed.
- The **Sable** .NET tool is installed by running the following command:

```bash
dotnet tool install -g BloombergLP.Sable.Cli
```

See [.NET tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools) to learn more about how .NET tools work.

### Application Configuration

This guide assumes you have experience with configuring Marten together with its command line tooling support in .NET projects. If that is not the case, please take a look at the following guides before proceeding:
- [Getting Started](https://martendb.io/getting-started.html) with Marten
- [Command Line Tooling](https://martendb.io/configuration/cli.html#command-line-tooling) in Marten

Now we're going to integrate **Sable** into a new project.

- Create a new project:
```bash
dotnet new webapi
```
- Configure Marten along with its command line tooling support.
- Add **Sable** integration support to the project:
```bash
dotnet add package BloombergLP.Sable
```

Now for the fun part. Replace whatever overload of `AddMarten` you're using with `AddMartenWithSableSupport`. That's all it takes to complete the integration.

At this point, you should have a configuration that looks something like this:
```c#
using Marten;
using Oakton;
using BloombergLP.Sable.Extensions;
using BloombergLP.Sable.Samples.Core;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();
builder.Services.AddMartenWithSableSupport(_ =>
{
    var options = new StoreOptions();
    options.Connection(builder.Configuration["Databases:Books:BasicTier"]);
    options.DatabaseSchemaName = "books";
    options.AutoCreateSchemaObjects = AutoCreate.None;
    options.Schema.For<Book>()
        .Index(x => x.Contents);
    return options;
});

var app = builder.Build();
app.MapGet("/", () => "💪🏾");

return await app.RunOaktonCommands(args);
```

### Initialize Migration Infrastructure

Okay. Now that your project is properly configured, what's next?
In your project directory, run the following command:

```bash 
sable init --database <database-name> --schema <schema-name>
```

The default values for the database and schema names are `Marten` and `public`, respectively.
`Marten` is the name associated with the database for the default configuration. This is important, especially when multiple databases are used in the same project.

Running the command above should also have created some migration files in the `./sable/<database-name>/migrations` directory.

### Update Database

Now, to update the database, follow either one of the following stategies:
- Create a migration script that you can apply manually:

```bash 
sable migrations script --database <database-name>
```

Running the command above should have created a migration script in the `./sable/<database-name>/scripts` directory.
You can now take that script and apply it manually to your database.

OR

- Point **Sable** to the database and have it run the migration it for you:

```bash 
sable database update <connection-string> --database <database-name>
```

Running the command above should have applied the pending migrations to your database.

### Add Migration
Okay. Everything is good so far, but you just added a new index to a document and want to update the database. What do you do?
That's pretty simple. Just add a new migration:

```bash 
sable migrations add AddIndexOnName --database <database-name>
```

Running the command above should have created a new migration file in the `./sable/<database-name>/migrations` directory.

To apply that migration, just follow one of the database update strategies outlined above one more time.

## Documentation

To learn more, check out the [documentation](https://bloomberg.github.io/sable/).

## Contributions

We :heart: contributions.

Have you had a good experience with this project? Why not share some love and contribute code, or just let us know about any issues you had with it?

We welcome issue reports [here](../../issues); be sure to choose the proper issue template for your issue, so that we can be sure you're providing the necessary information.

Before sending a [Pull Request](../../pulls), please make sure you read our [Contribution Guidelines](https://github.com/bloomberg/.github/blob/master/CONTRIBUTING.md).

## License

Please read the [LICENSE](LICENSE) file.

## Code of Conduct

This project has adopted a [Code of Conduct](https://github.com/bloomberg/.github/blob/master/CODE_OF_CONDUCT.md).
If you have any concerns about the Code, or behavior which you have experienced in the project, please
contact us at opensource@bloomberg.net.

## Security Policy

- [Security Policy](https://github.com/bloomberg/sable/security/policy)

If you believe you have identified a security vulnerability in this project, you may submit a private vulnerability disclosure.

Please do NOT open an issue in the GitHub repository, as we'd prefer to keep vulnerability reports private until we've had an opportunity to review and address them.

If you have any questions or concerns, please send an email to the Bloomberg OSPO at opensource@bloomberg.net.

---
