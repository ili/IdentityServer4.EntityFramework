// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using LinqToDB;
using Xunit;
using LinqToDB.Data;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
	public class PersistedGrantStoreTests : IClassFixture<DatabaseProviderFixture>
	{
		public static readonly TheoryData<IDataConnectionFactory> TestDatabaseProviders =
			new TheoryData<IDataConnectionFactory>();

		public PersistedGrantStoreTests(DatabaseProviderFixture fixture)
		{
			foreach (var context in fixture.Connections)
				TestDatabaseProviders.Add(context);
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

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void StoreAsync_WhenPersistedGrantStored_ExpectSuccess(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			store.StoreAsync(persistedGrant).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.NotNull(foundGrant);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			var foundPersistedGrant = store.GetAsync(persistedGrant.Key).Result;
			Assert.NotNull(foundPersistedGrant);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned(
			IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			var foundPersistedGrants = store.GetAllAsync(persistedGrant.SubjectId).Result.ToList();

			Assert.NotNull(foundPersistedGrants);
			Assert.NotEmpty(foundPersistedGrants);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			store.RemoveAsync(persistedGrant.Key).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void RemoveAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void RemoveAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void Store_should_create_new_record_if_key_does_not_exist(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.Null(foundGrant);

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			store.StoreAsync(persistedGrant).Wait();

			foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.NotNull(foundGrant);
		}

		[Theory, MemberData(nameof(TestDatabaseProviders))]
		public void Store_should_update_record_if_key_already_exists(IDataConnectionFactory factory)
		{
			var persistedGrant = CreateTestObject();
			var db = factory.GetContext();

			db.Insert(persistedGrant.ToEntity());

			var newDate = persistedGrant.Expiration.AddHours(1);

			var store = new PersistedGrantStore(factory, FakeLogger<PersistedGrantStore>.Create());
			persistedGrant.Expiration = newDate;
			store.StoreAsync(persistedGrant).Wait();

			var foundGrant = db.PersistedGrants().FirstOrDefault(x => x.Key == persistedGrant.Key);
			Assert.NotNull(foundGrant);
			Assert.Equal(newDate, persistedGrant.Expiration);
		}
	}
}