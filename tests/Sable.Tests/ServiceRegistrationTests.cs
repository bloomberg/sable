// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Sable.Extensions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;
using Xunit;

namespace Sable.Tests;

public class ServiceRegistrationTests
{
    [Fact]
    public Task EnsureConfigurationIsOverridenCase1()
    {
        Environment.SetEnvironmentVariable(SableConstants.ConnectionStringOverride,
            "Host=localhost;Port=5432;Database=sable;Username=postgres;password=P0stG&e$;SSL Mode=Disable",
            EnvironmentVariableTarget.Process);
        var services = new ServiceCollection();
        services.AddMartenWithSableSupport(_ =>
        {
            var options = new StoreOptions();
            options.Advanced.Migrator.TableCreation = CreationStyle.DropThenCreate;
            return options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var storeOptions = serviceProvider.GetRequiredService<StoreOptions>();
        var actualCreationStyle = storeOptions.Advanced.Migrator.TableCreation;
        Assert.Equal(CreationStyle.CreateIfNotExists, actualCreationStyle);
        return Task.CompletedTask;
    }

    [Fact]
    public Task EnsureConfigurationIsOverridenCase2()
    {
        Environment.SetEnvironmentVariable(SableConstants.ConnectionStringOverride,
            "Host=localhost;Port=5432;Database=sable;Username=postgres;password=P0stG&e$;SSL Mode=Disable",
            EnvironmentVariableTarget.Process);
        var services = new ServiceCollection();
        services.AddMartenWithSableSupport(options =>
        {
            options.Advanced.Migrator.TableCreation = CreationStyle.DropThenCreate;
        });
        var serviceProvider = services.BuildServiceProvider();
        var storeOptions = serviceProvider.GetRequiredService<StoreOptions>();
        var actualCreationStyle = storeOptions.Advanced.Migrator.TableCreation;
        Assert.Equal(CreationStyle.CreateIfNotExists, actualCreationStyle);
        return Task.CompletedTask;
    }

    [Fact]
    public Task EnsureConfigurationIsOverridenCase3()
    {
        Environment.SetEnvironmentVariable(SableConstants.ConnectionStringOverride,
            "Host=localhost;Port=5432;Database=sable;Username=postgres;password=P0stG&e$;SSL Mode=Disable",
            EnvironmentVariableTarget.Process);
        var services = new ServiceCollection();
        services.AddMartenStoreWithSableSupport<IOtherDatabase>(_ =>
        {
            var options = new StoreOptions();
            options.Advanced.Migrator.TableCreation = CreationStyle.DropThenCreate;
            return options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var otherDatabase = serviceProvider.GetRequiredService<IOtherDatabase>();
        var actualCreationStyle = ((AdvancedOptions)otherDatabase.Options.Advanced).Migrator.TableCreation;
        Assert.Equal(CreationStyle.CreateIfNotExists, actualCreationStyle);
        return Task.CompletedTask;
    }

    [Fact]
    public Task EnsureConfigurationIsOverridenCase4()
    {
        Environment.SetEnvironmentVariable(SableConstants.ConnectionStringOverride,
            "Host=localhost;Port=5432;Database=sable;Username=postgres;password=P0stG&e$;SSL Mode=Disable",
            EnvironmentVariableTarget.Process);
        var services = new ServiceCollection();
        services.AddMartenStoreWithSableSupport<IOtherDatabase>(options =>
        {
            options.Advanced.Migrator.TableCreation = CreationStyle.DropThenCreate;
        });
        var serviceProvider = services.BuildServiceProvider();
        var otherDatabase = serviceProvider.GetRequiredService<IOtherDatabase>();
        var actualCreationStyle = ((AdvancedOptions)otherDatabase.Options.Advanced).Migrator.TableCreation;
        Assert.Equal(CreationStyle.CreateIfNotExists, actualCreationStyle);
        return Task.CompletedTask;
    }
}
