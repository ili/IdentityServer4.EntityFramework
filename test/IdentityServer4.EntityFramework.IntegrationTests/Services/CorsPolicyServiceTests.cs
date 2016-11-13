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
		public readonly TheoryData<IDataConnectionFactory> TestDatabaseProviders = new TheoryData<IDataConnectionFactory>();

		public CorsPolicyServiceTests(DatabaseProviderFixture fixture)
		{
			foreach (var context in fixture.Connections)
				TestDatabaseProviders.Add(context);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue(IDataConnectionFactory factory)
		{
			const string testCorsOrigin = "https://identityserver.io/";
			var db = factory.GetContext();

			db.Insert(new Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = Guid.NewGuid().ToString(),
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
			}.ToEntity());

			db.Insert(new Client
			{
				ClientId = "2",
				ClientName = "2",
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com", testCorsOrigin}
			}.ToEntity());


			var service = new CorsPolicyService(factory, FakeLogger<CorsPolicyService>.Create());
			var result = service.IsOriginAllowedAsync(testCorsOrigin).Result;

			Assert.True(result);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse(IDataConnectionFactory factory)
		{
			var db = factory.GetContext();

			db.Insert(new Client
			{
				ClientId = Guid.NewGuid().ToString(),
				ClientName = Guid.NewGuid().ToString(),
				AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
			}.ToEntity());

			var service = new CorsPolicyService(factory, FakeLogger<CorsPolicyService>.Create());
			var result = service.IsOriginAllowedAsync("InvalidOrigin").Result;

			Assert.False(result);
		}
	}
}