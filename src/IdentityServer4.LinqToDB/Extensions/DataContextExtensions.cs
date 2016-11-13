using IdentityServer4.LinqToDB.Entities;

// ReSharper disable once CheckNamespace
namespace LinqToDB.Data
{
	public static class DataContextExtensions
	{
		public static ITable<Client> Clients(this IDataContext db) => db.GetTable<Client>();
		public static ITable<ClientClaim> ClientClaims(this IDataContext db) => db.GetTable<ClientClaim>();
		public static ITable<ClientCorsOrigin> ClientCorsOrigins(this IDataContext db) => db.GetTable<ClientCorsOrigin>();
		public static ITable<ClientGrantType> ClientGrantTypes(this IDataContext db) => db.GetTable<ClientGrantType>();
		public static ITable<ClientIdPRestriction> ClientIdPRestrictions(this IDataContext db) => db.GetTable<ClientIdPRestriction>();
		public static ITable<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris(this IDataContext db) => db.GetTable<ClientPostLogoutRedirectUri>();
		public static ITable<ClientRedirectUri> ClientRedirectUris(this IDataContext db) => db.GetTable<ClientRedirectUri>();
		public static ITable<ClientScope> ClientScopes(this IDataContext db) => db.GetTable<ClientScope>();
		public static ITable<ClientSecret> ClientSecrets(this IDataContext db) => db.GetTable<ClientSecret>();
		public static ITable<Scope> Scopes(this IDataContext db) => db.GetTable<Scope>();
		public static ITable<ScopeClaim> ScopeClaims(this IDataContext db) => db.GetTable<ScopeClaim>();
		public static ITable<ScopeSecret> ScopeSecrets(this IDataContext db) => db.GetTable<ScopeSecret>();
		public static ITable<PersistedGrant> PersistedGrants(this IDataContext db) => db.GetTable<PersistedGrant>();
	}
}