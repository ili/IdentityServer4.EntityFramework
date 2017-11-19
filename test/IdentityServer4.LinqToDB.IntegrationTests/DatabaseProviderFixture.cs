// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Contrib.LinqToDB.Entities;
using IdentityServer4.Contrib.LinqToDB.Interfaces;
using IdentityServer4.Models;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using ApiResource = IdentityServer4.Contrib.LinqToDB.Entities.ApiResource;
using Client = IdentityServer4.Contrib.LinqToDB.Entities.Client;
using IdentityResource = IdentityServer4.Contrib.LinqToDB.Entities.IdentityResource;

namespace IdentityServer4.LinqToDB.IntegrationTests
{
	/// <summary>
	///     xUnit ClassFixture for creating and deleting integration test databases.
	/// </summary>
	public class DatabaseProviderFixture : IDisposable
	{
		private static int Counter;
		private readonly SqliteConnection _connection;
		private readonly string _connectionString;

		static DatabaseProviderFixture()
		{
			MappingExtensions.ApplyDefaultEntitiesMappings();
		}


		public DatabaseProviderFixture()
		{
			_connectionString = GetConnectionString();
			Factory = new InMemoryFactory(_connectionString);


			_connection = new SqliteConnection(_connectionString);
			_connection.Open();

			var db = new DataConnection(SQLiteTools.GetDataProvider(), _connectionString);
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
			db.CreateTable<PersistedGrant>();
			db.CreateTable<IdentityClaim>();
			db.CreateTable<IdentityResource>();
		}


		public IDataConnectionFactory Factory { get; }

		public void Dispose()
		{
			_connection.Dispose();
		}

		private static string GetConnectionString()
		{
			return $"Data Source=file:memdb{Counter++}?mode=memory&cache=shared";
		}

		private class InMemoryFactory : IDataConnectionFactory
		{
			private readonly string _connectionString;

			public InMemoryFactory(string connectionString)
			{
				_connectionString = connectionString;
			}

			public DataContext GetContext()
			{
				return new DataContext(SQLiteTools.GetDataProvider(), _connectionString);
			}

			public DataConnection GetConnection()
			{
				return new DataConnection(SQLiteTools.GetDataProvider(), _connectionString);
			}
		}
	}
}