// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, 
			IDataConnectionFactory dataConnectionFactory,
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            builder.Services.AddSingleton(dataConnectionFactory);

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IScopeStore, ScopeStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            var options = new ConfigurationStoreOptions();
            storeOptionsAction?.Invoke(options);
            builder.Services.AddSingleton(options);

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddMemoryCache(); // TODO: remove once update idsvr since it does this
            builder.AddInMemoryCaching();

            // these need to be registered as concrete classes in DI for
            // the caching decorators to work
            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ScopeStore>();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddScopeStoreCache<ScopeStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
			IDataConnectionFactory dataConnectionFactory,
			Action<OperationalStoreOptions> storeOptionsAction = null,
            Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
			builder.Services.AddSingleton(dataConnectionFactory);

			builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var storeOptions = new OperationalStoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            builder.Services.AddSingleton(storeOptions);

            var tokenCleanupOptions = new TokenCleanupOptions();
            tokenCleanUpOptions?.Invoke(tokenCleanupOptions);
            builder.Services.AddSingleton(tokenCleanupOptions);
            builder.Services.AddSingleton<TokenCleanup>();
            
            return builder;
        }

        public static IApplicationBuilder UseIdentityServerEfTokenCleanup(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            var tokenCleanup = app.ApplicationServices.GetService<TokenCleanup>();
            if(tokenCleanup == null)
            {
                throw new InvalidOperationException("AddOperationalStore must be called on the service collection.");
            }
            applicationLifetime.ApplicationStarted.Register(tokenCleanup.Start);
            applicationLifetime.ApplicationStopping.Register(tokenCleanup.Stop);

            return app;
        }
    }
}