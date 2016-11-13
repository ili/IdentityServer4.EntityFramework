﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
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
		public static readonly TheoryData<IDataConnectionFactory> TestDatabaseProviders =
			new TheoryData<IDataConnectionFactory>();

		public ScopeStoreTests(DatabaseProviderFixture fixture)
		{
			foreach (var context in fixture.Connections)
				TestDatabaseProviders.Add(context);
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

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void FindScopesAsync_WhenScopesExist_ExpectScopesReturned(IDataConnectionFactory factory)
		{
			var firstTestScope = CreateTestObject();
			var secondTestScope = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(firstTestScope.ToEntity());
			db.Insert(secondTestScope.ToEntity());

			IList<Scope> scopes;
			var store = new ScopeStore(factory, FakeLogger<ScopeStore>.Create());
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

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void GetScopesAsync_WhenAllScopesRequested_ExpectAllScopes(IDataConnectionFactory factory)
		{
			var db = factory.GetContext();

			db.Insert(CreateTestObject().ToEntity());
			db.Insert(new Entities.Scope {Name = "hidden_scope_return", ShowInDiscoveryDocument = false});

			IList<Scope> scopes;
			var store = new ScopeStore(factory, FakeLogger<ScopeStore>.Create());
			scopes = store.GetScopesAsync(false).Result.ToList();

			Assert.NotNull(scopes);
			Assert.NotEmpty(scopes);

			Assert.True(scopes.Any(x => !x.ShowInDiscoveryDocument));
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void GetScopesAsync_WhenAllDiscoveryScopesRequested_ExpectAllDiscoveryScopes(IDataConnectionFactory factory)
		{
			var db = factory.GetContext();

			db.Insert(CreateTestObject().ToEntity());
			db.Insert(new Entities.Scope {Name = "hidden_scope", ShowInDiscoveryDocument = false});

			IList<Scope> scopes;
			var store = new ScopeStore(factory, FakeLogger<ScopeStore>.Create());
			scopes = store.GetScopesAsync().Result.ToList();

			Assert.NotNull(scopes);
			Assert.NotEmpty(scopes);

			Assert.True(scopes.All(x => x.ShowInDiscoveryDocument));
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void FindScopesAsync_WhenScopeHasSecrets_ExpectScopeAndSecretsReturned(IDataConnectionFactory factory)
		{
			var scope = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(scope.ToEntity());

			IList<Scope> scopes;
			var store = new ScopeStore(factory, FakeLogger<ScopeStore>.Create());
			scopes = store.FindScopesAsync(new List<string>
			{
				scope.Name
			}).Result.ToList();

			Assert.NotNull(scopes);
			var foundScope = scopes.Single();

			Assert.NotNull(foundScope.ScopeSecrets);
			Assert.NotEmpty(foundScope.ScopeSecrets);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void FindScopesAsync_WhenScopeHasClaims_ExpectScopeAndClaimsReturned(IDataConnectionFactory factory)
		{
			var scope = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(scope.ToEntity());

			IList<Scope> scopes;
			var store = new ScopeStore(factory, FakeLogger<ScopeStore>.Create());
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