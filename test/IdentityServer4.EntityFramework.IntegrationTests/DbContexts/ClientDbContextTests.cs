// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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
		public readonly TheoryData<IDataConnectionFactory> TestDatabaseProviders = new TheoryData<IDataConnectionFactory>();

		public ClientDbContextTests(DatabaseProviderFixture fixture)
		{
			foreach (var context in fixture.Connections)
				TestDatabaseProviders.Add(context);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void CanAddAndDeleteClientScopes(IDataConnectionFactory factory)
		{
			var db = factory.GetContext();

			var id = (int) db.InsertWithIdentity(new Client
			{
				ClientId = "test-client-scopes",
				ClientName = "Test Client"
			});

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

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void CanAddAndDeleteClientRedirectUri(IDataConnectionFactory factory)
		{
			var db = factory.GetContext();

			var id = (int) db.InsertWithIdentity(new Client
			{
				ClientId = "test-client",
				ClientName = "Test Client"
			});

			var client = db.Clients().LoadWith(x => x.RedirectUris).First(x => x.Id == id);

			client.RedirectUris.Add(new ClientRedirectUri
			{
				RedirectUri = "https://redirect-uri-1"
			});

			client = db.Clients().LoadWith(x => x.RedirectUris).First(x => x.Id == id);
			var redirectUri = client.RedirectUris.First();

			client.RedirectUris.Remove(redirectUri);

			client = db.Clients().LoadWith(x => x.RedirectUris).First(x => x.Id == id);

			Assert.Equal(0, client.RedirectUris.Count());
		}
	}
}