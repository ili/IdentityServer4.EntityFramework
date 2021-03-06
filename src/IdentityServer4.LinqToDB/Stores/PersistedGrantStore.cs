﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Contrib.LinqToDB.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LinqToDB;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Contrib.LinqToDB.Stores
{
	public class PersistedGrantStore : IPersistedGrantStore
	{
		private readonly IDataConnectionFactory _dataConnectionFactory;
		private readonly ILogger _logger;

		public PersistedGrantStore(IDataConnectionFactory dataConnectionFactory, ILogger<PersistedGrantStore> logger)
		{
			_dataConnectionFactory = dataConnectionFactory;
			_logger = logger;
		}

		public Task StoreAsync(PersistedGrant token)
		{
			try
			{
				_dataConnectionFactory.GetContext()
					.InsertOrReplace(token);
			}
			catch (Exception ex)
			{
				_logger.LogError(0, ex, "Exception storing persisted grant");
				throw;
			}

			return Task.FromResult(0);
		}

		public Task<PersistedGrant> GetAsync(string key)
		{
			var persistedGrant = _dataConnectionFactory.GetContext()
				.GetTable<PersistedGrant>()
				.FirstOrDefault(x => x.Key == key);

			_logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, persistedGrant != null);

			return Task.FromResult(persistedGrant);
		}

		public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
		{
			var persistedGrants = _dataConnectionFactory.GetContext()
				.GetTable<PersistedGrant>()
				.Where(x => x.SubjectId == subjectId);

			var model = persistedGrants.ToList();

			_logger.LogDebug("{persistedGrantCount} persisted grants found for {subjectId}", model.Count, subjectId);

			return Task.FromResult((IEnumerable<PersistedGrant>) model);
		}

		public Task RemoveAsync(string key)
		{
			var deleted = _dataConnectionFactory.GetContext()
				.GetTable<PersistedGrant>()
				.Where(x => x.Key == key)
				.Delete();

			_logger.LogDebug(
				deleted > 0
					? "removing {persistedGrantKey} persisted grant from database"
					: "no {persistedGrantKey} persisted grant found in database", key);

			return Task.FromResult(0);
		}

		public Task RemoveAllAsync(string subjectId, string clientId)
		{
			var deleted = _dataConnectionFactory.GetContext()
				.GetTable<PersistedGrant>()
				.Where(x => x.SubjectId == subjectId && x.ClientId == clientId)
				.Delete();

			_logger.LogDebug(
				"removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}",
				deleted, subjectId, clientId);

			return Task.FromResult(0);
		}

		public Task RemoveAllAsync(string subjectId, string clientId, string type)
		{
			var persistedGrants = _dataConnectionFactory.GetContext()
				.GetTable<PersistedGrant>()
				.Where(x =>
					x.SubjectId == subjectId &&
					x.ClientId == clientId &&
					x.Type == type)
				.Delete();

			_logger.LogDebug(
				"removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}, grantType {persistedGrantType}",
				persistedGrants, subjectId, clientId, type);

			return Task.FromResult(0);
		}
	}
}