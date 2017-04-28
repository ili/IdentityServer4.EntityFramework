// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.LinqToDB.Stores;
using IdentityServer4.Models;
using LinqToDB;
using Xunit;
using IdentityModel;
using IdentityServer4.Stores;

namespace IdentityServer4.LinqToDB.IntegrationTests.Stores
{
	public class ResourceStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		private DatabaseProviderFixture _fixture;

		public ResourceStoreTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		private static IdentityResource CreateIdentityTestResource()
		{
			return new IdentityResource()
			{
				Name = Guid.NewGuid().ToString(),
				DisplayName = Guid.NewGuid().ToString(),
				Description = Guid.NewGuid().ToString(),
				ShowInDiscoveryDocument = true,
				UserClaims =
				{
					JwtClaimTypes.Subject,
					JwtClaimTypes.Name,
				}
			};
		}

		private static ApiResource CreateApiTestResource()
		{
			return new ApiResource()
			{
				Name = Guid.NewGuid().ToString(),
				ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
				Scopes =
					new List<Scope>
					{
						new Scope
						{
							Name = Guid.NewGuid().ToString(),
							UserClaims = {Guid.NewGuid().ToString()}
						}
					},
				UserClaims =
				{
					Guid.NewGuid().ToString(),
					Guid.NewGuid().ToString(),
				}
			};
		}

