// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using LinqToDB;
using LinqToDB.Data;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.DbContexts
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

			var id = Convert.ToInt32(db.InsertWithIdentity(new Client
			{
				ClientId = "test-client-scopes",
				ClientName = "Test Client"
			}));

			var client = db.Clients().LoadWith(x => x.AllowedScopes).First(x => x.Id == id);

			db.Insert(new ClientScope
			{
				Scope = "test",
				ClientId = client.Id

			});

			client = db.Clients().LoadWith(x => x.AllowedScopes).First(x => x.Id == id);
			var scope = client.AllowedScopes.First();

			db.Delete(scope);

			client = db.Clients().LoadWith(x => x.AllowedScopes).First(x => x.Id == id);
			Assert.Empty(client.AllowedScopes);
		}

		[Fact]
		public void CanAddAndDeleteClientRedirectUri()
		{
			var db = _fixture.Factory.GetContext();

			var id = Convert.ToInt32(db.InsertWithIdentity(new Client
			{
				ClientId = "test-client",
				ClientName = "Test Client"
			}));

			var client = db.Clients().LoadWith(x => x.RedirectUris).First(x => x.Id == id);

			db.Insert(new ClientRedirectUri
			{
				RedirectUri = "https://redirect-uri-1",
				ClientId = id
			});

			client = db.Clients().LoadWith(x => x.RedirectUris).First(x => x.Id == id);
			var redirectUri = client.RedirectUris.First();

			db.ClientRedirectUris().Where(_ => _.ClientId == id).Delete();

			client = db.Clients().LoadWith(x => x.RedirectUris).First(x => x.Id == id);

			Assert.Equal(0, client.RedirectUris.Count());
		}
	}
}