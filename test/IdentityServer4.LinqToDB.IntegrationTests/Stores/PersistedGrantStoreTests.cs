// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.LinqToDB.Mappers;
using IdentityServer4.LinqToDB.Stores;
using IdentityServer4.Models;
using LinqToDB;
using LinqToDB.Data;
using Xunit;

namespace IdentityServer4.LinqToDB.IntegrationTests.Stores
{
	public class PersistedGrantStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		private DatabaseProviderFixture _fixture;

		public PersistedGrantStoreTests(DatabaseProviderFixture fixture)
		{
			_fixture = fixture;
		}

		private static PersistedGrant CreateTestObject()
		{
			return new PersistedGrant
			{
				Key = Guid.NewGuid().ToString(),
				Type = "authorization_code",
				ClientId = Guid.NewGuid().ToString(),
				SubjectId = Guid.NewGuid().ToString(),
				CreationTime = new DateTime(2016, 08, 01),
				Expiration = new DateTime(2016, 08, 31),
				Data = Guid.NewGuid().ToString()
			};
		}

		[Fact]
		public void StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			store.StoreAsync(persistedGrant).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.NotNull(foundGrant);
		}

		[Fact]
		public void GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			var foundPersistedGrant = store.GetAsync(persistedGrant.Key).Result;
			Assert.NotNull(foundPersistedGrant);
		}

		[Fact]
		public void GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			var foundPersistedGrants = store.GetAllAsync(persistedGrant.SubjectId).Result.ToList();

			Assert.NotNull(foundPersistedGrants);
			Assert.NotEmpty(foundPersistedGrants);
		}

		[Fact]
		public void RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			store.RemoveAsync(persistedGrant.Key).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);
		}

		[Fact]
		public void RemoveAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);
		}

		[Fact]
		public void RemoveAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);
		}

		[Fact]
		public void Store_should_create_new_record_if_key_does_not_exist()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			store.StoreAsync(persistedGrant).Wait();

			foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.NotNull(foundGrant);
		}

		[Fact]
		public void Store_should_update_record_if_key_already_exists()
		{
			var persistedGrant = CreateTestObject();
			var db = _fixture.Factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var newDate = persistedGrant.Expiration.AddHours(1);

			var store = new PersistedGrantStore(_fixture.Factory, FakeLogger<PersistedGrantStore>.Create());
			persistedGrant.Expiration = newDate;
			store.StoreAsync(persistedGrant).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.NotNull(foundGrant);
			Assert.Equal(newDate, persistedGrant.Expiration);
		}
	}
}