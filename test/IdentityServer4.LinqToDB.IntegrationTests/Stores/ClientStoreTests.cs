// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.LinqToDB.Stores;
using IdentityServer4.Models;
using LinqToDB;
using Xunit;

namespace IdentityServer4.LinqToDB.IntegrationTests.Stores
{
	public class ClientStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		private DatabaseProviderFixture _fixture;

		public ClientStoreTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public void FindClientByIdAsync_WhenClientExists_ExpectClientRetured()
		{
			var db = _fixture.Factory.GetContext();
			var testClient = new Entities.Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = "Test Client",
			};

			db.Insert(testClient);
			var agt = testClient.AllowedGrantTypes.First();

			db.Insert(new Entities.ClientGrantType(){ClientId = testClient.ClientId, GrantType = agt});

			var store = new ClientStore(_fixture.Factory, FakeLogger<ClientStore>.Create());
			var client = store.FindClientByIdAsync(testClient.ClientId).Result;

			Assert.NotNull(client);
		}
	}
}