// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Services;
using LinqToDB;
using Xunit;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer4.LinqToDB.IntegrationTests.Services
{
	public class CorsPolicyServiceTests : IClassFixture<DatabaseProviderFixture>
	{
		private DatabaseProviderFixture _fixture;

		public CorsPolicyServiceTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public void IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue()
		{
			const string testCorsOrigin = "https://identityserver.io/";
			var db = _fixture.Factory.GetContext();

			var entity = new Entities.Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = Guid.NewGuid().ToString(),
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
			};

			db.Insert(entity);
			db.Insert(new ClientCorsOrigin {ClientId = entity.ClientId, Origin = entity.AllowedCorsOrigins.First()});

			var entity2 = new Entities.Client
			{
				ClientId = "2",
				ClientName = "2",
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com", testCorsOrigin}
			};

			db.Insert(entity2);
			db.Insert(new ClientCorsOrigin { ClientId = entity2.ClientId, Origin = entity2.AllowedCorsOrigins.First() });
			db.Insert(new ClientCorsOrigin { ClientId = entity2.ClientId, Origin = entity2.AllowedCorsOrigins.Skip(1).First() });

			var service = new CorsPolicyService(_fixture.Factory, FakeLogger<CorsPolicyService>.Create());
			var result = service.IsOriginAllowedAsync(testCorsOrigin).Result;

			Assert.True(result);
		}

		[Fact]
		public void IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
		{
			var db = _fixture.Factory.GetContext();

			db.Insert(new Entities.Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = Guid.NewGuid().ToString(),
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
			});

			var service = new CorsPolicyService(_fixture.Factory, FakeLogger<CorsPolicyService>.Create());
			var result = service.IsOriginAllowedAsync("InvalidOrigin").Result;

			Assert.False(result);
		}
	}
}