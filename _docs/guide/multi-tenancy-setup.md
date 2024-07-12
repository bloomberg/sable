# Multi-tenancy Setup

So, you already went through the [Getting Started](../introduction/getting-started) guide, and may be wondering if there's 
any additional configuration needed for multi-tenancy setups. Given that all Postgres databases for the configured tenants must have identical structures, there's none. Everything you learned in the getting started guide
still applies for any single database setup with multi-tenancy configured. As such, a multi-tenancy setup will look something like this:

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

var app = builder.Build();
app.MapGet("/", () => "💪🏾");

return await app.RunOaktonCommands(args);
```

Again, given that all Postgres databases for the configured tenants must have identical structures, there's no need to generate a different set of migrations for every single database identifier. So, for the example shown above,
you just need to specify the database name as `Marten`, not `books_basic` nor `books_basic`, when running **Sable** commands. That's it.

See [Multi-tencancy Sample](https://martendb.io/configuration/multitenancy.html) for a sample application with a multi-tenancy setup.

To learn more about multi-tenancy in Marten, see [Marten Multi-tencancy](https://martendb.io/configuration/multitenancy.html).