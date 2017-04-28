// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Stores;
using LinqToDB;
using LinqToDB.Data;
using Xunit;

namespace IdentityServer4.LinqToDB.IntegrationTests.DbContexts
{
	public class ClientDbContextTests : IClassFixture<DatabaseProviderFixture>
	{
		private DatabaseProviderFixture _fixture;

		public ClientDbContextTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public void CanAddAndDeleteClientScopes()
		{
			var db = _fixture.Factory.GetContext();

			var testClient = new Client
			{
				ClientId = "test-client-scopes",
				ClientName = "Test Client"
			};
			db.Insert(testClient);


			var client = db.Clients().LoadWith(x => x.AllowedScopes).First(x => x.ClientId == testClient.ClientId);

			db.Insert(new ClientScope
			{
				Scope = "test",
				ClientId = client.ClientId

			});

			client = db.Clients().LoadWith(x => x.AllowedScopes).First(x => x.ClientId == testClient.ClientId);
			var scope = client.AllowedScopes.First();

			db.Delete(scope);

			client = db.Clients().LoadWith(x => x.AllowedScopes).First(x => x.ClientId == testClient.ClientId);
			Assert.Empty(client.AllowedScopes);
		}

		[Fact]
		public void CanAddAndDeleteClientRedirectUri()
		{
			var db = _fixture.Factory.GetContext();

			var testClient = new Client
			{
				ClientId = "test-client",
				ClientName = "Test Client"
			};
			db.Insert(testClient);

			var store = new ClientStore(_fixture.Factory, new FakeLogger<ClientStore>());

			var client = store.FindClientByIdAsync(testClient.ClientId).Result;

			db.Insert(new ClientRedirectUri
			{
				RedirectUri = "https://redirect-uri-1",
				ClientId = testClient.ClientId
			});

			client = store.FindClientByIdAsync(testClient.ClientId).Result;
			var redirectUri = client.RedirectUris.First();

			db.ClientRedirectUris().Where(x => x.ClientId == testClient.ClientId).Delete();

			client = store.FindClientByIdAsync(testClient.ClientId).Result;

			Assert.Equal(0, client.RedirectUris.Count());
		}
	}
}