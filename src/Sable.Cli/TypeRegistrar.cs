// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Sable.Cli;

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _serviceCollection;

    public TypeRegistrar(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
    }

    public ITypeResolver Build()
    {
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        return new TypeResolver(serviceProvider);
    }

    public void Register(Type serviceType, Type implementationType)
    {
        _serviceCollection.AddSingleton(serviceType, implementationType);
    }

    public void RegisterInstance(Type serviceType, object implementationType)
    {
        _serviceCollection.AddSingleton(serviceType, implementationType);
    }

    public void RegisterLazy(Type serviceType, Func<object> implementationFactory)
    {
        if (implementationFactory is null)
        {
            throw new ArgumentNullException(nameof(implementationFactory));
        }
        _serviceCollection.AddSingleton(serviceType, (_) => implementationFactory());
    }

    public sealed class TypeResolver : ITypeResolver, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        public TypeResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object Resolve(Type serviceType)
        {
            return serviceType == null ? null : _serviceProvider.GetService(serviceType);
        }

        public void Dispose()
        {
            if (_serviceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
    }
}
