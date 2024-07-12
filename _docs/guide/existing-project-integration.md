# Existing Project Integration

So, you've read the [Getting Started](../introduction/getting-started) guide, and may be wondering how to integrate **Sable** into an existing project. It's very simple.

## Prerequisites
- If you have yet to do so, make sure to read the [Getting Started](../introduction/getting-started) guide. The process for integrating Sable into an existing project is very
similar to what is described there, but with a slight twist, so everything learned there will be applicable for existing projects.
- Make sure all of your Postgres databases across every environment where your code is running have already converged to the same latest state.

Once those prerequisites are met, you're all set to go.

## Application Configuration

- Ensure support for sable is configured. Your configuration should look something like the following:
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

The default values for the database and schema are `Marten` and `public`, respectively.
`Marten` is the name associated with the database for the default configuration. This is important when multiple databases are referenced in the same project.

Running the command above should have created some migration file in the `./sable/<database-name>/migrations` directory.

## Backfill Initial Migrations

Once the migration infrastructure has been initialized for the Marten database, there's one more thing to do before proceeding.
The Postgres databases are already up to date, so we must not apply the newly generated migrations. Instead, we'll just backfill them.
**Sable** maintains a table to keep track of applied migrations in the database. 
Whenever a new migration is applied, **Sable** inserts a new record in that table for that migration to ensure it is applied only once.
In our case, since the Postgres databases are already up to date, the newly generated migrations have already been applied without **Sable**, so we'll just backfill them:

```bash 
sable migrations backfill --database <database-name>
```

Running the command above should have created a new migration file in the  `./sable/<database-name>` directory called `<timestamap>_backfill.sql`.
Apply it to your database. That's all it takes to integrate **Sable** into an existing project. From this point on, treat the project as if it had been integrated with **Sable** from the very beginning.

To learn more about how **Sable** works, see [How Sable Works](../reference/how-sable-works).