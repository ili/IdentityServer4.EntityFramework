// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Contrib.LinqToDB.Entities;
using IdentityServer4.Contrib.LinqToDB.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;
using ApiResource = IdentityServer4.Models.ApiResource;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer4.Contrib.LinqToDB.Stores
{
	public class ResourceStore : IResourceStore
	{
		private readonly IDataConnectionFactory _dataConnectionFactory;
		private readonly ILogger<ResourceStore> _logger;

		public ResourceStore(IDataConnectionFactory dataConnectionFactory, ILogger<ResourceStore> logger)
		{
			_dataConnectionFactory = dataConnectionFactory ?? throw new ArgumentNullException(nameof(dataConnectionFactory));
			_logger = logger;
		}


		public Task<ApiResource> FindApiResourceAsync(string name)
		{
			using (var db = _dataConnectionFactory.GetConnection())
			{
				var api = Upload(db.GetTable<Entities.ApiResource>().Where(_ => _.Name == name), db).FirstOrDefault();

				_logger.LogDebug(
					api != null ? "Found {name} API resource in database" : "Did not find {name} API resource in database", name);

				return Task.FromResult((ApiResource) api);
			}
		}

		public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
		{
			using (var db = _dataConnectionFactory.GetConnection())
			{
				var query = from a in db.GetTable<Entities.ApiResource>()
					join scope in db.GetTable<ApiScope>() on a.Id equals scope.ApiResourceId
					where scopeNames.Contains(scope.Name)
					select a;

				var apis = Upload(query, db);

				_logger.LogDebug("Found {scopes} API scopes in database", apis.SelectMany(x => x.Scopes).Select(x => x.Name));

				return Task.FromResult(apis.Cast<ApiResource>());
			}
		}

		public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
		{
			using (var db = _dataConnectionFactory.GetConnection())
			{
				//var scopes = scopeNames.ToArray();

				var query = db.GetTable<Entities.IdentityResource>()
					.Where(_ => scopeNames.Contains(_.Name));


				var resources = Upload(query, db);

				_logger.LogDebug("Found {scopes} identity scopes in database", resources.Select(x => x.Name));

				return Task.FromResult(resources.Cast<IdentityResource>());
			}
		}

		public Task<Resources> GetAllResources()
		{
			using (var db = _dataConnectionFactory.GetConnection())
			{
				var identityResources = Upload(db.GetTable<Entities.IdentityResource>(), db);
				var apiResources = Upload(db.GetTable<Entities.ApiResource>(), db);

				var result = new Resources(identityResources, apiResources);

				_logger.LogDebug("Found {scopes} as all scopes in database",
					result.IdentityResources.Select(x => x.Name)
						.Union(result.ApiResources.SelectMany(x => x.Scopes).Select(x => x.Name)));

				return Task.FromResult(result);
			}
		}

		private static List<Entities.IdentityResource> Upload(IQueryable<Entities.IdentityResource> query, DataConnection db)
		{
			var claims = (from ir in query
					join ic in db.GetTable<IdentityClaim>() on ir.Id equals ic.IdentityResourceId
					select ic).AsEnumerable()
				.GroupBy(_ => _.IdentityResourceId, _ => _.Type)
				.ToDictionary(_ => _.Key, _ => _.ToList());

			var resources = query.ToList();

			foreach (var resource in resources)
			{
				List<string> c;
				if (claims.TryGetValue(resource.Id, out c))
					resource.UserClaims = c;
			}
			return resources;
		}

		private static List<Entities.ApiResource> Upload(IQueryable<Entities.ApiResource> query, DataConnection db)
		{
			var apis = query.ToList();

			if (apis.Count == 0)
				return apis;

			var apiSecrets = (from a in query
					join secret in db.GetTable<ApiSecret>() on a.Id equals secret.ApiResourceId
					select secret)
				.AsEnumerable()
				.GroupBy(_ => _.ApiResourceId, _ => _)
				.ToDictionary(_ => _.Key, _ => _.Cast<Secret>().ToList());

			var scopesQuery = from a in query
				join scope in db.GetTable<ApiScope>() on a.Id equals scope.Id
				select scope;

			var apiScopes = scopesQuery
				.AsEnumerable()
				.GroupBy(_ => _.ApiResourceId, _ => _)
				.ToDictionary(_ => _.Key, _ => _.ToList());


			var scopeClaims = db.GetTable<ApiScopeClaim>()
				.Join(scopesQuery, _ => _.ApiScopeId, _ => _.Id, (c, a) => new {Claim = c, Api = a})
				.Select(_ => _.Claim)
				.AsEnumerable()
				.GroupBy(_ => _.ApiScopeId, _ => _.Type)
				.ToDictionary(_ => _.Key, _ => _.ToList());

			var apiClaims = (from a in query
					join claim in db.GetTable<ApiResourceClaim>() on a.Id equals claim.ApiResourceId
					select claim)
				.AsEnumerable()
				.GroupBy(_ => _.ApiResourceId, _ => _.Type)
				.ToDictionary(_ => _.Key, _ => _.ToList());

			foreach (var scope in apiScopes.Values.SelectMany(_ => _))
			{
				List<string> claims = null;
				if (scopeClaims.TryGetValue(scope.Id, out claims))
					scope.UserClaims = claims;
			}

			foreach (var apiResource in apis)
			{
				List<Secret> secrets;
				if (apiSecrets.TryGetValue(apiResource.Id, out secrets))
					apiResource.ApiSecrets = secrets;

				List<ApiScope> scopes;
				if (apiScopes.TryGetValue(apiResource.Id, out scopes))
					apiResource.Scopes = scopes.Cast<Scope>().ToList();

				List<string> claims;
				if (apiClaims.TryGetValue(apiResource.Id, out claims))
					apiResource.UserClaims = claims;
			}

			return apis;
		}
	}
}