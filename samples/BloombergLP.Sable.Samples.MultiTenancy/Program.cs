// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using BloombergLP.Sable.Extensions;
using BloombergLP.Sable.Samples.Core;
using Marten;
using Oakton;
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
