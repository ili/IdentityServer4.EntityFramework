// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using LinqToDB;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
	public class ClientStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		public static readonly TheoryData<IDataConnectionFactory> TestDatabaseProviders =
			new TheoryData<IDataConnectionFactory>();

		public ClientStoreTests(DatabaseProviderFixture fixture)
		{
			foreach (var context in fixture.Connections)
				TestDatabaseProviders.Add(context);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void FindClientByIdAsync_WhenClientExists_ExpectClientRetured(IDataConnectionFactory options)
		{
			var db = options.GetContext();
			var testClient = new Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = "Test Client"
			};

			db.Insert(testClient.ToEntity());

			var store = new ClientStore(options, FakeLogger<ClientStore>.Create());
			var client = store.FindClientByIdAsync(testClient.ClientId).Result;

			Assert.NotNull(client);
		}
	}
}