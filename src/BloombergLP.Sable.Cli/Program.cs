// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using BloombergLP.Sable.Cli;
using BloombergLP.Sable.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IConsoleLogger, AnsiConsoleLogger>();
serviceCollection.AddSingleton<IMartenMigrationManager, MartenMigrationManager>();
var typeRegistrar = new TypeRegistrar(serviceCollection);
var commandApp = new CommandApp(typeRegistrar);

commandApp.Configure(configurator =>
{
    configurator.SetApplicationName("sable");

    configurator.AddCommand<InitializeInfrastructureCommand>("init")
        .WithDescription("Initialize the migration infrastructure for a database.");

    configurator.AddBranch("migrations", migrations =>
    {
        migrations.SetDescription("Commands to manage migrations.");
        migrations.AddCommand<AddMigrationCommand>("add")
            .WithDescription("Add a new migration for a database.");
        migrations.AddCommand<CreateMigrationScriptCommand>("script")
            .WithDescription("Create an idempotent migration script from existing migrations that can be used to bring a database up to date.");
        migrations.AddCommand<BackfillMigrationsCommand>("backfill")
            .WithDescription(
                "For an existing database that is already up to date, and for which the migration infrastructure has newly been initialized, backfill the newly created migrations.");
    });

    configurator.AddBranch("database", database =>
    {
        database.SetDescription("Commands to manage Marten databases.");
        database.AddCommand<UpdateDatabaseCommand>("update")
            .WithDescription("Use pending migrations to bring a database up to date.");
    });
});

try
{
    commandApp.Run(args);
}
catch (Exception e)
{
    AnsiConsole.WriteException(e,
        ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
        ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
}

