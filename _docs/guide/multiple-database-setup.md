# Multiple Database Setup

After going through the [Getting Started](../introduction/getting-started) guide, you may be wondering how to manage migrations for a project where multiple databases are configured with **Sable**.
It's pretty simple. Sable will just maintain migrations in separate directories for the configured databases. Let's say your configuration looks something like the following:
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
    var options = new StoreOptions
    {
        DatabaseSchemaName = "books"
    };
    options.Connection(builder.Configuration["Databases:Books:BasicTier"]);
    options.MultiTenantedDatabases(x =>
    {
        x.AddMultipleTenantDatabase(builder.Configuration["Databases:Books:GoldTier"], "books_gold")
            .ForTenants("gold1", "gold2");
        x.AddSingleTenantDatabase(builder.Configuration["Databases:Books:SilverTier"], "books_silver");
    });
    options.AutoCreateSchemaObjects = AutoCreate.None;
    options.Schema.For<Book>();
    return options;
});

builder.Services.AddMartenStoreWithSableSupport<IOtherDocumentStore>(_ =>
{
    var options = new StoreOptions
    {
        DatabaseSchemaName = "orders"
    };
    options.Connection(builder.Configuration["Databases:Orders:BasicTier"]);
    options.MultiTenantedDatabases(x =>
    {
        x.AddMultipleTenantDatabase(builder.Configuration["Databases:Books:GoldTier"], "orders_gold")
            .ForTenants("gold1", "gold2");
        x.AddSingleTenantDatabase(builder.Configuration["Databases:Books:SilverTier"], "orders_silver");
    });
    options.AutoCreateSchemaObjects = AutoCreate.None;
    options.Schema.For<Order>();
    return options;
});

var app = builder.Build();
app.MapGet("/", () => "💪🏾");

return await app.RunOaktonCommands(args);
```

When running a **Sable** command, just specify for which database you intend to use it for. For instance, in the example above, you would specify the name of the database as either `Marten` (the default database name) or `IOtherDocumentStore`.
Sable will take care of the rest, and manage migrations for those databases in two separate directories called `Marten` and `IOtherDocumentStore`, respectively.

To learn more about multi-database setups in Marten, see [Marten Multi-Database Setups](https://jeremydmiller.com/2022/03/29/working-with-multiple-marten-databases-in-one-application/).
