// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.LinqToDB.Interfaces;
using IdentityServer4.LinqToDB.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LinqToDB;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.LinqToDB.Stores
{
	public class ScopeStore : IScopeStore
	{
		private readonly IDataConnectionFactory _dataConnectionFactory;
		private readonly ILogger<ScopeStore> _logger;

		public ScopeStore(IDataConnectionFactory dataConnectionFactory, ILogger<ScopeStore> logger)
		{
			if (dataConnectionFactory == null) throw new ArgumentNullException(nameof(dataConnectionFactory));

			_dataConnectionFactory = dataConnectionFactory;
			_logger = logger;
		}

		public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
		{
			var oldValue = global::LinqToDB.Common.Configuration.Linq.AllowMultipleQuery;

			try
			{
				global::LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

				var scopes = _dataConnectionFactory.GetContext()
					.GetTable<Entities.Scope>()
					.LoadWith(x => x.Claims)
					.LoadWith(x => x.ScopeSecrets)
					.Where(x => scopeNames.Contains(x.Name));

				var foundScopes = scopes.ToList();

				_logger.LogDebug("Found {scopes} scopes in database", foundScopes.Select(x => x.Name));

				var model = foundScopes.Select(x => x.ToModel());

				return Task.FromResult(model);
			}
			finally
			{
				global::LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = oldValue;
			}
		}

		public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
		{
			var oldValue = global::LinqToDB.Common.Configuration.Linq.AllowMultipleQuery;

			try
			{
				global::LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
				IQueryable<Entities.Scope> scopes = _dataConnectionFactory.GetContext()
					.GetTable<Entities.Scope>()
					.LoadWith(x => x.Claims)
					.LoadWith(x => x.ScopeSecrets);

				if (publicOnly)
					scopes = scopes.Where(x => x.ShowInDiscoveryDocument);

				var foundScopes = scopes.ToList();

				_logger.LogDebug("Found {scopes} scopes in database", foundScopes.Select(x => x.Name));

				var model = foundScopes.Select(x => x.ToModel());

				return Task.FromResult(model);
			}
			finally
			{
				global::LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = oldValue;
			}
		}
	}
}