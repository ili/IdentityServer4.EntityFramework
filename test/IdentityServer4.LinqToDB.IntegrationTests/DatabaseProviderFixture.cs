// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.Sqlite;

namespace IdentityServer4.EntityFramework.IntegrationTests
{
    /// <summary>
    /// xUnit ClassFixture for creating and deleting integration test databases.
    /// </summary>
    public class DatabaseProviderFixture : IDisposable 
    {
		static string GetConnectionString() => $"Data Source=file:memdb{Counter++}?mode=memory&cache=shared";
	    private static int Counter;
	    private SqliteConnection _connection;

	    class InMemoryFactory : IDataConnectionFactory
	    {
		    private string _connectionString;

		    public InMemoryFactory(string connectionString)
		    {
			    _connectionString = connectionString;
		    }

			public DataContext GetContext()
		    {
			    return new DataContext(LinqToDB.DataProvider.SQLite.SQLiteTools.GetDataProvider(), _connectionString);
		    }

		    public DataConnection GetConnection()
		    {
				return new DataConnection(LinqToDB.DataProvider.SQLite.SQLiteTools.GetDataProvider(), _connectionString);
			}
		}

		
	    public IDataConnectionFactory Factory { get; private set; }
	    private readonly string _connectionString;


		public DatabaseProviderFixture()
		{
			_connectionString = GetConnectionString();
		    LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
			Factory = new InMemoryFactory(_connectionString);


			_connection = new SqliteConnection(_connectionString);
			_connection.Open();

			var db = new DataConnection(LinqToDB.DataProvider.SQLite.SQLiteTools.GetDataProvider(), _connectionString);
			db.CreateTable<Client>();
			db.CreateTable<ClientClaim>();
			db.CreateTable<ClientCorsOrigin>();
			db.CreateTable<ClientGrantType>();
			db.CreateTable<ClientIdPRestriction>();
			db.CreateTable<ClientPostLogoutRedirectUri>();
			db.CreateTable<ClientRedirectUri>();
			db.CreateTable<ClientScope>();
			db.CreateTable<ClientSecret>();
			db.CreateTable<PersistedGrant>();
			db.CreateTable<Scope>();
			db.CreateTable<ScopeClaim>();
			db.CreateTable<ScopeSecret>();
		}

		public void Dispose()
        {
			_connection.Dispose();
        }
    }
}