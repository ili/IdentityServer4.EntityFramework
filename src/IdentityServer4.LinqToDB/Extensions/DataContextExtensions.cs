using System;
using System.Linq;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.Models;
using ApiResource = IdentityServer4.LinqToDB.Entities.ApiResource;
using Client = IdentityServer4.LinqToDB.Entities.Client;
using IdentityResource = IdentityServer4.LinqToDB.Entities.IdentityResource;

// ReSharper disable once CheckNamespace
namespace LinqToDB.Data
{
	public static class DataContextExtensions
	{
		public static ITable<Client> Clients(this IDataContext db)
		{
			return db.GetTable<Client>();
		}

		public static ITable<ClientClaim> ClientClaims(this IDataContext db)
		{
			return db.GetTable<ClientClaim>();
		}

		public static ITable<ClientCorsOrigin> ClientCorsOrigins(this IDataContext db)
		{
			return db.GetTable<ClientCorsOrigin>();
		}

		public static ITable<ClientGrantType> ClientGrantTypes(this IDataContext db)
		{
			return db.GetTable<ClientGrantType>();
		}

		public static ITable<ClientIdentityProviderRestrictions> ClientIdPRestrictions(this IDataContext db)
		{
			return db.GetTable<ClientIdentityProviderRestrictions>();
		}

		public static ITable<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris(this IDataContext db)
		{
			return db.GetTable<ClientPostLogoutRedirectUri>();
		}

		public static ITable<ClientRedirectUri> ClientRedirectUris(this IDataContext db)
		{
			return db.GetTable<ClientRedirectUri>();
		}

		public static ITable<ClientScope> ClientScopes(this IDataContext db)
		{
			return db.GetTable<ClientScope>();
		}

		public static ITable<ClientSecret> ClientSecrets(this IDataContext db)
		{
			return db.GetTable<ClientSecret>();
		}

		public static ITable<ApiResource> ApiResources(this IDataContext db)
		{
			return db.GetTable<ApiResource>();
		}

		public static ITable<ApiResourceClaim> ApiResourceClaims(this IDataContext db)
		{
			return db.GetTable<ApiResourceClaim>();
		}

		public static ITable<ApiScope> ApiScopes(this IDataContext db)
		{
			return db.GetTable<ApiScope>();
		}

		public static ITable<ApiScopeClaim> ApiScopeClaims(this IDataContext db)
		{
			return db.GetTable<ApiScopeClaim>();
		}

		public static ITable<ApiSecret> ApiSecrets(this IDataContext db)
		{
			return db.GetTable<ApiSecret>();
		}

		public static ITable<IdentityClaim> IdentityClaims(this IDataContext db)
		{
			return db.GetTable<IdentityClaim>();
		}

		public static ITable<IdentityResource> s(this IDataContext db)
		{
			return db.GetTable<IdentityResource>();
		}

		public static ITable<PersistedGrant> PersistedGrants(this IDataContext db)
		{
			return db.GetTable<PersistedGrant>();
		}

		public static Client ComplexInsert(this DataConnection db, IdentityServer4.Models.Client client)
		{
			var res = MappingExtensions.GetSimpleMap<IdentityServer4.Models.Client, Client>(client);
			res.AllowedScopes = client.AllowedScopes;
			res.AllowedCorsOrigins = client.AllowedCorsOrigins;
			res.AllowedGrantTypes = client.AllowedGrantTypes;
			res.Claims = client.Claims;
			res.ClientSecrets = client.ClientSecrets;
			res.IdentityProviderRestrictions = client.IdentityProviderRestrictions;
			res.PostLogoutRedirectUris = client.PostLogoutRedirectUris;
			res.RedirectUris = client.RedirectUris;

			db.Insert(client);

			db.BulkCopy(client.AllowedCorsOrigins.Select(
				_ => new ClientCorsOrigin {ClientId = client.ClientId, Origin = _}));

			db.BulkCopy(client.AllowedGrantTypes.Select(
				_ => new ClientGrantType {ClientId = client.ClientId, GrantType = _}));

			db.BulkCopy(client.AllowedScopes.Select(
				_ => new ClientScope {ClientId = client.ClientId, Scope = _}));

			db.BulkCopy(client.Claims.Select(_ => new ClientClaim(_.Type, _.Value) {ClientId = client.ClientId}));

			db.BulkCopy(client.ClientSecrets.Select(_ => new ClientSecret
			{
				ClientId = client.ClientId,
				Description = _.Description,
				Expiration = _.Expiration,
				Type = _.Type,
				Value = _.Value
			}));

			db.BulkCopy(
				client.IdentityProviderRestrictions.Select(
					_ => new ClientIdentityProviderRestrictions {ClientId = client.ClientId, Provider = _}));

			db.BulkCopy(client.PostLogoutRedirectUris.Select(
				_ => new ClientPostLogoutRedirectUri {ClientId = client.ClientId, PostLogoutRedirectUri = _}));


			db.BulkCopy(client.RedirectUris.Select(
				_ => new ClientPostLogoutRedirectUri {ClientId = client.ClientId, PostLogoutRedirectUri = _}));


			return res;
		}

		public static ApiResource ComplexInsert(this DataConnection db, IdentityServer4.Models.ApiResource resource)
		{
			var res = MappingExtensions.GetSimpleMap<IdentityServer4.Models.ApiResource, ApiResource>(resource);
			res.ApiSecrets = resource.ApiSecrets;
			res.Scopes = resource.Scopes;
			res.UserClaims = resource.UserClaims;

			var id = Convert.ToInt32(db.InsertWithIdentity(res));

			db.BulkCopy(resource.ApiSecrets.Select(_ => new ApiSecret
			{
				ApiResourceId = id,
				Description = _.Description,
				Expiration = _.Expiration,
				Type = _.Type,
				Value = _.Value
			}));

			foreach (var s in resource.Scopes.Select(_ => new ApiScope
			{
				ApiResourceId = id,
				Description = _.Description,
				DisplayName = _.DisplayName,
				Emphasize = _.Emphasize,
				Name = _.Name,
				Required = _.Required,
				ShowInDiscoveryDocument = _.ShowInDiscoveryDocument,
				UserClaims = _.UserClaims
			}))
			{
				var scopeId = Convert.ToInt32(db.InsertWithIdentity(s));

				db.BulkCopy(s.UserClaims.Select(_ => new ApiScopeClaim {ApiScopeId = scopeId, Type = _}));
			}

			db.BulkCopy(resource.UserClaims.Select(_ => new ApiResourceClaim {ApiResourceId = id, Type = _}));

			return res;
		}

		public static IdentityResource ComplexInsert(this DataConnection db, IdentityServer4.Models.IdentityResource resource)
		{
			var res = MappingExtensions.GetSimpleMap<IdentityServer4.Models.IdentityResource, IdentityResource>(resource);
			res.UserClaims = resource.UserClaims;

			var id = Convert.ToInt32(db.InsertWithIdentity(resource));

			db.BulkCopy(resource.UserClaims.Select(_ => new IdentityClaim {IdentityResourceId = id, Type = _}));

			return res;
		}
	}
}