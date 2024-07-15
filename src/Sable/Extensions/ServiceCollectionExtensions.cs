// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Marten;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace Sable.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register the default store with support for Sable migration management.
    /// </summary>
    /// <param name="configure">Func that will be used to build the options for the store. Takes in the application service provider as input.</param>
    /// <param name="services">Service collection.</param>
    public static MartenServiceCollectionExtensions.MartenConfigurationExpression AddMartenWithSableSupport(
        this IServiceCollection services, Func<IServiceProvider, StoreOptions> configure)
    {
        services.ConfigureMarten(options =>
        {
            var connectionStringOverride =
                Environment.GetEnvironmentVariable(SableConstants.ConnectionStringOverride) ?? "";
            if (string.IsNullOrWhiteSpace(connectionStringOverride))
                return;
            options.Connection(connectionStringOverride);
            options.Advanced.Migrator.TableCreation = CreationStyle.CreateIfNotExists;
        });
        var configurationExpression = services.AddMarten(configure);
        return configurationExpression;
    }

    /// <summary>
    /// Register the default store with support for Sable migration management.
    /// </summary>
    /// <param name="configure">Action that will be used to build the options for the store.</param>
    /// <param name="services">Service collection.</param>
    public static MartenServiceCollectionExtensions.MartenConfigurationExpression AddMartenWithSableSupport(
        this IServiceCollection services, Action<StoreOptions> configure)
    {
        services.ConfigureMarten(options =>
        {
            var connectionStringOverride =
                Environment.GetEnvironmentVariable(SableConstants.ConnectionStringOverride) ?? "";
            if (string.IsNullOrWhiteSpace(connectionStringOverride))
                return;
            options.Connection(connectionStringOverride);
            options.Advanced.Migrator.TableCreation = CreationStyle.CreateIfNotExists;
        });
        var configurationExpression = services.AddMarten(configure);
        return configurationExpression;
    }

    /// <summary>
    /// Register a secondary store with support for Sable migration management.
    /// </summary>
    /// <param name="configure">Action that will be used to build the options for the store.</param>
    /// <param name="services">Service collection.</param>
    public static MartenServiceCollectionExtensions.MartenStoreExpression<T> AddMartenStoreWithSableSupport<T>(this IServiceCollection services,
        Action<StoreOptions> configure) where T : class, IDocumentStore
    {
        services.ConfigureMarten<T>(options =>
        {
            var connectionStringOverride =
                Environment.GetEnvironmentVariable(SableConstants.ConnectionStringOverride) ?? "";
            if (string.IsNullOrWhiteSpace(connectionStringOverride))
                return;
            options.Connection(connectionStringOverride);
            options.Advanced.Migrator.TableCreation = CreationStyle.CreateIfNotExists;
        });
        var configurationExpression = services.AddMartenStore<T>(configure);
        return configurationExpression;
    }

    /// <summary>
    /// Register a secondary store with support for Sable migration management.
    /// </summary>
    /// <param name="configure">Func that will be used to build the options for the store. Takes in the application service provider as input.</param>
    /// <param name="services">Service collection.</param>
    public static MartenServiceCollectionExtensions.MartenStoreExpression<T> AddMartenStoreWithSableSupport<T>(this IServiceCollection services,
        Func<IServiceProvider, StoreOptions> configure) where T : class, IDocumentStore
    {
        services.ConfigureMarten<T>(options =>
        {
            var connectionStringOverride =
                Environment.GetEnvironmentVariable(SableConstants.ConnectionStringOverride) ?? "";
            if (string.IsNullOrWhiteSpace(connectionStringOverride))
                return;
            options.Connection(connectionStringOverride);
            options.Advanced.Migrator.TableCreation = CreationStyle.CreateIfNotExists;
        });
        var configurationExpression = services.AddMartenStore<T>(configure);
        return configurationExpression;
    }
}
