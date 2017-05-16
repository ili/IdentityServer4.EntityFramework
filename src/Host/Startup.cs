// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using Host.Configuration;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Interfaces;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Validation;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Host
{
	public class Startup
	{
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			// Linq To DB specific
			var factory = DataConnectionFactory.Instance;

			services.AddIdentityServer()
				.AddTemporarySigningCredential()
				.AddSecretParser<ClientAssertionSecretParser>()
				.AddSecretValidator<PrivateKeyJwtSecretValidator>()
				.AddTestUsers(TestUsers.Users)

				// Linq To DB specific
				.AddConfigurationStore(factory)
				.AddOperationalStore(factory);

			return services.BuildServiceProvider(true);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime,
			ILoggerFactory loggerFactory)
		{
			// serilog filter
			Func<LogEvent, bool> serilogFilter = e =>
			{
				var context = e.Properties["SourceContext"].ToString();

				return context.StartsWith("\"IdentityServer") ||
				       context.StartsWith("\"IdentityModel") ||
				       e.Level == LogEventLevel.Error ||
				       e.Level == LogEventLevel.Fatal;
			};

			var serilog = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.Enrich.FromLogContext()
				.Filter.ByIncludingOnly(serilogFilter)
				.WriteTo.LiterateConsole(
					outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
				.WriteTo.File(@"c:\logs\IdentityServer4.EntityFramework.Host.txt")
				.CreateLogger();

			loggerFactory.AddSerilog(serilog);

			// Setup Databases
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				EnsureSeedData(serviceScope.ServiceProvider.GetService<IDataConnectionFactory>());
			}

			app.UseIdentityServer();
			app.UseIdentityServerEfTokenCleanup(applicationLifetime);

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();
		}

		private static void EnsureSeedData(IDataConnectionFactory context)
		{
			using (var db = context.GetConnection())
			{
				db.CreateTable<ApiResource>();
				db.CreateTable<ApiResourceClaim>();
				db.CreateTable<ApiScope>();
				db.CreateTable<ApiScopeClaim>();
				db.CreateTable<ApiSecret>();
				db.CreateTable<Client>();
				db.CreateTable<ClientClaim>();
				db.CreateTable<ClientCorsOrigin>();
				db.CreateTable<ClientGrantType>();
				db.CreateTable<ClientIdentityProviderRestrictions>();
				db.CreateTable<ClientPostLogoutRedirectUri>();
				db.CreateTable<ClientRedirectUri>();
				db.CreateTable<ClientScope>();
				db.CreateTable<ClientSecret>();
				db.CreateTable<IdentityClaim>();
				db.CreateTable<IdentityResource>();
				db.CreateTable<Secret>();


				foreach (var client in Clients.Get())
					db.ComplexInsert(client);

				db.BulkCopy(Resources.GetIdentityResources());
				db.BulkCopy(Resources.GetApiResources().ToList());
			}
		}
	}
}