		[Fact]
		public void FindResourcesAsync_WhenResourcesExist_ExpectResourcesReturned()
		{
			var testIdentityResource = CreateIdentityTestResource();
			var testApiResource = CreateApiTestResource();
			using (var db = _fixture.Factory.GetConnection())
			{
				db.Insert(testIdentityResource);
				db.Insert(testApiResource);

				var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());

				var resources = store.FindResourcesByScopeAsync(new List<string>
				{
					testIdentityResource.Name,
					testApiResource.Scopes.First().Name
				}).Result;


				Assert.NotNull(resources);
				Assert.NotNull(resources.IdentityResources);
				Assert.NotEmpty(resources.IdentityResources);
				Assert.NotNull(resources.ApiResources);
				Assert.NotEmpty(resources.ApiResources);
				Assert.NotNull(resources.IdentityResources.FirstOrDefault(x => x.Name == testIdentityResource.Name));
				Assert.NotNull(resources.ApiResources.FirstOrDefault(x => x.Name == testApiResource.Name));
			}
		}

		[Fact]
		public void FindResourcesAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
		{
			var testIdentityResource = CreateIdentityTestResource();
			var testApiResource = CreateApiTestResource();
			using (var db = _fixture.Factory.GetConnection())
			{
				db.Insert(testIdentityResource);
				db.Insert(testApiResource);
				db.Insert(CreateIdentityTestResource());
				db.Insert(CreateApiTestResource());

				Resources resources;
				var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());
				resources = store.FindResourcesByScopeAsync(new List<string>
					{
						testIdentityResource.Name,
						testApiResource.Scopes.First().Name
					})
					.Result;

				Assert.NotNull(resources);
				Assert.NotNull(resources.IdentityResources);
				Assert.NotEmpty(resources.IdentityResources);
				Assert.NotNull(resources.ApiResources);
				Assert.NotEmpty(resources.ApiResources);
				Assert.Equal(1, resources.IdentityResources.Count);
				Assert.Equal(1, resources.ApiResources.Count);
			}
		}

		[Fact]
		public void GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
		{
			var visibleIdentityResource = CreateIdentityTestResource();
			var visibleApiResource = CreateApiTestResource();
			var hiddenIdentityResource = new IdentityResource { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false };
			var hiddenApiResource = new ApiResource
			{
				Name = Guid.NewGuid().ToString(),
				Scopes = new List<Scope> { new Scope { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false } }
			};

			using (var db = _fixture.Factory.GetConnection())
			{
				db.Insert(visibleIdentityResource);
				db.Insert(visibleApiResource);
				db.Insert(hiddenIdentityResource);
				db.Insert(hiddenApiResource);
			}

			var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());
				var resources = store.GetAllResources().Result;

			Assert.NotNull(resources);
			Assert.NotEmpty(resources.IdentityResources);
			Assert.NotEmpty(resources.ApiResources);

			Assert.True(resources.IdentityResources.Any(x => !x.ShowInDiscoveryDocument));
			Assert.True(resources.ApiResources.Any(x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument)));
		}

		[Fact]
		public void FindIdentityResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
		{
			var resource = CreateIdentityTestResource();

			_fixture.Factory.GetContext().Insert(resource);

			IList<IdentityResource> resources;
				var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());
				resources = store.FindIdentityResourcesByScopeAsync(new List<string>
				{
					resource.Name
				}).Result.ToList();

			Assert.NotNull(resources);
			Assert.NotEmpty(resources);
			var foundScope = resources.Single();

			Assert.Equal(resource.Name, foundScope.Name);
			Assert.NotNull(foundScope.UserClaims);
			Assert.NotEmpty(foundScope.UserClaims);
		}

		[Fact]
		public void FindIdentityResourcesByScopeAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
		{
			var resource = CreateIdentityTestResource();

			using (var db = _fixture.Factory.GetConnection())
			{
				db.Insert(resource);
				db.Insert(CreateIdentityTestResource());
			}

			IList<IdentityResource> resources;
				var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());
				resources = store.FindIdentityResourcesByScopeAsync(new List<string>
				{
					resource.Name
				}).Result.ToList();

			Assert.NotNull(resources);
			Assert.NotEmpty(resources);
			Assert.Equal(1, resources.Count);
		}

		[Fact]
		public void FindApiResourceAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
		{
			var resource = CreateApiTestResource();

			_fixture.Factory.GetContext().Insert(resource);

			ApiResource foundResource;
				var store = new ResourceStore(_fixture.Factory
					, FakeLogger<ResourceStore>.Create());
				foundResource = store.FindApiResourceAsync(resource.Name).Result;

			Assert.NotNull(foundResource);

			Assert.NotNull(foundResource.UserClaims);
			Assert.NotEmpty(foundResource.UserClaims);
			Assert.NotNull(foundResource.ApiSecrets);
			Assert.NotEmpty(foundResource.ApiSecrets);
			Assert.NotNull(foundResource.Scopes);
			Assert.NotEmpty(foundResource.Scopes);
			Assert.True(foundResource.Scopes.Any(x => x.UserClaims.Any()));
		}

		[Fact]
		public void FindApiResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
		{
			var resource = CreateApiTestResource();
			_fixture.Factory.GetContext().Insert(resource);

			IList<ApiResource> resources;
				var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());
				resources = store.FindApiResourcesByScopeAsync(new List<string> { resource.Scopes.First().Name }).Result.ToList();

			Assert.NotEmpty(resources);
			Assert.NotNull(resources);

			Assert.NotNull(resources.First().UserClaims);
			Assert.NotEmpty(resources.First().UserClaims);
			Assert.NotNull(resources.First().ApiSecrets);
			Assert.NotEmpty(resources.First().ApiSecrets);
			Assert.NotNull(resources.First().Scopes);
			Assert.NotEmpty(resources.First().Scopes);
			Assert.True(resources.First().Scopes.Any(x => x.UserClaims.Any()));
		}

		[Fact]
		public void FindApiResourcesByScopeAsync_WhenMultipleResourcesExist_ExpectOnlyRequestedResourcesReturned()
		{
			var resource = CreateApiTestResource();

			using (var db = _fixture.Factory.GetConnection())
			{
				db.Insert(resource);
				db.Insert(CreateApiTestResource());
				db.Insert(CreateApiTestResource());
			}

			IList<ApiResource> resources;
				var store = new ResourceStore(_fixture.Factory, FakeLogger<ResourceStore>.Create());
				resources = store.FindApiResourcesByScopeAsync(new List<string> { resource.Scopes.First().Name }).Result.ToList();

			Assert.NotNull(resources);
			Assert.NotEmpty(resources);
			Assert.Equal(1, resources.Count);
		}

	}
}