// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.Models;
using Xunit;
using IdentityServer4.EntityFramework.Interfaces;
using LinqToDB;

namespace IdentityServer4.EntityFramework.IntegrationTests.Services
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

			var entity = new Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = Guid.NewGuid().ToString(),
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
			}.ToEntity();

			db.Insert(entity);
			db.Insert(entity.AllowedCorsOrigins[0]);

			var entity2 = new Client
			{
				ClientId = "2",
				ClientName = "2",
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com", testCorsOrigin}
			}.ToEntity();

			db.Insert(entity2);
			db.Insert(entity2.AllowedCorsOrigins[0]);
			db.Insert(entity2.AllowedCorsOrigins[1]);

			var service = new CorsPolicyService(_fixture.Factory, FakeLogger<CorsPolicyService>.Create());
			var result = service.IsOriginAllowedAsync(testCorsOrigin).Result;

			Assert.True(result);
		}

		[Fact]
		public void IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
		{
			var db = _fixture.Factory.GetContext();

			db.Insert(new Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = Guid.NewGuid().ToString(),
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
			}.ToEntity());

			var service = new CorsPolicyService(_fixture.Factory, FakeLogger<CorsPolicyService>.Create());
			var result = service.IsOriginAllowedAsync("InvalidOrigin").Result;

			Assert.False(result);
		}
	}
}