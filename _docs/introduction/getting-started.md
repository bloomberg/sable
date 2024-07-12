# Getting Started

## Prerequisites

Before starting, ensure the following prerequisites are met:
- [Docker](https://docs.docker.com/engine/install/) is installed. To learn why Docker is needed, see [How Sable Works](../reference/how-sable-works).
- The **Sable** dotnet tool is installed by running the following command:

```bash
dotnet tool install -g BloombergLP.Sable.Cli
```

See [.NET Tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools) to learn more about how .NET tools work.

## Application Configuration

This guide assumes you have experience with configuring Marten along with its command line tooling support in .NET projects. If that is not the case, please take a look at the following guides before proceeding:
- [Getting Started With Marten](https://martendb.io/getting-started.html)
- [Marten Command Line Tooling](https://martendb.io/configuration/cli.html#command-line-tooling)

Ok. Let's move on. We're going to integrate **Sable** into a new project. To learn how to integrate **Sable** into an exiting project, see [Existing Project Integration](../guide/existing-project-integration) after going through this guide.
The process is similar, so everything learned here will be applicable for existing projects as well.

- Create a new project:
```bash
dotnet new webapi
```
- Configure marten along with its command line tooling support.
- Add **Sable** integration support to the project:
```bash
dotnet add package BloombergLP.Sable
```

Now the fun part. Replace whatever overload of `AddMarten` you're using with `AddMartenWithSableSupport`. That's all there is to it for the integration.

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

## Initialize Migration Infrastructure

Ok. Now that your project is properly configured, what's next? 
In your project directory, run the following command:

```bash 
sable init --database <database-name> --schema <schema-name>
```

The default values for the database and schema names are `Marten` and `public`, respectively.
`Marten` is the name associated with the database for the default configuration. This is important when multiple databases are used in the same project.

Running the command above should have created some migration files in the `./sable/<database-name>/migrations` directory.



## Update Database

Now, to update the database, follow either one of the following strategies:
- Create a migration script that you can then apply manually:

```bash 
sable migrations script --database <database-name>
```

Running the command above should have created a migration script in the `./sable/<database-name>/scripts` directory.
You can now take that script and apply it manually to your database.

- Point **Sable** to the database and have it run the migration it for you:

```bash 
sable database update <connection-string> --database <database-name>
```

Running the command above should have applied the pending migrations to your database.

## Add Migration
Ok. All good so far, but you just added a new index to a document and want to update the database. What do you do?
Pretty simple. Just add a new migration:

```bash 
sable migrations add <migration-name> --database <database-name>
```

Running the command above should have created a new migration file in the `./sable/<database-name>/migrations` directory.

To apply that migration, just follow one of the database update strategies outlined above one more time.


## What's Next?

- Want do see some more sample configurations? See [Sample Configurations](https://github.com/bloomberg/sable/tree/main/samples).

- You have a more complicated configuration with a multi-tenancy setup? See [Multi-Tenancy Setup](../guide/multi-tenancy-setup)

- You have a more complicated configuration with multiple database references? See [Multiple Database Setup](../guide/multiple-database-setup)

- Curious to know how Sable works? See [How Sable Works](../reference/how-sable-works)