﻿// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Sable.Extensions;
using Sable.Samples.Core;
using Marten;
using Oakton;
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
        .Index(x => x.Name, i =>
        {
            i.IsConcurrent = true;
        })
        .Index(x => x.Contents);
    return options;
});

var app = builder.Build();
app.MapGet("/", () => "💪🏾");

return await app.RunOaktonCommands(args);
