// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using LinqToDB;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
	public class ScopeStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		private DatabaseProviderFixture _fixture;

		public ScopeStoreTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		private static Scope CreateTestObject()
		{
			return new Scope
			{
				Name = Guid.NewGuid().ToString(),
				Type = ScopeType.Identity,
				ShowInDiscoveryDocument = true,
				ScopeSecrets = new List<Secret> {new Secret("secret".Sha256())},
				Claims = new List<ScopeClaim>
				{
					new ScopeClaim("name"),
					new ScopeClaim("role")
				}
			};
		}

		[Fact]
		public void FindScopesAsync_WhenScopesExist_ExpectScopesReturned()
		{
			var firstTestScope = CreateTestObject();
			var secondTestScope = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(firstTestScope.ToEntity());
			db.Insert(secondTestScope.ToEntity());

			IList<Scope> scopes;
			var store = new ScopeStore(_fixture.Factory, FakeLogger<ScopeStore>.Create());
			scopes = store.FindScopesAsync(new List<string>
			{
				firstTestScope.Name,
				secondTestScope.Name
			}).Result.ToList();

			Assert.NotNull(scopes);
			Assert.NotEmpty(scopes);
			Assert.NotNull(scopes.FirstOrDefault(x => x.Name == firstTestScope.Name));
			Assert.NotNull(scopes.FirstOrDefault(x => x.Name == secondTestScope.Name));
		}

		[Fact]
		public void GetScopesAsync_WhenAllScopesRequested_ExpectAllScopes()
		{
			var db = _fixture.Factory.GetContext();

			db.Insert(CreateTestObject().ToEntity());
			db.Insert(new Entities.Scope {Name = "hidden_scope_return", ShowInDiscoveryDocument = false});

			IList<Scope> scopes;
			var store = new ScopeStore(_fixture.Factory, FakeLogger<ScopeStore>.Create());
			scopes = store.GetScopesAsync(false).Result.ToList();

			Assert.NotNull(scopes);
			Assert.NotEmpty(scopes);

			Assert.True(scopes.Any(x => !x.ShowInDiscoveryDocument));
		}

		[Fact]
		public void GetScopesAsync_WhenAllDiscoveryScopesRequested_ExpectAllDiscoveryScopes()
		{
			var db = _fixture.Factory.GetContext();

			db.Insert(CreateTestObject().ToEntity());
			db.Insert(new Entities.Scope {Name = "hidden_scope", ShowInDiscoveryDocument = false});

			IList<Scope> scopes;
			var store = new ScopeStore(_fixture.Factory, FakeLogger<ScopeStore>.Create());
			scopes = store.GetScopesAsync().Result.ToList();

			Assert.NotNull(scopes);
			Assert.NotEmpty(scopes);

			Assert.True(scopes.All(x => x.ShowInDiscoveryDocument));
		}

		[Fact]
		public void FindScopesAsync_WhenScopeHasSecrets_ExpectScopeAndSecretsReturned()
		{
			var scope = CreateTestObject().ToEntity();
			var db = _fixture.Factory.GetContext();

			var id = Convert.ToInt32(db.InsertWithIdentity(scope));
			var secret = scope.ScopeSecrets[0];
			secret.ScopeId = id;
			db.Insert(secret);

			IList<Scope> scopes;
			var store = new ScopeStore(_fixture.Factory, FakeLogger<ScopeStore>.Create());
			scopes = store.FindScopesAsync(new List<string>
			{
				scope.Name
			}).Result.ToList();

			Assert.NotNull(scopes);
			var foundScope = scopes.Single();

			Assert.NotNull(foundScope.ScopeSecrets);
			Assert.NotEmpty(foundScope.ScopeSecrets);
		}

		[Fact]
		public void FindScopesAsync_WhenScopeHasClaims_ExpectScopeAndClaimsReturned()
		{
			var scope = CreateTestObject().ToEntity();
			var db = _fixture.Factory.GetContext();

			var id = Convert.ToInt32(db.InsertWithIdentity(scope));
			var claim = scope.Claims.First();
			claim.ScopeId = id;
			db.Insert(claim);

			IList<Scope> scopes;
			var store = new ScopeStore(_fixture.Factory, FakeLogger<ScopeStore>.Create());
			scopes = store.FindScopesAsync(new List<string>
			{
				scope.Name
			}).Result.ToList();

			Assert.NotNull(scopes);
			var foundScope = scopes.Single();

			Assert.NotNull(foundScope.Claims);
			Assert.NotEmpty(foundScope.Claims);
		}
	}
}