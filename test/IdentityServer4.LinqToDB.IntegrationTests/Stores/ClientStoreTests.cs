// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Stores;
using LinqToDB;
using Xunit;

namespace IdentityServer4.LinqToDB.IntegrationTests.Stores
{
	public class ClientStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		public ClientStoreTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		private readonly DatabaseProviderFixture _fixture;

		[Fact]
		public void FindClientByIdAsync_WhenClientExists_ExpectClientRetured()
		{
			var db = _fixture.Factory.GetContext();
			var testClient = new Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = "Test Client"
			};

			db.Insert(testClient);
			var agt = testClient.AllowedGrantTypes.First();

			db.Insert(new ClientGrantType {ClientId = testClient.ClientId, GrantType = agt});

			var store = new ClientStore(_fixture.Factory, FakeLogger<ClientStore>.Create());
			var client = store.FindClientByIdAsync(testClient.ClientId).Result;

			Assert.NotNull(client);
		}
	}
}