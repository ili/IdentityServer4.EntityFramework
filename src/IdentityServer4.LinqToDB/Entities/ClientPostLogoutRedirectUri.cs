using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientPostLogoutRedirectUri
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string PostLogoutRedirectUri { get; set; }
		public string ClientId { get; set; }
	}
}