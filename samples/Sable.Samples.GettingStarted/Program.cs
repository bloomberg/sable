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
    var options = new StoreOptions();
    options.Connection(builder.Configuration["Databases:Books:BasicTier"]);
    options.DatabaseSchemaName = "books";
    options.AutoCreateSchemaObjects = AutoCreate.None;
    options.Schema.For<Book>()
        .Index(x => x.Name, i =>
        {
            i.IsConcurrent = true;
        })
        .Index(x => x.Contents);
    return options;
});

var app = builder.Build();
app.MapGet("/", () => "💪🏾");

return await app.RunJasperFxCommands(args);
