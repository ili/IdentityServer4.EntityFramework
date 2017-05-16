// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.LinqToDB;
using IdentityServer4.LinqToDB.Interfaces;
using IdentityServer4.LinqToDB.Services;
using IdentityServer4.LinqToDB.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
	public static class IdentityServerEntityFrameworkBuilderExtensions
	{
		public static IIdentityServerBuilder AddConfigurationStore(
			this IIdentityServerBuilder builder,
			IDataConnectionFactory dataConnectionFactory)
		{
			builder.Services.AddSingleton(dataConnectionFactory);

			builder.Services.AddTransient<IClientStore, ClientStore>();
			builder.Services.AddTransient<IResourceStore, ResourceStore>();
			builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

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
			builder.Services.AddTransient<ResourceStore>();

			// add the caching decorators
			builder.AddClientStoreCache<ClientStore>();
			builder.AddResourceStoreCache<ResourceStore>();

			return builder;
		}

		public static IIdentityServerBuilder AddOperationalStore(
			this IIdentityServerBuilder builder,
			IDataConnectionFactory dataConnectionFactory,
			Action<TokenCleanupOptions> initTokenCleanUpOptions = null)
		{
			builder.Services.AddSingleton(dataConnectionFactory);

			builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

			var tokenCleanupOptions = new TokenCleanupOptions();
			initTokenCleanUpOptions?.Invoke(tokenCleanupOptions);

			builder.Services.AddSingleton(tokenCleanupOptions);
			builder.Services.AddSingleton<TokenCleanup>();

			return builder;
		}

		public static IApplicationBuilder UseIdentityServerEfTokenCleanup(this IApplicationBuilder app,
			IApplicationLifetime applicationLifetime)
		{
			var tokenCleanup = app.ApplicationServices.GetService<TokenCleanup>();
			if (tokenCleanup == null)
				throw new InvalidOperationException("AddOperationalStore must be called on the service collection.");
			applicationLifetime.ApplicationStarted.Register(tokenCleanup.Start);
			applicationLifetime.ApplicationStopping.Register(tokenCleanup.Stop);

			return app;
		}
	}
}