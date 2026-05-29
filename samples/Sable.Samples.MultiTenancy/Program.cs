// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using JasperFx;
using Marten;
using Sable.Extensions;
using Sable.Samples.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

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

return await app.RunJasperFxCommands(args);
