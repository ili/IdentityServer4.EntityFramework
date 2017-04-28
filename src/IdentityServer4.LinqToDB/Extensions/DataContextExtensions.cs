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

		public static ITable<ClientIdPRestriction> ClientIdPRestrictions(this IDataContext db)
		{
			return db.GetTable<ClientIdPRestriction>();
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

		public static ITable<PersistedGrant> PersistedGrants(this IDataContext db) => db.GetTable<PersistedGrant>();

		//public static ITable<> s(this IDataContext db) => db.GetTable<>();
	}
}