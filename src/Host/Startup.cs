// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Host.Configuration;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using LinqToDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

namespace Host
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			const string connectionString =
				@"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.LinqToDB;trusted_connection=yes;";

			DataConnection.AddConfiguration("Default", connectionString, SqlServerTools.GetDataProvider(SqlServerVersion.v2012));
			DataConnection.DefaultConfiguration = "Default";

			services.AddMvc();

			var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
			var factory = new DataConnectionFactory();

			services.AddIdentityServer()
				.AddTemporarySigningCredential()
				.AddInMemoryUsers(Users.Get())
				.AddConfigurationStore(factory)
				.AddOperationalStore(factory);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime,
			ILoggerFactory loggerFactory)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.File(@"c:\logs\IdentityServer4.EntityFramework.Host.txt")
				.CreateLogger();

			loggerFactory.AddConsole();
			loggerFactory.AddDebug();
			loggerFactory.AddSerilog();

			//app.UseDeveloperExceptionPage();

			// Setup Databases
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				EnsureDatabase();
				EnsureSeedData(serviceScope.ServiceProvider.GetService<IDataConnectionFactory>());
			}

			app.UseIdentityServer();
			app.UseIdentityServerEfTokenCleanup(applicationLifetime);

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();
		}

		private void EnsureDatabase()
		{
			var connectionString = DataConnection.GetConnectionString(DataConnection.DefaultConfiguration);
			var csb = new SqlConnectionStringBuilder(connectionString);
			var dbName = csb.InitialCatalog;
			csb.InitialCatalog = "master";
			var masterConnectionString = csb.ConnectionString;

			using (var db = new DataConnection(SqlServerTools.GetDataProvider(SqlServerVersion.v2012), masterConnectionString))
			{
				try
				{
					db.Execute($"create database [{dbName}]");
				}
				catch
				{
					//
				}
			}

			using (var db = new DataConnection())
			{
				TryCreateTable<Client>(db);
				TryCreateTable<ClientClaim>(db);
				TryCreateTable<ClientCorsOrigin>(db);
				TryCreateTable<ClientGrantType>(db);
				TryCreateTable<ClientIdPRestriction>(db);
				TryCreateTable<ClientPostLogoutRedirectUri>(db);
				TryCreateTable<ClientRedirectUri>(db);
				TryCreateTable<ClientScope>(db);
				TryCreateTable<ClientSecret>(db);
				TryCreateTable<PersistedGrant>(db);
				TryCreateTable<Scope>(db);
				TryCreateTable<ScopeClaim>(db);
				TryCreateTable<ScopeSecret>(db);
			}

		}

		private static void TryCreateTable<T>(IDataContext db)
		{
			try
			{
				db.CreateTable<T>();
			}
			catch
			{
				//
			}
	    }


		private static void EnsureSeedData(IDataConnectionFactory context)
        {
	        using (var db = context.GetConnection())
	        {
		        if (!db.Clients().Any())
		        {
			        var clients = Clients.Get().Select(_ => _.ToEntity());
			        db.BulkCopy(clients);
		        }

		        if (!db.Scopes().Any())
		        {
			        var scopes = Scopes.Get().Select(_ => _.ToEntity());
			        db.BulkCopy(scopes);
		        }
	        }
        }
    }

	public class DataConnectionFactory : IDataConnectionFactory
	{
		public DataContext GetContext()
		{
			return new DataContext();
		}

		public DataConnection GetConnection()
		{
			return new DataConnection();
		}
	}
}